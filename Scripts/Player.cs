using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class Player : MonoBehaviour
{
    public ParticleSystem explosion;
    private Movement movement;
    private Rigidbody2D rb;
    public Animator nextLevAnim;
    public GameObject happySquare;

    // Start is called before the first frame update
    void Start()
    {
        movement = GetComponent<Movement>();
        rb = GetComponent<Rigidbody2D>();
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    private void OnTriggerEnter2D(Collider2D other) 
    {
        if(other.CompareTag("Trap")) {
            StartCoroutine("deathCount");
        }
    }

    IEnumerator deathCount() {
        movement.gameOver = true;
        rb.velocity = Vector2.zero;
        explosion.Play();
        GetComponentInChildren<SpriteRenderer>().enabled = false; 
        yield return new WaitForSeconds(0.5f);
        SceneManager.LoadScene( SceneManager.GetActiveScene().buildIndex );
    }
}
