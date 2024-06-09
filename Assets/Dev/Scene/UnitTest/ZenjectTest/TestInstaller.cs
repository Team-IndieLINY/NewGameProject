using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Zenject;

public class TestInstaller : MonoInstaller<TestInstaller>
{
    public override void InstallBindings()
    {
        Container.BindInterfacesTo<TestUpdater>().AsSingle();
    }
}

public class TestUpdater : ITickable
{
    private Actor.Factory _factory;

    [Inject]
    public TestUpdater(Actor.Factory factory)
    {
        _factory = factory;
    }

    public void Tick()
    {
        if (Input.GetKeyDown(KeyCode.Space))
        {
            var obj = _factory.Create();
            obj.name = "!!!";
            Debug.Log(obj.name);
        }
    }
}