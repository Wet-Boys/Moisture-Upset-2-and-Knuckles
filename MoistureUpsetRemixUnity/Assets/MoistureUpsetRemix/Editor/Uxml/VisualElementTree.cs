using System;
using System.Collections.Generic;
using UnityEngine.UIElements;

namespace MoistureUpsetRemix.Editor.Uxml
{
    public class VisualElementTree : Dictionary<string, VisualElementTree>
    {
        public VisualElement VisualElement { get; private set; }

        public VisualElementTree(VisualElement visualElement)
        {
            VisualElement = visualElement;
        }

        public bool TryGetVisualElement(string hierarchyPath, out VisualElement visualElement)
        {
            visualElement = null;
            var parts = hierarchyPath.Split('/', StringSplitOptions.RemoveEmptyEntries);
            var tree = this;

            foreach (var part in parts)
            {
                if (tree is null)
                    return false;
                
                if (!tree.TryGetValue(part, out tree))
                    return false;
            }

            if (tree is null)
                return false;
            
            visualElement = tree.VisualElement;
            return true;
        }
    }
}