using System.Collections;
using UnityEngine;

/**
 * Obstacle Behaviour Towards Player
 */
public class MovingObstacle : MonoBehaviour
{
    public bool LeftObstacle;
    public bool RightObstacle;

    private float StartXPosition;

    public float MaxXWayDistance;
    public float MaxXPosition;

    private float MovingDirection = 1f;

    public float _MovingSpeed = 5f;

    private bool StartMove = false;

    public float WaitTime;

    private void Awake()
    {
        StartXPosition = transform.position.x;
    }
    private IEnumerator Start()
    {
        yield return new WaitForSeconds(WaitTime);
        StartMove = true;

        if (!GetComponent<Renderer>().isVisible)
            enabled = false;

    }
    private void Update()
    {
        if (StartMove)
        {
            if (RightObstacle)
            {
                transform.Translate(Vector3.right * MovingDirection * _MovingSpeed * Time.deltaTime);
                if (transform.position.x >= MaxXPosition)
                    MovingDirection = -1f;
                else if (Mathf.Round(transform.position.x) == Mathf.Round(StartXPosition))
                    MovingDirection = 1f;
            }
            else if (LeftObstacle)
            {
                transform.Translate(Vector3.left * MovingDirection * _MovingSpeed * Time.deltaTime);
                if (transform.position.x <= MaxXPosition)
                    MovingDirection = -1f;

                else if (Mathf.Round(transform.position.x) == Mathf.Round(StartXPosition))
                    MovingDirection = 1f;
            }
        }
    }
    private void OnBecameVisible()
    {
        enabled = true;
    }
}