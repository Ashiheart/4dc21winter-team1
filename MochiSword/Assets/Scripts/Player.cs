﻿using Library;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player : MonoBehaviour
{
    private enum Direction
    {
        LEFT,
        RIGHT
    }

    private enum AttackState
    {
        IDLE,
        FURI,
        TUKI
    }

    [SerializeField] private GameManager manager;
    [SerializeField] private Animator anim;
    [SerializeField] private GroundCheck groundCheck;

    [SerializeField] private GameObject attack_tuki;
    [SerializeField] private GameObject attack_furi;
    [SerializeField] private AudioSource jump_sound;

    [SerializeField] private float move_speed = 100f;
    [SerializeField] private float jump_force = 100f;
    [SerializeField] private Direction start_direction = Direction.RIGHT;

    [SerializeField] private int health = 3;

    private Rigidbody2D rb2d;
    private Vector2 input_movement;
    private bool jump_trigger = false;
    private AttackState attack_state = AttackState.IDLE;

    private Direction before_dir;

    // Start is called before the first frame update
    void Start()
    {
        rb2d = GetComponent<Rigidbody2D>();

        before_dir = Direction.RIGHT;

        HideAttack();
    }

    void Update()
    {
        input_movement = GetHorizontalMovement() * move_speed;
        if (IsGrounded())
        {
            if (Input.GetKeyDown(KeyCode.Space))
            {
                Jump();
            }
        }

        UpdateAttack();
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        rb2d.velocity = new Vector2(0, rb2d.velocity.y);
        rb2d.AddForce(input_movement * Time.deltaTime);
    }

    void UpdateAttack()
    {
        if (attack_state == AttackState.IDLE)
        {
            if (Input.GetKeyDown(KeyCode.Z))
            {
                AttackFURI();
            }
            if (Input.GetKeyDown(KeyCode.X))
            {
                AttackTUKI();
            }
        }
    }

    void AttackFURI()
    {
        attack_state = AttackState.FURI;
        attack_furi.SetActive(true);
        Invoke("HideAttack", 0.1f);

        NotifyAttackState(AttackState.FURI);
    }

    void AttackTUKI()
    {
        attack_state = AttackState.TUKI;
        attack_tuki.SetActive(true);
        Invoke("HideAttack", 0.1f);

        NotifyAttackState(AttackState.TUKI);
    }

    void HideAttack()
    {
        attack_state = AttackState.IDLE;
        attack_furi.SetActive(false);
        attack_tuki.SetActive(false);
    }

    void NotifyAttackState(AttackState state)
    {
        /*
        === Add notify system here. ===
        */

        Debugger.Log("Notify AttackState [" + state + "]");
    }

    Vector2 GetHorizontalMovement()
    {
        Vector2 movement = Vector2.zero;

        movement.x = Input.GetAxis("Horizontal");
        
        if(movement.x > 0)
        {
            UpdateFlip(Direction.RIGHT);
        }
        if(movement.x < 0)
        {
            UpdateFlip(Direction.LEFT);
        }


        return movement;
    }

    void UpdateFlip(Direction dir)
    {
        if(dir != before_dir)
        {
            before_dir = dir;
            transform.localScale = new Vector3(transform.localScale.x * -1, transform.localScale.y, transform.localScale.z);
        }
    }

    bool IsGrounded()
    {
        return groundCheck.isGrounded();
    }

    void Jump()
    {
        jump_sound.Play();
        rb2d.velocity = new Vector2(rb2d.velocity.x, 0);
        rb2d.AddForce(Vector2.up * jump_force);
    }

    void OnTriggerEnter2D(Collider2D col)
    {
        Debug.Log("Hit!!!");
        if (col.tag == "DeathZone")
        {
            Damage(9999);
        }

        if (col.tag == "Enemy" || col.tag == "E_Attack")
        {
            Damage(1);
        }
    }

    public void Damage(int value)
    {
        health -= value;
        if(health <= 0)
        {
            KillPlayer();
        }
    }

    void KillPlayer()
    {
        Debug.Log("You Dead!");
        KillPlayerAnim();
        manager.GameOver();
    }

    void KillPlayerAnim()
    {
        anim.SetTrigger("Die");
    }
}
