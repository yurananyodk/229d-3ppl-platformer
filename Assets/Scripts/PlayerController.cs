using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : MonoBehaviour
{
    CharacterController controller;
    public float speed = 6.0f;
    public float jumpSpeed = 8.0f;
    public float gravity = 20.0f;

    public float mass = 70.0f; // มวลของตัวละคร (kg)
    public float force;       // แรงที่เกิดขึ้นจากสูตร F = ma

    public float friction = 0.9f;
    public float minSpeedThreshold = 0.1f;

    private Vector3 moveDirection = Vector3.zero;

    public Transform cam;
    public float turnSmoothTime = 0.1f;
    float turnSmoothVelocity;
    private Vector3 initialPosition;
    private Quaternion initialRotation;

    public PauseMenu pauseMenu;

    public Animator animator;
    public bool isFalling = false;

    void Start()
    {
        controller = GetComponent<CharacterController>();
        initialPosition = transform.position;
        initialRotation = transform.rotation;
        initialPosition.y += 50;
        pauseMenu = GetComponent<PauseMenu>();
        animator = transform.Find("ty").GetComponent<Animator>();
    }

    void Update()
    {
        if (pauseMenu.paused)
            return;

        float xDisplacement = Input.GetAxis("Horizontal");
        float zDisplacement = Input.GetAxis("Vertical");

        animator.SetBool("isGrounded", controller.isGrounded);
        if (isFalling)
            xDisplacement = zDisplacement = 0.0f;

        if (controller.isGrounded)
        {
            animator.SetBool("isFalling", false);
            moveDirection = new Vector3(xDisplacement * speed, 0, zDisplacement * speed);

            if (Input.GetKeyDown(KeyCode.Space))
            {
                moveDirection.y = jumpSpeed;
                // กฎข้อที่ 2 — กฎของแรง
                // คำนวณแรงจากการกระโดด (F = m * a)
                float jumpForce = mass * jumpSpeed;
                Debug.Log($"[กระโดด] แรงที่เกิดขึ้น: {jumpForce} N");

                animator.SetTrigger("isJumping");
                animator.SetBool("isRunning", false);
                animator.SetBool("isIdling", false);
            }
        }
        else
        {
            moveDirection = new Vector3(xDisplacement * speed, moveDirection.y, zDisplacement * speed);
        }

        if (controller.isGrounded)
        {
            // กฎข้อที่ 1 — กฎของความเฉื่อย

            // เพิ่มแรงเสียดทาน (Friction) เพื่อหยุดตัวละครเมื่อไม่มีแรงมากระทำ

            moveDirection.x *= friction;
            moveDirection.z *= friction;

            if (Mathf.Abs(moveDirection.x) < minSpeedThreshold) moveDirection.x = 0;
            if (Mathf.Abs(moveDirection.z) < minSpeedThreshold) moveDirection.z = 0;
        }

        // คำนวณแรงจากการเคลื่อนที่ (F = m * a)
        Vector3 acceleration = (moveDirection - Vector3.zero) / Time.deltaTime;
        force = mass * acceleration.magnitude;

        Debug.Log($"[เคลื่อนที่] แรงที่เกิดขึ้น: {force} N");

        moveDirection.y += gravity * Time.deltaTime;

        if (moveDirection.x != 0.0f || moveDirection.z != 0.0f)
        {
            float targetAngle = Mathf.Atan2(moveDirection.x, moveDirection.z) * Mathf.Rad2Deg + cam.eulerAngles.y;
            float angle = Mathf.SmoothDampAngle(transform.eulerAngles.y, targetAngle, ref turnSmoothVelocity, turnSmoothTime);
            transform.rotation = Quaternion.Euler(0f, angle, 0f);

            Vector3 moveDir = Quaternion.Euler(0f, targetAngle, 0f) * Vector3.forward;
            moveDir.y = moveDirection.y;
            moveDir.x *= speed;
            moveDir.z *= speed;

            controller.Move(moveDir * Time.deltaTime);

            if (controller.isGrounded)
            {
                animator.SetBool("isRunning", true);
                animator.SetBool("isIdling", false);
            }
            else
            {
                animator.SetBool("isRunning", false);
                animator.SetBool("isIdling", false);
            }
        }
        else
        {
            controller.Move(moveDirection * Time.deltaTime);
            if (controller.isGrounded)
            {
                animator.SetBool("isRunning", false);
                animator.SetBool("isIdling", true);
            }
            else
            {
                animator.SetBool("isRunning", false);
                animator.SetBool("isIdling", false);
            }
        }

        if (transform.position.y < -30)
        {
            transform.position = initialPosition;
            transform.rotation = initialRotation;
            animator.SetBool("isFalling", true);
        }
    }
    // กฎข้อที่ 3 — กฎของแรงกิริยาและแรงปฏิกิริยา
    void OnControllerColliderHit(ControllerColliderHit hit)
    {
        Rigidbody body = hit.collider.attachedRigidbody;
        if (body != null && !body.isKinematic)
        {
            Vector3 pushDirection = new Vector3(hit.moveDirection.x, 0, hit.moveDirection.z);
            body.linearVelocity = pushDirection * force / body.mass;
            Debug.Log($"[ชนวัตถุ] แรงปฏิกิริยาที่กระทำ: {force} N");
        }
    }
}