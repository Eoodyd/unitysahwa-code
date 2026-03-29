using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class FrameCheckUI : MonoBehaviour
{
    [SerializeField] private Text frameText;
    [SerializeField] private Button frame30;
    [SerializeField] private Button frame60;
    [SerializeField] private Button frame144;
    [SerializeField] private Button frameNoLimit;
    private float deltaTime;    

    private void Awake()
    {
        frame30.onClick.AddListener(() => Application.targetFrameRate = 30);
        frame60.onClick.AddListener(() => Application.targetFrameRate = 60);
        frame144.onClick.AddListener(() => Application.targetFrameRate = 144);
        frameNoLimit.onClick.AddListener(() => Application.targetFrameRate = -1);
    }
    private void Update()
    {
        //부드러운 전환을 위한 보간
        //deltaTime += (Time.unscaledDeltaTime - deltaTime) * 0.1f;
        deltaTime = Time.unscaledDeltaTime;

        float fps = 1.0f / deltaTime;
        frameText.text = $"{Mathf.RoundToInt(fps)} FPS";
    }
}
