using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CeilingLights : MonoBehaviour
{
    public bool shouldBurst = false;
    private bool bursting = false;
    public bool burst = false;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if (shouldBurst && !bursting)
        {
            bursting = true;
            StartCoroutine(BurstBulbs());
        }
    }

    IEnumerator BurstBulbs()
    {
        transform.Find("Point Light").GetComponent<Light>().enabled = false;
        GetComponent<AudioSource>().Play();
        yield return new WaitForSeconds(GetComponent<AudioSource>().clip.length);
        burst = true;
    }
}
