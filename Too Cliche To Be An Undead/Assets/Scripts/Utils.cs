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

        public static class VectorMaths
        {

            public static bool Vector2ApproximatlyEquals(Vector2 a, Vector2 b, float approx)
            {
                if (b.x <= a.x + approx && b.y <= a.y + approx &&
                    b.y >= a.y - approx && b.y >= a.y - approx) return true;

                return false;
            }

            public static Vector2 Truncate(Vector2 original, float max)
            {
                if (original.magnitude > max)
                {
                    original.Normalize();

                    original *= max;

                }
                
                return original;
            }
            public static void Truncate(ref Vector2 original, float max)
            {
                if (original.magnitude > max)
                {
                    original.Normalize();

                    original *= max;
                }
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

    namespace RenderersUtils
    {
        public static class RendererExtensions
        {
            /// <summary>
            /// Counts the bounding box corners of the given RectTransform that are visible from the given Camera in screen space.
            /// </summary>
            /// <returns>The amount of bounding box corners that are visible from the Camera.</returns>
            /// <param name="rectTransform">Rect transform.</param>
            /// <param name="camera">Camera.</param>
            private static int CountCornersVisibleFrom(this RectTransform rectTransform, Camera camera)
            {
                Rect screenBounds = new Rect(0f, 0f, Screen.width, Screen.height); // Screen space bounds (assumes camera renders across the entire screen)
                Vector3[] objectCorners = new Vector3[4];
                rectTransform.GetWorldCorners(objectCorners);

                int visibleCorners = 0;
                Vector3 tempScreenSpaceCorner; // Cached
                for (var i = 0; i < objectCorners.Length; i++) // For each corner in rectTransform
                {
                    tempScreenSpaceCorner = camera.WorldToScreenPoint(objectCorners[i]); // Transform world space position of corner to screen space
                    if (screenBounds.Contains(tempScreenSpaceCorner)) // If the corner is inside the screen
                    {
                        visibleCorners++;
                    }
                }
                return visibleCorners;
            }

            /// <summary>
            /// Determines if this RectTransform is fully visible from the specified camera.
            /// Works by checking if each bounding box corner of this RectTransform is inside the cameras screen space view frustrum.
            /// </summary>
            /// <returns><c>true</c> if is fully visible from the specified camera; otherwise, <c>false</c>.</returns>
            /// <param name="rectTransform">Rect transform.</param>
            /// <param name="camera">Camera.</param>
            public static bool IsFullyVisibleFrom(this RectTransform rectTransform, Camera camera)
            {
                return CountCornersVisibleFrom(rectTransform, camera) == 4; // True if all 4 corners are visible
            }

            /// <summary>
            /// Determines if this RectTransform is at least partially visible from the specified camera.
            /// Works by checking if any bounding box corner of this RectTransform is inside the cameras screen space view frustrum.
            /// </summary>
            /// <returns><c>true</c> if is at least partially visible from the specified camera; otherwise, <c>false</c>.</returns>
            /// <param name="rectTransform">Rect transform.</param>
            /// <param name="camera">Camera.</param>
            public static bool IsVisibleFrom(this RectTransform rectTransform, Camera camera)
            {
                return CountCornersVisibleFrom(rectTransform, camera) > 0; // True if any corners are visible
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

            public static void GameObjectField(string label, ref GameObject gO, bool allowSceneObjects = true)
            {
                gO = (GameObject)EditorGUILayout.ObjectField(label, gO, typeof(GameObject), allowSceneObjects);
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

            public static void EditorScriptDraw(Type scriptType, ScriptableObject sO, bool allowSceneObject = false)
            {
                GUI.enabled = false;
                EditorGUILayout.ObjectField("Editor Script", MonoScript.FromScriptableObject(sO), scriptType, allowSceneObject);
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

        public static class OpenWindow
        {
            private const string _inspectorWindowTypeName = "UnityEditor.InspectorWindow";
            private const string _browserWindowTypeName = "UnityEditor.ProjectBrowser";
            private const string _hierarchyWindowTypeName = "UnityEditor.SceneHierarchyWindow";
            private const string _gameViewWindowTypeName = "UnityEditor.GameView";
            private const string _consoleViewWindowTypeName = "UnityEditor.ConsoleWindow";
            private const string _sceneWindowTypeName = "UnityEditor.SceneWindow";

            public static void OpenCurrentInspectorInNewWindow(bool locked) => OpenInNewWindow(_inspectorWindowTypeName, locked);
            public static void OpenCurrentBrowserInNewWindow(bool locked) => OpenInNewWindow(_browserWindowTypeName, locked);
            public static void OpenCurrentHierarchyInNewWindow(bool locked) => OpenInNewWindow(_hierarchyWindowTypeName, locked);
            public static void OpenCurrentGameViewInNewWindow() => OpenInNewWindow(_gameViewWindowTypeName);
            public static void OpenCurrentConsoleInNewWindow() => OpenInNewWindow(_consoleViewWindowTypeName);
            public static void OpenCurrentSceneInNewWindow() => OpenInNewWindow(_sceneWindowTypeName);

            public static void OpenInNewWindow(string windowTypeName)
            {
                var inspectorWindowType = typeof(Editor).Assembly.GetType(windowTypeName);

                System.Reflection.MethodInfo method = typeof(EditorWindow).GetMethod(nameof(EditorWindow.CreateWindow), new[] { typeof(Type[]) });
                System.Reflection.MethodInfo generic = method.MakeGenericMethod(inspectorWindowType);

                generic.Invoke(null, new object[] { new Type[] { } });
            }
            public static void OpenInNewWindow(string windowTypeName, bool locked)
            {
                OpenInNewWindow(windowTypeName);
                ActiveEditorTracker.sharedTracker.isLocked = locked;
            }
        }

        public static class GUIDraw
        {
            public static void DrawStringGUI(string text, Vector3 worldPosition, Color textColor, Vector2 anchor, float textSize = 15f)
            {
                var view = SceneView.currentDrawingSceneView;
                if (!view)
                    return;
                Vector3 screenPosition = view.camera.WorldToScreenPoint(worldPosition);
                if (screenPosition.y < 0 || screenPosition.y > view.camera.pixelHeight || screenPosition.x < 0 || screenPosition.x > view.camera.pixelWidth || screenPosition.z < 0)
                    return;
                var pixelRatio = HandleUtility.GUIPointToScreenPixelCoordinate(Vector2.right).x - HandleUtility.GUIPointToScreenPixelCoordinate(Vector2.zero).x;
                Handles.BeginGUI();
                var style = new GUIStyle(GUI.skin.label)
                {
                    fontSize = (int)textSize,
                    normal = new GUIStyleState() { textColor = textColor }
                };
                Vector2 size = style.CalcSize(new GUIContent(text)) * pixelRatio;
                var alignedPosition =
                    ((Vector2)screenPosition +
                    size * ((anchor + Vector2.left + Vector2.up) / 2f)) * (Vector2.right + Vector2.down) +
                    Vector2.up * view.camera.pixelHeight;
                GUI.Label(new Rect(alignedPosition / pixelRatio, size / pixelRatio), text, style);
                Handles.EndGUI();
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
                return (int)(1.0 / Time.unscaledDeltaTime);
            }
            public static double GetFPS()
            {
                return (1.0 / Time.unscaledDeltaTime);
            }
        }
    }

}

