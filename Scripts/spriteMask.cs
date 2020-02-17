using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class spriteMask : MonoBehaviour
{
    public GameObject player;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void LateUpdate()
    {
        Movement movement = GameObject.FindWithTag("Player").GetComponent<Movement>();
        if(!movement.gameOver) {
            transform.position = player.transform.position;
        }
        
    }
}
