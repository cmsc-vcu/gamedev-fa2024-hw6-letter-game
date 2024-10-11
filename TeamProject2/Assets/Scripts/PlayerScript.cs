using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerScript : MonoBehaviour
{
    public float playerSpeed = 1f;
    public GameObject startingRoom;
    public GameObject room;
    public GameObject positionCircle;

    public static GameObject activeRoom;
    public static Vector2 playerPosition;

    private Rigidbody2D rb;
    private Collider2D col;
    private Vector2 movement;

    // Start is called before the first frame update
    void Start()
    {
        // Initialize Rigidbody
        rb = GetComponent<Rigidbody2D>();
        rb.constraints = RigidbodyConstraints2D.FreezeRotation;

        col = GetComponent<Collider2D>();

        activeRoom = startingRoom;

        playerPosition = rb.position;
    }

    // Update is called once per frame
    void Update()
    {
        movement = new Vector2(Input.GetAxisRaw("Horizontal"), Input.GetAxisRaw("Vertical"));
    }

    private void FixedUpdate()
    {
        room = activeRoom;
        rb.velocity = movement * playerSpeed * Time.fixedDeltaTime;

        playerPosition = rb.position;

        positionCircle.transform.position = rb.position;
    }
}
