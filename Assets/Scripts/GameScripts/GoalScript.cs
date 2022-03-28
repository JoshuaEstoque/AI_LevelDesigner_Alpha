using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GoalScript : MonoBehaviour
{
    int count = 0;
    // Use this for initialization
    void Start()
    {
        //theRB = GetComponent<Rigidbody2D>();
    }

    // Update is called once per frame
    void Update()
    {
        //theRB.velocity = new Vector2(bulletSpeed * (transform.localScale.x / 3), 0);
    }

    void OnTriggerEnter2D(Collider2D other)
    {
        if (other.tag == "Player")
        {
            
            //PLEASE REPLACE THIS LATER
            //FindObjectOfType<GameManager>().ResetStage();
        }
    }
}
