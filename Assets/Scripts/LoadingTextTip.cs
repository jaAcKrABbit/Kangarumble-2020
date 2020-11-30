using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class LoadingTextTip : MonoBehaviour
{
    private GameObject playerAmountObj;
    public Text testObj;
    public string[] tipsText = new string[5];
    public bool[] tipsRead = new bool[5];

    
    void Start()
    {
        testObj.CrossFadeAlpha(0f,0.01f, true);
        playerAmountObj = GameObject.Find("PlayerAmountObj");
        playerAmountObj.GetComponent<PlayerAmount>().tipsRead.CopyTo(tipsRead, 0);
        int displayingIndex;
        displayingIndex = Random.Range(0, 5);

        if (tipsRead[0] && tipsRead[1] && tipsRead[2] && tipsRead[3] && tipsRead[4])
        {
            Debug.Log("Error: all tips read without getting in the loop on LoadingTextTip");
        }

        // Random pick displaying text tip
        while (tipsRead[displayingIndex])
        {
            displayingIndex = Random.Range(0, 5);
        }
        tipsRead[displayingIndex] = true;
        playerAmountObj.GetComponent<PlayerAmount>().tipsRead[displayingIndex] = true;

        //TEMP LOCK IN DISPLAY ONLY ONE
        //testObj.text = tipsText[displayingIndex];
        testObj.text = tipsText[1];

        //If all tips read then reset the read info
        if (tipsRead[0] && tipsRead[1] && tipsRead[2] && tipsRead[3] && tipsRead[4])
        {
            tipsRead[0] = false;
            tipsRead[1] = false;
            tipsRead[2] = false;
            tipsRead[3] = false;
            tipsRead[4] = false;
            tipsRead[displayingIndex] = true;
            tipsRead.CopyTo(playerAmountObj.GetComponent<PlayerAmount>().tipsRead, 0);
        }
        Invoke("SetCrossFadeAlpha", 0.5f);
    }

    void Update()
    {

    }
    void SetCrossFadeAlpha()
    {
        testObj.CrossFadeAlpha(1f, 2f, true);
    }

    public void SetCrossFadeAlphaDown()
    {
        testObj.CrossFadeAlpha(0f, 0.5f, true);
    }
}
