using System.Collections;
using UnityEngine;

public class PlayerCtrl : MonoBehaviour
{
    public float moveSpeed = 5f;
    public float mouseSensitivity = 100f;
    public float gravity = -9.81f;
    public float jumpHeight = 2f;
    private bool jump = false;

    private float h, v;

    private CharacterController controller;
    private float rotationY = 0f;
    private Vector3 velocity;

    private Animation anim;

    void Start()
    {
        controller = GetComponent<CharacterController>();
        Cursor.lockState = CursorLockMode.Locked;
        anim = GetComponent<Animation>();
        anim.Play("Idle");
    }

    void Update()
    {
        MovePlayer();
        RotatePlayer();
        ApplyGravityAndJump();
        PlayerAnim(h, v);
    }

    void PlayerAnim(float h, float v)
    {
        if (v >= 0.1f) { anim.CrossFade("JogForward", 0.25f); }
        else if (v <= -0.1f) { anim.CrossFade("JogBackward", 0.25f); }
        else if (h <= -0.1f) { anim.CrossFade("JogLeft", 0.25f); }
        else if (h >= 0.1f) { anim.CrossFade("JogRight", 0.25f); }
        else if (jump) { anim.CrossFade("Jump", 0.25f); }
        else { anim.CrossFade("Idle", 0.25f); }
    }

    void MovePlayer()
    {
        h = Input.GetAxis("Horizontal");
        v = Input.GetAxis("Vertical");

        Vector3 move = transform.right * h + transform.forward * v;
        controller.Move(move * moveSpeed * Time.deltaTime);
    }

    void RotatePlayer()
    {
        float mouseX = Input.GetAxis("Mouse X") * mouseSensitivity * Time.deltaTime;
        rotationY += mouseX;
        transform.rotation = Quaternion.Euler(0f, rotationY, 0f);
    }

    float groundedTimer = 0f;           // 바닥에 있던 시간 누적
    float coyoteTime = 0.2f;            // 점프猶予시간 (코요테 타임)

    void ApplyGravityAndJump()
    {
        // 바닥 체크 (지속 시간 기반)
        if (controller.isGrounded)
        {
            groundedTimer = coyoteTime; // 바닥에 닿아 있으면 리셋
            if (velocity.y < 0) velocity.y = -2f;
            jump = false;
        }
        else
        {
            groundedTimer -= Time.deltaTime; // 공중에 있으면 시간 감소
        }

        // 스페이스바 눌렀고, 아직 코요테 타임 안 끝났으면 점프
        if (Input.GetButtonDown("Jump") && groundedTimer > 0f)
        {
            velocity.y = Mathf.Sqrt(jumpHeight * -2f * gravity);
            groundedTimer = 0f; // 점프 후 중복 방지
            jump = true;
        }

        velocity.y += gravity * Time.deltaTime;
        controller.Move(velocity * Time.deltaTime);
    }

}

