/* Enhanced Hierarchy for Unity
 * Version 2.1.0
 * Samuel Schultze
 * samuelschultze@gmail.com
*/

using System;
using System.Collections.Generic;
using System.Linq;
using UnityEditor;
using UnityEditorInternal;
using UnityEngine;

namespace EnhancedHierarchy {
    /// <summary>
    /// Main class, draws hierarchy items.
    /// </summary>
    [InitializeOnLoad]
    internal static class EnhancedHierarchy {

        private const string UNTAGGED = "Untagged";
        private const int UNLAYERED = 0;

        private static int warningsIconCount;
        private static bool isFirstVisible;
        private static bool isRepaintEvent;
        private static bool isGameObject;
        private static bool hasTag;
        private static bool hasLayer;
        private static string goLogs;
        private static string goWarnings;
        private static string goErrors;
        private static Rect rawRect;
        private static Rect lastRect;
        private static Rect selectionRect;
        private static Color currentColor;
        private static Vector2 selectionStart;
        private static GameObject currentGameObject;
        private static List<GameObject> dragSelection;
        private static GUIContent tempTooltipContent = new GUIContent();

        static EnhancedHierarchy() {
            Utility.ForceUpdateHierarchyEveryFrame();
            EditorApplication.hierarchyWindowItemOnGUI += SetItemInformation;
            EditorApplication.hierarchyWindowItemOnGUI += OnItemGUI;
        }

        private static void SetItemInformation(int id, Rect rect) {
            if(!Preferences.Enabled)
                return;

            using(new ProfilerSample("Enhanced Hierarchy"))
            using(new ProfilerSample("Getting items information"))
                try {
                    isRepaintEvent = Event.current.type == EventType.Repaint;

                    rawRect = rect;
                    currentGameObject = EditorUtility.InstanceIDToObject(id) as GameObject;

                    isGameObject = currentGameObject;

                    if(isGameObject) {
                        hasTag = currentGameObject.tag != UNTAGGED;
                        hasLayer = currentGameObject.layer != UNLAYERED;
                        currentColor = Utility.GetHierarchyColor(currentGameObject);
                    }

                    isFirstVisible = rawRect.y <= lastRect.y;

                    //if(isFirstVisible)
                    //finalRect = lastRect;

                    //isLastVisible = finalRect == rawRect;

                    //if(isFirstVisible)
                    //    EditorGUI.DrawRect(rawRect, Color.red);
                    //if(isLastVisible)
                    //    EditorGUI.DrawRect(rawRect, Color.blue);

                    if(isRepaintEvent && isFirstVisible)
                        Utility.ShowFPS();
                }
                catch(Exception e) {
                    Utility.LogException(e);
                }
        }

        private static void OnItemGUI(int id, Rect rect) {
            if(!Preferences.Enabled)
                return;

            using(new ProfilerSample("Enhanced Hierarchy"))
                try {
                    if(Preferences.Selection)
                        DoSelection(rawRect);

                    if(isFirstVisible && !Preferences.AllowSelectingLocked)
                        IgnoreLockedSelection();

                    if(isFirstVisible && isRepaintEvent) {
                        if(Preferences.ColorSeparator)
                            ColorSort(rawRect);
                        if(Preferences.LineSeparator)
                            DrawHorizontalSeparator(rawRect);
                    }

                    if(isGameObject && isRepaintEvent) {
                        if(Preferences.Tree)
                            DrawTree(rawRect);
                        warningsIconCount = 0;
                        if(Preferences.Warnings)
                            GetGameObjectWarnings();
                        if(Preferences.Trailing)
                            DoTrailing(rawRect);
                        if(Preferences.Warnings)
                            DrawWarnings(rawRect);
                    }

                    if(isGameObject) {
                        Undo.RecordObject(currentGameObject, "Hierarchy Changed");

                        rect.xMin = rect.xMax - rect.height;
                        rect.x += rect.height - Preferences.Offset;
                        rect.y++;

                        for(var i = 0; i < Preferences.DrawOrder.Count; i++) {
                            rect.x -= rect.height;
                            GUI.backgroundColor = Styles.backgroundColorEnabled;
                            switch(Preferences.DrawOrder[i]) {
                                case DrawType.Enabled:
                                    DrawActiveButton(rect);
                                    break;

                                case DrawType.Static:
                                    DrawStaticButton(rect);
                                    break;

                                case DrawType.Lock:
                                    DrawLockButton(rect);
                                    break;

                                case DrawType.Icon:
                                    DrawIcon(rect);
                                    break;

                                case DrawType.ApplyPrefab:
                                    DrawPrefabApply(rect);
                                    break;

                                case DrawType.Tag:
                                    DrawTag(rect);
                                    break;

                                case DrawType.Layer:
                                    DrawLayer(rect);
                                    break;
                            }
                        }

                        DrawMiniLabel(ref rect);
                        GUI.backgroundColor = Color.white;
                    }

                    if(Preferences.Tooltips && isGameObject && isRepaintEvent)
                        DrawTooltip(rect);
                }
                catch(Exception e) {
                    Utility.LogException(e);
                }
                finally {
                    if(isRepaintEvent)
                        lastRect = rawRect;
                }
        }

