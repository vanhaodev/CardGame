using UnityEngine;
using UnityEngine.EventSystems;

namespace Utils
{
    public class ButtonHoldUI : ButtonHold, IPointerDownHandler, IPointerUpHandler, IPointerExitHandler
    {
        [Tooltip("Khoảng cách tối đa được di chuyển để vẫn tính là click")]
        public float dragThreshold = 10f;

        private Vector2 _pointerDownPos;
        private float _pointerDownTime;

        public void OnPointerDown(PointerEventData eventData)
        {
            if (eventData.button != PointerEventData.InputButton.Left) return;

            _pointerDownPos = eventData.position;
            _pointerDownTime = Time.unscaledTime;
            BeginHold();
        }

        public void OnPointerUp(PointerEventData eventData)
        {
            float moved = Vector2.Distance(_pointerDownPos, eventData.position);
            float duration = Time.unscaledTime - _pointerDownTime;

            bool isClick = moved < dragThreshold && duration < holdTime;
            EndHold(isClick);
        }

        public void OnPointerExit(PointerEventData eventData)
        {
            CancelHold();
        }
    }
}