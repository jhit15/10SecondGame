using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerMovement : MonoBehaviour
{
    private float horizontal;
    private float speed = 13f;
    private float jumpingPower = 20f;
    private bool isfacingRight = true;
    
    private bool doubleJump;
    
    private bool canDash = true;
    private bool isDashing;
    private float dashingPower = 10f;
    private float dashingTime = 0.2f;
    private float dashingCooldown = 1f;

    private bool isWallSliding;
    private float wallSlidingSpeed = 8f;

    private bool isWallJumping;
    private float wallJumpingDirection;
    private float wallJumpingTime = 0.2f;
    private float wallJumpingCounter;
    private float wallJumpingDuration = 0.4f;
    private Vector2 wallJumpingPower = new Vector2(6f,18f);

    private Animator animator;

    [SerializeField] private Rigidbody2D rb;
    [SerializeField] private Transform groundCheck;
    [SerializeField] private LayerMask groundLayer;
    [SerializeField] private TrailRenderer tr;
    [SerializeField] private Transform wallCheck;
    [SerializeField] private LayerMask wallLayer;
    [SerializeField] private AudioManager2 audioManager;
    //here

    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        if (animator == null)
        {
            animator = GetComponent<Animator>();
        }
    }
    
    // Update is called once per frame
    void Update()
    {   
        if (isDashing)
        {
            return;
        }

        // Main movement (Jump mechanic & double jump mechanic)
        horizontal = Input.GetAxisRaw("Horizontal");
        
        if (IsGrounded() && !Input.GetButton("Jump"))
        {
            doubleJump = false;
        }
        
        if (IsGrounded() && Mathf.Abs(rb.velocity.x) < 0.1f)
        {
            animator.SetTrigger("Idle");
        }
            if (Input.GetButtonDown("Jump"))
            {
              if (Mathf.Abs(horizontal) > 0f)
            {
                animator.ResetTrigger("Run");
            }
            
            if (IsGrounded() || doubleJump)
            {
                rb.velocity = new Vector2(rb.velocity.x, jumpingPower);
                animator.SetTrigger("Jump");
                doubleJump = !doubleJump;
                //here
                if (audioManager != null)
                {
                    audioManager.PlayJumpSound();
                }

            }  
        }

        // Jump is higher with hold of "jump"
        if (Input.GetButtonUp("Jump") && rb.velocity.y > 0f)
        {
            rb.velocity = new Vector2(rb.velocity.x, rb.velocity.y * 0.5f);
        }

        // Dash Keybind and routine
        if (Input.GetKeyDown(KeyCode.LeftShift) && canDash)
        {
            StartCoroutine(Dash());
        }

        //Wallslide mechanic
        WallSlide();

        //Walljump mechanic
        WallJump();

        if (!isWallJumping)
        {
            // Flip inside to make sure character doesnt flip while wall jumping
            Flip();
        }
    }

    // Movement Speed 
    private void FixedUpdate()
    {     
        if (isDashing)
        {
            return;
        }

        if (!isWallJumping)
        {
            rb.velocity = new Vector2(horizontal * speed, rb.velocity.y);
            if (Mathf.Abs(horizontal) > 0)
            {
                animator.SetTrigger("Run");
            }
            
        }

    }
    
    // Checks if character is grounded
    private bool IsGrounded()
    {   
        return Physics2D.OverlapCircle(groundCheck.position, 0.2f, groundLayer);
    }

    // Flips character depending on direction they are facing    
    private void Flip()
    {   
        if (isfacingRight && horizontal <0f || !isfacingRight && horizontal > 0f)
        {
            isfacingRight = !isfacingRight;
            Vector3 localScale = transform.localScale;
            localScale.x *= -1f;
            transform.localScale = localScale;
        }
    }

    // Checks if character touches wall
    private bool isWalled()
    {
        return Physics2D.OverlapCircle(wallCheck.position, 0.2f, wallLayer);
    }
    // Wallslide Mechanic
    private void WallSlide()
    {//change from isWalled
        if (IsWallSliding() && !IsGrounded() && horizontal != 0f)
        {
            isWallSliding = true;
            rb.velocity = new Vector2(rb.velocity.x, Mathf.Clamp(rb.velocity.y, -wallSlidingSpeed, float.MaxValue));
            animator.SetTrigger("WallSlide");
        }
        else
        {
            isWallSliding = false;
        }
    }

    // Walljumping Mechanic
    private void WallJump()
    {//added anim here
        if (isWallSliding)
        {
            isWallJumping = false;
            wallJumpingDirection = -transform.localScale.x;
            wallJumpingCounter = wallJumpingTime;
            CancelInvoke(nameof(StopWallJumping));
        }
        else
        {
            wallJumpingCounter -= Time.deltaTime;
        }

        if (Input.GetButtonDown("Jump") && wallJumpingCounter > 0f)
        {
            isWallJumping = true;
            rb.velocity = new Vector2(wallJumpingDirection * wallJumpingPower.x, wallJumpingPower.y);
            wallJumpingCounter = 0f;
            if (transform.localScale.x != wallJumpingDirection)
            {
                isfacingRight = !isfacingRight;
                Vector3 localScale = transform.localScale;
                localScale.x *= -1f;
                transform.localScale = localScale;
            }

            Invoke(nameof(StopWallJumping), wallJumpingDuration);
        }
    }

    //Check if character is not walljumping
    private void StopWallJumping()
    {
        isWallJumping = false;
    }

    // Dashing Mechanic
    private IEnumerator Dash()
    {
        canDash = false;
        isDashing = true;
        float orginalGravity = rb.gravityScale;
        rb.gravityScale = 0f;
        rb.velocity = new Vector2(transform.localScale.x * dashingPower, 0f);
        tr.emitting = true;
        animator.SetTrigger("Dash");
        yield return new WaitForSeconds(dashingTime);
        tr.emitting = false;
        rb.gravityScale = orginalGravity;
        isDashing = false;
        yield return new WaitForSeconds(dashingCooldown);
        canDash = true;
    }

    private bool IsWallSliding()
    {
        return Physics2D.OverlapCircle(wallCheck.position, 0.2f, wallLayer);
    }

    
}
 