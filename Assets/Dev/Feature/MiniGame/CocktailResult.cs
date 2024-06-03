using System;
using System.Collections;
using System.Collections.Generic;
using MyBox;
using TMPro;
using UnityEngine;

public class CocktailResult : MonoBehaviour
{
    [field: SerializeField, InitializationField, MustBeAssigned]
    private Transform _pivot;

    [field: SerializeField]
    private TMP_Text _text;
    
    private GameObject _cocktail;
    
    public void SetCocktail(CocktailData data)
    {
        if (_cocktail)
        {
            Destroy(_cocktail);
            _cocktail = null;
        }
        
        _cocktail = data.ClonePrefab();

        _cocktail.transform.position = _pivot.position;
        _text.text = data.CocktailName;
    }
    

    private void __MiniGame_Reset__()
    {
        if (_cocktail)
        {
            Destroy(_cocktail);

            _cocktail = null;
        }
    }
}
