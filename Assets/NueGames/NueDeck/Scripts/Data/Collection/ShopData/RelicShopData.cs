using UnityEngine;
namespace NueGames.NueDeck.Scripts.Data.Collection
{
    [CreateAssetMenu(fileName = "Relic Shop Data", menuName = "NueDeck/Collection/Relic Shop Data", order = 2)]
    public class RelicShopData : ShopDataBase
    {
        [SerializeField] private RelicData relicData;
        [SerializeField] private int relicPrice;
        public RelicData RelicData => relicData;
        public int RelicPrice => relicPrice;
    }
}