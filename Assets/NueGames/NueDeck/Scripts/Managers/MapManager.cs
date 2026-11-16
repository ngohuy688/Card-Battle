using System.Collections.Generic;
using System.Linq;
using NueGames.NueDeck.Scripts.Data.Containers;
using NueGames.NueDeck.Scripts.Enums;
using NueGames.NueDeck.Scripts.UI;
using NueGames.NueDeck.Scripts.UI.Shop;
using NueGames.NueDeck.Scripts.Utils;
using UnityEngine;
using UnityEngine.UI;

namespace NueGames.NueDeck.Scripts.Managers
{
    public class MapManager : MonoBehaviour
    {
        [SerializeField] private EncounterButton encounterButtonPrefab;

        private GameManager GameManager => GameManager.Instance;
        private UIManager UIManager => UIManager.Instance;
        private RectTransform _nodeRoot;
        
        private void Start()
        {
            BuildMap();
        }
        
        private void BuildMap()
        {
            if (!encounterButtonPrefab)
            {
                Debug.LogError("MapManager requires an EncounterButton prefab.", this);
                return;
            }

            _nodeRoot = FindNodeRoot();
            if (!_nodeRoot)
            {
                Debug.LogError("MapManager could not find the MapCanvas/EncounterParent RectTransform.", this);
                return;
            }

            var nodes = GameManager.EncounterData.GetMapNodes(GameManager.PersistentGameplayData.CurrentStageId);
            if (nodes == null)
                return;

            ClearMapRoot();
            DrawConnections(nodes);
            BuildNodes(nodes);
            UpdateRoomLabel(nodes);
        }

        private RectTransform FindNodeRoot()
        {
            foreach (var child in GetComponentsInChildren<Transform>(true))
            {
                if (child.name == "EncounterParent")
                    return child as RectTransform;
            }

            return null;
        }

        private void ClearMapRoot()
        {
            var layout = _nodeRoot.GetComponent<HorizontalOrVerticalLayoutGroup>();
            if (layout)
                layout.enabled = false;

            for (var i = _nodeRoot.childCount - 1; i >= 0; i--)
            {
                var child = _nodeRoot.GetChild(i);
                child.gameObject.SetActive(false);
                Destroy(child.gameObject);
            }
        }

        private void DrawConnections(List<EncounterMapNode> nodes)
        {
            var nodesById = nodes.ToDictionary(node => node.NodeId);
            foreach (var node in nodes)
            {
                if (node.NextNodeIds == null)
                    continue;

                foreach (var nextNodeId in node.NextNodeIds)
                {
                    if (!nodesById.TryGetValue(nextNodeId, out var nextNode))
                    {
                        Debug.LogError(
                            $"Map node '{node.NodeId}' references missing next node '{nextNodeId}'.",
                            GameManager.EncounterData);
                        continue;
                    }

                    CreateConnection(node.Position, nextNode.Position);
                }
            }
        }

        private void CreateConnection(Vector2 from, Vector2 to)
        {
            var lineObject = new GameObject("MapConnection", typeof(RectTransform), typeof(CanvasRenderer), typeof(Image));
            lineObject.transform.SetParent(_nodeRoot, false);

            var lineRect = lineObject.GetComponent<RectTransform>();
            var fromPosition = GetAnchoredPosition(from);
            var toPosition = GetAnchoredPosition(to);
            var direction = toPosition - fromPosition;

            lineRect.anchorMin = new Vector2(0.5f, 0.5f);
            lineRect.anchorMax = new Vector2(0.5f, 0.5f);
            lineRect.pivot = new Vector2(0.5f, 0.5f);
            lineRect.anchoredPosition = (fromPosition + toPosition) * 0.5f;
            lineRect.sizeDelta = new Vector2(direction.magnitude, 6f);
            lineRect.localRotation = Quaternion.Euler(0f, 0f, Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg);

            var image = lineObject.GetComponent<Image>();
            image.color = new Color(0.85f, 0.82f, 0.72f, 0.7f);
            image.raycastTarget = false;
        }

        private void BuildNodes(List<EncounterMapNode> nodes)
        {
            var progress = GameManager.PersistentGameplayData;

            foreach (var mapNode in nodes)
            {
                var nodeButton = Instantiate(encounterButtonPrefab, _nodeRoot);
                var sceneChanger = nodeButton.GetComponent<SceneChanger>();
                if (mapNode.NodeType != EncounterMapNodeType.Shop && !sceneChanger)
                {
                    Debug.LogError("An instantiated EncounterButton is missing SceneChanger.", nodeButton);
                    Destroy(nodeButton.gameObject);
                    continue;
                }

                var nodeId = mapNode.NodeId;
                nodeButton.Configure(mapNode.DisplayName, mapNode.NodeType, () =>
                {
                    if (mapNode.NodeType == EncounterMapNodeType.Shop &&
                        (!UIManager || !UIManager.ShopCanvas))
                    {
                        Debug.LogError("UIManager has no ShopCanvas assigned.", UIManager);
                        return;
                    }

                    if (!GameManager.TrySelectMapNode(nodeId))
                        return;

                    if (mapNode.NodeType == EncounterMapNodeType.Shop)
                    {
                        if (UIManager.CombatCanvas)
                            UIManager.CombatCanvas.CloseCanvas();

                        UIManager.ShopCanvas.OpenShop(mapNode.ShopContainerData, () =>
                        {
                            if (this)
                                BuildMap();
                        });
                    }
                    else
                        sceneChanger.OpenCombatScene();
                });

                var nodeRect = nodeButton.GetComponent<RectTransform>();
                nodeRect.anchorMin = new Vector2(0.5f, 0.5f);
                nodeRect.anchorMax = new Vector2(0.5f, 0.5f);
                nodeRect.pivot = new Vector2(0.5f, 0.5f);
                nodeRect.anchoredPosition = GetAnchoredPosition(mapNode.Position);
                nodeRect.sizeDelta = new Vector2(240f, 76f);

                if (progress.CompletedMapNodeIds.Contains(nodeId))
                    nodeButton.SetStatus(EncounterButtonStatus.Completed);
                else if (progress.AvailableMapNodeIds.Contains(nodeId))
                    nodeButton.SetStatus(EncounterButtonStatus.Active);
                else
                    nodeButton.SetStatus(EncounterButtonStatus.Passive);
            }
        }

        private Vector2 GetAnchoredPosition(Vector2 normalizedPosition)
        {
            var rect = _nodeRoot.rect;
            return new Vector2(
                (normalizedPosition.x - 0.5f) * rect.width,
                (normalizedPosition.y - 0.5f) * rect.height);
        }

        private void UpdateRoomLabel(List<EncounterMapNode> nodes)
        {
            var progress = GameManager.PersistentGameplayData;
            var nextNodeId = progress.AvailableMapNodeIds.FirstOrDefault();
            var nextNode = nodes.FirstOrDefault(node => node.NodeId == nextNodeId);
            var roomNumber = nextNode != null ? nextNode.RoomNumber : progress.CurrentEncounterId + 1;

            UIManager.InformationCanvas.SetRoomText(
                roomNumber,
                GameManager.GameplayData.UseStageSystem,
                progress.CurrentStageId + 1);
        }
    }
}
