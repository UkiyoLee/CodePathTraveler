using UnityEngine.UI;
using UnityEngine.EventSystems;
using TMPro;
using System;
using Unity.VisualScripting;

public class ItemButton : MonoBehaviour, ISelectHandler, IDeselectHandler
{

    [SerializeField] private Image itemIcon;
    [SerializeField] private TMP_Text itemName;
    [SerializeField] private TMP_Text itemDescription;
    [SerializeField] private TMP_Text itemQuantity;
    [SerializeField] private GameObject itemTips;

    protected Button _button;
    public Button CurrentButton => _button;

    protected InventoryItem _currentItem;
    public ItemDefinitionSO CurrentItemDefinition => _currentItem?.ItemDefinition;

    private Action<ItemDefinitionSO> _onItemClick;

    protected virtual void Awake()
    {
        _button = GetComponent<Button>();
        _button.onClick.RemoveAllListeners();
        _button.onClick.AddListener(OnClick);
    }

    public virtual void SetupButton(InventoryItem item) => SetupButton(item, null);

    public virtual void SetupButton(InventoryItem item, Action<ItemDefinitionSO> onItemClick)
    {
        _currentItem = item;
        _onItemClick = onItemClick;

        itemIcon.sprite = InventoryManager.Instance.IconSet.GetIcon(item.ItemDefinition.itemIconKey);
        itemName.text = item.ItemDefinition.itemName;
        itemDescription.text = item.ItemDefinition.itemDescription;

        if (itemQuantity != null)
        {
            itemQuantity.text = item.Quantity.ToString();
        }
    }

    protected virtual void OnClick()
    {
        if (_onItemClick is not null)
        {
            _onItemClick.Invoke(CurrentItemDefinition);
        }
    }

    public void UpdateQuantity(int quantity)
    {
        if (itemQuantity != null)
        {
            itemQuantity.text = quantity.ToString();
        }
    }

    #region UI状态回调
    public void OnSelect(BaseEventData eventData)
    {
        itemTips.SetActive(true);
    }

    public void OnDeselect(BaseEventData eventData)
    {
        itemTips.SetActive(false);
    }
    #endregion
}
