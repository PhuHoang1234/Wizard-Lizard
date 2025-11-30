using UnityEngine;

public class EnemyDeath : MonoBehaviour
{
    [Header("Animation")]
    public Animator anim;                // drag GoblinModel Animator here
    public string deathTrigger = "Die";  // trigger name in Animator

    [Header("Timing")]
    public float destroyDelay = 2.5f;    // how long before we delete goblin

    bool isDead = false;

    void Awake()
    {
        if (!anim)
            anim = GetComponentInChildren<Animator>();  // finds GoblinModel
    }

    public void Die()
    {
        if (isDead) return;
        isDead = true;

        Debug.Log("EnemyDeath.Die called on " + gameObject.name);

        // 1) Play death animation
        if (anim && !string.IsNullOrEmpty(deathTrigger))
            anim.SetTrigger(deathTrigger);

        // 2) Disable AI / movement scripts so he stops
        foreach (MonoBehaviour mb in GetComponentsInChildren<MonoBehaviour>())
        {
            if (mb != this)              // keep EnemyDeath running
                mb.enabled = false;
        }

        // 3) Delete goblin after delay
        Destroy(gameObject, destroyDelay);
    }
}
