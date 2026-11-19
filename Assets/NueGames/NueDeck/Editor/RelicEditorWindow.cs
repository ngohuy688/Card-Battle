using System.Collections.Generic;
using System.Linq;
using NueGames.NueDeck.Scripts.Data.Collection;
using UnityEditor;
using UnityEditor.Callbacks;
using UnityEngine;

namespace NueGames.NueDeck.Editor
{
    public class RelicEditorWindow : ExtendedEditorWindow
    {
        private const string RelicDataDefaultPath = "Assets/NueGames/NueDeck/Data/Relics/";
        private static RelicData CachedRelicData { get; set; }

        private List<RelicData> _allRelicData = new List<RelicData>();
        private RelicData _selectedRelicData;
        private SerializedObject _serializedObject;
        private Vector2 _listScrollPosition;
        private Vector2 _detailsScrollPosition;

        [MenuItem("Tools/NueDeck/Relic Editor")]
        public static void OpenRelicEditor()
        {
            GetWindow<RelicEditorWindow>("Relic Editor");
        }

        public static void OpenRelicEditor(RelicData targetData)
        {
            CachedRelicData = targetData;
            OpenRelicEditor();
        }

        private void OnEnable()
        {
            RefreshRelicData();
            if (CachedRelicData)
                SelectRelic(CachedRelicData);
            Selection.selectionChanged += Repaint;
        }

        private void OnDisable()
        {
            Selection.selectionChanged -= Repaint;
            CachedRelicData = null;
        }

        private void OnGUI()
        {
            EditorGUILayout.BeginHorizontal();
            DrawRelicList();
            DrawSelectedRelic();
            EditorGUILayout.EndHorizontal();
        }

        private void DrawRelicList()
        {
            EditorGUILayout.BeginVertical("box", GUILayout.Width(190), GUILayout.ExpandHeight(true));
            EditorGUILayout.BeginHorizontal();
            EditorGUILayout.LabelField("Relics", EditorStyles.boldLabel);
            if (GUILayout.Button("Refresh", GUILayout.Width(65)))
                RefreshRelicData();
            EditorGUILayout.EndHorizontal();

            _listScrollPosition = EditorGUILayout.BeginScrollView(_listScrollPosition);
            foreach (var relic in _allRelicData)
            {
                if (relic && GUILayout.Button(string.IsNullOrEmpty(relic.RelicName) ? relic.name : relic.RelicName))
                    SelectRelic(relic);
            }
            EditorGUILayout.EndScrollView();

            if (GUILayout.Button("+ New Relic"))
                CreateNewRelic();
            EditorGUILayout.EndVertical();
        }

        private void DrawSelectedRelic()
        {
            EditorGUILayout.BeginVertical("box", GUILayout.ExpandHeight(true));
            if (!_selectedRelicData || _serializedObject == null)
            {
                EditorGUILayout.LabelField("Select a relic or create a new one.");
                EditorGUILayout.EndVertical();
                return;
            }

            _serializedObject.Update();
            _detailsScrollPosition = EditorGUILayout.BeginScrollView(_detailsScrollPosition);
            EditorGUILayout.PropertyField(_serializedObject.FindProperty("relicId"), new GUIContent("Relic ID"));
            EditorGUILayout.PropertyField(_serializedObject.FindProperty("relicName"), new GUIContent("Relic Name"));
            EditorGUILayout.PropertyField(_serializedObject.FindProperty("relicSprite"), new GUIContent("Relic Sprite"));
            EditorGUILayout.PropertyField(_serializedObject.FindProperty("relicDescription"), new GUIContent("Description"));
            EditorGUILayout.PropertyField(_serializedObject.FindProperty("isUnique"), new GUIContent("Unique"));
            DrawEffects(_serializedObject.FindProperty("relicEffectDatas"));
            EditorGUILayout.EndScrollView();

            _serializedObject.ApplyModifiedProperties();
            EditorGUILayout.BeginHorizontal();
            GUILayout.FlexibleSpace();
            if (GUILayout.Button("Save", GUILayout.Width(100), GUILayout.Height(30)))
            {
                EditorUtility.SetDirty(_selectedRelicData);
                AssetDatabase.SaveAssets();
            }
            EditorGUILayout.EndHorizontal();
            EditorGUILayout.EndVertical();
        }

