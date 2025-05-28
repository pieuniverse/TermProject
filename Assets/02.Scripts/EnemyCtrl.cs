using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyCtrl : MonoBehaviour
{
    public Transform pointA;
    public Transform pointB;
    public float speed = 1f;
    private Vector3 target;

    private Animator animator;
    private bool isTurning = false;

    [Header("시야 설정")]
    public float viewAngle = 90f; // 시야각 (도 단위)
    public float viewDistance = 5f; // 시야 거리
    public LayerMask targetMask; // 감지할 레이어 (플레이어)
    public LayerMask obstacleMask; // 장애물 레이어

    public Transform eyePosition; // 눈 위치 기준점 (없으면 this.transform 사용)
    private Transform targetPlayer = null;

    [Header("총알 발사 관련")]
    public FireCtrl fireCtrl; // FireCtrl 컴포넌트 연결
    public float fireCooldown = 2f; // 발사 간격
    private float lastFireTime = -999f;
    bool isPlayerDetected = false;

    void OnDrawGizmosSelected()
    {
        // 시야각 시각화를 위한 기점
        Transform origin = eyePosition != null ? eyePosition : transform;

        // 시야 거리 원
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(origin.position, viewDistance);

        // 시야각 선 (좌/우)
        Vector3 forward = transform.forward;
        Vector3 leftDir = Quaternion.Euler(0, -viewAngle / 2f, 0) * forward;
        Vector3 rightDir = Quaternion.Euler(0, viewAngle / 2f, 0) * forward;

        Gizmos.color = Color.cyan;
        Gizmos.DrawLine(origin.position, origin.position + leftDir * viewDistance);
        Gizmos.DrawLine(origin.position, origin.position + rightDir * viewDistance);
    }

    void Start()
    {
        animator = GetComponent<Animator>();
        target = pointB.position;
    }

    void Update()
    {
        if (isTurning) return; // 회전 중에는 이동하지 않음

        // 이동 애니메이션 재생
        animator.Play("RunAim");

        // 이동
        transform.position = Vector3.MoveTowards(transform.position, target, speed * Time.deltaTime);

        // 반환점 도착 처리
        if (Vector3.Distance(transform.position, target) < 0.1f)
        {
            // 다음 목표 지점 변경
            target = (target == pointA.position) ? pointB.position : pointA.position;
            transform.Rotate(0f, 180f, 0f);
        }

        CheckView();
    }

    void CheckView()
    {
        Transform origin = eyePosition != null ? eyePosition : transform;
        Collider[] targetsInView = Physics.OverlapSphere(origin.position, viewDistance, targetMask);

        targetPlayer = null; // 초기화

        foreach (Collider col in targetsInView)
        {
            Transform potentialTarget = col.transform;
            Vector3 dirToTarget = (potentialTarget.position - origin.position).normalized;
            float angle = Vector3.Angle(transform.forward, dirToTarget);

            if (angle < viewAngle / 2f)
            {
                float distance = Vector3.Distance(origin.position, potentialTarget.position);

                if (!Physics.Raycast(origin.position, dirToTarget, distance, obstacleMask))
                {
                    isPlayerDetected = true;
                    animator.speed = 0f;
                    speed = 0f;
                    targetPlayer = potentialTarget;
                    Debug.Log("플레이어 감지됨: " + targetPlayer.name);

                    TryFire();
                    break;
                }
            }
        }

        if (targetPlayer == null)
        {
            isPlayerDetected = false;
            animator.speed = 1f;
            speed = 1f;
        }
    }

    void TryFire()
    {
        if (fireCtrl == null) return;
        if (Time.time - lastFireTime > fireCooldown)
        {
            fireCtrl.Fire();
            lastFireTime = Time.time;
        }
    }
}
