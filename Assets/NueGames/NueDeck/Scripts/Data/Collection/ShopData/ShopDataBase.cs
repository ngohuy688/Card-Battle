using UnityEngine;
namespace NueGames.NueDeck.Scripts.Data.Collection
{
    [CreateAssetMenu(fileName = "Shop Data", menuName = "NueDeck/Collection/Shop Data", order = 4)]
    public class ShopDataBase : ScriptableObject
    {
        [SerializeField] private int goldCost;
        [SerializeField] private int cardCost;
        [SerializeField] private int relicCost;

        public int GoldCost => goldCost;
        public int CardCost => cardCost;
        public int RelicCost => relicCost;
    }
}