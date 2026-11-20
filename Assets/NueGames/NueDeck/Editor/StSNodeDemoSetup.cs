using System.Collections.Generic;
using NueGames.NueDeck.Scripts.Managers;
using TMPro;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

namespace NueGames.NueDeck.Editor
{
    public static class StSNodeDemoSetup
    {
        private const string CoreScenePath = "Assets/NueGames/NueDeck/Scenes/NueCore.unity";

        [MenuItem("Tools/NueDeck/Setup StS Node Demo Canvas")]
        private static void SetupCanvas()
        {
            var scene = SceneManager.GetSceneByPath(CoreScenePath);
            if (!scene.IsValid() || !scene.isLoaded)
                scene = EditorSceneManager.OpenScene(CoreScenePath, OpenSceneMode.Single);

            var uiManager = Object.FindFirstObjectByType<UIManager>(FindObjectsInactive.Include);
            if (!uiManager)
            {
                Debug.LogError("UIManager was not found in NueCore scene.");
                return;
            }

            var existing = uiManager.GetComponentInChildren<NueGames.NueDeck.Scripts.UI.NodeEventCanvas>(true);
            if (existing)
            {
                Debug.Log("Node event canvas already exists. Reusing the existing canvas.", existing);
                AssignCanvas(uiManager, existing);
                return;
            }

            var canvasObject = new GameObject("NodeEventCanvas", typeof(RectTransform), typeof(Canvas), typeof(CanvasScaler), typeof(GraphicRaycaster));
            Undo.RegisterCreatedObjectUndo(canvasObject, "Create node event canvas");
            canvasObject.transform.SetParent(uiManager.transform, false);
            var canvas = canvasObject.GetComponent<Canvas>();
            canvas.renderMode = RenderMode.ScreenSpaceOverlay;
            var scaler = canvasObject.GetComponent<CanvasScaler>();
            scaler.uiScaleMode = CanvasScaler.ScaleMode.ScaleWithScreenSize;
            scaler.referenceResolution = new Vector2(1920, 1080);

            var panel = CreateUiObject("EventPanel", canvasObject.transform, new Vector2(760, 600), new Color(0.08f, 0.07f, 0.11f, 0.96f), true);
            var title = CreateText("Title", panel.transform, "Event", 40, new Vector2(680, 90));
            SetAnchoredPosition(title.rectTransform, new Vector2(0, 220));
            var description = CreateText("Description", panel.transform, "Event description", 26, new Vector2(680, 140));
            SetAnchoredPosition(description.rectTransform, new Vector2(0, 105));

            var choices = new GameObject("Choices", typeof(RectTransform), typeof(VerticalLayoutGroup));
            choices.transform.SetParent(panel.transform, false);
            var choicesRect = choices.GetComponent<RectTransform>();
            choicesRect.anchorMin = choicesRect.anchorMax = new Vector2(0.5f, 0.5f);
            choicesRect.anchoredPosition = new Vector2(0, -100);
            choicesRect.sizeDelta = new Vector2(660, 240);
            var layout = choices.GetComponent<VerticalLayoutGroup>();
            layout.spacing = 16;
            layout.childControlWidth = true;
            layout.childControlHeight = true;
            layout.childForceExpandWidth = true;
            layout.childForceExpandHeight = false;

            var button = CreateButtonPrefab(choices.transform);
            var eventCanvas = canvasObject.AddComponent<NueGames.NueDeck.Scripts.UI.NodeEventCanvas>();
            var serialized = new SerializedObject(eventCanvas);
            serialized.FindProperty("eventPanel").objectReferenceValue = panel;
            serialized.FindProperty("titleText").objectReferenceValue = title;
            serialized.FindProperty("descriptionText").objectReferenceValue = description;
            serialized.FindProperty("choicesRoot").objectReferenceValue = choices.transform;
            serialized.FindProperty("choiceButtonPrefab").objectReferenceValue = button;
            serialized.ApplyModifiedPropertiesWithoutUndo();
            button.gameObject.SetActive(false);
            AssignCanvas(uiManager, eventCanvas);

            var eventSystem = Object.FindFirstObjectByType<EventSystem>(FindObjectsInactive.Include);
            if (!eventSystem)
            {
                var eventSystemObject = new GameObject("EventSystem", typeof(EventSystem), typeof(StandaloneInputModule));
                Undo.RegisterCreatedObjectUndo(eventSystemObject, "Create event system");
                SceneManager.MoveGameObjectToScene(eventSystemObject, scene);
            }

            EditorSceneManager.MarkSceneDirty(scene);
            EditorSceneManager.SaveScene(scene);
            AssetDatabase.SaveAssets();
            Debug.Log("Created and wired the demo NodeEventCanvas in NueCore. Open the Map scene to test.", canvasObject);
        }

        private static void AssignCanvas(UIManager uiManager, NueGames.NueDeck.Scripts.UI.NodeEventCanvas canvas)
        {
            var serialized = new SerializedObject(uiManager);
            serialized.FindProperty("nodeEventCanvas").objectReferenceValue = canvas;
            serialized.ApplyModifiedPropertiesWithoutUndo();
            EditorSceneManager.MarkSceneDirty(uiManager.gameObject.scene);
            EditorSceneManager.SaveScene(uiManager.gameObject.scene);
        }

        private static GameObject CreateUiObject(string name, Transform parent, Vector2 size, Color color, bool raycast)
        {
            var go = new GameObject(name, typeof(RectTransform), typeof(CanvasRenderer), typeof(Image));
            go.transform.SetParent(parent, false);
            var rect = go.GetComponent<RectTransform>();
            rect.anchorMin = rect.anchorMax = new Vector2(0.5f, 0.5f);
            rect.anchoredPosition = Vector2.zero;
            rect.sizeDelta = size;
            var image = go.GetComponent<Image>();
            image.color = color;
            image.raycastTarget = raycast;
            return go;
        }

        private static TMP_Text CreateText(string name, Transform parent, string text, float size, Vector2 rectSize)
        {
            var go = new GameObject(name, typeof(RectTransform), typeof(CanvasRenderer), typeof(TextMeshProUGUI));
            go.transform.SetParent(parent, false);
            var rect = go.GetComponent<RectTransform>();
            rect.anchorMin = rect.anchorMax = new Vector2(0.5f, 0.5f);
            rect.sizeDelta = rectSize;
            var label = go.GetComponent<TextMeshProUGUI>();
            label.text = text;
            label.fontSize = size;
            label.alignment = TextAlignmentOptions.Center;
            label.color = Color.white;
            if (TMP_Settings.defaultFontAsset) label.font = TMP_Settings.defaultFontAsset;
            return label;
        }

        private static Button CreateButtonPrefab(Transform parent)
        {
            var buttonObject = CreateUiObject("ChoiceButtonPrefab", parent, new Vector2(640, 90), new Color(0.32f, 0.24f, 0.14f, 1f), true);
            var button = buttonObject.AddComponent<Button>();
            button.targetGraphic = buttonObject.GetComponent<Image>();
            var label = CreateText("Label", buttonObject.transform, "Choice", 24, new Vector2(600, 80));
            label.raycastTarget = false;
            var layoutElement = buttonObject.AddComponent<LayoutElement>();
            layoutElement.preferredHeight = 90;
            return button;
        }

        private static void SetAnchoredPosition(RectTransform rect, Vector2 position)
        {
            rect.anchorMin = rect.anchorMax = new Vector2(0.5f, 0.5f);
            rect.anchoredPosition = position;
        }
    }
}
