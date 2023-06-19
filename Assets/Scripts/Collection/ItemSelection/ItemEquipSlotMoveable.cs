using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
public class ItemEquipSlotMoveable : ItemSlotMoveable
{
    [HideInInspector]public ItemSlotManager itemSlotManager;

    public DragableItem dragableItem;

    //public int slotNum;
    public void Init(MonsterItemSO i, GameManager man, ItemSlotManager itemMan)
    {
        //slotNum = num;
        dragableItem.manager = man;
        type = ItemSlotType.EquipSlot;
        manager = man;
        itemSlotManager = itemMan;

        item = i;
        icon.sprite = item.icon;
        itemSlotManager.nameText.text = item.itemName;
        itemSlotManager.descText.text = item.desc;
    }

    public void UpdateItem()
    {
        icon.sprite = item.icon;
        itemSlotManager.nameText.text = item.name;
        itemSlotManager.descText.text = item.desc;
    }

    public override void OnClick()
    {
        itemSlotManager.panel.OpenSelectItemWindow(itemSlotManager.slotNum);
    }


}
