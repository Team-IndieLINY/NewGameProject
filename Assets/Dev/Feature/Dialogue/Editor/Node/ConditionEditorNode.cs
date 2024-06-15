using System;
using System.Globalization;
using System.Linq;
using DS.Runtime;
using UnityEditor.Experimental.GraphView;
using UnityEditor.UIElements;
using UnityEngine;
using UnityEngine.UIElements;
using DS.Core;

namespace DS.Editor
{
    [DialogueNodeType("Add Condition", "Condition")]
    public class ConditionEditorNode : DialogueEditorNode
    {
        private readonly Vector2 NodeSize = new Vector2(150, 200);
        private readonly DialogueGraphView _dialogueGraphView;

        #region Main Container

        private ConditionNodeData _data= new();

        public override DialogueNodeData ToSerializedData()
        {
            Position = GetPosition().position;
            return _data;
        }

        private TextField _itemIDField;
        public string ItemID
        {
            get => _data.ItemID;
            set
            {
                _data.ItemID = value;
                if (_itemIDField != null)
                    _itemIDField.SetValueWithoutNotify(value);
            }
        }

        public override string GUID
        {
            get => _data.GUID;
            set => _data.GUID = value;
        }
        public override string NodeTitle 
        {
            get => _data.NodeTitle;
            set => _data.NodeTitle = value;
        }
        public override string TypeName 
        {
            get => _data.TypeName;
            set => _data.TypeName = value;
        }
        public override Vector2 Position 
        {
            get => _data.Position;
            set => _data.Position = value;
        }

        private IntegerField _equalOrManyField;
        public int EqualOrMany
        {
            get => _data.EqualOrMany;
            set
            {
                _data.EqualOrMany = value;
                if (_equalOrManyField != null)
                    _equalOrManyField.SetValueWithoutNotify(value);
            }
        }

        #endregion

        public ConditionEditorNode(DialogueGraphView dialogueGraphView, string name)
        {
            title = name;
            NodeTitle = name;
            TypeName = name;
            GUID = Guid.NewGuid().ToString();
            
            _dialogueGraphView = dialogueGraphView;
        }

        //TODO: withoutOutput 어떤 기능인지 파악 후 처리하기
        public override void Initialize(Vector2 position/**, bool withoutOutput = false*/)
        {
            var inputPort = GeneratePort(Direction.Input, Port.Capacity.Multi);
            inputPort.portName = "Connection";
            inputContainer.Add(inputPort);
            
            if (true)
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
            
            Position = position;
            _dialogueGraphView.AddElement(this);
            SetPosition(new Rect(position, NodeSize));
        }

        public override void FromSerializedData(DialogueNodeData data, DialogueContainer containerCache)
        {
            if (data is not ConditionNodeData myData) return;
            
            _data = myData;
            
            _itemIDField.value = _data.ItemID;
            _equalOrManyField.value = _data.EqualOrMany;
            
            _dialogueGraphView.AddElement(this);
            
            var nodePorts = containerCache.NodeLinks.Where(x => x.BaseNodeGuid == data.GUID).ToList();
            nodePorts.ForEach(x => this.AddConditionPort(x.PortName));
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
