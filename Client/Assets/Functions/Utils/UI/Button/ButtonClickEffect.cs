using System;
using DG.Tweening;
using Globals;
using UnityEngine;
using UnityEngine.UI;

namespace Utils
{
    [RequireComponent(typeof(Button))]
    public class ButtonClickEffect : MonoBehaviour
    {
        [SerializeField] private Button _button;
        /// <summary>
        /// Nếu button hay spam thì sẽ có cách xử lý tối ưu cho spam (không kill liên tục) <br/>
        /// không thay đổi trong runtime
        /// </summary>
        [SerializeField] private bool _isSpamClick;

        private Tween _tween = null;

        private void OnValidate()
        {
            _button = GetComponent<Button>();
        }

        private void OnEnable()
        {
            _button.onClick.AddListener(OnClick);
        }

        private void CreateEffect()
        {
            // Nếu tween chưa được tạo thì mới tạo mới
            if (_tween == null)
            {
                _tween = transform.DOPunchScale(new Vector3(0.3f, 0.3f, 0.3f), 0.3f, 1, 0)
                    .Pause(); // Dừng lại sau khi tạo, chờ play

                // Chỉ set AutoKill khi tween chưa được tạo
                _tween.SetAutoKill(!_isSpamClick); // Nếu không phải spam click, tween sẽ tự kill sau khi hoàn thành
                _tween.OnKill(()=> _tween = null);
            }
        }

        public void OnClick()
        {
            CreateEffect(); // Tạo hoặc lấy tween đã có
            _tween.Restart();
            Global.Instance.Get<SoundManager>().PlaySoundOneShot( "FX_Touch.wav");
        }

        private void OnDisable()
        {
            // Nếu tween còn sống thì sẽ tự kill nếu AutoKill được bật
            if (_tween != null)
            {
                _tween.Kill();
                _tween = null;
            }

            _button.onClick.RemoveAllListeners();
        }
    }
}