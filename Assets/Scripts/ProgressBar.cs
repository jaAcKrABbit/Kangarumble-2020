using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class ProgressBar : MonoBehaviour
{
    private Slider slider;

    private float dest;
    private bool loadDone = false;

    public float smoothDampRateS = 1f;
    public float smoothDampRateM = 1f;
    public float smoothDampRateF = 1f;
    public float smoothDampRate;

    private GameObject playerAmountObj;

    void Start()
    {
        playerAmountObj = GameObject.Find("PlayerAmountObj");

        slider = gameObject.GetComponent<Slider>();
        slider.value = 0;

        Invoke("firstBoost", 0.5f);
        Invoke("secondBoost", 2f);
        Invoke("thirdBoost", 4.5f);
    }

    // Update is called once per frame
    void Update()
    {
        if(loadDone)
            slider.value = Damp(slider.value, dest, smoothDampRate, Time.deltaTime);

        if ((1f - slider.value) < 0.03f)
            Invoke("loadLevel", 0.5f);
    }

    void firstBoost()
    {
        smoothDampRate = smoothDampRateS;
        dest = 0.3f;
        loadDone = true;
    }
    void secondBoost()
    {
        smoothDampRate = smoothDampRateF;
        dest = 0.9f;
        loadDone = true;
    }
    void thirdBoost()
    {
        smoothDampRate = smoothDampRateM;
        dest = 1f;
        loadDone = true;
    }
    void loadLevel()
    {
        SceneManager.LoadScene(playerAmountObj.GetComponent<PlayerAmount>().currentLevelName);
    }

    public static float Damp(float a, float b, float lambda, float dt)
    {
        return Mathf.Lerp(a, b, 1 - Mathf.Exp(-lambda * dt));
    }
}
