using UnityEngine;

public class PlayerMovement : MonoBehaviour
{
    public CharacterController controller;
    public Transform cam;
    public Animator animator;

    // 1. Define separate speeds
    public float walkSpeed = 2f;
    public float runSpeed = 6f;
    
    public float jumpHeight = 2f;
    public float gravity = -9.81f;
    public float turnSmoothTime = 0.1f;

    float turnSmoothVelocity;
    Vector3 velocity;
    bool isGrounded;

    void Start()
    {
        Cursor.lockState = CursorLockMode.Locked;
        if (controller == null) controller = GetComponent<CharacterController>();
        if (cam == null) cam = Camera.main.transform;
        if (animator == null) animator = GetComponentInChildren<Animator>();
    }

    void Update()
    {
        isGrounded = controller.isGrounded; 
        if (isGrounded && velocity.y < 0) velocity.y = -2f; 

        // 2. Check for Sprint Key (Left Shift)
        bool isSprinting = Input.GetKey(KeyCode.LeftShift);
        
        // 3. Set current speed based on sprint status
        float currentSpeed = isSprinting ? runSpeed : walkSpeed;

        float horizontal = Input.GetAxisRaw("Horizontal");
        float vertical = Input.GetAxisRaw("Vertical");
        Vector3 direction = new Vector3(horizontal, 0f, vertical).normalized;

        // --- ANIMATION FIX ---
        // If moving:
        //   Walk = 0.5 (So the blend tree plays 'Walk')
        //   Run  = 1.0 (So the blend tree plays 'Run')
        float animationTarget = 0f;
        if (direction.magnitude >= 0.1f)
        {
            animationTarget = isSprinting ? 1f : 0.5f;
        }

        // Send to Animator with dampening (smooth transition)
        animator.SetFloat("Speed", animationTarget, 0.1f, Time.deltaTime);
        animator.SetBool("IsGrounded", isGrounded);

        // --- MOVEMENT ---
        if (direction.magnitude >= 0.1f)
        {
            float targetAngle = Mathf.Atan2(direction.x, direction.z) * Mathf.Rad2Deg + cam.eulerAngles.y;
            float angle = Mathf.SmoothDampAngle(transform.eulerAngles.y, targetAngle, ref turnSmoothVelocity, turnSmoothTime);
            
            transform.rotation = Quaternion.Euler(0f, angle, 0f);

            Vector3 moveDir = Quaternion.Euler(0f, targetAngle, 0f) * Vector3.forward;
            controller.Move(moveDir.normalized * currentSpeed * Time.deltaTime);
        }

        if (Input.GetButtonDown("Jump") && isGrounded)
        {
            velocity.y = Mathf.Sqrt(jumpHeight * -2f * gravity);
            animator.SetTrigger("Jump");
        }

        velocity.y += gravity * Time.deltaTime;
        controller.Move(velocity * Time.deltaTime);
    }
}