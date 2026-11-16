using UnityEngine;
namespace NueGames.NueDeck.Scripts.Data.Collection
{
    [CreateAssetMenu(fileName = "Shop Data", menuName = "NueDeck/Collection/Shop Data", order = 4)]
    public class ShopDataBase : ScriptableObject
    {
        [SerializeField] private int goldCost;
        public int GoldCost => goldCost;
    }
}