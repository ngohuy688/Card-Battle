using NueGames.NueDeck.Scripts.Data.Collection;
using UnityEngine;
using UnityEngine.UI;

namespace NueGames.NueDeck.Scripts.Relic
{
    public class RelicUI : MonoBehaviour
    {
        [SerializeField] private RelicData relicData;
        [SerializeField] private Image relicImage;

        public RelicData RelicData => relicData;

        public void SetRelic(RelicData data)
        {
            if (!data)
            {
                Debug.LogError("Cannot display a null relic.", this);
                return;
            }

            if (!relicImage)
                relicImage = GetComponent<Image>();

            if (!relicImage)
            {
                Debug.LogError("RelicUI requires an Image reference.", this);
                return;
            }

            if (!data.RelicSprite)
            {
                Debug.LogError($"Relic '{data.RelicName}' does not have a sprite assigned.", data);
                relicImage.sprite = null;
                relicImage.enabled = false;
                return;
            }

            relicData = data;
            relicImage.sprite = data.RelicSprite;
            relicImage.preserveAspect = true;
            relicImage.raycastTarget = false;
            relicImage.enabled = true;
        }
    }
}