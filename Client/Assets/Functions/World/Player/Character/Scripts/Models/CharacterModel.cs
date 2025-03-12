using System;
using System.Collections.Generic;

namespace World.Player.Character
{
    [System.Serializable]
    public class CharacterModel
    {
        public string Id;
        public ushort AvatarId;
        public ushort AvatarFrameId;
        public string Name;
        public bool Gender;
        public DateTime Birthday;
        public string Bio;
        public List<CurrencyModel> Currencies;

        public void SetDefault()
        {
            Id = "74ehvdrv784gerg45098";
            AvatarId = 1;
            AvatarFrameId = 1;
            Name = "Player";
            Gender = true;
            Birthday = new DateTime(2001, 10, 25);
            Bio = "";
            Currencies = new List<CurrencyModel>()
            {
                new CurrencyModel()
                {
                    Type = CurrencyType.Gold,
                    Amount = 0
                },
                new CurrencyModel()
                {
                    Type = CurrencyType.Sliver,
                    Amount = 0
                }
            };
        }
    }
}