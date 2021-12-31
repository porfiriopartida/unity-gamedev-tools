using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using UnityEditor;
using UnityEngine;

namespace Editor.PorfirioPartida.SearchEverywhere
{
    public class MenuItemsScanner : IActionScanner
    {
        private HashSet<string> _actions;
        private void SearchMenuItems()
        {
            _actions = new HashSet<string>();
            var editorAssembly = Assembly.GetAssembly(typeof(EditorWindow));
            var defaultAssembly = Assembly.GetExecutingAssembly();
            //Library\ScriptAssemblies
            Debug.Log($"Loading Editor Assembly: {editorAssembly.Location}");
            Debug.Log($"Loading Default assembly: {defaultAssembly.Location}");
            SearchMenuItemsByAssembly(editorAssembly);
            SearchMenuItemsByAssembly(defaultAssembly);
        }

        private void SearchMenuItemsByAssembly(Assembly assembly)
        {
            var types = assembly.GetTypes();
            foreach (var type in types)
            {
                var menuItems = MenuItemsScanner.GetClassMenuItems(type);
                if (menuItems == null || menuItems.Count == 0)
                {
                    continue;
                }

                foreach (var menuItem in menuItems)
                {
                    _actions.Add(menuItem.menuItem);
                }
            }
        }
        private static List<MenuItem> GetClassMenuItems(Type t)
        {
            return GetMethodsMenuItems(t);
        }

        private static List<MenuItem> GetMethodsMenuItems(Type t)
        {
            var methodsInfo = t.GetMethods(BindingFlags.NonPublic | BindingFlags.Public | BindingFlags.Static);
            return methodsInfo.Select(GetMenuItem).Where(menuItem => menuItem != null).ToList();
        }

        private static MenuItem GetMenuItem(MemberInfo memberInfo)
        {
            return (MenuItem)Attribute.GetCustomAttribute(memberInfo, typeof (MenuItem));
        }

        public void Scan()
        {
            SearchMenuItems();
        }

        public HashSet<string> GetActions()
        {
            return _actions;
        }
    }
}