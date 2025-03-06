using _Project.Script.Conveyor;
using UnityEngine;
using UnityEngine.InputSystem;

public class Input : MonoBehaviour
{
    [SerializeField] private Camera _mainCamera;
    [SerializeField] private MoverController _moverController;
    
    private PlayerInput _playerInput;

    private void Awake()
    {
        _playerInput = new PlayerInput();
        
        InitializeInputActions();
    }

    private void OnEnable()
    {
        EnableInput();
    }

    private void OnDisable()
    {
        DisableInput();
    }
    
    private void InitializeInputActions()
    {
        _playerInput.Gameplay.Click.performed += OnClick;
    }

    private void OnClick(InputAction.CallbackContext obj)
    {
        Ray ray = _mainCamera.ScreenPointToRay(Mouse.current.position.ReadValue());
        RaycastHit hit;
        
        if (Physics.Raycast(ray, out hit))
        {
            Box box = hit.collider.gameObject.GetComponent<Box>();
            Bottle item = hit.collider.GetComponent<Bottle>();
            
            if (box != null)
            {
                box.Move(_moverController.GetTargetToMove());
            }
            
            if (item != null && item.HasReachedEnd)
            {
                item.Interact();
            }
        }
    }

    private void EnableInput()
    {
        _playerInput.Enable();
    }

    private void DisableInput()
    {
        _playerInput.Disable();
    }
}
