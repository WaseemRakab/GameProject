using UnityEngine;

/**
 * handling when to open the gate of Boss behaviour
 */
public class BossGate : MonoBehaviour
{
    /*Lift the gate up when hitting the sign on the ground and going down when away*/
    public BossSign _BossSign;

    private readonly float GateUpDownSpeed = 3f;

    private readonly float EndpositionY = 22f;
    private float StartpositionY;

    void Start()
    {
        StartpositionY = transform.position.y;

        if (!GetComponent<Renderer>().isVisible)
            enabled = false;
    }

    void Update()
    {
        if (_BossSign.SignTrigger == true)
        {
            if (transform.position.y <= EndpositionY)
                transform.Translate(Vector3.right * GateUpDownSpeed * Time.deltaTime);
        }
        else
        {
            if (transform.position.y >= StartpositionY)
                transform.Translate(Vector3.left * GateUpDownSpeed * Time.deltaTime);
        }
    }
    private void OnBecameVisible()
    {
        enabled = true;
    }
}