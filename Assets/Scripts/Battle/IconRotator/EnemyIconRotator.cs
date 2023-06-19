using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyIconRotator : MonoBehaviour
{
    public IconRotatorItem mainItem;

    public List<IconRotatorItem> backups;

    public void SetMonsterRotator(Monster midMonster, List<Monster> bMonsters)
    {
        Color midColour = GetColourV(midMonster.colour.colour, 1f);
        mainItem.SetIconImage(midMonster.dynamicSprite, midMonster.staticSprite, midMonster.variant.variantStillSprite, midColour);

        //Debug.Log(topMonster.name + " " + botMonster.name);

        for (int i = 0; i < backups.Count; i++)
        {
            backups[i].SetOff();
        }


        for (int i = 0; i < bMonsters.Count; i++)
        {
            Color c = GetColourV(bMonsters[i].colour.colour, .5f);
            backups[i].SetIconImage(bMonsters[i].dynamicSprite, bMonsters[i].staticSprite, bMonsters[i].variant.variantStillSprite, c);
        }

    }

    private Color GetColourV(Color colour, float value) // value 0-1 0.5 is 50
    {
        float h, s, v;
        Color returnColour;

        Color.RGBToHSV(colour, out h, out s, out v);

        returnColour = Color.HSVToRGB(h, s, value);

        return returnColour;

    }
}
