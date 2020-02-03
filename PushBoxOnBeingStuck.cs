using System.Collections;
using UnityEngine;

/**
 * Controlling PushableBox 
 * Object When Being Stuck
 */
public class PushBoxOnBeingStuck : MonoBehaviour
{
    private float WhichPushDir;
    private bool PushedBox = false;
    private float PushXDirection;
    private float StartXPosition;
    private void Awake()
    {
        StartXPosition = transform.position.x;
    }
    private void OnCollisionEnter2D(Collision2D other)
    {
        if (other.gameObject.CompareTag("PushableBox"))
        {
            if (PushedBox == false)
            {
                if (other.transform.position.x - transform.position.x > 0)
                {
                    WhichPushDir = 1f;
                    PushXDirection = transform.position.x + (WhichPushDir * 2);
                }
                else
                {
                    WhichPushDir = -1f;
                    PushXDirection = transform.position.x + (WhichPushDir * 2);
                }
                StartCoroutine(AddForceToPushBox());
                PushedBox = true;
            }
        }
    }
    private IEnumerator AddForceToPushBox()
    {
        if (WhichPushDir == -1f)
        {
            while (transform.position.x > PushXDirection)
            {
                transform.Translate(new Vector2(-0.1f, 0f));
                yield return null;
            }
            while (transform.position.x < StartXPosition)
            {
                transform.Translate(new Vector2(0.1f, 0f));
                yield return null;
            }
        }
        else
        {
            while (transform.position.x < PushXDirection)
            {
                transform.Translate(new Vector2(0.1f, 0f));
                yield return null;
            }
            while (transform.position.x > StartXPosition)
            {
                transform.Translate(new Vector2(-0.1f, 0f));
                yield return null;
            }
        }
        PushedBox = false;
    }
}