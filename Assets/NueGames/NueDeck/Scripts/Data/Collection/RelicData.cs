using System;
using System.Collections.Generic;
using System.Text;
using NueGames.NueDeck.Scripts.Enums;
using NueGames.NueDeck.Scripts.Managers;
using NueGames.NueDeck.Scripts.NueExtentions;
using UnityEngine;

namespace NueGames.NueDeck.Scripts.Data.Collection
{
    [CreateAssetMenu(fileName = "Relic Data",menuName = "NueDeck/Collection/Relic",order = 0)]
    public class RelicData : ScriptableObject
    {
        [SerializeField] private string relicId;
        [SerializeField] private string relicName;
        [SerializeField] private Sprite relicSprite;

        [SerializeField] private int relicCost;
        [TextArea] [SerializeField] private string relicDescription;
        [SerializeField] private RelicEffectType effectType;
        [SerializeField] private RelicCondition conditionType;
        [SerializeField] private TargetType targetType;
        [SerializeField] private int effectValue;

        public string RelicId => relicId;
        public string RelicName => relicName;
        public Sprite RelicSprite => relicSprite;
        public int RelicCost => relicCost;
        public string RelicDescription => relicDescription;
        public RelicEffectType EffectType => effectType;
        public RelicCondition ConditionType => conditionType;
        public TargetType TargetType => targetType;
        public int EffectValue => effectValue;

    }
}