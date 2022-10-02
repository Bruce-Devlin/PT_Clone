using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class DoorController : MonoBehaviour
{
    public PathManager pathManager;
    public PlayerController playerController;
    public GameObject door;
    public bool debug = false;

    public float doorSpeed = 5f;
    public bool forceOpen = false;
    public bool forceClose = false;
    public bool locked = true;
    private bool unlocked = false;
    private bool wasLocked = false;

    private AudioSource audio;
    private bool playing = false;
    public AudioClip closingAudio;
    public AudioClip openingAudio;
    public AudioClip unlockingAudio;


    [HideInInspector]public bool open = false;
    private bool moving = false;
    private Vector3 targetAngle = new Vector3();

    Image interact;

    // Start is called before the first frame update
    void Start()
    {
        interact = pathManager.playerController.userInterface.transform.Find("Interact").GetComponent<Image>();
        audio = door.GetComponent<AudioSource>();
    }

    void RotateDoor(float angle, bool opening = false, bool closing = false, bool unlocking = false)
    {
        if (!playing)
        {
            if (opening)
            {
                audio.clip = openingAudio;
                audio.Play();
            }
            else if (closing)
            {
                audio.clip = closingAudio;
                audio.Play();
            }
            else if (unlocking)
            {
                audio.clip = unlockingAudio;
                audio.Play();
            }
            playing = true;
        }


        moving = true;
        targetAngle = door.transform.localEulerAngles;

        if (debug) playerController.Log("Rotating from: " + targetAngle.y.ToString() + " to: " + angle);

        targetAngle.y = Mathf.LerpAngle(door.transform.localEulerAngles.y, angle, doorSpeed * Time.deltaTime);
        door.transform.localEulerAngles = targetAngle;
        if (debug) playerController.Log("Angle: " + door.transform.localEulerAngles.y.ToString());

        if (angle == 1) angle = 0;

        if (targetAngle.y.ToString().StartsWith(angle.ToString()))
        {
            if (opening)
            {
                open = true;
                forceOpen = false;
                unlocked = false;
            }
            else if (closing)
            {
                open = false;
                locked = true;
                forceClose = false;
                unlocked = false;
            }
            else if (unlocking)
            {
                locked = false;
                unlocked = true;
            }
            if (debug) playerController.Log("Done moving");
            moving = false;
            audio.Stop();
            playing = false;
        }
    }

    // Update is called once per frame
    void Update()
    {
        if (forceOpen && !open)
        {
            if (debug) playerController.Log("Opening door...");

            GetComponent<BoxCollider>().enabled = false;
            RotateDoor(59f, opening: true);
        }

        if (forceClose && open)
        {
            if (debug) playerController.Log("Closing door...");

            GetComponent<BoxCollider>().enabled = true;
            RotateDoor(179f, closing: true);
        }

        if (!locked && !open && !unlocked &&(wasLocked || moving))
        {
            if (debug) playerController.Log("Unlocking door...");
            RotateDoor(160f, unlocking: true);
        }

        if (!locked && !open && !moving && !unlocked) wasLocked = true;
        else wasLocked = false;
    }

    private void OnMouseEnter()
    {
        if (!locked && !open)
        {
            interact.enabled = true;
            playerController.ShowHint("(Left-click to open)");
        }
    }

    private void OnMouseExit()
    {
        interact.enabled = false;
    }

    private void OnMouseUp()
    {
        if (!moving && !locked)
        {
            forceOpen = true;
        }
    }
}
