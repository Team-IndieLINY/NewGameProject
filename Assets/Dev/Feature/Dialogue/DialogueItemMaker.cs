using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;


public class DialogueItemMaker : Unit
{
    [DoNotSerialize] [PortLabelHidden] public ControlInput _cInput { get; private set; }
    [DoNotSerialize] [PortLabelHidden] public ControlOutput _cOutput { get; private set; }
    
    [DoNotSerialize] [PortLabelHidden] public ValueInput TextList { get; private set; }
    [DoNotSerialize] [PortLabelHidden] public ValueInput MasterList { get; private set; }
    [DoNotSerialize] [PortLabelHidden] public ValueOutput Result { get; private set; }

    private List<DialogueItem> _result;
    
    protected override void Definition()
    {
        _cInput = ControlInput("", Make);
        _cOutput = ControlOutput("");

        TextList = ValueInput<List<string>>("text");
        MasterList = ValueInput<List<bool>>("is master");
        Result = ValueOutput<List<DialogueItem>>("items", (x)=>_result);
    }

    private ControlOutput Make(Flow flow)
    {
        var texts = flow.GetValue<List<string>>(TextList);
        var masters = flow.GetValue<List<bool>>(MasterList);

        var result = new List<DialogueItem>(texts.Count);

        for (int i = 0; i < Mathf.Min(texts.Count, masters.Count); i++)
        {
            result.Add(new DialogueItem(texts[i], masters[i], ""));
        }

        _result = result;

        return _cOutput;
    }
}