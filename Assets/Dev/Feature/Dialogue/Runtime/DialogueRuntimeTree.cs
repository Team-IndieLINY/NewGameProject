using System.Collections;
using System.Collections.Generic;
using System.Linq;
using DS.Core;
using DS.Runtime;
using UnityEngine;

namespace DS.Core
{
    public class DialogueRuntimeTree
    {
        public static DialogueRuntimeTree Build(DialogueContainer container)
        {
            List<DialogueNodeData> datas = container.NodeData;
            List<NodeLinkData> links = container.NodeLinks;

            var entryPointData = datas.First(x => x.GUID == links.First().TargetNodeGuid);
            var currentRuntimeNode = entryPointData.CreateRuntimeNode();

            Link(currentRuntimeNode, datas, links);

            var tree = new DialogueRuntimeTree();
            tree.EntryPoint = currentRuntimeNode;
            tree._currentNode = currentRuntimeNode;

            return tree;
        }

        private static void Link(DialogueRuntimeNode currentNode, List<DialogueNodeData> datas,
            List<NodeLinkData> links)
        {
            if (currentNode == null) return;

            foreach (var link in links)
            {
                if (link.BaseNodeGuid == currentNode.Data.GUID)
                {
                    var data = datas.First(x => x.GUID == link.TargetNodeGuid);
                    var node = data.CreateRuntimeNode();

                    currentNode.AddNext(node);

                    Link(node, datas, links);
                }
            }
        }


        public DialogueRuntimeNode EntryPoint { get; private set; }

        private DialogueRuntimeNode _currentNode;

        public bool IsEnd => _currentNode == null;

        public void Reset()
        {
            _currentNode = EntryPoint;
        }

        public DialogueItem NextItem()
        {
            if (_currentNode == null) return null;

            DialogueItem item = _currentNode.CreateItem();

            if (item != null)
            {
                _currentNode = _currentNode.GetNext();
                return item;
            }

            int infinityLoopCheckCounter = 0;
            while (item == null)
            {
                infinityLoopCheckCounter++;
                if (infinityLoopCheckCounter >= 1000)
                {
                    Debug.LogError("무한루프 탐지");
                    _currentNode = null;
                    return null;
                }

                if (_currentNode.IsLeaf)
                {
                    _currentNode = null;
                    return null;
                }

                _currentNode = _currentNode.GetNext();
                item = _currentNode.CreateItem();
            }

            _currentNode = _currentNode.GetNext();
            return item;
        }
    }
}