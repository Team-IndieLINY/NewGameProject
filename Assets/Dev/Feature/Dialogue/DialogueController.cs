using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading;
using Cysharp.Threading.Tasks;
using DG.Tweening;
using MyBox;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.Serialization;
using UnityEngine.UI;



public class DialogueController : MonoBehaviour
{
    [field: SerializeField, AutoProperty(AutoPropertyMode.Scene), InitializationField, MustBeAssigned]
    private NpcController _npcController;
    [field: SerializeField, AutoProperty(AutoPropertyMode.Scene), InitializationField, MustBeAssigned]
    private HologramUI _hologramUI;
    
    [field: SerializeField, InitializationField, MustBeAssigned]
    private GameObject _content;

    [field: SerializeField, AutoProperty, InitializationField, MustBeAssigned]
    private TMP_Text _text;


    [field: SerializeField, InitializationField, MustBeAssigned]
    private Image _upDirection;

    [field: SerializeField, InitializationField, MustBeAssigned]
    private Image _downDirection;
    

    [field: SerializeField, InitializationField, MustBeAssigned]
    private Image _skipTip;

    [SerializeField] private float _skipTwinkleDuration = 0.5f;
    [SerializeField] private float _textCompletionDuration = 0.35f;
    [SerializeField] private float _skipTwinkleX = 0.35f;
    [SerializeField] private Ease _skipTwinkleEase = Ease.Unset;
    [SerializeField] private scriptTable _table;

    private void Awake()
    {
        Hide();
        StartCoroutine(CoUpdate());
    }

    public void Show()
    {
        _content.SetActive(true);
        _text.text = "";
    }

    public void Hide()
    {
        _content.SetActive(false);
        _text.text = "";
    }

    public DialogueContext BeginText(string scriptCode)
    {
        var textList = _table.CreateScriptsInstance(scriptCode);
        
        //TODO: 더미 코드
        if (_hologramUI)
        {
            _hologramUI.ResetTextBlock();
            foreach (var item in textList)      
            {
                _hologramUI.AddTextBlock(item.CharacterName, item.Text);
            }
        }
        
        
        var context = new DialogueContext(
            _npcController,
            textList,
            _textCompletionDuration,
            _text,
            _upDirection,
            _downDirection
        );
        return context;
    }

    private IEnumerator CoUpdate()
    {
        float x = _skipTip.transform.position.x;
        while (true)
        {
            yield return _skipTip
                    .transform
                    .DOMoveX(_skipTwinkleX + x, _skipTwinkleDuration)
                    .SetEase(_skipTwinkleEase)
                    .WaitForCompletion()
                ;
            yield return _skipTip
                    .transform
                    .DOMoveX(x, _skipTwinkleDuration)
                    .SetEase(_skipTwinkleEase)
                    .WaitForCompletion()
                ;
        }
    } 
}

