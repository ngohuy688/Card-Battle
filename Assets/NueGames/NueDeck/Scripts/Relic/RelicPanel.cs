using System;
using System.Collections.Generic;
using NueGames.NueDeck.Scripts.Data.Collection;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace NueGames.NueDeck.Scripts.Relic
{
    public class RelicPanel : MonoBehaviour, IPointerClickHandler
    {
        [SerializeField] private RectTransform relicRoot;
        [SerializeField] private RelicUI relicItemPrefab;
        [SerializeField] private RectTransform panelContent;

        private readonly List<RelicUI> _relicItems = new List<RelicUI>();
        private ScrollRect _scrollRect;

        public event Action CloseRequested;

        public void Show(IList<RelicData> relics)
        {
            Refresh(relics);
        }

        public void Refresh(IList<RelicData> relics)
        {
            if (!relicRoot)
            {
                Debug.LogError("RelicPanel requires a relic root.", this);
                return;
            }

            if (!relicItemPrefab)
            {
                Debug.LogError("RelicPanel requires a RelicUI prefab.", this);
                return;
            }

            foreach (var item in _relicItems)
                if (item)
                    Destroy(item.gameObject);
            _relicItems.Clear();

            if (relics == null)
                return;

            foreach (var relic in relics)
            {
                if (!relic)
                    continue;

                var item = Instantiate(relicItemPrefab, relicRoot, false);
                item.name = relic.RelicName;
                item.SetRelic(relic);
                _relicItems.Add(item);
            }

            Canvas.ForceUpdateCanvases();
            if (_scrollRect && _scrollRect.content)
                _scrollRect.verticalNormalizedPosition = 1f;
        }

        public void Close()
        {
            CloseRequested?.Invoke();
        }

        public void OnPointerClick(PointerEventData eventData)
        {
            if (!panelContent ||
                !RectTransformUtility.RectangleContainsScreenPoint(
                    panelContent, eventData.position, eventData.pressEventCamera))
            {
                Close();
            }
        }

        private void Awake()
        {
            if (!panelContent)
            {
                Debug.LogError("RelicPanel requires panel content.", this);
            }

            if (!relicRoot)
            {
                Debug.LogError("RelicPanel requires a relic root.", this);
            }
            else
            {
                _scrollRect = relicRoot.GetComponentInParent<ScrollRect>();
            }
        }
    }
}