using UnityEngine;
using UnityEngine.UI;

namespace World.TheCard
{
    public class CardStarUI : MonoBehaviour
    {
        // <summary>
        /// 5stars
        /// </summary>
        [SerializeField] private Image[] _imgStars;

        [SerializeField] private Sprite _spriteStar;
        [SerializeField] private Sprite _spriteNoneStar;

        public void Set(CardModel cardModel)
        {
            if (cardModel != null)
            {
                // Set star
                for (int i = 0; i < _imgStars.Length; i++)
                {
                    _imgStars[i].sprite = i < cardModel.Star ? _spriteStar : _spriteNoneStar;
                }
            }
            else
            {
                for (int i = 0; i < _imgStars.Length; i++)
                {
                    _imgStars[i].sprite = _spriteNoneStar;
                }
            }
        }
    }
}