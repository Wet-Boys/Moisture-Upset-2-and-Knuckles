using System;
using System.Collections.Generic;
using System.Reflection;
using MoistureUpsetRemix.Common.Utils;
using MoistureUpsetRemix.Editor.Utils;
using MoistureUpsetRemix.Skins;
using RoR2;
using UnityEditor;
using UnityEngine;
using UnityEngine.AddressableAssets;

namespace MoistureUpsetRemix.Editor.SkinDefMUR.Preview
{
    [ExecuteAlways]
    public class SkinDefMurPreviewEditor : EditorWindow
    {
        [MenuItem("Window/Moisture Upset Remix/SkinDefMUR Preview")]
        private static void InitializeWindow()
        {
            var window = GetOrCreateWindow();
            window.Show();
        }

        public static SkinDefMurPreviewEditor GetOrCreateWindow()
        {
            var window = GetWindow<SkinDefMurPreviewEditor>("SkinDefMUR Preview", true);
            window.titleContent.tooltip = "SkinDefMUR Preview";
            window.autoRepaintOnSceneChange = true;
            
            return window;
        }

        private bool _hadTarget;

        private SkinDefMoistureUpsetRemix _target;

        public SkinDefMoistureUpsetRemix Target
        {
            get => _target;
            set
            {
                if (_hadTarget)
                    Cleanup();
                
                _target = value;
                if (!_target)
                    return;
                
                _hadTarget = true;
                CreatePrefabInPreview();
            }
        }

        private PreviewRenderUtility _previewRenderUtilityInstance;
        
        private PreviewRenderUtility PreviewRender
        {
            get
            {
                if (_previewRenderUtilityInstance == null)
                {
                    _previewRenderUtilityInstance = new PreviewRenderUtility(true);
                    GC.SuppressFinalize(_previewRenderUtilityInstance);
                    _cameraPos = 5f * -Vector3.forward;
                }
                
                _previewRenderUtilityInstance.camera.clearFlags = CameraClearFlags.Skybox;
                _previewRenderUtilityInstance.camera.fieldOfView = 30f;
                _previewRenderUtilityInstance.camera.nearClipPlane = 0.1f;
                _previewRenderUtilityInstance.camera.farClipPlane = 1000f;
                _previewRenderUtilityInstance.cameraFieldOfView = 30f;
                _previewRenderUtilityInstance.camera.transform.position = _cameraPos;
                _previewRenderUtilityInstance.camera.transform.LookAt(Vector3.zero);

                return _previewRenderUtilityInstance;
            }
        }

        private ManagedAddressableAsset<GameObject> _prefabAsset;
        private GameObject _prefabInstance;

        private Vector3 _cameraPos;

        private bool _leftMouseButtonHeld;
        private bool _rightMouseButtonHeld;

        private uint _particleLayers;

