using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// 消耗品
/// </summary>
public class Consumable : Item
{
    /// <summary>
    /// hp
    /// </summary>
    public int HP { get; set; }
    /// <summary>
    /// mp
    /// </summary>
    public int MP { get; set; }

    public Consumable(int id, string name, ItemType type, ItemQuality quality, string des, int capacity, int buyPrice, int sellPrice, string sprite, int hp, int mp)
        : base(id, name, type, quality, des, capacity, buyPrice, sellPrice, sprite)
    {
        this.HP = hp;
        this.MP = mp;
    }

    /// <summary> 
    /// 获得提示面板应该显示什么样的内容
    /// </summary>
    /// <returns></returns>
    public override string GetToolTipText()
    {
        string text = base.GetToolTipText();

        string newText = string.Format("{0}\n\n<color=blue>加血：{1}\n加蓝：{2}</color>", text, HP, MP);

        return newText;
    }
}
