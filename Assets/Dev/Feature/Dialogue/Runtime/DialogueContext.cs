using System;
using System.Collections.Generic;
using System.Threading;
using Cysharp.Threading.Tasks;
using DS.Core;
using TMPro;
using UnityEngine;
using UnityEngine.UI;



[Serializable]
public class DialogueContext
{
    private DialogueRuntimeTree _tree;
    private float _duration;
    private Action<string> _textInput;
    private NpcController _controller;
    private DialogueView _view;

    private int _index;

    private CancellationTokenSource _source;

    public bool CanNext => _tree.IsEnd == false && _source != null;

    public string CurrentText { get; private set; }

    private UniTask _prevTask = UniTask.CompletedTask;

    public void Setup(NpcController npcController)
    {
        _controller = npcController;
    }

    public UniTask Next()
    {
        if (CanNext == false) return UniTask.CompletedTask;

        string str = string.Empty;
        if (_prevTask.Status == UniTaskStatus.Pending)
        {
            str = CurrentText;
        }
            
        _source.Cancel();
        _source = new CancellationTokenSource();

        if (string.IsNullOrEmpty(str) == false)
        {
            _textInput(str);
            return UniTask.CompletedTask;
        }

        var item = _tree.NextItem();
        if (item == null) return UniTask.CompletedTask;
        CurrentText = item.Text;

        _view.ArrowDirection = item.IsMaster ? DialogueView.EArrowDirection.Master : DialogueView.EArrowDirection.Npc;
        
        var task = TextUtil.DoTextUniTask(_textInput, CurrentText, _duration, _source.Token);
        _prevTask = task;

        if (_controller)
        {
            _controller.GetNpcAll().ForEach(x=>x.PlayAnimation(item.PoseKey));
        }

        return task;
    }

    public void Cancel()
    {
        _source?.Cancel();
        _source = null;
    }

    public DialogueContext(DialogueRuntimeTree tree, float duration, DialogueView view)
    {
        _tree = tree;
        _textInput = str => view.DialogueText = str;
        _duration = duration;
        _view = view;
        _source = new();
    }
}