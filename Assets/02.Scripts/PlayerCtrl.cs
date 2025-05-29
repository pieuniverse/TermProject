using System.Collections;
using UnityEngine;

public class PlayerCtrl : MonoBehaviour
{
    public enum State
    {
        IDLE,
        FORWARD,
        BACKWARD,
        RIGHT,
        LEFT,
        JUMP
    }

    public State state = State.IDLE;

    public float moveSpeed = 5f;
    public float mouseSensitivity = 100f;
    public float gravity = -9.81f;
    public float jumpHeight = 1.5f;

    private float h, v;

    private CharacterController controller;
    private float rotationY = 0f;
    private Vector3 velocity;

    private Animator anim;

    public float deathFallHeight = 10f;
    private float fallStartY = 0f;
    private bool isFalling = false;
    private Rigidbody rb;

    void Start()
    {
        controller = GetComponent<CharacterController>();
        anim = GetComponent<Animator>();
        rb = GetComponent<Rigidbody>();
        Cursor.lockState = CursorLockMode.Locked;
        StartCoroutine(PlayerAction());
    }

    IEnumerator PlayerAction()
    {
        while (true)
        {
            // 방향 키 입력 상태 → bool 파라미터 세팅
            anim.SetBool("Forward", Input.GetKey(KeyCode.W));
            anim.SetBool("Backward", Input.GetKey(KeyCode.S));
            anim.SetBool("Left", Input.GetKey(KeyCode.A));
            anim.SetBool("Right", Input.GetKey(KeyCode.D));

            // 아무 키도 안 눌렀을 때만 Idle
            bool anyMove = Input.GetKey(KeyCode.W) || Input.GetKey(KeyCode.S)
                        || Input.GetKey(KeyCode.A) || Input.GetKey(KeyCode.D);
            anim.SetBool("Idle", !anyMove);

            yield return new WaitForSeconds(0.1f);
        }
    }

    void Update()
    {
        MovePlayer();
        RotatePlayer();
        ApplyGravity();

        // 점프 트리거 처리 (한 번만 발동)
        if (Input.GetKeyDown(KeyCode.Space) && controller.isGrounded)
        {
            anim.SetTrigger("Jump");
            velocity.y = Mathf.Sqrt(jumpHeight * -2f * gravity);
        }

        //낙사 감지
        if (!isFalling && !IsGrounded())
        {
            // 땅에서 떨어짐 → 낙하 시작
            isFalling = true;
            fallStartY = transform.position.y;
        }

        if (isFalling && IsGrounded())
        {
            // 착지함 → 낙하 종료
            float fallDistance = fallStartY - transform.position.y;

            if (fallDistance > deathFallHeight)
            {
                GameOver();
            }

            isFalling = false;
        }
    }

    bool IsGrounded()
    {
        return controller.isGrounded;
    }

    void OnControllerColliderHit(ControllerColliderHit hit)
    {
        if (hit.gameObject.CompareTag("Enemy")) // "Enemy"는 NPC의 태그
        {
            GameOver();
        }
    }


    void GameOver()
    {
        Debug.Log("게임 오버");
    }

    void MovePlayer()
    {
        h = Input.GetAxis("Horizontal");
        v = Input.GetAxis("Vertical");
    }

    void RotatePlayer()
    {
        float mouseX = Input.GetAxis("Mouse X") * mouseSensitivity * Time.deltaTime;
        rotationY += mouseX;
        transform.rotation = Quaternion.Euler(0f, rotationY, 0f);
    }

    void ApplyGravity()
    {
        if (controller.isGrounded && velocity.y < 0)
        {
            velocity.y = -2f;
        }

        velocity.y += gravity * Time.deltaTime;
        controller.Move(velocity * Time.deltaTime);
    }

}