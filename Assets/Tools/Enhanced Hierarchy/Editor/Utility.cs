using System;
using System.Diagnostics;
using System.Reflection;
using UnityEditor;
using UnityEditorInternal;
using UnityEngine;
using Debug = UnityEngine.Debug;
using Object = UnityEngine.Object;

namespace EnhancedHierarchy {
    /// <summary>
    /// Misc utilities for Enhanced Hierarchy.
    /// </summary>
    internal static class Utility {

        private const double FPS_UPDATE_RATE = 0.5d; //2 times per second
        private const string MENU_ITEM_PATH = "Edit/Enhanced Hierarchy %h";
        public const BindingFlags FULL_BINDING = BindingFlags.Static | BindingFlags.NonPublic | BindingFlags.Instance | BindingFlags.Public;

        private static object playModeColor = typeof(Editor).Assembly.GetType("UnityEditor.HostView").GetField("kPlayModeDarken", FULL_BINDING).GetValue(null);
        private static PropertyInfo playModeColorProp = typeof(Editor).Assembly.GetType("UnityEditor.PrefColor").GetProperty("Color", FULL_BINDING);
        private static Type hierarchyWindowType = typeof(Editor).Assembly.GetType("UnityEditor.SceneHierarchyWindow");

        private static int errorCount;
        private static int frames;
        private static double lastFps;
        private static double lastTime;

        public static Color PlaymodeTint {
            get {
                try {
                    if(!EditorApplication.isPlayingOrWillChangePlaymode)
                        return Color.white;
                    return (Color)playModeColorProp.GetValue(playModeColor, null);
                }
                catch {
                    return Color.white;
                }
            }
        }
        public static bool HierarchyFocused { get { return EditorWindow.focusedWindow && EditorWindow.focusedWindow.GetType() == hierarchyWindowType; } }

        public static void LogException(Exception e) {
            Debug.LogErrorFormat("Unexpected exception in Enhanced Hierarchy: {0}", e);

            if(errorCount++ >= 3) {
                Debug.LogWarning("Automatically disabling Enhanced Hierarchy, if the error persists contact the developer");
                Preferences.Enabled.Value = false;
                errorCount = 0;
            }
        }

        [MenuItem(MENU_ITEM_PATH)]
        private static void EnableDisableHierarchy() {
            Preferences.Enabled.Value = !Preferences.Enabled;
            InternalEditorUtility.RepaintAllViews();
        }

        [MenuItem(MENU_ITEM_PATH, true)]
        private static bool CheckHierarchyEnabled() {
            Menu.SetChecked(MENU_ITEM_PATH, Preferences.Enabled);
            return true;
        }

        [Conditional("HIERARCHY_DEBUG")]
        public static void ShowFPS() {
            using(new ProfilerSample("FPS Counter")) {
                frames++;

                var rect = new Rect(0f, 0f, 65f, 16f);

                EditorGUI.DrawRect(rect, Color.yellow);
                EditorGUI.LabelField(rect, string.Format("{0:00.0} FPS", lastFps));

                if(EditorApplication.timeSinceStartup - lastTime < FPS_UPDATE_RATE)
                    return;

                lastFps = frames / (EditorApplication.timeSinceStartup - lastTime);
                lastTime = EditorApplication.timeSinceStartup;
                frames = 0;
            }
        }

        [Conditional("HIERARCHY_DEBUG")]
        public static void ForceUpdateHierarchyEveryFrame() {
            EditorApplication.update += EditorApplication.RepaintHierarchyWindow;
        }

        public static void ShowIconSelector(Object targetObj, Rect activatorRect, bool showLabelIcons) {
            try {
                var type = typeof(Editor).Assembly.GetType("UnityEditor.IconSelector");
                var instance = ScriptableObject.CreateInstance(type);
                var parameters = new object[] {
                    targetObj,
                    activatorRect,
                    showLabelIcons
                };

                type.InvokeMember("Init", FULL_BINDING | BindingFlags.InvokeMethod, null, instance, parameters);
            }
            catch(Exception e) {
                Debug.LogWarning("Failed to open icon selector\n" + e);
            }
        }

        public static Texture2D FindOrLoad(byte[] bytes, string name) {
            return FindTextureFromName(name) ?? ConvertToTexture(bytes, name);
        }

        public static Texture2D ConvertToTexture(byte[] bytes, string name) {
            try {
                var texture = new Texture2D(0, 0, TextureFormat.ARGB32, false, false) {
                    name = name,
                    hideFlags = HideFlags.HideAndDontSave
                };

                texture.LoadImage(bytes);
                return texture;
            }
            catch(Exception e) {
                Debug.LogException(e);
                return null;
            }
        }

        public static Texture2D FindTextureFromName(string name) {
            try {
                var textures = Resources.FindObjectsOfTypeAll<Texture2D>();

                for(var i = 0; i < textures.Length; i++)
                    if(textures[i].name == name)
                        return textures[i];

                return null;
            }
            catch(Exception e) {
                Debug.LogException(e);
                return null;
            }
        }

        public static Color GetHierarchyColor(GameObject go) {
            if(!go)
                return Color.black;

            var prefabType = PrefabUtility.GetPrefabType(PrefabUtility.FindPrefabRoot(go));
            var active = go.activeInHierarchy;
            var style = active ? Styles.labelNormal : Styles.labelDisabled;

            switch(prefabType) {
                case PrefabType.PrefabInstance:
                case PrefabType.ModelPrefabInstance:
                    style = active ? Styles.labelPrefab : Styles.labelPrefabDisabled;
                    break;
                case PrefabType.MissingPrefabInstance:
                    style = active ? Styles.labelPrefabBroken : Styles.labelPrefabBrokenDisabled;
                    break;
            }

            return style.normal.textColor;
        }

        public static Color GetHierarchyColor(Transform t) {
            if(!t)
                return Color.black;

            return GetHierarchyColor(t.gameObject);
        }

        public static bool LastInHierarchy(Transform t) {
            if(!t)
                return true;

            return t.parent.GetChild(t.parent.childCount - 1) == t;
        }

        public static bool LastInHierarchy(GameObject go) {
            if(!go)
                return true;

            return LastInHierarchy(go.transform);
        }

        public static GUIStyle CreateStyleFromTextures(Texture2D on, Texture2D off) {
            var style = new GUIStyle();

            style.active.background = off;
            style.focused.background = off;
            style.hover.background = off;
            style.normal.background = off;
            style.onActive.background = on;
            style.onFocused.background = on;
            style.onHover.background = on;
            style.onNormal.background = on;
            style.imagePosition = ImagePosition.ImageOnly;
            style.fixedHeight = 15f;
            style.fixedWidth = 15f;

            return style;
        }
    }
}