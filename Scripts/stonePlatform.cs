using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class stonePlatform : MonoBehaviour
{
    private bool canCollide = true;
    public BoxCollider2D boxCol;
    public BoxCollider2D trig;


    private void OnTriggerEnter2D(Collider2D other) 
    {
        if(other.CompareTag("Player") && canCollide) {
            GetComponent<Animator>().SetTrigger("Shake");
            StartCoroutine(Collidable());
        }
    }

    IEnumerator Collidable() {
        yield return new WaitForSeconds(2f);
        canCollide = false;
        boxCol.enabled = false;
        trig.enabled = false;
        yield return new WaitForSeconds(2.5f);
        canCollide = true;
        boxCol.enabled = true;
        trig.enabled = true;
    }
}
