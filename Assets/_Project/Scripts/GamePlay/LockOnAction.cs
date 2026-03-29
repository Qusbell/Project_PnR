using System;
using UnityEngine;
using System.Collections.Generic;

public class LockOnAction : PnRAction, ITargeter
{
    // === Field === //

    [SerializeField]
    private LockOnConfig _config;

    private float Distance => _config.Distance;
    private float Angle => _config.Angle;
    private LayerMask TargetLayer => _config.TargetLayer;

    /// <summary>
    /// 현재 타겟팅된 대상 <br/>
    /// 타겟이 없는 경우 null
    /// </summary>
    public ITargetable Target { get; private set; }

    // OverlapCircleAll의 결과를 저장할 배열 (GC 방지)
    private List<Collider2D> Overlaps = new(64);
    private ContactFilter2D ContactFilter;

    public event Action<ITargetable> OnTargetChanged;


    // === Method === //

    private void Awake()
    {
        ContactFilter = new ContactFilter2D
        {
            layerMask = TargetLayer,
            useLayerMask = true
        };
    }


    protected override void OnReleasStarted(Vector2 direction)
    {
        // 타겟 확정
    }


    public void LookAt(ICompass compass)
    {
        if (compass == null ||
            !compass.IsActivate ||
            compass.Direction == Vector2.zero)
        {
            Target = null;
            return;
        }

        var newTarget = FindTarget(compass.Direction);
        if (newTarget != null && newTarget != Target)
        {
            Target = newTarget;
            OnTargetChanged?.Invoke(Target);
        }
    }

    private ITargetable FindTarget(Vector2 direction)
    {
        // 1. 범위 내의 모든 Collider2D 가져오기
        int count = Physics2D.OverlapCircle(transform.position, Distance, ContactFilter, Overlaps);

        ITargetable closestTarget = null;
        float minSqrDistance = float.MaxValue;

        for (int i = 0; i < count; i++)
        {
            var entity = Overlaps[i];

            // 2. ITargetable 검사
            if (entity.TryGetComponent<ITargetable>(out var target))
            {
                // 3. 타겟팅 가능 여부 || 자기 자신 여부
                if (!target.IsTargetable || entity.gameObject == gameObject) { continue; }

                // 타겟을 향하는 방향 벡터
                Vector2 offset = entity.transform.position - transform.position;

                // 4. 각도 검사
                Vector2 dirToTarget = offset.normalized;
                float angleToTarget = Vector2.Angle(direction, dirToTarget);

                if (angleToTarget <= Angle * 0.5f)
                {
                    // 5. 가장 가까운 타겟으로 설정
                    float distanceToTarget = offset.sqrMagnitude;
                    if (distanceToTarget < minSqrDistance)
                    {
                        minSqrDistance = distanceToTarget;
                        closestTarget = target;
                    }
                }
            }
        }

        return closestTarget;
    }



    // === Debug === //

    private void OnValidate()
    {
        if (_config == null)
        {
            Debug.LogError($"{name} : LockOnConfig : 미할당");
        }
    }

}
