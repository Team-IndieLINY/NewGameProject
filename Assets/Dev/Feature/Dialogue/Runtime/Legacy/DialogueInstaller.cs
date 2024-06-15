using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using Zenject;

public class DialogueInstaller : MonoInstaller<DialogueInstaller>
{
    public override void InstallBindings()
    {
        Container
            .BindInterfacesTo<DialogueView>()
            .AsSingle();
        
       // Container
       //     .BindInterfacesTo<DialogueController>()
       //     .AsSingle();
    }
}
