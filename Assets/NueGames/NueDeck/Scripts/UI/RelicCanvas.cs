using System.Collections.Generic;
using NueGames.NueDeck.Scripts.Data.Collection;
using NueGames.NueDeck.Scripts.Relic;
using UnityEngine;

namespace NueGames.NueDeck.Scripts.UI
{
    public class RelicCanvas : CanvasBase
    {
        [SerializeField] private RelicPanel relicPanel;

        private void Awake()
        {
            if (!relicPanel)
            {
                Debug.LogError("RelicCanvas requires a RelicPanel.", this);
                return;
            }

            relicPanel.CloseRequested += CloseCanvas;
        }

        public void Toggle(IList<RelicData> relics)
        {
            if (gameObject.activeSelf)
            {
                CloseCanvas();
                return;
            }

            OpenCanvas();
            if (relicPanel)
                relicPanel.Show(relics);
        }

        public void Refresh(IList<RelicData> relics)
        {
            if (gameObject.activeSelf && relicPanel)
                relicPanel.Refresh(relics);
        }

        public override void ResetCanvas()
        {
            CloseCanvas();
        }
    }
}
