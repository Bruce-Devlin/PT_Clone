using System.Diagnostics;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PathManager : MonoBehaviour
{
    #region Variables
    [Header("Options")]
    [HideInInspector]public bool debug;
    [HideInInspector]public string status = "";
    public bool preventProgress = false;
    public bool progressingPath = false;
    public int playerLoops = 0;
    public Stopwatch looptime = new Stopwatch();
    public Stopwatch runtime = new Stopwatch();

    public GameObject basement;
    public GameObject firstPath;

    public PlayerController playerController;

    [HideInInspector] public bool loaded = false;
    [HideInInspector] public GameObject currentPath;

    [HideInInspector] public GameObject EnterDoor;
    [HideInInspector] public GameObject ExitDoor;

    [HideInInspector] public SadisticAI sadisticAI;

    [HideInInspector] public GameObject Interactables;

    private bool starting;

    private bool basementUsed = false;
    #endregion

    #region Unity Methods
    private void OnEnable()
    {
        starting = true;
        status = "Loading";
    }

    // Start is called before the first frame update
    void Start()
    {
        debug = playerController.debug;
        Interactables = firstPath.transform.Find("AI_Interactables").gameObject;

        EnterDoor = Interactables.transform.Find("Enter_Door").gameObject;
        ExitDoor = Interactables.transform.Find("Exit_Door").gameObject;

        firstPath.transform.Find("trigger_Enter").GetComponent<PathTrigger>().TriggerEvent.AddListener(Trigger_OnPathEnter);
        firstPath.transform.Find("trigger_Exit").GetComponent<PathTrigger>().TriggerEvent.AddListener(Trigger_OnPathExit);

        basement.SetActive(true);
        sadisticAI = GetComponent<SadisticAI>();

        starting = false;
    }

    // Update is called once per frame
    void Update()
    {
        if (!loaded && !starting)
        {
            if (debug) playerController.Log("P_Man: Loaded.");
            status = "Active";
            loaded = true;
        }
    }
    #endregion

    #region Triggers
    bool oldPathActive = false;
    bool currentPathActive;
    void Trigger_OnPathEnter(Collider collider)
    {
        if (collider != null && collider.gameObject.tag == "Player")
        {
            sadisticAI.trigger_Enter_Active = true;

            if (!basementUsed)
            {
                basementUsed = true;
                EnterDoor.GetComponent<DoorController>().forceClose = true;
                StartCoroutine(DestroyPath(basement, 2));
                
                currentPath = firstPath;
                firstPath = null;
                currentPathActive = true;

                runtime.Start();
                looptime.Start();
            }
            else if (!currentPathActive)
            {
                playerLoops++;
                if (debug) playerController.Log("P_Man: Player entered new Path trigger (loops: " + playerLoops + ")");
                looptime.Restart();

                progressingPath = false;

                oldPathActive = true;

                EnterDoor = newEnterDoor;
                ExitDoor = newExitDoor;

                EnterDoor.GetComponent<DoorController>().forceClose = true;
            }
        }
        else if (collider == null)
        {
            sadisticAI.trigger_Enter_Active = false;

            if (debug) playerController.Log("P_Man: Player left new Path trigger");

            if (newPath == null)
            {
                SpawnNewPath();
            }
            
            if (oldPathActive)
            {
                currentPathActive = true;
                Destroy(currentPath);
                currentPath = newPath;

                newPath = null;
                newPath = null;
                newPath = null;


                Interactables = currentPath.transform.Find("AI_Interactables").gameObject;

                currentPath.transform.Find("trigger_FrontDoor").gameObject.GetComponent<PathTrigger>().TriggerEvent.RemoveAllListeners();
                currentPath.transform.Find("trigger_FrontDoor").gameObject.GetComponent<PathTrigger>().TriggerEvent.AddListener(sadisticAI.Trigger_FrontDoor_Event);
                currentPath.transform.Find("trigger_Table").gameObject.GetComponent<PathTrigger>().TriggerEvent.AddListener(sadisticAI.Trigger_Table_Event);
                currentPath.transform.Find("trigger_ExtraDoor").gameObject.GetComponent<PathTrigger>().TriggerEvent.AddListener(sadisticAI.Trigger_ExtraDoor_Event);
                ;
                sadisticAI.interact_CeilingLights.Clear();

                foreach (Transform light in currentPath.transform.Find("Lights"))
                {
                    sadisticAI.interact_CeilingLights.Add(light.gameObject);
                }

                sadisticAI.interact_FrontDoor = Interactables.transform.Find("Front_Door").gameObject;

                sadisticAI.interact_ExtraDoor_01 = Interactables.transform.Find("Extra_Door_01").gameObject;
                sadisticAI.interact_ExtraDoor_02 = Interactables.transform.Find("Extra_Door_02").gameObject;
                sadisticAI.interact_ExtraDoor_03 = Interactables.transform.Find("Extra_Door_03").gameObject;
                sadisticAI.interact_ExtraDoor_04 = Interactables.transform.Find("Extra_Door_04").gameObject;

                sadisticAI.interact_Phone = Interactables.transform.Find("Telephone").gameObject;
                oldPathActive = false;
            }
        }  
    }
    void Trigger_OnPathExit(Collider collider)
    {
        if (collider != null && collider.gameObject.tag == "Player" && basementUsed)
        {
            sadisticAI.trigger_Exit_Active = true;
            if (!preventProgress && !sadisticAI.thinking)
            {
                newPath.GetComponent<GamePath>().active = true;
                newEnterDoor.GetComponent<DoorController>().locked = false;
                currentPathActive = false;

                progressingPath = true;
            }
        }
        else if (collider == null)
        {
            sadisticAI.trigger_Exit_Active = false;
            
        }
    }
    #endregion

    #region Functions
    private GameObject newPath = null;

    private GameObject newEnterDoor = null;
    private GameObject newExitDoor = null;

    void SpawnNewPath()
    {
        List<GameObject> doors = new List<GameObject>()
        {
            ExitDoor,
            sadisticAI.interact_ExtraDoor_01,
            sadisticAI.interact_ExtraDoor_02,
            sadisticAI.interact_ExtraDoor_03
        };

        GameObject choosenDoor = doors[SadisticAI.RollDice("choosen door", 0, doors.Count)];

        Vector3 newPosition = choosenDoor.transform.position;

        newPosition.y = 0;

        UnityEngine.Debug.Log("Global: " + choosenDoor.transform.rotation.y + " - " + choosenDoor.transform.eulerAngles.y + " | Local: " + choosenDoor.transform.localRotation.y + " - " + choosenDoor.transform.localEulerAngles.y);

        Vector3 pathEuler = choosenDoor.transform.eulerAngles;

        float pathY = pathEuler.y;
        if (pathY.ToString().StartsWith("0")) pathEuler.y = 90;
        else pathEuler.y += 90;

        UnityEngine.Debug.Log(pathEuler.y);

        newPath = Instantiate(currentPath, newPosition, Quaternion.Euler(pathEuler), transform);

        newPath.GetComponent<GamePath>().pathManager = this;
        newPath.GetComponent<GamePath>().sadisticAI = sadisticAI;

        newPath.GetComponent<GamePath>().active = false;

        newEnterDoor = newPath.transform.Find("AI_Interactables").transform.Find("Enter_Door").gameObject;
        newExitDoor = newPath.transform.Find("AI_Interactables").transform.Find("Exit_Door").gameObject;

        newPath.transform.Find("trigger_Enter").GetComponent<PathTrigger>().TriggerEvent.AddListener(Trigger_OnPathEnter);
        newPath.transform.Find("trigger_Exit").GetComponent<PathTrigger>().TriggerEvent.AddListener(Trigger_OnPathExit);
        status = "Active";

        choosenDoor.SetActive(false);
    }

    IEnumerator DestroyPath(GameObject obj, float timeToWait, bool hide = false)
    {
        yield return new WaitForSeconds(timeToWait);
        obj.SetActive(false);
        if (!hide) Destroy(obj);
    }
    #endregion
}
