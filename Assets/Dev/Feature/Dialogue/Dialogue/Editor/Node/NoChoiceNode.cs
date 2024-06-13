using System;
using System.Globalization;
using System.Linq;
using UnityEditor.Experimental.GraphView;
using UnityEditor.UIElements;
using UnityEngine;
using UnityEngine.UIElements;

namespace DS.Editor
{
    public class NoChoiceNode : DialogueNode
    {
        private readonly Vector2 NodeSize = new Vector2(150, 200);
        private readonly DialogueGraphView _dialogueGraphView;

        #region Main Container

        private TextField _targetObjectIDTextField;
        private string _targetObjectID;
        public string TargetObjectID
        {
            get => _targetObjectID;
            set
            {
                _targetObjectID = value;
                if (_targetObjectIDTextField != null)
                    _targetObjectIDTextField.SetValueWithoutNotify(value);
            }
        }
        
        private TextField _dialogueTextField;
        private string _dialogueText;
        public string DialogueText
        {
            get => _dialogueText;
            set
            {
                _dialogueText = value;
                if (_dialogueTextField != null)
                    _dialogueTextField.SetValueWithoutNotify(value);
            }
        }
        
        private FloatField _skipDelayTextField;
        private float _skipDelay;
        public float SkipDelay
        {
            get => _skipDelay;
            set
            {
                _skipDelay = value;
                if (_skipDelayTextField != null)
                    _skipDelayTextField.SetValueWithoutNotify(value);
            }
        }

        #endregion

        public NoChoiceNode(DialogueGraphView dialogueGraphView, string name)
        {
            title = name;
            NodeTitle = name;
            NodeType = NodeTypes.NodeType.NoChoice;
            GUID = Guid.NewGuid().ToString();
            
            _dialogueGraphView = dialogueGraphView;
        }

        public void Build(Vector2 position)
        {
            var inputPort = GeneratePort(Direction.Input, Port.Capacity.Multi);
            inputPort.portName = "Connection";
            inputContainer.Add(inputPort);
            
            var outputPort = GeneratePort(Direction.Output);
            outputPort.portName = "Next";
            outputContainer.Add(outputPort);
            
            LoadStyleSheet();

            AddTitleTextField();
            AddTargetObjectIDTextField();
            AddDialogueTextField();
            AddSkipDelayTextField();

            RefreshExpandedState();
            RefreshPorts();
            
            SetPosition(new Rect(position, NodeSize));
        }

        private void AddTitleTextField()
        {
            RemoveLabel(titleContainer, "title-label");
            
            var textField = new TextField(string.Empty);
            textField.RegisterValueChangedCallback(evt =>
            {
                title = evt.newValue;
                NodeTitle = evt.newValue;
            });
            textField.SetValueWithoutNotify(title);
            titleContainer.Insert(0, textField);
        }

        private void AddTargetObjectIDTextField()
        {
            _targetObjectIDTextField = new TextField("Target Object ID");
            _targetObjectIDTextField.RegisterValueChangedCallback(evt => 
            {
                TargetObjectID = evt.newValue;
            });
            _targetObjectIDTextField.SetValueWithoutNotify(TargetObjectID ?? string.Empty);
            _targetObjectIDTextField.AddToClassList("TargetObjectID-textfield");
            mainContainer.Add(_targetObjectIDTextField);
        }
        
        private void AddDialogueTextField()
        {
            _dialogueTextField = new TextField("Dialogue Text")
            {
                multiline = true
            };
            _dialogueTextField.RegisterValueChangedCallback(evt => 
            {
                DialogueText = evt.newValue;
            });
            _dialogueTextField.SetValueWithoutNotify(DialogueText ?? string.Empty);
            _dialogueTextField.AddToClassList("DialogueText-textfield");
            mainContainer.Add(_dialogueTextField);
        }
        
        private void AddSkipDelayTextField()
        {
            _skipDelayTextField = new FloatField("Skip Delay");
            _skipDelayTextField.RegisterValueChangedCallback(evt => 
            {
                SkipDelay = evt.newValue;
            });
            _skipDelayTextField.SetValueWithoutNotify(_skipDelay);
            _skipDelayTextField.AddToClassList("SkipDelay-textfield");
            mainContainer.Add(_skipDelayTextField);
        }
    }
}
