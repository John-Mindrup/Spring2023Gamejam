using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class Block : MonoBehaviour
{

    public int weight;
    public bool OnButton = false;
   

    private void OnTriggerEnter2D(Collider2D other)
    {
        GameObject o = other.gameObject;
        Gloob g = o.GetComponent<Gloob>();
        if (g != null && g.carriedWeight == 0 && g.weight > 4)
        {
            g.AddComponent<Block>();
            this.transform.position = o.transform.position;
            this.transform.parent = o.transform;
            g.carriedWeight = weight;
            g.SendMessage("carrying", true);
        }
    }
}
