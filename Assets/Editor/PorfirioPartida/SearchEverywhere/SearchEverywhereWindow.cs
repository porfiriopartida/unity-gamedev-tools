// using System.Collections.Generic;
// using System.IO;
// using UnityEditor;
// using UnityEngine;
//
// namespace Editor.PorfirioPartida.SearchEverywhere
// {
//     public class SearchEverywhereWindow : EditorWindow
//     {
//         private static HashSet<string> _prefabSearchResults;
//         private static HashSet<string> _scriptsResults;
//
//         private static string _searchString = "";
//         private void OnGUI()
//         {   
//             Debug.Log("OnGUI..");
//             EditorGUILayout.Space(5f, true);
//             RenderSearchText();
//             // if (_searchString.Length <= 0)
//             // {
//             //     return;
//             // }
//
//             // RenderPrefabs();
//             // RenderScripts();
//             // //RenderActions();
//             //
//             // if (!GUI.changed) return;
//             //
//             // PerformSearch();
//         }
//
//         private void RenderScripts()
//         {
//             if (_scriptsResults == null || _scriptsResults.Count == 0){
//                 return;
//             }
//             foreach (var s in _scriptsResults)
//             {
//                 EditorGUILayout.LabelField(s.ToString());
//             }
//         }
//
//         private void RenderPrefabs()
//         {
//             if (_prefabSearchResults == null || _prefabSearchResults.Count == 0){
//                 return;
//             }
//             // GUIStyle _buttonStyle = new GUIStyle
//             // {
//             //     alignment = TextAnchor.MiddleLeft,
//             //     padding = new RectOffset(5, 0, 1, 1),
//             //     normal = new GUIStyleState() { textColor = Color.white },
//             //     hover = new GUIStyleState() { textColor = Color.gray },
//             //     active = new GUIStyleState() { textColor = Color.gray },
//             // };
//
//             foreach (var t in _prefabSearchResults)
//             {
//                 // GUI.skin.button = _buttonStyle;
//                 var b = GUILayout.Button($"Prefab | {t}");
//                 // EditorGUILayout.LabelField(t);
//                 // var rect = GUILayoutUtility.GetLastRect();
//                 // var pos = Event.current.mousePosition;
//                 // if (rect.Contains(pos))
//                 // {
//                 //     Debug.Log($"Mouse over. {t}");
//                 //     Repaint();
//                 // }
//                 if (b)
//                 {
//                     Debug.Log($"Pressed button {t}");
//                 }
//             }
//         }
//
//
//         private void PerformSearch()
//         {
//             Debug.Log("Searching..");
//             GetPrefabs("*" + _searchString + "*");
//             GetScripts("*" + _searchString + "*");
//         }
//
//         private void GetScripts(string searchString)
//         {
//             _scriptsResults = new HashSet<string>();
//             
//             var scriptsResults  = System.IO.Directory.GetFiles("Assets" + Path.DirectorySeparatorChar, searchString +".cs", System.IO.SearchOption.AllDirectories);
//             foreach (var scriptResult in scriptsResults)
//             {
//                 _scriptsResults.Add(scriptResult);
//             }
//         }
//
//         private void RenderSearchText()
//         {
//             Debug.Log("Rendering search text");
//             GUI.SetNextControlName("searchString");
//             _searchString = EditorGUILayout.TextField(_searchString);
//             EditorGUI.FocusTextInControl("searchString");
//             
//             // EditorGUI.FocusTextInControl("searchString");
//         }
//
//         private void OnFocus()
//         {
//             EditorGUI.FocusTextInControl("searchString");
//         }
//
//         private void OnLostFocus()
//         {
//             // Close();
//         }
//
//         private static void GetPrefabs(string searchString)
//         {
//             _prefabSearchResults = new HashSet<string>();
//             
//             var prefabResults  = System.IO.Directory.GetFiles("Assets" + Path.DirectorySeparatorChar, searchString +".prefab", System.IO.SearchOption.AllDirectories);
//             foreach (var prefabResult in prefabResults)
//             {
//                 _prefabSearchResults.Add(prefabResult);
//             }
//             
//             var byDirectoryResults = System.IO.Directory.GetDirectories("Assets" + Path.DirectorySeparatorChar, searchString, System.IO.SearchOption.AllDirectories);
//             foreach (var directoryResult in byDirectoryResults)
//             {
//                 var innerPrefabResults  = System.IO.Directory.GetFiles(directoryResult + Path.DirectorySeparatorChar,"*.prefab", System.IO.SearchOption.AllDirectories);
//                 foreach (var prefabResult in innerPrefabResults)
//                 {
//                     _prefabSearchResults.Add(prefabResult);
//                 }
//             }
//         }
//     }
// }