using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class PlayerMovement : MonoBehaviour
{
    private Rigidbody2D rb;
    private Animator anim;
    private SpriteRenderer sprite;
    private BoxCollider2D coll;

    [SerializeField] private LayerMask jumpableGround;

    private float dirX = 0f;
    [SerializeField] private float moveSpeed = 7f;
    [SerializeField] private float jumpForce = 14f;

    private enum State { idle, running, jumping, falling }

    [SerializeField] private AudioSource jumpSoundEffect;

    private void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        anim = GetComponent<Animator>();
        sprite = GetComponent<SpriteRenderer>();
        coll = GetComponent<BoxCollider2D>();
    }

    private void Update()
    {
        dirX = Input.GetAxisRaw("Horizontal");
        rb.velocity = new Vector2(dirX * moveSpeed, rb.velocity.y);

        if (Input.GetButtonDown("Jump") && IsGrounded())
        {
            rb.velocity = new Vector2(rb.velocity.x, jumpForce);
            jumpSoundEffect.Play();
        }


        UpdateAnimation();
    }

    // Controls player sprite animation
    private void UpdateAnimation()
    {
        State state;
        
        // Controls player facing direction and running/idle animations
        if (dirX != 0)
        {
            state = State.running;
            sprite.flipX = dirX < 0;
        }
        else
        {
            state = State.idle;
        }

        // Controls jumping/falling animation
        if (Math.Abs(rb.velocity.y) > .1f)
        {
            state = rb.velocity.y > .1f ? State.jumping : State.falling;
        }
        anim.SetInteger("state", (int)state);
    }

    // Checks if the player is on the ground
    private bool IsGrounded()
    {
        return Physics2D.BoxCast(coll.bounds.center, coll.bounds.size, 0f, Vector2.down, .1f, jumpableGround);
    }
}
