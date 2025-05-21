using System;
using System.Collections.Generic;
using Save;
using World.TheCard;

namespace World.Player.Character
{
    [Serializable]
    public class CardCollectionModel
    {
        public List<CardModel> Cards;

        public CardCollectionModel()
        {
            Cards = new List<CardModel>();
        }
        public CardModel GetCard(int id)
        {
            return Cards.Find(x => x.Id == id);
        }
    }
}