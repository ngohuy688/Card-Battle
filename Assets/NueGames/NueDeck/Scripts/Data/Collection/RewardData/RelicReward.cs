using System.Collections.Generic;
using UnityEngine;

namespace NueGames.NueDeck.Scripts.Data.Collection.RewardData
{
    [CreateAssetMenu(fileName = "Relic Reward Data",menuName = "NueDeck/Collection/Rewards/",order = 0)]
    public class RelicRewardData : RewardDataBase
    {
        [SerializeField] private List<RelicData> relicData;
        public List<RelicData> RelicData => relicData;
    }
}