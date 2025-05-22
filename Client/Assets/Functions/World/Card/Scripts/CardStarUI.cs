using UnityEngine;

namespace World.TheCard
{
    public class CardStarUI : MonoBehaviour
    {
        // <summary>
        /// 5stars
        /// </summary>
        [SerializeField] private GameObject[] _objStars;

        public void Set(CardModel cardModel)
        {
            if (cardModel != null)
            {
                // Set star
                for (int i = 0; i < _objStars.Length; i++)
                {
                    _objStars[i].SetActive(i < cardModel.Star);
                }
            }
            else
            {
                for (int i = 0; i < _objStars.Length; i++)
                {
                    _objStars[i].SetActive(false);
                }
            }
        }
    }

}