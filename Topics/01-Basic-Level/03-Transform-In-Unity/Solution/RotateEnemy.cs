using UnityEngine;

public class RotateEnemy : MonoBehaviour
{
    void Update()
    {
        transform.eulerAngles += new Vector3(0, 30, 0);
    } 
}
