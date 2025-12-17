using UnityEngine.UIElements;

namespace MoistureUpsetRemix.Editor.Utils
{
    internal static class UxmlExtensions
    {
        public static T QueryInChildren<T>(this VisualElement root, string name)
            where T : VisualElement
        {
            var result = root.Q<T>(name);
            if (result is not null)
                return result;

            foreach (var child in root.Children())
                child.QueryInChildren<T>(name);

            return null;
        }
    }
}