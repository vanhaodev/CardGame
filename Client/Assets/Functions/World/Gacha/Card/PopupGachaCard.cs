using System;
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
using Utils.Tab;
using World.Player.Character;
using Random = Unity.Mathematics.Random;

namespace Functions.World.Gacha
{
    public class PopupGachaCard : PopupGacha
    {
        [BoxGroup("General")] [SerializeField] private AssetReferenceWithPath _assetRefSpriteBanner;
        [BoxGroup("General")] [SerializeField] private Image _imageDNAIconNormal;
        [BoxGroup("General")] [SerializeField] private Image _imageDNAIconStandard;
        [BoxGroup("General")] [SerializeField] private Image _imageDNAIconDeluxe;
        [BoxGroup("General")] [SerializeField] protected TextMeshProUGUI _txDNANormalAmount;
        [BoxGroup("General")] [SerializeField] protected TextMeshProUGUI _txDNAStandardAmount;
        [BoxGroup("General")] [SerializeField] protected TextMeshProUGUI _txDNADeluxeAmount;
        [BoxGroup("General")] [SerializeField] private Image _imageBanner;

        [BoxGroup("Tab")] [SerializeField] private AssetReferenceWithPath _assetRefSpriteTabNormal;
        [BoxGroup("Tab")] [SerializeField] private AssetReferenceWithPath _assetRefSpriteTabStandard;
        [BoxGroup("Tab")] [SerializeField] private AssetReferenceWithPath _assetRefSpriteTabDeluxe;
        [BoxGroup("Tab")] [SerializeField] private Image _imageTabNormal;
        [BoxGroup("Tab")] [SerializeField] private Image _imageTabStandard;
        [BoxGroup("Tab")] [SerializeField] private Image _imageTabDeluxe;

        [BoxGroup("Result")] [SerializeField] private GachaCardResultManager _resultManager;

        public override async UniTask SetupData()
        {
            await base.SetupData();

            var (iconNormal, iconStandard, iconDeluxe) = await UniTask.WhenAll(
                Global.Instance.Get<GameConfig>().GetItemIcon(1),
                Global.Instance.Get<GameConfig>().GetItemIcon(2),
                Global.Instance.Get<GameConfig>().GetItemIcon(3)
            );

            _imageDNAIconNormal.sprite = iconNormal;
            _imageDNAIconStandard.sprite = iconStandard;
            _imageDNAIconDeluxe.sprite = iconDeluxe;

            InitInventoryDNA();
        }


        public override async UniTask Show(float fadeDuration = 0.3f)
        {
            var (banner, tab1, tab2, tab3) = await UniTask.WhenAll(
                _assetRefSpriteBanner.AssetRef.LoadAssetAsync<Sprite>().ToUniTask(),
                _assetRefSpriteTabNormal.AssetRef.LoadAssetAsync<Sprite>().ToUniTask(),
                _assetRefSpriteTabStandard.AssetRef.LoadAssetAsync<Sprite>().ToUniTask(),
                _assetRefSpriteTabDeluxe.AssetRef.LoadAssetAsync<Sprite>().ToUniTask()
            );

            _imageBanner.sprite = banner;
            _imageTabNormal.sprite = tab1;
            _imageTabStandard.sprite = tab2;
            _imageTabDeluxe.sprite = tab3;
            OnHide += () =>
            {
                _imageBanner.sprite = null;
                _imageTabNormal.sprite = null;
                _imageTabStandard.sprite = null;
                _imageTabDeluxe.sprite = null;

                _assetRefSpriteBanner.AssetRef.ReleaseAsset();
                _assetRefSpriteTabNormal.AssetRef.ReleaseAsset();
                _assetRefSpriteTabStandard.AssetRef.ReleaseAsset();
                _assetRefSpriteTabDeluxe.AssetRef.ReleaseAsset();
            };
            await base.Show(fadeDuration);
        }

        public override void Close(float fadeDuration = 0.3f)
        {
            base.Close(fadeDuration);
        }

        private void InitInventoryDNA()
        {
            var charData = Global.Instance.Get<CharacterData>();
            var items = charData.CharacterModel.Inventory.Items;

            _txDNANormalAmount.text = (items.Find(i => i.Item.TemplateId == 1)?.Quantity.ToString()) ?? "0";
            _txDNAStandardAmount.text = (items.Find(i => i.Item.TemplateId == 2)?.Quantity.ToString()) ?? "0";
            _txDNADeluxeAmount.text = (items.Find(i => i.Item.TemplateId == 3)?.Quantity.ToString()) ?? "0";
        }


