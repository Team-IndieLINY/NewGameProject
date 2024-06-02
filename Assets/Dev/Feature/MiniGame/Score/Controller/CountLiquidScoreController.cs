using System;
using System.Collections;
using System.Collections.Generic;
using Cysharp.Threading.Tasks;
using MyBox;
using UnityEngine;

public class CountLiquidScoreController : ScoreController
{
    [SerializeField] private LiquidScoreDisplayer.Parameter _displayerParameter;

    private CountScoreBehaviour _behaviour;
    private LiquidScoreDisplayer _displayer;

    private void Awake()
    {
        Setup(null);
        
        Release();
    }

    public void Setup(CountScoreBehaviour.Parameter? behaviourParameter)
    {
        Release();

        if (behaviourParameter == null)
        {
            behaviourParameter = new CountScoreBehaviour.Parameter()
            {
                Range = new(),
                TargetCount = int.MaxValue
            };
        }
        
        _behaviour = new(behaviourParameter.Value);
        _displayer = new(_displayerParameter);
    }

    public EMiniGameScore CurrentScore => _behaviour.GetCalculatedScore();

    public void Release()
    {
        _behaviour?.Dispose();
        _displayer?.Dispose();
        
        _behaviour = null;
        _displayer = null;
    }

    private void __MiniGame_Reset__()
    {
        Release();
    }

    public void AddCount(int value)
    {
        _behaviour.CurrentScore += value;
    }

    public void SetCount(int value)
    {
        _behaviour.CurrentScore = value;
    }

    public void ShowLine(float pos)
    {
        _displayer.LineEnabled = true;
        _displayer.SetLinePosition(pos);
    }
    public void HideLine()
    {
        _displayer.LineEnabled = false;
    }

    public UniTask DisplayResult()
    {
        EMiniGameScore score = _behaviour.GetCalculatedScore();
        
        return _displayer.Display(score);
    }
    
}
