using System;
using System.Linq;
using MoistureUpsetRemix.Common.Materials;
using UnityEditor;
using UnityEditor.UIElements;
using UnityEngine;
using UnityEngine.UIElements;
using PopupWindow = UnityEditor.PopupWindow;

namespace MoistureUpsetRemix.Editor.SkinDefMUR
{
    public class SmrOverrideItemElement : VisualElement
    {
        public readonly VisualTreeAsset _uxml = AssetDatabase.LoadAssetAtPath<VisualTreeAsset>("Assets/MoistureUpsetRemix/Editor/SkinDefMUR/Uxml/SmrOverrideItem.uxml");

        public event Action<SmrOverrideItem> ItemModified;

        private MeshPreview _originalMeshPreview;
        private Mesh _originalMesh;

        public SmrOverrideItemElement()
        {
            _uxml.CloneTree(this);
            
            RegisterCallback<AttachToPanelEvent>(OnAttach);
            RegisterCallback<DetachFromPanelEvent>(OnDetach);
        }

        private void OnAttach(AttachToPanelEvent panelEvent)
        {
            var meshPreviewContainer = this.Q<IMGUIContainer>("smr-mesh-preview");
            meshPreviewContainer.onGUIHandler = OriginalMeshPreview;
            
            var toggle = this.Q<Toggle>("mesh-override-toggle");
            var meshField = this.Q<ObjectField>("mesh-override");
            
            meshField.SetEnabled(toggle.value);
            toggle.RegisterValueChangedCallback(evt => meshField.SetEnabled(evt.newValue));
        }

        private void OnDetach(DetachFromPanelEvent evt)
        {
            if (_originalMeshPreview != null)
            {
                _originalMeshPreview.Dispose();
                _originalMeshPreview = null;
            }
        }

        public void BindData(SmrOverrideItem item)
        {
            _originalMesh = item.OriginalSmrMesh;
            
            var smrLabel = this.Q<Label>("smr-label");
            var toggle = this.Q<Toggle>("mesh-override-toggle");
            var meshField = this.Q<ObjectField>("mesh-override");
            var matOverrideField = this.Q<ObjectField>("material-override");
            var createMatOverrideButton = this.Q<Button>("create-material-override-button");

            smrLabel.text = item.SmrPath;
            
            toggle.value = item.EnableMeshOverride;
            toggle.RegisterValueChangedCallback(evt =>
            {
                item.EnableMeshOverride = evt.newValue;
                ItemModified?.Invoke(item);
            });

            meshField.value = item.MeshOverride;
            meshField.RegisterValueChangedCallback(evt =>
            {
                item.MeshOverride = (Mesh)evt.newValue;
                ItemModified?.Invoke(item);
            });
            
            matOverrideField.value = item.MaterialOverride;
            matOverrideField.RegisterValueChangedCallback(evt =>
            {
                item.MaterialOverride = (HGStandardOverride)evt.newValue;
                ItemModified?.Invoke(item);
            });
            
            var activatorRect = createMatOverrideButton.worldBound;
            var popUp = new MaterialOverrideSearchPopUpContent(activatorRect.size.x);

            popUp.MaterialSelected += matPath =>
            {
                var materialName = matPath.Split('/').Last().Replace(".mat", "");
                
                var instance = ScriptableObject.CreateInstance<HGStandardOverride>();
                var path = EditorUtility.SaveFilePanel("Save HGMaterialOverride", Application.dataPath , materialName, "asset")
                    .Replace(Application.dataPath, "Assets");

                AssetDatabase.CreateAsset(instance, path);
                
                AssetDatabase.Refresh();
                
                var loadedOverride = AssetDatabase.LoadAssetAtPath<HGStandardOverride>(path);
                loadedOverride.originalMaterialAddressablePath = matPath;
                
                EditorUtility.SetDirty(loadedOverride);
                AssetDatabase.SaveAssetIfDirty(loadedOverride);
                
                AssetDatabase.Refresh();
                
                matOverrideField.value = loadedOverride;
                item.MaterialOverride = loadedOverride;
                ItemModified?.Invoke(item);
            };

            createMatOverrideButton.clicked += () => PopupWindow.Show(activatorRect, popUp);
        }

        private void OriginalMeshPreview()
        {
            if (!_originalMesh)
                return;

            if (_originalMeshPreview == null)
            {
                _originalMeshPreview = new MeshPreview(_originalMesh);
            }
            else if (_originalMeshPreview.mesh != _originalMesh)
            {
                _originalMeshPreview.mesh = _originalMesh;
            }

            var rect = GUILayoutUtility.GetRect(1, 250);
            _originalMeshPreview.OnPreviewGUI(rect, null);
        }
    }
}