        private static void DrawEffects(SerializedProperty effects)
        {
            if (effects == null)
                return;

            EditorGUILayout.Space();
            EditorGUILayout.LabelField("Effects", EditorStyles.boldLabel);
            for (var i = 0; i < effects.arraySize; i++)
            {
                var effect = effects.GetArrayElementAtIndex(i);
                EditorGUILayout.BeginVertical("box");
                EditorGUILayout.BeginHorizontal();
                EditorGUILayout.LabelField("Effect " + (i + 1), EditorStyles.boldLabel);
                if (GUILayout.Button("Remove", GUILayout.Width(65)))
                {
                    effects.DeleteArrayElementAtIndex(i);
                    EditorGUILayout.EndHorizontal();
                    EditorGUILayout.EndVertical();
                    break;
                }
                EditorGUILayout.EndHorizontal();
                EditorGUILayout.PropertyField(effect.FindPropertyRelative("effectType"), new GUIContent("Effect Type"));
                EditorGUILayout.PropertyField(effect.FindPropertyRelative("conditionType"), new GUIContent("Condition"));
                EditorGUILayout.PropertyField(effect.FindPropertyRelative("targetType"), new GUIContent("Target"));
                EditorGUILayout.PropertyField(effect.FindPropertyRelative("effectValue"), new GUIContent("Value"));
                EditorGUILayout.EndVertical();
            }

            if (GUILayout.Button("+ Add Effect"))
                effects.InsertArrayElementAtIndex(effects.arraySize);
        }

        private void RefreshRelicData()
        {
            _allRelicData = AssetDatabase.FindAssets("t:RelicData")
                .Select(AssetDatabase.GUIDToAssetPath)
                .Select(AssetDatabase.LoadAssetAtPath<RelicData>)
                .Where(relic => relic)
                .OrderBy(relic => relic.RelicName)
                .ToList();
        }

        private void SelectRelic(RelicData relic)
        {
            _selectedRelicData = relic;
            _serializedObject = relic ? new SerializedObject(relic) : null;
        }

        private void CreateNewRelic()
        {
            EnsureRelicFolderExists();
            var relic = CreateInstance<RelicData>();
            var serializedRelic = new SerializedObject(relic);
            serializedRelic.FindProperty("relicId").stringValue = "new_relic";
            serializedRelic.FindProperty("relicName").stringValue = "New Relic";
            serializedRelic.ApplyModifiedPropertiesWithoutUndo();

            var path = AssetDatabase.GenerateUniqueAssetPath(RelicDataDefaultPath + "New Relic.asset");
            AssetDatabase.CreateAsset(relic, path);
            AssetDatabase.SaveAssets();
            RefreshRelicData();
            SelectRelic(relic);
        }

        private static void EnsureRelicFolderExists()
        {
            const string dataFolder = "Assets/NueGames/NueDeck/Data";
            if (!AssetDatabase.IsValidFolder(dataFolder))
                AssetDatabase.CreateFolder("Assets/NueGames/NueDeck", "Data");
            if (!AssetDatabase.IsValidFolder(RelicDataDefaultPath.TrimEnd('/')))
                AssetDatabase.CreateFolder(dataFolder, "Relics");
        }
    }

    public class RelicDataAssetHandler
    {
        [OnOpenAsset]
        public static bool OpenEditor(int instanceId, int line)
        {
            var relic = EditorUtility.InstanceIDToObject(instanceId) as RelicData;
            if (!relic)
                return false;

            RelicEditorWindow.OpenRelicEditor(relic);
            return true;
        }
    }

    [CustomEditor(typeof(RelicData))]
    public class RelicDataCustomEditor : UnityEditor.Editor
    {
        public override void OnInspectorGUI()
        {
            DrawDefaultInspector();
            if (GUILayout.Button("Open in editor"))
                RelicEditorWindow.OpenRelicEditor((RelicData)target);
        }
    }
}
