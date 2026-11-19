using NueGames.NueDeck.Scripts.Card;
using NueGames.NueDeck.Scripts.Data.Collection;
using NueGames.NueDeck.Scripts.Data.Containers;
using NueGames.NueDeck.Scripts.Data.Settings;
using NueGames.NueDeck.Scripts.EnemyBehaviour;
using NueGames.NueDeck.Scripts.NueExtentions;
using NueGames.NueDeck.Scripts.Relic;
using NueGames.NueDeck.Scripts.Enums;
using UnityEngine;
using Random = UnityEngine.Random;

namespace NueGames.NueDeck.Scripts.Managers
{
    [DefaultExecutionOrder(-10)]
    public class GameManager : MonoBehaviour
    { 
        public GameManager(){}
        public static GameManager Instance { get; private set; }
        
        [Header("Settings")]
        [SerializeField] private GameplayData gameplayData;
        [SerializeField] private EncounterData encounterData;
        [SerializeField] private SceneData sceneData;


        #region Cache
        public SceneData SceneData => sceneData;
        public EncounterData EncounterData => encounterData;
        public GameplayData GameplayData => gameplayData;
        public PersistentGameplayData PersistentGameplayData { get; private set; }
        public bool PendingStageAdvanceAfterRewards { get; set; }
        protected UIManager UIManager => UIManager.Instance;
        #endregion
        
        #region Setup
        private void Awake()
        {
            if (Instance)
            {
                Destroy(gameObject);
                return;
               
            }
            else
            {
                transform.parent = null;
                Instance = this;
                DontDestroyOnLoad(gameObject);
                CardActionProcessor.Initialize();
                EnemyActionProcessor.Initialize();
                RelicEffectProcessor.Initialize();
                InitGameplayData();
                SetInitalHand();
            }
        }
        #endregion
        
        #region Public Methods
        public void InitGameplayData()
        { 
            PersistentGameplayData = new PersistentGameplayData(gameplayData);
            PendingStageAdvanceAfterRewards = false;
            var startingNodeId = EncounterData.GetStartingMapNodeId(PersistentGameplayData.CurrentStageId);
            if (!string.IsNullOrEmpty(startingNodeId))
                PersistentGameplayData.AvailableMapNodeIds.Add(startingNodeId);

            GrantStartingRelics();

            if (UIManager)
                UIManager.InformationCanvas.ResetCanvas();
           
        } 

        public void GrantStartingRelics()
        {
            var ownedRelics = PersistentGameplayData?.CurrentRelicsList;
            if (ownedRelics == null) return;

            if (GameplayData && GameplayData.InitialRelics != null)
            {
                foreach (var relic in GameplayData.InitialRelics)
                {
                    if (relic && !ownedRelics.Exists(x => x && x.RelicId == relic.RelicId))
                        RelicEffectProcessor.GrantRelic(relic);
                }
            }

            if (PersistentGameplayData.AllyList != null)
            {
                foreach (var ally in PersistentGameplayData.AllyList)
                {
                    if (ally && ally.AllyCharacterData && ally.AllyCharacterData.StartingRelics != null)
                    {
                        foreach (var relic in ally.AllyCharacterData.StartingRelics)
                        {
                            if (relic && !ownedRelics.Exists(x => x && x.RelicId == relic.RelicId))
                                RelicEffectProcessor.GrantRelic(relic);
                        }
                    }
                }
            }
        } 
        public CardBase BuildAndGetCard(CardData targetData, Transform parent)
        {
            var clone = Instantiate(GameplayData.CardPrefab, parent);
            clone.SetCard(targetData);
            return clone;
        }
        public void SetInitalHand()
        {
            PersistentGameplayData.CurrentCardsList.Clear();
            
            if (PersistentGameplayData.IsRandomHand)
                for (var i = 0; i < GameplayData.RandomCardCount; i++)
                    PersistentGameplayData.CurrentCardsList.Add(GameplayData.AllCardsList.RandomItem());
            else
                foreach (var cardData in GameplayData.InitalDeck.CardList)
                    PersistentGameplayData.CurrentCardsList.Add(cardData);
        }
        public void NextEncounter()
        {
            PersistentGameplayData.CurrentEncounterId++;
            if (PersistentGameplayData.CurrentEncounterId>=EncounterData.EnemyEncounterList[PersistentGameplayData.CurrentStageId].EnemyEncounterList.Count)
            {
                PersistentGameplayData.CurrentEncounterId = Random.Range(0,
                    EncounterData.EnemyEncounterList[PersistentGameplayData.CurrentStageId].EnemyEncounterList.Count);
            }
        }

        public bool TryAdvanceToNextStage()
        {
            if (!EncounterData.TryGetNextStageId(PersistentGameplayData.CurrentStageId, out var nextStageId))
                return false;

            var startingNodeId = EncounterData.GetStartingMapNodeId(nextStageId);
            if (string.IsNullOrEmpty(startingNodeId))
                return false;

            PersistentGameplayData.CurrentStageId = nextStageId;
            PersistentGameplayData.CurrentEncounterId = 0;
            PersistentGameplayData.IsFinalEncounter = false;
            PersistentGameplayData.CurrentMapNodeId = string.Empty;
            PersistentGameplayData.CompletedMapNodeIds.Clear();
            PersistentGameplayData.AvailableMapNodeIds.Clear();
            PersistentGameplayData.AvailableMapNodeIds.Add(startingNodeId);
            return true;
        }

        public bool TrySelectMapNode(string nodeId)
        {
            if (!PersistentGameplayData.AvailableMapNodeIds.Contains(nodeId))
            {
                Debug.LogError($"Map node '{nodeId}' is not currently available.", this);
                return false;
            }

            if (!EncounterData.TryGetMapNode(PersistentGameplayData.CurrentStageId, nodeId, out var node))
                return false;
            if (node.NodeType == EncounterMapNodeType.Shop)
            {
                if (!node.ShopContainerData)
                {
                    Debug.LogError($"Shop node '{node.NodeId}' has no shop data.", EncounterData);
                    return false;
                }
            }
            else if (EncounterData.GetEnemyEncounter(PersistentGameplayData.CurrentStageId, node) == null)
            {
                return false;
            }

            PersistentGameplayData.CurrentMapNodeId = node.NodeId;
            PersistentGameplayData.CurrentEncounterId = Mathf.Max(0, node.RoomNumber - 1);
            PersistentGameplayData.IsFinalEncounter = node.NodeType == EncounterMapNodeType.Boss;
            PersistentGameplayData.AvailableMapNodeIds.Clear();
            return true;
        }

        public bool CompleteCurrentMapNode()
        {
            var progress = PersistentGameplayData;
            if (string.IsNullOrEmpty(progress.CurrentMapNodeId) ||
                !EncounterData.TryGetMapNode(progress.CurrentStageId, progress.CurrentMapNodeId, out var node))
            {
                Debug.LogError("Cannot complete the current map node because no valid node is selected.", this);
                return false;
            }

            if (!progress.CompletedMapNodeIds.Contains(node.NodeId))
                progress.CompletedMapNodeIds.Add(node.NodeId);

            progress.AvailableMapNodeIds.Clear();
            if (node.NextNodeIds != null)
            {
                foreach (var nextNodeId in node.NextNodeIds)
                {
                    if (!EncounterData.TryGetMapNode(progress.CurrentStageId, nextNodeId, out _))
                        return false;
                }

                progress.AvailableMapNodeIds.AddRange(node.NextNodeIds);
            }

            return true;
        }

        public void OnExitApp()
        {
            
        }
        #endregion
      

    }
}
