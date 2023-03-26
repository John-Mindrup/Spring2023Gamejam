using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Propellor_Hat : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
    private void OnTriggerEnter2D(Collider2D other)
    {
        Gloob g = other.gameObject.GetComponent<Gloob>();
        if(g!= null && !g.hover && g.weight == 2)
        {
            Vector2 pos = g.transform.position;
            GameObject propellor_Hat = Instantiate(this.gameObject, pos, Quaternion.identity);
            Destroy(propellor_Hat.GetComponent<Collider2D>());
            propellor_Hat.transform.parent = g.gameObject.transform;
            propellor_Hat.transform.localPosition += new Vector3(-.7f, .7f);
            g.hover = true;
        }
        
        
    }
    }
