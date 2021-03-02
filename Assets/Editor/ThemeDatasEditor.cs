using System.Collections.Generic;
using System.Linq;
using UnityEditor;
using UnityEditorInternal;
using UnityEngine;

namespace JingYe.SelfIntro
{
    [CustomEditor(typeof(ThemeDatas))]
    public class ThemeDatasEditor : Editor
    {
        private void OnEnable()
        {
            m_ThemeList = serializedObject.FindProperty("m_ThemeList");
        }

        public override void OnInspectorGUI()
        {
            if (m_ThemeList == null) return;

            serializedObject.Update();
            // -----------------------------------------

            if (GUILayout.Button("Add new theme"))
            {
                // Add new element
                ++m_ThemeList.arraySize;
            }
            EditorGUILayout.Space();

            List<int> delThemeQueue = new List<int>();
            for (int i = 0; i < m_ThemeList.arraySize; ++i)
            {
                var theme = m_ThemeList.GetArrayElementAtIndex(i);
                _DrawTheme(theme, i, delThemeQueue);
                EditorGUILayout.Space();
            }

            foreach (int i in delThemeQueue.Distinct().OrderByDescending(i => i))
            {
                m_ThemeList.DeleteArrayElementAtIndex(i);
            }
            // -----------------------------------------
            serializedObject.ApplyModifiedProperties();
        }

        private void _DrawTheme(SerializedProperty theme, int themeIndex, IList<int> delQueue)
        {
            if (theme == null) return;

            var name = theme.FindPropertyRelative("Name");
            var background = theme.FindPropertyRelative("Background");
            var clips = theme.FindPropertyRelative("Clips");
            var foldoutClips = theme.FindPropertyRelative("FoldoutClips");
            
            EditorGUILayout.BeginVertical(GUI.skin.box);
            {
                EditorGUILayout.BeginHorizontal();
                {
                    EditorGUILayout.PropertyField(name);
                    if (GUILayout.Button("X", GUILayout.MaxWidth(40)))
                    {
                        delQueue.Add(themeIndex);
                    }
                }
                EditorGUILayout.EndHorizontal();

                EditorGUILayout.PropertyField(background);
                EditorGUILayout.Space();
                if (GUILayout.Button("Add new clip"))
                {
                    // Add new element
                    ++clips.arraySize;
                    foldoutClips.boolValue = true;
                }

                ++EditorGUI.indentLevel;
                foldoutClips.boolValue = EditorGUILayout.Foldout(foldoutClips.boolValue, "Clips");
                if (foldoutClips.boolValue)
                {
                    List<int> delClipQueue = new List<int>();
                    if (!m_ThemeClipReorderableList.TryGetValue(themeIndex, out ReorderableList reorderableList))
                    {
                        reorderableList = new ReorderableList(serializedObject, clips, true, false, true, false)
                        {
                            elementHeightCallback = (index) => _ThemeClipElementHeightCallback(clips, index),
                            drawElementCallback = (rect, index, isActive, isFocused) => _DrawThemeClipElementCallback(themeIndex, clips, rect, index, isActive, isFocused),
                        };
                        m_ThemeClipReorderableList.Add(themeIndex, reorderableList);
                    }

                    // Draw
                    reorderableList.DoLayoutList();

                    // Delete
                    if (m_ThemeClipRemoveQueue.TryGetValue(themeIndex, out List<int> queue))
                    {
                        foreach (int i in queue.Distinct().OrderByDescending(i => i))
                        {
                            clips.DeleteArrayElementAtIndex(i);
                        }
                        m_ThemeClipRemoveQueue.Remove(themeIndex);
                    }
                }
                --EditorGUI.indentLevel;
            }
            EditorGUILayout.EndVertical();
        }

