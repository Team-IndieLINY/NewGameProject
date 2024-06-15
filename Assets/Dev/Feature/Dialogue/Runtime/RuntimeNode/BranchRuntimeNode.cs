using DS.Core;

namespace DS.Runtime
{
    public class BranchRuntimeNode : DialogueRuntimeNodeT<BranchNodeData>
    {

        public override DialogueItem CreateItem()
            => null;

        public override DialogueRuntimeNode GetNext()
            => FirstNode;
    }
}