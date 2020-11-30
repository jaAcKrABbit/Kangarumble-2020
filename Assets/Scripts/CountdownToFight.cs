using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class CountdownToFight : MonoBehaviour
{
    public bool startCoundown;

    public Sprite[] countdownSprites = new Sprite[4];
    public int currentFrame = 0;

    public float scale;
    public float scaleDest;
    public float scaleBigDest;
    private float scaleMax;

    public bool shrink = false;

    private Image countdownImage;

    void Start()
    {
        shrink = false;
        scale = 0;
        scaleDest = scaleBigDest;
        scaleMax = 0.7f;
        startCoundown = false;

        countdownImage = GetComponent<Image>();
    }

    void Update()
    {
        countdownImage.sprite = countdownSprites[currentFrame];
        countdownImage.transform.localScale = new Vector3(scale, scale, 1);

        if (startCoundown) {
            if (shrink) {
                if (scale > 0.02f) {
                    scale = Damp(scale, 0, 20f, Time.deltaTime);
                }
                else {
                    scale = 0;
                    gameObject.SetActive(false);
                }
            }
            else {
                scale = Damp(scale, scaleDest, 11f, Time.deltaTime);
            }
        }

        // DEBUG: press shift to skip countdown
        if (Input.GetKeyUp(KeyCode.LeftShift)) {
            currentFrame = 3;
            shrink = true;
        }
    }

    void FixedUpdate()
    {
        if (startCoundown) {
            if (scale >= scaleBigDest - 2 && !shrink) {
                scaleDest += 0.0015f;
            }
        }
        scale = Mathf.Min(scale, scaleMax);
    }

    public void StartCountdown()
    {
        startCoundown = true;
        InvokeRepeating("NextFrame", 1.6f, 1.6f);
    }

    void NextFrame()
    {
        if (currentFrame < 3) {
            currentFrame++;
            scale = 0;
            scaleDest = scaleBigDest;
        }
        else {
            shrink = true;
        }
    }

    public static float Damp(float a, float b, float lambda, float dt)
    {
        return Mathf.Lerp(a, b, 1 - Mathf.Exp(-lambda * dt));
    }
}
