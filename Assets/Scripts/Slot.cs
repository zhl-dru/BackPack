using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

/// <summary>
/// 物品槽
/// </summary>
public class Slot : MonoBehaviour,IPointerEnterHandler,IPointerExitHandler,IPointerDownHandler
{
    public GameObject itemPrefab;
    /// <summary>
    /// 存入物品
    /// </summary>
    /// <param name="item"></param>
    public void StoreItem(Item item)
    {
        if (transform.childCount == 0)
        {
            GameObject itemGameObject = Instantiate(itemPrefab) as GameObject;
            itemGameObject.transform.SetParent(this.transform);
            itemGameObject.transform.localScale = Vector3.one;
            itemGameObject.transform.localPosition = Vector3.zero;
            itemGameObject.GetComponent<ItemUI>().SetItem(item);
        }
        else
        {
            transform.GetChild(0).GetComponent<ItemUI>().AddAmount();
        }
    }
    /// <summary>
    /// 获得当前物品槽存储的物品类型
    /// </summary>
    /// <returns></returns>
    public ItemType GetItemType()
    {
        return transform.GetChild(0).GetComponent<ItemUI>().Item.Type;
    }
    /// <summary>
    /// 获得当前物品槽存储的物品Id
    /// </summary>
    /// <returns></returns>
    public int GetItemId()
    {
        return transform.GetChild(0).GetComponent<ItemUI>().Item.ID;
    }
    /// <summary>
    /// 是否填充
    /// </summary>
    /// <returns></returns>
    public bool isFilled()
    {
        ItemUI itemUI = transform.GetChild(0).GetComponent<ItemUI>();
        return itemUI.Amount >= itemUI.Item.Capacity;
    }

    /// <summary>
    /// 鼠标移入
    /// </summary>
    /// <param name="eventData"></param>
    public void OnPointerEnter(PointerEventData eventData)
    {
        
        if (transform.childCount > 0)
        {
            string toolTipText = transform.GetChild(0).GetComponent<ItemUI>().Item.GetToolTipText();
            InventoryManager.Instance.ShowToolTip(toolTipText);
        }
    }
    /// <summary>
    /// 鼠标移出
    /// </summary>
    /// <param name="eventData"></param>
    public void OnPointerExit(PointerEventData eventData)
    {
        if (transform.childCount > 0)
        {
            InventoryManager.Instance.HideToolTip();
        }
    }
    /// <summary>
    /// 鼠标按下
    /// </summary>
    /// <param name="eventData"></param>
    public virtual void OnPointerDown(PointerEventData eventData)
    {
        if (eventData.button == PointerEventData.InputButton.Right)
        {
            if (InventoryManager.Instance.IsPickedItem == false && transform.childCount > 0)
            {
                ItemUI currentItemUI = transform.GetChild(0).GetComponent<ItemUI>();
                if (currentItemUI.Item is Equipment || currentItemUI.Item is Weapon)
                {
                    currentItemUI.AddAmount(-1);
                    Item currentItem = currentItemUI.Item;
                    if (currentItemUI.Amount <= 0)
                    {
                        DestroyImmediate(currentItemUI.gameObject);
                        InventoryManager.Instance.HideToolTip();
                    }
                    CharacterPanel.Instance.PutOn(currentItem);
                }
            }
        }

        if (eventData.button != PointerEventData.InputButton.Left) return;

        if (transform.childCount > 0)//点击的物品槽不为空
        {
            ItemUI currentItem = transform.GetChild(0).GetComponent<ItemUI>();

            if (InventoryManager.Instance.IsPickedItem == false)//没有已选中的物品
            {
                if (Input.GetKey(KeyCode.LeftControl))//按下ctrl,选中一半物品
                {
                    int amountPicked = (currentItem.Amount + 1) / 2;
                    InventoryManager.Instance.PickupItem(currentItem.Item, amountPicked);
                    if (currentItem.Amount <= amountPicked)
                    {
                        Destroy(currentItem.gameObject);
                    }
                    else
                    {
                        currentItem.AddAmount(-amountPicked); 
                    }

                }
                else//没有按下ctrl,选中所有物品
                {
                    InventoryManager.Instance.PickupItem(currentItem.Item, currentItem.Amount);
                    Destroy(currentItem.gameObject);
                }
            }
            else//有已经选中物品
            {
                if (currentItem.Item.ID == InventoryManager.Instance.PickedItem.Item.ID)//已选中物品id与格子中物品id相同
                {
                    if (currentItem.Item.Capacity > currentItem.Amount)//有空余
                    {
                        if (Input.GetKey(KeyCode.LeftControl))//按下ctrl,放置一个
                        {
                            currentItem.AddAmount();
                            InventoryManager.Instance.RemoveItem();
                        }
                        else//没有按下ctrl,尽量放下所有
                        {
                            int amountRemain = currentItem.Item.Capacity - currentItem.Amount;
                            if (amountRemain >= InventoryManager.Instance.PickedItem.Amount)//剩余容量大于等于已选中,放下所有
                            {
                                currentItem.AddAmount(InventoryManager.Instance.PickedItem.Amount);
                                InventoryManager.Instance.RemoveItem(InventoryManager.Instance.PickedItem.Amount);
                            }
                            else//剩余容量不足已选中,放下相当于剩余容量的数量
                            {
                                currentItem.AddAmount(amountRemain);
                                InventoryManager.Instance.RemoveItem(amountRemain);
                            }
                        }
                    }
                    else//没有空余
                    {
                        return;
                    }
                }
                else//已选中物品id与格子中的物品id不同
                {
                    //Item item = currentItem.Item;
                    //int amount = currentItem.Amount;
                    //currentItem.SetItem(InventoryManager.Instance.PickedItem.Item, InventoryManager.Instance.PickedItem.Amount);
                    //InventoryManager.Instance.PickedItem.SetItem(item, amount);
                    InventoryManager.Instance.PickedItem.Exchange(currentItem);
                }
            }
        }
        else//点击的物品槽为空
        {
            if (InventoryManager.Instance.IsPickedItem == true)//有已选中物品
            {
                if (Input.GetKey(KeyCode.LeftControl))//按下ctrl,放下一个
                {
                    this.StoreItem(InventoryManager.Instance.PickedItem.Item);
                    InventoryManager.Instance.RemoveItem();
                }
                else//没有按下ctrl,放下所有
                {

                    for (int i = 0; i < InventoryManager.Instance.PickedItem.Amount; i++)
                    {
                        this.StoreItem(InventoryManager.Instance.PickedItem.Item);
                    }
                    InventoryManager.Instance.RemoveItem(InventoryManager.Instance.PickedItem.Amount);
                }
            }
            else//没有已选中物品
            {
                return;
            }
        }
    }
}
