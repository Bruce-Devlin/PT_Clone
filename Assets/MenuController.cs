using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class MenuController : MonoBehaviour
{
    public Canvas cameraMovementCanvas;
    public Canvas mainMenuCanvas;
    public Canvas settingsCanvas;
    public Canvas creditsCanvas;

    private void OnEnable()
    {
        cameraMovementCanvas.enabled = false;
        mainMenuCanvas.enabled = false;
        settingsCanvas.enabled = false;
        creditsCanvas.enabled = false;
    }

    // Start is called before the first frame update
    void Start()
    {
        if (PlayerPrefs.GetString("CameraMovement") == "")
        {
            Debug.Log("NO CAMERA MOVEMENT SET");
            cameraMovementCanvas.enabled = true;
        }
        else
        {
            mainMenuCanvas.enabled = true;
        }
    }

    // Update is called once per frame
    void Update()
    {
        
    }


    //Buttons
    public void RealCameraMovement()
    {
        PlayerPrefs.SetString("CameraMovement", "real");
        cameraMovementCanvas.enabled = false;
        mainMenuCanvas.enabled = true;
    }

    public void ClassicCameraMovement()
    {
        PlayerPrefs.SetString("CameraMovement", "classic");
        cameraMovementCanvas.enabled = false;
        mainMenuCanvas.enabled = true;
    }

    public void StartGame()
    {
        SceneManager.LoadScene(1);
    }
}
