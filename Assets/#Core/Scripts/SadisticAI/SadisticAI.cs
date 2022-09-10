using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Timers;
using UnityEngine;
using UnityEngine.UI;

public class SadisticAI : MonoBehaviour
{
    #region Variables
    [SerializeField]
    [Header("Options")]


    // The Scene PathManager.
    public PathManager pathManager;
    bool debug;

    // The Player Controller.
    public PlayerController playerController;

    // How many loops the player has made.
    [HideInInspector]public int playerLoops = 0; // This is often used as a multiplier, the higher this number the higher all the other paramaters of the AI will be
    // Which path the player is currently in.
    [HideInInspector]public int currentPath = 0; // 1 || 2 (Current path the player is in)

    ///
    /// AI Settings
    ///
    [Header("AI Settings")]

    // How active the AI is
    [Tooltip("0 - 100 | 0: AI Is inactive, 100: \"Fuck this\"")]
    public int cruelty = 5;

    // How dangerous the AI is to the Player
    [Tooltip("0 - 100 | 0: Player is safe, 100: Player will be insta-killed")]
    public int danger = 0;

    // The timer used to dictate when the AI should think.
    [Tooltip("The Time (in seconds) the AI should wait before considering being active again.")]
    public int aiThinkingTime = 30;

    // Random generator used to make things random.
    public System.Random random = new System.Random();

    ///
    /// AI Player Triggers
    ///
    [Header("Player Triggers")]

    [Header("Path 1")]
    public PathTrigger trigger_FrontDoor_01;
    private bool trigger_FrontDoor_01_Active = false;
    public PathTrigger trigger_Table_01;
    private bool trigger_Table_01_Active = false;
    public PathTrigger trigger_ExtraDoor_01;
    private bool trigger_ExtraDoor_01_Active = false;

    [Header("Path 2")]
    public PathTrigger trigger_FrontDoor_02;
    private bool trigger_FrontDoor_02_Active = false;
    public PathTrigger trigger_Table_02;
    private bool trigger_Table_02_Active = false;
    public PathTrigger trigger_ExtraDoor_02;
    private bool trigger_ExtraDoor_02_Active = false;

    ///
    /// AI Interactables
    ///
    [Header("AI Interactables")]

    [Header("Path 1")]
    public GameObject interact_FrontDoor_01;
    public GameObject interact_Path1Door_01;
    public GameObject interact_Path2Door_01;
    public GameObject interact_ExtraDoor_01;
    public GameObject interact_Phone_01;
    public List<GameObject> interact_CeilingLights_01;
    public List<GameObject> interact_CeilingLights_02;

    [Header("Path 2")]
    public GameObject interact_FrontDoor_02;
    public GameObject interact_Path1Door_02;
    public GameObject interact_Path2Door_02;
    public GameObject interact_ExtraDoor_02;
    public GameObject interact_Phone_02;

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
    private void OnEnable()
    {
        pathManager.Log("SadisticAI: SadisticAI: Starting...");
        //scares.sadisticAI = GetComponent<SadisticAI>();
        InvokeRepeating("AiThink", 0f, aiThinkingTime);
        debug = pathManager.debug;
        interact_Phone_01.GetComponent<Telephone>().pathManager = pathManager;

        StartCoroutine(playerController.ShowHint("I have to get out of here, now."));

        //Handle Triggers
        trigger_FrontDoor_01.TriggerEvent.AddListener(Trigger_FrontDoor_01_Event);
        trigger_Table_01.TriggerEvent.AddListener(Trigger_Table_01_Event);
        trigger_ExtraDoor_01.TriggerEvent.AddListener(Trigger_ExtraDoor_01_Event);

        trigger_FrontDoor_02.TriggerEvent.AddListener(Trigger_FrontDoor_02_Event);
        trigger_Table_02.TriggerEvent.AddListener(Trigger_Table_02_Event);
        trigger_ExtraDoor_02.TriggerEvent.AddListener(Trigger_ExtraDoor_02_Event);
    }

    // Update is called once per frame
    void Update()
    {
        //Observe Player loops and current Path
        playerLoops = pathManager.playerLoops;
        currentPath = pathManager.currentPath;
    }

    #endregion

