using System;
using System.Collections;
using System.Collections.Generic;
using Cysharp.Threading.Tasks;
using DG.Tweening;
using MyBox;
using UniRx;
using UnityEngine;
using UnityEngine.SceneManagement;

public class MainMenu : MonoBehaviour
{
    private string SCENE = "FirstPersonView";
    private AsyncOperation _asyncOperation;
    
    private void Start()
    {
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
        _asyncOperation.allowSceneActivation = true;
    }

    public void GameQuit()
    {
        Application.Quit();
    }
}
