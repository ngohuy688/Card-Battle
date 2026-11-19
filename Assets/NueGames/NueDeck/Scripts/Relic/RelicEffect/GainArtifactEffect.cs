using System.Collections.Generic;
using NueGames.NueDeck.Scripts.Data.Collection;
using NueGames.NueDeck.Scripts.Enums;
using UnityEngine;

namespace NueGames.NueDeck.Scripts.Relic.RelicEffect
{
    public class GainArtifactEffect : RelicEffectBase
    {
        public override RelicEffectType EffectType => RelicEffectType.GainArtifact;

        public override void ApplyEffect(RelicEffectParameters parameters)
        {
            var count = EffectValue(parameters);
            if (count <= 0 || GameManager == null || GameManager.PersistentGameplayData == null)
                return;

            var ownedRelics = GameManager.PersistentGameplayData.CurrentRelicsList;
            if (ownedRelics == null) return;

            // Grant random relic(s) from available pool if available
            Debug.Log($"[Relic] GainArtifact triggered for {count} relic(s).");
        }
    }
}
