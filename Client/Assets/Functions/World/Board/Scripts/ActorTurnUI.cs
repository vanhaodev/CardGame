using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;

namespace World.Board
{
    public class ActorTurnUI : MonoBehaviour
    {
        //-------------- Comp -------------\\
        [SerializeField] Image _characterImage;

        //------------- Data ------------\\
        [SerializeField] int _factionIndex;
        [SerializeField] int _memberIndex;

        public void Setup(int factionIndex, int memberIndex, Sprite image)
        {
            _factionIndex = factionIndex;
            _memberIndex = memberIndex;
            _characterImage.sprite = image;
        }

        public void Show(bool isShow = true)
        {
            if (!isShow)
            {
                _characterImage.sprite = null;
            }

            gameObject.SetActive(isShow);
        }

        public bool IsCurrent(int factionIndex, int memberIndex)
        {
            return _factionIndex == factionIndex && _memberIndex == memberIndex;
        }
    }
}