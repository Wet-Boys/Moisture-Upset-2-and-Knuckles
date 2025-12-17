using System;
using System.Collections.Generic;
using System.Linq;
using MoistureUpsetRemix.Editor.Utils;
using UnityEditor;
using UnityEditor.UIElements;
using UnityEngine;
using UnityEngine.UIElements;

namespace MoistureUpsetRemix.Editor.SkinDefMUR
{
    public class MaterialOverrideSearchPopUpContent : PopupWindowContent
    {
        private readonly float _width;
        private readonly List<string> _results = new();

        public event Action<string> MaterialSelected;

        public MaterialOverrideSearchPopUpContent(float width)
        {
            _width = width;
        }

        public override Vector2 GetWindowSize()
        {
            return new Vector2(500, 400);
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

            var resultsListView = new ListView(_results, 25f, ResultsListViewMakeItem, ResultsListViewBindItem)
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

            if (item is not string matPath)
                return;

            MaterialSelected?.Invoke(matPath);
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

            var prefabRow = NewRow("prefab-path-row");
            var prefabPatchLabel = new Label("Prefab: ");
            prefabRow.Add(prefabPatchLabel);
            var prefabPath = new Label
            {
                name = "prefab-path"
            };
            prefabRow.Add(prefabPath);

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
            var prefabLabel = itemElement.Q<VisualElement>("prefab-path-row").Q<Label>("prefab-path");

            prefabLabel.text = info;
        }

        private void UpdateSearchResults(ListView resultsListView, string searchText)
        {
            var possibleResults = AddressableUtils.Locations.Where(l => l.IsMaterialAsset())
                .Select(l => l.PrimaryKey)
                .Where(path => path.Contains(searchText, StringComparison.InvariantCultureIgnoreCase));

            _results.Clear();
            _results.AddRange(possibleResults);

            resultsListView.RefreshItems();
        }
    }
}