        protected override async void OnTabSwitched(int index)
        {
            base.OnTabSwitched(index);
            switch (index)
            {
                case 0:
                {
                    _imagePlayGachaPriceIconX1.sprite = await Global.Instance.Get<GameConfig>().GetItemIcon(1);
                    _imagePlayGachaPriceIconX10.sprite = await Global.Instance.Get<GameConfig>().GetItemIcon(1);
                    break;
                }
                case 1:
                {
                    _imagePlayGachaPriceIconX1.sprite = await Global.Instance.Get<GameConfig>().GetItemIcon(2);
                    _imagePlayGachaPriceIconX10.sprite = await Global.Instance.Get<GameConfig>().GetItemIcon(2);
                    break;
                }
                case 2:
                {
                    _imagePlayGachaPriceIconX1.sprite = await Global.Instance.Get<GameConfig>().GetItemIcon(3);
                    _imagePlayGachaPriceIconX10.sprite = await Global.Instance.Get<GameConfig>().GetItemIcon(3);
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
                        await Global.Instance.Get<GameConfig>().GetCardGacha(_tabSwitcher.CurrentIndex + 1);
                    var result = gachaConfig[_gachaController.PlayGacha(gachaConfig.Cast<GachaResultModel>().ToList())];
                    var cardResultTempId = GetRandomCardTemplateId(result.CardTemplateIds);
                    var gachaCardRewardModel = new GachaCardRewardModel();
                    if (result.IsHaveCard)
                    {
                        gachaCardRewardModel.Card =
                            charData.CharacterModel.Inventory.AddNewCard(cardResultTempId, result.Quantity);
                        gachaCardRewardModel.Quantity = 1;
                    }
                    else //shard
                    {
                        var cardTemplate = await Global.Instance.Get<GameConfig>().GetCardTemplate(cardResultTempId);
                        gachaCardRewardModel.ShardModel =
                            charData.CharacterModel.Inventory.AddNewCardShard(cardTemplate.ShardId, result.Quantity);
                        gachaCardRewardModel.Quantity = result.Quantity;
                    }

                    _resultManager.Show(new List<GachaCardRewardModel>() { gachaCardRewardModel });
                    Debug.Log(JsonConvert.SerializeObject(result));
                    Debug.Log(JsonConvert.SerializeObject(gachaCardRewardModel));
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
                        await Global.Instance.Get<GameConfig>().GetCardGacha(_tabSwitcher.CurrentIndex + 1);
                    var results = new List<GachaCardModel>();
                    var gachaCardRewardModels = new List<GachaCardRewardModel>();
                    for (int i = 0; i < 10; i++)
                    {
                        var result =
                            gachaConfig[_gachaController.PlayGacha(gachaConfig.Cast<GachaResultModel>().ToList())];
                        results.Add(result);
                    }

                    foreach (var result in results)
                    {
                        var cardResultTempId = GetRandomCardTemplateId(result.CardTemplateIds);
                        var gachaCardRewardModel = new GachaCardRewardModel();
                        if (result.IsHaveCard)
                        {
                            gachaCardRewardModel.Card =
                                charData.CharacterModel.Inventory.AddNewCard(cardResultTempId, result.Quantity);
                            gachaCardRewardModel.Quantity = 1;
                        }
                        else //shard
                        {
                            var cardTemplate =
                                await Global.Instance.Get<GameConfig>().GetCardTemplate(cardResultTempId);
                            gachaCardRewardModel.ShardModel =
                                charData.CharacterModel.Inventory.AddNewCardShard(cardTemplate.ShardId,
                                    result.Quantity);
                            gachaCardRewardModel.Quantity = result.Quantity;
                        }

                        gachaCardRewardModels.Add(gachaCardRewardModel);
                    }

                    Debug.Log(JsonConvert.SerializeObject(results));
                    _resultManager.Show(gachaCardRewardModels);
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

        private uint GetRandomCardTemplateId(List<uint> templateIds)
        {
            return templateIds[UnityEngine.Random.Range(0, templateIds.Count)];
        }

        private uint GetItemNeedTemplateId()
        {
            uint itemTemplateId = 1;
            if (_tabSwitcher.CurrentIndex == 1) itemTemplateId = 2;
            else if (_tabSwitcher.CurrentIndex == 2) itemTemplateId = 3;

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
                Global.Instance.Get<PopupManager>().ShowToast("You don't have enough DNA");
                return false;
            }

            // Đủ rồi, trừ số lượng item luôn
            itemEntry.Quantity -= quantityNeed;

            // Nếu có event hoặc update UI khi thay đổi kho, nhớ gọi ở đây (nếu có)
            // Ví dụ: charData.CharacterModel.Inventory.OnInventoryChanged?.Invoke();

            // Nếu bạn có method cập nhật lại giao diện kho thì gọi ở đây
            InitInventoryDNA();

            return true;
        }
    }
}