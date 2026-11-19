using System;
using System.Collections.Generic;
using NueGames.NueDeck.Scripts.Card;
using NueGames.NueDeck.Scripts.Data.Collection;
using NueGames.NueDeck.Scripts.Data.Containers;
using NueGames.NueDeck.Scripts.Enums;
using NueGames.NueDeck.Scripts.NueExtentions;
using NueGames.NueDeck.Scripts.Relic;
using UnityEngine;

namespace NueGames.NueDeck.Scripts.UI.Reward
{
    public class RewardCanvas : CanvasBase
    {
        [Header("References")]
        [SerializeField] private RewardContainerData rewardContainerData;
        [SerializeField] private Transform rewardRoot;
        [SerializeField] private RewardContainer rewardContainerPrefab;
        [SerializeField] private Transform rewardPanelRoot;
        [Header("Choice")]
        [SerializeField] private Transform choice2DCardSpawnRoot;
        [SerializeField] private ChoiceCard choiceCardUIPrefab;
        [SerializeField] private ChoicePanel choicePanel;
        
        private readonly List<RewardContainer> _currentRewardsList = new List<RewardContainer>();
        private readonly List<ChoiceCard> _spawnedChoiceList = new List<ChoiceCard>();
        private readonly List<CardData> _cardRewardList = new List<CardData>();
        private readonly List<RewardContainer> _relicChoiceList = new List<RewardContainer>();

        public ChoicePanel ChoicePanel => choicePanel;
        
        #region Public Methods

        public void PrepareCanvas()
        {
            rewardPanelRoot.gameObject.SetActive(true);
        }
        public void BuildReward(RewardType rewardType)
        {
            if (rewardType == RewardType.Relic)
            {
                BuildRelicReward();
                return;
            }

            var rewardClone = Instantiate(rewardContainerPrefab, rewardRoot);
            _currentRewardsList.Add(rewardClone);
            
            switch (rewardType)
            {
                case RewardType.Gold:
                    var rewardGold = rewardContainerData.GetRandomGoldReward(out var goldRewardData);
                    rewardClone.BuildReward(goldRewardData.RewardSprite,goldRewardData.RewardDescription);
                    rewardClone.RewardButton.onClick.AddListener(()=>GetGoldReward(rewardClone,rewardGold));
                    break;
                case RewardType.Card:
                    var rewardCardList = rewardContainerData.GetRandomCardRewardList(out var cardRewardData);
                    _cardRewardList.Clear();
                    foreach (var cardData in rewardCardList)
                        _cardRewardList.Add(cardData);
                    rewardClone.BuildReward(cardRewardData.RewardSprite,cardRewardData.RewardDescription);
                    rewardClone.RewardButton.onClick.AddListener(()=>GetCardReward(rewardClone,3));
                    break;
                default:
                    throw new ArgumentOutOfRangeException(nameof(rewardType), rewardType, null);
            }
        }
        
        public override void ResetCanvas()
        {
            ResetRewards();

            ResetChoice();
        }

        private void ResetRewards()
        {
            foreach (var rewardContainer in _currentRewardsList)
                Destroy(rewardContainer.gameObject);

            _currentRewardsList?.Clear();
            _relicChoiceList.Clear();
        }

        private void ResetChoice()
        {
            foreach (var choice in _spawnedChoiceList)
            {
                Destroy(choice.gameObject);
            }

            _spawnedChoiceList?.Clear();
            ChoicePanel.DisablePanel();
        }

        #endregion
        
        #region Private Methods
        private void GetGoldReward(RewardContainer rewardContainer,int amount)
        {
            GameManager.PersistentGameplayData.CurrentGold += amount;
            RelicEffectProcessor.Trigger(RelicCondition.OnCoinGained,
                new RelicEffectParameters { Value = amount });
            _currentRewardsList.Remove(rewardContainer);
            UIManager.InformationCanvas.SetGoldText(GameManager.PersistentGameplayData.CurrentGold);
            Destroy(rewardContainer.gameObject);
        }

        private void BuildRelicReward()
        {
            var relicChoices = rewardContainerData.GetRandomRelicRewardList(out _);
            var ownedRelics = GameManager.PersistentGameplayData.CurrentRelicsList;
            relicChoices.RemoveAll(relic => !relic ||
                (relic.IsUnique && ownedRelics.Exists(owned => owned && owned.RelicId == relic.RelicId)));

            if (relicChoices.Count == 0)
            {
                Debug.LogWarning("No unowned relics are available for this reward.", rewardContainerData);
                return;
            }

            foreach (var relic in relicChoices)
            {
                var rewardContainer = Instantiate(rewardContainerPrefab, rewardRoot);
                _currentRewardsList.Add(rewardContainer);
                _relicChoiceList.Add(rewardContainer);
                rewardContainer.BuildReward(
                    relic.RelicSprite,
                    $"{relic.RelicName}\n{relic.RelicDescription}");
                rewardContainer.RewardButton.onClick.AddListener(() => GetRelicReward(relic));
            }
        }

        private void GetRelicReward(RelicData relicData)
        {
            if (!relicData)
                return;

            if (!RelicEffectProcessor.GrantRelic(relicData))
            {
                Debug.LogWarning($"Relic '{relicData.RelicName}' could not be added.", relicData);
                return;
            }

            foreach (var relicChoice in _relicChoiceList)
            {
                _currentRewardsList.Remove(relicChoice);
                if (relicChoice)
                    Destroy(relicChoice.gameObject);
            }

            _relicChoiceList.Clear();
        }

        private void GetCardReward(RewardContainer rewardContainer,int amount = 3)
        {
            ChoicePanel.gameObject.SetActive(true);
            
            for (int i = 0; i < amount; i++)
            {
                Transform spawnTransform = choice2DCardSpawnRoot;
              
                var choice = Instantiate(choiceCardUIPrefab, spawnTransform);
                
                var reward = _cardRewardList.RandomItem();
                choice.BuildReward(reward);
                choice.OnCardChose += ResetChoice;
                
                _cardRewardList.Remove(reward);
                _spawnedChoiceList.Add(choice);
                _currentRewardsList.Remove(rewardContainer);
                
            }
            
            Destroy(rewardContainer.gameObject);
        }
        #endregion
        
    }
}