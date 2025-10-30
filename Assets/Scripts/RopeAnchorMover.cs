using UnityEngine;

public class RopeAnchorMover: MonoBehaviour
{
    private Rigidbody2D rb;
    public float moveSpeed = 1f;

    private void Start()
    {
        rb = GetComponent<Rigidbody2D>();
    }

    private void FixedUpdate()
    {
        //float verticalMovement = Mathf.Sin(Time.time) * moveSpeed;
        

        rb.linearVelocity = new Vector2(0f, -moveSpeed);
     }
}
