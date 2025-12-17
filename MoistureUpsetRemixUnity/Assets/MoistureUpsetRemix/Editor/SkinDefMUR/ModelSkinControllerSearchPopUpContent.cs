using System;
using System.Collections.Generic;
using System.Linq;
using MoistureUpsetRemix.Skins;
using UnityEditor;
using UnityEditor.UIElements;
using UnityEngine;
using UnityEngine.UIElements;

namespace MoistureUpsetRemix.Editor.SkinDefMUR
{
    public class ModelSkinControllerSearchPopUpContent : PopupWindowContent
    {
        private readonly float _width;
        private readonly List<ModelSkinControllerUtils.ModelSkinControllerInfo> _results = new();

        public event Action<ModelSkinControllerProxy> ModelSkinControllerSelected;

        public ModelSkinControllerSearchPopUpContent(float width)
        {
            _width = width;
        }
        
        public override Vector2 GetWindowSize()
        {
            return new Vector2(_width, 400);
        }

        public override void OnGUI(Rect rect)
        {
            
        }

        public override void OnOpen()
        {
            _results.Clear();
            
            var root = editorWindow.rootVisualElement;
            root.Add(Search());
        }

        public override void OnClose()
        {
            _results.Clear();
        }

        private VisualElement Search()
        {
            var root = new VisualElement();
            
            var searchField = new ToolbarSearchField
            {
                value = "",
            };
            root.Add(searchField);
            
            var resultsListView = new ListView(_results, 35f, ResultsListViewMakeItem, ResultsListViewBindItem)
            {
                selectionType = SelectionType.Single,
            };
            resultsListView.onItemsChosen += ResultsListViewOnItemChosen;
            
            searchField.RegisterValueChangedCallback(evt => UpdateSearchResults(resultsListView, evt.newValue));
            root.Add(resultsListView);

            return root;
        }

        private void ResultsListViewOnItemChosen(IEnumerable<object> items)
        {
            var item = items.FirstOrDefault();

            if (item is not ModelSkinControllerUtils.ModelSkinControllerInfo info)
                return;
            
            ModelSkinControllerSelected?.Invoke(info.Proxy);
            editorWindow.Close();
        }

        private VisualElement ResultsListViewMakeItem()
        {
            var root = new Box
            {
                style =
                {
                    borderTopWidth = 1f,
                    borderTopColor = new Color(.1f, .1f, .1f),
                },
            };

            var nameRow = NewRow("display-name-row");
            var displayNameLabel = new Label("Name: ");
            nameRow.Add(displayNameLabel);
            var displayName = new Label
            {
                name = "display-name"
            };
            nameRow.Add(displayName);
            
            var prefabRow = NewRow("prefab-path-row");
            var prefabPatchLabel = new Label("Prefab: ");
            prefabRow.Add(prefabPatchLabel);
            var prefabPath = new Label
            {
                name = "prefab-path"
            };
            prefabRow.Add(prefabPath);
            
            root.Add(nameRow);
            root.Add(prefabRow);
            
            return root;

            VisualElement NewRow(string name)
            {
                var row = new VisualElement
                {
                    name = name,
                    style =
                    {
                        flexDirection = new StyleEnum<FlexDirection>(FlexDirection.Row)
                    },
                };

                return row;
            }
        }

        private void ResultsListViewBindItem(VisualElement itemElement, int i)
        {
            var info = _results[i];
            var nameLabel = itemElement.Q<VisualElement>("display-name-row").Q<Label>("display-name");
            var prefabLabel = itemElement.Q<VisualElement>("prefab-path-row").Q<Label>("prefab-path");
            
            nameLabel.text = info.DisplayName;
            prefabLabel.text = info.Proxy.prefabAddressablePath;
        }

        private void UpdateSearchResults(ListView resultsListView, string searchText)
        {
            var infos = ModelSkinControllerUtils.GetInfoArray();
            var possibleInfos = infos.Where(info => info.DisplayName.ToLowerInvariant().Contains(searchText.ToLowerInvariant()));
            
            _results.Clear();
            _results.AddRange(possibleInfos);
            
            resultsListView.RefreshItems();
        }
    }
}