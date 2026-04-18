using UnityEngine;
using UnityEngine.InputSystem;

public class Interactor : MonoBehaviour
{
    [SerializeField] private float _maxDistance = 5f;
    [SerializeField] private LayerMask _interactableLayer;

    private InputSystem_Actions _inputSystem;

    private void Awake()
    {
        _inputSystem = new InputSystem_Actions();
    }

    private void OnEnable()
    {
        _inputSystem.Player.Enable();
        _inputSystem.Player.Interact.performed += CastInteractionRay;
        _inputSystem.Player.Interact.canceled += CastInteractionRay;
    }

    private void OnDisable()
    {
        _inputSystem.Player.Disable();
        _inputSystem.Player.Interact.performed -= CastInteractionRay;
        _inputSystem.Player.Interact.canceled -= CastInteractionRay;
    }

    private void CastInteractionRay(InputAction.CallbackContext context)
    {
        Ray ray = Camera.main.ScreenPointToRay(new Vector3(Screen.width / 2, Screen.height / 2));
        RaycastHit hit;

        if (Physics.Raycast(ray, out hit, _maxDistance, _interactableLayer))
        {
            IInteractable interactable = hit.collider.GetComponent<IInteractable>();

            if (interactable != null)
            {
                interactable.Interact();
                Debug.DrawRay(ray.origin, ray.direction * hit.distance, Color.green, 0.5f);
            }
        }
        else
        {
            Debug.DrawRay(ray.origin, ray.direction * _maxDistance, Color.red, 0.5f);
        }
    }
}
