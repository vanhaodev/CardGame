using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Serialization;
using UnityEngine.UI;

namespace World.Board
{
    public class ActorTurnOrderItemUI : MonoBehaviour
    {
        //-------------- Comp -------------\\
        [SerializeField] Image _imageCharacterAvatar;
        [SerializeField] private GameObject _objDieMask;
        /// <summary>
        /// Phe ta (1) thì màu xanh lá, phe địch (2) màu đỏ
        /// </summary>
        [SerializeField] private Image[] _imageFactionTags;

        //------------- Data ------------\\
        [SerializeField] int _factionIndex;
        [SerializeField] int _memberIndex;

        public void Setup(int factionIndex, int memberIndex, Sprite image)
        {
            _factionIndex = factionIndex;
            _memberIndex = memberIndex;
            _imageCharacterAvatar.sprite = image;

            _imageFactionTags[0].gameObject.SetActive(factionIndex == 1);
            _imageFactionTags[1].gameObject.SetActive(factionIndex == 2);
            SetDieMask(false);
        }

        public void Show(bool isShow = true)
        {
            if (!isShow)
            {
                _imageCharacterAvatar.sprite = null;
            }

            gameObject.SetActive(isShow);
        }

        public bool IsCurrent(int factionIndex, int memberIndex)
        {
            return _factionIndex == factionIndex && _memberIndex == memberIndex;
        }

        public bool IsThis(int factionIndex, int memberIndex)
        {
            return _factionIndex == factionIndex && _memberIndex == memberIndex;
        }
        public void SetDieMask(bool isDie = true)
        {
            _objDieMask.SetActive(isDie);
        }
    }
}