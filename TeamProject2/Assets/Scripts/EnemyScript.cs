using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using TreeEditor;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Audio;

public class EnemyScript : MonoBehaviour
{
    [SerializeField] public float enemySpeed = 75;
    [SerializeField] public float searchCooldown = 3;
    [SerializeField] public GameObject startingRoom;
    [SerializeField] public GameObject positionCircle;
    [SerializeField] public AudioClip walkNoise;
    [SerializeField] public float walkNoiseTime = 0.75f;
    [SerializeField] public float runNoiseTime = 0.325f;
    [SerializeField] public float walkNoiseBlend = 0.95f;
    [SerializeField] public float runNoiseBlend = 0.5f;

    public static GameObject activeRoom;
    public static bool reachedMiddle = false;

    private Rigidbody2D rb;
    private Collider2D col;
    private AudioSource audioSource;
    private Animator animator;
    private Vector2 movement;
    private float timeLastSearched = 0f;
    private float timeLastWalkNoise = 0f;
    private float epsilon = 0.5f;
    private List<GameObject> path;

    // Start is called before the first frame update
    void Start()
    {
        // Initialize Rigidbody
        rb = GetComponent<Rigidbody2D>();
        rb.constraints = RigidbodyConstraints2D.FreezeRotation;

        col = GetComponent<Collider2D>();

        audioSource = GetComponent<AudioSource>();
        audioSource.clip = walkNoise;

        animator = GetComponent<Animator>();

        activeRoom = startingRoom;

        path = new List<GameObject>();
    }

    // Update is called once per frame
    void Update()
    {
        // Check if in the same room
        // If you are in the same room run toward player
        movement = new Vector2(0, 0);

        if (path.Count != 0 && activeRoom == path[0])
        {
            path.RemoveAt(0);
        }

        if (activeRoom == PlayerScript.activeRoom)
        {
            float angleDiff = Mathf.Atan2(PlayerScript.playerPosition.y - transform.position.y,
                PlayerScript.playerPosition.x - transform.position.x);

            movement = new Vector2(Mathf.Cos(angleDiff), Mathf.Sin(angleDiff));
            audioSource.spatialBlend = runNoiseBlend;
        }
        else // If not go to room they are in
        {
            // Don't want to search every frame
            if (Time.time > timeLastSearched + searchCooldown || path.Count == 0)
            {
                timeLastSearched = Time.time;
                path = GetToRoom(activeRoom, PlayerScript.activeRoom);
            }

            movement = getMovementVector();
            audioSource.clip = walkNoise;
            audioSource.spatialBlend = walkNoiseBlend;
        }

        Animate();
    }

    private void FixedUpdate()
    {
        float speedBoost = 1.0f;
        float audioTime = walkNoiseTime;

        if (activeRoom == PlayerScript.activeRoom)
        {
            speedBoost = 2.0f;
            audioTime = runNoiseTime;
            animator.speed = 1.5f;
        } else
        {
            animator.speed = 1.0f;
        }

        rb.velocity = movement * enemySpeed * speedBoost * Time.fixedDeltaTime;

        positionCircle.transform.position = rb.position;


        if(Time.time > timeLastWalkNoise + audioTime)
        {
            timeLastWalkNoise = Time.time;
            audioSource.Play();
        }

    }

    #region private functions

    private void Animate()
    {
        animator.SetFloat("MoveX", movement.x);
        animator.SetFloat("MoveY", movement.y);
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
        //Debug.Log("Start Room transform: " + startRoom.transform.position);
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


    private Vector2 getMovementVector()
    {
        float angleDiff = 0f;

        if (reachedMiddle)
        {
            angleDiff = Mathf.Atan2(path[0].transform.position.y - transform.position.y,
                                    path[0].transform.position.x - transform.position.x);
        }
        else
        {
            Vector2 diffVec = path[0].transform.position - transform.position;
            diffVec.x = Mathf.Abs(diffVec.x);
            diffVec.y = Mathf.Abs(diffVec.y);

            if (diffVec.x > diffVec.y)
            {
                float yDiff = Mathf.Abs(path[0].transform.position.y - transform.position.y);
                if (yDiff <= epsilon)
                {
                    angleDiff = Mathf.Atan2(path[0].transform.position.y - transform.position.y,
                                    path[0].transform.position.x - transform.position.x);
                }
                else
                {
                    angleDiff = Mathf.Atan2(path[0].transform.position.y - transform.position.y, 0);
                }
            }
            else
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

        return new Vector2(Mathf.Cos(angleDiff), Mathf.Sin(angleDiff));
    }

}

#endregion private functions
