using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using UnityEngine;

public class CeilingLights : MonoBehaviour
{
    public bool burst = false;
    public bool shouldBurst = false;
    private bool bursting = false;

    public int maxFlickers = 15;
    public bool shouldFlicker = false;
    private bool flickering = false;

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

        if (shouldFlicker && !flickering && !burst)
        {
            flickering = true;
            StartCoroutine(Flicker());
        }
    }

    IEnumerator BurstBulbs()
    {
        GetComponent<AudioSource>().Play();
        transform.Find("Point Light").GetComponent<Light>().enabled = false;
        yield return new WaitForSeconds(GetComponent<AudioSource>().clip.length);
        bursting = false;
        burst = true;
        shouldBurst = false;
    }

    IEnumerator Flicker()
    {
        //GetComponent<AudioSource>().Play();
        int flickers = SadisticAI.RollDice("flickers", 1, maxFlickers);

        foreach (int i in Enumerable.Range(1, flickers))
        {
            transform.Find("Point Light").GetComponent<Light>().enabled = true;
            Debug.Log("Flicker");
            yield return new WaitForSeconds(1);
            transform.Find("Point Light").GetComponent<Light>().enabled = false;
        }
        transform.Find("Point Light").GetComponent<Light>().enabled = true;
        flickering = false;
        shouldFlicker = false;
    }


}