        private float _ThemeClipElementHeightCallback(SerializedProperty clips, int index)
        {
            var action = clips.GetArrayElementAtIndex(index).FindPropertyRelative("Action");
            var clipAction = _GetClipAction(action);

            int lines = 2; // title
            if ((ThemeClip.ClipAction.ChangeDialog & clipAction) > 0)
                lines += 1;
            if ((ThemeClip.ClipAction.ChangeDescription & clipAction) > 0)
                lines += 1;
            if ((ThemeClip.ClipAction.AvatarLevelUp & clipAction) > 0)
                lines += 0;
            if ((ThemeClip.ClipAction.NextTheme & clipAction) > 0)
                lines += 1;
            if (0 != clipAction)
                lines += 4;

            return lines * (EditorGUIUtility.singleLineHeight + EditorGUIUtility.standardVerticalSpacing);
        }

        private void _DrawThemeClipElementCallback(int themeIndex, SerializedProperty clips, Rect rect, int index, bool isActive, bool isFocused)
        {
            // Calc rect
            var nameRect = new Rect(rect.x + padding
                , rect.y + EditorGUIUtility.standardVerticalSpacing
                , (rect.width - 2 * (padding + spaceing) - delButton_Width) / 2
                , EditorGUIUtility.singleLineHeight);

            var valueRect = nameRect;
            valueRect.x += nameRect.width + spaceing;

            var delRect = valueRect;
            delRect.x += valueRect.width + spaceing;
            delRect.width = delButton_Width;

            // Get clip
            var themeClip = clips.GetArrayElementAtIndex(index);
            if (themeClip == null) return;

            // Find properties
            var action = themeClip.FindPropertyRelative("Action");
            var dialogText = themeClip.FindPropertyRelative("DialogText");
            var descriptionText = themeClip.FindPropertyRelative("DescriptionText");
            var nextThemeName = themeClip.FindPropertyRelative("NextThemeName");
            var delayStartSeconds = themeClip.FindPropertyRelative("DelayStartSeconds");
            var delayEndSeconds = themeClip.FindPropertyRelative("DelayEndSeconds");
            var autoPlayNext = themeClip.FindPropertyRelative("AutoPlayNext");
            var allowSkip = themeClip.FindPropertyRelative("AllowSkip");
            
            // Title
            EditorGUI.LabelField(nameRect, $"[{index}]");
            if (GUI.Button(delRect, "X"))
            {
                if (!m_ThemeClipRemoveQueue.TryGetValue(themeIndex, out List<int> removeQueue))
                {
                    removeQueue = new List<int>();
                    m_ThemeClipRemoveQueue.Add(themeIndex, removeQueue);
                }
                removeQueue.Add(index);
            }
            _ShiftLine(1, ref nameRect);
            _ShiftLine(1, ref valueRect);

            // Action
            EditorGUI.LabelField(nameRect, action.displayName);
            EditorGUI.PropertyField(valueRect, action, GUIContent.none);
            _ShiftLine(1, ref nameRect);
            _ShiftLine(1, ref valueRect);

            // Draw properties
            var clipAction = _GetClipAction(action);
            if ((ThemeClip.ClipAction.ChangeDialog & clipAction) > 0)
            {
                EditorGUI.LabelField(nameRect, dialogText.displayName);
                EditorGUI.PropertyField(valueRect, dialogText, GUIContent.none);
                _ShiftLine(1, ref nameRect);
                _ShiftLine(1, ref valueRect);
            }

            if ((ThemeClip.ClipAction.ChangeDescription & clipAction) > 0)
            {
                EditorGUI.LabelField(nameRect, descriptionText.displayName);
                EditorGUI.PropertyField(valueRect, descriptionText, GUIContent.none);
                _ShiftLine(1, ref nameRect);
                _ShiftLine(1, ref valueRect);
            }

            if ((ThemeClip.ClipAction.AvatarLevelUp & clipAction) > 0) { /*skip*/ }

            if ((ThemeClip.ClipAction.NextTheme & clipAction) > 0)
            {
                EditorGUI.LabelField(nameRect, nextThemeName.displayName);
                EditorGUI.PropertyField(valueRect, nextThemeName, GUIContent.none);
                _ShiftLine(1, ref nameRect);
                _ShiftLine(1, ref valueRect);
            }

            if (0 != clipAction)
            {
                EditorGUI.LabelField(nameRect, delayStartSeconds.displayName);
                EditorGUI.PropertyField(valueRect, delayStartSeconds, GUIContent.none);
                _ShiftLine(1, ref nameRect);
                _ShiftLine(1, ref valueRect);

                EditorGUI.LabelField(nameRect, delayEndSeconds.displayName);
                EditorGUI.PropertyField(valueRect, delayEndSeconds, GUIContent.none);
                _ShiftLine(1, ref nameRect);
                _ShiftLine(1, ref valueRect);

                EditorGUI.LabelField(nameRect, autoPlayNext.displayName);
                EditorGUI.PropertyField(valueRect, autoPlayNext, GUIContent.none);
                _ShiftLine(1, ref nameRect);
                _ShiftLine(1, ref valueRect);

                EditorGUI.LabelField(nameRect, allowSkip.displayName);
                EditorGUI.PropertyField(valueRect, allowSkip, GUIContent.none);
                _ShiftLine(1, ref nameRect);
                _ShiftLine(1, ref valueRect);
            }
        }

