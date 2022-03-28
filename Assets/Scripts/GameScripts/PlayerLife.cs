using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class PlayerLife : MonoBehaviour
{

    public bool hasDied;
    public int health;
    public Transform spawnPoint;

    // Start is called before the first frame update
    void Start()
    {
        hasDied = false;
    }

    // Update is called once per frame
    void Update()
    {
        /*
        if(gameObject.transform.position.y < -7)
        {
            hasDied = true;
        }
        if (hasDied == true)
        {
            
            StartCoroutine("Die");
        }
        */
    }
    /*
    IEnumerator Die()
    {
        gameObject.transform.position = spawnPoint.position;
        hasDied = false;
        //SceneManager.LoadScene("Scene");
        yield return null;
    }
    */
}
