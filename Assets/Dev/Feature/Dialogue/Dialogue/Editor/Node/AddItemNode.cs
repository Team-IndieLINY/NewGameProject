using System;
using System.Globalization;
using System.Linq;
using UnityEditor.Experimental.GraphView;
using UnityEditor.UIElements;
using UnityEngine;
using UnityEngine.UIElements;

namespace DS.Editor
{
    public class AddItemNode : DialogueNode
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

        private IntegerField _itemCountField;
        private int _itemCount;
        public int ItemCount
        {
            get => _itemCount;
            set
            {
                _itemCount = value;
                if (_itemCountField != null)
                    _itemCountField.SetValueWithoutNotify(value);
            }
        }

        #endregion

        public AddItemNode(DialogueGraphView dialogueGraphView, string name)
        {
            title = name;
            NodeTitle = name;
            NodeType = NodeTypes.NodeType.AddItem;
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
            AddItemIDField();
            AddItemCountField();

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

        private void AddItemCountField()
        {
            _itemCountField = new IntegerField("Item Count");
            _itemCountField.RegisterValueChangedCallback(evt => 
            {
                ItemCount = evt.newValue;
            });
            _itemCountField.SetValueWithoutNotify(ItemCount);
            _itemCountField.AddToClassList("SkipDelay-textfield");
            mainContainer.Add(_itemCountField);
        }
    }
}
