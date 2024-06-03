
using System.Collections;
using System.Collections.Generic;
using MyBox;
using TMPro;
using UnityEngine;

public class  OpeningSceneItem: MonoBehaviour
{
    [Multiline]
    public string Script;
    public float TextAnimationDuration;
    
    
    [field: SerializeField, AutoProperty]
    public TMP_Text Text;

    public GameObject Parent => gameObject;
}
