using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class EnemyDummyTutorial : MonoBehaviour
{
    public bool GotAttacked = false;
    public bool EnemyIsDead = false;

    private float EnemyHealth = 100.0f;
    public GameObject EnemyHealthUI;
    public Image EnemyLiveHealth;

    public AudioManager _AudioManager;

    public Animator MyAnimator;
    public void Attacked()
    {
        _AudioManager?.PlayEnemyHurtSound(GetComponent<AudioSource>());
        GotAttacked = true;
        EnemyHealthUI.SetActive(true);
    }
    public void GotShot()
    {
        _AudioManager?.PlayEnemyHurtSound(GetComponent<AudioSource>());
        EnemyHealth -= 50.0f;
        EnemyLiveHealth.fillAmount = EnemyHealth / 100f;
        if (EnemyHealth == 0f)
            DieOnAttack();
    }
    public void DieOnAttack()
    {
        StartCoroutine(EnemyDead());
    }
    private IEnumerator EnemyDead()
    {
        _AudioManager?.PlayEnemyDeathSound(GetComponent<AudioSource>());
        MyAnimator.SetBool("Dead", true);
        yield return new WaitForSeconds(3.0f);
        EnemyIsDead = true;
        Destroy(gameObject, 0.02f);
    }
}