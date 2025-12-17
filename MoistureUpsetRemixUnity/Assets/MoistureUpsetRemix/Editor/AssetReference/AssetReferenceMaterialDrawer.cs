using MoistureUpsetRemix.Common.AssetManagement;
using UnityEditor;
using UnityEngine;

namespace MoistureUpsetRemix.Editor.AssetReference
{
    [CustomPropertyDrawer(typeof(AssetReferenceMaterial))]
    public class AssetReferenceMaterialDrawer : PropertyDrawer
    {
        public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
        {
            EditorGUI.BeginProperty(position, label, property);
            
            position = EditorGUI.PrefixLabel(position, GUIUtility.GetControlID(FocusType.Passive), label);
            
            
        }
        
        
    }
}