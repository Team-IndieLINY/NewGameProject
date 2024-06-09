using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Zenject;

public class Actor : MonoBehaviour
{
    [Inject] private TestGameController _controller;
    [Inject] private TestData _data;

    private void Update()
    {
        _controller.DoAction(_data.Data);
    }
    
    public class Factory : PlaceholderFactory<Actor>
    {
    }
}
