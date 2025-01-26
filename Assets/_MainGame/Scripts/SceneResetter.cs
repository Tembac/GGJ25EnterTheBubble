using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.InputSystem;

public class SceneResetter : MonoBehaviour
{
    [SerializeField] float timeToPress = 3.0f;
    [SerializeField] float pressTimer = 0.0f;
    [SerializeReference] GameObject player;

    private InputAction aButtonAction;

    bool pressedDown = false;

    void OnEnable()
    {
        var actionMap = new InputActionMap("OculusTouchController");
        // Bind the A button action
        aButtonAction = actionMap.AddAction("AButton", binding: "<XRController>{RightHand}/primaryButton");
        aButtonAction.Enable();
        aButtonAction.performed += OnAButtonPressed;
        aButtonAction.canceled += OnAButtonReleased;
    }

    void OnDisable()
    {
        aButtonAction.performed -= OnAButtonPressed;
        aButtonAction.canceled -= OnAButtonReleased;
        aButtonAction.Disable();
    }

    private void Update()
    {
        if(pressedDown)
        {
            pressTimer += Time.deltaTime;
            if (pressTimer > timeToPress)
            {
                ResetScene();
            }
        }
    }

    void OnAButtonPressed(InputAction.CallbackContext context)
    {
        pressedDown = true;


    }
    void OnAButtonReleased(InputAction.CallbackContext context)
    {
        pressTimer = 0.0f;
        pressedDown = false;
    }

    void ResetScene()
    {
        Destroy(player);
        SceneManager.LoadScene("InitialScene");
    }
}
