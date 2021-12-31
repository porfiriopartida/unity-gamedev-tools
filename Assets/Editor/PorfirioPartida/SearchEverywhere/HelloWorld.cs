using UnityEditor;
using UnityEngine;

namespace Editor.PorfirioPartida.SearchEverywhere
{
    public class HelloWorld : EditorWindow
    {
        [MenuItem("Tools/Hello World")]
        private static void ShowWindow()
        {
            var window = GetWindow<HelloWorld>();
            window.titleContent = new GUIContent("Hello World");
            window.Show();
        }

        private void OnGUI()
        {
            GUILayout.Label("Hello World");
        }
    }
}