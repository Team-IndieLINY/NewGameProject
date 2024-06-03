using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

[ExcelAsset]
public class scriptTable : ScriptableObject
{
	public List<ScriptEntity> _script;

	public List<ScriptEntity> Script => _script;
	
	public List<DialogueItem> CreateScriptsInstance(string scriptCode)
	{
		if (Script == null) return new List<DialogueItem>();
		if (Script.Count == 0) return new List<DialogueItem>();
		
		List<DialogueItem> list = Script
			.Where(x => x.ScriptCode == scriptCode)
			.Select(x => x.Clone())
			.OrderBy(entity => entity.order)
			.Select(x=> new DialogueItem(x.Script, x.IsMaster, x.character_name, x.PoseKey))
			.ToList();
		
		return list;
	}
}
