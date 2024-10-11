using System.Collections;
using System.Collections.Generic;
using System.Linq;
using TreeEditor;
using Unity.VisualScripting;
using UnityEngine;

public class EnemyScript : MonoBehaviour
{
    public float enemySpeed = 1f;
    public float searchCooldown = 10f;
    public GameObject startingRoom;
    public GameObject room;
    public GameObject positionCircle;

    public static GameObject activeRoom;
    public static bool reachedMiddle = false;

    private Rigidbody2D rb;
    private Collider2D col;
    private Vector2 movement;
    private float timeLastSearched = 100f;
    private float epsilon = 0.5f;
    private List<GameObject> path;

    // Start is called before the first frame update
    void Start()
    {
        // Initialize Rigidbody
        rb = GetComponent<Rigidbody2D>();
        rb.constraints = RigidbodyConstraints2D.FreezeRotation;

        col = GetComponent<Collider2D>();

        activeRoom = startingRoom;

        timeLastSearched = 100f;

        path = new List<GameObject>();
    }

    // Update is called once per frame
    void Update()
    {
        // Check if in the same room
        // If you are in the same room run toward player
        movement = new Vector2(0, 0);

        if (activeRoom == PlayerScript.activeRoom)
        {
            Debug.Log("In the same room");
            float angleDiff = Mathf.Atan2(PlayerScript.playerPosition.y - transform.position.y,
                PlayerScript.playerPosition.x - transform.position.x);

            movement = new Vector2(Mathf.Cos(angleDiff), Mathf.Sin(angleDiff));
            return;
        }
        else if(true) // If not go to room they are in
        {
            Debug.Log("Searching");
            timeLastSearched = Time.time;
            path = GetToRoom(activeRoom, PlayerScript.activeRoom);
            
            if (activeRoom == path[0])
            {
                path.RemoveAt(0);
            }

            float angleDiff = 0f;

            if(reachedMiddle)
            {
                angleDiff = Mathf.Atan2(path[0].transform.position.y - transform.position.y,
                                        path[0].transform.position.x - transform.position.x);
            } else
            {
                Vector2 diffVec = path[0].transform.position - transform.position;
                diffVec.x = Mathf.Abs(diffVec.x);
                diffVec.y = Mathf.Abs(diffVec.y);

                if(diffVec.x > diffVec.y)
                {
                    float yDiff = Mathf.Abs(path[0].transform.position.y - transform.position.y);
                    if (yDiff <= epsilon)
                    {
                        angleDiff = Mathf.Atan2(path[0].transform.position.y - transform.position.y,
                                        path[0].transform.position.x - transform.position.x);
                    } else
                    {
                        angleDiff = Mathf.Atan2(path[0].transform.position.y - transform.position.y, 0);
                    }
                } else
                {
                    float xDiff = Mathf.Abs(path[0].transform.position.x - transform.position.x);
                    if (xDiff <= epsilon)
                    {
                        angleDiff = Mathf.Atan2(path[0].transform.position.y - transform.position.y,
                                        path[0].transform.position.x - transform.position.x);
                    }
                    else
                    {
                        angleDiff = Mathf.Atan2(0, path[0].transform.position.x - transform.position.x);
                    }
                }
            }

            Debug.Log("End Room transform: " + path[0].transform.position);

            movement = new Vector2(Mathf.Cos(angleDiff), Mathf.Sin(angleDiff));
        } else
        {
            if (path.Count <= 0)
            {
                Debug.Log("Path Empty");
                Debug.Log(activeRoom.name + " -> " + PlayerScript.activeRoom.name);
                return;
            }
            if (activeRoom == path[0])
            {
                path.RemoveAt(0);
            }
            if (path.Count <= 0)
            {
                Debug.Log("Error");
                return;
            }

            float angleDiff = Mathf.Atan2(path[0].transform.position.y - transform.position.y,
            path[0].transform.position.x - transform.position.x);

            movement = new Vector2(Mathf.Cos(angleDiff), Mathf.Sin(angleDiff)); 
        }
    }

    private void FixedUpdate()
    {
        room = activeRoom;
        rb.velocity = movement * enemySpeed * Time.fixedDeltaTime;

        positionCircle.transform.position = rb.position;
    }

    private List<GameObject> GetToRoom(GameObject startRoom, GameObject endRoom)
    {
        List<GameObject> list = new List<GameObject>();
        return GetToRoomRecursive(startRoom, endRoom, list);
    }

    private List<GameObject> GetToRoomRecursive(GameObject startRoom, GameObject endRoom, List<GameObject> path)
    {
        if (startRoom == null || endRoom == null) return null;

        path.Add(startRoom);

        RoomScript startRoomScript = startRoom.GetComponent<RoomScript>();
        Debug.Log("Start Room transform: " + startRoom.transform.position);
        if (startRoomScript.TopRoom == endRoom)
        {
            path.Add(startRoomScript.TopRoom);
            return path;
        }
        else if (startRoomScript.BottomRoom == endRoom)
        {
            path.Add(startRoomScript.BottomRoom);
            return path;
        }
        else if (startRoomScript.LeftRoom == endRoom)
        {
            path.Add(startRoomScript.LeftRoom);
            return path;
        }
        else if (startRoomScript.RightRoom == endRoom)
        {
            path.Add(startRoomScript.RightRoom);
            return path;
        }
        else
        {
            List<GameObject> list = new List<GameObject>();
            if (!path.Contains(startRoomScript.TopRoom))
            {
                if ((list = GetToRoomRecursive(startRoomScript.TopRoom, endRoom, path)) != null)
                {
                    return list;
                }
            }
            if (!path.Contains(startRoomScript.BottomRoom))
            {
                if ((list = GetToRoomRecursive(startRoomScript.BottomRoom, endRoom, path)) != null)
                {
                    return list;
                }
            }
            if (!path.Contains(startRoomScript.LeftRoom))
            {
                if ((list = GetToRoomRecursive(startRoomScript.LeftRoom, endRoom, path)) != null)
                {
                    return list;
                }
            }
            if (!path.Contains(startRoomScript.RightRoom))
            {
                if ((list = GetToRoomRecursive(startRoomScript.RightRoom, endRoom, path)) != null)
                {
                    return list;
                }
            }
        }

        path.RemoveAt(path.Count - 1);

        return new List<GameObject>();
    }
}
