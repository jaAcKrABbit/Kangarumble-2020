using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameEnd : MonoBehaviour
{
    public bool gameEnd = false;
    public float scale = 0f;
    public float scaleDest = 0.45f;
    public float scaleBigDest = 0.45f;
    public float scaleMax = 0.5f;

    public AudioSource musicSource;

    private bool endSceneInvoked = false;

    private GameObject transitionManager;

    void Start()
    {
        gameEnd = false;
        transitionManager = GameObject.Find("SceneTransition");
    }

    void Update()
    {
        gameObject.transform.localScale = new Vector3(scale, scale, 1);

        if (gameEnd) {
            if (scale < scaleBigDest - 0.05f) {
                scale = Damp(scale, scaleBigDest, 11f, Time.deltaTime);
            }
            else {
                scale = Damp(scale, scaleDest, 11f, Time.deltaTime);
            }
        }
        scale = Mathf.Clamp(scale, 0, scaleMax);

        if (gameEnd && !endSceneInvoked) {
            musicSource.Stop();
            endSceneInvoked = true;
            transitionManager.GetComponent<TransitionManager>().Invoke("GameEndClose", 3f);
        }
    }

    void FixedUpdate()
    {
        if (gameEnd) {
            scaleDest += 0.0005f;
        }
    }

    public static float Damp(float a, float b, float lambda, float dt)
    {
        return Mathf.Lerp(a, b, 1 - Mathf.Exp(-lambda * dt));
    }
}
