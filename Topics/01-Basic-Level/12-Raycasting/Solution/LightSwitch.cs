using UnityEngine;

public class LightSwitch : MonoBehaviour, IInteractable
{
    [SerializeField] private Light _targetLight;
    private bool _isOn = true;

    public void Interact()
    {
        _isOn = !_isOn;
        _targetLight.intensity = _isOn ? 3f : 0f;
        Debug.Log($"Ligth is {(_isOn ? "on" : "off")}");
    }
}
