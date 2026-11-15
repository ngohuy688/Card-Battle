
using System.Collections.Generic;
using NueGames.NueDeck.Scripts.Data.Collection;
using NueGames.NueDeck.Scripts.Data.Collection.RewardData;
using NueGames.NueDeck.Scripts.NueExtentions;
using UnityEngine;
using Random = UnityEngine.Random;

namespace NueGames.NueDeck.Scripts.Data.Containers
{
    [CreateAssetMenu(fileName = "Shop Container", menuName = "NueDeck/Containers/Shop", order = 4)]
    public class ShopContainerData : ScriptableObject
    {
        // [SerializeField]
        [SerializeField] private List<CardShopData> cardRewardDataList;
        [SerializeField] private List<RelicShopData> relicRewardDataList;
        public List<CardShopData> CardRewardDataList => cardRewardDataList;
        public List<RelicShopData> RelicRewardDataList => relicRewardDataList;
    }
}