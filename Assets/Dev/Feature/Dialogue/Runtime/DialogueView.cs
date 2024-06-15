using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Zenject;
using DG.Tweening;
using TMPro;
using UnityEngine.Serialization;

public class DialogueView : MonoBehaviour
{
    [Serializable]
    public enum EArrowDirection
    {
        Npc,
        Master
    }
    
    [SerializeField] private RectTransform _upArrow;
    [SerializeField] private RectTransform _downArrow;
    [SerializeField] private RectTransform _skipArrow;
    [SerializeField] private TMP_Text _dialogueText;
    [SerializeField] private GameObject _content;

    [SerializeField] private float _skipTwinkleDuration = 0.5f;
    [SerializeField] private float _skipTwinkleX = 0.35f;
    [SerializeField] private Ease _skipTwinkleEase = Ease.Unset;
    

    private EArrowDirection _arrowDirection;

    public string DialogueText
    {
        get => _dialogueText.text;
        set => _dialogueText.text = value;
    }

    public EArrowDirection ArrowDirection
    {
        get => _arrowDirection;
        set
        {
            if (_arrowDirection == value) return;
            
            _arrowDirection = value;
            _upArrow.gameObject.SetActive(value == EArrowDirection.Npc);
            _downArrow.gameObject.SetActive(value == EArrowDirection.Master);
        }
    }

    public bool Visible
    {
        get => _content.activeSelf;
        set => _content.SetActive(value);
    }

    private void Awake()
    {
        StartCoroutine(CoAnimateSkipArrow());
    }
    private IEnumerator CoAnimateSkipArrow()
    {
        float x = _skipArrow.anchoredPosition.x;
        while (true)
        {
            yield return _skipArrow
                    .DOAnchorPosX(x - _skipTwinkleX, _skipTwinkleDuration)
                    .SetEase(_skipTwinkleEase)
                    .WaitForCompletion()
                ;
            yield return _skipArrow
                    .DOAnchorPosX(x, _skipTwinkleDuration)
                    .SetEase(_skipTwinkleEase)
                    .WaitForCompletion()
                ;
        }
    } 
}
