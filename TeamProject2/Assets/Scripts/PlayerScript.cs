using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class PlayerScript : MonoBehaviour
{
    [SerializeField] public float playerSpeed = 250;
    [SerializeField] public GameObject startingRoom;
    [SerializeField] public GameObject positionCircle;
    [SerializeField] public AudioClip walkNoise;
    [SerializeField] public float walkNoiseTime = 0.5f;
    [SerializeField] public float walkNoiseBlend = 0.95f;

    public static GameObject activeRoom;
    public static Vector2 playerPosition;

    private Rigidbody2D rb;
    private AudioSource audioSource;
    private Collider2D col;
    private Animator animator;
    private Vector2 lastMoveDirection;
    private Vector2 movement;
    private bool walking = false;
    private bool walkingLastFrame = false;

    // Start is called before the first frame update
    void Start()
    {
        // Initialize Rigidbody
        rb = GetComponent<Rigidbody2D>();
        rb.constraints = RigidbodyConstraints2D.FreezeRotation;

        audioSource = GetComponent<AudioSource>();
        audioSource.clip = walkNoise;
        audioSource.loop = true;

        animator = GetComponent<Animator>();

        col = GetComponent<Collider2D>();

        activeRoom = startingRoom;

        playerPosition = rb.position;
    }

    // Update is called once per frame
    void Update()
    {
        float moveX = Input.GetAxisRaw("Horizontal");
        float moveY = Input.GetAxisRaw("Vertical");

        walking = (moveX != 0 || moveY != 0);

        movement = new Vector2(moveX, moveY);
        
        movement.Normalize();
        Animate();

        if (walking)
        {
            lastMoveDirection = movement;
        }
    }

    private void FixedUpdate()
    {
        rb.velocity = movement * playerSpeed * Time.fixedDeltaTime;

        playerPosition = rb.position;

        positionCircle.transform.position = rb.position;

        if (walking && !walkingLastFrame)
        {
            audioSource.Play();
        } 
        else if(!walking) 
        {
        
            audioSource.Stop();
        }

        walkingLastFrame = walking;
    }

    private void Animate()
    {
        animator.SetFloat("MoveX", movement.x);
        animator.SetFloat("MoveY", movement.y);
        animator.SetFloat("MoveMagnitude", movement.magnitude);

        animator.SetFloat("LastMoveX", lastMoveDirection.x);
        animator.SetFloat("LastMoveY", lastMoveDirection.y);
        Debug.Log(lastMoveDirection);
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if(collision.gameObject.CompareTag("Enemy"))
        {
            SceneManager.LoadScene("GameOver");
        }
    }
}
