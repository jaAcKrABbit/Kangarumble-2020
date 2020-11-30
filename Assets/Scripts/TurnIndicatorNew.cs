using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TurnIndicatorNew : MonoBehaviour
{
    private GameObject camera;
    private GameObject countdownObj;

    public GameObject currentActivePlayer;
    public GameObject[] subspriteObj = new GameObject[4];
    public Sprite sprite;
    public Color spriteColor;

    public bool rotateToCenter;

    public float plusY;
    public float scale;
    public float subspriteLen;
    public float subspriteLenIn;
    public float subspriteLenOut;

    private float plusDir = 0;
    public float plusDirMultiply = 0;
    private int subspriteCount;

    public float zPos;

    void Start()
    {
        camera = GameObject.Find("Main Camera");
        countdownObj = GameObject.Find("321Rumble");

        subspriteCount = subspriteObj.Length;
        for (int i = 0; i < subspriteCount; i++) {
            subspriteObj[i].SetActive(false);
        }
    }

    void Update()
    {
        ResetPos();

        transform.localScale = new Vector3(scale, scale, scale);
        plusDir += Time.deltaTime * plusDirMultiply;

        if (camera.GetComponent<CameraFollow>().caughtUpPipe) {
            subspriteLen = Damp(subspriteLen, subspriteLenIn, 14f, Time.deltaTime);
        }


        for (int i = 0; i < subspriteCount; i++) {
            subspriteObj[i].SetActive(!countdownObj.active);

            float dir = i * (360 / (subspriteCount)) + (plusDir);
            float x = gameObject.transform.position.x + LengthdirX(subspriteLen, dir);
            float y = gameObject.transform.position.y + LengthdirY(subspriteLen, dir) + plusY;
            Vector3 pos = new Vector3(x, y, zPos);
            subspriteObj[i].transform.position = pos;
            subspriteObj[i].GetComponent<SpriteRenderer>().sprite = sprite;
            subspriteObj[i].GetComponent<SpriteRenderer>().color = spriteColor;

            if (rotateToCenter) {
                subspriteObj[i].transform.eulerAngles = new Vector3(0, 0, -dir);
            }
        }
    }

    public void ResetPos()
    {
        currentActivePlayer = camera.GetComponent<TurnSwitch>().currentActivePlayer;
        //gameObject.transform.position = currentActivePlayer.transform.position;
        Vector3 newPos = new Vector3(currentActivePlayer.transform.position.x, currentActivePlayer.transform.position.y, zPos);
        gameObject.transform.position = newPos;
        gameObject.transform.parent = currentActivePlayer.transform;
    }

    public float LengthdirX(float len, float dir) {
        return len * Mathf.Cos(dir * Mathf.Deg2Rad);
    }

    public float LengthdirY(float len, float dir) {
        return len * -Mathf.Sin(dir * Mathf.Deg2Rad);
    }

    public static float Damp(float a, float b, float lambda, float dt)
    {
        return Mathf.Lerp(a, b, 1 - Mathf.Exp(-lambda * dt));
    }
}