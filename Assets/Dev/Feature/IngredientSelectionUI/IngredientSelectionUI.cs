using System;
using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.Serialization;
using UnityEngine.UI;

public class IngredientSelectionUI : MonoBehaviour
{
    [SerializeField] private Transform[] _ingredientTransforms;
    [SerializeField] private Color32[] _ingredientColors;
    [SerializeField] private GameObject _ingredientPrefab;
    [SerializeField] private Transform _ingredientGroup;
    [SerializeField] private Sprite[] _ingredientSprites;
    [SerializeField] private DrinkData[] _ingredientDatas;
     private GameObject[] _ingredientPrefabs;

     [SerializeField] private Vector2 _ingredientCaptureIndexRange =new Vector2(0, 4);

     private Vector2 _backupRangeIngredientCaptureIndexRange;
     private InputAction _leftInput;
     private InputAction _rightInput;
     
     public DrinkData CurrentData { get; private set; }
     private int _dataIndex;

     private void Awake()
     {
         _backupRangeIngredientCaptureIndexRange = _ingredientCaptureIndexRange;
         _leftInput = InputManager.Actions.MoveLeftIngredientSelection;
         _rightInput = InputManager.Actions.MoveRightIngredientSelection;
     }

     private void Start()
     {
         _ingredientCaptureIndexRange = _backupRangeIngredientCaptureIndexRange;
        _ingredientPrefabs = new GameObject[_ingredientTransforms.Length];

        _dataIndex = 2;
        CurrentData = _ingredientDatas[_dataIndex];

        for (int i = 0; i < _ingredientPrefabs.Length; i++)
        {
            _ingredientPrefabs[i] = Instantiate(_ingredientPrefab,
                _ingredientTransforms[i].position,
                _ingredientTransforms[i].rotation,
                    _ingredientGroup);
            
            _ingredientPrefabs[i].transform.localScale = _ingredientTransforms[i].localScale;
            _ingredientPrefabs[i].GetComponent<Image>().color = _ingredientColors[i];
            _ingredientPrefabs[i].GetComponent<Image>().sprite = _ingredientSprites[i];
            _ingredientPrefabs[i].GetComponent<Image>().SetNativeSize();
        }
    }

    private void __MiniGame_Reset__()
    {
        DOTween.Kill(this);

        foreach (var obj in _ingredientPrefabs)
        {
            Destroy(obj);
        }
        
        Start();
        
    }

    public void MoveStop()
    {
        DOTween.Complete(this);
    }
    
    private void Update()
    {
        if (_leftInput.triggered)
        {
            TurnLeft();
        }
        if(_rightInput.triggered)
        {
            TurnRight();
        }
        
    }

    public void TurnRight()
    {
        if ((int)_ingredientCaptureIndexRange.x == _ingredientDatas.Length - 3)
        {
            return;
        }

        CurrentData = _ingredientDatas[++_dataIndex];
        
        DOTween.Kill(this);
        
        _ingredientCaptureIndexRange.x++;
        _ingredientCaptureIndexRange.y++;
        
        for (int i = 0; i < _ingredientPrefabs.Length; i++)
        {
            if (_ingredientPrefabs[i] == null)
            {
                continue;
            }
            
            if (i == 0)
            {
                Destroy(_ingredientPrefabs[i]);
                continue;
            }

            _ingredientPrefabs[i].transform.DOMove(_ingredientTransforms[i - 1].position, 0.5f).SetId(this);
            _ingredientPrefabs[i].transform.DOScale(_ingredientTransforms[i - 1].localScale, 0.5f).SetId(this);
            _ingredientPrefabs[i].GetComponent<Image>().DOColor(_ingredientColors[i - 1], 0.5f).SetId(this);
            _ingredientPrefabs[i].GetComponent<Image>().SetNativeSize();
            
            _ingredientPrefabs[i - 1] = _ingredientPrefabs[i];
            _ingredientPrefabs[i] = null;
            
            if (i == _ingredientPrefabs.Length - 1 && _ingredientDatas.Length - _ingredientCaptureIndexRange.y - 1 >= 0)
            {
                _ingredientPrefabs[i] = Instantiate(_ingredientPrefab,
                    _ingredientTransforms[i].position,
                    _ingredientTransforms[i].rotation,
                    _ingredientGroup);

                _ingredientPrefabs[i].GetComponent<Image>().sprite = _ingredientSprites[i + (int)_ingredientCaptureIndexRange.x];
                _ingredientPrefabs[i].GetComponent<Image>().SetNativeSize();
                _ingredientPrefabs[i].transform.localScale = _ingredientTransforms[i].localScale;
            }
        }
    }
    
    public void TurnLeft()
    {
        
        if ((int)_ingredientCaptureIndexRange.x == -2)
        {
            return;
        }
        
        CurrentData = _ingredientDatas[--_dataIndex];
        
        DOTween.Kill(this);
        
        _ingredientCaptureIndexRange.x--;
        _ingredientCaptureIndexRange.y--;
        for (int i = _ingredientPrefabs.Length - 1; i >= 0; i--)
        {
            if (_ingredientPrefabs[i] == null)
            {
                continue;
            }
            
            if (i == _ingredientPrefabs.Length - 1)
            {
                Destroy(_ingredientPrefabs[i]);
                continue;
            }

            _ingredientPrefabs[i].transform.DOMove(_ingredientTransforms[i + 1].position, 0.5f).SetId(this);
            _ingredientPrefabs[i].transform.DOScale(_ingredientTransforms[i + 1].localScale, 0.5f).SetId(this);
            _ingredientPrefabs[i].GetComponent<Image>().DOColor(_ingredientColors[i + 1], 0.5f).SetId(this);
            _ingredientPrefabs[i].GetComponent<Image>().SetNativeSize();
            
            _ingredientPrefabs[i + 1] = _ingredientPrefabs[i];

            _ingredientPrefabs[i] = null;
            if (i == 0 && _ingredientCaptureIndexRange.x + i >= 0)
            {
                _ingredientPrefabs[i] = Instantiate(_ingredientPrefab,
                    _ingredientTransforms[i].position,
                    _ingredientTransforms[i].rotation,
                    _ingredientGroup);
                
                _ingredientPrefabs[i].GetComponent<Image>().sprite = _ingredientSprites[i + (int)_ingredientCaptureIndexRange.x];
                _ingredientPrefabs[i].GetComponent<Image>().SetNativeSize();
                _ingredientPrefabs[i].transform.localScale = _ingredientTransforms[i].localScale;
            }
        }
    }
}  