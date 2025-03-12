using UnityEngine.Serialization;
using World.Player.Character;

namespace Save
{
    [System.Serializable]
    public class SavePlayerModel : SaveModel
    {
        public CharacterModel CharacterModel;

        public override void SetDefault()
        {
            base.SetDefault();
            CharacterModel = new CharacterModel();
            CharacterModel.SetDefault();
        }
    }
}