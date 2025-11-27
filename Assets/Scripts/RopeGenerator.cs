using UnityEngine;

[RequireComponent(typeof(Rigidbody2D))]
public class RopeGenerator: MonoBehaviour
{
    [Header("Rope Settings")]
    [Tooltip("Prefab for each rope segment")]
    public GameObject segmentPrefab;

    [Tooltip("Number of segments to generate")]
    public int numberOfSegments = 60;

    [Header("Segment Physics Settings")]
    //[Tooltip("Mass of each segment Rigidbody2D")]
    public float segmentMass = 5f;
    public float segmentDrag = 5f;
    public float segmentAngularDrag = 5f;
    public float gravityScale = 1f;
    public float jointLimitAngle = 10;

    // Claw connects to this
    public Rigidbody2D LastSegmentRigidbody { get; private set; }

    void Start()
    {
        if (segmentPrefab == null)
        {
            Debug.LogError("RopeGenerator: Missing segment prefab!");
            return;
        }

        GenerateRope();
    }

    void GenerateRope()
    {
        Rigidbody2D previousBody = GetComponent<Rigidbody2D>();
        Vector2 currentPosition = transform.position;

        for (int i = 0; i < numberOfSegments; i++)
        {
            // Instantiate segment
            GameObject newSegment = Instantiate(segmentPrefab, currentPosition, Quaternion.identity, transform);

            Rigidbody2D currentBody = newSegment.GetComponent<Rigidbody2D>();
            HingeJoint2D joint = newSegment.GetComponent<HingeJoint2D>();
            Collider2D col = newSegment.GetComponent<Collider2D>();

            // Validate prefab
            if (currentBody == null)
            {
                Debug.LogError("RopeGenerator: Segment prefab must include Rigidbody2D");
                Destroy(newSegment);
                return;
            }

            if (joint == null)
            {
                Debug.LogError("RopeGenerator: Segment prefab must include HingeJoint2D");
                Destroy(newSegment);
                return;
            }

            if (col == null)
            {
                Debug.LogError("RopeGenerator: Segment prefab must include a Collider2D");
                Destroy(newSegment);
                return;
            }

            // Configure Rigidbody2D for stability
            currentBody.mass = segmentMass;
            currentBody.drag = segmentDrag;
            currentBody.angularDrag = segmentAngularDrag;
            currentBody.gravityScale = gravityScale;
            currentBody.interpolation = RigidbodyInterpolation2D.Interpolate;

            // Determine segment height from collider
            float halfHeight = col.bounds.size.y * 0.5f;

            // Position segment under previous
            currentPosition.y -= (halfHeight * 2f);
            
            // Connect new segment to previous
            joint.connectedBody = previousBody;
            joint.autoConfigureConnectedAnchor = false;

            // Anchor  (top of this segment)
            joint.anchor = new Vector2(0f, halfHeight);

            // Connected anchor (bottom of previous)
            if (i == 0)
            {
                joint.connectedAnchor = Vector2.zero;
            }
            else
            {
                joint.connectedAnchor = new Vector2(0f, -halfHeight);
            }

            // Reduce springiness with HingeJoint limits
            joint.useLimits= true;
            JointAngleLimits2D limits = new JointAngleLimits2D();
            limits.min = -jointLimitAngle;
            limits.max = jointLimitAngle;
            joint.limits = limits;

            // Prepare for next iteration
            previousBody = currentBody;
        }

        // Store reference to final segment
        LastSegmentRigidbody = previousBody;
    }
}
