/*

*/


using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : MonoBehaviour
{
    private Collision coll;

    [HideInInspector]
    
    private Rigidbody2D rb;
    private Animator animator;
    private Transform currentPlatform;

    [Space]
    [Header("Stats")]
    public float speed = 10;
    public float jumpForce = 50;
    public float slideSpeed = 5;
    public float wallJumpLerp = 10;
    public float dashSpeed = 20;

    [Space]
    [Header("Booleans")]
    public bool canMove;
    public bool wallGrab;
    public bool wallJumped;
    public bool wallSlide;
    public bool isDashing;

    [Space]
    private bool groundTouch;
    private bool hasDashed;

    public int side = 1;
    [Space]
    
    private float coyoteTimer = 0;
    private float coyoteTime =0.2f;

    private float jumpBufferTime = 0.2f;
    private float jumpBufferTimer;

    
    /* [Space] Call particle systems here for polish */


    // Start is called before the first frame update
    void Start()
    {
        coll = GetComponent<Collision>();
        rb = GetComponent<Rigidbody2D>();
        animator = GetComponent<Animator>();
    }

    // Update is called once per frame
    void Update()
    {
        float x = Input.GetAxis("Horizontal");
        float y = Input.GetAxis("Vertical");
        float xRaw = Input.GetAxisRaw("Horizontal");
        float yRaw = Input.GetAxisRaw("Vertical");
        Vector2 dir = new Vector2(x,y);

        Walk(dir);
        HandleGravity();

        if (coll.onGround)
        {
            coyoteTimer = coyoteTime;
        }
        else{
            coyoteTimer -= Time.deltaTime;
        }


        if (coll.onWall && Input.GetButton("Fire3") && canMove)
        {
            wallGrab = true;
            wallSlide = false;
        }

        if (Input.GetButtonUp("Fire3") || !coll.onWall || !canMove)
        {
            wallGrab = false;
            wallSlide = false;
        }

        if (coll.onWall && !coll.onGround)
        {
            if (x != 0 && !wallGrab)
            {
                wallSlide = true;
                WallSlide();
            }
        }

        if (coll.onWall && !coll.onGround)
        {
            if (x != 0 && !wallGrab)
            {
                wallSlide = true;
                WallSlide();
            }
        }
        if (!coll.onWall || coll.onGround)
            wallSlide = false;
        if (Input.GetButtonDown("Jump"))
        {
            jumpBufferTimer = jumpBufferTime;
            
            if (coll.onGround)
                Jump(Vector2.up, false);
            
            if (coll.onWall && !coll.onGround)
                WallJump();
        }
        else
        {
            jumpBufferTimer -= Time.deltaTime;
               
            // if (coll.onWall && !coll.onGround) FINISH THIS LATER!
        }
         
        if (jumpBufferTimer > 0 && coyoteTimer > 0)
        {
            Jump(Vector2.up, false);
            jumpBufferTimer = 0f;
        }
        

        if (Input.GetButtonDown("Fire1") && !hasDashed)
        {
            if(xRaw != 0 || yRaw != 0)
            {
                Dash(xRaw, yRaw);
            }
            
            
        }
        if (coll.onGround && !isDashing)
        {
            wallJumped = false;
            GetComponent<GroundedDash>().enabled = true;
        }

         if (wallGrab && !isDashing)
        {
            
            if(x > .2f || x < -.2f)
            rb.velocity = new Vector2(rb.velocity.x, 0);

            float speedModifier = y > 0 ? .5f : 1;

            rb.velocity = new Vector2(rb.velocity.x, y * (speed * speedModifier));
        }
        
        
        if (coll.onGround && !groundTouch)
        {
            GroundTouch();
            groundTouch = true;
        }
        
        if (!coll.onGround && groundTouch)
        {
            groundTouch = false;
        }

        if (wallGrab || wallSlide || !canMove)
            return;
        
        if (x > 0)
        {
            side = 1;
        }
        if (x < 0)
        {
            side = -1;
        }
        
        
    }

    void HandleGravity()
    {
        if (coll.onGround)
        {
            rb.gravityScale = 0;
        }
        else rb.gravityScale =3;
    }

    private void Walk(Vector2 dir)
    {
        if (!canMove)
            return;
        
        if (wallGrab)
            return;

        if (!wallJumped)
        {
            rb.velocity = new Vector2(dir.x * speed, rb.velocity.y);
        }
        else
        {
            rb.velocity = Vector2.Lerp(rb.velocity, (new Vector2(dir.x * speed, rb.velocity.y)), wallJumpLerp * Time.deltaTime);
        }
    }
    
    private void WallJump()
    {
        if ((side == 1 && coll.onRightWall) || side == -1 && !coll.onRightWall)
        {
            side *= -1;
            
        }
        StopCoroutine(DisableMovement(0));
        StartCoroutine(DisableMovement(.1f));

        Vector2 wallDir = coll.onRightWall ? Vector2.left : Vector2.right;

        Jump((Vector2.up * 1.2f + wallDir / 1), true);

        wallJumped = true;
    }

    private void WallSlide()
    {
        /* if(coll.wallSide != side)
            anim.Flip(side * -1); */
        
        if (!canMove)
            return;
        
        bool pushingWall = false;
        if((rb.velocity.x > 0 && coll.onRightWall) || (rb.velocity.x < 0 && coll.onLeftWall))
        {
            pushingWall = true;
        }
        float push = pushingWall ? 0 : rb.velocity.x;

        rb.velocity = new Vector2(push, -slideSpeed);
    }
    void GroundTouch()
    {
        hasDashed = false;
        isDashing = false;
    }

    private void Dash(float x, float y)
    {

        hasDashed = true;
        

        rb.velocity = Vector2.zero;
        Vector2 dir =  new Vector2(x, y);

        rb.velocity += dir.normalized * dashSpeed;
        StartCoroutine(DashWait());

    }
    

    // Implement any particle system for jumping here
    private void Jump(Vector2 dir, bool wall)
    {
        rb.velocity = new Vector2(rb.velocity.x, 0);
        rb.velocity += dir * jumpForce;
    }
    
    IEnumerator DashWait()
    {
        StartCoroutine(GroundDash());

        rb.gravityScale = 0;
        GetComponent<GroundedDash>().enabled = false;
        //wallJumped = true;
        isDashing = true;

        yield return new WaitForSeconds(.3f);

        rb.gravityScale = 3;
        GetComponent<GroundedDash>().enabled = true;
        wallJumped = false;
        isDashing = false;
    }

    IEnumerator GroundDash()
    {
        yield return new WaitForSeconds(.15f);
        if (coll.onGround)
            hasDashed = false;
    }

    IEnumerator DisableMovement(float time)
    {
        canMove = false;
        yield return new WaitForSeconds(time);
        canMove = true;
    }

    void RigidbodyDrag(float x)
    {
        rb.drag = x;
    }
    
    void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag("MovingPlatform"))
        {
            this.transform.parent = collision.transform;
            currentPlatform = collision.transform;
        }
    }
    
    void OnCollisionExit2D(Collision2D collision)
    {
        if (collision.transform == currentPlatform)
        {
            this.transform.parent = null;
            currentPlatform = null;
        }
    }
    



    
    
}
