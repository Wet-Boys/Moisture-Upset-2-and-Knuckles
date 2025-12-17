using System;

namespace MoistureUpsetRemix.Editor.Uxml.Attributes
{
    [AttributeUsage(AttributeTargets.Field | AttributeTargets.Property)]
    public class BindVisualElementHierarchyAttribute : Attribute
    {
        public string HierarchyPath { get; }
        
        public BindVisualElementHierarchyAttribute(string hierarchyPath)
        {
            HierarchyPath = hierarchyPath;
        }
    }
}