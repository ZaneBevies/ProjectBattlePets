using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class IconRotatorItem : MonoBehaviour
{
    public Image dynamicLayer;
    public Image staticLayer;
    public Image variantLayer;
    public GameObject mainObject;

    public void SetIconImage(Sprite dynamicSprite, Sprite staticSprite, Sprite variantSprite, Color dynamicColour)
    {
        mainObject.SetActive(true);
        dynamicLayer.sprite = dynamicSprite;
        dynamicLayer.color = dynamicColour;
        staticLayer.sprite = staticSprite;
        variantLayer.sprite = variantSprite;
    }

    public void SetOff()
    {
        mainObject.SetActive(false);
    }
}
