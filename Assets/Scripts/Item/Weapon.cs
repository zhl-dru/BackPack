using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// 武器
/// </summary>
public class Weapon : Item
{
    /// <summary>
    /// 伤害
    /// </summary>
    public int Damage { get; set; }
    /// <summary>
    /// 武器类型
    /// </summary>
    public WeaponType WpType { get; set; }



    public Weapon(int id, string name, ItemType type, ItemQuality quality, string des, int capacity, int buyPrice, int sellPrice, string sprite,
       int damage, WeaponType wpType)
        : base(id, name, type, quality, des, capacity, buyPrice, sellPrice, sprite)
    {
        this.Damage = damage;
        this.WpType = wpType;
    }


    /// <summary> 
    /// 获得提示面板应该显示什么样的内容
    /// </summary>
    /// <returns></returns>
    public override string GetToolTipText()
    {
        string text = base.GetToolTipText();

        string wpTypeText = "";

        switch (WpType)
        {
            case WeaponType.OffHand:
                wpTypeText = "副手";
                break;
            case WeaponType.MainHand:
                wpTypeText = "主手";
                break;
        }

        string newText = string.Format("{0}\n\n<color=blue>武器类型：{1}\n攻击力：{2}</color>", text, wpTypeText, Damage);

        return newText;
    }
}
