
using System.Collections.Generic;
using NueGames.NueDeck.Scripts.Data.Collection;
using NueGames.NueDeck.Scripts.Data.Collection.RewardData;
using NueGames.NueDeck.Scripts.NueExtentions;
using UnityEngine;
using Random = UnityEngine.Random;

namespace NueGames.NueDeck.Scripts.Data.Containers
{
    [CreateAssetMenu(fileName = "Reward Container", menuName = "NueDeck/Containers/Reward", order = 4)]
    public class RewardContainerData : ScriptableObject
    {
        [SerializeField] private List<CardRewardData> cardRewardDataList;
        [SerializeField] private List<GoldRewardData> goldRewardDataList;
        [SerializeField] private List<RelicRewardData> relicRewardDataList;
        public List<CardRewardData> CardRewardDataList => cardRewardDataList;
        public List<GoldRewardData> GoldRewardDataList => goldRewardDataList;
        public List<RelicRewardData> RelicRewardDataList => relicRewardDataList;

        public List<CardData> GetRandomCardRewardList(out CardRewardData rewardData)
        {
            rewardData = CardRewardDataList.RandomItem();
            
            List<CardData> cardList = new List<CardData>();
            
            foreach (var cardData in rewardData.RewardCardList)
                cardList.Add(cardData);

            return cardList;
        } 
        public int GetRandomGoldReward(out GoldRewardData rewardData)
        { 
            rewardData = GoldRewardDataList.RandomItem();
            var value =Random.Range(rewardData.MinGold, rewardData.MaxGold);

            return value;
        } 

        public List<RelicData> GetRandomRelicRewardList(out RelicRewardData rewardData)
        {
            rewardData = null;
            if (relicRewardDataList == null || relicRewardDataList.Count == 0)
            {
                Debug.LogError("No relic reward data is configured.", this);
                return new List<RelicData>();
            }

            var availableRewards = relicRewardDataList.FindAll(x => x && x.RelicData != null && x.RelicData.Count > 0);
            if (availableRewards.Count == 0)
            {
                Debug.LogError("Configured relic reward data contains no relics.", this);
                return new List<RelicData>();
            }

            rewardData = availableRewards.RandomItem();
            return new List<RelicData>(rewardData.RelicData);
        }
    }

}