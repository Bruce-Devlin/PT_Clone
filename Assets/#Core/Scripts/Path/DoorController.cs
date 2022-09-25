using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class DoorController : MonoBehaviour
{
    public PathManager pathManager;
    public PlayerController playerController;
    public GameObject door;

    public bool forceOpen = false;
    public bool forceClose = false;
    public bool locked = true;

    [HideInInspector]public bool open = false;

    Image interact;

    // Start is called before the first frame update
    void Start()
    {
        interact = pathManager.playerController.userInterface.transform.Find("Interact").GetComponent<Image>();
    }

    // Update is called once per frame
    void Update()
    {
        if (forceOpen && !open)
        {
            open = true;
            locked = false;

            door.SetActive(false);
            forceOpen = false;

        }

        if (open && (forceClose || locked))
        {
            open = false;
            locked = true;

            door.SetActive(true);
            forceClose = false;
        }
        else forceClose = false;
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
        open = true;
        door.SetActive(false);
    }
}
