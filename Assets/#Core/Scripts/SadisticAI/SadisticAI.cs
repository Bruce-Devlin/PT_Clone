using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Timers;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;

public class Scare
{
    public int id { get; set; }
    public string name { get; set; }
    public Action method { get; set; }
}

public class SadisticAI : MonoBehaviour
{
    #region Variables
    [SerializeField]
    [Header("Options")]


    // The Scene PathManager.
    [HideInInspector]public PathManager pathManager;
    bool debug;
    [HideInInspector]public string status = "";
    public bool triggerDebug = false;

    // The Player Controller.
    public PlayerController playerController;

    // How many loops the player has made.
    [HideInInspector]public int playerLoops = 0; // This is often used as a multiplier, the higher this number the higher all the other paramaters of the AI will be

    ///
    /// AI Settings
    ///
    [Header("AI Settings")]

    // How active the AI is
    [Tooltip("0 - 100 | 0: AI Is inactive, 100: Active every chance it gets")]
    public int activity = 5;

    // How cruel the AI is
    [Tooltip("0 - 100 | 0: AI will never hurt the player, 100: \"Fuck this\"")]
    public int cruelty = 5;

    // How dangerous the AI is to the Player
    [Tooltip("0 - 100 | 0: Player is safe, 100: k???i????l???l??? ????t????h????e????? ?????p????l????a????y?????e?????r????")]
    public int danger = 0;

    // The timer used to dictate when the AI should think.
    [Tooltip("The Time (in seconds) the AI should wait before considering being active again.")]
    public int aiThinkingTime = 30;

    // Random generator used to make things random.
    public System.Random random;

    public bool thinking = false;

    ///
    /// AI Player Triggers
    ///
    [Header("Player Triggers")]

    public bool trigger_Enter_Active = false;
    public bool trigger_Exit_Active = false;

    PathTrigger trigger_FrontDoor;
    private bool trigger_FrontDoor_Active = false;
    PathTrigger trigger_Table;
    private bool trigger_Table_Active = false;
    PathTrigger trigger_ExtraDoor;
    private bool trigger_ExtraDoor_Active = false;

    ///
    /// AI Interactables
    ///
    [Header("AI Interactables")]

    public GameObject interact_FrontDoor;
    public GameObject interact_ExtraDoor_01;
    public GameObject interact_ExtraDoor_02;
    public GameObject interact_ExtraDoor_03;
    public GameObject interact_ExtraDoor_04;

    public GameObject interact_Phone;
    public List<GameObject> interact_CeilingLights;
    public List<GameObject> interact_ExtraRooms;
    public GameObject interact_CurrentExtraRoom;
    public GameObject interact_ShadowWalkby;

    [Space(10)]
    ///
    /// Scare Resources
    ///
    [Header("Scare Resources")]
    public AudioClip audio_creepyBreathing;
    public AudioClip audio_PizzaTime;
    public Text ui_FlashlightHint;
    #endregion

    #region Unity Events
    IEnumerator WaitOnPathManager()
    {
        ulong tics = 0;
        while (!pathManager.loaded)
        {
            if (debug) playerController.Log("Waiting for PathManager to start... (" + tics + "tics)");
            yield return new WaitForSeconds(1);
            tics++;
        }
        StartAI();
    }

    private void Start()
    {
        status = "Loading";
        StartCoroutine(WaitOnPathManager());
    }

    // Update is called once per frame
    void Update()
    {
        if (pathManager.loaded)
        {

            //Observe Player loops and current Path
            playerLoops = pathManager.playerLoops;
        }
    }
    #endregion

