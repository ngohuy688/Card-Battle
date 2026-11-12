using System;
using System.Collections.Generic;
using System.Linq;
using NueGames.NueDeck.Scripts.Characters;
using NueGames.NueDeck.Scripts.Data.Characters;
using NueGames.NueDeck.Scripts.Enums;
using NueGames.NueDeck.Scripts.NueExtentions;
using UnityEngine;

namespace NueGames.NueDeck.Scripts.Data.Containers
{
    [CreateAssetMenu(fileName = "Encounter Data", menuName = "NueDeck/Containers/EncounterData", order = 4)]
    public class EncounterData : ScriptableObject
    {
        [Header("Settings")] 
        [SerializeField] private bool encounterRandomlyAtStage;
        [SerializeField] private List<EnemyEncounterStage> enemyEncounterList;

        public bool EncounterRandomlyAtStage => encounterRandomlyAtStage;
        public List<EnemyEncounterStage> EnemyEncounterList => enemyEncounterList;

        public EnemyEncounter GetEnemyEncounter(int stageId = 0,int encounterId =0,bool isFinal = false)
        {
            var selectedStage = EnemyEncounterList.First(x => x.StageId == stageId);
            if (isFinal) return selectedStage.BossEncounterList.RandomItem();
           
            return EncounterRandomlyAtStage
                ? selectedStage.EnemyEncounterList.RandomItem()
                : selectedStage.EnemyEncounterList[encounterId] ?? selectedStage.EnemyEncounterList.RandomItem();
        }

        public bool TryGetNextStageId(int currentStageId, out int nextStageId)
        {
            nextStageId = 0;
            if (EnemyEncounterList == null || EnemyEncounterList.Count == 0)
            {
                Debug.LogError("EncounterData has no stages configured.", this);
                return false;
            }

            var orderedStages = EnemyEncounterList.OrderBy(stage => stage.StageId).ToList();
            var currentStageIndex = orderedStages.FindIndex(stage => stage.StageId == currentStageId);
            if (currentStageIndex < 0)
            {
                Debug.LogError($"Stage ID {currentStageId} is not configured in EncounterData.", this);
                return false;
            }

            if (currentStageIndex + 1 >= orderedStages.Count)
                return false;

            nextStageId = orderedStages[currentStageIndex + 1].StageId;
            return true;
        }

        public string GetStartingMapNodeId(int stageId)
        {
            var stage = EnemyEncounterList?.FirstOrDefault(x => x.StageId == stageId);
            if (stage == null)
            {
                Debug.LogError($"Stage ID {stageId} is not configured in EncounterData.", this);
                return null;
            }

            if (string.IsNullOrWhiteSpace(stage.MapStartNodeId) ||
                stage.MapNodeList == null ||
                !stage.MapNodeList.Any(x => x != null && x.NodeId == stage.MapStartNodeId))
            {
                Debug.LogError($"Stage {stageId} has no valid starting map node configured.", this);
                return null;
            }

            return stage.MapStartNodeId;
        }

        public List<EncounterMapNode> GetMapNodes(int stageId)
        {
            var stage = EnemyEncounterList?.FirstOrDefault(x => x.StageId == stageId);
            if (stage == null)
            {
                Debug.LogError($"Stage ID {stageId} is not configured in EncounterData.", this);
                return null;
            }

            if (stage.MapNodeList == null || stage.MapNodeList.Count == 0)
            {
                Debug.LogError($"Stage {stageId} has no map nodes configured.", this);
                return null;
            }

            var nodeIds = new HashSet<string>();
            foreach (var node in stage.MapNodeList)
            {
                if (node == null || string.IsNullOrWhiteSpace(node.NodeId) || !nodeIds.Add(node.NodeId))
                {
                    Debug.LogError($"Stage {stageId} has a null, empty, or duplicate map node ID.", this);
                    return null;
                }
            }

            if (!nodeIds.Contains(stage.MapStartNodeId))
            {
                Debug.LogError($"Stage {stageId} starting node '{stage.MapStartNodeId}' is missing.", this);
                return null;
            }

            foreach (var node in stage.MapNodeList)
            {
                if (!Enum.IsDefined(typeof(EncounterMapNodeType), node.NodeType) ||
                    node.Position.x < 0f || node.Position.x > 1f ||
                    node.Position.y < 0f || node.Position.y > 1f)
                {
                    Debug.LogError(
                        $"Map node '{node.NodeId}' has an invalid type or a position outside the normalized 0-1 range.",
                        this);
                    return null;
                }

                var encounters = node.NodeType == EncounterMapNodeType.Boss
                    ? stage.BossEncounterList
                    : stage.EnemyEncounterList;
                if (encounters == null || node.EncounterIndex < 0 || node.EncounterIndex >= encounters.Count)
                {
                    Debug.LogError(
                        $"Map node '{node.NodeId}' references an invalid encounter index for stage {stageId}.", this);
                    return null;
                }

                if (node.NextNodeIds == null)
                    continue;

                foreach (var nextNodeId in node.NextNodeIds)
                {
                    if (!nodeIds.Contains(nextNodeId))
                    {
                        Debug.LogError(
                            $"Map node '{node.NodeId}' references missing next node '{nextNodeId}' in stage {stageId}.",
                            this);
                        return null;
                    }
                }
            }

            return stage.MapNodeList;
        }