        private static void IgnoreLockedSelection() {
            var selection = Selection.objects;
            var changed = false;

            for(var i = 0; i < selection.Length; i++)
                if(selection[i] is GameObject && !EditorUtility.IsPersistent(selection[i]) && (selection[i].hideFlags & HideFlags.NotEditable) != 0) {
                    selection[i] = null;
                    changed = true;
                }

            if(changed) {
                Selection.objects = selection;
                EditorApplication.RepaintHierarchyWindow();
            }
        }

        private static void DrawStaticButton(Rect rect) {
            using(new ProfilerSample("Static toggle")) {
                GUI.changed = false;
                GUI.backgroundColor = currentGameObject.isStatic ? Styles.backgroundColorDisabled : Styles.backgroundColorEnabled;
                GUI.Toggle(rect, currentGameObject.isStatic, Styles.staticContent, Styles.staticToggleStyle);

                if(!GUI.changed)
                    return;

                var changeMode = Preferences.StaticAskMode.Value;

                if(currentGameObject.transform.childCount == 0)
                    changeMode = StaticMode.ObjectOnly;
                else if(changeMode == StaticMode.Ask) {
                    var result = EditorUtility.DisplayDialogComplex("Change Static Flags",
                                                                    "Do you want to " + (currentGameObject.isStatic ? "enable" : "disable") + " the static flags for all child objects as well?",
                                                                    "Yes, change children",
                                                                    "No, this object only",
                                                                    "Cancel");

                    if(result == 2)
                        return;

                    changeMode = (StaticMode)result;
                }

                var isStatic = !currentGameObject.isStatic;

                switch(changeMode) {
                    case StaticMode.ObjectOnly:
                        currentGameObject.isStatic = isStatic;
                        break;
                    case StaticMode.ObjectAndChildren:
                        foreach(var transform in currentGameObject.GetComponentsInChildren<Transform>())
                            transform.gameObject.isStatic = isStatic;
                        break;
                }
            }
        }

        private static void DrawLockButton(Rect rect) {
            using(new ProfilerSample("Lock toggle")) {
                var locked = (currentGameObject.hideFlags & HideFlags.NotEditable) != 0;

                GUI.changed = false;
                GUI.backgroundColor = locked ? Styles.backgroundColorEnabled : Styles.backgroundColorDisabled;
                GUI.Toggle(rect, locked, Styles.lockContent, Styles.lockToggleStyle);

                if(!GUI.changed)
                    return;

                currentGameObject.hideFlags += locked ? -8 : 8;
                InternalEditorUtility.RepaintAllViews();
            }
        }

        private static void DrawActiveButton(Rect rect) {
            using(new ProfilerSample("Active toggle")) {
                GUI.changed = false;
                GUI.backgroundColor = currentGameObject.activeSelf ? Styles.backgroundColorEnabled : Styles.backgroundColorDisabled;
                GUI.Toggle(rect, currentGameObject.activeSelf, Styles.activeContent, Styles.activeToggleStyle);

                if(GUI.changed)
                    currentGameObject.SetActive(!currentGameObject.activeSelf);
            }
        }

        private static void DrawIcon(Rect rect) {
            using(new ProfilerSample("Icon")) {
                var content = EditorGUIUtility.ObjectContent(currentGameObject, typeof(GameObject));

                if(!content.image)
                    return;

                if(Preferences.Tooltips)
                    content.tooltip = "Change Icon";

                content.text = string.Empty;
                rect.yMin++;
                rect.xMin++;

                if(GUI.Button(rect, content, EditorStyles.label))
                    Utility.ShowIconSelector(currentGameObject, rect, true);
            }
        }

