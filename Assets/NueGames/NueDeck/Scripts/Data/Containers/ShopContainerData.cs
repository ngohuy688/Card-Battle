
using System.Collections.Generic;
using NueGames.NueDeck.Scripts.Data.Collection;
using UnityEngine;
using Random = UnityEngine.Random;

namespace NueGames.NueDeck.Scripts.Data.Containers
{
    [CreateAssetMenu(fileName = "Shop Container", menuName = "NueDeck/Containers/Shop", order = 4)]
    public class ShopContainerData : ScriptableObject
    {
        // [SerializeField]
        [SerializeField] private List<CardShopData> cardShopDataList;
        [SerializeField] private List<RelicShopData> relicShopDataList;
        public int RefreshCost = 10;
        public int ShopCardSlotCount = 3;
        public int ShopRelicSlotCount = 3;
        public List<CardShopData> CardShopDataList => cardShopDataList;
        public List<RelicShopData> RelicShopDataList => relicShopDataList;

        public List<CardShopData> GetRandomCardOffers()
        {
            return GetRandomOffers(cardShopDataList, ShopCardSlotCount);
        }

        public List<RelicShopData> GetRandomRelicOffers()
        {
            return GetRandomOffers(relicShopDataList, ShopRelicSlotCount);
        }

        private List<T> GetRandomOffers<T>(List<T> offers, int slotCount) where T : Object
        {
            var selected = new List<T>();
            if (offers == null)
            {
                Debug.LogError("Shop offer list is not configured.", this);
                return selected;
            }

            var available = offers.FindAll(offer => offer);
            var count = Mathf.Min(Mathf.Max(0, slotCount), available.Count);
            for (var i = 0; i < count; i++)
            {
                var index = Random.Range(0, available.Count);
                selected.Add(available[index]);
                available.RemoveAt(index);
            }

            return selected;
        }
    }
}