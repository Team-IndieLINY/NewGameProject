using DS.Core;

namespace DS.Runtime
{
    public class TextRuntimeNode : DialogueRuntimeNodeT<TextNodeData>
    {
        

        public override DialogueItem CreateItem()
        {
            return new DialogueItem(MyData.DialogueText, false, "default", "default");
        }

        public override DialogueRuntimeNode GetNext()
            => FirstNode;
    }
}