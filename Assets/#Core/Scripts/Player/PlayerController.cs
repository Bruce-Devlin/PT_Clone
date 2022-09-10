using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PlayerController : MonoBehaviour
{
    public bool debugCam = false;
    public bool printDebugToHint = false;

    [Header("Debug Settings")]
    [SerializeField]
    public float flySpeed = 0.5f;

    bool shift = false;
    bool ctrl = false;
    [SerializeField]
    float accelerationRatio = 2;
    float slowDownRatio = 0.2f;

    [Header("Player Settings")]
    [Tooltip("The Player's movement speed. (default: 5)")]
    public bool canUseFlashlight = false;
    public static float movementSpeed = 5.0f;
    float og_movementSpeed;

    public bool canLook = true;
    public float sensitivityX = 15F;
    float sensitivityY;

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
    public Text hint;

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
        int v = sadisticAI.random.Next(0, _clips.Length);
        return _clips[v];
    }

    // Start is called before the first frame update
    void Start()
    {
        Cursor.lockState = CursorLockMode.Locked;
        // Make the rigid body not change rotation
        if (GetComponent<Rigidbody>())
            GetComponent<Rigidbody>().freezeRotation = true;
        playerFlashlight.transform.Find("Bulb").GetComponent<Light>().enabled = false;
        playerFlashlight.SetActive(false);
    }

    private void FixedUpdate()
    {
        sensitivityY = sensitivityX;
        if (canLook)
        {
            rotationY += Input.GetAxis("Mouse Y") * sensitivityY;
            rotationY = Mathf.Clamp(rotationY, minimumY, maximumY);

            rotationX += Input.GetAxis("Mouse X") * sensitivityX;

            //Camera LERP
            if (playerCamera.transform.eulerAngles.x != -rotationY)
            {
                Vector3 lerpRotateY =
                    new Vector3(
                        Mathf.LerpAngle(
                            playerCamera.transform.eulerAngles.x,
                            -rotationY,
                            sensitivityY * Time.deltaTime
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
                            sensitivityX * Time.deltaTime
                            ),
                        0);
                transform.eulerAngles = lerpRotateX;
            }
        }

        if (debugCam)
        {
            //use shift to speed up flight
            if (Input.GetKeyDown(KeyCode.LeftShift) || Input.GetKeyDown(KeyCode.RightShift))
            {
                shift = true;
                flySpeed *= accelerationRatio;
            }

            if (Input.GetKeyUp(KeyCode.LeftShift) || Input.GetKeyUp(KeyCode.RightShift))
            {
                shift = false;
                flySpeed /= accelerationRatio;
            }


            //use ctrl to slow up flight
            if (Input.GetKeyDown(KeyCode.LeftControl) || Input.GetKeyDown(KeyCode.RightControl))
            {
                ctrl = true;
                flySpeed *= slowDownRatio;
            }

            if (Input.GetKeyUp(KeyCode.LeftControl) || Input.GetKeyUp(KeyCode.RightControl))
            {
                ctrl = false;
                flySpeed /= slowDownRatio;
            }
            //
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
        else
        {
            if (Input.GetKey(KeyCode.W))
            {
                transform.Translate(Vector3.forward * Time.deltaTime * movementSpeed);
            }

            if (Input.GetKey(KeyCode.S))
            {
                transform.Translate(-1 * Vector3.forward * Time.deltaTime * movementSpeed);
            }

            if (Input.GetKey(KeyCode.A))
            {
                transform.Translate(Vector3.left * Time.deltaTime * (movementSpeed / 3));
            }

            if (Input.GetKey(KeyCode.D))
            {
                transform.Translate(Vector3.right * Time.deltaTime * (movementSpeed / 3));
            }
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

    // Update is called once per frame
    void Update()
    {
        if (playerFlashlight.transform.Find("Bulb").GetComponent<Light>().enabled)
        {
            Vector3 currentAngle = new Vector3(
             Mathf.LerpAngle(playerFlashlight.transform.eulerAngles.x, playerCamera.transform.eulerAngles.x, Time.deltaTime * sensitivityX / 2),
             Mathf.LerpAngle(playerFlashlight.transform.eulerAngles.y, playerCamera.transform.eulerAngles.y, Time.deltaTime * sensitivityX / 2),
             Mathf.LerpAngle(playerFlashlight.transform.eulerAngles.z, playerCamera.transform.eulerAngles.z, Time.deltaTime * sensitivityX / 2));

            playerFlashlight.transform.eulerAngles = currentAngle;
        }


        if (Input.GetKeyDown(KeyCode.F1))
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
        if (Input.GetKeyDown(KeyCode.F2))
        {
            
            if (sadisticAI.cruelty != 100)
            {
                StartCoroutine(ShowHint("Cruelty: 100"));
                sadisticAI.cruelty = 100;
            }
            else
            {
                StartCoroutine(ShowHint("Cruelty: 10"));
                sadisticAI.cruelty = 10;
            }

        }
        if (Input.GetKeyDown(KeyCode.F3))
        {
            if (printDebugToHint) printDebugToHint = false;
            else printDebugToHint = true;
        }

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
    }

    public IEnumerator ShowHint(string hintTxt = "", bool waitAndClear = true)
    {
        hint.text = hintTxt.ToUpper();
        if (waitAndClear)
        {
            yield return new WaitForSeconds(5);
            hint.text = "";
        }
    }
}
