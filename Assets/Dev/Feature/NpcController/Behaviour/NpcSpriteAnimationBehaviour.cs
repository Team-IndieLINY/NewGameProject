using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class NpcSpriteAnimationBehaviour : BaseNpcBehaviour
{
    private AnimatorOverrideController _overrideAni;
    private Animator _animator;
    
    private static readonly int Play = Animator.StringToHash("Play");
    private const string CLIP_KEY = "Clip";
    private const string CLIP_DEFAULT_KEY = "Default";
    
    protected override void Init()
    {
        _animator = gameObject.AddComponent<Animator>();
        var renderer = gameObject.AddComponent<SpriteRenderer>();
        renderer.sortingLayerName = "Npc";

        var runtimeAni = Resources.Load<RuntimeAnimatorController>("NpcRuntimeController");
        if (runtimeAni == false)
        {
            Debug.LogError("animation controller 불러오기 실패");
            return;
        }

        _overrideAni = new AnimatorOverrideController(runtimeAni);
        _animator.runtimeAnimatorController = _overrideAni;
        _overrideAni[CLIP_DEFAULT_KEY] = Data.DefaultAniClip;
    }
    public override bool PlayAnimation(string key)
    {
        if (Data.SpriteAnis == null) return false;

        var pair = Data.SpriteAnis.Find(x => x.Key == key);
        if (pair == null) return false;

        gameObject.SetActive(true);
        
        _overrideAni[CLIP_KEY] = pair.Clip;
        _animator.SetTrigger(Play);
        return true;
    }

    public override void Stop()
    {
        gameObject.SetActive(false);
    }
}
