using System.Collections.Generic;
using System.Linq;
using MoistureUpsetRemix.Editor.Utils;
using MoistureUpsetRemix.Editor.Uxml;
using MoistureUpsetRemix.Editor.Uxml.Attributes;
using MoistureUpsetRemix.Skins;
using UnityEditor;
using UnityEngine;
using UnityEngine.UIElements;
using PopupWindow = UnityEditor.PopupWindow;

namespace MoistureUpsetRemix.Editor.SkinDefMUR
{
    [CustomEditor(typeof(SkinDefMoistureUpsetRemix))]
    public class SkinDefMurEditor : UxmlEditor
    {
        private const string ModelSkinControllerAreaPath = "model-skin-controller-area";
        private const string SkinRemapDropDownPath = ModelSkinControllerAreaPath + "/msc-skin-remap-dropdown";
        private const string SelectModelSkinControllerButtonPath = ModelSkinControllerAreaPath + "/select-msc-button";
        private const string SmrOverrideAreaPath = "smr-override-area";
        private const string SmrOverrideListViewPath = SmrOverrideAreaPath + "/smr-override-list-view";
        
        private SkinDefMoistureUpsetRemix Target => target as SkinDefMoistureUpsetRemix;
        
        private ModelSkinControllerUtils.ModelSkinControllerInfo _info;

        private ModelSkinControllerUtils.ModelSkinControllerInfo Info
        {
            get
            {
                if (_info.Skins.Length == 0 && string.IsNullOrEmpty(_info.DisplayName))
                    _info = Target.msc.GetInfo();
                
                return _info;
            }
        }

        private readonly List<SmrOverrideItem> _smrOverrides = new();
        
        [BindVisualElementHierarchy(SkinRemapDropDownPath)]
        private DropdownField SkinRemapDropdownField { get; set; }
        
        [BindVisualElementHierarchy(SelectModelSkinControllerButtonPath)]
        private Button SelectModelSkinControllerButton { get; set; }
        
        [BindVisualElementHierarchy(SmrOverrideListViewPath)]
        private ListView SmrOverrideListView { get; set; }

        protected override void OnUxmlCreated()
        {
            if (Target.msc.IsValid())
            {
                _info = Target.msc.GetInfo();
                SetupSmrOverrideList();
            }

            UpdateSkinRemapDropdownChoices();
            SkinRemapDropdownField.RegisterValueChangedCallback(_ => MarkDirtyAndSave());
        }
        
        private void UpdateSkinRemapDropdownChoices()
        {
            SkinRemapDropdownField.choices.Clear();
            
            if (!Target.msc.IsValid())
                return;
            
            var choices = Info.Skins;
            SkinRemapDropdownField.choices.AddRange(choices);

            var value = SkinRemapDropdownField.value;
            if (!choices.Contains(value) || string.IsNullOrWhiteSpace(value))
                SkinRemapDropdownField.value = choices[0];
        }
        
        [BindButtonCallback(SelectModelSkinControllerButtonPath)]
        private void ShowSelectSkinControllerPopUp()
        {
            var activatorRect = SelectModelSkinControllerButton.worldBound;
            var popUp = new ModelSkinControllerSearchPopUpContent(activatorRect.size.x);

            popUp.ModelSkinControllerSelected += proxy =>
            {
                Undo.RecordObject(Target, "ModelSkinController Selected");
                Target.msc = proxy;
                _info = Target.msc.GetInfo();
                UpdateSkinRemapDropdownChoices();
                SetupSmrOverrideList();
                MarkDirtyAndSave();
            };
            
            PopupWindow.Show(activatorRect, popUp);
        }

        private void SetupSmrOverrideList()
        {
            _smrOverrides.Clear();

            var msc = Target.msc.GetModelSkinController();
            
            var smrArray = msc.GetComponentsInChildren<SkinnedMeshRenderer>(true);

            foreach (var skinnedMeshRenderer in smrArray)
            {
                var path = $"{Target.msc.mscPath}/{skinnedMeshRenderer.transform.GetPathToParent(msc.transform)}";
                
                var previewMesh = msc.GetOriginalMeshForSkin(Target.targetSkinName, path);
                if (!previewMesh)
                    previewMesh = skinnedMeshRenderer.sharedMesh;
                
                var item = new SmrOverrideItem
                {
                    SmrPath = path,
                    OriginalSmrMesh = previewMesh,
                    EnableMeshOverride = false,
                };
                
                var existingOverride = Target.smrOverrides.FirstOrDefault(smrOverride => smrOverride.smrPath == path);
                if (existingOverride.IsValid())
                {
                    item.EnableMeshOverride = existingOverride.enableMeshOverride;
                    item.MeshOverride = existingOverride.meshOverride;
                    item.MaterialOverride = existingOverride.materialOverride;
                }
                
                _smrOverrides.Add(item);
            }
            
            SmrOverrideListView.makeItem = () =>
            {
                var listItem = new SmrOverrideItemElement();
                
                listItem.ItemModified += OnSmrOverrideItemChanged;
                
                return listItem;
            };
            
            SmrOverrideListView.bindItem = (element, i) =>
            {
                if (element is not SmrOverrideItemElement listItem)
                    return;
                
                listItem.BindData(_smrOverrides[i]);
            };
            
            SmrOverrideListView.itemsSource = _smrOverrides;
            SmrOverrideListView.RefreshItems();
        }

        private void OnSmrOverrideItemChanged(SmrOverrideItem item)
        {
            var smrOverrideIndex = Target.smrOverrides.FindIndex(smrOverride => smrOverride.smrPath == item.SmrPath);
            
            var newSmrOverride = new SkinDefMoistureUpsetRemix.SkinnedMeshRendererOverride
            {
                smrPath = item.SmrPath,
                enableMeshOverride = item.EnableMeshOverride,
                meshOverride = item.MeshOverride,
                materialOverride = item.MaterialOverride,
            };

            if (smrOverrideIndex >= 0)
            {
                if (!item.IsModified())
                {
                    Target.smrOverrides.RemoveAt(smrOverrideIndex);
                }
                else
                {
                    Target.smrOverrides[smrOverrideIndex] = newSmrOverride;
                }
            }
            else
            {
                Target.smrOverrides.Add(newSmrOverride);
            }
            
            MarkDirtyAndSave();
        }

        private void MarkDirtyAndSave()
        {
            EditorUtility.SetDirty(Target);
            AssetDatabase.SaveAssetIfDirty(Target);
            serializedObject.Update();
            Repaint();
        }
    }
}