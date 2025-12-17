using System;

namespace MoistureUpsetRemix.Editor.Uxml.Attributes
{
    [AttributeUsage(AttributeTargets.Method, AllowMultiple = true)]
    public class BindButtonCallbackAttribute : Attribute
    {
        public string HierarchyPath { get; }
        
        public BindButtonCallbackAttribute(string hierarchyPath)
        {
            HierarchyPath = hierarchyPath;
        }
    }
}