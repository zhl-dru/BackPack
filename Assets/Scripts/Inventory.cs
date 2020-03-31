using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using System.Text;


public class Inventory : MonoBehaviour
{
    protected Slot[] slots;
    private CanvasGroup canvasGroup;
    private float targetAlpha = 1f;
    private float smoothing = 4f;
    

    public virtual void Start()
    {
        slots = GetComponentsInChildren<Slot>();
        canvasGroup = GetComponent<CanvasGroup>();
    }
    private void Update()
    {
        if (canvasGroup.alpha != targetAlpha)
        {
            canvasGroup.alpha = Mathf.Lerp(canvasGroup.alpha, targetAlpha, smoothing * Time.deltaTime);
            if (Mathf.Abs(canvasGroup.alpha - targetAlpha) < .01f)
            {
                canvasGroup.alpha = targetAlpha;
            }
        }
    }


    /// <summary>
    /// 存储物品
    /// </summary>
    /// <returns></returns>
    public bool StoreItem(int id)
    {
        Item item = InventoryManager.Instance.GetItemById(id);
        return StoreItem(item);
    }
    /// <summary>
    /// 存储物品
    /// </summary>
    /// <returns></returns>
    public bool StoreItem(Item item)
    {
        if (item == null)
        {
            Debug.LogWarning("物品不存在");
            return false;
        }

        if (item.Capacity == 1)
        {
            Slot slot = FindEmptySlot();
            if (slot == null)
            {
                Debug.LogWarning("没有空的物品槽");
                return false;
            }
            else
            {
                slot.StoreItem(item);
                return true;
            }
        }
        else
        {
            Slot slot = FindSameIdSlot(item);
            if (slot != null)
            {
                slot.StoreItem(item);
                return true;
            }
            else
            {
                Slot emptySlot = FindEmptySlot();
                if (emptySlot != null)
                {
                    emptySlot.StoreItem(item);
                    return true;
                }
                else
                {
                    Debug.LogWarning("没有空的物品槽");
                    return false;
                }
            }
        }
    }
    /// <summary>
    /// 找到一个空的物品槽
    /// </summary>
    /// <returns></returns>
    private Slot FindEmptySlot()
    {
        foreach(Slot slot in slots)
        {
            if (slot.transform.childCount == 0)
            {
                return slot;
            }
        }
        return null;
    }
    /// <summary>
    /// 找到存储相同物品的物品槽
    /// </summary>
    /// <param name="item"></param>
    /// <returns></returns>
    private Slot FindSameIdSlot(Item item)
    {
        foreach(Slot slot in slots)
        {
            if (slot.transform.childCount >= 1 && slot.GetItemId() == item.ID && slot.isFilled() == false)
            {
                return slot;
            }
        }
        return null;
    }
    /// <summary>
    /// 显示
    /// </summary>
    public void Show()
    {
        canvasGroup.blocksRaycasts = true;
        targetAlpha = 1;
    }
    /// <summary>
    /// 隐藏
    /// </summary>
    public void Hide()
    {
        canvasGroup.blocksRaycasts = false;
        targetAlpha = 0;
    }
    /// <summary>
    /// 显示开关
    /// </summary>
    public void DisplaySwitch()
    {
        if (targetAlpha == 0)
        {
            Show();
        }
        else
        {
            Hide();
        }
    }

    /// <summary>
    /// 保存
    /// </summary>
    public void SaveInventory()
    {
        StringBuilder sb = new StringBuilder();
        foreach (Slot slot in slots)
        {
            if (slot.transform.childCount > 0)
            {
                ItemUI itemUI = slot.transform.GetChild(0).GetComponent<ItemUI>();
                sb.Append(itemUI.Item.ID + "," + itemUI.Amount + "-");
            }
            else
            {
                sb.Append("0-");
            }
        }
        PlayerPrefs.SetString(this.gameObject.name, sb.ToString());
    }
    /// <summary>
    /// 加载
    /// </summary>
    public void LoadInventory()
    {
        if (PlayerPrefs.HasKey(this.gameObject.name) == false) return;
        string str = PlayerPrefs.GetString(this.gameObject.name);
        //print(str);
        string[] itemArray = str.Split('-');
        for (int i = 0; i < itemArray.Length - 1; i++)
        {
            string itemStr = itemArray[i];
            if (itemStr != "0")
            {
                //print(itemStr);
                string[] temp = itemStr.Split(',');
                int id = int.Parse(temp[0]);
                Item item = InventoryManager.Instance.GetItemById(id);
                int amount = int.Parse(temp[1]);
                for (int j = 0; j < amount; j++)
                {
                    slots[i].StoreItem(item);
                }
            }
        }
    }
}
