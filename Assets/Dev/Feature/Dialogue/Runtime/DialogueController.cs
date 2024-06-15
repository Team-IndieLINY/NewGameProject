using System;
using System.Collections;
using System.Collections.Generic;
using DS.Core;
using JetBrains.Annotations;
using UnityEngine;



public class DialogueController : MonoBehaviour
{
    [SerializeField] private DialogueView _view;
    
    [SerializeField] private float _textCompletionDuration = 0.35f;
    
    private DialogueContext LastestContext { get; set; }

    public bool Visible
    {
        get => _view.Visible;
        set => _view.Visible = value;
    }

    private void Awake()
    {
        ResetDialogue();
    }

    public void ResetDialogue()
    {
        _view.DialogueText = "";
        _view.Visible = false;

        LastestContext?.Cancel();
        LastestContext = null;
    }

    public DialogueContext CreateContext(DialogueContainer container)
        => CreateContext(DialogueRuntimeTree.Build(container));

    public DialogueContext CreateContext(DialogueRuntimeTree tree)
    {
        // 현재 Context가 누군가에게 점유되어 있으면 null 반환
        if (LastestContext != null && LastestContext.CanNext)
        {
            return null;
        }
        
        var context = new DialogueContext(
            tree,
            _textCompletionDuration,
            _view
        );

        LastestContext = context;
        return context;
    }
}

