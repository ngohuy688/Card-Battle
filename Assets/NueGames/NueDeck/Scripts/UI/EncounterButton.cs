using System;
using TMPro;
using NueGames.NueDeck.Scripts.Enums;
using NueGames.NueDeck.Scripts.Managers;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

namespace NueGames.NueDeck.Scripts.UI
{
    public class EncounterButton : MonoBehaviour
    {
        [Header("Settings")]
        [SerializeField] private Button button;
        [SerializeField] private Image completedImage;
        [SerializeField] private bool isFinal;

        private Action _onSelected;
        
        protected GameManager GameManager => GameManager.Instance;
        
        private void Awake()
        {
            if (completedImage)
                completedImage.gameObject.SetActive(false);
        }

        public void Configure(string nodeName, EncounterMapNodeType nodeType, Action onSelected)
        {
            if (!button)
            {
                Debug.LogError("EncounterButton is missing its Button reference.", this);
                return;
            }

            var label = GetComponentInChildren<TMP_Text>(true);
            if (!label)
            {
                Debug.LogError("EncounterButton is missing a TextMeshPro label.", this);
                return;
            }

            label.text = nodeName;
            isFinal = nodeType == EncounterMapNodeType.Boss;
            var colors = button.colors;
            if (isFinal)
            {
                colors.normalColor = new Color(1f, 0.78f, 0.42f, 1f);
                colors.highlightedColor = new Color(1f, 0.88f, 0.62f, 1f);
                colors.pressedColor = new Color(0.82f, 0.62f, 0.32f, 1f);
                colors.selectedColor = colors.highlightedColor;
            }
            else if (nodeType == EncounterMapNodeType.Shop)
            {
                colors.normalColor = new Color(0.66f, 0.9f, 0.72f, 1f);
                colors.highlightedColor = new Color(0.78f, 0.98f, 0.82f, 1f);
                colors.pressedColor = new Color(0.43f, 0.72f, 0.52f, 1f);
                colors.selectedColor = colors.highlightedColor;
                colors.disabledColor = new Color(0.58f, 0.7f, 0.61f, 0.65f);
            }

            button.colors = colors;
            if (button.targetGraphic)
                button.targetGraphic.color = Color.white;

            _onSelected = onSelected;
            for (var i = 0; i < button.onClick.GetPersistentEventCount(); i++)
                button.onClick.SetPersistentListenerState(i, UnityEventCallState.Off);

            button.onClick.RemoveAllListeners();
            button.onClick.AddListener(OnButtonClicked);
        }

        private void OnButtonClicked()
        {
            _onSelected?.Invoke();
        }
        
        public void SetStatus(EncounterButtonStatus targetStatus)
        {
            switch (targetStatus)
            {
                case EncounterButtonStatus.Active:
                    button.interactable = true;
                    if (completedImage)
                        completedImage.gameObject.SetActive(false);
                    break;
                case EncounterButtonStatus.Passive:
                    button.interactable = false;
                    if (completedImage)
                        completedImage.gameObject.SetActive(false);
                    break;
                case EncounterButtonStatus.Completed:
                    button.interactable = false;
                    if (completedImage)
                        completedImage.gameObject.SetActive(true);
                    break;
                default:
                    throw new ArgumentOutOfRangeException(nameof(targetStatus), targetStatus, null);
            }
        }
    }
}