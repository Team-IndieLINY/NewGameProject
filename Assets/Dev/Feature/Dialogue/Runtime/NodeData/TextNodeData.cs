using System;
using DS.Core;
using UnityEngine;

namespace DS.Runtime
{
    [Serializable]
    public class TextNodeData : DialogueNodeDataT<TextRuntimeNode>
    {
        public string DialogueText;

        public override DialogueNodeData Clone()
        {
            var data = new TextNodeData();

            data.GUID = GUID;
            data.NodeTitle = NodeTitle;
            data.TypeName = TypeName;
            data.Position = Position;
            
            data.DialogueText = DialogueText;

            return data;
        }

        public override bool IsEqual(DialogueNodeData other)
        {
            if (!base.IsEqual(other))
                return false;

            var otherNode = other as TextNodeData;
            
            if (DialogueText != otherNode.DialogueText)
                return false;
            
            return true;
        }
    }

}