using System.Collections.Generic;
using System.Text;
using NueGames.NueDeck.Scripts.Data.Collection;
using NueGames.NueDeck.Scripts.Managers;
using TMPro;
using UnityEngine;

namespace NueGames.NueDeck.Scripts.UI
{
    public class InformationCanvas : CanvasBase
    {
        [Header("Settings")] 
        [SerializeField] private GameObject randomizedDeckObject;
        [SerializeField] private TextMeshProUGUI roomTextField;
        [SerializeField] private TextMeshProUGUI goldTextField;
        [SerializeField] private TextMeshProUGUI nameTextField;
        [SerializeField] private TextMeshProUGUI healthTextField;
        [SerializeField] private TextMeshProUGUI relicsTextField;

        public GameObject RandomizedDeckObject => randomizedDeckObject;
        public TextMeshProUGUI RoomTextField => roomTextField;
        public TextMeshProUGUI GoldTextField => goldTextField;
        public TextMeshProUGUI NameTextField => nameTextField;
        public TextMeshProUGUI HealthTextField => healthTextField;
        
        
        #region Setup
        private void Awake()
        {
            ResetCanvas();
        }
        #endregion
        
        #region Public Methods
        public void SetRoomText(int roomNumber,bool useStage = false, int stageNumber = -1) => 
            RoomTextField.text = useStage ? $"Room {stageNumber}/{roomNumber}" : $"Room {roomNumber}";

        public void SetGoldText(int value)=>GoldTextField.text = $"{value}";

        public void SetNameText(string name) => NameTextField.text = $"{name}";

        public void SetHealthText(int currentHealth,int maxHealth) => HealthTextField.text = $"{currentHealth}/{maxHealth}";

        public void SetRelicsText(IList<RelicData> relics)
        {
            if (!relicsTextField)
                return;

            var text = new StringBuilder("Relics: ");
            var hasRelic = false;
            if (relics != null)
            {
                foreach (var relic in relics)
                {
                    if (!relic)
                        continue;

                    if (hasRelic)
                        text.Append(", ");
                    text.Append(relic.RelicName);
                    hasRelic = true;
                }
            }

            relicsTextField.text = hasRelic ? text.ToString() : "Relics: None";
        }

        public override void ResetCanvas()
        {
            RandomizedDeckObject.SetActive(GameManager.PersistentGameplayData.IsRandomHand);

            var allyList = GameManager.PersistentGameplayData.AllyList;
            if (allyList != null && allyList.Count > 0 && allyList[0] != null &&
                allyList[0].AllyCharacterData != null)
            {
                var characterId = allyList[0].AllyCharacterData.CharacterID;
                var healthData = GameManager.PersistentGameplayData.AllyHealthDataList != null
                    ? GameManager.PersistentGameplayData.AllyHealthDataList.Find(x => x.CharacterId == characterId)
                    : null;
                var maxHealth = healthData != null
                    ? healthData.MaxHealth
                    : allyList[0].AllyCharacterData.MaxHealth;
                var currentHealth = healthData != null ? healthData.CurrentHealth : maxHealth;
                SetHealthText(currentHealth, maxHealth);
                SetNameText(allyList[0].AllyCharacterData.CharacterName);
            }
            else
            {
                HealthTextField.text = "--/--";
                SetNameText(GameManager.GameplayData.DefaultName);
            }

            SetRoomText(GameManager.PersistentGameplayData.CurrentEncounterId+1,GameManager.GameplayData.UseStageSystem,GameManager.PersistentGameplayData.CurrentStageId+1);
            UIManager.InformationCanvas.SetGoldText(GameManager.PersistentGameplayData.CurrentGold);
            SetRelicsText(GameManager.PersistentGameplayData.CurrentRelicsList);
        }
        #endregion
        
    }
}