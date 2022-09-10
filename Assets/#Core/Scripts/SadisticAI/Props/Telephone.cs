using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Telephone : MonoBehaviour
{
    [HideInInspector] public PathManager pathManager;
    public bool shouldRing = false;
    private bool ringing = false;
    private bool playingCall = false;
    [HideInInspector] public AudioClip callSound;
    public AudioClip phoneSound;
    private AudioClip ringingSound;

    public Image interact;
    private bool canInteract = false;

    // Start is called before the first frame update
    void Start()
    {
        ringingSound = GetComponent<AudioSource>().clip;
    }

    // Update is called once per frame
    void Update()
    {
        if (shouldRing && !ringing)
        {
            ringing = true;
            GetComponent<AudioSource>().clip = ringingSound; 
            GetComponent<AudioSource>().Play();
            GetComponent<AudioSource>().loop = true;
            canInteract = true;
        }
    }
    private bool clicked;

    private void OnMouseUp()
    {
        if (ringing && !playingCall)
        {
            canInteract = false;
            ringing = false;
            GetComponent<AudioSource>().Stop();
            GetComponent<AudioSource>().loop = false;
            StartCoroutine(PlayCall());
        }
    }

    private void OnMouseEnter()
    {
        if (canInteract) interact.enabled = true;
    }
    private void OnMouseExit()
    {
        if (interact.enabled) interact.enabled = false;
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
    }
}
