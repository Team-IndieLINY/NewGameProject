using System;
using DS.Core;

namespace DS.Runtime
{
    [Serializable]
    public class BranchNodeData : DialogueNodeDataT<BranchRuntimeNode>
    {
        public override DialogueNodeData Clone()
        {
            var data = new BranchNodeData();

            data.GUID = GUID;
            data.NodeTitle = NodeTitle;
            data.TypeName = TypeName;
            data.Position = Position;
            
            return data;
        }

        public override bool IsEqual(DialogueNodeData other)
        {
            if (!base.IsEqual(other))
                return false;

            var otherNode = other as BranchNodeData;
            
            return true;
        }
    }
}