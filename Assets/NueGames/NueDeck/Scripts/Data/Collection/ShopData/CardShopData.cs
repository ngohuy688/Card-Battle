using System.Collections.Generic;
using UnityEngine;
namespace NueGames.NueDeck.Scripts.Data.Collection
{
    [CreateAssetMenu(fileName = "Card Shop Data", menuName = "NueDeck/Collection/Card Shop Data", order = 0)]
    public class CardShopData : ShopDataBase
    {
        [SerializeField] private CardData cardData;
        public CardData CardData => cardData;
    }
}