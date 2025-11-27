using UnityEngine;

[RequireComponent(typeof(Rigidbody2D))]
public class RopeAnchorMover: MonoBehaviour
{

    public float moveSpeed = 2f; // Keep this low to avoid stretching

    private Rigidbody2D rb;

    void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        rb.isKinematic = true; // Anchor ignores forces, but HingeJoint2D respects it
    }

    void FixedUpdate()
    {
        float x = Input.GetAxisRaw("Horizontal");
        float y = Input.GetAxisRaw("Vertical");

        Vector2 input = new Vector2(x, y);

        // Clamp input to avoid very fast diagonal movement
        if (input.magnitude > 1f) input.Normalize();

        // Move the anchor slowly
        Vector2 newPos = rb.position + input * moveSpeed * Time.fixedDeltaTime;
        rb.MovePosition(newPos);
    }

    /*
    private Rigidbody2D rb;

    [Header("Movement Settings")]
    [Tooltip("Speed of the anchor movement in units per second.")]
    public float moveSpeed = 5f;

    private void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        rb.isKinematic = true; // Anchor doesn't react to physics but moves joints properly
    }

    private void FixedUpdate()
    {
        float x = Input.GetAxisRaw("Horizontal");
        float y = Input.GetAxisRaw("Vertical");

        Vector2 movement = new Vector2(x, y).normalized * moveSpeed * Time.fixedDeltaTime;

        // Move via Rigidbody2D to respect joints
        rb.MovePosition(rb.position + movement);
    }

    /*
    private void Update()
    {
        // Read input from Unity default axes
        float x = Input.GetAxisRaw("Horizontal");
        float y = Input.GetAxisRaw("Vertical");

        // Build movement vector
        Vector2 direction = new Vector2(x, y).normalized;

        // Apply movement
        transform.position += (Vector3)(direction * moveSpeed * Time.deltaTime);
    }
    * /
    */
}
