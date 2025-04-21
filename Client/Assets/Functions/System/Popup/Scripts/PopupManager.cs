using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using AYellowpaper.SerializedCollections;
using Cysharp.Threading.Tasks;
using Globals;
using Sirenix.OdinInspector;
using UnityEngine;
using UnityEngine.AddressableAssets;
using Utils;
using World.Card;

namespace Popup
{
    public partial class PopupManager : MonoBehaviour, IGlobal
    {
        [Button]
        public async void ShowCard(CardModel cardModel)
        {
            var pop = await GetPopup(nameof(PopupCard)) as PopupCard;
            pop.gameObject.SetActive(true);
            pop.Setup(cardModel);
            Global.Instance.Get<SoundManager>().PlaySoundOneShot("FX_Popup.wav");
        }
        public UniTask Init()
        {
            return UniTask.CompletedTask;
        }
    }

    public partial class PopupManager : MonoBehaviour, IGlobal
    {
        [SerializeField]
        private SerializedDictionary<string /*Popup class name*/, string /*addressabel path*/> _addressablePathByType
            = new SerializedDictionary<string, string>();

        private ConcurrentDictionary<string /*Popup class name*/, DynamicObjectPool<Popup>> _popupPools
            = new ConcurrentDictionary<string, DynamicObjectPool<Popup>>();

        public async UniTask<Popup> GetPopup(string classType)
        {
            if (_popupPools.TryGetValue(classType, out var pool))
            {
                return pool.Get();
            }
            else
            {
                var newPopupPool = await Addressables.LoadAssetAsync<GameObject>(_addressablePathByType[classType])
                    .ToUniTask();
                if (newPopupPool == null)
                {
                    throw new System.Exception("Failed to load popup pool because of null");
                }

                _popupPools[classType] = new DynamicObjectPool<Popup>(createFunc: () =>
                {
                    GameObject instance = Instantiate(newPopupPool, transform);
                    return instance.GetComponent<Popup>();
                }, resetAction: x => x.gameObject.SetActive(false));
                var pop = _popupPools[classType].Get();
                return pop;
            }
        }

        public void ClosePopup(Popup popup)
        {
            if (popup == null)
            {
                Debug.Log("NULL");
                throw new Exception(nameof(popup));
            }

            string popupTypeName = popup.GetType().Name; // Lấy tên class làm key
            Global.Instance.Get<SoundManager>().PlaySoundOneShot("FX_Popup.wav");
            if (_popupPools.TryGetValue(popupTypeName, out var pool))
            {
                Debug.Log("Put");
                pool.Put(popup);
            }
            else
            {
                Debug.Log("Destroy");
                Destroy(popup);
            }
        }
    }
}