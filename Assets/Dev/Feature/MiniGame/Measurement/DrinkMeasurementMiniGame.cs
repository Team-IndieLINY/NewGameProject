using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using Cysharp.Threading.Tasks;
using DG.Tweening;
using MyBox;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem;
using UnityEngine.Serialization;
using UnityEngine.UI;


public class DrinkMeasurementMiniGame : MonoBehaviour
{
    #region Inspector
    [field: SerializeField, OverrideLabel("계량 미니게임 데이터"), Foldout("데이터"), InitializationField, MustBeAssigned]
    private DrinkMeasurementData _data;
    
    [field: SerializeField, Foldout("컴포넌트(건들지 마시오)"), InitializationField, MustBeAssigned]
    private MiniGameCircleTimer _circleTimer;
    
    [field: SerializeField, Foldout("컴포넌트(건들지 마시오)"), InitializationField, MustBeAssigned]
    private HUDController _moneyHud;
    
    [field: SerializeField, Foldout("컴포넌트(건들지 마시오)"), InitializationField, MustBeAssigned]
    private GameObject _prefab;
    
    [field: SerializeField, Foldout("컴포넌트(건들지 마시오)"), InitializationField, MustBeAssigned]
    private MeshRenderer _renderTexturePannel;
    
    [field: SerializeField, Foldout("컴포넌트(건들지 마시오)"), InitializationField, MustBeAssigned]
    private Camera _renderCamera;
    
    [field: SerializeField, Foldout("컴포넌트(건들지 마시오)"), InitializationField, MustBeAssigned]
    private RenderTexture _renderTexture;
    #endregion


    #region Getter/Setter
    public AsyncReactiveProperty<int> LiquidCount { get; private set; } = new(0);
    public DrinkMeasurementData Data => _data;
    public Transform Jigger { get; set; }
    public DrinkPosition Drink { get; set; }
    #endregion


    
    private InputAction _keyAction;
    private Queue<GameObject> _liquidQueue = new (100);

    private Matrix4x4 _measureMatrix;
    private Vector3 _originPos;
    private Vector3 _validationOriginPos;
    private float _normalizedScore;
    private float _t;
    private float _creationTimer;

    public static Matrix4x4 GetMatrix(Vector3 position, Vector3 pivot, float angle)
    {
        Vector3 vector3 = Quaternion.AngleAxis(angle, Vector3.forward) * (position - pivot);
        position = pivot + vector3;
        
        var m =Matrix4x4.TRS(
            position,
            Quaternion.Euler(0f, 0f, angle),
            Vector3.one
        );

        return m;
    }
    public static Matrix4x4 GetInverseMatrix(Vector3 position, Vector3 pivot, float angle)
    {
        Vector3 vector3 = Quaternion.AngleAxis(-angle, Vector3.forward) * (position - pivot);
        position = pivot + vector3;
        
        var m =Matrix4x4.TRS(
            position,
            Quaternion.identity,
            Vector3.one
        );

        return m;
    }

    public static Vector3 GetOriginDrinkPosition(Vector3 jiggerPos, Vector2 jiggerOffset,
        float angle, Vector3 bottleOffset, Vector3 rotationPivotOffset, float bottleToPivotLength)
    {
        var bottlePos = jiggerPos + (Vector3)jiggerOffset;
        var pivot = bottlePos + Quaternion.AngleAxis(angle, Vector3.forward) * (rotationPivotOffset - bottleOffset);
        var m = GetInverseMatrix(bottlePos, pivot, angle);

        return m.GetPosition() + Vector3.down * bottleToPivotLength;
    }

    [ButtonMethod]
    private void ClearLiquid()
    {
        if (Application.isPlaying == false) return;
        
        while (_liquidQueue.Any())
        {
            Destroy(_liquidQueue.Dequeue());
        }
        
    }
    
    private void Awake()
    {
        _keyAction = InputManager.Actions.ShakingMiniGameInteraction;

        Started = false;
        LiquidCount.Value = 0;

        _renderCamera.targetTexture = _renderTexture;
    }

    private void OnDestroy()
    {
        _renderCamera.targetTexture = null;
    }

    public void ApplyLiquidMaterial()
    {
        if (Drink.Data == null) return;
        
        if (Drink.Data.LiquidMaterial)
        {
            if (_renderTexturePannel.sharedMaterial == Drink.Data.LiquidMaterial) return;
            
            _renderTexturePannel.sharedMaterial = Drink.Data.LiquidMaterial;
        }
    }

