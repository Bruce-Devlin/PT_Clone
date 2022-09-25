using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HeadBobber : MonoBehaviour
{
    public PlayerController player;

    public Vector3 restPosition; //local position where your camera would rest when it's not bobbing.
    public float transitionSpeed = 20f; //smooths out the transition from moving to not moving.
    public float bobSpeed = 2.8f; //how quickly the player's head bobs.
    float tmp_BobSpeed; //how quickly the player's head bobs.
    public float bobAmount = 0.05f; //how dramatic the bob is. Increasing this in conjunction with bobSpeed gives a nice effect for sprinting.

    float timer = Mathf.PI / 2; //initialized as this value because this is where sin = 1. So, this will make the camera always start at the crest of the sin wave, simulating someone picking up their foot and starting to walk--you experience a bob upwards when you start walking as your foot pushes off the ground, the left and right bobs come as you walk.
    Vector3 camPos;

    void Awake()
    {
        camPos = transform.localPosition;
        tmp_BobSpeed = bobSpeed;
    }

    void Update()
    {
        if (player.sprinting) bobSpeed = tmp_BobSpeed * 2;
        else bobSpeed = tmp_BobSpeed;
        if (Input.GetAxisRaw("Horizontal") != 0 || Input.GetAxisRaw("Vertical") != 0) //moving
        {
            timer += bobSpeed * Time.deltaTime;

            //use the timer value to set the position
            Vector3 newPosition = new Vector3(Mathf.Cos(timer) * bobAmount, restPosition.y + Mathf.Abs((Mathf.Sin(timer) * bobAmount)), restPosition.z); //abs val of y for a parabolic path
            camPos = newPosition;
        }
        else
        {
            timer = Mathf.PI / 2; //reinitialize

            Vector3 newPosition = new Vector3(Mathf.Lerp(camPos.x, restPosition.x, transitionSpeed * Time.deltaTime), Mathf.Lerp(camPos.y, restPosition.y, transitionSpeed * Time.deltaTime), Mathf.Lerp(camPos.z, restPosition.z, transitionSpeed * Time.deltaTime)); //transition smoothly from walking to stopping.
            camPos = newPosition;
        }

        if (timer > Mathf.PI * 2) //completed a full cycle on the unit circle. Reset to 0 to avoid bloated values.
            timer = 0;

        transform.localPosition = camPos;
    }
}
