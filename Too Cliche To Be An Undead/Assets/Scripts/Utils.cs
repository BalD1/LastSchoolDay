using UnityEngine;
using System.Collections.Generic;
using System;

#if UNITY_EDITOR
using UnityEditor;
#endif

namespace BalDUtilities
{

    namespace VectorUtils
    {

        public static class VectorClamps
        {

            public static Vector2 ClampVector2(Vector2 vector, float min, float max)
            {
                // X
                vector.x = Mathf.Clamp(vector.x, min, max);

                // Y
                vector.y = Mathf.Clamp(vector.y, min, max);

                return vector;
            }

            public static Vector2 ClampVector2(Vector2 vector, Vector2 min, Vector2 max)
            {
                // X
                vector.x = Mathf.Clamp(vector.x, min.x, max.x);

                // Y
                vector.y = Mathf.Clamp(vector.y, min.y, max.y);

                return vector;
            }

            public static Vector3 ClampVector3(Vector3 vector, float min, float max)
            {
                float vecZ = vector.z;

                // X & Y
                vector = ClampVector2(vector, min, max);

                // Z
                vector.z = Mathf.Clamp(vecZ, min, max);

                return vector;
            }

            public static Vector3 ClampVector3(Vector3 vector, Vector3 min, Vector3 max)
            {
                float vecZ = vector.z;

                // X & Y
                vector = ClampVector2(vector, min, max);

                // Z
                vector.z = Mathf.Clamp(vecZ, min.z, max.z);

                return vector;
            }
        }
    }

    namespace MouseUtils
    {
        public static class MousePosition
        {
            public static Vector3 GetMouseWorldPosition()
            {
                Vector3 pos = GetMouseWorldPositionWithZ();
                pos.z = 0;
                return pos;
            }
            public static Vector3 GetMouseWorldPosition(Camera cam)
            {
                Vector3 pos = GetMouseWorldPositionWithZ(cam);
                pos.z = 0;
                return pos;
            }
            public static void GetMouseWorldPosition(out Vector3 vector)
            {
                Vector3 pos;
                GetMouseWorldPositionWithZ(out pos);
                pos.z = 0;
                vector = pos;
            }
            public static void GetMouseWorldPosition(out Vector3 vector, Camera cam)
            {
                Vector3 pos;
                GetMouseWorldPositionWithZ(out pos, cam);
                pos.z = 0;
                vector = pos;
            }

            public static Vector3 GetMouseWorldPositionWithZ()
            {
                return Camera.main.ScreenToWorldPoint(Input.mousePosition);
            }
            public static Vector3 GetMouseWorldPositionWithZ(Camera cam)
            {
                return cam.ScreenToWorldPoint(Input.mousePosition);
            }
            public static void GetMouseWorldPositionWithZ(out Vector3 vector)
            {
                vector = Camera.main.ScreenToWorldPoint(Input.mousePosition);
            }
            public static void GetMouseWorldPositionWithZ(out Vector3 vector, Camera cam)
            {
                vector = cam.ScreenToWorldPoint(Input.mousePosition);
            }
        }
    }

    namespace CreateUtils
    {
        public static class CreateText
        {
            public static TextMesh CreateWorldText(string _text, Vector3 _localPosition, int _fontSize, Color _color, TextAnchor textAnchor, TextAlignment textAlignment, int _sortingOrder)
            {
                GameObject gO = new GameObject("World_Text", typeof(TextMesh));
                Transform transform = gO.transform;
                transform.localPosition = _localPosition;
                TextMesh textMesh = gO.GetComponent<TextMesh>();
                textMesh.anchor = textAnchor;
                textMesh.alignment = textAlignment;
                textMesh.text = _text;
                textMesh.fontSize = _fontSize;
                textMesh.color = _color;
                textMesh.GetComponent<MeshRenderer>().sortingOrder = _sortingOrder;

                return textMesh;
            }
        }
    }

#if UNITY_EDITOR
    namespace EditorUtils
    {
        public static class SimpleDraws
        {
            public static void HorizontalLine()
            {
                GUIStyle horizontalLine;
                horizontalLine = new GUIStyle();
                horizontalLine.normal.background = EditorGUIUtility.whiteTexture;
                horizontalLine.margin = new RectOffset(0, 0, 4, 4);
                horizontalLine.fixedHeight = 1;

                var c = GUI.color;
                GUI.color = Color.grey;
                GUILayout.Box(GUIContent.none, horizontalLine);
                GUI.color = c;
            }

            public static void HorizontalLine(Color color)
            {
                GUIStyle horizontalLine;
                horizontalLine = new GUIStyle();
                horizontalLine.normal.background = EditorGUIUtility.whiteTexture;
                horizontalLine.margin = new RectOffset(0, 0, 4, 4);
                horizontalLine.fixedHeight = 1;

                var c = GUI.color;
                GUI.color = color;
                GUILayout.Box(GUIContent.none, horizontalLine);
                GUI.color = c;
            }

            public static void HorizontalLine(Color color, GUIStyle horizontalLine)
            {
                var c = GUI.color;
                GUI.color = color;
                GUILayout.Box(GUIContent.none, horizontalLine);
                GUI.color = c;
            }

