using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;
public class OverworldUI : MonoBehaviour
{
    [Header("Interaction Button")]
    public Button interactionButton;
    public TextMeshProUGUI interactionText;
    [Header("Direction Butttons")]
    public Button northButton;
    public Button eastButton;
    public Button southButton;
    public Button westButton;

    [Header("Menu")]
    public GameObject menuUI;
    public GameManager GM;

    public BattleHealthBar healthBar;

    void Start()
    {
        healthBar.SetMaxHealth(100f);
        healthBar.SetHealth(GM.playerHP);
    }


    public void SetInteractionText(string text) // Sets the text on the interact button
    {
        if (text == "")
        {
            interactionText.text = text;
            interactionButton.gameObject.SetActive(false);
        }
        else
        {
            interactionButton.gameObject.SetActive(true);
            interactionText.text = text;
        }
        
    }

    public void SetDirections(Node northNode, int nLevelNeeded, int nObjNeeded, Node eastNode, int eLevelNeeded, int eObjNeeded, Node southNode, int sLevelNeeded, int sObjNeeded, Node westNode, int wLevelNeeded, int wObjNeeded) // Sets which directional buttons are active based on the nodes connected nodes
    {
        northButton.gameObject.SetActive(false);
        southButton.gameObject.SetActive(false);
        eastButton.gameObject.SetActive(false);
        westButton.gameObject.SetActive(false);

        if (northNode != null)
        {
            // if pass level + objective requirements set to true else false
            if (GM.CanPassToNode(nLevelNeeded, nObjNeeded))
            {
                if (GM.PassageClear(GM.playerManager.currentNode) || northNode == GM.playerManager.previousNode)
                {
                    northButton.gameObject.SetActive(true);
                }
            }
        }

        if (southNode != null)
        {
            // if pass level + objective requirements set to true else false
            if (GM.CanPassToNode(sLevelNeeded, sObjNeeded))
            {
                if (GM.PassageClear(GM.playerManager.currentNode) || southNode == GM.playerManager.previousNode)
                {
                    southButton.gameObject.SetActive(true);
                }
            }
        }

        if (eastNode != null)
        {
            // if pass level + objective requirements set to true else false
            //Debug.Log("objective: " + eastNode.eObjectiveNeeded);
            if (GM.CanPassToNode(eLevelNeeded, eObjNeeded))
            {
                if (GM.PassageClear(GM.playerManager.currentNode) || eastNode == GM.playerManager.previousNode)
                {
                    eastButton.gameObject.SetActive(true);
                }
                
            }
        }

        if (westNode != null)
        {
            // if pass level + objective requirements set to true else false
            if (GM.CanPassToNode(wLevelNeeded, wObjNeeded))
            {
                if (GM.PassageClear(GM.playerManager.currentNode) || westNode == GM.playerManager.previousNode)
                {
                    westButton.gameObject.SetActive(true);
                }
            }
        }

    }


    public void SetMenuOpen(bool state)
    {
        menuUI.SetActive(state);
    }
    
}
