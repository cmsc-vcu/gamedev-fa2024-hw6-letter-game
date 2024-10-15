using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ItemAreaScript : MonoBehaviour
{
    public GameObject ItemPicture;

    private bool playerNearItem = false;
    private bool itemShown = false;

    public void Update()
    {
        if(playerNearItem && Input.GetKeyUp(KeyCode.Space))
        {
            itemShown = !itemShown;
        }
    }

    public void FixedUpdate()
    {
        if(itemShown)
        {
            Debug.Log("Showing Item");
            ItemPicture.SetActive(true);
        }
        else
        {
            ItemPicture.SetActive(false);
        }
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.CompareTag("Player"))
        {
            Debug.Log("Player In item range");
            playerNearItem = true;
        }
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        if(collision.gameObject.CompareTag("Player"))
        {
            playerNearItem = false;
            itemShown = false;
        }
    }
}
