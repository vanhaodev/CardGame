using UnityEngine.EventSystems;

namespace Utils
{
    public class ButtonHoldUI : ButtonHold, IPointerDownHandler, IPointerUpHandler, IPointerExitHandler
    {
        public void OnPointerDown(PointerEventData eventData)
        {
            StartHold();
        }

        public void OnPointerUp(PointerEventData eventData)
        {
            ReleaseHold();
        }

        public void OnPointerExit(PointerEventData eventData)
        {
            _cts?.Cancel();
            isHolding = false;
        }
    }
}