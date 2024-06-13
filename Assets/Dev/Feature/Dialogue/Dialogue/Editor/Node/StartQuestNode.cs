using System;
using System.Linq;
using UnityEditor.Experimental.GraphView;
using UnityEngine;
using UnityEngine.UIElements;

namespace DS.Editor
{
    public class StartQuestNode : DialogueNode
    {
        private readonly Vector2 NodeSize = new Vector2(150, 200);
        private readonly DialogueGraphView _dialogueGraphView;

        #region Main Container
        
        private TextField _questTypeTextField;
        private string _questType;
        public string QuestType
        {
            get => _questType;
            set
            {
                _questType = value;
                if (_questTypeTextField != null)
                    _questTypeTextField.SetValueWithoutNotify(value);
            }
        }

        private TextField _questIDTextField;
        private string _questID;
        public string QuestID
        {
            get => _questID;
            set
            {
                _questID = value;
                if (_questIDTextField != null)
                    _questIDTextField.SetValueWithoutNotify(value);
            }
        }

        #endregion

        public StartQuestNode(DialogueGraphView dialogueGraphView, string name)
        {
            title = name;
            NodeTitle = name;
            NodeType = NodeTypes.NodeType.StartQuest;
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
            AddQuestTypeTextField();
            AddQuestIDTextField();

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

        private void AddQuestTypeTextField()
        {
            _questTypeTextField = new TextField("Quest Type");
            _questTypeTextField.RegisterValueChangedCallback(evt => 
            {
                QuestType = evt.newValue;
            });
            _questTypeTextField.SetValueWithoutNotify(QuestType ?? string.Empty);
            _questTypeTextField.AddToClassList("QuestType-textfield");
            mainContainer.Add(_questTypeTextField);
        }

        private void AddQuestIDTextField()
        {
            _questIDTextField = new TextField("Quest ID");
            _questIDTextField.RegisterValueChangedCallback(evt => 
            {
                QuestID = evt.newValue;
            });
            _questIDTextField.SetValueWithoutNotify(QuestID ?? string.Empty);
            _questIDTextField.AddToClassList("QuestID-textfield");
            mainContainer.Add(_questIDTextField);
        }
    }
}