using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using Cysharp.Threading.Tasks;
using Functions.World.Player.Inventory;
using Globals;
using Popups.Commons.Choice;
using Sirenix.OdinInspector;
using UnityEngine;
using Utils;
using World.Player.PopupCharacter;
using World.TheCard;

namespace Popups
{
    public partial class PopupManager : MonoBehaviour, IGlobal
    {
        public async void ShowChoice(string content, List<ButtonChoiceModel> choices,
            bool isShowInput = false, string inputDefault = "", Action<Action> onCloseSetter = null)
        {
            var pop = GetPopup<PopupChoice>() as PopupChoice;
            await pop.SetupData();
            pop.Init(content, choices, isShowInput, inputDefault);
            pop.gameObject.SetActive(true);
            pop.Show().Forget();
            onCloseSetter?.Invoke(() => pop.Close());
        }

        public async void ShowItemInfo(InventoryItemModel item)
        {
            var pop = GetPopup<PopupItem>() as PopupItem;
            await pop.SetupData();
            pop.Init(item);
            pop.gameObject.SetActive(true);
            pop.Show().Forget();
        }

        public async void ShowEquipmentInfo(InventoryItemModel item)
        {
            var pop = GetPopup<PopupEquipment>() as PopupEquipment;
            await pop.SetupData();
            pop.SetItem(item);
            pop.Init(null);
            pop.gameObject.SetActive(true);
            pop.Show().Forget();
        }

        [Button]
        public async void ShowCharacter(int switchIndex = -1)
        {
            var pop = GetPopup<PopupCharacter>() as PopupCharacter;
            await pop.SetupData();
            pop.SetSwitchIndexFirst(switchIndex);
            pop.gameObject.SetActive(true);
            pop.Show().Forget();
        }

        [Button]
        public async void ShowSetting()
        {
            var pop = GetPopup<PopupSetting>() as PopupSetting;
            await pop.SetupData();
            pop.gameObject.SetActive(true);
            pop.Show().Forget();
        }

        [Button]
        public async void ShowCard(PopupCardModel model)
        {
            var pop = GetPopup<PopupCard>() as PopupCard;
            pop.SetupCard(model);
            await pop.SetupData();
            pop.gameObject.SetActive(true);
            pop.Show().Forget();
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
            pop.Show(0.2f).Forget();
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