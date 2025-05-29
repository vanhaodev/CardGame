using System;
using System.Collections.Generic;
using UnityEngine;
using Utils.Tab;

namespace World.Player.PopupCharacter
{
    public class InventoryUI : MonoBehaviour
    {
        [SerializeField] private TabSwitcher _tabSwitcherItemType;
        [SerializeField] private InventoryItemUI _prefabInventoryItemUI;
        [SerializeField] private Transform _inventoryItemContainer;
        [SerializeField] private List<InventoryItemUI> _inventoryItemUIs;
        private void OnEnable()
        {
            _tabSwitcherItemType.OnTabSwitched += FilterItem;
        }

        private void OnDisable()
        {
            _tabSwitcherItemType.OnTabSwitched -= FilterItem;
        }

        private void FilterItem(int indexType)
        {
            foreach (var item in _inventoryItemUIs)
            {
                // item.transform.gameObject.SetActive(indexType == it);
            }   
        }
        
    }
}