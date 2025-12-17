using UnityEngine;

namespace MoistureUpsetRemix.Editor.Utils
{
    internal static class TransformUtils
    {
        public static string GetPathToParent(this Transform transform, Transform targetParent)
        {
            if (transform == targetParent || !transform || !targetParent)
                return "";

            var path = transform.name;
            
            var current = transform.parent;
            while (current != targetParent && current)
            {
                path = $"{current.name}/{path}";
                current = current.parent;
            }

            return path;
        }

        public static Transform GetRoot(this Transform transform)
        {
            var current = transform;
            while (current.parent)
                current = current.parent;
            
            return current;
        }

        public static string GetPathToRoot(this Transform transform)
        {
            var root = transform.GetRoot();
            return transform.GetPathToParent(root);
        }
    }
}