        private void CreatePrefabInPreview()
        {
            if (_prefabInstance)
                DestroyImmediate(_prefabInstance);

            if (_prefabAsset is not null)
            {
                _prefabAsset.Dispose();
                _prefabAsset = null;
            }
            
            _prefabAsset = Target.msc.GetPrefab();
            if (!_prefabAsset.Asset)
            {
                Debug.LogError("Failed to get prefab of skin!");
                return;
            }

            _prefabInstance = (GameObject)Instantiate(_prefabAsset);
            _prefabInstance.transform.position = Vector3.zero;
            _prefabInstance.hideFlags = HideFlags.HideAndDontSave;
            PreviewRender.AddSingleGO(_prefabInstance);
            
            typeof(CharacterModel).GetMethod("InitSharedMaterialsArrays", BindingFlags.NonPublic | BindingFlags.Static)
                ?.Invoke(null, Array.Empty<object>());

            var msc = _prefabInstance.GetComponentInChildren<ModelSkinController>();
            var cm = _prefabInstance.GetComponentInChildren<CharacterModel>();

            var skinDef = Instantiate(Target.AsSkinDef());
            skinDef.skinDefParams = Instantiate(skinDef.skinDefParams);
            
            for (var i = 0; i < skinDef.skinDefParams.rendererInfos.Length; i++)
            {
                var rendererInfo = skinDef.skinDefParams.rendererInfos[i];
                
                rendererInfo.defaultMaterial = MaterialUtils.TrySwapShader(rendererInfo.defaultMaterial);
                rendererInfo.defaultMaterial.CloneTextures();
                
                skinDef.skinDefParams.rendererInfos[i] = rendererInfo;
            }

            msc.skins = new[] { skinDef };

            msc.InvokeAwake();
            msc.InvokeStart();
            msc.InvokeUpdate();
            
            cm.InvokeAwake();
            cm.InvokeStart();
            cm.InvokeUpdate();

            cm.baseRendererInfos = skinDef.skinDefParams.rendererInfos;
            
            var enumerator = skinDef.ApplyAsync(msc.gameObject, new List<AssetReferenceT<Material>>(), new List<AssetReferenceT<Mesh>>());
            do
            {
                // Nothing
            }
            while (enumerator.MoveNext());
            
            cm.visibility = VisibilityLevel.Visible;
            cm.InvokeMethod("UpdateMaterials");

            var renderers = _prefabInstance.GetComponentsInChildren<Renderer>();
            foreach (var renderer in renderers)
            {
                if (renderer.sharedMaterial)
                    renderer.material = MaterialUtils.TrySwapShader(renderer.sharedMaterial);

                var mats = new List<Material>();
                foreach (var sharedMaterial in renderer.sharedMaterials)
                {
                    var mat = MaterialUtils.TrySwapShader(sharedMaterial);
                    mat.CloneTextures();
                    mats.Add(mat);
                }
                renderer.materials = mats.ToArray();
            }
        }

        private void Update()
        {
            Repaint();
        }

        private void OnBecameVisible()
        {
            _particleLayers = ParticleSystemUtils.PreviewLayer;
            ParticleSystemUtils.PreviewLayer = (uint)ParticleSystemUtils.PreviewLayerType.Everything;
        }

        private void OnBecameInvisible()
        {
            ParticleSystemUtils.PreviewLayer = _particleLayers;
        }

        private void HandleMouseInput()
        {
            if (!Target)
                return;

            if (Event.current is null)
                return;

            if (Event.current.type is EventType.MouseDown or EventType.MouseUp)
            {
                if (Event.current.button == 0)
                {
                    _leftMouseButtonHeld = Event.current.type switch
                    {
                        EventType.MouseDown => true,
                        EventType.MouseUp => false,
                        _ => _leftMouseButtonHeld
                    };
                }
            }

            var delta = Event.current.delta;

            if (Event.current.type == EventType.ScrollWheel)
            {
                _cameraPos.z -= delta.y * 0.1f;
            }
            else if (_leftMouseButtonHeld && _prefabInstance)
            {
                _prefabInstance.transform.Rotate(-Vector3.up, delta.x * 0.1f, Space.World);
                _prefabInstance.transform.Rotate(-Vector3.right, delta.y * 0.1f, Space.World);
            }
        }

        private void OnGUI()
        {
            HandleMouseInput();
            
            if (!Target)
            {
                if (_hadTarget)
                {
                    Cleanup();
                    _hadTarget = false;
                }
                
                return;
            }
            
            PreviewRender.BeginPreview(new Rect(0, 0, 512, 512), GUIStyle.none);
            PreviewRender.camera.Render();

            var texture = PreviewRender.EndPreview();
            
            GUI.DrawTexture(new Rect(0, 0, 512, 512), texture);
        }

        private void CreateGUI()
        {
            
        }

        private void OnEnable()
        {
            Cleanup();
        }

        private void OnDisable()
        {
            Cleanup();
        }

        private void Cleanup()
        {
            _previewRenderUtilityInstance?.Cleanup();
            _previewRenderUtilityInstance = null;
            _hadTarget = false;
            
            _prefabAsset?.Dispose();
            _prefabAsset = null;

            // Resources.UnloadUnusedAssets();
        }
    }
}