    #region AI Thinking Handler
    void AiThink()
    {
        if (debug) pathManager.Log("SadisticAI: Player Loops: " + playerLoops + " Players Current Path: " + currentPath);

        if (playerLoops < 2 || pathManager.preventProgress)
        {
            if (debug) pathManager.Log("Either player is below 2 loops or preventProgress it true, doing nothing.");
            return;
        }
        if (!debug)
        {
            if (random.Next(1, 100) / playerLoops < cruelty)
            {
                ScareHandler();
            }
        }
        else
        {
            pathManager.Log("SadisticAI: Calculating activity...");
            int rndNumber = random.Next(1, 100);
            pathManager.Log("SadisticAI: Random Number: " + rndNumber);

            float rndWithLoops = rndNumber / playerLoops;
            pathManager.Log("SadisticAI: Random Divided by Loops: " + rndWithLoops);

            pathManager.Log("SadisticAI: Will AI Act: ( Is number \"" + rndWithLoops + "\" less than cruelty \"" + cruelty + "\"");

            if (rndWithLoops < cruelty && rndWithLoops != 0)
            {
                pathManager.Log("SadisticAI: Yes. Time to make this fun...");
                ScareHandler();
            }
            else pathManager.Log("SadisticAI: No. Remaining idle, for now...");
        }
    }
    #endregion

    #region Scares
    void ScareHandler()
    {
        if (debug) pathManager.Log("SadisticAI: Deciding how to scare the player...");
        if (trigger_FrontDoor_01_Active) // Do something near the front door (Path 1)
        {
            if (debug) pathManager.Log("SadisticAI: Doing something near the front door...");
            KnockKnock(interact_FrontDoor_01);
        }
        else if (trigger_Table_01_Active) // Do something near the table (Path 1)
        {
            if (debug) pathManager.Log("SadisticAI: Doing something near the table...");
            PhoneRing();
        }
        else if (trigger_ExtraDoor_01_Active)// Do something near the Extra door (Path 1)
        {
            if (debug) pathManager.Log("SadisticAI: Doing something near the Extra door...");
            KnockKnock(interact_ExtraDoor_01);
        }
        else if (trigger_FrontDoor_02_Active) // Do something near the front door (Path 2)
        {
            if (debug) pathManager.Log("SadisticAI: Doing something near the front door (Path 2)...");
            KnockKnock(interact_FrontDoor_02);
        }
        else if (trigger_Table_02_Active) // Do something near the table (Path 2)
        {
            if (debug) pathManager.Log("SadisticAI: Doing something near the table (Path 2)...");
            PhoneRing();
        }
        else if (trigger_ExtraDoor_02_Active)
        {
            if (debug) pathManager.Log("SadisticAI: Doing something near the Extra door...");
            KnockKnock(interact_ExtraDoor_02);

        }
        else // Do something else anywhere
        {
            if (currentPath == 1)
            {
                if (debug) pathManager.Log("SadisticAI: Doing something somewhere... (Path 1)");

                TurnOffLights();
            }
            else if (currentPath == 2)
            {
                if (debug) pathManager.Log("SadisticAI: Doing something somewhere... (Path 2)");
                TurnOffLights();
            }
            else
            {

            }
        }
    }
    #endregion

    #region Scares
    bool lightsOff = false;
    public void TurnOffLights()
    {
        if (!lightsOff)
        {
            lightsOff = true;
            playerController.canUseFlashlight = true;
            StartCoroutine(playerController.ShowHint("I'm sure I got a flashlight here somewhere..."));

            playerController.playerFlashlight.transform.Find("Bulb").GetComponent<Light>().enabled = true;
            playerController.playerFlashlight.SetActive(true);

            if (debug) pathManager.Log("Bursting Lights...");
            foreach (GameObject obj in interact_CeilingLights_01)
            {
                if (debug) pathManager.Log("Object: " + obj.name);
                if (obj.name == "Ceiling Light")
                {
                    obj.GetComponent<CeilingLights>().shouldBurst = true;
                }
                else if (obj.name.StartsWith("Lamp")) obj.GetComponent<Light>().enabled = false;
            }
            foreach (GameObject obj in interact_CeilingLights_02)
            {
                if (debug) pathManager.Log("Object: " + obj.name);
                if (obj.name == "Ceiling Light")
                {
                    obj.GetComponent<CeilingLights>().shouldBurst = true;
                }
                else if (obj.name.StartsWith("Lamp")) obj.GetComponent<Light>().enabled = false;
            }
        }
    }

