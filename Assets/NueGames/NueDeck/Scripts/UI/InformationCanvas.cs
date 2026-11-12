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

        public override void ResetCanvas()
        {
            RandomizedDeckObject.SetActive(GameManager.PersistentGameplayData.IsRandomHand);

            var allyList = GameManager.PersistentGameplayData.AllyList;
            if (allyList != null && allyList.Count > 0 && allyList[0] != null &&
                allyList[0].AllyCharacterData != null)
            {
                var maxHealth = allyList[0].AllyCharacterData.MaxHealth;
                SetHealthText(maxHealth, maxHealth);
                SetNameText(allyList[0].AllyCharacterData.CharacterName);
            }
            else
            {
                HealthTextField.text = "--/--";
                SetNameText(GameManager.GameplayData.DefaultName);
            }

            SetRoomText(GameManager.PersistentGameplayData.CurrentEncounterId+1,GameManager.GameplayData.UseStageSystem,GameManager.PersistentGameplayData.CurrentStageId+1);
            UIManager.InformationCanvas.SetGoldText(GameManager.PersistentGameplayData.CurrentGold);
        }
        #endregion
        
    }
}