using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// 物品管理器
/// </summary>
public class InventoryManager : MonoBehaviour
{
    #region 单例模式
    private static InventoryManager _instance;

    public static InventoryManager Instance
    {
        get
        {
            if (_instance == null)
            {
                _instance = GameObject.Find("InventoryManager").GetComponent<InventoryManager>();
            }
            return _instance;
        }
    }
    #endregion

    private List<Item> itemList;

    #region ToolTip
    private ToolTip toolTip;
    private Vector2 toolTipPosionOffset = new Vector2(10, -10);
    private bool isToolTipShow = false;
    #endregion

    private Canvas canvas;

    #region PickedItem
    private bool isPickedItem = false;
    public bool IsPickedItem
    {
        get
        {
            return isPickedItem;
        }
    }

    private ItemUI pickedItem;//鼠标选中的物体
    public ItemUI PickedItem
    {
        get
        {
            return pickedItem;
        }
    }
    #endregion




    private void Awake()
    {
        ParseItemJson();
    }
    private void Start()
    {
        toolTip = GameObject.FindObjectOfType<ToolTip>();
        canvas = GameObject.Find("Canvas").GetComponent<Canvas>();
        pickedItem = GameObject.Find("PickedItem").GetComponent<ItemUI>();
        pickedItem.Hide();
    }
    private void Update()
    {
        Vector2 position;

        if (isPickedItem)
        {
            //选中物品跟随鼠标
            RectTransformUtility.ScreenPointToLocalPointInRectangle(canvas.transform as RectTransform, Input.mousePosition, null, out position);
            pickedItem.SetLocalPosition(position);
        }

        if (isPickedItem && Input.GetMouseButtonDown(0) && UnityEngine.EventSystems.EventSystem.current.IsPointerOverGameObject(-1) == false)
        {
            //丢弃物品
            isPickedItem = false;
            pickedItem.Hide();
        }
    }



    /// <summary>
    /// 解析物品信息
    /// </summary>
    void ParseItemJson()
    {
        itemList = new List<Item>();

        TextAsset itemText = Resources.Load<TextAsset>("Items");
        string itemJson = itemText.text;
        JSONObject j = new JSONObject(itemJson);

        foreach (JSONObject temp in j.list)
        {
            Item item = null;
            #region 物品公有属性
            string typeStr = temp["type"].str;
            ItemType type = (ItemType)System.Enum.Parse(typeof(ItemType), typeStr);
            int id = (int)(temp["id"].n);
            string name = temp["name"].str;
            ItemQuality quality = (ItemQuality)System.Enum.Parse(typeof(ItemQuality), temp["quality"].str);
            string description = temp["description"].str;
            int capacity = (int)(temp["capacity"].n);
            int buyPrice = (int)(temp["buyPrice"].n);
            int sellPrice = (int)(temp["sellPrice"].n);
            string sprite = temp["sprite"].str;
            #endregion
            switch (type)
            {
                #region 类型特有属性
                case ItemType.Consumable:
                    int hp = (int)(temp["hp"].n);
                    int mp = (int)(temp["mp"].n);
                    item = new Consumable(id, name, type, quality, description, capacity, buyPrice, sellPrice, sprite, hp, mp);
                    break;
                case ItemType.Equipment:
                    //
                    int strength = (int)temp["strength"].n;
                    int intellect = (int)temp["intellect"].n;
                    int agility = (int)temp["agility"].n;
                    int stamina = (int)temp["stamina"].n;
                    EquipmentType equipType = (EquipmentType)System.Enum.Parse(typeof(EquipmentType), temp["equipType"].str);
                    item = new Equipment(id, name, type, quality, description, capacity, buyPrice, sellPrice, sprite, strength, intellect, agility, stamina, equipType);
                    break;
                case ItemType.Weapon:
                    //
                    int damage = (int)temp["damage"].n;
                    WeaponType wpType = (WeaponType)System.Enum.Parse(typeof(WeaponType), temp["weaponType"].str);
                    item = new Weapon(id, name, type, quality, description, capacity, buyPrice, sellPrice, sprite, damage, wpType);
                    break;
                case ItemType.Material:
                    //
                    item = new Material(id, name, type, quality, description, capacity, buyPrice, sellPrice, sprite);
                    break;
                    #endregion
            }
            itemList.Add(item);

        }
    }

    /// <summary>
    /// 根据ID获得物品对象
    /// </summary>
    /// <param name="id"></param>
    /// <returns></returns>
    public Item GetItemById(int id)
    {
        foreach (Item item in itemList)
        {
            if (item.ID == id)
            {
                return item;
            }
        }
        return null;
    }

    /// <summary>
    /// 显示
    /// </summary>
    /// <param name="content"></param>
    public void ShowToolTip(string content)
    {
        if (this.isPickedItem) return;//捡起物品时不显示

        isToolTipShow = true;
        toolTip.Show(content);
    }

    /// <summary>
    /// 隐藏
    /// </summary>
    public void HideToolTip()
    {
        isToolTipShow = false;
        toolTip.Hide();
    }
    /// <summary>
    /// 捡起指定数量的物品
    /// </summary>
    /// <param name="itemUI"></param>
    /// <param name="amount"></param>
    public void PickupItem(Item item, int amount)
    {
        pickedItem.SetItem(item, amount);
        isPickedItem = true;

        pickedItem.Show();//显示
        this.toolTip.Hide();
        //如果我们捡起了物品，我们就要让物品跟随鼠标
        Vector2 position;
        RectTransformUtility.ScreenPointToLocalPointInRectangle(canvas.transform as RectTransform, Input.mousePosition, null, out position);
        pickedItem.SetLocalPosition(position);
    }
    /// <summary>
    /// 移除已选中的物品
    /// </summary>
    /// <param name="amount"></param>
    public void RemoveItem(int amount = 1)
    {
        PickedItem.AddAmount(-amount);
        if (pickedItem.Amount <= 0)
        {
            isPickedItem = false;
            pickedItem.Hide();
        }
    }

    /// <summary>
    /// 保存
    /// </summary>
    public void SaveInventory()
    {
        Knapsack.Instance.SaveInventory();
        Chest.Instance.SaveInventory();
        CharacterPanel.Instance.SaveInventory();
        Forge.Instance.SaveInventory();
        PlayerPrefs.SetInt("CoinAmount", GameObject.FindGameObjectWithTag("Player").GetComponent<Player>().CoinAmount);
    }
    /// <summary>
    /// 加载
    /// </summary>
    public void LoadInventory()
    {
        Knapsack.Instance.LoadInventory();
        Chest.Instance.LoadInventory();
        CharacterPanel.Instance.LoadInventory();
        Forge.Instance.LoadInventory();
        if (PlayerPrefs.HasKey("CoinAmount"))
        {
            GameObject.FindGameObjectWithTag("Player").GetComponent<Player>().CoinAmount = PlayerPrefs.GetInt("CoinAmount");
        }
    }
}
