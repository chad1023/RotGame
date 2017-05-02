using System;
using System.Collections.Generic;
using System.Linq;
using UnityEditor;
using UnityEditorInternal;
using UnityEngine;

namespace EnhancedHierarchy {
    /// <summary>
    /// Save and load hierarchy preferences
    /// </summary>
    internal static class Preferences {

        private static int currentKeyIndex;
        private static string[] allKeys = new string[16];

        public class PrefItem<T> {
            public T Value {
                get { return _value; }
                set { SetValue(Key, _value = value); }
            }

            public string Key { get; private set; }
            public GUIContent Content { get; private set; }

            private T _value;

            public PrefItem(string key, T defaultValue, string text, string tooltip) {
                _value = (T)LoadValue(allKeys[currentKeyIndex++] = Key = key, defaultValue);
                Content = new GUIContent(text, tooltip);
            }

            private void SetValue(string key, object value) {
                if(value is int || value is Enum)
                    EditorPrefs.SetInt(key, (int)value);
                else if(value is float)
                    EditorPrefs.SetFloat(key, (float)value);
                else if(value is bool)
                    EditorPrefs.SetBool(key, (bool)value);
                else if(value is string)
                    EditorPrefs.SetString(key, (string)value);
            }

            private object LoadValue(string key, object defaultValue) {
                var typeOfT = typeof(T);

                if(typeOfT == typeof(int) || typeOfT.IsEnum)
                    return EditorPrefs.GetInt(key, (int)defaultValue);
                if(typeOfT == typeof(float))
                    return EditorPrefs.GetFloat(key, (float)defaultValue);
                if(typeOfT == typeof(bool))
                    return EditorPrefs.GetBool(key, (bool)defaultValue);
                if(typeOfT == typeof(string))
                    return EditorPrefs.GetString(key, (string)defaultValue);

                return null;
            }

            public static implicit operator T(PrefItem<T> pb) {
                return pb.Value;
            }

            public static implicit operator GUIContent(PrefItem<T> pb) {
                return pb.Content;
            }

        }

        static Preferences() {
            ReloadPrefs();
        }

        private static readonly GUIContent listContent = new GUIContent("Buttons and Toggles", "The buttons and toggles that will appear in the hierarchy");
        public static PrefItem<int> Offset { get; private set; }
        public static PrefItem<bool> Tree { get; private set; }
        public static PrefItem<bool> Warnings { get; private set; }
        public static PrefItem<bool> Tooltips { get; private set; }
        public static PrefItem<bool> Selection { get; private set; }
        public static PrefItem<bool> Trailing { get; private set; }
        public static PrefItem<bool> AllowSelectingLocked { get; private set; }
        public static PrefItem<bool> Enabled { get; private set; }
        public static PrefItem<MiniLabelType> LabelType { get; private set; }
        public static PrefItem<StripType> Separators { get; private set; }
        public static PrefItem<StaticMode> StaticAskMode { get; private set; }
        public static List<DrawType> DrawOrder {
            get { return _drawOrder; }
            private set {
                EditorPrefs.SetInt("HierarchyDrawOrder" + "Count", value.Count);
                for(var i = 0; i < value.Count; i++)
                    EditorPrefs.SetInt("HierarchyDrawOrder" + i, (int)value[i]);
                _drawOrder = value;
            }
        }

        public static bool LineSeparator { get { return (Separators & StripType.Lines) != 0; } }
        public static bool ColorSeparator { get { return (Separators & StripType.Color) != 0; } }

        private static Vector2 scroll;
        private static ReorderableList rList;
        private static List<DrawType> _drawOrder;

        private static GenericMenu Menu {
            get {
                var menu = new GenericMenu();

                for(var i = (DrawType)0; i < DrawType.MaxValue - 1; i++)
                    if(!rList.list.Contains(i)) {
                        var draw = i;
                        menu.AddItem(new GUIContent(ObjectNames.NicifyVariableName(draw.ToString())), false, () => rList.list.Add(draw));
                    }

                return menu;
            }
        }

