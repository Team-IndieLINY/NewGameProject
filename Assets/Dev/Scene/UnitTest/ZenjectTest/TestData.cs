using System;
using Unity.VisualScripting;
using UnityEngine;
using Zenject;

[CreateAssetMenu(fileName = "TestData", menuName = "IndieLINY/TestData")]
public class TestData : ScriptableObject
{
    [SerializeField] private int _data;

    public int Data => _data;
}