    public UniTask Calculate()
    {
        var originDrinkPos = GetOriginDrinkPosition(
            Jigger.position,
            Data.JiggerOffset,
            Data.Angle,
            Drink.BottleLocalPos, Drink.RotatingPivotLocalPos,
            Vector3.Magnitude(Drink.BottleLocalPos)
        );
        
        _renderTexturePannel.gameObject.SetActive(true);
        
        _originPos = originDrinkPos;
        var back = Drink.transform.position;
        _validationOriginPos = back;
        Drink.transform.position = _originPos;
        _measureMatrix = GetMatrix(_originPos, Drink.RotatingPivotWorldPos, Data.Angle);
        Drink.transform.position = back;
        
        
        return UniTask.WhenAll(
            Drink.transform.DOMove(
                GetMatrix(_originPos, Drink.RotatingPivotLocalPos + _originPos, Data.DefaultAngle).GetPosition(), //position
                Data.StartOfRollbackDuration) // duration
                .SetId(this).AsyncWaitForCompletion().AsUniTask(), // task 처리
            
            Drink.transform.DORotateQuaternion(
                Quaternion.Lerp(Quaternion.identity, _measureMatrix.rotation, Data.DefaultAngle / Data.Angle), // rotation
                Data.StartOfRollbackDuration) //duration
                .SetId(this).AsyncWaitForCompletion().AsUniTask() // task 처리

            ).WithCancellation(GlobalCancelation.PlayMode);
    }

    public bool Started
    {
        get => enabled;
        set
        {
            if (enabled == value)
                return;

            enabled = value;
            
            if(enabled)
                _circleTimer.TimerStart(Data.GameDuration);
        }
    }

    public void GameReset()
    {
        while (_liquidQueue.Any())
        {
            Destroy(_liquidQueue.Dequeue());
        }

        _t = 0f;
        LiquidCount.Value = 0;

        DOTween.Kill(this);
        _renderTexturePannel.gameObject.SetActive(false);
        
        if(_circleTimer)
            _circleTimer.TimerStop();
    }

    private void __MiniGame_Reset__()
    {
        GameReset();
        Started = false;

    }
    
    private void Update()
    {
        if (Data.IsValidation)
        {
            if (Application.isPlaying == false) return;
            if (Jigger == false) return;
            if (Data == false) return;
            if (Drink == false) return;
            DOTween.Kill(this);
        
            _t = 0f;
            Drink.transform.position = _validationOriginPos;
            Drink.transform.rotation = Quaternion.identity;

            Calculate();

            Data.IsValidation = false;
        }
        
        
        _t += Time.deltaTime * (_keyAction.IsPressed() ? Data.MeasurementSpeed : -Data.BackToOriginDuration);
        _t = Mathf.Clamp(_t, Data.DefaultAngle / Data.Angle, Data.MaxAngle / Data.Angle);
        
        Quaternion rot = _measureMatrix.rotation;

        Drink.transform.position = GetMatrix(_originPos, Drink.RotatingPivotLocalPos + _originPos, _t * Data.Angle)
            .GetPosition();
        Drink.transform.rotation = Quaternion.Lerp(Quaternion.identity, rot, _t);
        
        
        _creationTimer += Time.deltaTime;

        if (_t * Data.Angle >= Data.LiquidCreationStartAngle)
        {
            if (_creationTimer > Data.CircleCreationDelay)
            {
                CreateCircle();
                _creationTimer = 0f;
            }
        }
    }

    private void CreateCircle()
    {
        var obj = Instantiate(_prefab);
        obj.SetActive(true);
        if (Drink == false || Drink.Data == false)
        {
            Debug.LogError("Drink 혹은 Drink의 데이터가 null 입니다.");
            return;
        }
        
        while (_liquidQueue.Count > Data.MaxCircleCount)
        {
            var deletionObj = _liquidQueue.Dequeue();
            Destroy(deletionObj);
        }

        var rigid = obj.GetComponent<Rigidbody2D>();
        rigid.AddForce(Drink.transform.up * Data.LiquidForce, ForceMode2D.Impulse);

        obj.transform.position = Drink.BottleWorldPos;
        _liquidQueue.Enqueue(obj);
        _moneyHud.SetValue( _moneyHud.Value - 10, false);
        LiquidCount.Value += 1;
    }

}
