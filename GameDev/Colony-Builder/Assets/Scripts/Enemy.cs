using UnityEngine;

public class Enemy : MonoBehaviour
{
    public enum State
    {
        Idle, 
        Searching, 
        Attacking
    }

    public Rigidbody2D rb;
    public Collider2D detectionCollider;
    public Health health;

    public float speed;
    public float endurance = 10;

    public State state
    {
        get => _state;
        set
        {
            if (_state != value)
            {
                _state = value;
                OnStateUpdate();
            }
        }
    }
    [SerializeField]
    private State _state = State.Idle;

    public Transform target;
    private Vector2 prev = Vector2.zero;


    private void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        health = GetComponent<Health>();
        prev = rb.linearVelocity;
    }


    void Update()
    {
        if(state != State.Idle)
        {
            Vector2 dir = target.position - transform.position;
            float distance = dir.magnitude;
            Vector2 force = (speed / distance) * dir.normalized; // Inverse distance for realism
            rb.AddForce(force, ForceMode2D.Impulse);
        }


        float str = (prev - rb.linearVelocity).magnitude;

        if (str > 20)
        {
            int damage = Mathf.FloorToInt((str - 20f) / endurance);
            health.Damage(damage);
        }
        
        prev = rb.linearVelocity;
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.GetComponent<PlayerController>())
        {
            target = collision.transform;
            state = State.Attacking;
        }

        //float str = collision.relativeVelocity.magnitude;

        //if (str > 20f)
        //{
        //    int damage = Mathf.FloorToInt((str - 20f) / endurance);
        //    health.Damage(damage);
        //}
    }

    public void OnStateUpdate()
    {

    }
}
