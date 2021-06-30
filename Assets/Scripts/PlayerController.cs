using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using UnityEngine;


public class PlayerController : MonoBehaviour
{
    Rigidbody2D rb;
    Animator anim;
    SpriteRenderer sprite;
    Collider2D col;
    public float moveSpeed;
    public float jumpForce = 10f;
    bool insideDoor;
    bool insideTrigger;
    public GameObject[] puertas;
    public int idPuerta;
    float eKTimer = 0f;
    public bool eKEnable = false;
    bool canMove;

    public delegate void FNotify();
    public static event FNotify OnPlayerDeath;

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
        anim.speed = 0;
    }
    public void GameManager_OnGameplay()
    {
        canMove = true;
        anim.speed = 1;
    }

    public void GameManager_OnRoundOver()
    {
        canMove = false;
    }
    public void GameManager_OnGameOver()
    {
        canMove = false;
        anim.speed = 0;
    }

    List<Color> EcolorList = new List<Color>()
    {
        Color.red,
        Color.blue,
        Color.green
    };
   
    private void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        anim = GetComponent<Animator>();
        sprite = GetComponent<SpriteRenderer>();
        col = GetComponent<Collider2D>();
    }
    // Start is called before the first frame update
    void Start()
    {
        insideDoor = false;
        insideTrigger = false;
    }

    void MovePlayer()
    {
        float x = Input.GetAxis("Horizontal");
        Vector2 targetVelocity = Vector2.right * x * moveSpeed;
        targetVelocity.y = rb.velocity.y;

        rb.velocity = targetVelocity;
        sprite.flipX = x < 0;
        anim.SetFloat("Speed", Mathf.Abs(rb.velocity.x));
    }

    void Jump()
    {
        if (Mathf.Abs(rb.velocity.y) > 0f) return;

        rb.AddForce(jumpForce * Vector2.up, ForceMode2D.Impulse);

    }
    
    void Update()
    {
        if (canMove==true && insideDoor == false)
        {
            MovePlayer();
            if (Input.GetButtonDown("Jump"))
                Jump();
        }

        if ((Input.GetKeyDown(KeyCode.S) || Input.GetKeyDown(KeyCode.DownArrow)) && insideDoor == true && eKEnable == false)
        {
            int numintem = 0;
            //player sale de la puerta
            insideDoor = false;
            col.isTrigger = false;
            rb.constraints = RigidbodyConstraints2D.None;
            sprite.enabled = !sprite.enabled;
            rb.constraints = RigidbodyConstraints2D.FreezeRotation;

            //cambio color puerta del player
            Color coloaractual = puertas[idPuerta].gameObject.GetComponent<Door>().GetComponent<SpriteRenderer>().color;
            foreach (var item in EcolorList)
            {
                if (coloaractual == item)
                {
                    if (numintem == 2) { numintem = 0; } else { numintem++; }
                    puertas[idPuerta].gameObject.GetComponent<Door>().GetComponent<SpriteRenderer>().color = EcolorList[numintem];
                }
                numintem++;
            }

            //se abren todas las puertas del mismo color
            puertas[idPuerta].transform.GetChild(1).gameObject.SetActive(true);
            int i = 0;
            foreach (var item in puertas)
            {
                if (coloaractual == item.gameObject.GetComponent<Door>().GetComponent<SpriteRenderer>().color)
                {
                    puertas[i].transform.GetChild(1).gameObject.SetActive(true);
                    eKEnable = true;
                }
                i++;
            }
        }

        if (eKEnable == true)
        {
            eKTimer += Time.deltaTime;
            if (eKTimer >= 0.15f)
            {
                for (int p = 0; p < puertas.Length; p++)
                {
                    puertas[p].transform.GetChild(1).gameObject.SetActive(false);
                }
                eKTimer = 0;
                eKEnable = false;
            }
        }

        if ((Input.GetKeyDown(KeyCode.W) || Input.GetKeyDown(KeyCode.UpArrow)) && insideTrigger == true && insideDoor == false)
        {
            insideDoor = true;
            col.isTrigger = true;
            rb.constraints = RigidbodyConstraints2D.FreezeAll;
            sprite.enabled = !sprite.enabled;
        }
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.GetComponent<Enemy>())
        {
            GameManager.Instance.gameState = GameState.GameOver;
            OnPlayerDeath.Invoke();
            Debug.Log("Game Over");
        }
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {

        if (collision.gameObject.GetComponent<Door>())
        {
            insideTrigger = true;
            idPuerta = Convert.ToInt32(collision.name);
        }
    }
    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.gameObject.GetComponent<Door>())
        {
            insideTrigger = false;
        }
    }
}
