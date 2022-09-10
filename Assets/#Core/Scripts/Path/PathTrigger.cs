using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class PathTrigger : MonoBehaviour
{
    public UnityEvent<Collider> TriggerEvent;

    private void OnTriggerEnter(Collider other)
    {
        TriggerEvent.Invoke(other);
    }

    private void OnTriggerExit(Collider other)
    {
        TriggerEvent.Invoke(null);
    }
}
