using System;
using System.Collections;
using System.Collections.Generic;
using Cysharp.Threading.Tasks;
using DG.Tweening;
using MyBox;
using UniRx;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class MainMenu : MonoBehaviour
{
    private string SCENE = "FirstPersonView";
    private AsyncOperation _asyncOperation;

    [SerializeField] private Image _fadeoutImage;
    [SerializeField] private float _fadeoutDuration =1f;
    
    private void Start()
    {
        _fadeoutImage.color = new Color(0f, 0f, 0f, 0f);
        _asyncOperation = SceneManager.LoadSceneAsync(SCENE, LoadSceneMode.Additive);
        _asyncOperation.allowSceneActivation = false;
        _asyncOperation.completed += o =>
        {
            DOTween.KillAll();
            SceneManager.SetActiveScene(SceneManager.GetSceneByName(SCENE));
            SceneManager.UnloadSceneAsync("MainMenu");
        };
    }

    public void GameStart()
    {
        _fadeoutImage.color = new Color(0f, 0f, 0f, 0f);
        _fadeoutImage.DOColor(new Color(0f, 0f, 0f, 1f), _fadeoutDuration)
            .OnComplete(()=>_asyncOperation.allowSceneActivation = true);
    }

    public void GameQuit()
    {
        Application.Quit();
    }
}
