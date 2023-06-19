using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class Node : MonoBehaviour
{
    public int id;
    public string text;
    public string doneText;
    public NodeType nodeType;
    [Header("Connected Nodes - NESW")]

    [Header("NORTH")]
    public Node northNode;
    public int nLevelNeeded = 0; // if 0 is open to all levels
    public int nObjectiveNeeded = 0; // if 0, no objective is on 0 so is open always
    public int nMoveAmount = 1;
    [Header("SOUTH")]
    public Node southNode;
    public int sLevelNeeded = 0; // if 0 is open to all levels
    public int sObjectiveNeeded = 0; // if 0, no objective is on 0 so is open always
    public int sMoveAmount = 1;
    [Header("EAST")]
    public Node eastNode;
    public int eLevelNeeded = 0; // if 0 is open to all levels
    public int eObjectiveNeeded = 0; // if 0, no objective is on 0 so is open always
    public int eMoveAmount = 1;
    [Header("WEST")]
    public Node westNode;
    public int wLevelNeeded = 0; // if 0 is open to all levels
    public int wObjectiveNeeded = 0; // if 0, no objective is on 0 so is open always
    public int wMoveAmount = 1;

    [Header("Cutscenes")]
    public CutsceneSO cutscene;
    [Header("Manager")]

    public GameManager GM;

    public abstract void OnEnter(); // Is called from the last node on after passing through the exit method, sets UI to active and sets the directions and interaction buttons to that of the node

    public abstract void OnExit(Node previousNode, Node currentNode, Node newNode); // Is called when the player presses in a direction and checks for the movement of roaming enemies on the map
    public abstract void OnInteract(); // Initiates after pressing the interaction button and starts an interation based on what node the player is on

    public abstract void SetComplete(bool state);
    public abstract bool IsComplete();

    public abstract void Refresh();
}


public enum NodeType
{
    Blank,
    Battle,
    Town,
    Punk,
    Challenge
}