        private void _DrawThemeClip(SerializedProperty themeClip, int index, IList<int> delQueue)
        {
            if (themeClip == null) return;

            var action = themeClip.FindPropertyRelative("Action");
            var dialogText = themeClip.FindPropertyRelative("DialogText");
            var descriptionText = themeClip.FindPropertyRelative("DescriptionText");
            var nextThemeName = themeClip.FindPropertyRelative("NextThemeName");
            var delayStartSeconds = themeClip.FindPropertyRelative("DelayStartSeconds");
            var delayEndSeconds = themeClip.FindPropertyRelative("DelayEndSeconds");
            var allowSkip = themeClip.FindPropertyRelative("AllowSkip");

            EditorGUILayout.BeginHorizontal();
            {
                EditorGUILayout.LabelField($"[{index}]");
                if (GUILayout.Button("X", GUILayout.MaxWidth(40)))
                {
                    delQueue.Add(index);
                }
            }
            EditorGUILayout.EndHorizontal();

            ++EditorGUI.indentLevel;
            EditorGUILayout.BeginVertical();
            {
                var clipAction = _GetClipAction(action);

                EditorGUILayout.PropertyField(action);

                if ((ThemeClip.ClipAction.ChangeDialog & clipAction) > 0)
                    EditorGUILayout.PropertyField(dialogText);

                if ((ThemeClip.ClipAction.ChangeDescription & clipAction) > 0)
                    EditorGUILayout.PropertyField(descriptionText);

                if ((ThemeClip.ClipAction.AvatarLevelUp & clipAction) > 0) { /*skip*/ }

                if ((ThemeClip.ClipAction.NextTheme & clipAction) > 0)
                    EditorGUILayout.PropertyField(nextThemeName);

                if (0 != clipAction)
                {
                    EditorGUILayout.PropertyField(delayStartSeconds);
                    EditorGUILayout.PropertyField(delayEndSeconds);
                    EditorGUILayout.PropertyField(allowSkip);
                }
            }
            EditorGUILayout.EndVertical();
            --EditorGUI.indentLevel;
        }

        private ThemeClip.ClipAction _GetClipAction(SerializedProperty action)
        {
            return (ThemeClip.ClipAction)action.intValue;
        }

        private void _ShiftLine(int line, ref Rect rect)
        {
            rect.y += line * (EditorGUIUtility.singleLineHeight + EditorGUIUtility.standardVerticalSpacing);
        }

        // Params
        private readonly float padding = 5f;
        private readonly float spaceing = 5f;
        private readonly float delButton_Width = 30f;

        private SerializedProperty m_ThemeList;
        private readonly Dictionary<int/*theme index*/, ReorderableList> m_ThemeClipReorderableList = new Dictionary<int, ReorderableList>();
        private readonly Dictionary<int/*theme index*/, List<int>> m_ThemeClipRemoveQueue = new Dictionary<int, List<int>>();
    } // END class
} // END namespace
