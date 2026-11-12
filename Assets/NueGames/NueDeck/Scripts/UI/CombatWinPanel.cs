using TMPro;
using UnityEngine;

namespace NueGames.NueDeck.Scripts.UI
{
    public class CombatWinPanel : MonoBehaviour
    {
        [SerializeField] private TextMeshProUGUI continueButtonText;

        public void SetNextStageAvailable(bool hasNextStage)
        {
            if (!continueButtonText)
            {
                Debug.LogError("The continue button text is not assigned on CombatWinPanel.", this);
                return;
            }

            continueButtonText.text = hasNextStage ? "Continue" : "Main Menu";
        }
    }
}