using System;
using System.Collections;
using System.Collections.Generic;
using DS.Core;
using DS.Runtime;
using UnityEngine;

public class DialogueRuntimeTreeTester : MonoBehaviour
{
    public DialogueContainer Container;

    private void Start()
    {
        var tree = DialogueRuntimeTree.Build(Container);

        Traval(0, tree.EntryPoint);
    }

    private void Traval(int depth, DialogueRuntimeNode node)
    {
        print($"depth({depth}), title({node.Data.NodeTitle}), guid({node.Data.GUID}), remained({node.NextNodes.Count})");

        if (node.IsLeaf) return;

        depth++;
        foreach (DialogueRuntimeNode next in node.NextNodes)
        {
            Traval(depth, next);
        }
    }
}
