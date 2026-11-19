using System;
using System.Collections.Generic;
using NueGames.NueDeck.Scripts.Enums;
using UnityEngine;

namespace NueGames.NueDeck.Scripts.Data.Collection
{
    [CreateAssetMenu(fileName = "Relic Data",menuName = "NueDeck/Collection/Relic",order = 0)]
    public class RelicData : ScriptableObject, ISerializationCallbackReceiver
    {
        [SerializeField] private string relicId;
        [SerializeField] private string relicName;
        [SerializeField] private Sprite relicSprite;

        [TextArea] [SerializeField] private string relicDescription;

        // Backward compatibility fields for assets serialized prior to List<RelicEffectData>
        [HideInInspector] [SerializeField] private RelicEffectType effectType;
        [HideInInspector] [SerializeField] private RelicCondition conditionType;
        [HideInInspector] [SerializeField] private TargetType targetType;
        [HideInInspector] [SerializeField] private int effectValue;

        [SerializeField] private List<RelicEffectData> relicEffectDatas = new List<RelicEffectData>();
        [SerializeField] private bool isUnique;

        public string RelicId => relicId;
        public string RelicName => relicName;
        public Sprite RelicSprite => relicSprite;
        public string RelicDescription => relicDescription;
        public bool IsUnique => isUnique;
        public List<RelicEffectData> RelicEffectDatas => relicEffectDatas;

        public void OnBeforeSerialize() { }

        public void OnAfterDeserialize()
        {
            if ((relicEffectDatas == null || relicEffectDatas.Count == 0) && effectValue > 0)
            {
                relicEffectDatas = new List<RelicEffectData>
                {
                    new RelicEffectData(effectType, conditionType, targetType, effectValue)
                };
            }
        }
    }

    [Serializable]
    public class RelicEffectData
    {
        [SerializeField] private RelicEffectType effectType;
        [SerializeField] private RelicCondition conditionType;
        [SerializeField] private TargetType targetType;
        [SerializeField] private int effectValue;

        public RelicEffectData() { }

        public RelicEffectData(RelicEffectType effectType, RelicCondition conditionType, TargetType targetType, int effectValue)
        {
            this.effectType = effectType;
            this.conditionType = conditionType;
            this.targetType = targetType;
            this.effectValue = effectValue;
        }

        public RelicEffectType EffectType => effectType;
        public RelicCondition ConditionType => conditionType;
        public TargetType TargetType => targetType;
        public int EffectValue => effectValue;
    }
}