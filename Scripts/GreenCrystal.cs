using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GreenCrystal : MonoBehaviour
{
    private Movement movement;
    private bool canCollide = true;
    public Color normal;
    public Color off;

    private void OnTriggerEnter2D(Collider2D other) 
    {
        if(other.CompareTag("Player") && canCollide) {
            movement = other.GetComponent<Movement>();
            movement.ResetDash();
            StartCoroutine("Collidable");
        }
    }

    IEnumerator Collidable() {
        canCollide = false;
        GetComponent<SpriteRenderer>().color = off;
        yield return new WaitForSeconds(2.5f);
        canCollide = true;
        GetComponent<SpriteRenderer>().color = normal;
    }
}
