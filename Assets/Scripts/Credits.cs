using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.InputSystem.Users;

public class Credits : MonoBehaviour
{
    private GameObject transitionManager;
    private GameObject creditsImage;

    public float scrollYStart;
    public float scrollYEnd;
    public float scrollY;
    public float scrollPlusY;
    public float scrollSpeed;


    void Awake()
    {
        GameObject inputManager = GameObject.FindGameObjectWithTag("menuInput");
        if (inputManager != null)
            inputManager.GetComponent<MenuInputUserControls>().UpdateScenedata();
    }
    void Start()
    {
        creditsImage = GameObject.Find("CreditsImage");
        transitionManager = GameObject.Find("SceneTransition");

        scrollY = 0;
        scrollYStart = -(Screen.height * 1.35f);

        creditsImage.transform.position = new Vector3(
            Screen.width / 2,
            scrollYStart,
            0
        );
    }

    void Update()
    {
        scrollPlusY += scrollSpeed * Time.deltaTime;
        scrollY = scrollYStart + scrollPlusY;

        creditsImage.transform.position = new Vector3(
            Screen.width / 2,
            scrollY,
            0
        );

        if (scrollY >= scrollYEnd) {
            transitionManager.GetComponent<TransitionManagerMenu>().CreditsOver();
        }
    }

    public void Escape(InputAction.CallbackContext context)
    {
        if (context.performed)
        {
            transitionManager.GetComponent<TransitionManagerMenu>().CreditsOver();
        }
    }
}
