using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Colokai : Agent
{
    public float HungerDecayRate { get; private set; }
    public float ThirstDecayRate { get; private set; }
    public float DesireToReproduceRate { get; private set; }

public Colokai() : base(0, SpeciesT.Colokai, new Vector3(3, 3, 3), 4f) { }

    // Start is called before the first frame update
    public override void Start()
    {
        base.Start();
        gameObject.tag = "Colokai";

        Fitness = 10000;
        HungerDecayRate = 2 / Fitness;
        ThirstDecayRate = 5 / Fitness;
        DesireToReproduceRate = 0.002f;
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

        Vector3 v;
        if (State == StateT.Fleeing)
        {
            dir = -DirToClosestEnemy;
            dir.y = 1;
            v = Vector3.Scale(Velocity, dir);
        }
        else if (State == StateT.Flirty)
        {
            // Move towards the closest Colokai that's also in the mood, consensually ofc
            Colokai closestColokai = null;
            float distance = float.PositiveInfinity;

            foreach (Colokai colokai in FriendlyNear)
            {
                float d = Vector3.Distance(gameObject.transform.position, colokai.gameObject.transform.position);
                if (colokai.State == StateT.Flirty & distance < d) {
                    closestColokai = colokai;
                    distance = d;
                }
            }
            if (closestColokai != null)
                dir = closestColokai.transform.position - gameObject.transform.position;

            dir.y = 1;
            v = Vector3.Scale(Velocity, dir);
        }
        else if (State == StateT.Idle)
        {
            v = Vector3.Scale(Velocity, dir);
        }
        else
        {
            v = Vector3.Scale(Velocity, dir);
        }

        gameObject.GetComponent<Rigidbody>().velocity = v;
        float rotateSpeed = GetComponent<Rigidbody>().velocity.magnitude * 50;
        transform.Rotate(0, rotateSpeed * Time.deltaTime, 0);

        RaycastHit hit;

        if (Physics.Raycast(transform.position, transform.TransformDirection(Vector3.down), out hit, 30))
        {
            if (hit.transform.gameObject.name == "Floor")
            {
                if (hit.distance > Velocity.y / 1.1)
                {
                    // There's a bug.... and I think this fixes it... hopefully...
                    Debug.Log("Caught Colokai Flying! They don't have wings!!");
                    caughtFlying();

                } else
                {
                    Color color = Terrain.WhereAmI(transform.position.y);
                    if (color.r < 50.0f / 255.0f && color.g < 50.0f / 255.0f && color.b > 100.0f / 255.0f)
                    {
                        Drink(0.2f); // drink in rivers
                    }
                    else if (color.r > 200.0f / 255.0f && color.g > 200.0f / 255.0f && color.b > 200.0f / 255.0f)
                    {
                        Eat(0.3f); // eat on mountains
                    }
                }
            }
        }
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

        // If we are not thirsty, old enough, and more flirty, Let's Marvin Gaye and Get It On
        if (Age > 18 & State != StateT.Thirsty & DesireToReproduce <= Hunger)
            SetState(StateT.Flirty);

        if (IsNearEnemy) // Prioritize self preservation first
            SetState(StateT.Fleeing);
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
            Colokai colokai = collision.collider.gameObject.GetComponent<Colokai>();
            if (colokai.State == StateT.Flirty && State == StateT.Flirty)
            {
                    Reproduce(colokai);
                    colokai.Reproduce(this);
            }
        }
    }
}
