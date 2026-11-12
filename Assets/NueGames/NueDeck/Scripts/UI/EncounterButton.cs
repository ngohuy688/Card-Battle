using System;
using TMPro;
using NueGames.NueDeck.Scripts.Enums;
using NueGames.NueDeck.Scripts.Managers;
using UnityEngine;
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

        public void Configure(string nodeName, bool finalNode, Action onSelected)
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
            isFinal = finalNode;
            if (button.targetGraphic)
                button.targetGraphic.color = isFinal ? new Color(1f, 0.78f, 0.42f, 1f) : Color.white;

            _onSelected = onSelected;
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