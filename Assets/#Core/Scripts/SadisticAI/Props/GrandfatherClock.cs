using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GrandfatherClock : MonoBehaviour
{
    public AudioClip tickingClip;
    public AudioClip chimeClip;

    public bool shouldTick = true;
    bool ticking = false;

    public bool shouldChime = false;
    bool chiming = false;

    AudioSource source;

    private void OnEnable()
    {
        source = GetComponent<AudioSource>();
    }

    // Start is called before the first frame update
    void Start()
    {
        source.clip = tickingClip;
    }

    // Update is called once per frame
    void Update()
    {
        if (shouldTick && !ticking) StartCoroutine(Tick());
    }

    IEnumerator Tick()
    {
        ticking = true;
        source.clip = tickingClip;
        source.Play();
        yield return new WaitForSeconds(tickingClip.length);
        ticking = false;    
    }
}
