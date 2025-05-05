using Cysharp.Threading.Tasks;
using Save;
using UnityEngine;

namespace Popup
{
    public class SettingController : MonoBehaviour
    {
        SaveManager _saveManager = new SaveManager();
        public async UniTask<SaveSettingSoundModel> LoadSound()
        {
            return await _saveManager.Load<SaveSettingSoundModel>();
        }
        public async UniTask<SaveSettingGraphicModel> LoadGraphic()
        {
            return await _saveManager.Load<SaveSettingGraphicModel>();
        }
        public async UniTask SaveSound(SaveSettingSoundModel model)
        {
            await _saveManager.Save(model);
        }
        public async UniTask SaveGraphic(SaveSettingGraphicModel model)
        {
            await  _saveManager.Save(model);
        }
        public SaveSettingSoundModel RestoreSoundDefault()
        {
            var model = new SaveSettingSoundModel();
            model.SetDefault();
            return model;
        }
        public SaveSettingGraphicModel RestoreGraphicDefault()
        {
            var model = new SaveSettingGraphicModel();
            model.SetDefault();
            return model;
        }
    }
}