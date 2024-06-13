using System.Collections.Generic;
using System.Linq;
using DS.Runtime;
using UnityEditor;
using UnityEditor.Experimental.GraphView;
using UnityEngine;
using UnityEngine.UIElements;
using Edge = UnityEditor.Experimental.GraphView.Edge;

namespace DS.Editor
{
    public class GraphSaveUtility
    {
        private DialogueGraphView _targetGraphView;
        private DialogueContainer _containerCache;

        private List<Edge> Edges => _targetGraphView.edges.ToList();
        private List<DialogueNode> Nodes => _targetGraphView.nodes.ToList().Cast<DialogueNode>().ToList();

        public static GraphSaveUtility GetInstance(DialogueGraphView targetGraphView)
        {
            return new GraphSaveUtility
            {
                _targetGraphView = targetGraphView
            };
        }

        public bool CheckAnyChange(string fullPath)
        {
            var dialogueContainer = ScriptableObject.CreateInstance<DialogueContainer>();

            SaveNodes(dialogueContainer);
            SaveLinks(dialogueContainer);

            string relativePath = ConvertFullToRelativePath(fullPath);
            _containerCache = AssetDatabase.LoadAssetAtPath<DialogueContainer>(relativePath);

            if (_containerCache == null)
                return true;

            return !dialogueContainer.IsEqual(_containerCache);
        }

        public void SaveGraph(string fullPath)
        {
            string relativePath = ConvertFullToRelativePath(fullPath);
            
            if (string.IsNullOrEmpty(relativePath))
                return;

            DialogueContainer dialogueContainer = AssetDatabase.LoadAssetAtPath<DialogueContainer>(relativePath);
            
            if (dialogueContainer == null)
            {
                dialogueContainer = ScriptableObject.CreateInstance<DialogueContainer>();
                AssetDatabase.CreateAsset(dialogueContainer, relativePath);
            }

            ResetContainer(dialogueContainer);
            SaveNodes(dialogueContainer);
            SaveLinks(dialogueContainer);

            EditorUtility.SetDirty(dialogueContainer);
            AssetDatabase.SaveAssets();
        }

        private void ResetContainer(DialogueContainer container)
        {
            container.NodeData = new List<DialogueNodeData>();
            container.NodeLinks = new List<NodeLinkData>();
        }

        private void SaveLinks(DialogueContainer container)
        {
            var connectedPorts = Edges.Where(x => x.input.node != null).ToArray();

            var entryNode = Nodes.FirstOrDefault(node => node.EntryPoint);
            container.NodeLinks.Add(new NodeLinkData
            {
                BaseNodeGuid = entryNode.GUID
            });
              
            foreach (var port in connectedPorts)
            {
                var outputNode = port.output.node as DialogueNode;
                var inputNode = port.input.node as DialogueNode;

                if (outputNode.GUID == entryNode.GUID)
                {
                    container.NodeLinks[0].PortName = port.output.portName;
                    container.NodeLinks[0].TargetNodeGuid = inputNode.GUID;
                    continue;
                }

                container.NodeLinks.Add(new NodeLinkData
                {
                    BaseNodeGuid = outputNode.GUID,
                    PortName = port.output.portName,
                    TargetNodeGuid = inputNode.GUID
                });
            }
        }

