using System.Collections.Generic;
using System.Linq;
using Cysharp.Threading.Tasks;
using GameConfigs;
using Globals;
using Newtonsoft.Json;
using Popups;
using Sirenix.OdinInspector;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using Utils;
using World.Player.Character;

namespace Functions.World.Gacha
{
    public class PopupGachaEquipment : PopupGacha
    {
        [BoxGroup("General")] [SerializeField] private AssetReferenceWithPath _assetRefSpriteBanner;
        [BoxGroup("General")] [SerializeField] private Image _imageBanner;
        [BoxGroup("General")] [SerializeField] private Image _imageIronKeyIcon;
        [BoxGroup("General")] [SerializeField] private Image _imageSilverKeyIcon;
        [BoxGroup("General")] [SerializeField] private Image _imageGoldenKeyIcon;
        [BoxGroup("General")] [SerializeField] protected TextMeshProUGUI _txIronKeyAmount;
        [BoxGroup("General")] [SerializeField] protected TextMeshProUGUI _txSilverKeyAmount;
        [BoxGroup("General")] [SerializeField] protected TextMeshProUGUI _txGoldenAmount;

        [BoxGroup("Tab")] [SerializeField] private AssetReferenceWithPath _assetRefSpriteTabIron;
        [BoxGroup("Tab")] [SerializeField] private AssetReferenceWithPath _assetRefSpriteTabSilver;
        [BoxGroup("Tab")] [SerializeField] private AssetReferenceWithPath _assetRefSpriteTabGolden;
        [BoxGroup("Tab")] [SerializeField] private Image _imageTabIron;
        [BoxGroup("Tab")] [SerializeField] private Image _imageTabSilver;
        [BoxGroup("Tab")] [SerializeField] private Image _imageTabGolden;
        [BoxGroup("Result")] [SerializeField] private GachaCardResultManager _resultManager;

        public override async UniTask SetupData()
        {
            await base.SetupData();

            var (iconNormal, iconStandard, iconDeluxe) = await UniTask.WhenAll(
                Global.Instance.Get<GameConfig>().GetItemIcon(4),
                Global.Instance.Get<GameConfig>().GetItemIcon(5),
                Global.Instance.Get<GameConfig>().GetItemIcon(6)
            );

            _imageIronKeyIcon.sprite = iconNormal;
            _imageSilverKeyIcon.sprite = iconStandard;
            _imageGoldenKeyIcon.sprite = iconDeluxe;

            InitInventoryAmount();
        }

        public override async UniTask Show(float fadeDuration = 0.3f)
        {
            var (banner, tab1, tab2, tab3) = await UniTask.WhenAll(
                _assetRefSpriteBanner.AssetRef.LoadAssetAsync<Sprite>().ToUniTask(),
                _assetRefSpriteTabIron.AssetRef.LoadAssetAsync<Sprite>().ToUniTask(),
                _assetRefSpriteTabSilver.AssetRef.LoadAssetAsync<Sprite>().ToUniTask(),
                _assetRefSpriteTabGolden.AssetRef.LoadAssetAsync<Sprite>().ToUniTask()
            );

            _imageBanner.sprite = banner;
            // _resultManager.SetFadeSprite(banner);
            _imageTabIron.sprite = tab1;
            _imageTabSilver.sprite = tab2;
            _imageTabGolden.sprite = tab3;
            OnHide += () =>
            {
                _imageBanner.sprite = null;
                // _resultManager.SetFadeSprite(null);
                _imageTabIron.sprite = null;
                _imageTabSilver.sprite = null;
                _imageTabGolden.sprite = null;

                _assetRefSpriteBanner.AssetRef.ReleaseAsset();
                _assetRefSpriteTabIron.AssetRef.ReleaseAsset();
                _assetRefSpriteTabSilver.AssetRef.ReleaseAsset();
                _assetRefSpriteTabGolden.AssetRef.ReleaseAsset();

                _resultManager.Clear();
            };
            await base.Show(fadeDuration);
        }

        private void InitInventoryAmount()
        {
            var charData = Global.Instance.Get<CharacterData>();
            var items = charData.CharacterModel.Inventory.Items;

            _txIronKeyAmount.text = (items.Find(i => i.Item.TemplateId == 4)?.Quantity.ToString()) ?? "0";
            _txSilverKeyAmount.text = (items.Find(i => i.Item.TemplateId == 5)?.Quantity.ToString()) ?? "0";
            _txGoldenAmount.text = (items.Find(i => i.Item.TemplateId == 6)?.Quantity.ToString()) ?? "0";
        }

        protected override async void OnTabSwitched(int index)
        {
            base.OnTabSwitched(index);
            switch (index)
            {
                case 0:
                {
                    _imagePlayGachaPriceIconX1.sprite = await Global.Instance.Get<GameConfig>().GetItemIcon(4);
                    _imagePlayGachaPriceIconX10.sprite = await Global.Instance.Get<GameConfig>().GetItemIcon(4);
                    break;
                }
                case 1:
                {
                    _imagePlayGachaPriceIconX1.sprite = await Global.Instance.Get<GameConfig>().GetItemIcon(5);
                    _imagePlayGachaPriceIconX10.sprite = await Global.Instance.Get<GameConfig>().GetItemIcon(5);
                    break;
                }
                case 2:
                {
                    _imagePlayGachaPriceIconX1.sprite = await Global.Instance.Get<GameConfig>().GetItemIcon(6);
                    _imagePlayGachaPriceIconX10.sprite = await Global.Instance.Get<GameConfig>().GetItemIcon(6);
                    break;
                }
            }

            _txPlayGachaPriceAmountX1.text = "1";
            _txPlayGachaPriceAmountX10.text = "10";
        }

