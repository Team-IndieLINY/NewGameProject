using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Zenject;

public class GameInstaller : MonoInstaller<GameInstaller>
{
    [SerializeField] private GameObject _installer;

    public override void InstallBindings()
    {
        Container
            .BindFactory<Actor, Actor.Factory>()
            .FromSubContainerResolve()
            .ByNewContextPrefab(_installer)
            .UnderTransformGroup("Actors")
            .AsSingle();

        Container
            .BindInterfacesTo<Updater>()
            .AsSingle();
    }
}


public class Updater : ITickable
{
    private Actor.Factory _factory;

    public Updater(Actor.Factory factory)
    {
        _factory = factory;
    }

    public void Tick()
    {
        if (Input.GetMouseButtonDown(0))
        {
            var obj = _factory.Create();
            Debug.Log(obj.name);
        }
    }
}