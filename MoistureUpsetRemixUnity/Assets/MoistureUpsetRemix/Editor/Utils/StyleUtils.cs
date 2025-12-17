using UnityEngine.UIElements;

namespace MoistureUpsetRemix.Editor.Utils
{
    internal static class StyleUtils
    {
        public static void SetBorderWidth(this IStyle style, StyleFloat width)
        {
            style.borderBottomWidth = width;
            style.borderLeftWidth = width;
            style.borderRightWidth = width;
            style.borderTopWidth = width;
        }
        
        public static void SetBorderColor(this IStyle style, StyleColor color)
        {
            style.borderBottomColor = color;
            style.borderLeftColor = color;
            style.borderRightColor = color;
            style.borderTopColor = color;
        }
    }
}