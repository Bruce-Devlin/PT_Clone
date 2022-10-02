using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class ExtraRoom_01 : MonoBehaviour
{
    public ExtraRoom root;
    public GameObject floor;
    public List<GameObject> lights;

    // Start is called before the first frame update
    void Start()
    {
        transform.Find("trigger_Enter").GetComponent<PathTrigger>().TriggerEvent.AddListener(Trigger_OnEnter);
        transform.Find("trigger_Centre").GetComponent<PathTrigger>().TriggerEvent.AddListener(Trigger_OnCentre);

    }

    // Update is called once per frame
    void Update()
    {
        
    }

    void Trigger_OnEnter(Collider collider)
    {
        if (collider != null && collider.gameObject.tag == "Player")
        {
            Debug.Log("ExtraRoom: Playered entered room");
            root.door.GetComponent<DoorController>().forceClose = true;
        }
    }

    void Trigger_OnCentre(Collider collider)
    {
        if (collider != null && collider.gameObject.tag == "Player")
        {
            StartCoroutine(ExitRoom());
            
        }
    }

    IEnumerator ExitRoom()
    {
        Vector3 newPos = transform.Find("trigger_Centre").position;
        newPos.y = newPos.y - 50f;

        root.sadisticAI.interact_CeilingLights.Find(item => item.gameObject.name == "Chandiler").transform.position = newPos;

        yield return new WaitForSeconds(2);
        floor.gameObject.SetActive(false);
        root.sadisticAI.playerController.canMove = false;
        yield return new WaitForSeconds(5);
        root.sadisticAI.playerController.canMove = true;
    }

}
