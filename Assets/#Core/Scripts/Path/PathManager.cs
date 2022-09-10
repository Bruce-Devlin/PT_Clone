using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class PathManager : MonoBehaviour
{
    [Header("Options")]
    public bool debug = false;
    public bool preventProgress = false;

    [Header("Player Triggers")]

    public PathTrigger onTriggerEnterPath1;
    public PathTrigger onTriggerEnterPath2;

    public PathTrigger onTriggerLeavePath1;
    public PathTrigger onTriggerLeavePath2;

    [Space(10)]
    [Header("Game Objects")]

    //Path GameObjects.
    public GameObject path1;
    public GameObject path1Door;
    public GameObject path2Door;

    public GameObject path2;
    public GameObject basement;
    public PlayerController playerController;

    [HideInInspector]
    public int playerLoops = 0;
    [HideInInspector]
    public int currentPath = 0;

    //Basement used.
    private bool basementUsed = false;

    public void Log(string txt = "")
    {
        if (playerController.debugCam || playerController.printDebugToHint) StartCoroutine(playerController.ShowHint(txt, false));
        else Debug.Log(txt);
    }

    void OnEnable()
    {
        onTriggerEnterPath1.TriggerEvent.AddListener(OnPathTriggerEnter1);
        onTriggerEnterPath2.TriggerEvent.AddListener(OnPathTriggerEnter2);

        onTriggerLeavePath1.TriggerEvent.AddListener(OnPathTriggerLeave1);
        onTriggerLeavePath2.TriggerEvent.AddListener(OnPathTriggerLeave2);
    }

    void OnDisable()
    {
        onTriggerEnterPath1.TriggerEvent.RemoveListener(OnPathTriggerEnter1);
        onTriggerEnterPath2.TriggerEvent.RemoveListener(OnPathTriggerEnter2);

        onTriggerLeavePath1.TriggerEvent.RemoveListener(OnPathTriggerLeave1);
        onTriggerLeavePath2.TriggerEvent.RemoveListener(OnPathTriggerLeave2);
    }

    // Start is called before the first frame update
    void Start()
    {
        path1Door.transform.Find("Door").gameObject.SetActive(false);
        path2.SetActive(false);
        basement.SetActive(true);
    }

    void OnPathTriggerEnter1(Collider collider)
    {
        if (collider != null && collider.gameObject.tag == "Player")
        {
            if (debug) Log("PathManager: Player Entered Path 1");
            currentPath = 1;
            path2Door.transform.Find("Door").gameObject.SetActive(true);
            path1Door.transform.Find("Door").gameObject.SetActive(true);
            path2.SetActive(false);
            if (!basementUsed)
            {
                basementUsed = true;

                basement.SetActive(false);
                StartCoroutine(playerController.ShowHint("Did that door just close behind me?"));
            }
            else
            {
                playerLoops++;
                if (debug) Log("PathManager: Player Entered Path 2 (Loops: " + playerLoops + ")");
                path2.SetActive(true);
            }    
        }
    }

    void OnPathTriggerEnter2(Collider collider)
    {
        if (collider != null && collider.gameObject.tag == "Player")
        {
            playerLoops++;
            if (debug) Log("PathManager: Player Entered Path 2 (Loops: " + playerLoops + ")");
            currentPath = 2;
            path1Door.transform.Find("Door").gameObject.SetActive(true);
            path2Door.transform.Find("Door").gameObject.SetActive(true);
            path1.SetActive(false);
        }
    }
  

    void OnPathTriggerLeave1(Collider collider)
    {
        if (collider != null && collider.gameObject.tag == "Player" && !preventProgress)
        {
            if (debug) Log("PathManager: Player Left Path 1");
            path2.SetActive(true);
            path2Door.transform.Find("Door").gameObject.SetActive(false);
        }
    }
    void OnPathTriggerLeave2(Collider collider)
    {
        if (collider != null && collider.gameObject.tag == "Player" && !preventProgress)
        {
            if (debug) Log("PathManager: Player Left Path 2");
            path1.SetActive(true);
            path1Door.transform.Find("Door").gameObject.SetActive(false);
        }
    }
}
