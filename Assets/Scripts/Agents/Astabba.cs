using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Astabba : Agent
{
    public float HungerDecayRate { get; private set; }
    public float ThirstDecayRate { get; private set; }
    public float DesireToReproduceRate { get; private set; } = 0.00005f;

    // Start is called before the first frame update
    public Astabba() : base(0, SpeciesT.Astabba, new Vector3(2, 2, 2), 7f) { }

    public override void Start()
    {
        base.Start();
        gameObject.tag = "Astabba";
        SetState(StateT.Hungry);

        Fitness = 10000;
        HungerDecayRate = 2 / Fitness;
        ThirstDecayRate = 1 / Fitness;
    }

    public void Create(Vector3 velocity, float trackingDistance, float fitness)
    {   // Agent Create
        Create(velocity, trackingDistance);

        Fitness = fitness;
        HungerDecayRate = 2 / Fitness;
        ThirstDecayRate = 1 / Fitness;
    }

    public void Move()
    {
        Vector2 dir2D = Random.insideUnitCircle.normalized;
        Vector3 dir = new Vector3(dir2D.x, 1, dir2D.y);

        if (!IsGrounded) return;

        if (State == StateT.Hungry)
        {
            // Move towards enemy, otherwise randomly search for enemy
            if (DirToClosestEnemy != Vector3.zero)
            {
                dir = DirToClosestEnemy;
            }
        } else if (State == StateT.Thirsty) {
            // Move randomly
        } else if (State == StateT.Flirty)
        {
            // Move towards the closest friendly that's also in the mood, consensually ofc
            // Search randomly if no one is nearby
            if (DirToClosestFlirtyFriendly != Vector3.zero)
            {
                dir = DirToClosestFlirtyFriendly;
            }
        }
        else if (State == StateT.Idle)
        {
            // Move randomly
        }
        else
        {
            // Move Randomly
        }

        dir.y = 1;
        Vector3 v = Vector3.Scale(Velocity, dir);

        if (transform.position.y > Velocity.y)
        {
            // There's a bug.... and I think this fixes it... hopefully...
            Debug.Log("Caught Astabba Flying! They don't have wings!!");
            caughtFlying();
        }

        gameObject.GetComponent<Rigidbody>().velocity = v;
        float rotateSpeed = GetComponent<Rigidbody>().velocity.magnitude * 50;
        transform.Rotate(0, rotateSpeed * Time.deltaTime, 0);
    }

    public void Step()
    {
        DecayHunger(HungerDecayRate);
        DecayThirst(ThirstDecayRate);
        IncreaseDesireToReproduce(DesireToReproduceRate);

        if (Thirst <= Hunger) // Prioritize Thirst
            SetState(StateT.Thirsty);
        else // Otherwise, eat
            SetState(StateT.Hungry);

        // If we are not thirsty old enough, and more flirty, Let's Marvin Gaye and Get It On
        if (Age > 18 & State != StateT.Thirsty & DesireToReproduce <= Hunger)
            SetState(StateT.Flirty);
    }

    public void FixedUpdate()
    {
    }

    // Update is called once per frame
    public override void Update()
    {
        base.Update();
        Step();
        Move();
    }

    public override void OnCollisionEnter(Collision collision)
    {
        base.OnCollisionEnter(collision);

        if (collision.collider.gameObject.GetComponent<Colokai>() != null)
        {
            gameObject.GetComponent<Rigidbody>().velocity = Vector3.zero;
            Destroy(collision.collider.gameObject);
            Eat(1);
            gameObject.GetComponent<Rigidbody>().velocity = Vector3.zero;
        } else if(collision.collider.gameObject.GetComponent<Astabba>() != null)
        {
            Astabba astabba = collision.collider.gameObject.GetComponent<Astabba>();
            if (astabba.State == StateT.Flirty && State == StateT.Flirty)
            {
                    Reproduce(astabba);
                    astabba.Reproduce(this);
            }
        }
    }
}
