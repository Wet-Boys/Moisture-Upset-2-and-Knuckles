using System;

namespace MoistureUpsetRemix.Editor.Uxml.Attributes
{
    [AttributeUsage(AttributeTargets.Method)]
    public class BindValueChangedCallbackAttribute : Attribute
    {
        public string HierarchyPath { get; }
        
        public BindValueChangedCallbackAttribute(string hierarchyPath)
        {
            HierarchyPath = hierarchyPath;
        }
    }
}