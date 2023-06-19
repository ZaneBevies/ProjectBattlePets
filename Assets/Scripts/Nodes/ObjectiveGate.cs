using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ObjectiveGate : MonoBehaviour
{
    

    public Sprite open;
    public Sprite closed;

    public SpriteRenderer r;

    public void Open()
    {
        r.sprite = open;
    }

    public void Close()
    {
        r.sprite = closed;
    }

}
