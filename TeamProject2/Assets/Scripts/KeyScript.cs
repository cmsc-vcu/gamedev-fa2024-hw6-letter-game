using UnityEngine;

public class KeyScript : MonoBehaviour
{
    public GameObject GotKeyText;
    public GameObject KeyUI;

    private bool playerNearby = false;
    private bool alreadyGotKey = false;

    private void Update()
    {
        if (playerNearby && Input.GetKeyUp(KeyCode.Space))
        {
            PlayerScript.hasKey = true;
        }
    }

    private void FixedUpdate()
    {
        KeyUI.SetActive(PlayerScript.hasKey);
        GotKeyText.SetActive(playerNearby && PlayerScript.hasKey && !alreadyGotKey);
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.CompareTag("Player"))
        {
            playerNearby = true;
        }
    }
    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.gameObject.CompareTag("Player"))
        {
            playerNearby = false;
            alreadyGotKey = PlayerScript.hasKey;
        }
    }
}
