using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SetAnimationParents : MonoBehaviour
{
    public GameObject[] playerObjs = new GameObject[4];
    public GameObject[] animationObjs = new GameObject[4];

    public Vector3[] charPlusPos = new Vector3[4];

    private GameObject playerVarsObj;
    private int counter = 3;
    private bool haveSetParents = false;

    void Start()
    {
        playerVarsObj = GameObject.Find("PlayerVars");

        for (int i = 0; i < 4; i++) {
            animationObjs[i].SetActive(false);
        }
    }

    void Update()
    {
        if (counter > 0) {
            counter--;
        }
        else if (!haveSetParents) {
            SetParents();
            haveSetParents = true;
        }
    }

    void SetParents()
    {
        for (int i = 0; i < 4; i++) {
            //int currentPlayerOrder = playerVarsObj.GetComponent<PlayerVars>().playerOrder[i];
            int currentCharSelect = playerVarsObj.GetComponent<PlayerVars>().charSelect[i];
            GameObject currentPlayerObj = playerObjs[i];
            GameObject currentAnimationObj = animationObjs[currentCharSelect];

            if (playerVarsObj.GetComponent<PlayerVars>().playerIn[i]) {
                animationObjs[currentCharSelect].SetActive(true);
                currentPlayerObj.GetComponent<Player>().myAnimatorObj = currentAnimationObj;
                currentAnimationObj.transform.position = currentPlayerObj.transform.position + charPlusPos[currentCharSelect];
                currentAnimationObj.transform.parent = currentPlayerObj.transform;
            }
        }
    }
}
