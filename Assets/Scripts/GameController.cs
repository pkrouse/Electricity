using UnityEngine;
using UnityEngine.InputSystem;

public class GameController : MonoBehaviour
{
    // uses UnityEngine.InputSystem
    public InputActionReference inputActivationReference;
    
    IActivatable activator;
    void Awake()
    {
        inputActivationReference.action.performed += Activate;
    }

    public void SetActive(IActivatable activatable)
    {
        activator = activatable;
    }
    private void Activate(InputAction.CallbackContext obj)
    {
        activator.Activate();
    }

    void Update()
    {
        if (Keyboard.current.spaceKey.wasPressedThisFrame)
            activator.Activate();
    }
}
