using System;
using System.Linq;
using UnityEditor.Experimental.GraphView;
using UnityEngine;
using UnityEngine.UIElements;

namespace DS.Editor
{
    public class MultiChoiceNode : DialogueNode
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
        
        #endregion

        public MultiChoiceNode(DialogueGraphView dialogueGraphView, string name)
        {
            title = name;
            NodeTitle = name;
            NodeType = NodeTypes.NodeType.MultiChoice;
            GUID = Guid.NewGuid().ToString();
            
            _dialogueGraphView = dialogueGraphView;
        }

        public void Build(Vector2 position)
        {
            var inputPort = GeneratePort(Direction.Input, Port.Capacity.Multi);
            inputPort.portName = "Connection";
            inputContainer.Add(inputPort);
            
            LoadStyleSheet();

            AddTitleTextField();
            AddNewChoiceButton();
            AddTargetObjectIDTextField();
            AddDialogueTextField();

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
            textField.AddToClassList("title-textfield");
            titleContainer.Insert(0, textField);
        }
        
        private void AddNewChoiceButton()
        {
            var button = new Button(() => { AddChoicePort(); }) { text = "+" };
            button.AddToClassList("choice-button");
            titleContainer.Add(button);
        }
        
        public void AddChoicePort(string overriddenPortName = "")
        {
            var generatedPort = GeneratePort(Direction.Output);
            RemoveLabel(generatedPort.contentContainer, "type");

            var choicePortName = GenerateChoicePortName(overriddenPortName);
            ConfigureTextFieldForPort(generatedPort, choicePortName);
            AddDeleteButtonToPort(generatedPort);

            generatedPort.portName = choicePortName;
            outputContainer.Add(generatedPort);
            
            RefreshExpandedState();
            RefreshPorts();
        }
        
        private string GenerateChoicePortName(string overriddenPortName)
        {
            var outputPortCount = outputContainer.Query("connector").ToList().Count;
            return string.IsNullOrEmpty(overriddenPortName)
                ? $"Choice {outputPortCount + 1}"
                : overriddenPortName;
        }

        private static void ConfigureTextFieldForPort(Port generatedPort, string portName)
        {
            var textField = new TextField { name = string.Empty, value = portName };
            textField.RegisterValueChangedCallback(evt => generatedPort.portName = evt.newValue);
            textField.AddToClassList("output-textfield");
            generatedPort.contentContainer.Add(new Label("    "));
            generatedPort.contentContainer.Add(textField);
        }

        private void AddDeleteButtonToPort(Port generatedPort)
        {
            var deleteButton = new Button(() => RemovePort(generatedPort)) { text = "X" };
            deleteButton.AddToClassList("delete-choice-button");
            generatedPort.contentContainer.Add(deleteButton);
        }

        private void RemovePort(Port generatedPort)
        {
            var targetEdge = _dialogueGraphView.edges.ToList().FirstOrDefault(x =>
                x.output.portName == generatedPort.portName && x.output.node == generatedPort.node);

            if (targetEdge != null)
            {
                targetEdge.input.Disconnect(targetEdge);
                _dialogueGraphView.RemoveElement(targetEdge);
            }

            outputContainer.Remove(generatedPort);
            RefreshPorts();
            RefreshExpandedState();
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
    }
}