    public void PhoneRing()
    {
        pathManager.preventProgress = true;
        if (currentPath == 1)
        {
            interact_Phone_01.GetComponent<Telephone>().shouldRing = true;
            interact_Phone_01.GetComponent<Telephone>().callSound = audio_creepyBreathing;
            StartCoroutine(playerController.ShowHint("what the fuck, is that a phone ringing?"));
        }
        else
        {
            interact_Phone_02.GetComponent<Telephone>().shouldRing = true;
            interact_Phone_02.GetComponent<Telephone>().callSound = audio_creepyBreathing;
            StartCoroutine(playerController.ShowHint("what the fuck, is that a phone ringing?"));
        }
    }

    public void KnockKnock(GameObject door)
    {
        door.GetComponent<AudioSource>().Play();
    }
    #endregion

    #region Trigger Handlers
    #region Path 1
    void Trigger_FrontDoor_01_Event(Collider collider)
    {
        if (collider == null)
        {
            trigger_FrontDoor_01_Active = false;
            if (debug) pathManager.Log("SadisticAI: Player left FrontDoor Trigger. (Path 1)");

        }
        else if (collider.gameObject.tag == "Player")
        {
            trigger_FrontDoor_01_Active = true;
            if (debug) pathManager.Log("SadisticAI: Player entered FrontDoor Trigger. (Path 1)");

        }
    }
    void Trigger_Table_01_Event(Collider collider)
    {
        if (collider == null)
        {
            trigger_Table_01_Active = false;
            if (debug) pathManager.Log("SadisticAI: Player left Table Trigger. (Path 1)");
        }
        else if (collider.gameObject.tag == "Player")
        {
            trigger_Table_01_Active = true;
            if (debug) pathManager.Log("SadisticAI: Player entered Table Trigger. (Path 1)");
        }
    }
    void Trigger_ExtraDoor_01_Event(Collider collider)
    {
        if (collider == null)
        {
            trigger_ExtraDoor_01_Active = false;
            if (debug) pathManager.Log("SadisticAI: Player left ExtraDoor Trigger. (Path 1)");

        }
        else if (collider.gameObject.tag == "Player")
        {
            trigger_ExtraDoor_01_Active = true;
            if (debug) pathManager.Log("SadisticAI: Player entered ExtraDoor Trigger. (Path 1)");

        }
    }
    #endregion
    #region Path 2
    void Trigger_FrontDoor_02_Event(Collider collider)
    {
        if (collider == null)
        {
            trigger_FrontDoor_02_Active = false;
            if (debug) pathManager.Log("SadisticAI: Player left FrontDoor Trigger. (Path 2)");

        }
        else if (collider.gameObject.tag == "Player")
        {
            trigger_FrontDoor_02_Active = true;
            if (debug) pathManager.Log("SadisticAI: Player entered FrontDoor Trigger. (Path 2)");

        }
    }
    void Trigger_Table_02_Event(Collider collider)
    {
        if (collider == null)
        {
            trigger_Table_02_Active = false;
            if (debug) pathManager.Log("SadisticAI: Player left Table Trigger. (Path 2)");
        }
        else if (collider.gameObject.tag == "Player")
        {
            trigger_Table_02_Active = true;
            if (debug) pathManager.Log("SadisticAI: Player entered Table Trigger. (Path 2)");
        }
    }
    void Trigger_ExtraDoor_02_Event(Collider collider)
    {
        if (collider == null)
        {
            trigger_ExtraDoor_02_Active = false;
            if (debug) pathManager.Log("SadisticAI: Player left ExtraDoor Trigger. (Path 2)");

        }
        else if (collider.gameObject.tag == "Player")
        {
            trigger_ExtraDoor_02_Active = true;
            if (debug) pathManager.Log("SadisticAI: Player entered ExtraDoor Trigger. (Path 2)");

        }
    }
    #endregion
    #endregion
}