        public override async void PlayGachaX1()
        {
            _btnPlayGachaX1.interactable = false;
            _btnPlayGachaX10.interactable = false;

            var charData = Global.Instance.Get<CharacterData>();
            uint itemTemplateId = GetItemNeedTemplateId();
            var isWeightEnough = await charData.CharacterModel.Inventory.IsWeightEnough(itemTemplateId, 1);
            //Step 1: check weight
            if (isWeightEnough)
            {
                //Step 2: check requirement
                if (CheckItemEnough(1))
                {
                    var gachaConfig =
                        await Global.Instance.Get<GameConfig>().GetEquipmentGacha(_tabSwitcher.CurrentIndex + 1);
                    var result = gachaConfig[_gachaController.PlayGacha(gachaConfig.Cast<GachaResultModel>().ToList())];
                    var resulTemplateId = GetRandomTemplateId(result.EquipmentTemplateIds);
                    var gachaRewardModel = new GachaRewardModel();
                    gachaRewardModel.ItemModel =
                        await charData.CharacterModel.Inventory.AddNewEquipment(resulTemplateId, result.Rarity);
                    gachaRewardModel.Quantity = 1;

                    _resultManager.Show(new List<GachaRewardModel>() { gachaRewardModel });
                    Debug.Log(JsonConvert.SerializeObject(result));
                    Debug.Log(JsonConvert.SerializeObject(gachaRewardModel));
                    await charData.CharacterModel.Inventory.Arrange();
                }
            }
            else
            {
                Global.Instance.Get<PopupManager>().ShowToast("You don't have enough inventory space");
            }

            _btnPlayGachaX1.interactable = true;
            _btnPlayGachaX10.interactable = true;
        }

        public override async void PlayGachaX10()
        {
            _btnPlayGachaX1.interactable = false;
            _btnPlayGachaX10.interactable = false;

            var charData = Global.Instance.Get<CharacterData>();
            uint itemTemplateId = GetItemNeedTemplateId();
            var isWeightEnough = await charData.CharacterModel.Inventory.IsWeightEnough(itemTemplateId, 10);
            //Step 1: check weight
            if (isWeightEnough)
            {
                //Step 2: check requirement
                if (CheckItemEnough(10))
                {
                    var gachaConfig =
                        await Global.Instance.Get<GameConfig>().GetEquipmentGacha(_tabSwitcher.CurrentIndex + 1);
                    var results = new List<GachaEquipmentModel>();
                    var rewardModels = new List<GachaRewardModel>();
                    for (int i = 0; i < 10; i++)
                    {
                        var result =
                            gachaConfig[_gachaController.PlayGacha(gachaConfig.Cast<GachaResultModel>().ToList())];
                        results.Add(result);
                    }

                    foreach (var result in results)
                    {
                        var gachaRewardModel = new GachaRewardModel();
                        var resulTemplateId = GetRandomTemplateId(result.EquipmentTemplateIds);
                        Debug.Log(resulTemplateId);
                        gachaRewardModel.ItemModel =
                            await charData.CharacterModel.Inventory.AddNewEquipment(resulTemplateId, result.Rarity);
                        gachaRewardModel.Quantity = 1;
                        rewardModels.Add(gachaRewardModel);
                    }

                    Debug.Log(JsonConvert.SerializeObject(results));
                    _resultManager.Show(rewardModels);
                    await charData.CharacterModel.Inventory.Arrange();
                }
            }
            else
            {
                Global.Instance.Get<PopupManager>().ShowToast("You don't have enough inventory space");
            }

            _btnPlayGachaX1.interactable = true;
            _btnPlayGachaX10.interactable = true;
        }

        private uint GetRandomTemplateId(List<uint> templateIds)
        {
            return templateIds[UnityEngine.Random.Range(0, templateIds.Count)];
        }

        private uint GetItemNeedTemplateId()
        {
            uint itemTemplateId = 4;
            if (_tabSwitcher.CurrentIndex == 1) itemTemplateId = 5;
            else if (_tabSwitcher.CurrentIndex == 2) itemTemplateId = 6;

            return itemTemplateId;
        }

        private bool CheckItemEnough(uint quantityNeed)
        {
            var charData = Global.Instance.Get<CharacterData>();
            var items = charData.CharacterModel.Inventory.Items;

            // Xác định itemTemplateId dựa vào tab hiện tại
            uint itemTemplateId = GetItemNeedTemplateId();

            // Tìm item trong kho
            var itemEntry = items.Find(i => i.Item.TemplateId == itemTemplateId);

            var currentQuantity = itemEntry?.Quantity ?? 0;

            if (currentQuantity < quantityNeed)
            {
                Global.Instance.Get<PopupManager>().ShowToast("You don't have enough Key");
                return false;
            }

            // Đủ rồi, trừ số lượng item luôn
            itemEntry.Quantity -= quantityNeed;

            // Nếu có event hoặc update UI khi thay đổi kho, nhớ gọi ở đây (nếu có)
            // Ví dụ: charData.CharacterModel.Inventory.OnInventoryChanged?.Invoke();

            // Nếu bạn có method cập nhật lại giao diện kho thì gọi ở đây
            InitInventoryAmount();

            return true;
        }
    }
}