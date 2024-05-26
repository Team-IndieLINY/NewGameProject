using System;
using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using MyBox;
using UnityEngine;
using UnityEngine.Serialization;

public class BarPasser : MonoBehaviour
{
    [SerializeField] private Vector3 _startPos;
    [SerializeField] private Vector3 _endPos;
    [SerializeField] private float _resetDelay = 10f;
    [SerializeField] private float _movementDuration = 10f;

    [ButtonMethod]
    private void SetStartPosition()
    {
        _startPos = transform.position;
    }
    [ButtonMethod]
    private void SetEndPosition()
    {
        _endPos = transform.position;
    }

    private void Awake()
    {
        StartCoroutine(CoUpdate());
    }

    private IEnumerator CoUpdate()
    {
        while (true)
        {
            transform.position = _startPos;
            var tween = transform.DOMove(_endPos, _movementDuration).SetEase(Ease.Linear);

            yield return tween.WaitForCompletion();
            
            yield return new WaitForSeconds(_resetDelay);
        }
    }
}