using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Chandiler : MonoBehaviour
{
    public List<Light> Bulbs;
    [HideInInspector]public bool burst = false;
    public bool shouldSwing = false;
    public bool shouldBurst = false;
    bool swinging = false;
    

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if (shouldSwing && !swinging)
        {
            swinging = true;
            shouldSwing = false;
            StartCoroutine(Swing());
        }
        if (!burst && shouldBurst)
        {
            StartCoroutine(BurstBulbs());
        }
    }

    IEnumerator Swing()
    {
        while (swinging)
        {
            GetComponent<HingeJoint>().useMotor = true;
            GetComponent<AudioSource>().Play();
            yield return new WaitForSeconds(1);
            GetComponent<HingeJoint>().useMotor = false;
            yield return new WaitForSeconds(18);
            swinging = false;
        }
    }
        
    IEnumerator BurstBulbs()
    {
        GetComponent<AudioSource>().Play();
        foreach (Light light in Bulbs)
        {
            light.enabled = false;
        }
        yield return new WaitForSeconds(GetComponent<AudioSource>().clip.length);
        burst = true;
    }
}
