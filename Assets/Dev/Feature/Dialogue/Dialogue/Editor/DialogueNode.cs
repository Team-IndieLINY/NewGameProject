using UnityEditor;
using UnityEditor.Experimental.GraphView;
using UnityEngine;
using UnityEngine.UIElements;

namespace DS.Editor
{
    public class DialogueNode : Node
    {
        public string GUID;
        public string NodeTitle;
        public NodeTypes.NodeType NodeType;
        public bool EntryPoint = false;
        
        protected void LoadStyleSheet()
        {
            var styleSheet =
                AssetDatabase.LoadAssetAtPath<StyleSheet>(DialogueEditorResourcesPath.DIALOGUE_NODE_STYLESHEET_PATH);
            
            styleSheets.Add(styleSheet);
        }

        protected static void RemoveLabel(VisualElement content, string labelName)
        {
            var oldLabel = content.Q<Label>(labelName);
            if (oldLabel != null)
            {
                content.Remove(oldLabel);
            }
        }

        protected Port GeneratePort(Direction portDirection, Port.Capacity capacity = Port.Capacity.Single)
        {
            return InstantiatePort(Orientation.Horizontal, portDirection, capacity, typeof(float));
        }
    }
}