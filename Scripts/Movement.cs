using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

public class Movement : MonoBehaviour
{

    [Header("Stats")]

    private Rigidbody2D rb;
    public float speed = 10;
    public float jumpForce;
    public float wallKickForce;
    public float slideSpeed;
    public float wallJumpLerp = 10;
    public float dashSpeed = 20;
    public int side = 1;
    public int dashCount;
    private float dirX;
    private float dirY;
    private AnimationScript anim;
    private Collision col;
    

    [Space]
    [Header("Boolean")]

    public bool canMove;
    public bool wallGrab;
    public bool wallJumped;
    public bool wallSlided;
    public bool isDashing;
    private bool groundTouch;
    public bool canDash;
    public bool airDashed;
    public bool groundDashed;
    public bool gameOver;

    [Space]
    [Header("Time")]

    public float timeBtwGroundDash;
    public float timeBtwAirDash;
    
    [Space]
    [Header("Polish")]

    public ParticleSystem dashParticle;
    public ParticleSystem jumpParticle;
    public ParticleSystem wallJumpParticle;
    public ParticleSystem slideParticle;


    // Start is called before the first frame update
    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        col = GetComponent<Collision>();
        anim = GetComponentInChildren<AnimationScript>();
    }

    // Update is called once per frame
    void Update()
    {
        if(!gameOver) 
        {
        //Preset
        float x = Input.GetAxis("Horizontal");
        float y = Input.GetAxis("Vertical");
        float xRaw = Input.GetAxisRaw("Horizontal");
        float yRaw = Input.GetAxisRaw("Vertical");
        Vector2 dir = new Vector2(x, y);

        if(col.onTop && y > 0) {
            y = 0;
        }

        Walk(dir);

        anim.SetHorizontalMovement(x, y, rb.velocity.y);
        
        wallGrab = col.onWall && Input.GetKey(KeyCode.LeftShift);

        checkSide();
        checkDash();
        
        //Particles
        if(wallSlided == true) {

            slideParticle.Play();
        } else if(wallSlided == false) {
            slideParticle.Stop();
        }
        
        
        if (col.onWall && Input.GetButton("Fire3") && canMove)
        {
            if(side != col.wallSide)
                side = side *- 1;
            wallGrab = true;
            wallSlided = false;
        }

        if (Input.GetButtonUp("Fire3") || !col.onWall || !canMove)
        {
            wallGrab = false;
            wallSlided = false;
        }
        
        //Wallgrab
        if (wallGrab && !isDashing)
        {
            rb.gravityScale = 0;
            if(x > .2f || x < -.2f)
            rb.velocity = new Vector2(rb.velocity.x, 0);

            float speedModifier = y > 0 ? .5f : 1;
            
            rb.velocity = new Vector2(rb.velocity.x, y * (speed * speedModifier));
        }else {
            rb.gravityScale = 3;
        }     

        if (Input.GetButtonDown("Jump"))
        {
            //Jump
            anim.SetTrigger("jump");

            if (col.onGround)
                Jump(Vector2.up, false);
            if (col.onWall && !col.onGround)
                wallJump();
        }

        if (Input.GetKeyDown(KeyCode.Z) && dashCount > 0 && canDash && !groundDashed && !airDashed)
        {
            //Dash
            if(xRaw == 0 && yRaw == 0) {
                dirX = side;
            }
            if(xRaw == 0 && yRaw != 0) {
                dirX = 0;
            }
            if(yRaw == 0) {
                dirY = 0;
            }
            if(xRaw > 0) {
                dirX = 1;
            }
            if(xRaw < 0) {
                dirX = -1;
            }
            if(yRaw > 0) {
                dirY = 1;
            }
            if(yRaw < 0) {
                dirY = -1;
            }

            Dash(dirX, dirY);
        }

        //Collisions

        if(col.onRightWall && side == 1 && !col.onGround) {
            side = -1;
        }
        if(col.onLeftWall && side == -1 && !col.onGround) {
            side = 1;
        }

        if(col.onWall && !col.onGround)
        {
            if (x != 0 && !wallGrab)
            {
                wallSlided = true;
                wallSlide();
            }
        } 

        if (col.onGround && !isDashing)
        {
            wallJumped = false;
            GetComponent<BetterJump>().enabled = true;
        }

        if (!col.onWall || col.onGround)
            wallSlided = false;

        if(col.onGround && dashCount == 0) {
            dashCount = 1;
        }

        if (col.onGround && !groundTouch)
        {
            GroundTouch();
            groundTouch = true;
        }

        if(!col.onGround && groundTouch)
        {
            groundTouch = false;
        }

        if (wallGrab || wallSlided || !canMove)
            return;

        //Movement
        if(x > 0 && !wallGrab && canMove && !col.onWall && !wallJumped)
        {
            side = 1;
        }else if (x < 0 && !wallGrab && canMove && !col.onWall && !wallJumped)
        {
            side = -1;
        } 

        if(xRaw > 0 && !wallGrab && canMove && !col.onWall && wallJumped)
        {
            side = 1;
        }else if (xRaw < 0 && !wallGrab && canMove && !col.onWall && !wallJumped)
        {
            side = -1;
        }

        
        }
    }

    void checkSide() {
        //Check side
        Vector3 scaler = transform.localScale;
        scaler.x = side;
        transform.localScale = scaler;
    }

    void checkDash() {
        //Check can dash
        if(airDashed || groundDashed) {
            canDash = false;
        }
        if(!airDashed && !groundDashed && !canDash) {
            canDash = true;
        }
    }

    void GroundTouch()
    {
        if(airDashed) {
            canDash = true;
            airDashed = false;
        }
        if(groundDashed) {
            canDash = true;
            groundDashed = false;
        }
        isDashing = false;
        
        jumpParticle.Play();
    }


    private void Dash(float x, float y) {
        dashCount--;
        Camera.main.transform.DOComplete();
        Camera.main.transform.DOShakePosition(.2f, .5f, 14, 90, false, true);

        StartCoroutine(DashWait(0.5f));
        isDashing = true;
        rb.velocity = Vector2.zero;
        Vector2 dir = new Vector2(x, y);

        rb.velocity += dir.normalized * dashSpeed;
    }

    private void wallSlide() {
        if (!canMove)
            return;

        bool pushingWall = false;

        if((rb.velocity.x > 0 && col.onRightWall) || (rb.velocity.x < 0 && col.onLeftWall))
        {
            pushingWall = true;
        }
        float push = pushingWall ? 0 : rb.velocity.x;

        rb.velocity = new Vector2(push, -slideSpeed);
        
    }

    private void Walk(Vector2 dir) {
        if(!canMove) 
            return;
        if(wallGrab)
            return;

        if(!wallJumped) {
            rb.velocity = (new Vector2(dir.x * speed, rb.velocity.y));
        } else{
            rb.velocity = Vector2.Lerp(rb.velocity, (new Vector2(dir.x * speed, rb.velocity.y)), .5f * Time
            .deltaTime);
        }
    }

    private void Jump(Vector2 dir, bool wall) {
        rb.velocity = new Vector2(rb.velocity.x, 0);
        if(!wall) 
        {
            rb.velocity += dir * jumpForce;
        }
        
        if(wall) {
            rb.velocity += dir * wallKickForce;
        }
    }

    private void wallJump() {
        wallJumpParticle.Play();
        if ((side == 1 && col.onRightWall) || side == -1 && !col.onRightWall)
        {
            side *= -1;
        }
        StartCoroutine(DisableMovement(0));
        StartCoroutine(DisableMovement(0.1f));
        
        Vector2 wallDir = col.onRightWall ? Vector2.left : Vector2.right;
        
        Jump((Vector2.up / 1.5f + wallDir / 1.5f), true);
        wallJumped = true;
    }
    
    void RigidbodyDrag(float x)
    {
        if(!gameOver) 
        {
            x = x/1.5f;
            rb.drag = x;
        }
        
    }
    
    public void ResetDash() {
        canDash = true;
        airDashed = false;
        groundDashed = false;
        if(dashCount == 0) {
            dashCount++;
        }

    }

    IEnumerator DisableMovement(float time)
    {
        canMove = false;
        yield return new WaitForSeconds(time);
        canMove = true;
    }

    IEnumerator DashWait(float time)
    {
        if(col.onGround) 
        {
            StartCoroutine(GroundDash());
        }
        if(!col.onGround) 
        {
            airDashed = true;
        }
        
        //FindObjectOfType<RippleEffect>().Emit(Camera.main.WorldToViewportPoint(transform.position));
        FindObjectOfType<DashEffect>().ShowGhost();
        DOVirtual.Float(14, 0, .8f, RigidbodyDrag);

        dashParticle.Play();
        rb.gravityScale = 0;
        GetComponent<BetterJump>().enabled = false;
        wallJumped = true;
        isDashing = true;   

        yield return new WaitForSeconds(timeBtwAirDash);

        dashParticle.Stop();
        rb.gravityScale = 3;
        GetComponent<BetterJump>().enabled = true;
        wallJumped = false;
        isDashing = false;
    }

    IEnumerator GroundDash()
    {
        groundDashed = true;
        yield return new WaitForSeconds(timeBtwGroundDash);
        if(col.onGround) 
        {
            groundDashed = false;
        }
        
    }
    
}
