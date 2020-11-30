using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.InputSystem.Users;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class OpeningMenu : MonoBehaviour
{
    public int menuState = 0;
    // 0 --> press START
    // 1 --> first menu
    // 2 --> JOIN IN!
    // 3 --> level select
    // 4 --> level settings

    public bool hideTitle;
    public float titleYHidden = 8;
    public float titleYShow = 1.47f;
    private float titleY;

    public float pressStartZ;

    public float titleScalePlus;
    public float titleScale;
    public float currentTime;

    private GameObject pressStartText;
    private float pressStartYRatioShow;
    private float pressStartYRatioHidden;
    private float pressStartTextYShow;
    private float pressStartTextYHidden;

    private GameObject firstMenuObj;
    private GameObject levelSelectObj;

    private GameObject groundObj;
    public float groundYRatio;

    public Vector2 selectDir;

    InputUser generalUser;

    void Start()
    {
        titleScale = 0;
        currentTime = 0;
        pressStartText = GameObject.Find("PressStartText");
        levelSelectObj = GameObject.Find("LevelSelect");
        firstMenuObj = GameObject.Find("FirstMenu");
        groundObj = GameObject.Find("Ground");

        pressStartYRatioShow = 0.09f;
        pressStartYRatioHidden = -0.3f;

        Cursor.visible = false;
        generalUser = InputUser.all[0];
        int i = 0;
        while (i< Gamepad.all.Count)
            InputUser.PerformPairingWithDevice(Gamepad.all[i++], generalUser);
    }

    void Update()
    {
        // control title Y position
        titleY = Mathf.Lerp(titleY, (hideTitle) ? titleYHidden : titleYShow, 0.1f);
        transform.position = new Vector3(transform.position.x, titleY, transform.position.z);
        hideTitle = (menuState != 0);

        // animate title size
        currentTime = Time.time;
        titleScale = (Mathf.Sin(currentTime * 4) / 12) + titleScalePlus;
        transform.localScale = new Vector3(titleScale, titleScale, titleScale);

        // hide press start text if necessary
        pressStartTextYShow = Screen.height * pressStartYRatioShow;
        pressStartTextYHidden = Screen.height * pressStartYRatioHidden;
        float pressStartTextY = pressStartText.GetComponent<Transform>().position.y;
        float pressStartTextYDest = (menuState == 0) ? pressStartTextYShow : pressStartTextYHidden;
        if (menuState == 0) {
            pressStartText.GetComponent<Transform>().eulerAngles = new Vector3(0, 0, Mathf.Sin(Time.time * 3) * 2f);
        }
        pressStartText.GetComponent<Transform>().position = new Vector3(Screen.width / 2, Mathf.Lerp(pressStartTextY, pressStartTextYDest, 0.15f), pressStartZ);

        // set position of ground
        /*
        groundObj.transform.position = new Vector3(
            Screen.width / 2,
            Screen.height * groundYRatio,
            groundObj.transform.position.z
        );
        */
    }

    public void Escape(InputAction.CallbackContext context)
    {
        if (context.performed)
        {
            if (menuState > 0) {
                menuState--;
                if (menuState == 3) {
                    levelSelectObj.GetComponent<LevelSelect>().inputTimer = true;
                }
            }
            //Temp disable Control selection state
            if (menuState == 0)
            {
                Debug.Log("Quitting game...");
                Application.Quit();
            }
        }

    }

    public void Starting(InputAction.CallbackContext context)
    {
        // take player to level
        if (context.performed)
        {
            if (menuState == 0)
            {
                Debug.Log("Going to FirstMenu");
                menuState = 1;
                firstMenuObj.GetComponent<FirstMenu>().inputTimer = false;
                firstMenuObj.GetComponent<FirstMenu>().InvokeResetInputTimer();
            }
        }
    }
}
