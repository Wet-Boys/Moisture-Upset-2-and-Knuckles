using System;
using System.Linq;
using System.Reflection;
using MoistureUpsetRemix.Editor.Uxml.Attributes;
using UnityEngine;
using UnityEngine.UIElements;

namespace MoistureUpsetRemix.Editor.Uxml
{
    public abstract class UxmlEditor : UnityEditor.Editor
    {
        [SerializeField]
        private VisualTreeAsset uxml;

        [SerializeField]
        private StyleSheet uss;
        
        protected VisualElement RootVisualElement { get; private set; }
        protected VisualElementTree Tree { get; private set; }

        public override VisualElement CreateInspectorGUI()
        {
            RootVisualElement = uxml.CloneTree();
            CreateBindings();
            OnUxmlCreated();
            
            return RootVisualElement;
        }

        private void CreateBindings()
        {
            Tree = new VisualElementTree(RootVisualElement);
            CacheHierarchyTree(Tree);

            var type = GetType();
            var fields = type.GetFields(BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance);
            foreach (var field in fields)
            {
                var hierarchyAttribute = field.GetCustomAttributes(typeof(BindVisualElementHierarchyAttribute), true)
                    .OfType<BindVisualElementHierarchyAttribute>()
                    .FirstOrDefault();

                if (hierarchyAttribute is null)
                    continue;

                var path = hierarchyAttribute.HierarchyPath;
                if (!Tree.TryGetVisualElement(path, out var visualElement))
                    continue;
                
                field.SetValue(this, visualElement);
            }
            
            var props = type.GetProperties(BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance);
            foreach (var prop in props)
            {
                var hierarchyAttribute = prop.GetCustomAttributes(typeof(BindVisualElementHierarchyAttribute), true)
                    .OfType<BindVisualElementHierarchyAttribute>()
                    .FirstOrDefault();

                if (hierarchyAttribute is null)
                    continue;

                var path = hierarchyAttribute.HierarchyPath;
                if (!Tree.TryGetVisualElement(path, out var visualElement))
                    continue;
                
                prop.SetValue(this, visualElement);
            }

            var registerCallBackMethod = typeof(CallbackEventHandler).GetMethods()
                .Where(method => method.Name == "RegisterCallback")
                .Where(method => method.GetParameters().Length == 2)
                .Where(method =>
                {
                    var parameters = method.GetParameters();
                    if (parameters[0].ParameterType.Name != typeof(EventCallback<>).Name)
                        return false;
                    return parameters[1].ParameterType.Name == nameof(TrickleDown);
                })
                .FirstOrDefault();
            
            var methods = type.GetMethods(BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance);
            foreach (var method in methods)
            {
                MethodValueCallbackBinding(method, registerCallBackMethod);
                MethodButtonBinding(method);
            }
        }

        private void MethodValueCallbackBinding(MethodInfo method, MethodInfo registerCallBackMethod)
        {
            if (registerCallBackMethod is null)
                return;
                
            var callbackAttribute = method.GetCustomAttributes(typeof(BindValueChangedCallbackAttribute), true)
                .OfType<BindValueChangedCallbackAttribute>()
                .FirstOrDefault();

            if (callbackAttribute is null)
                return;
                
            var path = callbackAttribute.HierarchyPath;
            if (!Tree.TryGetVisualElement(path, out var visualElement))
                return;
            
            if (visualElement is not CallbackEventHandler eventHandler)
                return;

            var parameters = method.GetParameters();
            if (parameters.Length != 1)
                return;
                
            var eventType = typeof(EventCallback<>).MakeGenericType(parameters[0].ParameterType);
            var typedRegisterMethod = registerCallBackMethod.MakeGenericMethod(parameters[0].ParameterType);
                
            typedRegisterMethod.Invoke(eventHandler, new object[] { Delegate.CreateDelegate(eventType, this, method), TrickleDown.NoTrickleDown });
        }

        private void MethodButtonBinding(MethodInfo method)
        {
            var callbackAttribute = method.GetCustomAttributes(typeof(BindButtonCallbackAttribute), true)
                .OfType<BindButtonCallbackAttribute>()
                .FirstOrDefault();
            
            if (callbackAttribute is null)
                return;
                
            var path = callbackAttribute.HierarchyPath;
            if (!Tree.TryGetVisualElement(path, out var visualElement))
                return;

            if (visualElement is not Button button)
                return;
            
            button.clicked += () => method.Invoke(this, Array.Empty<object>());
        }

        private void CacheHierarchyTree(VisualElementTree tree)
        {
            foreach (var child in tree.VisualElement.Children())
            {
                if (string.IsNullOrEmpty(child.name))
                    continue;
                
                tree[child.name] = new VisualElementTree(child);
                CacheHierarchyTree(tree[child.name]);
            }
        }

        protected abstract void OnUxmlCreated();
    }
}