    #region AI Functions
    void StartAI()
    {
        pathManager = GetComponent<PathManager>();
        playerController.Log("S_AI: Starting...");
        //scares.sadisticAI = GetComponent<SadisticAI>();
        InvokeRepeating("AiThink", 0f, aiThinkingTime);
        debug = pathManager.debug;
        interact_Phone.GetComponent<Telephone>().pathManager = pathManager;

        StartCoroutine(playerController.ShowHint("\"I have to get out of here, now.\""));

        interact_FrontDoor = pathManager.Interactables.transform.Find("Front_Door").gameObject;

        interact_ExtraDoor_01 = pathManager.Interactables.transform.Find("Extra_Door_01").gameObject;
        interact_ExtraDoor_02 = pathManager.Interactables.transform.Find("Extra_Door_02").gameObject;
        interact_ExtraDoor_03 = pathManager.Interactables.transform.Find("Extra_Door_03").gameObject;
        interact_ExtraDoor_04 = pathManager.Interactables.transform.Find("Extra_Door_04").gameObject;

        interact_Phone = pathManager.Interactables.transform.Find("Telephone").gameObject;

        pathManager.firstPath.transform.Find("trigger_FrontDoor").gameObject.GetComponent<PathTrigger>().TriggerEvent.AddListener(Trigger_FrontDoor_Event);
        pathManager.firstPath.transform.Find("trigger_Table").gameObject.GetComponent<PathTrigger>().TriggerEvent.AddListener(Trigger_Table_Event);
        pathManager.firstPath.transform.Find("trigger_ExtraDoor").gameObject.GetComponent<PathTrigger>().TriggerEvent.AddListener(Trigger_ExtraDoor_Event);

        //Get Lights
        foreach (Transform light in pathManager.firstPath.transform.Find("Lights"))
        {
            interact_CeilingLights.Add(light.gameObject);
        }

        pathManager.EnterDoor.GetComponent<DoorController>().forceOpen = true;
        status = "Active";
    }

    void AiThink()
    {
        if (debug) playerController.Log("S_AI: Player Loops: " + playerLoops);
        if (playerLoops < 2 || pathManager.preventProgress || pathManager.progressingPath)
        {
            if (debug) playerController.Log("S_AI: Either player is below 2 loops, the player has progressed path or preventProgress it true, doing nothing.");
            return;
        }
        else
        {
            thinking = true;
            status = "Thinking...";
            if (debug) playerController.Log("S_AI: Calculating activity...");
            int rndNumber = RollDice("activity random", 3, 100);
            if (debug) playerController.Log("S_AI: Random Number: " + rndNumber);

            float rndWithLoops = rndNumber / playerLoops;
            if (debug) playerController.Log("S_AI: Random Divided by Loops: " + rndWithLoops);

            if (debug) playerController.Log("S_AI: Will AI Act: ( Is number \"" + rndWithLoops + "\" less than activity \"" + activity + "\"");

            if (rndWithLoops < activity && rndWithLoops != 0)
            {
                if (debug) playerController.Log("S_AI: Yes. Time to make this fun...");
                if (danger < 50) //Scare the player, no danger.
                {
                    if (debug) playerController.Log("S_AI: I will scare the player.");
                    ScareHandler();
                }
                else
                {
                    if (debug) playerController.Log("S_AI: I will attempt to hurt the player.");
                    playerController.TakeDamage(RollDice("hurt player", 10, 20));

                    danger = 0;
                }
            }
            else if (debug) playerController.Log("S_AI: No. Remaining idle, for now...");
            thinking = false;
            status = "Active";
        }
    }
    #endregion

    #region Scare Handler
    List<Scare> scares = null;
    List<Scare> ScareCompilier()
    {
        if (debug) playerController.Log("S_AI: Compiling scares...");
        // Here we create and compile all the available scares
        List<Scare> scares = new List<Scare>()
        {
            new Scare()
            {
            id = 0,
            name = "BurstLight",
            method = BurstLight,
            },
            new Scare()
            {
            id = 1,
            name = "PhoneRing",
            method = PhoneRing,
            },
            new Scare()
            {
            id = 2,
            name = "KnockKnock",
            method = KnockKnock,
            },
            new Scare()
            {
            id = 3,
            name = "SwingChandiler",
            method = SwingChandiler,
            },
            new Scare()
            {
            id = 4,
            name = "RandomRoom",
            method = RandomRoom,
            }
        };

        if (debug)
        {
            foreach (Scare scare in scares)
            {
                playerController.Log("S_AI: Compiled scare: \"" + scare.name + "\" ID: \"" + scare.id + "\"");
            }
        }

        return scares;
    }

