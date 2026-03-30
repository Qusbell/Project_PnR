using System;
using UnityEngine;
using System.Collections.Generic;

public class LockOnAction : PnRAction, ITargeter
{
    // === Field === //

    [SerializeField]
    private LockOnConfig _config;

    private float Range => _config.Range;
    private float Angle => _config.Angle;
    private LayerMask TargetLayer => _config.TargetLayer;
    private LayerMask ObstacleLayer => _config.ObstacleLayer;

    /// <summary>
    /// 현재 타겟팅된 대상 <br/>
    /// 타겟이 없는 경우 null
    /// </summary>
    public ITargetable Target { get; private set; }

    // OverlapCircleAll의 결과를 저장할 배열 (GC 방지)
    private List<Collider2D> Overlaps = new(64);
    private ContactFilter2D ContactFilter;

    public event Action<ITargetable> OnTargetChanged;


    private ICompass Compass { get; set; }
    private Vector2 Direction => Compass != null ? Compass.Direction : Vector2.zero;
    private bool IsActivate => Direction != Vector2.zero && Compass.IsActivate;

    // === Method === //

    private void Awake()
    {
        ContactFilter = new ContactFilter2D
        {
            layerMask = TargetLayer,
            useLayerMask = true
        };
    }


    /// <summary>
    /// 지속적으로 Target을 찾음
    /// </summary>
    private void Update()
    {
        TryLockOn(Direction);
    }


    /// <summary>
    /// Release 순간의 Target을 확정
    /// </summary>
    /// <param name="direction">Key를 떼는 순간의 대각선 Intent를 반영한 direction</param>
    protected override void OnReleasStarted(Vector2 direction)
    {
        TryLockOn(direction);

#if UNITY_EDITOR
        // 디버깅
        Position_Debug = transform.position;
        Direction_Debug = direction;
        GizmoEndTime_Debug = Time.time + GizmoDuration_Debug;
#endif
    }


    public void LookBy(ICompass compass)
    {
        Compass = compass;
    }


    private void TryLockOn(Vector2 direction)
    {
        // 비활성 상태라면 Target은 없음
        if (!IsActivate)
        {
            Target = null;
            return;
        }

        // 지속적으로 TryLockOn 시도
        var newTarget = FindTarget(direction);
        if (newTarget != Target)
        {
            Target = newTarget;
            OnTargetChanged?.Invoke(Target);
        }
    }


    private ITargetable FindTarget(Vector2 direction)
    {
        // 1. 범위 내의 모든 Collider2D 가져오기
        int count = Physics2D.OverlapCircle(transform.position, Range, ContactFilter, Overlaps);

        ITargetable closestTarget = null;
        float sqrMinDistance = float.MaxValue;
        float checkAngle = Angle * 0.5f;
        LayerMask combinedMask = TargetLayer | ObstacleLayer;


        for (int i = 0; i < count; i++)
        {
            var entity = Overlaps[i];

            // 2. ITargetable 검사
            if (entity.TryGetComponent<ITargetable>(out var target))
            {
                // 3. 타겟팅 가능 여부 || 자기 자신 여부
                if (!target.IsTargetable || entity.gameObject == gameObject) { continue; }

                // <-- (차후) target.Context.Faction 체크하고
                // 같은지 다른지 등 체크 (또 다른 옵션 따라서?)

                // 4. 가장 가까운 타겟인지 체크
                Vector2 offset = entity.transform.position - transform.position;
                float sqrDistanceToTarget = offset.sqrMagnitude;
                if (sqrDistanceToTarget > sqrMinDistance) { continue; }

                // 5. 각도 검사
                Vector2 dirToTarget = offset.normalized;
                float angleToTarget = Vector2.Angle(direction, dirToTarget);

                if (angleToTarget > checkAngle) { continue; }

                // 6. Raycast로 장애물 체크
                float DistanceToTarget = Mathf.Sqrt(sqrDistanceToTarget);
                RaycastHit2D hit = Physics2D.Raycast(transform.position, dirToTarget, DistanceToTarget, combinedMask);

#if UNITY_EDITOR
                Color rayColor = Color.green; // 기본은 녹색 (통과)
                if (hit.collider != null && hit.collider != entity)
                {
                    rayColor = Color.red; // 장애물에 걸리면 빨간색
                }
                // Scene 뷰에서만 보이며, 0.1초 동안 유지됨
                Debug.DrawLine(transform.position, (Vector2)transform.position + dirToTarget * DistanceToTarget, rayColor, 0.1f);
#endif

                // 뭔가를 맞췄는데 && 그 맞춘 게 검사 중인 entity가 아니라면
                if (hit.collider != null && hit.collider != entity) { continue; }

                // 7. 타겟 변경
                sqrMinDistance = sqrDistanceToTarget;
                closestTarget = target;
            }
        }

        return closestTarget;
    }



    // === Debug === //

#if UNITY_EDITOR
    private Vector2 Direction_Debug { get; set; }
    private Vector2 Position_Debug { get; set; }
    private float GizmoEndTime_Debug { get; set; } = -1f; // 기즈모 표시가 끝날 시간
    private float GizmoDuration_Debug { get; set; } = 0.4f; // 유지 시간 (1초)

    private void OnValidate()
    {
        if (_config == null)
        {
            Debug.LogError($"{name} : LockOnConfig : 미할당");
        }
    }

    private void OnDrawGizmos()
    {
        if (GizmoEndTime_Debug < Time.time)
        {
            Gizmos.color = Color.white;
            DrawArcGizmo(transform.position, Direction, Range, Angle);
        }
        else
        {
            Gizmos.color = Color.yellow; // 가독성을 위해 색상 추가
            DrawArcGizmo(Position_Debug, Direction_Debug, Range, Angle);
        }
    }

    private void DrawArcGizmo(Vector3 center, Vector2 direction, float radius, float totalAngle)
    {
        if (totalAngle <= 0) return;
        if (direction == Vector2.zero) { return; }

        float halfAngle = totalAngle * 0.5f;
        // 기준 방향의 각도(Z축 회전값) 계산
        float baseAngle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;

        // 1. 부채꼴의 양 끝 직선 그리기
        Vector3 leftDir = Quaternion.Euler(0, 0, baseAngle + halfAngle) * Vector3.right;
        Vector3 rightDir = Quaternion.Euler(0, 0, baseAngle - halfAngle) * Vector3.right;

        Gizmos.DrawLine(center, center + leftDir * radius);
        Gizmos.DrawLine(center, center + rightDir * radius);

        // 2. 부채꼴의 호(Arc) 그리기
        int segments = 20; // 선의 촘촘함 (숫자가 높을수록 부드러운 곡선)
        float step = totalAngle / segments;
        Vector3 previousPoint = center + leftDir * radius;

        for (int i = 1; i <= segments; i++)
        {
            float currentAngle = (baseAngle + halfAngle) - (step * i);
            Vector3 nextPoint = center + (Quaternion.Euler(0, 0, currentAngle) * Vector3.right) * radius;

            Gizmos.DrawLine(previousPoint, nextPoint);
            previousPoint = nextPoint;
        }
    }
#endif
}
