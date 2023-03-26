using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Bucket_Hat : MonoBehaviour
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
        g.fireProof = true;
    }
}