        private static void ReloadPrefs() {
            Enabled = new PrefItem<bool>("HierarchyEnabled", true, "Enabled (Ctrl+H)", "Enable or disable the entire plugin, it will be automatically disabled if any error occurs");
            Offset = new PrefItem<int>("HierarchyOffset", 2, "Offset", "Offset for icons, useful if you have more extensions that also uses hierarchy");
            Tree = new PrefItem<bool>("HierarchyTree", true, "Hierarchy tree", "Shows lines connecting child transforms to their parent, useful if you have multiple childs inside childs");
            Warnings = new PrefItem<bool>("HierarchyWarning", true, "Warnings", "Shows icons next to the GameObject if any error, warning or log is referencing it");
            Tooltips = new PrefItem<bool>("HierarchyTooltip", true, "Tooltips", "Shows tooltips, like this one");
            Selection = new PrefItem<bool>("HierarchySelection", true, "Enhanced Selection", "Allow selecting GameObjects by dragging over them with right mouse button");
            Separators = new PrefItem<StripType>("HierarchyStrips", StripType.ColorAndLines, "Separators", string.Empty);
            LabelType = new PrefItem<MiniLabelType>("HierarchyMiniLabel", MiniLabelType.TagOrLayer, "Mini label", "The little label next to the GameObject name");
            StaticAskMode = new PrefItem<StaticMode>("HierarchyStaticMode", StaticMode.Ask, "Static change mode", "How children of objects should react when changing you click on the static toggle");
            Trailing = new PrefItem<bool>("HierarchyTrailing", true, "Trailing", "Append ... when names are bigger than the view area");
            AllowSelectingLocked = new PrefItem<bool>("HierarchySelectLocked", true, "Allow locked selection", "Allow selecting objects that are locked");

            var list = new List<DrawType>();

            if(!EditorPrefs.HasKey(allKeys[currentKeyIndex++] = "HierarchyDrawOrderCount")) {
                list.Add(DrawType.Icon);
                list.Add(DrawType.Enabled);
                list.Add(DrawType.Lock);
                list.Add(DrawType.Static);
                list.Add(DrawType.ApplyPrefab);
                list.Add(DrawType.Tag);
                list.Add(DrawType.Layer);
            }
            else
                for(var i = 0; i < EditorPrefs.GetInt("HierarchyDrawOrderCount"); i++)
                    list.Add((DrawType)EditorPrefs.GetInt("HierarchyDrawOrder" + i));

            _drawOrder = list;
            rList = new ReorderableList(DrawOrder, typeof(DrawType), true, true, true, true);
            rList.drawHeaderCallback += rect => EditorGUI.LabelField(rect, listContent, EditorStyles.boldLabel);
            rList.drawElementCallback += (rect, index, focused, active) => EditorGUI.LabelField(rect, ObjectNames.NicifyVariableName(rList.list[index].ToString()));
            rList.onAddDropdownCallback += (rect, newList) => Menu.DropDown(rect);
        }

        public static void DeleteSavedValues() {
            currentKeyIndex = 0;
            foreach(var key in allKeys)
                EditorPrefs.DeleteKey(key);
            ReloadPrefs();
        }

        [PreferenceItem("Hierarchy")]
        private static void OnPreferencesGUI() {
            try {
                scroll = EditorGUILayout.BeginScrollView(scroll, false, false);

                EditorGUILayout.Separator();
                GUI.enabled = Enabled.Value = EditorGUILayout.Toggle(Enabled, Enabled);
                EditorGUILayout.Separator();
                Offset.Value = EditorGUILayout.IntField(Offset, Offset);
                EditorGUILayout.Separator();

                Tree.Value = EditorGUILayout.Toggle(Tree, Tree);
                Warnings.Value = EditorGUILayout.Toggle(Warnings, Warnings);
                Tooltips.Value = EditorGUILayout.Toggle(Tooltips, Tooltips);
                Selection.Value = EditorGUILayout.Toggle(Selection, Selection);
                Trailing.Value = EditorGUILayout.Toggle(Trailing, Trailing);
                AllowSelectingLocked.Value = EditorGUILayout.Toggle(AllowSelectingLocked, AllowSelectingLocked);

                EditorGUILayout.Separator();

                Separators.Value = (StripType)EditorGUILayout.EnumPopup(Separators, Separators);
                LabelType.Value = (MiniLabelType)EditorGUILayout.EnumPopup(LabelType, LabelType);
                GUI.enabled = Enabled && DrawOrder.Contains(DrawType.Static);
                StaticAskMode.Value = (StaticMode)EditorGUILayout.EnumPopup(StaticAskMode, StaticAskMode);
                GUI.enabled = Enabled;

                EditorGUILayout.Separator();

                DrawOrder = rList.list.Cast<DrawType>().ToList();
                rList.displayAdd = Menu.GetItemCount() > 0;
                rList.DoLayoutList();

                GUI.enabled = true;
                EditorGUILayout.EndScrollView();

                if(GUILayout.Button("Use Defaults", GUILayout.Width(120f)))
                    DeleteSavedValues();

                Styles.ReloadTooltips();
                EditorApplication.RepaintHierarchyWindow();
            }
            catch(Exception e) {
                EditorGUILayout.HelpBox(e.ToString(), MessageType.Error);
            }
        }

    }
}