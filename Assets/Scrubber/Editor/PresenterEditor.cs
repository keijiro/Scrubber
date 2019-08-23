using UnityEngine;
using UnityEditor;

namespace Scrubber
{
    [CustomEditor(typeof(Presenter))]
    class PresenterEditor : Editor
    {
        SerializedProperty _deck;

        void OnEnable()
        {
            _deck = serializedObject.FindProperty("_deck");
        }

        public override void OnInspectorGUI()
        {
            serializedObject.Update();

            EditorGUILayout.PropertyField(_deck);

            serializedObject.ApplyModifiedProperties();
        }

        [MenuItem("GameObject/Scrubber/Presenter", false, 10)]
        static void CreatePresenter()
        {
            var path1 = "Packages/jp.keijiro.scrubber/Runtime/Presenter.prefab";
            var path2 = "Assets/Scrubber/Runtime/Presenter.prefab";

            var prefab = AssetDatabase.LoadAssetAtPath<GameObject>(path1) ??
                         AssetDatabase.LoadAssetAtPath<GameObject>(path2);

            var go = PrefabUtility.InstantiatePrefab(prefab);
            Selection.activeGameObject = (GameObject)go;
        }
    }
}
