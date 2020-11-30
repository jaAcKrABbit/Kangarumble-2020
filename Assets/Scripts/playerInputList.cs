using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;

public class playerInputList : MonoBehaviour
{
    [SerializeField] private Scene currentScene;

    //Needed objects to handle menu inputs
    private GameObject gameTitle;
    private OpeningMenu openingMenu;
    private PlayerSelect playerSelect;
    private GameObject levelSelectObj;
    private GameObject levelSettingsObj;
    private LevelSelect levelSelect;
    private GameObject firstMenuObj;
    private LevelSettingsNew levelSettingsNew;
    private FirstMenu firstMenu;
    private GameObject player;
    private Player playerScript;
    private EndScene endScript;
    private GameObject menuController;
    private Menus pauseMenuScript;

    //Needed objects to handle game inputs

    void Awake()
    {
        DontDestroyOnLoad(this.gameObject);
    }

    void Start()
    {
        //Retrieve current scene
        currentScene = SceneManager.GetActiveScene();
        //Import necessary objects as reference for menu
        if(currentScene.name == "OpeningMenu")
        {
            gameTitle = GameObject.Find("GameTitle");
            openingMenu = gameTitle.GetComponent<OpeningMenu>();
            playerSelect = gameTitle.GetComponent<PlayerSelect>();
            levelSelectObj = GameObject.Find("LevelSelect");
            levelSelect = levelSelectObj.GetComponent<LevelSelect>();
            levelSettingsObj = GameObject.Find("LevelSettingsUI");
            levelSettingsNew = levelSettingsObj.GetComponent<LevelSettingsNew>();
            firstMenuObj = GameObject.Find("FirstMenu");
            firstMenu = firstMenuObj.GetComponent<FirstMenu>();
        }
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    //Menu Actions
    public void LockIn(InputAction.CallbackContext ctx)
    {
        if (ctx.performed)
        {
            playerSelect.LockIn(gameObject);
        }
    }

    public void Continue(InputAction.CallbackContext ctx)
    {
        if (ctx.performed)
        {
            playerSelect.Continue(gameObject);
            levelSelect.Continue();
            levelSettingsNew.Continue();
            firstMenu.Continue();
        }
    }

    public void Drop_Return(InputAction.CallbackContext ctx)
    {
        if (ctx.performed)
        {
            playerSelect.Drop_Return(gameObject);
        }
    }

    public void SelectDir(InputAction.CallbackContext ctx)
    {
        //if (ctx.performed)
        //{
            playerSelect.updateDir(gameObject, ctx.ReadValue<Vector2>().x);
            levelSelect.updateDir(ctx.ReadValue<Vector2>().x);
            levelSettingsNew.updateDir(ctx.ReadValue<Vector2>());
            firstMenu.updateDir(ctx.ReadValue<Vector2>());
        //}
    }
    public void Back(InputAction.CallbackContext ctx)
    {
        if (ctx.performed)
        {
            levelSelect.Back();
            levelSettingsNew.Back();
        }
    }
   
    //Game Actions
    public void JumpDown(InputAction.CallbackContext ctx)
    {
        if (ctx.performed)
        {
            playerScript.JumpKeyDown();
        }
    }

    public void JumpUp(InputAction.CallbackContext ctx)
    {
        if (ctx.performed)
        {
            playerScript.JumpKeyUp();
        }
    }

    public void StompDown(InputAction.CallbackContext ctx)
    {
        if (ctx.performed)
        {
            playerScript.StompKeyDown();
        }
    }
    public void Move(InputAction.CallbackContext ctx)
    {
        if (ctx.performed)
        {
            playerScript.Move(ctx.ReadValue<Vector2>());
        }
    }

    //End Actions

    public void Vote(InputAction.CallbackContext ctx)
    {
        if (ctx.performed)
        {
            endScript.Vote(gameObject);
        }
    }

    //Pause Menu Actions
    public void Pause(InputAction.CallbackContext ctx)
    {
        if (ctx.performed)
        {
            pauseMenuScript.Pause();
        }
    }

    public void PauseBack(InputAction.CallbackContext ctx)
    {
        if (ctx.performed)
        {
            pauseMenuScript.Back();
        }
    }

    public void PauseupdateDir(InputAction.CallbackContext ctx)
    {
        //if (ctx.performed)
        //{
            pauseMenuScript.updateDir(ctx.ReadValue<Vector2>());
        //}
    }

    public void PauseContinue(InputAction.CallbackContext ctx)
    {
        if (ctx.performed)
        {
            pauseMenuScript.Continue();
        }
    }


    public void UpdateScenedata()
    {
        currentScene = SceneManager.GetActiveScene();
        if (currentScene.name == "OpeningMenu")
        {
            gameObject.GetComponent<PlayerInput>().SwitchCurrentActionMap("JoiningMenu");
            gameTitle = GameObject.Find("GameTitle");
            openingMenu = gameTitle.GetComponent<OpeningMenu>();
            playerSelect = gameTitle.GetComponent<PlayerSelect>();
            levelSelectObj = GameObject.Find("LevelSelect");
            levelSelect = levelSelectObj.GetComponent<LevelSelect>();
            levelSettingsObj = GameObject.Find("LevelSettingsUI");
            levelSettingsNew = levelSettingsObj.GetComponent<LevelSettingsNew>();
            firstMenuObj = GameObject.Find("FirstMenu");
            firstMenu = firstMenuObj.GetComponent<FirstMenu>();
            player = null;
            playerScript = null;
        }
        else if (currentScene.name == "Loading")
        {
            gameObject.GetComponent<PlayerInput>().SwitchCurrentActionMap("Disabled");
        }
        else
        {
            gameTitle = null;
            openingMenu = null;
            playerSelect = null;
            levelSelectObj = null;
            levelSelect = null;
            levelSettingsNew = null;
            firstMenuObj = null;
            firstMenu = null;

            if (currentScene.name == "EndScene")
            {
                gameObject.GetComponent<PlayerInput>().SwitchCurrentActionMap("End");
                endScript = GameObject.Find("Main Camera").GetComponent<EndScene>();
            }
            else
            {
                gameObject.GetComponent<PlayerInput>().SwitchCurrentActionMap("Game");
                switch (gameObject.GetComponent<PlayerInput>().playerIndex)
                {
                    case 0:
                        player = GameObject.Find("Player");
                        break;
                    case 1:
                        player = GameObject.Find("PlayerII");
                        break;
                    case 2:
                        player = GameObject.Find("PlayerIII");
                        break;
                    case 3:
                        player = GameObject.Find("PlayerIV");
                        break;
                    default:
                        Debug.LogError("PlayerInputList found a bad index entering game scene");
                        break;
                }
                playerScript = player.GetComponent<Player>();
                playerScript.RetrieveObject(gameObject);
                menuController = GameObject.Find("MenuController");
                pauseMenuScript = menuController.GetComponent<Menus>();
            }
        }
    }

    //Vibration
    IEnumerator StopVibration(Gamepad gamepad, float delayTime)
    {
        yield return new WaitForSecondsRealtime(delayTime);
        gamepad.SetMotorSpeeds(0f, 0f);
    }
    public void Vibration()
    {
        if (gameObject.GetComponent<PlayerInput>().currentControlScheme == "Gamepads")
        {
            Gamepad gamepad = (Gamepad)gameObject.GetComponent<PlayerInput>().devices[0];
            gamepad.SetMotorSpeeds(0.5f, 0.5f);
            StartCoroutine(StopVibration(gamepad, 0.3f));
        }
    }
}
