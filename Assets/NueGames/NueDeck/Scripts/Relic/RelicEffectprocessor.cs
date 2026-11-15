using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using NueGames.NueDeck.Scripts.Data.Collection;
using NueGames.NueDeck.Scripts.Enums;
using NueGames.NueDeck.Scripts.Managers;
using UnityEngine;

namespace NueGames.NueDeck.Scripts.Relic
{
    public static class RelicEffectProcessor
    {
        private static readonly Dictionary<RelicEffectType, RelicEffectBase> RelicEffectDict =
            new Dictionary<RelicEffectType, RelicEffectBase>();

        public static bool IsInitialized { get; private set; }

        public static void Initialize()
        {
            RelicEffectDict.Clear();

            var allRelicEffects = Assembly.GetAssembly(typeof(RelicEffectBase)).GetTypes()
                .Where(t => typeof(RelicEffectBase).IsAssignableFrom(t) && !t.IsAbstract);

            foreach (var relicEffect in allRelicEffects)
            {
                var effect = Activator.CreateInstance(relicEffect) as RelicEffectBase;
                if (effect != null)
                    RelicEffectDict.Add(effect.EffectType, effect);
            }
    
            IsInitialized = true;
        }

        public static bool GrantRelic(RelicData relicData)
        {
            EnsureInitialized();

            if (!relicData)
            {
                Debug.LogError("Cannot grant a null relic.");
                return false;
            }

            if (string.IsNullOrWhiteSpace(relicData.RelicId))
            {
                Debug.LogError($"Relic '{relicData.name}' must have a non-empty ID.", relicData);
                return false;
            }

            if (!RelicEffectDict.ContainsKey(relicData.EffectType))
            {
                Debug.LogError(
                    $"No relic effect is registered for '{relicData.EffectType}' ({relicData.name}).",
                    relicData);
                return false;
            }

            var gameManager = GameManager.Instance;
            if (!gameManager || gameManager.PersistentGameplayData == null)
                throw new InvalidOperationException("Cannot grant a relic before gameplay data is initialized.");

            var ownedRelics = gameManager.PersistentGameplayData.CurrentRelicsList;
            if (ownedRelics == null)
                throw new InvalidOperationException("The current relic collection is not initialized.");

            if (ownedRelics.Exists(x => x && x.RelicId == relicData.RelicId))
                return false;

            ownedRelics.Add(relicData);
            TriggerRelic(relicData, RelicCondition.OnRelicEquipped, new RelicEffectParameters());
            if (UIManager.Instance && UIManager.Instance.InformationCanvas)
                UIManager.Instance.InformationCanvas.SetRelicsText(ownedRelics);
            return true;
        }

        public static void Trigger(RelicCondition condition, RelicEffectParameters parameters = null)
        {
            EnsureInitialized();

            var gameManager = GameManager.Instance;
            if (!gameManager || gameManager.PersistentGameplayData == null)
                return;

            var ownedRelics = gameManager.PersistentGameplayData.CurrentRelicsList;
            if (ownedRelics == null)
                return;

            var relicSnapshot = new List<RelicData>(ownedRelics);
            foreach (var relicData in relicSnapshot)
            {
                if (relicData && relicData.ConditionType == condition)
                    TriggerRelic(relicData, condition, parameters);
            }
        }

        private static void TriggerRelic(
            RelicData relicData,
            RelicCondition condition,
            RelicEffectParameters parameters)
        {
            if (relicData.ConditionType != condition)
                return;

            if (!RelicEffectDict.TryGetValue(relicData.EffectType, out var effect))
                throw new InvalidOperationException(
                    $"No relic effect is registered for '{relicData.EffectType}' ({relicData.name}).");

            parameters = parameters ?? new RelicEffectParameters();
            parameters.RelicData = relicData;
            effect.ApplyEffect(parameters);
        }

        private static void EnsureInitialized()
        {
            if (!IsInitialized)
                throw new InvalidOperationException("RelicEffectProcessor must be initialized before use.");
        }
    }
}