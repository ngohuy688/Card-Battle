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

            if (relicData.RelicEffectDatas == null || relicData.RelicEffectDatas.Count == 0)
            {
                Debug.LogError($"Relic '{relicData.name}' has no effects configured.", relicData);
                return false;
            }

            var gameManager = GameManager.Instance;
            if (!gameManager || gameManager.PersistentGameplayData == null)
                throw new InvalidOperationException("Cannot grant a relic before gameplay data is initialized.");

            var ownedRelics = gameManager.PersistentGameplayData.CurrentRelicsList;
            if (ownedRelics == null)
                throw new InvalidOperationException("The current relic collection is not initialized.");

            if (relicData.IsUnique && ownedRelics.Exists(x => x && x.RelicId == relicData.RelicId))
                return false;

            ownedRelics.Add(relicData);
            TriggerRelic(relicData, RelicCondition.OnRelicEquipped, new RelicEffectParameters());
            if (UIManager.Instance && UIManager.Instance.RelicCanvas)
                UIManager.Instance.RelicCanvas.Refresh(ownedRelics);
            if (UIManager.Instance && UIManager.Instance.InformationCanvas)
                UIManager.Instance.InformationCanvas.ResetCanvas();
            Debug.Log($"[Relic] Successfully granted relic '{relicData.RelicName}' (ID: {relicData.RelicId}) with {relicData.RelicEffectDatas.Count} effect(s).");
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
                if (relicData)
                    TriggerRelic(relicData, condition, parameters);
            }
        }

        private static void TriggerRelic(
            RelicData relicData,
            RelicCondition condition,
            RelicEffectParameters parameters)
        {
            if (relicData.RelicEffectDatas == null)
                return;

            parameters = parameters ?? new RelicEffectParameters();
            parameters.RelicData = relicData;

            foreach (var effectData in relicData.RelicEffectDatas)
            {
                if (effectData == null)
                    continue;

                // Chỉ kích hoạt effect nếu condition khớp
                if (effectData.ConditionType != condition)
                    continue;

                if (!RelicEffectDict.TryGetValue(effectData.EffectType, out var effect))
                {
                    Debug.LogWarning(
                        $"No relic effect registered for '{effectData.EffectType}' on relic '{relicData.name}'.");
                    continue;
                }

                parameters.CurrentEffectData = effectData;
                effect.ApplyEffect(parameters);
                Debug.Log($"[Relic] Triggered effect '{effectData.EffectType}' on relic '{relicData.RelicName}' for condition '{condition}'.");
            }
        }

        private static void EnsureInitialized()
        {
            if (!IsInitialized)
                Initialize();
        }
    }
}