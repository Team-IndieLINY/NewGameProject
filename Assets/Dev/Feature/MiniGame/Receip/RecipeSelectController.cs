using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading;
using Cysharp.Threading.Tasks;
using IndieLINY.MessagePipe;
using MyBox;
using UnityEngine;


public class CountScorePipeEvent : IMessagePipeEvent
{
    public CountScoreBehaviour.Parameter Parameter;
}

public class CountScoreChannel : PubSubMessageChannel<CountScorePipeEvent>
{
}

public class RecipeSelectController : MonoBehaviour, IMessagePipePublisher<CountScorePipeEvent>
{
    [field: SerializeField, AutoProperty(AutoPropertyMode.Scene), InitializationField, MustBeAssigned]
    private HologramUI _view;

    [field: SerializeField, AutoProperty(AutoPropertyMode.Scene), InitializationField, MustBeAssigned]
    private BarController _barController;

    public AsyncReactiveProperty<RecipeData> RecipeData { get; private set; } = new(null);

    private void OnEnable()
    {
        _view.OnClickPreview += OnSelected;
    }

    private void OnDisable()
    {
        _view.OnClickPreview -= OnSelected;
    }

    public void Open()
    {
        _view.OnClickOpenButton();
        _view.OnClickRecipeIconButton();
    }

    private void __MiniGame_Reset__()
    {
        RecipeData.Value = null;
        _barController.CurrentRecipeData = null;
        _barController.Context.Reset();
    }

    private void OnSelected(RecipeData data)
    {
        _barController.CurrentRecipeData = data;

        var context = _barController.Context;
        
        RecipeData.Value = data;
        context.Reset();
        context.IceMaxCount = data.Iceparameter.CountScoreParam.TargetCount;
        AddTable(data.MeansurementParameter1);
        AddTable(data.MeansurementParameter2);
        AddTable(data.MeansurementParameter3);
        AddTable(data.MeansurementParameter4);
    }

    private void AddTable(MiniMeasurementInfo info)
    {
        if (info == null) return;
        if (info.DrinkData == null) return;
        
        var context = _barController.Context;
        context.MeasuredDrinkTable[info.DrinkData] = new MeasureItem()
        {
            Info = info,
            Score = EMiniGameScore.Bad,
            IsEnd = false
        };
    }
}