        private void SaveNodes(DialogueContainer container)
        {
            foreach (var dialogueNode in Nodes.Where(node => !node.EntryPoint))
            {
                switch (dialogueNode.NodeType)
                {
                    case NodeTypes.NodeType.NoChoice:
                        var noChoiceNode = dialogueNode as NoChoiceNode;
                        container.NodeData.Add(new NoChoiceNodeData
                        {
                            GUID = noChoiceNode.GUID,
                            NodeTitle = noChoiceNode.NodeTitle,
                            NodeType = noChoiceNode.NodeType,
                            Position = noChoiceNode.GetPosition().position,
                            TargetObjectID = noChoiceNode.TargetObjectID,
                            DialogueText = noChoiceNode.DialogueText,
                            SkipDelay = noChoiceNode.SkipDelay
                        });
                        break;
                    case NodeTypes.NodeType.MultiChoice:
                        var multiChoiceNode = dialogueNode as MultiChoiceNode;
                        container.NodeData.Add(new MultiChoiceNodeData
                        {
                            GUID = multiChoiceNode.GUID,
                            NodeTitle = multiChoiceNode.NodeTitle,
                            NodeType = multiChoiceNode.NodeType,
                            Position = multiChoiceNode.GetPosition().position,
                            TargetObjectID = multiChoiceNode.TargetObjectID,
                            DialogueText = multiChoiceNode.DialogueText
                        });
                        break;
                    case NodeTypes.NodeType.StartQuest:
                        var startQuestNode = dialogueNode as StartQuestNode;
                        container.NodeData.Add(new StartQuestNodeData
                        {
                            GUID = startQuestNode.GUID,
                            NodeTitle = startQuestNode.NodeTitle,
                            NodeType = startQuestNode.NodeType,
                            Position = startQuestNode.GetPosition().position,
                            QuestType = startQuestNode.QuestType,
                            QuestID = startQuestNode.QuestID
                        });
                        break;
                    case NodeTypes.NodeType.AddItem:
                        var addItemNode = dialogueNode as AddItemNode;
                        container.NodeData.Add(new AddItemNodeData
                        {
                            GUID = addItemNode.GUID,
                            NodeTitle = addItemNode.NodeTitle,
                            NodeType = addItemNode.NodeType,
                            Position = addItemNode.GetPosition().position,
                            ItemID = addItemNode.ItemID,
                            ItemCount = addItemNode.ItemCount
                        });
                        break;
                    case NodeTypes.NodeType.Condition:
                        var conditionNode = dialogueNode as ConditionNode;
                        container.NodeData.Add(new ConditionNodeData
                        {
                            GUID = conditionNode.GUID,
                            NodeTitle = conditionNode.NodeTitle,
                            NodeType = conditionNode.NodeType,
                            Position = conditionNode.GetPosition().position,
                            ItemID = conditionNode.ItemID,
                            EqualOrMany = conditionNode.EqualOrMany
                        });
                        break;
                }
            }
        }

        public void LoadGraph(string fullPath)
        {
            string relativePath = ConvertFullToRelativePath(fullPath);
            _containerCache = AssetDatabase.LoadAssetAtPath<DialogueContainer>(relativePath);

            if (_containerCache == null)
            {
                EditorUtility.DisplayDialog("File Not Found", "Target dialogue graph file does not exist!\n\nPath: " + relativePath, "OK");
                return;
            }
            
            ClearGraph();
            CreateNodes();
            ConnectNodes();
        }
        
        private string ConvertFullToRelativePath(string fullPath)
        {
            string projectPath = Application.dataPath;
            if (fullPath.StartsWith(projectPath))
            {
                string relativePath = "Assets" + fullPath.Substring(projectPath.Length);
                return relativePath;
            }

            EditorUtility.DisplayDialog("Outside of the Project", "File path is outside of the project: " + fullPath, "OK");
            return "";
        }

        public void ClearGraph()
        {
            foreach (var node in Nodes)
            {
                if (node.EntryPoint)
                    continue;
                
                Edges.Where(x => x.input.node == node).ToList()
                    .ForEach(edge => _targetGraphView.RemoveElement(edge));
                
                _targetGraphView.RemoveElement(node);
            }
        }

        private void CreateNodes()
        {
            if (_containerCache.NodeLinks.Count > 0)
            {
                Nodes.Find(x => x.EntryPoint).GUID = _containerCache.NodeLinks[0].BaseNodeGuid;
            }
            
            foreach (var nodeData in _containerCache.NodeData)
            {
                switch (nodeData.NodeType)
                {
                    case NodeTypes.NodeType.NoChoice:
                        CreateNoChoiceNode(nodeData as NoChoiceNodeData);
                        break;
                    case NodeTypes.NodeType.MultiChoice:
                        CreateMultiChoiceNode(nodeData as MultiChoiceNodeData);
                        break;
                    case NodeTypes.NodeType.StartQuest:
                        CreateStartQuestNode(nodeData as StartQuestNodeData);
                        break;
                    case NodeTypes.NodeType.AddItem:
                        CreateAddItemNode(nodeData as AddItemNodeData);
                        break;
                    case NodeTypes.NodeType.Condition:
                        CreateConditionNode(nodeData as ConditionNodeData);
                        break;
                }
            }
        }

