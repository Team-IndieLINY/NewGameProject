using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using MyBox;
using TMPro;
using UnityEngine;

public class RecipeSummaryController : MonoBehaviour
{
    [field: SerializeField, AutoProperty(AutoPropertyMode.Scene), InitializationField, MustBeAssigned]
    private HologramUI _view;
    [field: SerializeField, AutoProperty(AutoPropertyMode.Scene), InitializationField, MustBeAssigned]
    private BarController _barController;

    [field: SerializeField, Multiline] private string _iceTextTemplate;
    [field: SerializeField, Multiline] private string _measurementTextTemplate;
    [field: SerializeField, Multiline] private string _shakingTextTemplate;
    
    
    private void __MiniGame_Reset__()
    {
        
    }
    
    private void Update()
    {
        var context = _barController.Context;

        if (context.IsIceEnd)
        {
            //_view.EvaluateMaterialBlock(context.is);
        }

        foreach (var item in context.MeasuredDrinkTable)
        {
            if (item.Value.IsEnd)
            {
                _view.EvaluateMaterialBlock(item.Key, item.Value.Score);
            }
        }
        
        
    }
}
