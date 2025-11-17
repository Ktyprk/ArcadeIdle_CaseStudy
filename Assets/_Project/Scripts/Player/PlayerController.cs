using UnityEngine;

public class PlayerController : MonoBehaviour
{
    private Rigidbody rb;

    [SerializeField] private float moveSpeed = 5f;
    [SerializeField] private float rotateSpeed = 500f;
    [SerializeField] private Joystick joystick; 

    [SerializeField] private Animator animator;
    private bool isIdling;
    public bool CanMove { get; set; } = true;

    private void Awake()
    {
        rb = GetComponent<Rigidbody>();
    }

    private void FixedUpdate()
    {
        Move();
        LookForward();
    }
    
    public void Move()
    {
        if (!rb || !CanMove) return;

        var input = joystick ? joystick.Value : Vector2.zero;

        if (input == Vector2.zero)
        {
            animator.SetBool("IsWalking", false);
            rb.linearVelocity = new Vector3(0f, rb.linearVelocity.y, 0f);
            return;
        }
        
        animator.SetBool("IsWalking",true);
        Vector3 v = new Vector3(input.x, 0f, input.y) * moveSpeed;
        rb.linearVelocity = new Vector3(v.x, rb.linearVelocity.y, v.z);
    }

    public void LookForward()
    {
        if (!rb || !joystick) return;

        Vector2 input = joystick.Value;
        if (input == Vector2.zero) return;

        Vector3 dir = new Vector3(input.x, 0f, input.y);
        Quaternion target = Quaternion.LookRotation(dir);
        Quaternion next   = Quaternion.RotateTowards(rb.rotation, target, rotateSpeed * Time.fixedDeltaTime);

        Vector3 e = next.eulerAngles; e.x = 0f; e.z = 0f;
        rb.MoveRotation(Quaternion.Euler(e));
    }
}
