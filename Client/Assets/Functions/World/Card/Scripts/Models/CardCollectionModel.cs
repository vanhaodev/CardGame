using System;
using System.Collections.Generic;
using Save;

namespace World.Card
{
    [Serializable]
    public class CardCollectionModel
    {
        public List<CardModel> Cards;

        public CardCollectionModel()
        {
            Cards = new List<CardModel>();
        }
    }
}