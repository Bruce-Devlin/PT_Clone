using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Telephone : MonoBehaviour
{
    public PathManager pathManager;
    public bool shouldRing = false;
    private bool ringing = false;
    private bool playingCall = false;
    [HideInInspector] public AudioClip callSound = null;
    public AudioClip phoneSound;
    public AudioClip ringingSound;
    public PlayerController playerController;

    Image interact;
    private bool canInteract = false;

    public AudioClip pizzaTime;

    string number = "";

    // Start is called before the first frame update
    void Start()
    {
        interact = pathManager.playerController.userInterface.transform.Find("Interact").GetComponent<Image>();
    }

    // Update is called once per frame
    void Update()
    {
        if (number.Length > 3) number = "";
        else if (number != "")
        {
            if (number == "123")
            {
                callSound = pizzaTime;
                shouldRing = true;
            }
        }

        if (shouldRing && !ringing)
        {
            shouldRing = false;
            pathManager.preventProgress = true;
            ringing = true;
            GetComponent<AudioSource>().clip = ringingSound; 
            GetComponent<AudioSource>().Play();
            GetComponent<AudioSource>().loop = true;
            canInteract = true;
        }
    }

    private void OnMouseUp()
    {
        canInteract = false;
        if (ringing && !playingCall)
        { 
            ringing = false;
            GetComponent<AudioSource>().Stop();
            GetComponent<AudioSource>().loop = false;
            StartCoroutine(PlayCall());
        }
    }

    private void OnMouseEnter()
    {
        if (canInteract)
        {
            interact.enabled = true;
            playerController.ShowHint("(Left-click to interact)");
        }


        //Numbers
        if (Input.GetKeyDown(KeyCode.Alpha1))
        {
            Debug.Log("1 Key down");
            number = number + "1";
            Debug.Log(number);
        }
        if (Input.GetKeyDown(KeyCode.Alpha2)) number += "2";
        if (Input.GetKeyDown(KeyCode.Alpha3)) number += "3";
        if (Input.GetKeyDown(KeyCode.Alpha4)) number += "4";
        if (Input.GetKeyDown(KeyCode.Alpha5)) number += "5";
        if (Input.GetKeyDown(KeyCode.Alpha6)) number += "6";
        if (Input.GetKeyDown(KeyCode.Alpha7)) number += "7";
        if (Input.GetKeyDown(KeyCode.Alpha8)) number += "8";
        if (Input.GetKeyDown(KeyCode.Alpha9)) number += "9";
    }
    private void OnMouseExit()
    {
        if (interact.enabled) interact.enabled = false;
        number = "";
    }

    IEnumerator PlayCall()
    {
        playingCall = true;
        yield return new WaitForSeconds(phoneSound.length);
        GetComponent<AudioSource>().Stop();
        GetComponent<AudioSource>().clip = callSound;
        GetComponent<AudioSource>().Play();
        yield return new WaitForSeconds(callSound.length);
        GetComponent<AudioSource>().Stop();
        GetComponent<AudioSource>().clip = phoneSound;
        GetComponent<AudioSource>().Play();
        yield return new WaitForSeconds(phoneSound.length);
        GetComponent<AudioSource>().Stop();
        pathManager.preventProgress = false;
        playingCall = false;
        callSound = null;
    }
}
