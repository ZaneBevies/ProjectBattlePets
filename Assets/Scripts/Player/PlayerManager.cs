using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerManager : MonoBehaviour
{
    [Header("References")]
    public OverworldUI overworldUI;
    public GameManager GM;
    [Header("Node")]
    public Node homeNode;

    public Node currentNode;
    public Node previousNode;


    //MOVEMENT
    private bool isMoving;
    private Vector3 originalPos, targetPos;
    private float timeToMove = .2f;
    private int distanceMod = 2;


    public void StartNew()
    {
        currentNode = homeNode;
        previousNode = homeNode;
        SetUI("");
    }

    public void StartLoad(Node node, Node prevNode)
    {
        //Debug.Log("Current node: " + node.id + ". Prev node: " + prevNode.id);

        
        previousNode = prevNode;
        currentNode = node;

        if (currentNode.IsComplete())
        {
            SetUI(node.doneText);
        }
        else
        {
            SetUI(node.text);
        }
        

        transform.position = currentNode.transform.position;
    }

    public void OnRefreshNode(string text)
    {
        SetUI(text);

        GM.SaveData();
    }

    public void OnEnterNode(string text, Node node)
    {
        previousNode = currentNode;
        currentNode = node;
        SetUI(text);


        GM.SaveData();
    }

    private void SetUI(string txt)
    {
        overworldUI.SetDirections(
            currentNode.northNode, currentNode.nLevelNeeded, currentNode.nObjectiveNeeded,
            currentNode.eastNode, currentNode.eLevelNeeded, currentNode.eObjectiveNeeded,
            currentNode.southNode, currentNode.sLevelNeeded, currentNode.sObjectiveNeeded,
            currentNode.westNode, currentNode.wLevelNeeded, currentNode.wObjectiveNeeded
            );
        overworldUI.SetInteractionText(txt);
    }


    public void Interact()
    {
        currentNode.OnInteract();
    }

    public void Move(int direction)
    {
        // direction 1 = go north
        // direction 2 = go east
        // direction 3 = go south
        // direction 4 = go west

        if (isMoving) return;

        if (direction == 1)
        {
            int extraMove = currentNode.nMoveAmount;
            currentNode.OnExit(previousNode, currentNode, currentNode.northNode);

            StartCoroutine(MovePlayer(Vector3.up * (distanceMod * extraMove)));
        }

        if (direction == 2)
        {
            int extraMove = currentNode.eMoveAmount;
            currentNode.OnExit(previousNode, currentNode, currentNode.eastNode);
            StartCoroutine(MovePlayer(Vector3.right * (distanceMod * extraMove)));
        }

        if (direction == 3)
        {
            int extraMove = currentNode.sMoveAmount;
            currentNode.OnExit(previousNode, currentNode, currentNode.southNode);
            StartCoroutine(MovePlayer(Vector3.down * (distanceMod * extraMove)));
        }

        if (direction == 4)
        {
            int extraMove = currentNode.wMoveAmount;
            currentNode.OnExit(previousNode, currentNode, currentNode.westNode);
            StartCoroutine(MovePlayer(Vector3.left * (distanceMod * extraMove)));
        }
    }



    private IEnumerator MovePlayer(Vector3 direction)
    {
        isMoving = true;

        float elapsedTime = 0;

        originalPos = transform.position;
        targetPos = originalPos + direction;

        while (elapsedTime < timeToMove)
        {
            transform.position = Vector3.Lerp(originalPos, targetPos, (elapsedTime / timeToMove));
            elapsedTime += Time.deltaTime;
            yield return null;
        }

        transform.position = targetPos;

        isMoving = false;
    }
}
