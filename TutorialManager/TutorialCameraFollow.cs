using UnityEngine;

public class TutorialCameraFollow : MonoBehaviour
{
    public Transform target;
    private Vector3 velocity = Vector3.zero;

    private readonly float dampTime = 0.15f;
    private void Awake()
    {
        if (target == null)
            GameObject.FindGameObjectWithTag("Player")?.TryGetComponent(out target);
    }
    private void LateUpdate()
    {
        if (target)
        {
            Vector3 point = GetComponent<Camera>().WorldToViewportPoint(target.position);
            Vector3 Delta = target.position - GetComponent<Camera>().ViewportToWorldPoint(new Vector3(0.5f, 0.25f, point.z));
            Vector3 destination = transform.position + Delta;
            transform.position = Vector3.SmoothDamp(transform.position, destination, ref velocity, dampTime);
        }
    }
}