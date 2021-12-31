using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using UnityEditor;
using UnityEngine;
/*
 * TODO: Optimize lookups.
 * TODO: Support Custom Assembly. Most likely inside ListActions
 * TODO: Support / and \ for file lookups.
 * TODO: Support refresh OnEnable/OnAwake
 * TODO: Add Colors
 * TODO: Add panel instead of button
 * TODO: Add tabs
 * TODO: Index for big projects?
 */
namespace Editor.PorfirioPartida.SearchEverywhere
{
    public class SearchEverywhereEditor : EditorWindow
    {
        private GUIStyle _buttonStyle;
        private static HashSet<string> _filesResult;
        private static string _searchString = "";
        private readonly MenuItemsScanner _menuItemsScanner = new MenuItemsScanner();
        
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
            _menuItemsScanner.Scan();
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
            RenderAction(_menuItemsScanner.GetActions(), "Action");
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
        private static readonly string[] Keys = { "%", "&" , "#"};

        private string replaceCtrlShiftAlt(string pattern)
        {
            var shortcutDictionary = new Dictionary<string, string>
            {
                { "%", "Ctrl" },
                { "&", "Alt" },
                { "#", "Shift" }
            };

            var shortcutList = new List<string>();
            if (Keys.Any(pattern.Contains))
            {
                foreach(var key in Keys){
                    if (pattern.Contains(key))
                    {
                        pattern = pattern.Replace(key, "");
                        shortcutList.Add(shortcutDictionary[key]);
                    }
                }
                shortcutList.Add(pattern.ToUpper());

                var newPattern = string.Join(" + ", shortcutList.ToArray());
                return $" ({newPattern})";
            }

            return pattern;
        }

        private string CleanAction(string action)
        {
            var tokens = action.Split(' ');
            if (tokens.Length <= 1)
            {
                return action;
            }

            var lastItem = tokens[tokens.Length - 1];
            var allButLast = string.Join(" ", tokens, 0, tokens.Length - 1);
            return allButLast + replaceCtrlShiftAlt(lastItem);
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

                var b = GUILayout.Button($"{prefix} | {CleanAction(r)}", _buttonStyle);
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
            GetMiscFiles(_filesResult, "*" + _searchString + "*");
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
            var scriptsResults  = Directory.GetFiles("Assets" + Path.DirectorySeparatorChar, searchString +".cs", System.IO.SearchOption.AllDirectories);
            foreach (var scriptResult in scriptsResults)
            {
                _filesResult.Add(scriptResult);
            }
        }

        private void GetMiscFiles(ISet<string> holder, string searchString, string fileType = "")
        {
            var searchPattern = searchString + fileType;
            var scriptsResults  = Directory
                .GetFiles("Assets" + Path.DirectorySeparatorChar, searchPattern, SearchOption.AllDirectories)
                .Where(n => !n.EndsWith(".meta"));
            foreach (var scriptResult in scriptsResults)
            {
                // if (scriptResult.EndsWith(".meta"))
                // {
                //     continue;
                // }
                holder.Add(scriptResult);
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