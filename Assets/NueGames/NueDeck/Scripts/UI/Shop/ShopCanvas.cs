using System;
using System.Collections.Generic;
using System.Linq;
using NueGames.NueDeck.Scripts.Data.Collection;
using NueGames.NueDeck.Scripts.Data.Containers;
using NueGames.NueDeck.Scripts.Managers;
using NueGames.NueDeck.Scripts.Relic;
using NueGames.NueDeck.Scripts.UI.Reward;
using UnityEngine;
using UnityEngine.UI;

namespace NueGames.NueDeck.Scripts.UI.Shop
{
    public class ShopCanvas : CanvasBase
    {
        [SerializeField] private Transform itemHolderRoot;
        [SerializeField] private RewardContainer offerPrefab;
        [SerializeField] private Button returnButton;

        private readonly List<ShopContainer> _activeOffers = new List<ShopContainer>();
        private ShopContainerData _currentShop;
        private Action _onShopClosed;

        private void Awake()
        {
            if (!returnButton)
                Debug.LogError("ShopCanvas requires a return button.", this);
            else
                returnButton.onClick.AddListener(ExitShop);
        }

        public void OpenShop(ShopContainerData shopData, Action onShopClosed)
        {
            if (!shopData)
            {
                Debug.LogError("Cannot open a shop without ShopContainerData.", this);
                return;
            }

            ClearOffers();
            _currentShop = shopData;
            _onShopClosed = onShopClosed;
            OpenCanvas();
            BuildOffers();
        }

        public override void ResetCanvas()
        {
            ClearOffers();
            _currentShop = null;
            _onShopClosed = null;
        }

        public void ExitShop()
        {
            if (!GameManager || !GameManager.CompleteCurrentMapNode())
                return;

            var onShopClosed = _onShopClosed;
            ClearOffers();
            _currentShop = null;
            _onShopClosed = null;
            CloseCanvas();
            onShopClosed?.Invoke();
        }

        private void BuildOffers()
        {
            if (!itemHolderRoot || !offerPrefab)
            {
                Debug.LogError("ShopCanvas requires an item holder and offer prefab.", this);
                return;
            }

            foreach (var cardOffer in _currentShop.GetRandomCardOffers())
                CreateCardOffer(cardOffer);

            var ownedRelics = GameManager.PersistentGameplayData.CurrentRelicsList;
            if (ownedRelics == null)
            {
                Debug.LogError("The current relic collection is not initialized.", this);
                return;
            }

            var ownedRelicIds = new HashSet<string>(ownedRelics
                .Where(relic => relic && !string.IsNullOrWhiteSpace(relic.RelicId))
                .Select(relic => relic.RelicId));
            foreach (var relicOffer in _currentShop.GetRandomRelicOffers())
                CreateRelicOffer(relicOffer, ownedRelicIds);

            UpdateOfferAvailability();
        }

        private void CreateCardOffer(CardShopData offer)
        {
            if (!offer || !offer.CardData || !offer.CardData.CardSprite || offer.GoldCost < 0)
            {
                Debug.LogError("Shop contains an invalid card offer.", this);
                return;
            }

            offer.CardData.UpdateDescription();
            var description = $"{offer.CardData.CardName}\n{offer.CardData.MyDescription}";
            CreateOffer(offer.CardData.CardSprite, description, offer.GoldCost,
                () => TryPurchaseCard(offer));
        }

        private void CreateRelicOffer(RelicShopData offer, HashSet<string> offeredRelicIds)
        {
            if (!offer || !offer.RelicData || !offer.RelicData.RelicSprite ||
                string.IsNullOrWhiteSpace(offer.RelicData.RelicId) || offer.GoldCost < 0)
            {
                Debug.LogError("Shop contains an invalid relic offer.", this);
                return;
            }

            if (!offeredRelicIds.Add(offer.RelicData.RelicId))
                return;

            var description = $"{offer.RelicData.RelicName}\n{offer.RelicData.RelicDescription}";
            CreateOffer(offer.RelicData.RelicSprite, description, offer.GoldCost,
                () => TryPurchaseRelic(offer));
        }

        private void CreateOffer(Sprite sprite, string description, int cost, Func<bool> purchase)
        {
            var reward = Instantiate(offerPrefab, itemHolderRoot, false);
            var offer = reward.gameObject.AddComponent<ShopContainer>();
            offer.Configure(reward, sprite, description, cost, purchase);
            _activeOffers.Add(offer);
        }

        private bool TryPurchaseCard(CardShopData offer)
        {
            var runData = GameManager.PersistentGameplayData;
            if (runData.CurrentGold < offer.GoldCost)
                return false;

            if (runData.CurrentCardsList == null)
            {
                Debug.LogError("The current card collection is not initialized.", this);
                return false;
            }

            runData.CurrentGold -= offer.GoldCost;
            runData.CurrentCardsList.Add(offer.CardData);
            UpdateGoldDisplay();
            UpdateOfferAvailability();
            return true;
        }

        private bool TryPurchaseRelic(RelicShopData offer)
        {
            var runData = GameManager.PersistentGameplayData;
            if (runData.CurrentGold < offer.GoldCost)
                return false;

            if (!RelicEffectProcessor.GrantRelic(offer.RelicData))
                return false;

            runData.CurrentGold -= offer.GoldCost;
            UpdateGoldDisplay();
            UpdateOfferAvailability();
            return true;
        }

        private void UpdateGoldDisplay()
        {
            if (UIManager && UIManager.InformationCanvas)
                UIManager.InformationCanvas.SetGoldText(GameManager.PersistentGameplayData.CurrentGold);
        }

        private void UpdateOfferAvailability()
        {
            var currentGold = GameManager.PersistentGameplayData.CurrentGold;
            foreach (var offer in _activeOffers)
                offer.SetCanPurchase(currentGold >= offer.GoldCost);
        }

        private void ClearOffers()
        {
            foreach (var offer in _activeOffers)
            {
                if (offer)
                    Destroy(offer.gameObject);
            }

            _activeOffers.Clear();
        }
    }
}
