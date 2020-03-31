using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ItemUI : MonoBehaviour
{
    #region Data
    public Item Item { get; private set; }
    public int Amount { get; private set; }
    #endregion
    #region UI Component
    private Image itemImage;
    private Text amountText;
    private Image ItemImage
    {
        get
        {
            if (itemImage == null)
            {
                itemImage = GetComponent<Image>();
            }
            return itemImage;
        }
    }
    private Text AmountText
    {
        get
        {
            if (amountText == null)
            {
                amountText = GetComponentInChildren<Text>();
            }
            return amountText;
        }
    }
    #endregion
    #region animationScale
    private float targetScale = 1f;
    private Vector3 animationScale = new Vector3(1.4f, 1.4f, 1.4f);
    private float smoothing = 4;
    #endregion

    private void Update()
    {
        if (transform.localScale.x != targetScale)
        {
            //动画
            float scale = Mathf.Lerp(transform.localScale.x, targetScale, smoothing * Time.deltaTime);
            transform.localScale = new Vector3(scale, scale, scale);
            if (Mathf.Abs(transform.localScale.x - targetScale) < .02f)
            {
                transform.localScale = new Vector3(targetScale, targetScale, targetScale);
            }
        }
    }

    /// <summary>
    /// 设置物品
    /// </summary>
    /// <param name="item"></param>
    /// <param name="amout"></param>
    public void SetItem(Item item, int amount = 1)
    {
        transform.localScale = animationScale;
        this.Item = item;
        this.Amount = amount;
        //upadte ui
        ItemImage.sprite = Resources.Load<Sprite>(item.Sprite);
        if (item.Capacity > 1)
        {
            AmountText.text = Amount.ToString();
        }
        else
        {
            AmountText.text = "";
        }
    }

    /// <summary>
    /// 增加数量
    /// </summary>
    /// <param name="amout"></param>
    public void AddAmount(int amout = 1)
    {
        transform.localScale = animationScale;
        this.Amount += amout;
        //upadte ui
        AmountText.text = Amount.ToString();
    }
    /// <summary>
    /// 显示
    /// </summary>
    public void Show()
    {
        gameObject.SetActive(true);
    }
    /// <summary>
    /// 隐藏
    /// </summary>
    public void Hide()
    {
        gameObject.SetActive(false);
    }
    /// <summary>
    /// 设置位置
    /// </summary>
    /// <param name="position"></param>
    public void SetLocalPosition(Vector3 position)
    {
        transform.localPosition = position;
    }
    /// <summary>
    /// 当前物品与一个物品交换
    /// </summary>
    /// <param name="itemUI"></param>
    public void Exchange(ItemUI itemUI)
    {
        Item itemTemp = itemUI.Item;
        int amountTemp = itemUI.Amount;
        itemUI.SetItem(this.Item, this.Amount);
        this.SetItem(itemTemp, amountTemp);
    }
}
