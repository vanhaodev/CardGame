using System;
using Cysharp.Threading.Tasks;
using Popups;
using Sirenix.OdinInspector;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using Utils.Tab;

namespace Functions.World.Gacha
{
    public abstract class PopupGacha : Popup
    {
        [SerializeField] protected TabSwitcher _tabSwitcher;
        protected GachaController _gachaController = null;

        [BoxGroup("Play")] [SerializeField] protected Image _imagePlayGachaPriceIconX1;
        [BoxGroup("Play")] [SerializeField] protected Image _imagePlayGachaPriceIconX10;
        [BoxGroup("Play")] [SerializeField] protected TextMeshProUGUI _txPlayGachaPriceAmountX1;
        [BoxGroup("Play")] [SerializeField] protected TextMeshProUGUI _txPlayGachaPriceAmountX10;

        [BoxGroup("Play")] [SerializeField] protected Button _btnPlayGachaX1;
        [BoxGroup("Play")] [SerializeField] protected Button _btnPlayGachaX10;

        protected virtual void OnEnable()
        {
            _tabSwitcher.OnTabSwitched += OnTabSwitched;
            _btnPlayGachaX1.onClick.AddListener(PlayGachaX1);
            _btnPlayGachaX10.onClick.AddListener(PlayGachaX10);
        }

        protected virtual void OnDisable()
        {
            _tabSwitcher.OnTabSwitched -= OnTabSwitched;
            _btnPlayGachaX1.onClick.RemoveListener(PlayGachaX1);
            _btnPlayGachaX10.onClick.RemoveListener(PlayGachaX10);
        }

        protected virtual void OnTabSwitched(int index)
        {
        }

        public override async UniTask SetupData()
        {
            await base.SetupData();
            _tabSwitcher?.Init();
            _gachaController = new GachaController();
        }

        public virtual async void PlayGachaX1()
        {
            
        }
        public virtual async void PlayGachaX10()
        {
            
        }
    }
}