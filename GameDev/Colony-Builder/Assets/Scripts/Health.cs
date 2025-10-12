using QFSW.QC.Actions;
using System;
using System.Collections;
using UnityEngine;

public class Health : MonoBehaviour
{
    public Ragdoll ragdoll;
    public SpriteRenderer sr;

    public int maxHealth = 20;
    public int health = 20;
    public float ratio { 
        get { 
            return (float)health / (float)maxHealth; 
        } 
        set { 
            health = Mathf.FloorToInt(Mathf.Clamp(value * maxHealth, 0, maxHealth)); 
        } 
    }

    // Cannot Die
    public bool immortal;
    
    // Cannot be Damaged
    public bool invincible;

    public int regen = 0;

    public event Action<GameObject> OnDeath;
    public event Action<GameObject, int> OnDamageTaken;
    public event Action<GameObject, int> OnHeal;
    public event Action<GameObject,int> OnRegen;

    public Vector3 healthBarPosition = Vector3.up;

    bool dead = false;

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.green;
        Gizmos.DrawWireSphere(transform.position + healthBarPosition, 0.5f);
    }

    private void Start()
    {
        if(ragdoll == null)
        {
            ragdoll = GameController.Instance.defaultRagdoll;
        }
        if(sr == null)
        {
            sr = GetComponent<SpriteRenderer>();
        }

        HealthBarManager.Instance.CreateHealthBar(this);
    }

    private void LateUpdate()
    {
        // Check in late update so all calculations are complete before checking for death
        if(!immortal && health <= 0 && !dead)
        {
            dead = true;
            StartCoroutine(Death());
        }
    }

    void ChangeHealth(int change)
    {
        health = Mathf.Clamp(health + change, 0, maxHealth);
    }

    public void Damage(int damage)
    {
        var trueDamage = CalculateDamage(damage);
        ChangeHealth(-trueDamage);
        OnDamageTaken?.Invoke(gameObject, trueDamage);
    }

    public void Heal(int health)
    {

    }

    int CalculateDamage(int damage)
    {
        if (invincible)
        {
            return 0;
        }

        return damage;
    }

    int CalculateHeal(int heal)
    {
        return heal;
    }

    int CalculateRegen(int regen)
    {
        return regen;
    }

    IEnumerator Regen()
    {
        while (true)
        {
            yield return new WaitForSeconds(1);

            if (regen != 0)
            {
                var trueRegen = CalculateRegen(regen);
                ChangeHealth(trueRegen);
                OnRegen?.Invoke(gameObject, trueRegen);
            }
            
            
        }
    }

    // Death occurs on the frame after health hits 0
    IEnumerator Death()
    {
        yield return null;
        OnDeath?.Invoke(gameObject);

        var doll = Instantiate(ragdoll, transform.position, transform.rotation);
        doll.transform.localScale = sr.transform.localScale;
        var dollSR = doll.GetComponent<SpriteRenderer>();
        if(dollSR != null)
        {
            dollSR.sprite = sr.sprite;
        }

        doll.BeginDeath();

        Destroy(gameObject);
    }
}
