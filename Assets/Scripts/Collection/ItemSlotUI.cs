using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class ItemSlotUI : MonoBehaviour
{
    public Image iconImage;
    public TextMeshProUGUI nameText;
    public TextMeshProUGUI numberText;

    private MonsterItemSO item;
    public void Init(Sprite spr, string name, int number, MonsterItemSO i)
    {
        iconImage.sprite = spr;
        nameText.text = name;
        numberText.text = number.ToString();
        item = i;
    }

    public void SelectItem()
    {
    }
}
