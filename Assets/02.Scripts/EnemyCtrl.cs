using System.Collections;
using UnityEngine;

public class EnemyCtrl : MonoBehaviour
{
    public Transform pointA;
    public Transform pointB;
    public float speed = 1f;
    private Vector3 target;

    private Animator animator;
    private bool isTurning = false;

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
            //StartCoroutine(PlayTurnAnimation());
            // 다음 목표 지점 변경
            target = (target == pointA.position) ? pointB.position : pointA.position;
            transform.Rotate(0f, 180f, 0f);
        }
    }

    // 반환점에서 Turn 애니메이션 실행
    private System.Collections.IEnumerator PlayTurnAnimation()
    {
        isTurning = true;
        //animator.SetTrigger("Turn"); // 상태머신에서 Turn 트리거 활성화

        // 애니메이션 B가 끝날 때까지 대기 (길이에 따라 수정)
        yield return new WaitForSeconds(animator.GetCurrentAnimatorStateInfo(0).length);

        isTurning = false;
    }
}
