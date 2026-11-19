using System.Collections.Generic;
using NueGames.NueDeck.Scripts.Card;
using NueGames.NueDeck.Scripts.Characters;
using NueGames.NueDeck.Scripts.Data.Collection;
using UnityEngine;

namespace NueGames.NueDeck.Scripts.Data.Settings
{
    [CreateAssetMenu(fileName = "Gameplay Data", menuName = "NueDeck/Settings/GameplayData", order = 0)]
    public class GameplayData : ScriptableObject
    {
        [Header("Gameplay Settings")] 
        [SerializeField] private int drawCount = 4;
        [SerializeField] private int maxMana = 3;
        [SerializeField] private List<AllyBase> initalAllyList;
        [SerializeField] private List<AllyBase> allyRoster;
        
        [Header("Decks")] 
        [SerializeField] private DeckData initalDeck;
        [SerializeField] private int maxCardOnHand;
        
        [Header("Relics")]
        [SerializeField] private List<RelicData> initialRelics;

        [Header("Card Settings")] 
        [SerializeField] private List<CardData> allCardsList;
        [SerializeField] private CardBase cardPrefab;

        [Header("Customization Settings")] 
        [SerializeField] private string defaultName = "Nig";
        [SerializeField] private bool useStageSystem;
        
        [Header("Modifiers")]
        [SerializeField] private bool isRandomHand = false;
        [SerializeField] private int randomCardCount;
        
        #region Encapsulation
        public int DrawCount => drawCount;
        public int MaxMana => maxMana;
        public bool IsRandomHand => isRandomHand;
        public List<AllyBase> InitalAllyList => initalAllyList;
        public List<AllyBase> AllyRoster => allyRoster;
        public DeckData InitalDeck => initalDeck;
        public List<RelicData> InitialRelics => initialRelics;
        public int RandomCardCount => randomCardCount;
        public int MaxCardOnHand => maxCardOnHand;
        public List<CardData> AllCardsList => allCardsList;
        public CardBase CardPrefab => cardPrefab;
        public string DefaultName => defaultName;
        public bool UseStageSystem => useStageSystem;
        #endregion
    }
}