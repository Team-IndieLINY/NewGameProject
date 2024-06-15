using System;
using DS.Core;

namespace DS.Runtime
{
    
    [Serializable]
    public class ConditionNodeData : DialogueNodeDataT<ConditionRuntimeNode>
    {
        public string ItemID;
        public int EqualOrMany;

        public override DialogueNodeData Clone()
        {
            var data = new ConditionNodeData();

            data.GUID = GUID;
            data.NodeTitle = NodeTitle;
            data.TypeName = TypeName;
            data.Position = Position;

            data.ItemID = ItemID;
            data.EqualOrMany = EqualOrMany;

            return data;
        }

        public override bool IsEqual(DialogueNodeData other)
        {
            if (!base.IsEqual(other))
                return false;

            var otherNode = other as ConditionNodeData;
            
            if (ItemID != otherNode.ItemID)
                return false;
            
            if (EqualOrMany != otherNode.EqualOrMany)
                return false;

            return true;
        }
    }
}