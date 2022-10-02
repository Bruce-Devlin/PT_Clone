using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class PlayerController : MonoBehaviour
{
    #region Variables
    public bool debug = false;
    public bool debugStats = false;
    public bool debugCam = false;
    public bool printDebugToHint = false;
    System.Random random;

    [Header("Debug Settings")]
    [SerializeField]
    public float flySpeed = 0.5f;

    [SerializeField]
    float accelerationRatio = 2;
    float slowDownRatio = 0.2f;

    [Header("Player Settings")]
    public bool invincible = false;
    public bool canBlink = true;
    public bool canCloseEyes = true;
    public int blinkInterval;
    public float blinkLength;
    public bool blinking = false;
    public bool eyesClosed = false;
    public float health = 100.0f;

    public bool canUseFlashlight = false;
    public bool canMove = true;

    public bool canSprint = true;

    public float movementSpeed;
    private float sprintSpeed;
    private float currMovementSpeed;

    public float MaximumVelocity = 1f;

    public bool moving = false;
    public bool sprinting = false;

    float distToGround;
    bool isGrounded()
    {
        return true;
        int layer_mask = LayerMask.GetMask("Floor");

        return Physics.Raycast(transform.position, -Vector3.up, distToGround + 5f, layer_mask);
    }

    public bool canLook = true;
    float sensitivityX;
    float sensitivityY;
    string cameraMovement = "";
    string quality = "";

    public float minimumX = -360F;
    public float maximumX = 360F;

    public float minimumY = -60F;
    public float maximumY = 60F;

    float rotationY = 0F;
    float rotationX = 0F;


    [Header("Objects")]
    public PathManager pathManager;

    public Transform playerHead;
    public Transform playerCamera;

    public Transform playerBody;
    public GameObject playerFlashlight;
    public SadisticAI sadisticAI;
    public GameObject userInterface;
    GameObject debugStatsUI = null;

    public AudioSource footSteps;

    private float _lastPosX;

    private bool walking()
    {
        float newPosX = transform.position.x;
        if (newPosX != _lastPosX)
        {
            _lastPosX = newPosX;
            return true;
        }
        else return false;
    }
    private bool playingFootsteps = false;

    public AudioClip[] _clips;
    public AudioClip this[int index]
    {
        get { return _clips[index]; }
    }

    public AudioClip PickRandomFootstep()
    {
        //int v = sadisticAI. (0, _clips.Length);
        return _clips[1];
    }
    #endregion

    #region Unity Methods
    private void OnEnable()
    {
        cameraMovement = PlayerPrefs.GetString("CameraMovement");
        sensitivityX = PlayerPrefs.GetFloat("Sensitivity");
        quality = PlayerPrefs.GetString("Quality");

        if (quality == "high")
        {
            QualitySettings.SetQualityLevel(0, true);
        }
        else if (quality == "medium")
        {
            QualitySettings.SetQualityLevel(1, true);
        }
        if (quality == "high")
        {
            QualitySettings.SetQualityLevel(2, true);
        }
    }

    // Start is called before the first frame update
    void Start()
    {
        debugStatsUI = userInterface.transform.Find("DebugStats").gameObject;
        if (debugStatsUI.active && !debugStats) debugStatsUI.SetActive(false);

        if (canLook) Cursor.lockState = CursorLockMode.Locked;

        playerFlashlight.transform.Find("Bulb").GetComponent<Light>().enabled = false;
        playerFlashlight.SetActive(false);

        distToGround = playerBody.GetComponent<CapsuleCollider>().bounds.extents.y;

        StartCoroutine(Blinking());
    }

    private void FixedUpdate()
    {
        #region Player Camera Controls
        sensitivityY = sensitivityX;
        if (canLook)
        {
            rotationY += Input.GetAxis("Mouse Y") * sensitivityY;
            rotationY = Mathf.Clamp(rotationY, minimumY, maximumY);

            rotationX += Input.GetAxis("Mouse X") * sensitivityX;
            //Debug.Log("X: " + sensitivityX + " \\ " + rotationX + " | Y: " + sensitivityY + " \\ " + rotationY);

            #region Realist camera movement
            if (cameraMovement == "real")
            {
                //Camera LERP
                if (playerCamera.transform.eulerAngles.x != -rotationY)
                {
                    Vector3 lerpRotateY =
                        new Vector3(
                            Mathf.LerpAngle(
                                    playerCamera.transform.eulerAngles.x,
                                    -rotationY,
                                    sensitivityY * 5 * Time.deltaTime
                                ),
                            rotationX,
                            0);
                    playerCamera.eulerAngles = lerpRotateY;
                }

                //Body Lerp
                if (transform.eulerAngles.y != rotationX)
                {
                    Vector3 lerpRotateX =
                        new Vector3(
                            0,
                            Mathf.LerpAngle(
                                transform.eulerAngles.y,
                                rotationX,
                                sensitivityX * 5 * Time.deltaTime
                                ),
                            0);
                    transform.eulerAngles = lerpRotateX;
                }

            }
            #endregion
            #region Classic camera movement
            else
            {
                rotationY += Input.GetAxis("Mouse Y") * sensitivityY;
                rotationY = Mathf.Clamp(rotationY, minimumY, maximumY);

                rotationX += Input.GetAxis("Mouse X") * sensitivityX;

                transform.Rotate(-transform.up * rotationX);
            }
            #endregion

        }
        #endregion

        #region Debug Movement Controls
        if (debugCam)
        {
            //use shift to speed up flight
            if (Input.GetKeyDown(KeyCode.LeftShift) || Input.GetKeyDown(KeyCode.RightShift))
            {
                flySpeed *= accelerationRatio;
            }

            if (Input.GetKeyUp(KeyCode.LeftShift) || Input.GetKeyUp(KeyCode.RightShift))
            {
                flySpeed /= accelerationRatio;
            }


            //use ctrl to slow up flight
            if (Input.GetKeyDown(KeyCode.LeftControl) || Input.GetKeyDown(KeyCode.RightControl))
            {
                flySpeed *= slowDownRatio;
            }

            if (Input.GetKeyUp(KeyCode.LeftControl) || Input.GetKeyUp(KeyCode.RightControl))
            {
                flySpeed /= slowDownRatio;
            }

            if (Input.GetAxis("Vertical") != 0)
            {
                transform.Translate(Vector3.forward * flySpeed * Input.GetAxis("Vertical"));
            }

            if (Input.GetAxis("Horizontal") != 0)
            {
                transform.Translate(Vector3.right * flySpeed * Input.GetAxis("Horizontal"));
            }

            if (Input.GetKey(KeyCode.E))
            {
                transform.Translate(Vector3.up * flySpeed);
            }
            else if (Input.GetKey(KeyCode.Q))
            {
                transform.Translate(Vector3.down * flySpeed);
            }
        }
        #endregion

        #region Player Movement Controls
        sprintSpeed = movementSpeed * 2;
        if (canMove && isGrounded())
        {
            if (Input.GetKey(KeyCode.LeftShift) || Input.GetKey(KeyCode.RightShift))
            {
                sprinting = true;
                currMovementSpeed = sprintSpeed;
            }
            else
            {
                sprinting = false;
                currMovementSpeed = movementSpeed;
            }

            if (Input.GetKey(KeyCode.W))
            {
                moving = true;
                GetComponent<Rigidbody>().AddForce(transform.forward * currMovementSpeed, ForceMode.Acceleration);
            }
            else moving = false;

            if (Input.GetKey(KeyCode.A))
            {
                moving = true;
                GetComponent<Rigidbody>().AddForce(-transform.right * currMovementSpeed, ForceMode.Acceleration);
            }
            else moving = false;


            if (Input.GetKey(KeyCode.S))
            {
                moving = true;
                GetComponent<Rigidbody>().AddForce(-1 * transform.forward * currMovementSpeed, ForceMode.Acceleration);
            }
            else moving = false;


            if (Input.GetKey(KeyCode.D))
            {
                moving = true;
                GetComponent<Rigidbody>().AddForce(transform.right * currMovementSpeed, ForceMode.Acceleration);

            }
            else moving = false;

            float tmpMaxVolocity = MaximumVelocity;

            if (sprinting) tmpMaxVolocity *= 2;

            GetComponent<Rigidbody>().velocity = new Vector3(Mathf.Clamp(GetComponent<Rigidbody>().velocity.x, -tmpMaxVolocity, tmpMaxVolocity),
                              GetComponent<Rigidbody>().velocity.y,
                              Mathf.Clamp(GetComponent<Rigidbody>().velocity.z, -tmpMaxVolocity, tmpMaxVolocity));
    }
        #endregion
    }

    // Update is called once per frame
    void Update()
    {
        if (debugStats) UpdateStats();

        #region Close eyes
        if (canCloseEyes)
        {
            if (Input.GetKeyDown(KeyCode.Space) && !eyesClosed)
            {
                eyesClosed = true;
                playerCamera.GetComponent<Camera>().nearClipPlane = 999f;
            }
            else if (Input.GetKeyUp(KeyCode.Space) && eyesClosed)
            {
                eyesClosed = false;
                playerCamera.GetComponent<Camera>().nearClipPlane = 0.01f;
            }
        }
        #endregion

        #region Toggle Flashlight
        if (Input.GetKeyDown(KeyCode.F))
        {
            if (canUseFlashlight)
            {
                if (playerFlashlight.transform.Find("Bulb").GetComponent<Light>().enabled)
                {
                    playerFlashlight.transform.Find("Bulb").GetComponent<Light>().enabled = false;
                    playerFlashlight.SetActive(false);
                }
                else
                {
                    playerFlashlight.transform.Find("Bulb").GetComponent<Light>().enabled = true;
                    playerFlashlight.SetActive(true);
                }
            }
        }
        #endregion

        #region Flashlight movement controller
        if (playerFlashlight.transform.Find("Bulb").GetComponent<Light>().enabled)
        {
            Vector3 currentAngle = new Vector3(
             Mathf.LerpAngle(playerFlashlight.transform.eulerAngles.x, playerCamera.transform.eulerAngles.x, Time.deltaTime * sensitivityX / 2),
             Mathf.LerpAngle(playerFlashlight.transform.eulerAngles.y, playerCamera.transform.eulerAngles.y, Time.deltaTime * sensitivityX / 2),
             Mathf.LerpAngle(playerFlashlight.transform.eulerAngles.z, playerCamera.transform.eulerAngles.z, Time.deltaTime * sensitivityX / 2));

            playerFlashlight.transform.eulerAngles = currentAngle;
        }
        #endregion

        #region Debug Functions
        if (debug && Input.GetKeyDown(KeyCode.F1))
        {
            if (debugCam)
            {
                StartCoroutine(ShowHint("Disabling freecam"));
                playerBody.gameObject.GetComponent<CapsuleCollider>().enabled = true;
                playerHead.Find("Head").gameObject.GetComponent<SphereCollider>().enabled = true;
                this.GetComponent<Rigidbody>().useGravity = true;
                debugCam = false;
            }
            else
            {
                StartCoroutine(ShowHint("Enabling Freecam"));
                playerBody.gameObject.GetComponent<CapsuleCollider>().enabled = false;
                playerHead.Find("Head").gameObject.GetComponent<SphereCollider>().enabled = false;
                this.GetComponent<Rigidbody>().useGravity = false;
                canUseFlashlight = true;
                debugCam = true;
            }
        }
        if (debug && Input.GetKeyDown(KeyCode.F2))
        {
            if (sadisticAI.activity != 100)
            {
                StartCoroutine(ShowHint("activity: 100"));
                sadisticAI.activity = 100;
                if (pathManager.playerLoops <= 2) pathManager.playerLoops = 2;
            }
            else
            {
                StartCoroutine(ShowHint("activity: 10"));
                sadisticAI.activity = 10;
            }
        }
        if (debug && Input.GetKeyDown(KeyCode.F3))
        {
            if (printDebugToHint) printDebugToHint = false;
            else printDebugToHint = true;
        }
        if (debug && Input.GetKeyDown(KeyCode.F4))
        {
            SceneManager.LoadScene(0);
        }
        if (debug && Input.GetKeyDown(KeyCode.F5))
        {
            if (!debugStats)
            {
                debugStats = true;
                debugStatsUI.SetActive(true);
            }
            else
            {
                debugStats = false;
                debugStatsUI.SetActive(false);
            }
            
        }
        #endregion
    }
    #endregion

    #region Functions
    public void Log(string txt = "")
    {
        if (debugCam || printDebugToHint || debugStats) StartCoroutine(ShowHint(txt, false));
        Debug.Log(txt);
    }

    public void TakeDamage(float amount)
    {
        if (!invincible)
        {
            health = health - amount;
            if (health <= 20)
            {

            }
            else if (health <= 50)
            {

            }
        }
    }    

    IEnumerator Blinking()
    {
        while (true)
        {
            if (!eyesClosed && canBlink)
            {
                canCloseEyes = false;
                playerCamera.GetComponent<Camera>().nearClipPlane = 999f;
            }
            yield return new WaitForSeconds(blinkLength);
            if (!eyesClosed && canBlink)
            {
                playerCamera.GetComponent<Camera>().nearClipPlane = 0.01f;
                canCloseEyes = true;
            }

            yield return new WaitForSeconds(SadisticAI.RollDice("blinking", 0, blinkInterval));
        } 
    }

    public IEnumerator ShowHint(string hintTxt = "", bool waitAndClear = true)
    {
        userInterface.transform.Find("Hint").GetComponent<Text>().text = hintTxt.ToUpper();
        if (waitAndClear)
        {
            yield return new WaitForSeconds(5);
            userInterface.transform.Find("Hint").GetComponent<Text>().text = "";
        }
    }

    IEnumerator PlayFootSteps()
    {
        while (walking())
        {
            AudioClip clip = PickRandomFootstep();
            footSteps.clip = clip;
            footSteps.Play();
            yield return new WaitForSeconds(clip.length);
        }
    }

    void UpdateStats()
    {
        //FPS Counter
        float fps = 1.0f / Time.smoothDeltaTime;
        Text fpsTxt = debugStatsUI.transform.Find("FPS_Counter").GetComponent<Text>();
        fpsTxt.text = fps.ToString().Substring(0, fps.ToString().LastIndexOf(".") + 2);

        if (fps < 30) fpsTxt.color = Color.red;
        else if (fps < 50) fpsTxt.color = Color.yellow;
        else fpsTxt.color = Color.green;

        //System Runtime
        TimeSpan runtime = pathManager.runtime.Elapsed;

        Text runtimeTxt = debugStatsUI.transform.Find("System_Runtime").GetComponent<Text>();
        runtimeTxt.text = runtime.ToString();

        //System Looptime
        TimeSpan looptime = pathManager.looptime.Elapsed;

        Text looptimeTxt = debugStatsUI.transform.Find("System_Looptime").GetComponent<Text>();
        looptimeTxt.text = looptime.ToString();

        //Sadistic AI
        string s_AIStatus = sadisticAI.status;
        Text s_AI_StatusTxt = debugStatsUI.transform.Find("S_AI_StatusTxt").GetComponent<Text>();
        s_AI_StatusTxt.text = s_AIStatus;

        if (s_AIStatus.Contains("Active")) s_AI_StatusTxt.color = Color.green;
        else s_AI_StatusTxt.color = Color.red;

        //Activity
        int s_AIactivity = sadisticAI.activity;
        Text s_AI_ActivityTxt = debugStatsUI.transform.Find("S_AI_ActivityTxt").GetComponent<Text>();
        s_AI_ActivityTxt.text = s_AIactivity.ToString();

        if (s_AIactivity < 10) s_AI_ActivityTxt.color = Color.yellow;
        else if (s_AIactivity == 10) s_AI_ActivityTxt.color = Color.green;
        else s_AI_ActivityTxt.color = Color.red;

        //Cruelty
        int s_AIcruelty = sadisticAI.cruelty;
        Text s_AI_CrueltyTxt = debugStatsUI.transform.Find("S_AI_CrueltyTxt").GetComponent<Text>();
        s_AI_CrueltyTxt.text = s_AIcruelty.ToString();

        if (s_AIcruelty < 5) s_AI_CrueltyTxt.color = Color.yellow;
        else if (s_AIcruelty == 5) s_AI_CrueltyTxt.color = Color.green;
        else s_AI_CrueltyTxt.color = Color.red;

        //ThinkTime
        int s_AIthinkTime = sadisticAI.aiThinkingTime;
        Text s_AI_ThinkTimeTxt = debugStatsUI.transform.Find("S_AI_ThinkTimeTxt").GetComponent<Text>();
        s_AI_ThinkTimeTxt.text = s_AIthinkTime.ToString();

        if (s_AIthinkTime < 20) s_AI_ThinkTimeTxt.color = Color.yellow;
        else if (s_AIthinkTime == 20) s_AI_ThinkTimeTxt.color = Color.green;
        else s_AI_ThinkTimeTxt.color = Color.red;

        //Danger
        int s_AIdanger = sadisticAI.danger;
        Text s_AI_DangerTxt = debugStatsUI.transform.Find("S_AI_DangerTxt").GetComponent<Text>();
        s_AI_DangerTxt.text = s_AIdanger.ToString();

        if (s_AIdanger < 20) s_AI_DangerTxt.color = Color.green;
        else if (s_AIdanger >= 20) s_AI_DangerTxt.color = Color.yellow;
        else s_AI_DangerTxt.color = Color.red;


        //Path Manager
        string p_manStatus = pathManager.status;
        int p_manLoops = pathManager.playerLoops;

        Text p_Man_StatusTxt = debugStatsUI.transform.Find("P_Man_StatusTxt").GetComponent<Text>();
        p_Man_StatusTxt.text = p_manStatus + " (loops: " + p_manLoops + ")";

        if (p_manStatus.Contains("Active")) p_Man_StatusTxt.color = Color.green;
        else p_Man_StatusTxt.color = Color.red;

        //Prevent Progress
        bool p_manPreventProgress = pathManager.preventProgress;
        Text p_Man_PreventProgressTxt = debugStatsUI.transform.Find("P_Man_PreventProgressTxt").GetComponent<Text>();
        p_Man_PreventProgressTxt.text = p_manPreventProgress.ToString();

        if (!p_manPreventProgress) p_Man_PreventProgressTxt.color = Color.green;
        else p_Man_PreventProgressTxt.color = Color.red;

        //Progressing Path
        bool p_manProgressingPath = pathManager.progressingPath;
        Text p_Man_ProgressingPathTxt = debugStatsUI.transform.Find("P_Man_ProgressingPathTxt").GetComponent<Text>();
        p_Man_ProgressingPathTxt.text = p_manProgressingPath.ToString();

        if (!p_manProgressingPath) p_Man_ProgressingPathTxt.color = Color.green;
        else p_Man_ProgressingPathTxt.color = Color.red;

        //Player Health
        float tmp_health = health;
        Text player_Health = debugStatsUI.transform.Find("Player_Health").GetComponent<Text>();
        player_Health.text = tmp_health.ToString().Replace("f", "");

        if (tmp_health <= 20)
        {
            player_Health.color = Color.red;
        }
        else if (tmp_health <= 50)
        {
            player_Health.color = Color.yellow;
        }
        else
        {
            player_Health.color = Color.green;
        }

        //Player Invincible
        bool tmp_invincible = invincible;
        Text player_Invincible = debugStatsUI.transform.Find("Player_Invincible").GetComponent<Text>();
        player_Invincible.text = tmp_invincible.ToString();

        if (tmp_invincible)
        {
            player_Invincible.color = Color.green;
        }
        else
        {
            player_Invincible.color = Color.red;
        }
    }
    #endregion
}