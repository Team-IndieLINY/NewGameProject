using System.Collections;
using System.Collections.Generic;
using System.Threading;
using Cysharp.Threading.Tasks;
using UnityEngine;
using UnityEngine.Timeline;

public class MakingResultMarker : Marker, IMiniGameMarker
{
    public PropertyName id => "MakingResult";

    public IMiniGameBehaviour Create()
        => new MakingResultBehaviour();
}

public class MakingResultBehaviour : IMiniGameBehaviour
{
    public UniTask Invoke(IMiniGameBinder binder, CancellationTokenSource source)
    {
        var barController = binder.GetComponentT<BarController>("BarController");
        var result = binder.GetComponentT<CocktailResult>("CocktailResult");
        var hologram = binder.GetComponentT<HologramUI>("Hologram");

        hologram.CloseSummaryWindow();
        result.SetCocktail(barController.CurrentCocktailData);
        
        return UniTask.CompletedTask;
    }
}