        private static void DrawPrefabApply(Rect rect) {
            using(new ProfilerSample("Prefab apply button")) {
                var isPrefab = PrefabUtility.GetPrefabType(currentGameObject) == PrefabType.PrefabInstance;

                GUI.contentColor = isPrefab ? Styles.backgroundColorEnabled : Styles.backgroundColorDisabled;

                if(GUI.Button(rect, Styles.prefabApplyContent, Styles.applyPrefabStyle))
                    if(isPrefab) {
                        var selected = Selection.instanceIDs;
                        Selection.activeGameObject = currentGameObject;
                        EditorApplication.ExecuteMenuItem("GameObject/Apply Changes To Prefab");
                        Selection.instanceIDs = selected;
                    }
                    else {
                        var path = EditorUtility.SaveFilePanelInProject("Save prefab", "New Prefab", "prefab", "Save the selected prefab");
                        if(path.Length > 0)
                            PrefabUtility.CreatePrefab(path, currentGameObject, ReplacePrefabOptions.ConnectToPrefab);
                    }

                GUI.contentColor = Color.white;
            }
        }

        private static void DrawLayer(Rect rect) {
            using(new ProfilerSample("Layer")) {
                GUI.changed = false;

                EditorGUI.LabelField(rect, Styles.layerContent);
                var layer = EditorGUI.LayerField(rect, currentGameObject.layer, Styles.layerStyle);

                if(GUI.changed)
                    currentGameObject.layer = layer;
            }
        }

        private static void DrawTag(Rect rect) {
            using(new ProfilerSample("Tag")) {
                GUI.changed = false;

                EditorGUI.LabelField(rect, Styles.tagContent);
                var tag = EditorGUI.TagField(rect, Styles.tagContent, currentGameObject.tag, Styles.tagStyle);

                if(GUI.changed)
                    currentGameObject.tag = tag;
            }
        }

        private static void DrawHorizontalSeparator(Rect rect) {
            using(new ProfilerSample("Horizontal separator")) {
                rect.xMin = 0f;
                rect.yMax = rect.yMin + 1f;

                var count = Mathf.Max(100, (lastRect.y - rect.y) / lastRect.height);

                for(var i = 0; i < count; i++) {
                    rect.y += lastRect.height;
                    EditorGUI.DrawRect(rect, Styles.lineColor);
                }
            }
        }

        private static void ColorSort(Rect rect) {
            using(new ProfilerSample("Colored sort")) {
                rect.xMin = 0f;

                var count = Mathf.Max(100, (lastRect.y - rect.y) / lastRect.height);

                for(var i = 0; i < count; i++) {
                    if(rect.y / 16f % 2 < 1f)
                        EditorGUI.DrawRect(rect, Styles.sortColor);
                    rect.y += rect.height;
                }
            }
        }

        private static void DrawTree(Rect rect) {
            using(new ProfilerSample("Hierarchy tree")) {
                rect.xMin -= 14f;
                rect.xMax = rect.xMin + 14f;

                GUI.color = currentColor;

                if(currentGameObject.transform.childCount == 0 && currentGameObject.transform.parent) {
                    if(Utility.LastInHierarchy(currentGameObject.transform))
                        GUI.DrawTexture(rect, Styles.treeEndTexture);
                    else
                        GUI.DrawTexture(rect, Styles.treeMiddleTexture);
                }

                var parent = currentGameObject.transform.parent;

                for(rect.x -= 14f; rect.xMin > 0f && parent && parent.parent; rect.x -= 14f) {
                    GUI.color = Utility.GetHierarchyColor(parent.parent);
                    if(!Utility.LastInHierarchy(parent))
                        GUI.DrawTexture(rect, Styles.treeLineTexture);
                    parent = parent.parent;
                }

                GUI.color = Color.white;
            }
        }

