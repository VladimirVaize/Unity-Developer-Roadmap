using UnityEngine;

public class FaceTarget : MonoBehaviour
{
    private GameObject _target;
    
    private void Awake()
    {
        _target = GameObject.Find("Target");
    }

    void Update()
    {
        transform.rotation = Quaternion.LookRotation(_target.transform.position - transform.position);
    }
}
