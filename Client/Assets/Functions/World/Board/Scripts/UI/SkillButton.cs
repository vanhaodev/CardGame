using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

namespace World.Board
{
    public class SkillButton : MonoBehaviour
    {
        [SerializeField] public Button Button;
        [SerializeField] private GameObject _objRaycast;
        [SerializeField] private Image _imgUnableCoverBlock;

        /// <summary>
        /// Chứa các hiệu ứng nổi bật khi có thể sài, ví dụ ulti có lửa cháy bắt mắt
        /// </summary>
        [SerializeField] private GameObject _objUseableContainer;

        [SerializeField] private Image _imgSkillIcon;

        public void SetSkillIcon(Sprite icon)
        {
            _imgSkillIcon.sprite = icon;
        }

        public void SetSkillUsable(float progress)
        {
            _imgUnableCoverBlock.fillAmount = 1f - (progress / 100f);
            _objUseableContainer.SetActive(progress >= 100);
            _objRaycast.SetActive(progress >= 100);
        }
    }
}