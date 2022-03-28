using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Instantiation : MonoBehaviour
{
    public GameObject gridPrefab;
    // Start is called before the first frame update
    void Start()
    {
        Instantiate(gridPrefab, new Vector3(transform.position.x+25, transform.position.y+10, transform.position.z), Quaternion.identity);
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
