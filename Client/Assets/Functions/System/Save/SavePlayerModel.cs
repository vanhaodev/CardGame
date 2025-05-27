using Functions.World.Data;
using UnityEngine.Serialization;
using World.Player.Character;

namespace Save
{
    [System.Serializable]
    public class SavePlayerModel : SaveModel
    {
        public CharacterModel CharacterModel;
        public UniqueIdentityModel UniqueIdentityModel;

        public SavePlayerModel()
        {
            DataName = "Player";
        }
        public override void SetDefault()
        {
            base.SetDefault();
            CharacterModel = new CharacterModel();
            CharacterModel.SetDefault();
            
            UniqueIdentityModel = new UniqueIdentityModel();
            UniqueIdentityModel.SetDefault();
        }
    }
}