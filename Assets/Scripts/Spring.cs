using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Spring : MonoBehaviour
{
    public GameObject spriteParentObj;

    private float xScale;
    public float yScale;
    private float yScaleNormal;
    private float yScaleExtend = 0.9f;
    private float yScaleDest;

    private float extendDampRate = 30f;
    private float retractDampRate = 15f;

    void Start()
    {
        xScale = spriteParentObj.transform.localScale.x;
        yScaleNormal = spriteParentObj.transform.localScale.y;
        yScaleDest = yScaleNormal;
        yScale = yScaleDest;
    }

    void FixedUpdate()
    {
        if (Mathf.Abs(yScale - yScaleExtend) < 0.005f) {
            yScaleDest = yScaleNormal;
        }

        float dampRate = (yScale < yScaleDest) ? extendDampRate : retractDampRate;
        yScale = Damp(yScale, yScaleDest, dampRate, Time.deltaTime);
        if (Mathf.Abs(spriteParentObj.transform.localScale.y - yScale) > 0.005f) {
            spriteParentObj.transform.localScale = new Vector3(xScale, yScale, 1);
        }
    }

    public void TriggerSpringAnimation()
    {
        yScaleDest = yScaleExtend;
    }

    public static float Damp(float a, float b, float lambda, float dt)
    {
        return Mathf.Lerp(a, b, 1 - Mathf.Exp(-lambda * dt));
    }
}