        private void CreateNoChoiceNode(NoChoiceNodeData nodeData)
        {
            var tempNode = _targetGraphView.CreateNoChoiceNode(nodeData.NodeTitle, nodeData.Position) as NoChoiceNode;
            tempNode.GUID = nodeData.GUID;
            tempNode.TargetObjectID = nodeData.TargetObjectID;
            tempNode.DialogueText = nodeData.DialogueText;
            tempNode.SkipDelay = nodeData.SkipDelay;
            _targetGraphView.AddElement(tempNode);
        }

        private void CreateMultiChoiceNode(MultiChoiceNodeData nodeData)
        {
            var tempNode = _targetGraphView.CreateMultiChoiceNode(nodeData.NodeTitle, nodeData.Position) as MultiChoiceNode;
            tempNode.GUID = nodeData.GUID;
            tempNode.TargetObjectID = nodeData.TargetObjectID;
            tempNode.DialogueText = nodeData.DialogueText;
            _targetGraphView.AddElement(tempNode);

            var nodePorts = _containerCache.NodeLinks.Where(x => x.BaseNodeGuid == nodeData.GUID).ToList();
            nodePorts.ForEach(x => tempNode.AddChoicePort(x.PortName));
        }
        
        private void CreateStartQuestNode(StartQuestNodeData nodeData)
        {
            var tempNode = _targetGraphView.CreateStartQuestNode(nodeData.NodeTitle, nodeData.Position) as StartQuestNode;
            tempNode.GUID = nodeData.GUID;
            tempNode.QuestType = nodeData.QuestType;
            tempNode.QuestID = nodeData.QuestID;
            _targetGraphView.AddElement(tempNode);
        }
        
        private void CreateAddItemNode(AddItemNodeData nodeData)
        {
            var tempNode = _targetGraphView.CreateAddItemNode(nodeData.NodeTitle, nodeData.Position) as AddItemNode;
            tempNode.GUID = nodeData.GUID;
            tempNode.ItemID = nodeData.ItemID;
            tempNode.ItemCount = nodeData.ItemCount;
            _targetGraphView.AddElement(tempNode);
        }
        
        private void CreateConditionNode(ConditionNodeData nodeData)
        {
            var tempNode = _targetGraphView.CreateConditionNode(nodeData.NodeTitle, nodeData.Position, true) as ConditionNode;
            tempNode.GUID = nodeData.GUID;
            tempNode.ItemID = nodeData.ItemID;
            tempNode.EqualOrMany = nodeData.EqualOrMany;
            _targetGraphView.AddElement(tempNode);
            
            var nodePorts = _containerCache.NodeLinks.Where(x => x.BaseNodeGuid == nodeData.GUID).ToList();
            nodePorts.ForEach(x => tempNode.AddConditionPort(x.PortName));
        }

        private void ConnectNodes()
        {
            foreach (var node in Nodes)
            {
                var connections = _containerCache.NodeLinks.Where(x => x.BaseNodeGuid == node.GUID).ToList();
                for (int j = 0; j < connections.Count; j++)
                {
                    var targetNodeGuid = connections[j].TargetNodeGuid;
                    var targetNode = Nodes.First(x => x.GUID == targetNodeGuid);
                    LinkNodes(node.outputContainer[j].Q<Port>(), (Port)targetNode.inputContainer[0]);
                    
                    targetNode.SetPosition(new Rect(
                        _containerCache.NodeData.First(x => x.GUID == targetNodeGuid).Position,
                        _targetGraphView.DefaultNodeSize
                    ));
                }
            }
        }

        private void LinkNodes(Port output, Port input)
        {
            var tempEdge = new Edge
            {
                output = output,
                input = input
            };
            tempEdge?.input.Connect(tempEdge);
            tempEdge?.output.Connect(tempEdge);
            
            _targetGraphView.Add(tempEdge);
        }
    }
}
