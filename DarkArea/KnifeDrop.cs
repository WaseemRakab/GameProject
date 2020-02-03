using System.Collections;
using UnityEngine;

/**
 * Behaviour of Dropping knife Obstacle
 * 
 */
public class KnifeDrop : MonoBehaviour
{
    private readonly float DropSpeed = 35.0f;

    private bool KnifeTriggered = false;

    private void Start()
    {
        if (!GetComponent<Renderer>().isVisible)
            enabled = false;
    }

    private void Update()
    {
        if (KnifeTriggered == true)
        {
            transform.Translate(Vector3.down * DropSpeed * Time.deltaTime);
            if (WentOutOfMap())
                Destroy(gameObject);
        }
    }
    private bool WentOutOfMap()
    {
        return transform.position.y < -100f;
    }
    public IEnumerator ActivatingKnifeDropRoutine()
    {
        yield return new WaitForSeconds(0.2f);
        GetComponent<BoxCollider2D>().enabled = true;
        GetComponent<SpriteRenderer>().sortingLayerName = "Foreground";
        KnifeTriggered = true;
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            other.GetComponent<Player>().DecreaseLive(true);
        }
    }

    private void OnBecameVisible()
    {
        enabled = true;
    }
}