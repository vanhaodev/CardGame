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
            /*
            // Scale lại progress để khi 99% thì vẫn chỉ hiển thị như ~80%
            // vì viền UI bị che nên tiến độ ở 99 như 100
            float visualProgress = Mathf.Pow(progress / 100f, 0.35f); // Exponent < 1 sẽ làm tăng tốc độ ban đầu và chậm dần về cuối (càng nhỏ thì càng chậm)

            _imgUnableCoverBlock.fillAmount = 1f - visualProgress;
            bool isReady = progress >= 100f;
            _objUseableContainer.SetActive(isReady);
            _objRaycast.SetActive(isReady);
            */
            _imgUnableCoverBlock.fillAmount = progress >= 100f ? 0f : 1;
            bool isReady = progress >= 100f;
            _objUseableContainer.SetActive(isReady);
            _objRaycast.SetActive(isReady);
        }
    }
}