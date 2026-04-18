using UnityEngine;

public class Door : MonoBehaviour, IInteractable
{
    [SerializeField] private Material _openMaterial;
    [SerializeField] private Material _closeMaterial;

    private Renderer _renderer;
    private bool _isOpen = false;

    private void Awake()
    {
        _renderer = GetComponent<Renderer>();
        if (_renderer == null)
        {
            Debug.LogWarning($"{transform.name} don't have Renderer");
            enabled = false;
            return;
        }
    }

    private void Start()
    {
        _renderer.material = _closeMaterial;
    }

    public void Interact()
    {
        _isOpen = !_isOpen;
        _renderer.material = _isOpen ? _openMaterial : _closeMaterial;
        Debug.Log($"Door is {(_isOpen ? "open" : "close")}");
    }
}
