using UnityEngine;

/*Camera Controller on Game Ending*/
public class CreditsCamera : MonoBehaviour
{
    private Vector3 startPos;
    private Vector3 endPos;

    private const float lerpTime = 40f;
    private float currentLerpTime;

    public Transform currentTarget;
    public Vector3 Delta;

    private void Awake()
    {
        Cursor.visible = true;
    }
    private void Start()
    {
        startPos = transform.position;
        endPos = transform.position + currentTarget.position;
    }
    private void Update()
    {
        currentLerpTime += Time.deltaTime;
        if (currentLerpTime > lerpTime)
            currentLerpTime = lerpTime;
        float perc = currentLerpTime / lerpTime;
        transform.position = Vector3.Lerp(startPos, endPos, perc);
    }
    private void LateUpdate()
    {
        Vector3 point = GetComponent<Camera>().WorldToViewportPoint(currentTarget.position);
        Delta = currentTarget.position - GetComponent<Camera>().ViewportToWorldPoint(new Vector3(0.5f, 0.25f, point.z));
    }
    public bool ReachedEndOfGameCredits()
    {
        return Delta.x >= 0f && Delta.x <= 1f;
    }
}