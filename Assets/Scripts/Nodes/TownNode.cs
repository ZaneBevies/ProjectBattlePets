using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TownNode : Node
{
    public override void SetComplete(bool state)
    {

    }
    public override void OnEnter() // Entering node
    {
        GM.playerManager.OnEnterNode(text, this);

        if (cutscene != null)
        {
            GM.cutsceneController.PlayCutscene(cutscene);
        }
    }

    public override void OnExit(Node previousNode, Node currentNode, Node newNode)
    {
        if (previousNode == null || previousNode != newNode)
        {
            newNode.OnEnter();
        }
        else
        {
            newNode.OnEnter();
        }
    }

    public override void OnInteract()
    {
        GM.playerHP = 100f;
        GM.popupManager.FullyHealed();
        GM.overworldUI.healthBar.SetHealth(100f);
        GM.playerHomeNodeID = id;
        GM.collectionManager.OpenInterface();
    }

    public override bool IsComplete()
    {
        return false;
    }

    public override void Refresh()
    {
        GM.playerManager.OnRefreshNode(text);
    }
}
