using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class ItemStorageArea : MonoBehaviour, IDropHandler
{
    public CollectionInspectPanel manager;

    public void OnDrop(PointerEventData eventData)
    {
        GameObject dropped = eventData.pointerDrag;
        ItemSlotMoveable slot = dropped.GetComponent<ItemSlotMoveable>();

        manager.g.itemInspectManagerPopup.CloseCurrentPanel();

        if (slot == null) { return; }

        if (slot.type == ItemSlotType.EquipSlot)
        {

            //SPAWN ITEM IN STORAGE
            manager.g.AddItemToStorage(slot.item);

            //REMOVE ITEM FROM EQUIP SLOT
            manager.g.RemoveItemFromMonster(dropped.GetComponent<ItemEquipSlotMoveable>().itemSlotManager.slotNum);
        }

        manager.UpdateInspectPanel();
    }
}
