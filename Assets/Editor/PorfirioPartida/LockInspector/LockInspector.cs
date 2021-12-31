using UnityEditor;

namespace Editor.PorfirioPartida.LockInspector
{
    public class LockInspector : UnityEditor.Editor
    {
        [MenuItem("Tools/Toggle Inspector Lock %l")] // Ctrl + L
        public static void ToggleInspectorLock()
        {
            ActiveEditorTracker.sharedTracker.isLocked = !ActiveEditorTracker.sharedTracker.isLocked;
            ActiveEditorTracker.sharedTracker.ForceRebuild();
        }
    }
}
