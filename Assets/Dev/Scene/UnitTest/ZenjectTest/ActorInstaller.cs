using UnityEngine;
using Zenject;

public class ActorInstaller : MonoInstaller<ActorInstaller>
{
    [SerializeField] private TestGameController _controller;
    [SerializeField] private TestData _data;

    public override void InstallBindings()
    {
        Container
            .Bind<TestData>()
            .FromScriptableObject(_data)
            .AsSingle();

        Container.Bind<TestGameController>()
            .FromInstance(_controller)
            .AsSingle();
    }

}
