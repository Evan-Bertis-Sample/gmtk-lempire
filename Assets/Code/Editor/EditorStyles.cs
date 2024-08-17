using UnityEngine;
using UnityEditor;
using System;

namespace CurlyEditor
{
    public static class CurlyEditorStyles
    {
        private static GUIStyle _lightBoxStyle;
        private static GUIStyle _clearBoxStyle;
        private static GUIStyle _titleStyle;
        private static GUIStyle _messageStyle;

        public static GUIStyle LightBoxStyle
        {
            get
            {
                // Lazy initialization to ensure it's set up when needed
                if (_lightBoxStyle == null || _lightBoxStyle.normal.background == null)
                {
                    _lightBoxStyle = new GUIStyle(GUI.skin.box);
                    _lightBoxStyle.normal.background = MakeTex(2, 2, new Color(0.4f, 0.4f, 0.4f, 0.4f));
                }
                return _lightBoxStyle;
            }
        }

        public static GUIStyle ClearBoxStyle
        {
            get
            {
                // Lazy initialization to ensure it's set up when needed
                if (_clearBoxStyle == null || _clearBoxStyle.normal.background == null)
                {
                    _clearBoxStyle = new GUIStyle(GUI.skin.box);
                    _clearBoxStyle.normal.background = MakeTex(2, 2, new Color(0.5f, 0.5f, 0.5f, 0.0f));
                }
                return _clearBoxStyle;
            }
        }

        public static GUIStyle TitleStyle
        {
            get
            {
                // Lazy initialization
                if (_titleStyle == null)
                {
                    _titleStyle = new GUIStyle(GUI.skin.label)
                    {
                        fontSize = 24,
                        fontStyle = FontStyle.Bold,
                        alignment = TextAnchor.MiddleCenter
                    };
                }
                return _titleStyle;
            }
        }

        public static GUIStyle MessageStyle
        {
            get
            {
                // Lazy initialization
                if (_messageStyle == null)
                {
                    _messageStyle = new GUIStyle(GUI.skin.label)
                    {
                        fontSize = 16,
                        alignment = TextAnchor.MiddleCenter,
                        wordWrap = true
                    };
                }
                return _messageStyle;
            }
        }

        public static GUIStyle DefaultBox => GUI.skin.box;

        public static GUIStyle CenteredLabel
        {
            get
            {
                GUIStyle style = new GUIStyle(GUI.skin.label)
                {
                    alignment = TextAnchor.MiddleCenter
                };
                return style;
            }
        }

        public static GUIStyle BoldLabel
        {
            get
            {
                GUIStyle style = new GUIStyle(GUI.skin.label)
                {
                    fontStyle = FontStyle.Bold
                };
                return style;
            }
        }

        public static bool BetterDropDownHeader(bool show, System.Action drawButtonContent)
        {
            EditorGUILayout.BeginHorizontal(LightBoxStyle);
            drawButtonContent();
            EditorGUILayout.EndHorizontal();
            Rect rect = GUILayoutUtility.GetLastRect();
            if (GUI.Button(rect, "", ClearBoxStyle))
            {
                show = !show;
            }
            return show;
        }

        public static bool BetterDropDownHeader(bool show, string label, bool bold = false)
        {
            return BetterDropDownHeader(show, () =>
            {
                EditorGUILayout.BeginHorizontal();
                EditorGUILayout.LabelField(label, bold ? BoldLabel : null);
                GUILayout.FlexibleSpace();

                // if not show, down arrow, else up arrow
                string arrow = !show ? "▼" : "▲";
                EditorGUILayout.LabelField(arrow, CenteredLabel, GUILayout.Width(20));

                EditorGUILayout.EndHorizontal();

                Rect rect = GUILayoutUtility.GetLastRect();

            });
        }

        // Utility method to create a Texture2D with a given color
        private static Texture2D MakeTex(int width, int height, Color col)
        {
            Color[] pix = new Color[width * height];
            for (int i = 0; i < pix.Length; i++)
                pix[i] = col;

            Texture2D result = new Texture2D(width, height);
            result.SetPixels(pix);
            result.Apply();
            return result;
        }
    }
}
