using BepInEx;
using BepInEx.Logging;
using System.Collections.Generic;
using BepInEx.Configuration;

namespace DragAndDrop
{
    public abstract class DragAndDropCore : BaseUnityPlugin
    {
        public const string PluginName = "Drag & Drop";
        public const string GUID = "keelhauled.draganddrop";
        public const string Version = "1.3.0";
        internal static new ManualLogSource Logger;

        private UnityDragAndDropHook hook;
        internal static ConfigEntry<bool> ShowSceneOverwriteWarnings;

        private void Awake()
        {
            Logger = base.Logger;

            ShowSceneOverwriteWarnings = Config.Bind("General", "Show scene overwrite warnings", true, "Show a confirmation dialog box when loading the dropped scene would result in losing data in the currently loaded scene.");
        }

        private void Start() // Run in start instead of onenable to avoid the hook timing out and failing in rare cases
        {
            hook = new UnityDragAndDropHook();
            hook.InstallHook();
            hook.OnDroppedFiles += (aFiles, aPos) => ThreadingHelper.Instance.StartSyncInvoke(() => OnFiles(aFiles, aPos));
        }

        private void OnDestroy()
        {
            hook.UninstallHook();
        }

        internal abstract void OnFiles(List<string> aFiles, POINT aPos);
    }
}
