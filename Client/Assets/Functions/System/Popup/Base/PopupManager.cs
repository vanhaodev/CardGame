using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using Cysharp.Threading.Tasks;
using Globals;
using Sirenix.OdinInspector;
using UnityEngine;
using Utils;
using World.TheCard;

namespace Popup
{
    public partial class PopupManager : MonoBehaviour, IGlobal
    {
        [Button]
        public async void ShowCard(CardModel cardModel)
        {
            var pop = GetPopup<PopupCard>() as PopupCard;
            pop.SetupCard(cardModel);
            await pop.SetupData();
            pop.gameObject.SetActive(true);
            pop.Show(1).Forget();
        }

        private HashSet<string> _toastContents = new HashSet<string>();

        [Button]
        public async void ShowToast(string content, PopupToastSoundType soundType = PopupToastSoundType.None)
        {
            if (soundType != PopupToastSoundType.None)
            {
                Global.Instance.Get<SoundManager>().PlaySoundOneShot("FX_" + soundType.ToString());
            }

            if (!_toastContents.Add(content)) return;
            var pop = GetPopup<PopupToast>() as PopupToast;
            pop.OnHide = () => _toastContents.Remove(content);
            pop.SetContent(content);
            await pop.SetupData();
            pop.gameObject.SetActive(true);
            pop.Show(0.3f).Forget();
        }
    }

    public partial class PopupManager : MonoBehaviour, IGlobal
    {
        [SerializeField] private List<Popup> _prefabPopups = new List<Popup>();

        // Dùng Type thay vì instance làm key
        private ConcurrentDictionary<Type, DynamicObjectPool<Popup>> _popupPools = new();

        /// <summary>
        /// Lấy prefab theo type cụ thể
        /// </summary>
        private Popup GetPrefabForType<T>() where T : Popup
        {
            var prefab = _prefabPopups.FirstOrDefault(p => p is T);
            if (prefab == null)
            {
                throw new Exception($"No prefab found for popup type {typeof(T).Name}");
            }

            return prefab;
        }

        /// <summary>
        /// Get popup từ pool (hoặc tạo mới pool nếu chưa có)
        /// </summary>
        public Popup GetPopup<T>() where T : Popup
        {
            var type = typeof(T);
            if (_popupPools.TryGetValue(type, out var pool))
            {
                return pool.Get();
            }
            else
            {
                var prefab = GetPrefabForType<T>();
                var newPool = new DynamicObjectPool<Popup>(
                    createFunc: () =>
                    {
                        var instance = Instantiate(prefab, transform);
                        return instance.GetComponent<Popup>();
                    },
                    resetAction: x => x.gameObject.SetActive(false)
                );
                _popupPools[type] = newPool;
                return newPool.Get();
            }
        }

        /// <summary>
        /// Trả popup về lại pool hoặc destroy nếu không nằm trong pool
        /// </summary>
        public void ClosePopup(Popup popup)
        {
            if (popup == null)
            {
                Debug.LogWarning("Tried to close a null popup.");
                return;
            }

            var type = popup.GetType();

            if (_popupPools.TryGetValue(type, out var pool))
            {
                pool.Put(popup);
            }
            else
            {
                Destroy(popup.gameObject);
            }
        }

        public UniTask Init() => UniTask.CompletedTask;
    }
}