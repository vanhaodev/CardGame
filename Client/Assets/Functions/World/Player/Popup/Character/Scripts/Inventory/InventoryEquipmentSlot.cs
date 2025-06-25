using TMPro;
using UnityEngine;

public class InventoryEquipmentSlot : InventoryItemSelectSlot
{
    [SerializeField] GameObject _objLevelRequirement;
    [SerializeField] TextMeshProUGUI _txLevelRequirement;
    [SerializeField] ushort _levelRequirement;

    public void InitLevelRequirement(ushort currentLevel)
    {
        _objLevelRequirement.SetActive(_levelRequirement > currentLevel);
        _objAdd.SetActive(_levelRequirement <= currentLevel);
        _btnSelect.gameObject.SetActive(_levelRequirement <= currentLevel);
        if (_levelRequirement > currentLevel)
        {
            _txLevelRequirement.text = $"Level {_levelRequirement} Required";
        }
        else
        {
            _txLevelRequirement.text = string.Empty;
        }
    }
}