using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace KWY
{
    [RequireComponent(typeof(CanvasRenderer))]
    public class CharacterInfoPanel : MonoBehaviour, IInstantiatableUI
    {
        [SerializeField]
        Image icon;

        [SerializeField]
        TMP_Text exLabel;

        [SerializeField]
        TMP_Text nameLabel;

        [SerializeField]
        TMP_Text atkLabel;
        [SerializeField]
        TMP_Text hpLabel;
        [SerializeField]
        TMP_Text spdLabel;

        [SerializeField]
        Image passiveIcon;

        [SerializeField]
        TMP_Text passiveExLabel;

        public void SetData(CharacterBase cb)
        {
            icon.sprite = cb.icon;
            exLabel.text = cb.characterEx;
            nameLabel.text = cb.characterName;
            atkLabel.text = cb.atk.ToString();
            hpLabel.text = cb.hp.ToString();
            spdLabel.text = cb.spd.ToString();
            passiveIcon.sprite = cb.passiveIcon;
            passiveExLabel.text = cb.passiveEx;
        }

        public void Init()
        {
            // empty
        }

        public void OnClickClose()
        {
            Destroy(gameObject);
        }
    }
}
