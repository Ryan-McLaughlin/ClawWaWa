using UnityEngine;
using UnityEngine.InputSystem;

public class ClawController: MonoBehaviour
{
    [Header("Input Actions")]
    //public InputActionReference moveActionReference;
        
    // Action Variables
    InputAction moveAction;
    //InputAction lowerAction;
    //InputAction clampAction;
    //InputAction returnAction;

    [Header("Movement Settings")]
    [SerializeField] float horizontalSpeed = 5f;
    [SerializeField] float verticalSpeed = 3f;

    // State Tracking
    bool isClamped = false; // ?need this?

    private void Awake()
    {
        //moveAction = moveActionReference.action;

        moveAction = InputSystem.actions.FindAction("MoveHorizontal");
        //lowerAction = InputSystem.actions.FindAction("Lower");
        //clampAction = InputSystem.actions.FindAction("Clamp");
        //returnAction = InputSystem.actions.FindAction("Return");
    }

    private void OnEnable()
    {
        moveAction.Enable();
        //lowerAction.Enable();
        //clampAction.Enable();
        //returnAction.Enable();
    }

    private void OnDisable()
    {
        moveAction.Disable();
        //lowerAction.Disable();
        //clampAction.Disable();
        //returnAction.Disable();
    }

    private void Update()
    {
        //float horizontalInput = moveAction.ReadValue<float>();
        Vector2 moveInput = moveAction.ReadValue<Vector2>();

        // I had to invert the x value, it is backwards for some reason...
        float horizontalInput = -moveInput.x;

        Vector2 horizontalMove = new Vector2(horizontalInput, 0f) * horizontalSpeed * Time.deltaTime;
        transform.Translate(horizontalMove);
    }
}
