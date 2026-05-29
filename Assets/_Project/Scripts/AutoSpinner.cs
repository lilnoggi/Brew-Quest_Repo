using UnityEngine;

public class AutoSpinner : MonoBehaviour
{
    [Tooltip("Speed of rotation on the Y axis.")]
    [SerializeField] private float _spinSpeed = 45f;

    void Update()
    {
        transform.Rotate(Vector3.up * _spinSpeed * Time.deltaTime);
    }
}
