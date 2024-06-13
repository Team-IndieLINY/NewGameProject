using System;
using UnityEditor.Experimental.GraphView;
using UnityEngine;

namespace DS.Editor
{
    public class EntryPointNode : DialogueNode
    {
        private readonly Vector2 NodeSize = new Vector2(100, 150);
        
        public EntryPointNode(string name)
        {
            title = name;
            NodeTitle = name;
            EntryPoint = true;
            GUID = Guid.NewGuid().ToString();
        }

        public void Build()
        {
            var outputPort = GeneratePort(Direction.Output);
            outputPort.portName = "Execute";
            outputContainer.Add(outputPort);

            capabilities &= ~Capabilities.Movable;
            capabilities &= ~Capabilities.Deletable;
            
            LoadStyleSheet();

            RefreshExpandedState(); 
            RefreshPorts();

            SetPosition(new Rect(new Vector2(100, 200),  NodeSize));
        }
    }
}