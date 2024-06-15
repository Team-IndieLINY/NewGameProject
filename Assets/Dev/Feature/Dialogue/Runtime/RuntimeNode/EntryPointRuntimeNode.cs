using System.Collections;
using System.Collections.Generic;
using DS.Core;
using JetBrains.Annotations;
using UnityEngine;


namespace DS.Runtime
{
    public class EntryPointRuntimeNode : DialogueRuntimeNode
    {
        public EntryPointRuntimeNode(DialogueNodeData data, params DialogueRuntimeNode[] nexts) : base(data, nexts)
        {
        }

        public override DialogueItem CreateItem()
            => DialogueItem.Default;

        public override DialogueRuntimeNode GetNext()
            => FirstNode;
    }
}