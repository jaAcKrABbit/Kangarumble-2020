using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;

public class MenuInputUserControls : MonoBehaviour
{
    [SerializeField] private Scene currentScene;
    private GameObject gameTitle;
    private OpeningMenu openingMenu;
    private PlayerSelect playerSelect;
    private GameObject firstMenuObj;
    private GameObject creditsCamera;

    void Awake()
    {
        DontDestroyOnLoad(this.gameObject);
    }
    // Start is called before the first frame update
    void Start()
    {
        gameTitle = GameObject.Find("GameTitle");
        openingMenu = gameTitle.GetComponent<OpeningMenu>();
        playerSelect = gameTitle.GetComponent<PlayerSelect>();
        firstMenuObj = GameObject.Find("FirstMenu");
    }

    // Update is called once per frame
    void Update()
    {
    }

    public void Escape(InputAction.CallbackContext context)
    {
            openingMenu.Escape(context);
    }

    public void Starting(InputAction.CallbackContext context)
    {
        // take player to level
        openingMenu.Starting(context);
    }

    public void SelectDir(InputAction.CallbackContext ctx)
    {
        //Debug.Log("MenuInputUserControls SelectDir()");
        firstMenuObj.GetComponent<FirstMenu>().updateDir(ctx.ReadValue<Vector2>());
    }

    public void JoinPlayer(InputAction.CallbackContext context)
    {
        playerSelect.JoinPlayer(context);
    }

    public void Continue(InputAction.CallbackContext context)
    {
        firstMenuObj.GetComponent<FirstMenu>().Continue();
    }

    //Credits escape
    public void CreditsEscape(InputAction.CallbackContext context)
    {
        creditsCamera.GetComponent<Credits>().Escape(context);
    }


    public void UpdateScenedata()
    {
        currentScene = SceneManager.GetActiveScene();
        if (currentScene.name == "OpeningMenu")
        {
            gameObject.GetComponent<PlayerInput>().SwitchCurrentActionMap("Inputs");

            gameTitle = GameObject.Find("GameTitle");
            openingMenu = gameTitle.GetComponent<OpeningMenu>();
            playerSelect = gameTitle.GetComponent<PlayerSelect>();
            firstMenuObj = GameObject.Find("FirstMenu");
            creditsCamera = null;
        }
        else if (currentScene.name == "Credits")
        {
            gameObject.GetComponent<PlayerInput>().SwitchCurrentActionMap("Credits");
            creditsCamera = GameObject.Find("Main Camera");
            gameTitle = null;
            openingMenu = null;
            playerSelect = null;
            firstMenuObj = null;
        }
        else
        {
            gameTitle = null;
            openingMenu = null;
            playerSelect = null;
            firstMenuObj = null;
            creditsCamera = null;
            gameObject.GetComponent<PlayerInput>().SwitchCurrentActionMap("Disabled");
        }
    }
}
