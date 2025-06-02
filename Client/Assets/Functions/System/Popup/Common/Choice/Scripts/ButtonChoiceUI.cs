using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace Popups.Commons.Choice
{
    public class ButtonChoiceUI : MonoBehaviour
    {
        [SerializeField] private TextMeshProUGUI _txContent;
        [SerializeField] private Button _btn;

        public void Init(ButtonChoiceModel model, TMP_InputField input)
        {
            _txContent.text = model.Content;
            _btn.onClick.RemoveAllListeners();
            _btn.onClick.AddListener(()=>model.Action?.Invoke(input.text));
        }
    }
}