        private static void GetGameObjectWarnings() {
            using(new ProfilerSample("Retrieve warnings")) {
                List<LogEntry> contextEntries;

                goLogs = string.Empty;
                goWarnings = string.Empty;
                goErrors = string.Empty;

                if(!LogEntry.ReferencedObjects.TryGetValue(currentGameObject, out contextEntries))
                    return;

                var count = contextEntries.Count;
                var components = currentGameObject.GetComponents<MonoBehaviour>();

                for(var i = 0; i < components.Length; i++)
                    if(!components[i]) {
                        goWarnings += "Missing mono behaviour\n";
                        break;
                    }

                for(var i = 0; i < count; i++)
                    if(goLogs.Length < 150 && contextEntries[i].HasMode(EntryMode.ScriptingLog))
                        goLogs += contextEntries[i] + "\n";
                    else if(goWarnings.Length < 150 && contextEntries[i].HasMode(EntryMode.ScriptingWarning))
                        goWarnings += contextEntries[i] + "\n";
                    else if(goErrors.Length < 150 && contextEntries[i].HasMode(EntryMode.ScriptingError))
                        goErrors += contextEntries[i] + "\n";

                if(goLogs.Length > 0)
                    warningsIconCount++;
                if(goWarnings.Length > 0)
                    warningsIconCount++;
                if(goErrors.Length > 0)
                    warningsIconCount++;
            }
        }

        private static void DrawWarnings(Rect rect) {
            var labelSize = EditorStyles.label.CalcSize(new GUIContent(currentGameObject.name)).x;

            rect.xMin += labelSize;
            rect.xMin = Math.Min(rect.xMax - (Preferences.DrawOrder.Count + warningsIconCount) * rect.height - CalcMiniLabelSize().x - 5f - Preferences.Offset, rect.xMin);
            rect.height = 17f;
            rect.xMax = rect.xMin + rect.height;

            if(goLogs.Length > 0) {
                tempTooltipContent.tooltip = goLogs;
                GUI.DrawTexture(rect, Styles.infoIcon, ScaleMode.ScaleToFit);
                EditorGUI.LabelField(rect, tempTooltipContent);
                rect.x += rect.width;
            }
            if(goWarnings.Length > 0) {
                tempTooltipContent.tooltip = goWarnings;
                GUI.DrawTexture(rect, Styles.warningIcon, ScaleMode.ScaleToFit);
                EditorGUI.LabelField(rect, tempTooltipContent);
                rect.x += rect.width;
            }
            if(goErrors.Length > 0) {
                tempTooltipContent.tooltip = goErrors;
                GUI.DrawTexture(rect, Styles.errorIcon, ScaleMode.ScaleToFit);
                EditorGUI.LabelField(rect, tempTooltipContent);
                rect.x += rect.width;
            }
        }

        private static void DoTrailing(Rect rect) {
            using(new ProfilerSample("Trailing")) {
                var size = Styles.labelNormal.CalcSize(new GUIContent(currentGameObject.name));

                rect.xMax -= (Preferences.DrawOrder.Count + warningsIconCount) * rect.height + CalcMiniLabelSize().x + Preferences.Offset;

                if(size.x < rect.width)
                    return;

                rect.yMin += 2f;
                rect.xMin = rect.xMax - 18f;
                rect.xMax = 1000f;

                EditorGUI.DrawRect(rect, Styles.normalColor * Utility.PlaymodeTint);
                if(Preferences.ColorSeparator && rect.y / 16f % 2 <= 1f)
                    EditorGUI.DrawRect(rect, Styles.sortColor * Utility.PlaymodeTint);
                if(Selection.gameObjects.Contains(currentGameObject))
                    EditorGUI.DrawRect(rect, Utility.HierarchyFocused ? Styles.selectedFocusedColor : Styles.selectedUnfocusedColor);

                GUI.contentColor = currentColor;
                EditorGUI.LabelField(rect, "...");
                GUI.contentColor = Color.white;

                return;
            }
        }

        private static void TagField(ref Rect rect) {
            rect.xMin -= Styles.miniLabelStyle.CalcSize(new GUIContent(currentGameObject.tag)).x;
            var tag = EditorGUI.TagField(rect, currentGameObject.tag, Styles.miniLabelStyle);

            if(GUI.changed)
                currentGameObject.tag = tag;
        }

        private static void LayerField(ref Rect rect) {
            rect.xMin -= Styles.miniLabelStyle.CalcSize(new GUIContent(LayerMask.LayerToName(currentGameObject.layer))).x;
            var layer = EditorGUI.LayerField(rect, currentGameObject.layer, Styles.miniLabelStyle);

            if(GUI.changed)
                currentGameObject.layer = layer;
        }

