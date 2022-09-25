using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CeilingLights : MonoBehaviour
{
    public bool burst = false;
    public bool shouldBurst = false;
    private bool bursting = false;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if (shouldBurst && !bursting && !burst)
        {
            bursting = true;
            StartCoroutine(BurstBulbs());
        }
    }

    IEnumerator BurstBulbs()
    {
        GetComponent<AudioSource>().Play();
        transform.Find("Point Light").GetComponent<Light>().enabled = false;
        yield return new WaitForSeconds(GetComponent<AudioSource>().clip.length);
        bursting = false;
        burst = true;
    }
}
