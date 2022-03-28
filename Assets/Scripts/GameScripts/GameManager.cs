using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    public GameObject player;
    public Transform spawnPoint;
    // Start is called before the first frame update
    void Start()
    {
        player.transform.position = spawnPoint.position;
    }

    // Update is called once per frame
    //void Update()
    //{
        
    //}

    public void ResetStage()
    {
        player.transform.position = spawnPoint.position;
    }
}
