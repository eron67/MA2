using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(CharacterController))]
public class PlayerCharacterControllerCompleted : MonoBehaviour
{

    [SerializeField]
    private float speed;
    [SerializeField]
    private float jumpForce;
    [SerializeField]
    private float dampTime;
    [SerializeField] private float rotationBlendSpeed = 10;

    private Vector3 dampVelocity;
    private Vector3 verticalVelocity;
    private bool jump, grounded;

    private CharacterController controller;
    private Animator animator;

    // Start is called before the first frame update
    void Start()
    {
        controller = GetComponent<CharacterController>();
        animator = GetComponent<Animator>();
    }

    // Update is called once per frame
    void Update()
    {
        if (controller.isGrounded && Input.GetKeyDown(KeyCode.Space))
        {
            animator.SetTrigger("Jump");
        }

        // Update animator
        if (animator)
        {
            var localVelocity = transform.worldToLocalMatrix.MultiplyVector(controller.velocity);

            animator.SetFloat("Speed", localVelocity.z / speed);
            animator.SetFloat("Strafe", localVelocity.x / speed);
            animator.SetFloat("VerticalSpeed", localVelocity.y);
        }

        // Move the X and Z
        var vertical = Input.GetAxis("Vertical");
        var horizontal = Input.GetAxis("Horizontal");
        var xzForward = Vector3.ProjectOnPlane(Camera.main.transform.forward, Vector3.up);

        Move(vertical, horizontal, xzForward, Camera.main.transform.right);
        Rotate();

        jump = false;
    }

    private void Rotate()
    {
        if (controller.velocity.sqrMagnitude > 0.01f && grounded)
        {
            transform.rotation = Quaternion.Slerp(
                    transform.rotation,
                    Quaternion.LookRotation(
                        Vector3.ProjectOnPlane(Camera.main.transform.forward, Vector3.up),
                        Vector3.up),
                    rotationBlendSpeed * Time.deltaTime
                    );
        }
    }

    private void Move(float vertical, float horizontal, Vector3 xzForward, Vector3 xzRight)
    {
        var xzVelocity = Vector3.ProjectOnPlane(controller.velocity, Vector3.up);
        var desiredVelocity = grounded
            ? ((xzForward * vertical) + (xzRight * horizontal)) * speed
            : xzVelocity + ((xzForward * vertical) + (xzRight * horizontal)) * (speed / 10f);

        desiredVelocity = Vector3.ClampMagnitude(desiredVelocity, speed);
        desiredVelocity = Vector3.SmoothDamp(xzVelocity, desiredVelocity, ref dampVelocity, dampTime);

        // Move Vertically
        verticalVelocity += Physics.gravity * Time.deltaTime;
        if (grounded)
        {
            verticalVelocity.y = -1;
            // Jumping
            if (jump)
            {
                verticalVelocity.y = jumpForce;
            }
        }

        // Perform the movement
        controller.Move((verticalVelocity + desiredVelocity) * Time.deltaTime);
        grounded = controller.isGrounded;
        animator.SetBool("Ground", grounded);
    }

    public void DoJump()
    {
        jump = true;
    }
}
