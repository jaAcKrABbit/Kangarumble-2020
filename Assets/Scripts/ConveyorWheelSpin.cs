using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ConveyorWheelSpin : MonoBehaviour
{
    public bool turnLeft;
    public float turnSpeed;
    
    private float randomPlusRot;

    void Start()
    {
        randomPlusRot = Random.Range(0, 360);
        gameObject.transform.Rotate(0, 0, randomPlusRot);
    }

    void Update()
    {
        int negate = (turnLeft) ? 1 : -1;
        gameObject.transform.Rotate(0, 0, turnSpeed * negate * Time.deltaTime);
    }
}
