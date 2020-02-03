using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/**
 * Interactable Object Behaviour Towards Player
 */
public class Teleporter : MonoBehaviour
{
    private AudioManager _AudioManager;


    public int TeleportID; // 0=source , 1=Destination

    public GameObject TeleportDestination;

    public GameObject _TargetPlayer;

    public Sprite TeleportOffSprite;

    private const float TeleportWaitTime = 3.5f;

    public bool IsTeleported = false;

    public List<GameObject> Stones;

    private GameObject BoulderObject;
    private Boulder _Boulder;

    private void Awake()
    {
        GameObject audio = GameObject.FindGameObjectWithTag("AudioManager");
        if (audio != null)
            audio.TryGetComponent(out _AudioManager);

        BoulderObject = GameObject.FindGameObjectWithTag("Boulder");
        if (BoulderObject != null)
            _Boulder = BoulderObject.GetComponent<Boulder>();
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player"))//Triggered with the Player
        {
            if (TeleportID == 0) //If Source Teleporter
            {
                _AudioManager.PlayTeleporterSound();
                GetComponent<Animator>().enabled = true;
                StartCoroutine(Teleporting());
            }
            else//Destination Telporter
            {
                if (IsTeleported)
                {
                    if (_Boulder != null)
                    {
                        _Boulder.StartRolling();
                    }
                    StartCoroutine(AfterTeleporting());
                }
            }
        }
    }

    private void OnTriggerExit2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            if (TeleportID == 0)//Source teleporter
            {
                _AudioManager.StopTeleporterSound();
                StopAllCoroutines();//Stops Teleporting if the player decided to go off the teleport ground
                TeleportOff();
            }
        }
    }

    private void ResetStonesState()
    {
        for (int i = 0; i < Stones.Count; ++i)
        {
            Stones[i].GetComponent<MovingSurface>().ResetStoneState();
        }
    }

    public void AddMovingSurfaces(GameObject MovingSurface)
    {
        Stones.Add(MovingSurface);
    }

    private IEnumerator Teleporting()
    {
        yield return new WaitForSeconds(TeleportWaitTime);
        TeleportToDestination();
        TeleportOff();
        if (Stones.Count > 0)
            ResetStonesState();
    }
    private void TeleportOff()
    {
        GetComponent<SpriteRenderer>().sprite = TeleportOffSprite;
        GetComponent<Animator>().enabled = false;
    }

    private void TeleportToDestination()
    {
        _TargetPlayer.transform.position = TeleportDestination.transform.position + new Vector3(0, 3.5f, 0);
        TeleportDestination.GetComponent<Teleporter>().IsTeleported = true;
    }

    private IEnumerator AfterTeleporting()
    {
        yield return new WaitForSeconds(TeleportWaitTime);
        TeleportOff();
        IsTeleported = false;
    }
}