        public bool TryGetMapNode(int stageId, string nodeId, out EncounterMapNode mapNode)
        {
            mapNode = null;
            var nodes = GetMapNodes(stageId);
            if (nodes == null)
                return false;

            mapNode = nodes.FirstOrDefault(x => x.NodeId == nodeId);
            if (mapNode == null)
                Debug.LogError($"Map node '{nodeId}' is not configured for stage {stageId}.", this);

            return mapNode != null;
        }

        public EnemyEncounter GetEnemyEncounter(int stageId, EncounterMapNode mapNode)
        {
            if (mapNode == null)
            {
                Debug.LogError($"Cannot resolve an encounter for a null map node in stage {stageId}.", this);
                return null;
            }

            var stage = EnemyEncounterList?.FirstOrDefault(x => x.StageId == stageId);
            if (stage == null)
            {
                Debug.LogError($"Stage ID {stageId} is not configured in EncounterData.", this);
                return null;
            }

            var encounters = mapNode.NodeType == EncounterMapNodeType.Boss
                ? stage.BossEncounterList
                : stage.EnemyEncounterList;
            if (mapNode.NodeType == EncounterMapNodeType.Encounter && EncounterRandomlyAtStage)
            {
                if (encounters != null && encounters.Count > 0)
                    return encounters.RandomItem();

                Debug.LogError($"Stage {stageId} has no regular encounters configured.", this);
                return null;
            }

            if (encounters == null || mapNode.EncounterIndex < 0 || mapNode.EncounterIndex >= encounters.Count)
            {
                Debug.LogError(
                    $"Map node '{mapNode.NodeId}' references encounter index {mapNode.EncounterIndex}, " +
                    $"which is not configured for stage {stageId}.", this);
                return null;
            }

            return encounters[mapNode.EncounterIndex];
        }
        
    }


    [Serializable]
    public class EnemyEncounterStage
    {
        [SerializeField] private string name;
        [SerializeField] private int stageId;
        [SerializeField] private List<EnemyEncounter> bossEncounterList;
        [SerializeField] private List<EnemyEncounter> enemyEncounterList;
        [SerializeField] private string mapStartNodeId;
        [SerializeField] private List<EncounterMapNode> mapNodeList;
        public string Name => name;
        public int StageId => stageId;
        public List<EnemyEncounter> BossEncounterList => bossEncounterList;
        public List<EnemyEncounter> EnemyEncounterList => enemyEncounterList;
        public string MapStartNodeId => mapStartNodeId;
        public List<EncounterMapNode> MapNodeList => mapNodeList;
    }

    public enum EncounterMapNodeType
    {
        Encounter,
        Boss
    }

    [Serializable]
    public class EncounterMapNode
    {
        [SerializeField] private string nodeId;
        [SerializeField] private string displayName;
        [SerializeField] private EncounterMapNodeType nodeType;
        [SerializeField] private int encounterIndex;
        [SerializeField] private int roomNumber;
        [SerializeField] private Vector2 position;
        [SerializeField] private List<string> nextNodeIds;

        public string NodeId => nodeId;
        public string DisplayName => displayName;
        public EncounterMapNodeType NodeType => nodeType;
        public int EncounterIndex => encounterIndex;
        public int RoomNumber => roomNumber;
        public Vector2 Position => position;
        public List<string> NextNodeIds => nextNodeIds;
    }
    
    
    [Serializable]
    public class EnemyEncounter : EncounterBase
    {
        [SerializeField] private List<EnemyCharacterData> enemyList;
        public List<EnemyCharacterData> EnemyList => enemyList;
    }
    
    [Serializable]
    public abstract class EncounterBase
    {
        [SerializeField] private BackgroundTypes targetBackgroundType;

        public BackgroundTypes TargetBackgroundType => targetBackgroundType;
    }
}