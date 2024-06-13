using System;
using System.Globalization;
using System.Linq;
using UnityEditor.Experimental.GraphView;
using UnityEditor.UIElements;
using UnityEngine;
using UnityEngine.UIElements;

namespace DS.Editor
{
    public class ConditionNode : DialogueNode
    {
        private readonly Vector2 NodeSize = new Vector2(150, 200);
        private readonly DialogueGraphView _dialogueGraphView;

        #region Main Container

        private TextField _itemIDField;
        private string _itemID;
        public string ItemID
        {
            get => _itemID;
            set
            {
                _itemID = value;
                if (_itemIDField != null)
                    _itemIDField.SetValueWithoutNotify(value);
            }
        }

        private IntegerField _equalOrManyField;
        private int _equalOrMany;
        public int EqualOrMany
        {
            get => _equalOrMany;
            set
            {
                _equalOrMany = value;
                if (_equalOrManyField != null)
                    _equalOrManyField.SetValueWithoutNotify(value);
            }
        }

        #endregion

        public ConditionNode(DialogueGraphView dialogueGraphView, string name)
        {
            title = name;
            NodeTitle = name;
            NodeType = NodeTypes.NodeType.Condition;
            GUID = Guid.NewGuid().ToString();
            
            _dialogueGraphView = dialogueGraphView;
        }

        public void Build(Vector2 position, bool withoutOutput = false)
        {
            var inputPort = GeneratePort(Direction.Input, Port.Capacity.Multi);
            inputPort.portName = "Connection";
            inputContainer.Add(inputPort);
            
            if (!withoutOutput)
            {
                var outputTruePort = GeneratePort(Direction.Output);
                outputTruePort.portName = "True";
                outputContainer.Add(outputTruePort);
                
                var outputFalsePort = GeneratePort(Direction.Output);
                outputFalsePort.portName = "False";
                outputContainer.Add(outputFalsePort);
            }
            
            LoadStyleSheet();

            AddTitleTextField();
            AddItemIDField();
            AddEqualOrManyField();

            RefreshExpandedState();
            RefreshPorts();
            
            SetPosition(new Rect(position, NodeSize));
        }
        
        public void AddConditionPort(string portName)
        {
            var generatedPort = GeneratePort(Direction.Output);

            generatedPort.portName = portName;
            outputContainer.Add(generatedPort);
            
            RefreshExpandedState();
            RefreshPorts();
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

        private void AddItemIDField()
        {
            _itemIDField = new TextField("Item ID");
            _itemIDField.RegisterValueChangedCallback(evt => 
            {
                ItemID = evt.newValue;
            });
            _itemIDField.SetValueWithoutNotify(ItemID ?? string.Empty);
            _itemIDField.AddToClassList("ItemID-textfield");
            mainContainer.Add(_itemIDField);
        }

        private void AddEqualOrManyField()
        {
            _equalOrManyField = new IntegerField("Equal Or Many");
            _equalOrManyField.RegisterValueChangedCallback(evt => 
            {
                EqualOrMany = evt.newValue;
            });
            _equalOrManyField.SetValueWithoutNotify(EqualOrMany);
            _equalOrManyField.AddToClassList("EqualOrMany-textfield");
            mainContainer.Add(_equalOrManyField);
        }
    }
}
