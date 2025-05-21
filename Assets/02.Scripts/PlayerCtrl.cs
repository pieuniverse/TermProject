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
    public float jumpHeight = 2f;
    private bool jump = false;

    private float h, v;

    private CharacterController controller;
    private float rotationY = 0f;
    private Vector3 velocity;

    private Animator anim;

    void Start()
    {
        controller = GetComponent<CharacterController>();
        Cursor.lockState = CursorLockMode.Locked;
        anim = GetComponent<Animator>();
        //StartCoroutine(CheckPlayerState());
        StartCoroutine(PlayerAction());
    }

    IEnumerator CheckPlayerState()
    {
        while (true)
        {
            yield return new WaitForSeconds(0.3f);
            
            if (v >= 0.1f) { state = State.FORWARD; }
            else if (v <= -0.1f) { state = State.BACKWARD; }
            else if (h >= 0.1f) { state = State.RIGHT; }
            else if (h <= -0.1f) { state = State.LEFT; }
            else if (jump == true) { state = State.JUMP; }
            else { state = State.IDLE; }
        }
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

            // 점프 트리거 처리 (한 번만 발동)
            if (Input.GetKeyDown(KeyCode.Space))
            {
                anim.SetTrigger("Jump");
            }

            // 아무 키도 안 눌렀을 때만 Idle
            bool anyMove = Input.GetKey(KeyCode.W) || Input.GetKey(KeyCode.S)
                        || Input.GetKey(KeyCode.A) || Input.GetKey(KeyCode.D);
            anim.SetBool("Idle", !anyMove);

            yield return new WaitForSeconds(0.1f);
        }
    }



    // IEnumerator PlayerAction()
    // {
    //     while (true)
    //     {
    //         switch (state)
    //         {
    //             case State.IDLE:
    //                 anim.SetBool("Idle", true);
    //                 break;
    //             case State.FORWARD:
    //                 anim.SetBool("Forward", true);
    //                 break;
    //             case State.BACKWARD:
    //                 anim.SetBool("Backward", true);
    //                 break;
    //             case State.RIGHT:
    //                 anim.SetBool("Right", true);
    //                 break;
    //             case State.LEFT:
    //                 anim.SetBool("Left", true);
    //                 break;
    //             case State.JUMP:
    //                 anim.SetBool("Jump", true);
    //                 break;
    //         }
    //         yield return new WaitForSeconds(0.3f);
    //     }
    // }

    void Update()
    {
        MovePlayer();
        RotatePlayer();
        ApplyGravityAndJump();
    }

    void MovePlayer()
    {
        h = Input.GetAxis("Horizontal");
        v = Input.GetAxis("Vertical");

        // Vector3 move = transform.right * h + transform.forward * v;
        // controller.Move(move * moveSpeed * Time.deltaTime);
    }

    void RotatePlayer()
    {
        float mouseX = Input.GetAxis("Mouse X") * mouseSensitivity * Time.deltaTime;
        rotationY += mouseX;
        transform.rotation = Quaternion.Euler(0f, rotationY, 0f);
    }

    void ApplyGravityAndJump()
    {
        if (controller.isGrounded)
        {
            if (velocity.y < 0)
                velocity.y = -2f;

            // 스페이스바 입력 시 점프
            if (Input.GetButtonDown("Jump"))
            {
                velocity.y = Mathf.Sqrt(jumpHeight * -2f * gravity);
                jump = true;
            }
        }

        velocity.y += gravity * Time.deltaTime;
        controller.Move(velocity * Time.deltaTime);
    }

}

