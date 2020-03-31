using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class EquipmentSlot : Slot
{
    public EquipmentType equipmentType;
    public WeaponType weaponType;


    public override void OnPointerDown(PointerEventData eventData)
    {
        if (eventData.button == PointerEventData.InputButton.Right)//右键
        {
            if (eventData.button == PointerEventData.InputButton.Right)
            {
                if (InventoryManager.Instance.IsPickedItem == false && transform.childCount > 0)
                {
                    ItemUI currentItemUI = transform.GetChild(0).GetComponent<ItemUI>();
                    Item itemTemp = currentItemUI.Item;
                    DestroyImmediate(currentItemUI.gameObject);
                    //脱掉放到背包里面
                    transform.parent.SendMessage("PutOff", itemTemp);
                    InventoryManager.Instance.HideToolTip();
                }
            }
        }

        if (eventData.button != PointerEventData.InputButton.Left) return;

        bool isUpdateProperty = false;
        if (InventoryManager.Instance.IsPickedItem == true)//手上有东西的情况
        {
            ItemUI pickedItem = InventoryManager.Instance.PickedItem;
            if (transform.childCount > 0)//装备槽里有东西
            {
                ItemUI currentItemUI = transform.GetChild(0).GetComponent<ItemUI>();//当前装备槽里面的物品

                if (IsRightItem(pickedItem.Item))//装备适合放入
                {
                    InventoryManager.Instance.PickedItem.Exchange(currentItemUI);
                    isUpdateProperty = true;
                }
            }
            else//装备槽为空
            {
                if (IsRightItem(pickedItem.Item))
                {
                    this.StoreItem(InventoryManager.Instance.PickedItem.Item);
                    InventoryManager.Instance.RemoveItem(1);
                    isUpdateProperty = true;
                }

            }
        }
        else//手上没东西的情况
        {
            if (transform.childCount > 0)//装备槽里有东西
            {
                ItemUI currentItemUI = transform.GetChild(0).GetComponent<ItemUI>();
                InventoryManager.Instance.PickupItem(currentItemUI.Item, currentItemUI.Amount);
                DestroyImmediate(currentItemUI.gameObject);
                isUpdateProperty = true;
            }
        }

        if (isUpdateProperty)//更换装备时更新属性
        {
            transform.parent.SendMessage("UpdatePropertyText");
        }
    }

    /// <summary>
    /// 判断装备是否适合放入
    /// </summary>
    /// <param name="item"></param>
    /// <returns></returns>
    public bool IsRightItem(Item item)
    {
        if ((item is Equipment && ((Equipment)(item)).EquipType == this.equipmentType) ||
                    (item is Weapon && ((Weapon)(item)).WpType == this.weaponType))
        {
            return true;
        }
        return false;
    }
}
