using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using UnityEditor;
using UnityEngine;
/*
 * TODO: Support Custom Assembly. Most likely inside ListActions
 * TODO: Support / and \ for file lookups.
 * TODO: Support refresh OnEnable/OnAwake
 */
namespace Editor.PorfirioPartida.SearchEverywhere
{
    public class SearchEverywhereEditor : EditorWindow
    {
        private GUIStyle _buttonStyle;
        private static HashSet<string> _filesResult;
        private static HashSet<string> _actions;
        private static string _searchString = "";
        
        [MenuItem("Tools/SearchEverywhere #&f")] // Alt + Shift + F
        public static void SearchEverywhere()
        {
            var window = GetWindow<SearchEverywhereEditor>(true, "Search Everywhere", true);
            // window.titleContent = new GUIContent("Search Everywhere");
            window.Init();
            window.ShowUtility();
        }

        private void Init()
        {
            _buttonStyle = new GUIStyle
            {
                alignment = TextAnchor.MiddleLeft,
                padding = new RectOffset(5, 0, 1, 1),
                normal = new GUIStyleState() { textColor = Color.white },
                hover = new GUIStyleState() { textColor = Color.gray },
                active = new GUIStyleState() { textColor = Color.gray },
            };
        }

        private void OnEnable()
        {
            ListActions();
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

        private void ListActions()
        {
            _actions = new HashSet<string>();
            var editorAssembly = Assembly.GetAssembly(typeof(EditorWindow));
            var defaultAssembly = Assembly.GetExecutingAssembly();
            listActionByAssembly(editorAssembly);
            listActionByAssembly(defaultAssembly);
        }

        private static void listActionByAssembly(Assembly assembly)
        {
            var types = assembly.GetTypes();
            foreach (var type in types)
            {
                var menuItems = GetClassMenuItems(type);
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

        private void OnGUI()
        {   
            EditorGUILayout.Space(5f, true);
            RenderSearchText();
            if (_searchString.Length <= 0)
            {
                return;
            }

            if (_filesResult != null)
            {
                var filesResult = new HashSet<string>(_filesResult);
                RenderPrefabs(filesResult);
                RenderScripts(filesResult);
                RenderMisc(filesResult);
                RenderActions();
            }

            if (!GUI.changed)
            {
                return;
            }

            PerformSearch();
        }

        private void RenderActions()
        {
            RenderAction(_actions, "Action");
        }

        private void RenderMisc(HashSet<string> filesResult)
        {
            RenderAsset(filesResult, "File");
        }

        private void RenderScripts(HashSet<string> filesResult)
        {
            RenderAsset(filesResult, "Script", ".cs");
        }

        private void RenderPrefabs(HashSet<string> filesResult)
        {
            RenderAsset(filesResult, "Prefab", ".prefab");
        }

        private void RenderAction(HashSet<string> results, string prefix)
        {
            if (results == null || results.Count == 0){
                return;
            }

            foreach (var r in results)
            {
                if (!r.ToUpper().Contains(_searchString.ToUpper()))
                {
                    continue;
                }

                var b = GUILayout.Button($"{prefix} | {r}", _buttonStyle);
                if (!b) continue;
                
                ClickAction(r);
            }
        }

        private void ClickAction(string s)
        {
            EditorApplication.ExecuteMenuItem(s);
        }

        private void RenderAsset(HashSet<string> results, string prefix, string format = "")
        {
            if (results == null || results.Count == 0){
                return;
            }

            var toBeRemoved = new HashSet<string>();
            foreach (var r in results)
            {
                if (!string.IsNullOrEmpty(format) && !r.EndsWith(format))
                {
                    continue;
                }

                var b = GUILayout.Button($"{prefix} | {r}", _buttonStyle);
                if (b)
                {
                    // Debug.Log($"Pressed button {r}");
                    ClickResource(r);
                }

                toBeRemoved.Add(r);
            }

            foreach (var tbr in toBeRemoved)
            {
                results.Remove(tbr);
            }
        }

        private void ClickResource(string s)
        {
            var obj = AssetDatabase.LoadAssetAtPath(s, typeof(UnityEngine.Object));
            EditorGUIUtility.PingObject(obj);
        }


        private void PerformSearch()
        {
            _filesResult = new HashSet<string>();
            GetPrefabs("*" + _searchString + "*");
            GetScripts("*" + _searchString + "*");
            GetMiscFiles("*" + _searchString + "*");
        }
        private static void GetPrefabs(string searchString)
        {
            // _prefabSearchResults = new HashSet<string>();
            var prefabResults  = System.IO.Directory.GetFiles("Assets" + Path.DirectorySeparatorChar, searchString +".prefab", System.IO.SearchOption.AllDirectories);
            foreach (var prefabResult in prefabResults)
            {
                // _prefabSearchResults.Add(prefabResult);
                _filesResult.Add(prefabResult);
            }
            
            var byDirectoryResults = System.IO.Directory.GetDirectories("Assets" + Path.DirectorySeparatorChar, searchString, System.IO.SearchOption.AllDirectories);
            foreach (var directoryResult in byDirectoryResults)
            {
                var innerPrefabResults  = System.IO.Directory.GetFiles(directoryResult + Path.DirectorySeparatorChar,"*.prefab", System.IO.SearchOption.AllDirectories);
                foreach (var prefabResult in innerPrefabResults)
                {
                    // _prefabSearchResults.Add(prefabResult);
                    _filesResult.Add(prefabResult);
                }
            }
        }
        private void GetScripts(string searchString)
        {
            // _scriptsResults = new HashSet<string>();
            
            var scriptsResults  = System.IO.Directory.GetFiles("Assets" + Path.DirectorySeparatorChar, searchString +".cs", System.IO.SearchOption.AllDirectories);
            foreach (var scriptResult in scriptsResults)
            {
                // _scriptsResults.Add(scriptResult);
                _filesResult.Add(scriptResult);
            }
        }

        private void GetMiscFiles(string searchString)
        {
            var scriptsResults  = System.IO.Directory.GetFiles("Assets" + Path.DirectorySeparatorChar, searchString, System.IO.SearchOption.AllDirectories);
            foreach (var scriptResult in scriptsResults)
            {
                if (scriptResult.EndsWith(".meta"))
                {
                    continue;
                }
                _filesResult.Add(scriptResult);
            }
        }

        private void RenderSearchText()
        {
            // Debug.Log("Rendering search text");
            GUI.SetNextControlName("searchString");
            _searchString = EditorGUILayout.TextField(_searchString);
            EditorGUI.FocusTextInControl("searchString");
        }

        private void OnFocus()
        {
            EditorGUI.FocusTextInControl("searchString");
        }

        private void OnLostFocus()
        {
            // Close();
        }
    }
}