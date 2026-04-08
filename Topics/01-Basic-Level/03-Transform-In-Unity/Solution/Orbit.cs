using UnityEngine;

public class Orbit : MonoBehaviour
{
    private Vector3 _parentHolderPosition;

    private void Awake()
    {
        _parentHolderPosition = transform.parent.position;
    }

    void Update()
    {
        transform.RotateAround(_parentHolderPosition, Vector3.up, 30 * Time.deltaTime);
    }
}
