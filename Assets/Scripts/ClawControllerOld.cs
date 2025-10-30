using UnityEngine;
using System.Collections; // Required for Coroutines
using UnityEngine.InputSystem;

public class ClawControllerOld: MonoBehaviour
{
    // --- Claw Movement Settings ---
    public float moveSpeed = 5f;        // Horizontal movement speed
    public float dropSpeed = 3f;        // Vertical drop speed
    public float minX = -8f;            // Left boundary
    public float maxX = 8f;             // Right boundary
    public float minY = -4f;            // Lowest drop point
    public float startY = 5f;           // Initial Y position (starting height)
    public Transform clawHead;          // The actual claw object to move (assign in Inspector)

    // --- State Management ---
    private enum ClawState { Idle, MovingHorizontal, Dropping, Clamping, Returning }
    private ClawState currentState = ClawState.Idle;
    private Vector3 initialPosition;
    private bool isClamped = false;
    private bool isInputEnabled = true;

    // --- Initialization ---
    void Start()
    {
        // Store the starting position of the claw
        initialPosition = new Vector3(clawHead.position.x, startY, clawHead.position.z);
        clawHead.position = initialPosition;
        currentState = ClawState.Idle;
    }

    // --- Main Update Loop for Movement ---
    void Update()
    {
        // HandleMovement is now the only function called in Update for movement.
        if(!isInputEnabled) return;

        HandleMovement();

        /* / Only process movement if input is enabled and the claw is in a movement state
        if(!isInputEnabled) return;

        HandleInput();
        HandleMovement();
        */
    }

    // --- Input Handling (Map these to your UI Buttons or Keyboard/Gamepad) ---
    void HandleInput()
    {
        // 1. Horizontal Movement (Move Right/Left)
        if(currentState == ClawState.Idle || currentState == ClawState.MovingHorizontal)
        {
            // --- New Input System (Example) ---
            // If using Unity's new Input System, you would check for button press events here.
            // Example:
            // if (Input.GetButton("MoveRight")) { Move(1); }
            // else if (Input.GetButton("MoveLeft")) { Move(-1); }
            // else { StopMovement(); }

            /*/ --- Old Input System (For quick testing/simplicity) ---
            if(Input.GetKey(KeyCode.D) || Input.GetKey(KeyCode.RightArrow))
            {
                Move(1); // Right
            }
            else if(Input.GetKey(KeyCode.A) || Input.GetKey(KeyCode.LeftArrow))
            {
                Move(-1); // Left
            }
            /*else if(currentState == ClawState.MovingHorizontal)
            {
                StopMovement(); // Stop if no key is pressed
            }*/
        }


        // 2. Drop Claw (Lower)
        /* You only get one chance to drop!
        if(currentState == ClawState.Idle && (Input.GetKeyDown(KeyCode.Space) || Input.GetKeyDown(KeyCode.Return)))
        {
            LowerClaw();
        }

        / / --- Other Actions (Clamp/Unclamp/Return are handled inside Coroutines) ---
        */
    }


    // --- Public Functions for Button/Input Mapping ---

    // 1. Move Right/Left
    public void Move(int direction) // direction: 1 for Right, -1 for Left
    {
        if(currentState == ClawState.Idle || currentState == ClawState.MovingHorizontal)
        {
            currentState = ClawState.MovingHorizontal;
            // Store the direction for HandleMovement()
            transform.localScale = new Vector3(direction, 1, 1); // Not strictly needed for 2D but common
        }
    }

    // 2. Stop Horizontal Movement
    public void StopMovement()
    {
        if(currentState == ClawState.MovingHorizontal)
        {
            currentState = ClawState.Idle;
        }
    }

    // 3. Lower/Drop Claw
    public void LowerClaw()
    {
        if(currentState == ClawState.Idle)
        {
            currentState = ClawState.Dropping;
            isInputEnabled = false; // Disable horizontal controls once dropping starts
        }
    }

    // 4. Clamping (A timed action)
    // NOTE: This is called internally when the claw hits its lowest point (minY)
    private IEnumerator ClampRoutine()
    {
        currentState = ClawState.Clamping;
        // Simulate a brief clamping animation/delay
        yield return new WaitForSeconds(0.5f);

        // Perform the actual clamping logic (e.g., check for prize collision)
        bool successfullyClamped = CheckForPrize();
        isClamped = successfullyClamped;

        // Visual/Game feedback here (e.g., claw animation, sound)
        Debug.Log("Clamping finished. Success: " + successfullyClamped);

        // Immediately start the return journey
        StartCoroutine(ReturnClawRoutine());
    }

    // Placeholder for actual game logic (Claw-Prize collision check)
    private bool CheckForPrize()
    {
        // Implement your collision detection here, e.g., using Physics2D.OverlapCircle
        // For now, let's just assume a 50% chance of success
        return Random.value > 0.5f;
    }

    // 5. Unclamp (Called internally when returning finishes)
    private void Unclamp()
    {
        // Drop the prize if held (if the destination is the drop-off zone)
        if(isClamped)
        {
            // Logic to release the prize object
            Debug.Log("Prize released/Unclamped!");
            isClamped = false;
        }
        // Reset state and enable input for the next round
        currentState = ClawState.Idle;
        isInputEnabled = true;
    }

    // 6. Return to Initial Position (A timed action)
    private IEnumerator ReturnClawRoutine()
    {
        currentState = ClawState.Returning;
        float duration = Vector3.Distance(clawHead.position, initialPosition) / (moveSpeed * 2); // Faster return
        float time = 0;
        Vector3 startPos = clawHead.position;

        while(time < duration)
        {
            // Move back to initial position smoothly
            clawHead.position = Vector3.Lerp(startPos, initialPosition, time / duration);
            time += Time.deltaTime;

            // If successfully clamped, move the prize with the claw
            if(isClamped)
            {
                // Logic to move the held prize object to match clawHead.position
            }

            yield return null;
        }
        clawHead.position = initialPosition; // Ensure final position is exact

        // Now that we're back, unclamp and reset
        Unclamp();
    }

    // --- Core Movement Logic ---
    void HandleMovement()
    {
        Vector3 newPosition = clawHead.position;

        switch(currentState)
        {
            case ClawState.MovingHorizontal:
                // Get the direction from the input/local scale
                float direction = transform.localScale.x;
                if(Input.GetKey(KeyCode.A) || Input.GetKey(KeyCode.LeftArrow)) { direction = -1; }
                else if(Input.GetKey(KeyCode.D) || Input.GetKey(KeyCode.RightArrow)) { direction = 1; }

                newPosition.x += direction * moveSpeed * Time.deltaTime;
                // Clamp position within boundaries
                newPosition.x = Mathf.Clamp(newPosition.x, minX, maxX);
                break;

            case ClawState.Dropping:
                newPosition.y -= dropSpeed * Time.deltaTime;
                // Check if we hit the lowest point
                if(newPosition.y <= minY)
                {
                    newPosition.y = minY;
                    // Start the clamping and return process
                    StartCoroutine(ClampRoutine());
                }
                break;

            case ClawState.Returning:
                // Movement is handled by the ReturnClawRoutine coroutine
                return; // Exit early
        }

        clawHead.position = newPosition;
    }
}