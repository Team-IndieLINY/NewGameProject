using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using Cysharp.Threading.Tasks;
using DG.Tweening;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;


public class OpeningController : MonoBehaviour
{

    
    [SerializeField] private Image _pannel;
    [SerializeField] private float _fadeinDuration = 1f;
    [SerializeField] private float _fadeoutDuration = 1f;

    [SerializeField] private List<OpeningSceneItem> _scenes;

    private string SCENE = "FirstPersonView";
    private AsyncOperation _asyncOperation;
    
    private void LoadScene()
    {
        _asyncOperation = SceneManager.LoadSceneAsync(SCENE, LoadSceneMode.Additive);
        _asyncOperation.allowSceneActivation = false;
        _asyncOperation.completed += o =>
        {
            DOTween.KillAll();
            SceneManager.SetActiveScene(SceneManager.GetSceneByName(SCENE));
            SceneManager.UnloadSceneAsync("Opening");
        };
    }
    private void Start()
    {
        bool first = true;
        foreach (var item in _scenes)
        {
            if (item == false) continue;
            
            item.Text.text = "";
            
            if (first)
            {
                first = false;
                item.Parent.SetActive(true);
            }
            else
            {
                item.Parent.SetActive(false);
            }
            
        }

        LoadScene();
        DoColor().Forget();
    }

    private async UniTaskVoid DoColor()
    {
        var color = _pannel.color = new Color(0f, 0f, 0f, 1f);

        float t = 0f;
        
        while (true)
        {
            color.a = Mathf.Lerp(1f, 0f, t);
            _pannel.color = color;

            if (t > 1f)
            {
                break;
            }

            t += Time.deltaTime * (1f / _fadeinDuration);

            await UniTask.NextFrame(PlayerLoopTiming.Update, GlobalCancelation.PlayMode);
        }
        
        
        Begin().Forget();
    }

    private async UniTaskVoid Begin()
    {
        foreach (var item in _scenes)
        {
            try
            {
                await TextAnimate(item);
            }
            catch (Exception e) when (e is not OperationCanceledException)
            {
                Debug.LogException(e);
            }
        }

        _scenes.Last().Parent.SetActive(true);
        _pannel.color = new Color(0f, 0f, 0f, 0f);
        _pannel.DOColor(new Color(0f, 0f, 0f, 1f), _fadeoutDuration).OnComplete(() =>
        {
            _asyncOperation.allowSceneActivation = true;
        });
    }

    private async UniTask TextAnimate(OpeningSceneItem item)
    {
        item.Parent.SetActive(true);

        var source = new CancellationTokenSource();

        item.Text.text = "";
        
        await UniTask.WhenAny(
            item.Text.DoTextUniTask(item.Script, item.TextAnimationDuration, source.Token),
            UniTask.WaitUntil(() => InputManager.Actions.DialogueSkip.triggered, PlayerLoopTiming.Update, GlobalCancelation.PlayMode)
        ).WithCancellation(GlobalCancelation.PlayMode);

        source.Cancel();
        item.Text.text = item.Script;

        await UniTask.WaitUntil(() => InputManager.Actions.DialogueSkip.triggered, PlayerLoopTiming.Update, GlobalCancelation.PlayMode);
        item.Parent.SetActive(false);
    }
}