            public static int DelayedIntWithLabel(string label, int value)
            {
                EditorGUILayout.BeginHorizontal();

                EditorGUILayout.LabelField(label);
                value = EditorGUILayout.DelayedIntField(value);

                EditorGUILayout.EndHorizontal();

                return value;
            }
            public static void DelayedIntWithLabel(string label, ref int value)
            {
                EditorGUILayout.BeginHorizontal();

                EditorGUILayout.LabelField(label);
                value = EditorGUILayout.DelayedIntField(value);

                EditorGUILayout.EndHorizontal();
            }

            public static float DelayedFloatWithLabel(string label, float value)
            {
                EditorGUILayout.BeginHorizontal();

                EditorGUILayout.LabelField(label);
                value = EditorGUILayout.DelayedFloatField(value);

                EditorGUILayout.EndHorizontal();

                return value;
            }
            public static void DelayedFloatWithLabel(string label, ref float value)
            {
                EditorGUILayout.BeginHorizontal();

                EditorGUILayout.LabelField(label);
                value = EditorGUILayout.DelayedFloatField(value);

                EditorGUILayout.EndHorizontal();
            }
        }

        public static class ReadOnlyDraws
        {
            public static void GameObjectDraw(GameObject go, string label = "Object", bool allowSceneObject = true)
            {
                GUI.enabled = false;
                go = (GameObject)EditorGUILayout.ObjectField(label, go, typeof(GameObject), allowSceneObject);
                GUI.enabled = true;
            }

            public static void ScriptDraw(Type scriptType, MonoBehaviour mono, bool allowSceneObject = false)
            {
                GUI.enabled = false;
                EditorGUILayout.ObjectField("Script", MonoScript.FromMonoBehaviour(mono), scriptType, false);
                GUI.enabled = true;
            }
        }

        public static class MixedDraws
        {
            public static void ListFoldoutWithSize<T>(ref bool toggle, string label, List<T> list)
            {
                FoldoutWithSize(ref toggle, label, list.Count);
            }
            public static bool ListFoldoutWithEditableSize<T>(ref bool toggle, string label, List<T> list)
            {
                EditorGUILayout.BeginHorizontal();

                bool sizeChanged = false;

                toggle = EditorGUILayout.Foldout(toggle, label);
                int currentSize = list.Count;
                int newSize = currentSize;

                if ((newSize = EditorGUILayout.DelayedIntField(newSize, GUILayout.MaxWidth(50))) != currentSize)
                {
                    ChangeListSize<T>(list, newSize);
                    sizeChanged = true;
                }

                EditorGUILayout.EndHorizontal();

                return sizeChanged;
            }

            public static void ArrayFoldoutWithSize<T>(ref bool toggle, string label, T[] array)
            {
                FoldoutWithSize(ref toggle, label, array.Length);
            }

            public static bool ArrayFoldoutWithEditableSize<T>(ref bool toggle, string label, ref T[] array)
            {
                EditorGUILayout.BeginHorizontal();

                bool sizeChanged = false;

                toggle = EditorGUILayout.Foldout(toggle, label);
                int currentSize = array.Length;
                int newSize = currentSize;

                if ((newSize = EditorGUILayout.DelayedIntField(newSize, GUILayout.MaxWidth(50))) != currentSize)
                {
                    ChangeArraySize<T>(ref array, newSize);
                    sizeChanged = true;
                }

                EditorGUILayout.EndHorizontal();

                return sizeChanged;
            }

            private static void FoldoutWithSize(ref bool toggle, string label, int size)
            {
                EditorGUILayout.BeginHorizontal();

                toggle = EditorGUILayout.Foldout(toggle, label);

                GUI.enabled = false;
                EditorGUILayout.IntField(size, GUILayout.MaxWidth(50));
                GUI.enabled = true;

                EditorGUILayout.EndHorizontal();
            }

            public static void ChangeListSize<T>(List<T> list, int newSize)
            {
                if (newSize != 0)
                {
                    int currentSize = list.Count;

                    // Add i new items
                    if (newSize > list.Count)
                    {
                        for (int i = 0; i < newSize - currentSize; i++)
                        {
                            list.Insert(list.Count, default(T));
                        }
                    }
                    // Remove i items
                    else
                    {
                        for (int i = currentSize - 1; i >= newSize; i--)
                        {
                            list.RemoveAt(i);
                        }
                    }
                }
                else
                    list.Clear();
            }

            public static void ChangeArraySize<T>(ref T[] array, int newSize)
            {
                Array.Resize(ref array, newSize);
            }
        }
    }
#endif

    namespace Misc
    {
        public static class EnumsExtension
        {
            public static string EnumToString(Enum enumName)
            {
                return Enum.GetName(enumName.GetType(), enumName);
            }
        }
        public static class FPS
        {
            public static int GetFPSRounded()
            {
                return (int)(1.0 / Time.deltaTime);
            }
            public static double GetFPS()
            {
                return (1.0 / Time.deltaTime);
            }
        }
    }

}

