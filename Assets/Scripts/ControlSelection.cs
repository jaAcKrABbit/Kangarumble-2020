using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ControlSelection : MonoBehaviour
{
    [SerializeField]
    private LevelSelect levelSelect;
    [SerializeField]
    private PlayerSelect playerSelect;
    private bool inputAccept = false;
    public float pressChoicesTimer = 0;

    [SerializeField]
    private GameObject controlSprite;
    public float spriteYShow = 0;
    public float spriteYHidden = 0;
    private float spriteY = 0;
    private float yDest = 0;

    public KeyCode advanceKeyCode;
    public KeyCode advanceKeyCodeController;

    void Start()
    {
        spriteY = spriteYHidden;
    }
    void Update()
    {
        yDest = spriteYHidden;
        spriteYShow = Screen.height * 0.6f;

        if (gameObject.GetComponent<OpeningMenu>().menuState == 0)
        {
            inputAccept = false;
        }
        if (gameObject.GetComponent<OpeningMenu>().menuState == 1)
        {
            //TEMP DISABLE CONTROL SELECT
        
        //    yDest = spriteYShow;
        //    if (pressChoicesTimer < 30)
        //    {
        //        pressChoicesTimer++;
        //    }
        //    if (pressChoicesTimer == 30)
        //    {
        //        inputAccept = true;
        //    }


        //    if (inputAccept && Input.GetKeyDown(advanceKeyCode))
        //    {
        //        levelSelect.controllerInput = false;
        //        playerSelect.controllerInput = false;
        //        gameObject.GetComponent<OpeningMenu>().menuState = 2;
        //        Debug.Log("Keyboard control YES!");
        //    }
        //    else if (inputAccept && Input.GetKeyDown(advanceKeyCodeController))
        //    {
        //        levelSelect.controllerInput = true;
        //        playerSelect.controllerInput = true;
        //        gameObject.GetComponent<OpeningMenu>().menuState = 2;
        //        Debug.Log("Controller control YES!");
        //    }
        }
        //// set Y positions for the sprite
        //spriteY = Mathf.Lerp(spriteY, yDest, 0.15f);
        //controlSprite.GetComponent<Transform>().position = new Vector3(controlSprite.GetComponent<Transform>().position.x, spriteY, 1);

    }
}
