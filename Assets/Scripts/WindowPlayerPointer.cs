using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using CodeMonkey.Utils;

public class WindowPlayerPointer : MonoBehaviour
{
    private Camera mainCam;
    private GameObject countdownObj;
    private GameObject playerVarsObj;

    public GameObject[] followObj = new GameObject[4];
    public GameObject[] pointerObj = new GameObject[4];
    public GameObject[] pointerCharObj = new GameObject[4];

    private Vector3[] targetPos = new Vector3[4];
    private RectTransform[] pointerRectTransform = new RectTransform[4];
    private RectTransform[] pointerCharRectTransform = new RectTransform[4];

    private float borderRatio = 0.08f;
    private float borderRatioBottom = 0.21f;

    private float scaleFull = 0.9f;
    private float scaleCharMultiplier = 0.4f;
    private float[] scaleDest = new float[4];
    private float[] scale = new float[4];

    public Sprite[] avatarSprite = new Sprite[4];
    private int[] charSelect;

    void Awake()
    {
        for (int i = 0; i < 4; i++) {
            targetPos[i] = new Vector3(200, 45);
            pointerRectTransform[i] = pointerObj[i].GetComponent<RectTransform>();
            pointerCharRectTransform[i] = pointerCharObj[i].GetComponent<RectTransform>();
            pointerObj[i].SetActive(false);
            pointerCharObj[i].SetActive(false);
        }
        mainCam = GameObject.Find("Main Camera").GetComponent<Camera>();
        countdownObj = GameObject.Find("321Rumble");
        playerVarsObj = GameObject.Find("PlayerVars");
    }

    void Start()
    {
        charSelect = playerVarsObj.GetComponent<PlayerVars>().charSelect;
    }

    void Update()
    {
        GameObject currentActivePlayer = mainCam.GetComponent<TurnSwitch>().currentActivePlayer;

        float border = Screen.height * borderRatio;
        float borderBottom = Screen.height * borderRatioBottom;

        for (int i = 0; i < 4; i++) {

            GameObject currentPointerCharObj = pointerCharObj[i];
            currentPointerCharObj.GetComponent<Image>().sprite = avatarSprite[charSelect[i]];

            if (currentActivePlayer == followObj[i] || countdownObj.active || !followObj[i].active) {
                pointerObj[i].SetActive(false);
                pointerCharObj[i].SetActive(false);
            }
            else {
                targetPos[i] = followObj[i].transform.position;

                Vector3 toPos = targetPos[i];
                Vector3 fromPos = Camera.main.transform.position;
                fromPos.z = 0;
                Vector3 dir = (toPos - fromPos).normalized;
                float angle = UtilsClass.GetAngleFromVectorFloat(dir);
                pointerRectTransform[i].localEulerAngles = new Vector3(0, 0, angle);

                Vector3 targetPosScreenPoint = Camera.main.WorldToScreenPoint(targetPos[i]);

                bool isPlayerDead = followObj[i].GetComponent<Player>().imDead;
                bool isOffscreen = targetPosScreenPoint.x <= 0 || targetPosScreenPoint.x >= Screen.width
                                    || targetPosScreenPoint.y <= 0 || targetPosScreenPoint.y >= Screen.height;

                scaleDest[i] = (isOffscreen && !isPlayerDead) ? scaleFull : 0;
                scale[i] = Mathf.Lerp(scale[i], scaleDest[i], 0.35f);
                pointerRectTransform[i].localScale = new Vector3(scale[i], scale[i], 1);
                pointerCharRectTransform[i].localScale = new Vector3(scale[i] * scaleCharMultiplier, scale[i] * scaleCharMultiplier, 1);
                pointerObj[i].SetActive(scale[i] > 0.05f);
                pointerCharObj[i].SetActive(scale[i] > 0.05f);

                if (isOffscreen) {
                    Vector3 cappedTargetScreenPos = targetPosScreenPoint;

                    cappedTargetScreenPos.x = Mathf.Clamp(cappedTargetScreenPos.x, border, Screen.width - border);
                    cappedTargetScreenPos.y = Mathf.Clamp(cappedTargetScreenPos.y, borderBottom, Screen.height - border);

                    Vector3 pointerWorldPos = cappedTargetScreenPos;
                    pointerRectTransform[i].position = pointerWorldPos;
                    pointerCharRectTransform[i].position = pointerWorldPos;
                    pointerRectTransform[i].localPosition = new Vector3(
                        pointerRectTransform[i].localPosition.x,
                        pointerRectTransform[i].localPosition.y,
                        0f
                    );
                    pointerCharRectTransform[i].localPosition = pointerRectTransform[i].localPosition;
                }
            }
        }
        
    }
}
 