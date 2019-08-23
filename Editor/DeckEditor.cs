using UnityEngine;
using UnityEditor;
using UnityEditorInternal;
using Path = System.IO.Path;

namespace Scrubber
{
    [CustomEditor(typeof(Deck))]
    class DeckEditor : Editor
    {
        ReorderableList _pageList;

        void OnEnable()
        {
            _pageList = new ReorderableList(
                serializedObject, serializedObject.FindProperty("_pages"),
                true, true, true, true
            );
            _pageList.drawHeaderCallback = DrawHeader;
            _pageList.drawElementCallback = DrawElement;
        }

        (Rect, Rect, Rect, Rect, Rect) CalculateColumnRects(Rect r)
        {
            var h = EditorGUIUtility.singleLineHeight;
            return (
                new Rect(r.x, r.y, 80, h),
                new Rect(r.x + 84, r.y, 14, h),
                new Rect(r.x + 102, r.y, 14, h),
                new Rect(r.x + 120, r.y, r.width - 120 - 104, h),
                new Rect(r.x + r.width - 100, r.y, 100, h)
            );
        }

        void DrawHeader(Rect rect)
        {
            rect.xMin += 14;
            rect.y++;
            rect.height = 16;

            var columns = CalculateColumnRects(rect);

            GUI.Label(columns.Item1, "Video Name", EditorStyles.label);
            GUI.Label(columns.Item2, "A", EditorStyles.label);
            GUI.Label(columns.Item3, "L", EditorStyles.label);
            GUI.Label(columns.Item4, "Text", EditorStyles.label);
            GUI.Label(columns.Item5, "Image", EditorStyles.label);
        }

        void DrawElement(Rect rect, int index, bool active, bool focused)
        {
            rect.y += 2;

            var columns = CalculateColumnRects(rect);

            var e = _pageList.serializedProperty.GetArrayElementAtIndex(index);

            EditorGUI.PropertyField(columns.Item1, e.FindPropertyRelative("videoName"), GUIContent.none);
            EditorGUI.PropertyField(columns.Item2, e.FindPropertyRelative("autoPlay"), GUIContent.none);
            EditorGUI.PropertyField(columns.Item3, e.FindPropertyRelative("loop"), GUIContent.none);
            EditorGUI.PropertyField(columns.Item4, e.FindPropertyRelative("text"), GUIContent.none);
            EditorGUI.PropertyField(columns.Item5, e.FindPropertyRelative("image"), GUIContent.none);
        }

        public override void OnInspectorGUI()
        {
            serializedObject.Update();

            EditorGUILayout.Space();
            _pageList.DoLayoutList();

            serializedObject.ApplyModifiedProperties();
        }

        [MenuItem("Assets/Create/Scrubber/Deck")]
        public static void CreateDeckAsset()
        {
            // Make a proper path from the current selection.
            var path = AssetDatabase.GetAssetPath(Selection.activeObject);

            if (string.IsNullOrEmpty(path))
                path = "Assets";
            else if (Path.GetExtension(path) != "")
                path = path.Replace(Path.GetFileName(path), "");

            var assetPathName = AssetDatabase.GenerateUniqueAssetPath
                (path + "/Deck.asset");

            // Create a deck asset.
            var asset = ScriptableObject.CreateInstance<Deck>();
            AssetDatabase.CreateAsset(asset, assetPathName);
            AssetDatabase.SaveAssets();

            // Tweak the selection.
            EditorUtility.FocusProjectWindow();
            Selection.activeObject = asset;
        }
    }
}
