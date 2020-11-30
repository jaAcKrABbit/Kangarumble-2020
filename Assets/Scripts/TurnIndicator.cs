using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TurnIndicator : MonoBehaviour
{
    private GameObject camera;
    public GameObject currentActivePlayer;

    void Start()
    {
        camera = GameObject.Find("Main Camera");
    }

    void Update()
    {
        // follow currentActivePlayer object
        currentActivePlayer = camera.GetComponent<TurnSwitch>().currentActivePlayer;
        float currentActivePlayerX = currentActivePlayer.transform.position.x;
        float currentActivePlayerY = currentActivePlayer.transform.position.y;
        transform.position = new Vector3(currentActivePlayerX, currentActivePlayerY, transform.position.z);

        // spin constantly
        transform.Rotate(0, 0, 50 * Time.deltaTime);
    }
}
