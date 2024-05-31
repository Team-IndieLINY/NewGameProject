using System;
using System.Collections;
using System.Collections.Generic;
using MyBox;
using UnityEngine;
using UnityEngine.UI;

public class TestA : MonoBehaviour
{
    [DisplayInspector] public DrinkData Data;

    public DrinkMeasurementData GameData;

    public Transform Target;
    public Transform MirrorTarget;
    public Transform TargetPivot;
    public Transform Jigger;
    public Transform Bottle;

    public SpriteRenderer A;
    public SpriteRenderer B;


    private Vector3 _targetPivot;
    private Vector3 _target;


    private void Awake()
    {
        var position = TargetPivot.position;
        var position1 = Target.position;

        _targetPivot = position;
        _target = position1;
    }

    private void OnValidate()
    {
        if (Data == false) return;
        if (A == false) return;
        if (B == false) return;

        A.sprite = Data.Sprite;
        B.sprite = Data.Sprite;
    }

    [ButtonMethod]
    private void SetOffset()
    {
        if (Data == false) return;
        if (TargetPivot == false) return;
        if (Bottle == false) return;

        Data.EDITOR_SetPositions(TargetPivot.localPosition, Bottle.localPosition);
    }

    private void Update()
    {
        SetToOrigin();
    }

    private void SetToOrigin()
    {
        var targetPos = DrinkMeasurementMiniGame.GetOriginDrinkPosition(
            Jigger.position,
            GameData.JiggerOffset,
            GameData.Angle,
            Data.BottlePosition,
            Data.RotationPivotPosition,
            Data.BottlePosition.magnitude);


        Target.position = targetPos;
        Target.rotation = Quaternion.identity;

        var m = DrinkMeasurementMiniGame.GetMatrix(targetPos, targetPos + Data.RotationPivotPosition, GameData.Angle);

        MirrorTarget.position = DrinkMeasurementMiniGame
            .GetMatrix(targetPos, Data.RotationPivotPosition + targetPos, GameData.MaxAngle)
            .GetPosition();
        MirrorTarget.rotation = Quaternion.Lerp(Quaternion.identity, m.rotation, GameData.MaxAngle / GameData.Angle);
    }
}