    int RollDice(string title, int minValue, int maxValue)
    {
        if (debug) playerController.Log("S_AI: Rolling the dice for \"" + title +  "\" (" + minValue + "-" + maxValue + ")");
        random = new System.Random();
        int result = random.Next(minValue, maxValue);
        if (debug) playerController.Log("S_AI: Dice landed: " + result);
        return result;
    }

    void ScareHandler()
    {
        if (scares == null ) scares = ScareCompilier();

        if (debug) playerController.Log("S_AI: Deciding how to scare the player...");
        if (trigger_Enter_Active)
        {
            switch (RollDice("scares", 1, 5))
            {
                case 1: scares[0].method.Invoke(); break;
                case 2: scares[1].method.Invoke(); break;
                case 3: scares[2].method.Invoke(); break;
                case 4: scares[3].method.Invoke(); break;
                case 5: scares[4].method.Invoke(); break;
            }
        }
        else if (trigger_FrontDoor_Active)
        {
            if (debug) playerController.Log("S_AI: Doing something near the front door...");
            switch (RollDice("scares", 1, 4))
            {
                case 1: scares[0].method.Invoke(); break;
                case 2: scares[1].method.Invoke(); break;
                case 3: scares[2].method.Invoke(); break;
                case 4: scares[4].method.Invoke(); break;
            }
        }
        else if (trigger_Table_Active)
        {
            if (debug) playerController.Log("S_AI: Doing something near the table...");
            switch (RollDice("scares", 1, 3))
            {
                case 1: scares[0].method.Invoke(); break;
                case 2: scares[1].method.Invoke(); break;
                case 3: scares[4].method.Invoke(); break;
            }
        }
        else if (trigger_ExtraDoor_Active)
        {
            if (debug) playerController.Log("S_AI: Doing something near the Extra door...");
            switch (RollDice("scares", 1, 5))
            {
                case 1: scares[0].method.Invoke(); break;
                case 2: scares[1].method.Invoke(); break;
                case 3: scares[2].method.Invoke(); break;
                case 4: scares[3].method.Invoke(); break;
                case 5: scares[4].method.Invoke(); break;
            }
        }
        else
        {
            switch (RollDice("scares", 1, 4))
            {
                case 1: scares[0].method.Invoke(); break;
                case 2: scares[1].method.Invoke(); break;
                case 3: scares[2].method.Invoke(); break;
                case 4: scares[4].method.Invoke(); break;
            }
        }
        danger = danger + (cruelty * RollDice("danger multiplier", cruelty + 2,8));
    }
    #endregion
    #region Scares
    public void BurstLight()
    {
        List<GameObject> lights = new List<GameObject>();

        foreach (GameObject _tmpLight in interact_CeilingLights)
        {
            if (_tmpLight.name.Contains("Ceiling Light"))
            {
                if (!_tmpLight.GetComponent<CeilingLights>().burst)
                {
                    lights.Add(_tmpLight);
                }
            }
            else if (_tmpLight.name.Contains("Chandiler"))
            {
                if (!_tmpLight.GetComponent<Chandiler>().burst)
                {
                    lights.Add(_tmpLight);
                }
            }
        }

        if (!playerController.canUseFlashlight)
        {
            playerController.canUseFlashlight = true;
            StartCoroutine(playerController.ShowHint("(Press F for flashlight)"));
        }

        if (lights.Count == 0)
        {
            if (debug) playerController.Log("S_AI: No lights to burst");
        }
        else
        {
            GameObject lightToBurst = lights[RollDice("light to burst", 0, lights.Count)];

            if (lightToBurst.name.Contains("Ceiling Light"))
            {
                lightToBurst.GetComponent<CeilingLights>().shouldBurst = true;
            }
            else if (lightToBurst.name.Contains("Chandiler"))
            {
                lightToBurst.GetComponent<Chandiler>().shouldBurst = true;
            }
        }
    }

