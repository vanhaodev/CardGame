using System.Collections.Generic;
using TMPro;
using UnityEngine;

namespace Popups.Commons.Choice
{
    public class PopupChoice : Popup
    {
        [SerializeField] private TextMeshProUGUI _txContent;
        [SerializeField] private ButtonChoiceUI _prefabButton;
        [SerializeField] private Transform _transformButtonContainer;
        [SerializeField] private TMP_InputField _inputField;
        [SerializeField] private RectTransform _rectContent; //140 & 212
        [SerializeField] private List<ButtonChoiceUI> _choiceUIs;

        public void Init(string content, List<ButtonChoiceModel> choices, bool isShowInput = false,
            string inputDefault = "")
        {
            _txContent.text = content;
            //
            _inputField.gameObject.SetActive(isShowInput);
            Vector2 offsetMin = _rectContent.offsetMin;
            offsetMin.y = isShowInput ? 212f : 140f;
            _rectContent.offsetMin = offsetMin;
            _inputField.text = inputDefault;
            //
            //choice

            if (choices != null)
            {
                while (_choiceUIs.Count < choices.Count)
                {
                    var choice = Instantiate(_prefabButton, _transformButtonContainer);
                    _choiceUIs.Add(choice);
                }

                for (int i = 0; i < _choiceUIs.Count; i++)
                {
                    if (i < choices.Count)
                    {
                        _choiceUIs[i].gameObject.SetActive(true);
                        _choiceUIs[i].Init(choices[i], _inputField);
                    }
                    else
                    {
                        _choiceUIs[i].gameObject.SetActive(false);
                    }
                }
            }
            else
            {
                for (int i = 0; i < _choiceUIs.Count; i++) _choiceUIs[i].gameObject.SetActive(false);
            }
        }
    }
}