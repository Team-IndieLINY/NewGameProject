using System;
using System.Collections;
using System.Collections.Generic;
using MyBox;
using UnityEngine;

[RequireComponent(typeof(SpriteRenderer))]
public class DrinkPosition : MonoBehaviour
{
    [field: SerializeField, AutoProperty, InitializationField]
    private SpriteRenderer _renderer;


    public SpriteRenderer Renderer => _renderer;

    public DrinkData Data { get; set; }

    public Vector3 BottleWorldPos =>
        transform.localToWorldMatrix.MultiplyPoint(Data.BottlePosition);
    public Vector3 RotatingPivotWorldPos =>
        transform.localToWorldMatrix.MultiplyPoint(Data.RotationPivotPosition);

    public Vector3 BottleLocalPos
        => Data.BottlePosition;
    public Vector3 RotatingPivotLocalPos
        => Data.RotationPivotPosition;

    private void Awake()
    {
        _renderer.sprite = null;
    }

    private void __MiniGame_Reset__()
    {
        _renderer.sprite = null;
    }
}
