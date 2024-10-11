using System.Collections;
using System.Collections.Generic;
using System.Numerics;
using UnityEngine;

public class RoomScript : MonoBehaviour
{
    public GameObject LeftRoom;
    public GameObject RightRoom;
    public GameObject TopRoom;
    public GameObject BottomRoom;

    // Start is called before the first frame update
    void Start()
    {

        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if(collision.gameObject.CompareTag("PlayerCircle"))
        {
            Debug.Log("Player Entered new Room");
            PlayerScript.activeRoom = gameObject;
        } 
        else if (collision.gameObject.CompareTag("EnemyCircle"))
        {
            Debug.Log("Enemy Entered new Room");
            EnemyScript.activeRoom = gameObject;
        }
    }
}
