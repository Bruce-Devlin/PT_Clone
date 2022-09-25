using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class MenuController : MonoBehaviour
{
    public Canvas cameraMovementCanvas;
    public Canvas mainMenuCanvas;
    public Canvas settingsCanvas;
    public Canvas creditsCanvas;

    public PlayerController playerController;

    public bool completed = false;

    private void OnEnable()
    {
        cameraMovementCanvas.enabled = false;
        mainMenuCanvas.enabled = false;
        settingsCanvas.enabled = false;
        creditsCanvas.enabled = false;

        Cursor.lockState = CursorLockMode.Confined;

        if (PlayerPrefs.GetInt("Completed") == 1) completed = true;

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

    #region Buttons
    #region Camera Selection
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
    #endregion

    #region Menu
    public void BeginBtn()
    {
        playerController.playerCamera.GetComponent<Camera>().nearClipPlane = Mathf.Lerp(0.01f, 999f, Time.deltaTime);
        SceneManager.LoadScene(1);
    }

    public void SettingBtn()
    {
        //Get-Set Settings
        float savedSensitivity = PlayerPrefs.GetFloat("Sensitivity");
        if (savedSensitivity == 0f)
        {
            savedSensitivity = 15f;
            PlayerPrefs.SetFloat("Sensitivity", 15f);
        }
        settingsCanvas.transform.Find("Sensitivity").GetComponent<Slider>().value = savedSensitivity;

        string cameraMovement = PlayerPrefs.GetString("CameraMovement");
        if (cameraMovement == "real")
        {
            settingsCanvas.transform.Find("CameraMovement").GetComponent<Dropdown>().value = 0;
        }
        else if (cameraMovement == "classic")
        {
            settingsCanvas.transform.Find("CameraMovement").GetComponent<Dropdown>().value = 1;
        }

        mainMenuCanvas.enabled = false;
        settingsCanvas.enabled = true;
    }

    public void ExitBtn()
    {
        Application.Quit();
    }

    public void BackBtn()
    {
        cameraMovementCanvas.enabled = false;
        settingsCanvas.enabled = false;
        creditsCanvas.enabled = false;
        mainMenuCanvas.enabled = true;
    }
    #endregion
    #endregion

    #region Settings


    public void OnSensitivityChange()
    {
        float sliderValue = settingsCanvas.transform.Find("Sensitivity").GetComponent<Slider>().value;

        PlayerPrefs.SetFloat("Sensitivity", sliderValue);
        settingsCanvas.transform.Find("Sensitivity").Find("Display").GetComponent<Text>().text = 
            sliderValue
            .ToString()
            .Replace("f", "")
            .Substring(
                0,
                sliderValue.ToString().IndexOf(".") + 1
                )
            .Replace(".", "");
    }

    public void OnCameraControlChange()
    {
        Dropdown dropdown = settingsCanvas.transform.Find("CameraMovement").GetComponent<Dropdown>();
        Dropdown.OptionData selectedOption = dropdown.options[dropdown.value];
        if (selectedOption.text == "REAL CAMERA CONTROLS")
        {
            PlayerPrefs.SetString("CameraMovement", "real");
        }
        else if (selectedOption.text == "CLASSIC CAMERA CONTROLS")
        {
            PlayerPrefs.SetString("CameraMovement", "classic");
        }
    }

    #endregion
}
