using System.Collections.Generic;
using NueGames.NueDeck.Scripts.Data.Collection;
using UnityEngine;

namespace NueGames.NueDeck.Scripts.Data.Characters
{
    [CreateAssetMenu(fileName = "Ally Character Data ",menuName = "NueDeck/Characters/Ally",order = 0)]
    public class AllyCharacterData : CharacterDataBase
    {
        [SerializeField] private Sprite portrait;
        [SerializeField] private List<RelicData> startingRelics;

        public Sprite Portrait => portrait;
        public List<RelicData> StartingRelics => startingRelics;
    }
}