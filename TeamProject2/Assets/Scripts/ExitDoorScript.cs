using UnityEngine;
using UnityEngine.SceneManagement;

public class ExitDoorScript : MonoBehaviour
{
    public GameObject InteractText;
    public GameObject NeedKeyText;

    private bool failedToInteract = false;
    private bool playerNearDoor = false;

    // Update is called once per frame
    void Update()
    {
        if (playerNearDoor && Input.GetKeyUp(KeyCode.Space))
        {
            if(PlayerScript.hasKey)
            {
                // Win
                SceneManager.LoadScene("WinScreen");
                return;
            }

            failedToInteract = true;
        }
    }

    public void FixedUpdate()
    {
        if (playerNearDoor)
        {
            InteractText.SetActive(!failedToInteract);
            NeedKeyText.SetActive(failedToInteract);
        } else
        {
            InteractText.SetActive(false);
            NeedKeyText.SetActive(false);
        }
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.CompareTag("Player"))
        {
            playerNearDoor = true;
            failedToInteract = false;
        }
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.gameObject.CompareTag("Player"))
        {
            playerNearDoor = false;
        }
    }
}
