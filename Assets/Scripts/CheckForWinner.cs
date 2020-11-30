using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CheckForWinner : MonoBehaviour
{
    public int scoreLimit = 10;
    private GameObject playerAmountObj;
    private GameObject playerVarsObj;
    private GameObject gameEndObj;

    void Start()
    {
        playerAmountObj = GameObject.Find("PlayerAmountObj");
        playerVarsObj = GameObject.Find("PlayerVars");
        gameEndObj = GameObject.Find("GameEnd");

        for (int i = 0; i < 4; i++) {
            playerVarsObj.GetComponent<PlayerVars>().playerScores[i] = 0;
        }
        scoreLimit = playerAmountObj.GetComponent<PlayerAmount>().scoreLimit;
    }

    void Update()
    {
        // if any player's score reaches the score limit, we jump into the EndScene
        for (int i = 0; i < 4; i++) {
            if (playerVarsObj.GetComponent<PlayerVars>().playerScores[i] >= scoreLimit) {
                gameEndObj.GetComponent<GameEnd>().gameEnd = true;
            }
        }
    }
}
