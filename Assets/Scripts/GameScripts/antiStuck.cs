using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class antiStuck : MonoBehaviour
{
    public bool stuck;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public bool getStuck()
    {
        return stuck;
    }

    public void OnTriggerStay2D(Collider2D other)
    {
        Renderer r = GetComponent<Renderer>();
        if (other.tag != "Player"&&(other.bounds.Contains(r.bounds.min)))
            {
            Debug.Log("STUCK");
            stuck = true;
        }
        stuck = false;
    }
}