        private static void DrawMiniLabel(ref Rect rect) {
            using(new ProfilerSample("Mini label")) {
                rect.x -= rect.height + 4f;
                rect.xMin += 15f;

                GUI.contentColor = currentColor;
                GUI.changed = false;

                switch(Preferences.LabelType.Value) {
                    case MiniLabelType.Tag:
                        if(hasTag)
                            TagField(ref rect);
                        break;

                    case MiniLabelType.Layer:
                        if(hasLayer)
                            LayerField(ref rect);
                        break;

                    case MiniLabelType.LayerOrTag:
                        if(hasLayer)
                            LayerField(ref rect);
                        else if(hasTag)
                            TagField(ref rect);
                        break;

                    case MiniLabelType.TagOrLayer:
                        if(hasTag)
                            TagField(ref rect);
                        else if(hasLayer)
                            LayerField(ref rect);
                        break;
                }

                GUI.contentColor = Color.white;
            }
        }

        private static Vector2 CalcMiniLabelSize() {
            using(new ProfilerSample("Calculating mini label size"))
                switch(Preferences.LabelType.Value) {
                    case MiniLabelType.Tag:
                        if(hasTag)
                            return Styles.miniLabelStyle.CalcSize(new GUIContent(currentGameObject.tag));
                        break;

                    case MiniLabelType.Layer:
                        if(hasLayer)
                            return Styles.miniLabelStyle.CalcSize(new GUIContent(LayerMask.LayerToName(currentGameObject.layer)));
                        break;

                    case MiniLabelType.LayerOrTag:
                        if(hasLayer)
                            return Styles.miniLabelStyle.CalcSize(new GUIContent(LayerMask.LayerToName(currentGameObject.layer)));
                        else if(hasTag)
                            return Styles.miniLabelStyle.CalcSize(new GUIContent(currentGameObject.tag));
                        break;

                    case MiniLabelType.TagOrLayer:
                        if(hasTag)
                            return Styles.miniLabelStyle.CalcSize(new GUIContent(currentGameObject.tag));
                        else if(hasLayer)
                            return Styles.miniLabelStyle.CalcSize(new GUIContent(LayerMask.LayerToName(currentGameObject.layer)));
                        break;
                }

            return Vector2.zero;
        }

        private static void DrawTooltip(Rect rect) {
            using(new ProfilerSample("Tooltips")) {
                if(dragSelection != null)
                    return;

                rect.xMax = rect.xMin;
                rect.xMin = 0f;

                tempTooltipContent.tooltip = string.Format("{0}\nTag: {1}\nLayer: {2}", currentGameObject.name, currentGameObject.tag, LayerMask.LayerToName(currentGameObject.layer));
                EditorGUI.LabelField(rect, tempTooltipContent);
            }
        }

        private static void DoSelection(Rect rect) {
            using(new ProfilerSample("Enhanced selection")) {
                rect.xMin = 0f;

                if(isFirstVisible && Event.current.button == 1)
                    switch(Event.current.type) {
                        case EventType.MouseDrag:
                            if(dragSelection == null) {
                                dragSelection = new List<GameObject>();
                                selectionStart = Event.current.mousePosition;
                                selectionRect = new Rect();
                            }

                            selectionRect = new Rect() {
                                xMin = Mathf.Min(Event.current.mousePosition.x, selectionStart.x),
                                yMin = Mathf.Min(Event.current.mousePosition.y, selectionStart.y),
                                xMax = Mathf.Max(Event.current.mousePosition.x, selectionStart.x),
                                yMax = Mathf.Max(Event.current.mousePosition.y, selectionStart.y)
                            };

                            if(Event.current.control)
                                dragSelection.AddRange(Selection.gameObjects);

                            Selection.objects = dragSelection.ToArray();
                            Event.current.Use();
                            break;

                        case EventType.MouseUp:
                            if(dragSelection != null)
                                Event.current.Use();
                            dragSelection = null;
                            break;
                    }

                if(dragSelection != null && isGameObject)
                    if(dragSelection.Contains(currentGameObject) && !selectionRect.Overlaps(rect))
                        dragSelection.Remove(currentGameObject);
                    else if(!dragSelection.Contains(currentGameObject) && selectionRect.Overlaps(rect))
                        dragSelection.Add(currentGameObject);
            }
        }
    }
}