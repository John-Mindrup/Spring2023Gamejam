using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ConnectedScale : MonoBehaviour
{
    public int weigthTrigger;
    private int curWeight;
    private bool activated;
    public ConnectedScale otherScale;
    public Sprite depressed, up;
    private SpriteRenderer sr;
    // Start is called before the first frame update
    void Start()
    {
        sr = GetComponent<SpriteRenderer>();
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        GameObject o = other.gameObject;
        Gloob g = o.GetComponent<Gloob>();
        if (g != null)
        {
            curWeight += g.weight;
            if (curWeight >= weigthTrigger && otherScale.curWeight >= otherScale.weigthTrigger && activated == false)
            {
                sr.sprite = depressed;
                activated = true;
                otherScale.activated = true;
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
            curWeight -= g.weight;
        }
    }

}
