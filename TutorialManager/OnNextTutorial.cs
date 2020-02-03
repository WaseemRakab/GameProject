using UnityEngine;

public class OnNextTutorial : MonoBehaviour
{
    public bool OnNextTutorialCheck = false;
    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            OnNextTutorialCheck = true;
        }
    }
}