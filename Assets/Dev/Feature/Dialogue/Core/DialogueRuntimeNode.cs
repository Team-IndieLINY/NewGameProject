using System.Collections;
using System.Collections.Generic;
using System.Linq;
using DS.Core;
using JetBrains.Annotations;
using UnityEngine;

namespace DS.Core
{
    public abstract class DialogueRuntimeNode
    {
        public DialogueNodeData Data { get; private set; }
        public List<DialogueRuntimeNode> NextNodes { get; private set; }

        public DialogueRuntimeNode FirstNode
        {
            get
            {
                if (IsLeaf) return null;

                return NextNodes[0];
            }
        }

        public bool IsLeaf => NextNodes.Any() == false;

        public DialogueRuntimeNode(DialogueNodeData data, params DialogueRuntimeNode[] nexts)
        {
            Data = data;
            NextNodes = nexts.ToList();
        }

        public DialogueRuntimeNode()
        {
            NextNodes = new();
        }

        public void SetData(DialogueNodeData data)
        {
            Data = data;
        }

        public void AddNext(params DialogueRuntimeNode[] nexts)
        {
            NextNodes.AddRange(nexts);
        }

        public abstract DialogueItem CreateItem();
        public abstract DialogueRuntimeNode GetNext();
    }

    public abstract class DialogueRuntimeNodeT<T> : DialogueRuntimeNode
        where T : DialogueNodeData
    {
        public T MyData => Data as T;

        protected DialogueRuntimeNodeT(DialogueNodeData data, params DialogueRuntimeNode[] nexts) : base(data, nexts)
        {
        }

        protected DialogueRuntimeNodeT() : base()
        {
        }
    }
}