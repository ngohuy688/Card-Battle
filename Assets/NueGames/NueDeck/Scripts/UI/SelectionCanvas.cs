using System;
using System.Collections.Generic;
using NueGames.NueDeck.Scripts.Characters;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

namespace NueGames.NueDeck.Scripts.UI
{
    public class SelectionCanvas : CanvasBase
    {
        [Header("Buttons")]
        [SerializeField] private Button buttonLeft;
        [SerializeField] private Button buttonRight;
        [SerializeField] private Button buttonSelect;
        [SerializeField] private TextMeshProUGUI buttonSelectText;

        [Header("Character Portraits")]
        [SerializeField] private Image leftPortrait;
        [SerializeField] private Image selectedPortrait;
        [SerializeField] private Image rightPortrait;

        private readonly List<AllyBase> _roster = new List<AllyBase>();
        private int _selectedIndex;

        public event Action<AllyBase> SelectionConfirmed;

        private void OnEnable()
        {
            if (buttonLeft)
                buttonLeft.onClick.AddListener(SelectPrevious);
            if (buttonRight)
                buttonRight.onClick.AddListener(SelectNext);
            if (buttonSelect)
                buttonSelect.onClick.AddListener(ConfirmSelection);
        }

        private void OnDisable()
        {
            if (buttonLeft)
                buttonLeft.onClick.RemoveListener(SelectPrevious);
            if (buttonRight)
                buttonRight.onClick.RemoveListener(SelectNext);
            if (buttonSelect)
                buttonSelect.onClick.RemoveListener(ConfirmSelection);
        }

        public override void ResetCanvas()
        {
            _roster.Clear();

            var availableAllies = GameManager.GameplayData.AllyRoster;
            if (availableAllies == null || availableAllies.Count == 0)
            {
                Debug.LogError("SelectionCanvas needs at least one Ally in GameplayData.AllyRoster.", this);
                RefreshDisplay();
                return;
            }

            foreach (var ally in availableAllies)
            {
                if (ally)
                    _roster.Add(ally);
                else
                    Debug.LogWarning("GameplayData.AllyRoster contains an empty Ally entry.", this);
            }

            _selectedIndex = 0;
            var currentAlly = GameManager.PersistentGameplayData.AllyList;
            if (currentAlly != null && currentAlly.Count > 0)
            {
                var existingIndex = _roster.IndexOf(currentAlly[0]);
                if (existingIndex >= 0)
                    _selectedIndex = existingIndex;
            }

            RefreshDisplay();
        }

        private void SelectPrevious()
        {
            if (_roster.Count < 2)
                return;

            _selectedIndex = (_selectedIndex - 1 + _roster.Count) % _roster.Count;
            RefreshDisplay();
        }

        private void SelectNext()
        {
            if (_roster.Count < 2)
                return;

            _selectedIndex = (_selectedIndex + 1) % _roster.Count;
            RefreshDisplay();
        }

        private void ConfirmSelection()
        {
            if (_roster.Count == 0)
                return;

            var selectedAlly = _roster[_selectedIndex];
            var party = GameManager.PersistentGameplayData.AllyList;
            party.Clear();
            party.Add(selectedAlly);

            SelectionConfirmed?.Invoke(selectedAlly);
        }

        private void RefreshDisplay()
        {
            var hasAllies = _roster.Count > 0;
            if (buttonLeft)
                buttonLeft.interactable = _roster.Count > 1;
            if (buttonRight)
                buttonRight.interactable = _roster.Count > 1;
            if (buttonSelect)
                buttonSelect.interactable = hasAllies;

            if (!hasAllies)
            {
                SetPortrait(leftPortrait, null, false);
                SetPortrait(selectedPortrait, null, false);
                SetPortrait(rightPortrait, null, false);
                if (buttonSelectText)
                    buttonSelectText.text = "Select";
                return;
            }

            SetPortrait(leftPortrait, _roster[(_selectedIndex - 1 + _roster.Count) % _roster.Count],
                _roster.Count > 1);
            SetPortrait(selectedPortrait, _roster[_selectedIndex], true);
            SetPortrait(rightPortrait, _roster[(_selectedIndex + 1) % _roster.Count],
                _roster.Count > 1);

            if (buttonSelectText)
                buttonSelectText.text = $"Select {_roster[_selectedIndex].AllyCharacterData.CharacterName}";
        }

        private static void SetPortrait(Image image, AllyBase ally, bool visible)
        {
            if (!image)
                return;

            image.enabled = visible;
            if (!visible || !ally)
            {
                image.sprite = null;
                return;
            }

            var portrait = ally.AllyCharacterData.Portrait;
            if (!portrait)
            {
                var spriteRenderer = ally.GetComponentInChildren<SpriteRenderer>(true);
                if (spriteRenderer)
                    portrait = spriteRenderer.sprite;
            }

            image.sprite = portrait;
            if (!portrait)
                Debug.LogWarning($"No portrait is configured for ally '{ally.AllyCharacterData.CharacterName}'.", ally);
        }
    }
}
