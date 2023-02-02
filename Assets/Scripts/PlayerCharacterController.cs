using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(CharacterController))]
public class PlayerCharacterController : MonoBehaviour
{

    [SerializeField] private float speed;
    [SerializeField] private float jumpForce;
    [SerializeField] private float dampTime;
    [SerializeField] [Range(0, 0.5f)] private float desiredRotationSpeed = 0.1f; //Speed of the players rotation

    private Vector3 dampVelocity;
    private float rotateDampVelocity;
    private Vector3 verticalVelocity;
    private bool jump;
    private bool isGrounded;
    private Vector3 desiredVelocity;

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
            // In this line we change from world coordinates to localcoordinates allowing
            //  us to have a local perspective of the movement according to where the
            //  player is looking towards. 
            // Check this out for visual explanation: https://nord.instructure.com/courses/23352/pages/velocity-in-world-vs-local-coords
            var localVelocity = transform.worldToLocalMatrix.MultiplyVector(controller.velocity);

            animator.SetFloat("Speed", localVelocity.z / speed);
            animator.SetFloat("Strafe", localVelocity.x / speed);
            animator.SetFloat("VerticalSpeed", localVelocity.y);
        }

        var vertical = Input.GetAxis("Vertical");
        var horizontal = Input.GetAxis("Horizontal");
        var forward = transform.forward;
        var right = transform.right;
        var xzForward = Vector3.ProjectOnPlane(forward, Vector3.up).normalized;

        Move(vertical, horizontal, xzForward, right);
        RotateTowards(forward);
    }


    private void RotateTowards(Vector3 forward)
    {
        // Rotate towards the camera (Camera forward) if the character is moving (tip use sqrMagnitude and isGrounded)
        if (controller.isGrounded && controller.velocity.sqrMagnitude > 0.01f)
        {
            // Tip, use Quaternion.Slerp to do the rotation smoother
            var horizontalForward = Vector3.ProjectOnPlane(forward, Vector3.up);

            // This line will do the rotation
            transform.rotation = transform.rotation; // CHANGE THIS LINE TO ROTATE (Quaternion.LookRotation(horizontalForward, Vector3.up))
        }
    }

    // This move function does the movement based on two vectors DesiredVelocity (for the x and z components)
    // and VerticalVelocity (for the Y component)
    private void Move(float verticalAxis, float horizontalAxis, Vector3 xzForward, Vector3 xzRight)
    {
        // Calculate the movemnet for the X and Z
        var xzVelocity = Vector3.ProjectOnPlane(controller.velocity, Vector3.up);
        // Calculate the desired velocity based on the vertical and horizontal axis (0-1 value) using the speed

        // This line will move the character: desiredVelocity = (xzForward * vertical + xzRight * horizontal) * speed;
        desiredVelocity = desiredVelocity; // CHANGE THIS LINE (multiply xzForward with the vertical

        // (Optionally, if the controller is not grounded, continue moving)
        // (Optionally, use the smooth damp)

        // Calculate the vertical movement
        verticalVelocity += Vector3.zero; // CHANGE THIS LINE (Add Physics.Gravity * Time.deltaTime)
        if (controller.isGrounded)
        {
            // Vertical velocity has to be at least something
            verticalVelocity.y = -1f;

            // Jumping
            if (jump)
            {
                // verticalVelocity.y = ???? CHANGE THIS LINE (Add some kind of upwards force)
            }
        }
        jump = false;

        // Perform the movement
        controller.Move((verticalVelocity + desiredVelocity) * Time.deltaTime);

        // Update the "Ground" bool parameter in the animator based in controller.isGrounded
        animator.SetBool("Ground", controller.isGrounded);
    }

    // Make sure this event is called in the jump animation!!
    public void DoJump()
    {
        jump = true;
    }
}
