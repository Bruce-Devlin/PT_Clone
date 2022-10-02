using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GamePath : MonoBehaviour
{
    public bool active = false;
    private bool hidden;
    public PathManager pathManager;
    public SadisticAI sadisticAI;

    private void HideGameObjects(bool shouldHide)
    {
        bool hide;
        if (shouldHide) hide = false;
        else hide = true;

        foreach (Transform child in transform)
        {
            if (child.gameObject.name != "trigger_Exit" && child.gameObject.name != "AI_Interactables" && child.gameObject.name != "Lights")
            {
                child.gameObject.SetActive(hide);

            }
            else if (child.gameObject.name == "AI_Interactables")
            {
                foreach (Transform interactable in child)
                {
                    if (interactable.gameObject.name != "Enter_Door")
                    {
                        interactable.gameObject.SetActive(hide);
                    }
                }
            }
            else if (child.gameObject.name == "Lights")
            {
                foreach (Transform light in child)
                {
                    if (light.gameObject.name.StartsWith("Ceiling Light"))
                    {
                        if (!light.GetComponent<CeilingLights>().burst)
                        {
                            light.gameObject.SetActive(hide);
                        }
                    }
                    else
                    {
                        if (!light.GetComponent<Chandiler>().burst)
                        {
                            light.gameObject.SetActive(hide);
                        }
                    }

                    
                }
            }
            hidden = shouldHide;
        }
    }

    private void Start()
    {
        if (!active)
        {
            HideGameObjects(true);
        }
        else
        {
            HideGameObjects(false);
        }
    }

    // Update is called once per frame
    void Update()
    {
        if (active && hidden)
        {
            HideGameObjects(false);
        }
        else if (!active && !hidden)
        {
            HideGameObjects(true);
        }
    }
}
