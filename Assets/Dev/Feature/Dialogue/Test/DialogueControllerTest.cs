using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using DS.Core;
using DS.Runtime;
using MyBox;
using UnityEngine;

public class DialogueControllerTest : MonoBehaviour
{
    public DialogueController Controller;
    public DialogueContainer Container;

    private DialogueContext _context;
    
    private void CreateContext()
    {
        if (Application.isPlaying == false) return;
        if (Controller == false) return;

        _context = Controller.CreateContext(DialogueRuntimeTree.Build(Container));
    }

    private void Start()
    {
        Controller.Visible = true;
        CreateContext();
    }

    private void Update()
    {
        if (_context == null) return;

        if (InputManager.Actions.DialogueSkip.triggered)
        {
            _context.Next();
            if (_context.CanNext == false)
            {
                _context = null;
                CreateContext();
            }
        }
    }
}
