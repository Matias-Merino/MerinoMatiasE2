using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading;
using UnityEngine;

public class Enemy : MonoBehaviour
{
    public float walkSpeed = 3f;

    [HideInInspector]
    public bool mustPatrol;
    private bool mustFlip;

    public Rigidbody2D rb;
    public Transform groundCheck;
    public LayerMask wallLayer;
    public LayerMask enemyLayer;
    public LayerMask groundLayer;
    public Collider2D bodyCollider;
    bool canMove;

    public delegate void FNotify();
    public static event FNotify OnEnemyDeath;

    private void OnDisable()
    {
        GameManager.OnWait -= GameManager_OnWait;
        GameManager.OnGameplay -= GameManager_OnGameplay;
        GameManager.OnRoundOver -= GameManager_OnRoundOver;
        GameManager.OnGameOver -= GameManager_OnGameOver;
    }

    private void OnEnable()
    {
        GameManager.OnWait += GameManager_OnWait;
        GameManager.OnGameplay += GameManager_OnGameplay;
        GameManager.OnRoundOver += GameManager_OnRoundOver;
        GameManager.OnGameOver += GameManager_OnGameOver;
    }

    public void GameManager_OnWait()
    {
        canMove = false;
    }
    public void GameManager_OnGameplay()
    {
        canMove = true;
    }

    public void GameManager_OnRoundOver()
    {
        canMove = false;
    }
    public void GameManager_OnGameOver()
    {
        canMove = false;
    }
    public void Awake()
    {
        GameManager.enemyAmount++;
    }

    private void Start()
    {
        mustPatrol = true;
    }

    private void Update()
    {
        if (mustPatrol && canMove == true)
        {
            Patrol();
        }
    }

    private void FixedUpdate()
    {
        if (mustPatrol)
        {
            mustFlip = !Physics2D.OverlapCircle(groundCheck.position, 0.1f, groundLayer);
        }
    }

    private void Patrol()
    {
        if (mustFlip || bodyCollider.IsTouchingLayers(enemyLayer) || bodyCollider.IsTouchingLayers(wallLayer))
        {
            Flip();
        }
        rb.velocity = new Vector2(walkSpeed * Time.fixedDeltaTime, rb.velocity.y);
    }

    void Flip()
    {
        mustPatrol = false;
        transform.localScale = new Vector2(transform.localScale.x * -1, transform.localScale.y);
        walkSpeed *= -1;
        mustPatrol = true;
    }


    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.GetComponent<EnemyKiller>())
        {
            OnEnemyDeath.Invoke();
            Destroy(gameObject);
        }
    }
}
