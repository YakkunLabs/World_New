using UnityEngine;

public class PlayerMovement : MonoBehaviour
{
    public CharacterController controller;
    public Transform cam;
    public Animator animator; // LINK THIS IN INSPECTOR

    public float speed = 6f;
    public float jumpHeight = 2f;
    public float gravity = -9.81f;
    public float turnSmoothTime = 0.1f;

    float turnSmoothVelocity;
    Vector3 velocity;
    bool isGrounded;

    public Transform groundCheck;
    public float groundDistance = 0.4f;
    public LayerMask groundMask;

    void Start()
    {
        Cursor.lockState = CursorLockMode.Locked;
        if (controller == null) controller = GetComponent<CharacterController>();
        if (cam == null) cam = Camera.main.transform;
        
        // Auto-find animator in children if not assigned
        if (animator == null) animator = GetComponentInChildren<Animator>();
    }

    void Update()
    {
        isGrounded = controller.isGrounded; 

        if (isGrounded && velocity.y < 0)
        {
            velocity.y = -2f; 
        }

        float horizontal = Input.GetAxisRaw("Horizontal");
        float vertical = Input.GetAxisRaw("Vertical");
        Vector3 direction = new Vector3(horizontal, 0f, vertical).normalized;

        // --- ANIMATION STEP 1: Calculate Speed ---
        // If we are moving, speed is 1. If stopped, speed is 0.
        // We use Damp to make it smooth (0 -> 0.1 -> 0.5...)
        float targetSpeed = direction.magnitude;
        animator.SetFloat("Speed", targetSpeed, 0.1f, Time.deltaTime);
        animator.SetBool("IsGrounded", isGrounded);

        if (direction.magnitude >= 0.1f)
        {
            float targetAngle = Mathf.Atan2(direction.x, direction.z) * Mathf.Rad2Deg + cam.eulerAngles.y;
            float angle = Mathf.SmoothDampAngle(transform.eulerAngles.y, targetAngle, ref turnSmoothVelocity, turnSmoothTime);
            
            transform.rotation = Quaternion.Euler(0f, angle, 0f);

            Vector3 moveDir = Quaternion.Euler(0f, targetAngle, 0f) * Vector3.forward;
            controller.Move(moveDir.normalized * speed * Time.deltaTime);
        }

        if (Input.GetButtonDown("Jump") && isGrounded)
        {
            velocity.y = Mathf.Sqrt(jumpHeight * -2f * gravity);
            // --- ANIMATION STEP 2: Trigger Jump ---
            animator.SetTrigger("Jump");
        }

        velocity.y += gravity * Time.deltaTime;
        controller.Move(velocity * Time.deltaTime);
    }
}