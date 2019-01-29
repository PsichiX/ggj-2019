using UnityEditor;
using UnityEngine;

namespace GaryMoveOut.Items
{
    [CustomEditor(typeof(ItemsDatabase))]
    public class ItemsDatabaseEditor : Editor
    {
        [SerializeField] private bool editMode = false;

        public override void OnInspectorGUI()
        {
            serializedObject.Update();

            //editMode = EditorGUILayout.BeginToggleGroup("Edit mode", editMode);
            //{
            //    if (GUILayout.Button("Refresh item schemes"))
            //    {
            //        var target = serializedObject.targetObject as ItemsDatabase;
            //        target?.RefreshDatabase();
            //    }
            //    if (GUILayout.Button("Generate item schemes"))
            //    {
            //        var target = serializedObject.targetObject as ItemsDatabase;
            //        target?.GenerateItemSchemes();
            //    }
            //}
            //EditorGUILayout.EndToggleGroup();

            base.OnInspectorGUI();

            serializedObject.ApplyModifiedProperties();
        }
    }
}