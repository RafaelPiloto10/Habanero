using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class UIPanel : MonoBehaviour
{
    [SerializeField]
    GameObject fpsObject;

    TextMeshProUGUI fpsText;
    GameObject canvas;

    float deltaTime = 0.0f;
    int fps = 0;
    

    private void Start()
    {
        fpsText = fpsObject.GetComponent<TextMeshProUGUI>();
        canvas = GameObject.FindGameObjectWithTag("Canvas");
    }

    private void Update()
    {
        deltaTime += Time.deltaTime;
        deltaTime /= 2.0f;
        fps = (int) (1.0f / deltaTime);

        fpsText.text = string.Format("FPS: {0}", fps);

        if (Input.GetKeyUp(KeyCode.D))
        {
            SimulationSettings.Instance().ToggleDebug();
            canvas.SetActive(SimulationSettings.Instance().IsDebug());
        }
    }
}
