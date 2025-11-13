using UnityEngine;

[RequireComponent(typeof(Rigidbody2D))]
public class RopeGenerator: MonoBehaviour
{
    [Header("Rope Settings")]
    [Tooltip("Prefab for each rope segment")]
    public GameObject segmentPrefab;

    [Tooltip("Number of segments to generate")]
    public int numberOfSegments = 15;

    [Tooltip("Length between each segment (spacing)")]
    public float segmentLength = 0.5f;

    // Claw connects to this
    public Rigidbody2D LastSegmentRigidbody { get; private set; }

    void Start()
    {
        if(segmentPrefab == null)
        {
            Debug.LogError("RopeGenerator: Missing segment prefab!");
            return;
        }

        GenerateRope();
    }

    void GenerateRope()
    {
        // The anchor's Rigidbody2D (this object)
        Rigidbody2D previousBody = GetComponent<Rigidbody2D>();
        Vector2 currentPosition = transform.position;

        for(int i = 0; i < numberOfSegments; i++)
        {
            // Step 1: Determin position of new segment
            currentPosition.y -= segmentLength;

            // Step 2: Instantiate setment and get Rigidbody
            GameObject newSegment = Instantiate(segmentPrefab, currentPosition, Quaternion.identity, transform);
            Rigidbody2D currentBody = newSegment.GetComponent<Rigidbody2D>();
            HingeJoint2D joint = newSegment.GetComponent<HingeJoint2D>();

            if(currentBody == null)
            {
                Debug.LogError("RopeGenerator: Segment prefab must include RigidBody2D");
                return;
            }
            //if(joint == null)
            //{
            //    Debug.LogError(
            //        "NO JOINT FOUND AT RUNTIME!\n" +
            //        "Name: " + newSegment.name + "\n" +
            //        "Components on object: \n" +
            //        string.Join("\n",
            //            System.Array.ConvertAll(
            //                newSegment.GetComponents<Component>(),
            //                c => c.GetType().Name
            //            )
            //        )
            //    );
            //    return;
            //}
            if(joint == null)
            {
                Debug.LogError("RopeGenerator: Segment prefab must include HingeJoint2D");
                return;
            }

            // Step 3: Connect new segment to previous one
            joint.connectedBody = previousBody;

            // Anchor for this segment = its top
            joint.anchor = new Vector2(0f, segmentLength * 0.5f);

            // Connected anchor - bottom of previous segent, or center of anchor for first segment
            if(i == 0)
            {
                joint.connectedAnchor = Vector2.zero;
            }
            else
            {
                joint.connectedAnchor = new Vector2(0f, -segmentLength * 0.5f);
            }

            // Prepare for next iteration
            previousBody = currentBody;
        }

        // Store reference to final segment
        LastSegmentRigidbody = previousBody;
    }
}
