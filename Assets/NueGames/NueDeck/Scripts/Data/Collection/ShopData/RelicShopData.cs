using UnityEngine;
namespace NueGames.NueDeck.Scripts.Data.Collection
{
    [CreateAssetMenu(fileName = "Relic Shop Data", menuName = "NueDeck/Collection/Relic Shop Data", order = 2)]
    public class RelicShopData : ShopDataBase
    {
        [SerializeField] private RelicData relicData;
        public RelicData RelicData => relicData;
    }
}