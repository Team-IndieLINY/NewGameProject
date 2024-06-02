using System;
using System.Collections;
using System.Collections.Generic;
using MyBox;
using Unity.VisualScripting;
using UnityEngine;

[System.Serializable]
public class DialougeDummyItem
{
    public string BadKey;
    public string GoodKey;
    public string PerfectKey;
}

public class DilaogueDummyItmeUnit : Unit
{
    [DoNotSerialize] private ControlInput _cInput;
    [DoNotSerialize] private ControlOutput _cOutput;

    [DoNotSerialize] private ValueInput _vDummyItemInput;
    [DoNotSerialize] private ValueInput _vDummyScoreInput;
    [DoNotSerialize] private ValueOutput _vOutput;
    
    protected override void Definition()
    {
        _cInput = ControlInput("", OnExecute);
        _cOutput = ControlOutput("");

        _vDummyItemInput = ValueInput<DialougeDummyItem>("item");
        _vDummyScoreInput = ValueInput<EMiniGameScore>("score");
        _vOutput = ValueOutput<string>("key");
    }

    private ControlOutput OnExecute(Flow flow)
    {
        var item = flow.GetValue<DialougeDummyItem>(_vDummyItemInput);
        var score = flow.GetValue<EMiniGameScore>(_vDummyScoreInput);
        
        switch (score)
        {
            case EMiniGameScore.Bad:
                flow.SetValue(_vOutput, item.BadKey);
                break;
            case EMiniGameScore.Good:
                flow.SetValue(_vOutput, item.GoodKey);
                break;
            case EMiniGameScore.Perfect:
                flow.SetValue(_vOutput, item.PerfectKey);
                break;
            default:
                throw new ArgumentOutOfRangeException();
        }

        return _cOutput;
    }
}

public class DummyManager : MonoBehaviour
{

    [SerializeField] private List<string> _npcKeys;
    [SerializeField] private List<string> _enterScriptCodeList;
    [SerializeField] private List<DialougeDummyItem> _exitScriptCodeList;
    [SerializeField] private List<CocktailData> _requirementCocktailList;
    [SerializeField] private BarController _controller;

    [SerializeField] private int _iceBadScoreToNumber;
    [SerializeField] private int _iceGoodScoreToNumber;
    [SerializeField] private int _icePerfectScoreToNumber;

    [SerializeField] private float _iceScoreCoverage;

    [SerializeField] private int _measurementBadScoreToNumber;
    [SerializeField] private int _measurementGoodScoreToNumber;
    [SerializeField] private int _measurementPerfectScoreToNumber;

    [SerializeField] private float _measurementScoreCoverage;
    

    [SerializeField] private int _shakingBadScoreToNumber;
    [SerializeField] private int _shakingGoodScoreToNumber;
    [SerializeField] private int _shakingPerfectScoreToNumber;
    
    [SerializeField] private float _shakingScoreCoverage;

    [SerializeField] private Vector2Int _totalBadRange = new Vector2Int(-99999999, 50);
    [SerializeField] private Vector2Int _totalGoodRange = new Vector2Int(51, 84);
    [SerializeField] private Vector2Int _totalPerfectRange = new Vector2Int(85, 100);
    
    public int CurrentEnterScriptCodeIndex { get; private set; }
    public int CurrentExitScriptCodeIndex { get; private set; }

    public bool IsGotoContinueScript => CurrentExitScriptCodeIndex >= _exitScriptCodeList.Count;
    public bool IsGotoContinueNpc => _currentNpcKeyIndex >= _npcKeys.Count;

    private int _currentNpcKeyIndex;
    private int _currentCocktailIndex;

    public string GetCurrentEnterScriptCode()
    {
        string key = _enterScriptCodeList[CurrentEnterScriptCodeIndex++];
        
        CurrentEnterScriptCodeIndex = Mathf.Clamp(CurrentEnterScriptCodeIndex, 0, _enterScriptCodeList.Count - 1);
        
        return key;
    }
    
    public DialougeDummyItem GetCurrentExitScriptCode()
    {
        var key = _exitScriptCodeList[CurrentExitScriptCodeIndex++];
        
        return key;
    }

    public string GetCurrentNpcKey()
    {
        _currentCocktailIndex++;
        return _npcKeys[_currentNpcKeyIndex++];
    }

    public string GetCurrentNpcKeyNoShiftIndex()
    {
        return _npcKeys[_currentNpcKeyIndex];
    }

    public EMiniGameScore JugdgeCocktailScore()
    {
        var context = _controller.Context;

        Func<EMiniGameScore, float> iCalc = (score) =>
        {
            int number = 0;
            switch (score)
            {
                case EMiniGameScore.Bad:
                    number = _iceBadScoreToNumber;
                    break;
                case EMiniGameScore.Good:
                    number =  _iceGoodScoreToNumber;
                    break;
                case EMiniGameScore.Perfect:
                    number =  _icePerfectScoreToNumber;
                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }

            return number / 100f * _iceScoreCoverage;
        };
        Func<EMiniGameScore, float, float> mCalc = (score, coverage) =>
        {
            int number = 0;
            switch (score)
            {
                case EMiniGameScore.Bad:
                    number = _measurementBadScoreToNumber;
                    break;
                case EMiniGameScore.Good:
                    number =  _measurementGoodScoreToNumber;
                    break;
                case EMiniGameScore.Perfect:
                    number =  _measurementPerfectScoreToNumber;
                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }

            return number / 100f * coverage;
        };
        Func<EMiniGameScore, float, float> sCalc = (score, coverage) =>
        {
            int number = 0;
            switch (score)
            {
                case EMiniGameScore.Bad:
                    number = _shakingBadScoreToNumber;
                    break;
                case EMiniGameScore.Good:
                    number =  _shakingGoodScoreToNumber;
                    break;
                case EMiniGameScore.Perfect:
                    number =  _shakingPerfectScoreToNumber;
                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }

            return number / 100f * coverage;
        };
        

        float totalScore = 0f;

        totalScore += iCalc(context.IceScore);
        context.MeasuredDrinkTable.ForEach(pair => totalScore += mCalc(pair.Value.Score, _measurementScoreCoverage / context.MeasuredDrinkTable.Count));
        totalScore += sCalc(context.ShakeScore1, _shakingScoreCoverage / 2f);
        totalScore += sCalc(context.ShakeScore2, _shakingScoreCoverage / 2f);

        int finalScoreNumber = Mathf.CeilToInt(totalScore * 100f);
        finalScoreNumber = Mathf.Clamp(finalScoreNumber, 0, 100);
        
        print(finalScoreNumber);
        

        Debug.Assert(_controller.CurrentCocktailData);
        
        if (_controller.CurrentCocktailData != _requirementCocktailList[_currentCocktailIndex])
        {
            return EMiniGameScore.Bad;
        }

        if (_totalBadRange.x <= finalScoreNumber && finalScoreNumber <= _totalBadRange.y)
        {
            return EMiniGameScore.Bad;
        }
        if (_totalGoodRange.x <= finalScoreNumber && finalScoreNumber <= _totalGoodRange.y)
        {
            return EMiniGameScore.Good;
        }
        if (_totalPerfectRange.x <= finalScoreNumber && finalScoreNumber <= _totalPerfectRange.y)
        {
            return EMiniGameScore.Perfect;
        }
        
        return EMiniGameScore.Bad;
    }
}
