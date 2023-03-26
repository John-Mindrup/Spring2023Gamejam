using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Scale : MonoBehaviour
{

    public int weigthTrigger;
    private int curWeight;
    public bool activated;
    public bool isActive;
    private int placedWeight;
    public Sprite depressed, up;
    private SpriteRenderer sr;

    // Start is called before the first frame update
    void Start()
    {
        sr = GetComponent<SpriteRenderer>();
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        GameObject o = other.gameObject;
        if (isActive)
        {
            Block b = o.GetComponentInChildren<Block>();
            if (b != null)
            {
                b.transform.position = this.transform.position;
                b.transform.parent = this.transform;
                placedWeight += b.weight;
                Gloob gg = o.GetComponent<Gloob>();
                if (gg != null)
                    gg.carrying(false);
            }
        }
        Gloob g = o.GetComponent<Gloob>();
        if(g != null)
        {
            curWeight += g.weight;
            if ((curWeight >= weigthTrigger || placedWeight >= weigthTrigger) && activated == false)
            {
                sr.sprite = depressed;
                activated = true;
                SceneStats.Instance.locks--;
            }
                
        }
    }
    private void OnTriggerExit2D(Collider2D other)
    {
        GameObject o = other.gameObject;
        Gloob g = o.GetComponent<Gloob>();
        if (g != null)
        {
            if(isActive && curWeight >= weigthTrigger && curWeight - g.weight < weigthTrigger && placedWeight < weigthTrigger)
            {
                sr.sprite = up;
                activated = false;
                SceneStats.Instance.locks++;
                
            }
            g.StopMoving();
            curWeight -= g.weight;
            
        }
    }
}
