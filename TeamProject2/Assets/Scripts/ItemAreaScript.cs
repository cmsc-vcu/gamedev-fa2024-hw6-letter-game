using UnityEngine;

public class ItemAreaScript : MonoBehaviour
{
    public GameObject ItemPicture;
    public GameObject InteractText;
    public GameObject NoteText;

    public static bool canUpdate = true;

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
            ItemPicture.SetActive(true);
            InteractText.SetActive(false);
            NoteText.SetActive(true);
        }
        else
        {
            if (playerNearItem)
            {
                InteractText.SetActive(true);
            }
            else if (canUpdate) 
            { 
                InteractText.SetActive(false); 
            }
            ItemPicture.SetActive(false);
            NoteText.SetActive(false);
        }
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.CompareTag("Player"))
        {
            playerNearItem = true;
            canUpdate = false;
        }
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        if(collision.gameObject.CompareTag("Player"))
        {
            playerNearItem = false;
            itemShown = false;
            canUpdate = true;
        }
    }
}
