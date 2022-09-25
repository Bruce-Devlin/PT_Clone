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
    void Trigger_OnPathEnter(Collider collider)
    {
        if (collider != null && collider.gameObject.tag == "Player")
        {
            sadisticAI.trigger_Enter_Active = true;
            if (!basementUsed)
            {
                basementUsed = true;
                EnterDoor.GetComponent<DoorController>().forceClose = true;
                basement.SetActive(false);
                currentPath = firstPath;
                firstPath = null;
            }

            if (_Path != null)
            {
                playerLoops++;
                if (debug) playerController.Log("P_Man: Player entered new Path (loops: " + playerLoops + ")");
                progressingPath = false;

                EnterDoor = _EnterDoor;
                ExitDoor = _ExitDoor;

                EnterDoor.GetComponent<DoorController>().forceClose = true;

                Destroy(currentPath);
                currentPath = _Path;

                _Path = null;
                _ExitDoor = null;
                _ExitDoor = null;

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
            }
        }
        else if (collider == null)
        {
            sadisticAI.trigger_Enter_Active = false;
        }
            
    }
    void Trigger_OnPathExit(Collider collider)
    {
        if (collider != null && collider.gameObject.tag == "Player")
        {
            sadisticAI.trigger_Exit_Active = true;
            if (_Path == null && !preventProgress && !sadisticAI.thinking)
            {
                progressingPath = true;
                SpawnNewPath();
                ExitDoor.GetComponent<DoorController>().forceOpen = true;
                _EnterDoor.GetComponent<DoorController>().locked = false;
            }
        }
        else if (collider == null)
        {
            sadisticAI.trigger_Exit_Active = false;
        }
    }
    #endregion

    #region Functions
    private GameObject _Path = null;

    private GameObject _EnterDoor = null;
    private GameObject _ExitDoor = null;

    void SpawnNewPath()
    {
        Vector3 newPosition = new Vector3();

        if (currentPath.transform.position.x.ToString().Contains("-")) newPosition.x = float.Parse(currentPath.transform.position.x.ToString().Replace("-", ""));
        else newPosition.x = float.Parse("-" + currentPath.transform.position.x.ToString());

        if (currentPath.transform.position.z.ToString().Contains("-")) newPosition.z = float.Parse(currentPath.transform.position.z.ToString().Replace("-", ""));
        else newPosition.z = float.Parse("-" + currentPath.transform.position.z.ToString());

        Quaternion newRotation = new Quaternion(0, 0, 0, 0);

        if (currentPath.transform.rotation.y == 0) newRotation.y = 180;
        else newRotation.y = 0;

        _Path = Instantiate(currentPath, newPosition, newRotation);

        _EnterDoor = _Path.transform.Find("AI_Interactables").transform.Find("Enter_Door").gameObject;
        _ExitDoor = _Path.transform.Find("AI_Interactables").transform.Find("Exit_Door").gameObject;

        _Path.transform.Find("trigger_Enter").GetComponent<PathTrigger>().TriggerEvent.AddListener(Trigger_OnPathEnter);
        _Path.transform.Find("trigger_Exit").GetComponent<PathTrigger>().TriggerEvent.AddListener(Trigger_OnPathExit);
        status = "Active";
    }
    #endregion
}
