using System;
using System.Collections;
using System.Collections.Generic;
using Cysharp.Threading.Tasks;
using TMPro;
using UnityEngine;
using UnityEngine.Serialization;
using Random = UnityEngine.Random;

public class ShakingMiniGameController : MonoBehaviour
{
    [SerializeField] private Transform _tip;
    [SerializeField] private Transform _tipPivot;
    [SerializeField] private Transform _hitboxPivot;
    [SerializeField] private Transform _parent;
    [SerializeField] private Transform _bar;

    [SerializeField] private Transform _hitboxContainer;
    [SerializeField] private Transform _perfectHitbox;
    [SerializeField] private Transform _normalHitbox;

    [SerializeField] private Bounds _barHitbox;
    [SerializeField] private Bounds _tipHitbox;

    [SerializeField] private TMP_Text _resultMessageText;

    [SerializeField] private float _tipMovementSpeed = 1f;
    [SerializeField] private float _remainingDisableDuration = 2f;

    [field: SerializeField] private Vector2 _hitboxRandomPositionRange = new Vector2(-5f, 5f);

    public Bounds PerfectHitBoxBound => new Bounds(_perfectHitbox.position, _perfectHitbox.lossyScale);
    public Bounds NormalHitBoxBound => new Bounds(_normalHitbox.position, _normalHitbox.lossyScale);
    public Bounds TipHitBoxBound => new Bounds(_tip.position + _tipHitbox.center, _tipHitbox.size);
    public Bounds BarHitBoxBound => new Bounds(_barHitbox.center + _bar.transform.position, _barHitbox.size);

    public bool IsStarted { get; private set; }

    private void Awake()
    {
        _parent.gameObject.SetActive(false);
    }

    public async UniTask<EMiniGameScore> GameStart()
    {
        return await GameUpdate();
    }

    private Vector3 GetRandomHitboxStartPosition()
    {
        var x = Random.Range(_hitboxRandomPositionRange.x, _hitboxRandomPositionRange.y);

        var position = _hitboxPivot.transform.position;
        return new Vector3(x, position.y, position.z);
    }

    private async UniTask<EMiniGameScore> GameUpdate()
    {
        IsStarted = true;
        _tip.position = _tipPivot.position;
        _hitboxContainer.position = GetRandomHitboxStartPosition();

        EMiniGameScore rank = EMiniGameScore.Bad;
        
        _parent.gameObject.SetActive(true);
        _resultMessageText.text = "";

        while (true)
        {
            if (InputManager.Actions.ShakingMiniGameInteraction.triggered)
            {
                if (PerfectHitBoxBound.Intersects(TipHitBoxBound))
                {
                    rank = EMiniGameScore.Perfect;
                    _resultMessageText.text = "Perfect!";
                    break;
                }
                if (NormalHitBoxBound.Intersects(TipHitBoxBound))
                {
                    rank = EMiniGameScore.Good;
                    _resultMessageText.text = "Good!";
                    break;
                }

                rank = EMiniGameScore.Bad;
                _resultMessageText.text = "Bad!";
                break;
            }

            if (BarHitBoxBound.Intersects(TipHitBoxBound) == false)
            {
                rank = EMiniGameScore.Bad;
                _resultMessageText.text = "Bad!";
                break;
            }
            
            _tip.position += Time.deltaTime * _tipMovementSpeed * Vector3.right;

            await UniTask.WaitForEndOfFrame();
        }
        
        await UniTask.Delay((int)(_remainingDisableDuration * 1000f));
        
        _parent.gameObject.SetActive(false);
        IsStarted = false;

        return rank;
    }

    private void OnDrawGizmos()
    {
        if (!(
                _perfectHitbox &&
                _normalHitbox &&
                _tip &&
                _hitboxPivot))
            return;

        Gizmos.color = Color.green;
        Gizmos.DrawWireCube(PerfectHitBoxBound.center, PerfectHitBoxBound.size);
        Gizmos.DrawWireCube(NormalHitBoxBound.center, NormalHitBoxBound.size);
        Gizmos.DrawWireCube(TipHitBoxBound.center, TipHitBoxBound.size);
        Gizmos.DrawWireCube(BarHitBoxBound.center, BarHitBoxBound.size);

        var tipPivotPosition = _hitboxPivot.position;
        Gizmos.DrawLine(
            tipPivotPosition + Vector3.right * _hitboxRandomPositionRange.x,
            tipPivotPosition + Vector3.right * _hitboxRandomPositionRange.y
        );
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(tipPivotPosition, 0.2f);
    }
}