using System.Collections;
using System.Collections.Generic;
using System.Threading;
using Cysharp.Threading.Tasks;
using UnityEngine;
using UnityEngine.Timeline;

public class RecipeSelectMarker : Marker, IMiniGameMarker
{
    public PropertyName id => "RecipeSelect";

    public IMiniGameBehaviour Create()
        => new RecipeSelectBehaviour();
}

public class RecipeSelectBehaviour : IMiniGameBehaviour
{
    public async UniTask Invoke(IMiniGameBinder binder, CancellationTokenSource source)
    {
        var controller = binder.GetComponentT<RecipeSelectController>("RecipeSelectController");
        //var summaryController = binder.GetComponentT<RecipeSummaryController>("RecipeSummaryController");

        controller.Open();

        await controller.RecipeData.WaitAsync();
    }
}