using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class ItemInspectPopup : MonoBehaviour
{
    public Image icon;
    public TextMeshProUGUI nameText;
    public TextMeshProUGUI descText;





    public void Init(MonsterItemSO item)
    {
        icon.sprite = item.icon;
        nameText.text = item.itemName;
        descText.text = item.desc;
    }
}