    public void PhoneRing()
    {
        interact_Phone.GetComponent<Telephone>().shouldRing = true;
        interact_Phone.GetComponent<Telephone>().callSound = audio_creepyBreathing;
        StartCoroutine(playerController.ShowHint("\"What the fuck, is that a phone ringing?\""));
    }

    public void KnockKnock()
    {
        if (trigger_FrontDoor_Active)
        {
            interact_FrontDoor.GetComponent<AudioSource>().Play();
        }
        else if (trigger_ExtraDoor_Active)
        {
            interact_ExtraDoor_01.GetComponent<AudioSource>().Play();
        }
        else if (trigger_Enter_Active)
        {
            pathManager.EnterDoor.GetComponent<AudioSource>().Play();
        }
    }

    public void SwingChandiler()
    {
        foreach (GameObject _tmpLight in interact_CeilingLights)
        {
            if (_tmpLight.gameObject.name.Contains("Chandiler"))
            {
                _tmpLight.GetComponent<Chandiler>().shouldSwing = true;
            }
        }
    }
    GameObject choosenDoor;
    public void RandomRoom()
    {
        pathManager.preventProgress = true;
        List<GameObject> doors = new List<GameObject>()
        {
            pathManager.ExitDoor,
            interact_ExtraDoor_01,
            interact_ExtraDoor_03,
        };

        choosenDoor = doors[RollDice("choosen door", 0, doors.Count)];

        Vector3 newPosition = new Vector3(-40,0);
        Quaternion newRotation = new Quaternion();

        interact_CurrentExtraRoom = Instantiate(interact_ExtraRooms[RollDice("extra room", 0, interact_ExtraRooms.Count)]);

        interact_CurrentExtraRoom.transform.parent = pathManager.currentPath.transform.Find("AI_Interactables");
        interact_CurrentExtraRoom.transform.position = choosenDoor.transform.position;
        interact_CurrentExtraRoom.transform.rotation = choosenDoor.transform.rotation;

        choosenDoor.GetComponent<DoorController>().forceOpen = true;
    }

    public void DestroyExtraRoom()
    {
        choosenDoor.GetComponent<DoorController>().forceClose = true;
        Destroy(interact_CurrentExtraRoom);
        interact_CurrentExtraRoom = null;
        pathManager.preventProgress = false;
    }

    public void ShadowWalkBy()
    {

    }
    #endregion

    #region Trigger Handlers
    public void Trigger_FrontDoor_Event(Collider collider)
    {
        if (collider == null)
        {
            trigger_FrontDoor_Active = false;
            if (triggerDebug) playerController.Log("S_AI: Player left FrontDoor Trigger.");

        }
        else if (collider.gameObject.tag == "Player")
        {
            trigger_FrontDoor_Active = true;
            if (triggerDebug) playerController.Log("S_AI: Player entered FrontDoor Trigger.");

        }
    }
    public void Trigger_Table_Event(Collider collider)
    {
        if (collider == null)
        {
            trigger_Table_Active = false;
            if (triggerDebug) playerController.Log("S_AI: Player left Table Trigger.");
        }
        else if (collider.gameObject.tag == "Player")
        {
            trigger_Table_Active = true;
            if (triggerDebug) playerController.Log("S_AI: Player entered Table Trigger.");
        }
    }
    public void Trigger_ExtraDoor_Event(Collider collider)
    {
        if (collider == null)
        {
            trigger_ExtraDoor_Active = false;
            if (triggerDebug) playerController.Log("S_AI: Player left ExtraDoor Trigger.");

        }
        else if (collider.gameObject.tag == "Player")
        {
            trigger_ExtraDoor_Active = true;
            if (triggerDebug) playerController.Log("S_AI: Player entered ExtraDoor Trigger.");

        }
    }
    #endregion
}
