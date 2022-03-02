using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Colokai : Agent
{

    float HungerDecayRate = 0.2f;
    float ThirstDecayRate = 0.05f;
    float DesireToReproduceRate = 0.2f;

    public Colokai() : base(0, SpeciesT.Colokai, new Vector3(3, 5, 3), 4f) { }

    // Start is called before the first frame update
    public override void Start()
    {
        base.Start();
        gameObject.tag = "Colokai";
    }

    public void Move()
    {
        Vector2 dir2D = Random.insideUnitCircle.normalized;
        Vector3 dir = new Vector3(dir2D.x, 1, dir2D.y);
        if (!IsGrounded) return;

        Vector3 v;
        if (State == StateT.Hungry)
        {
            dir = -DirToClosestEnemy;
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

    }

    public void Step()
    {
        DecayHunger(HungerDecayRate);
        DecayThirst(ThirstDecayRate);
        IncreaseDesireToReproduce(DesireToReproduceRate);
    }

    // Update is called once per frame
    public override void Update()
    {
        base.Update();
        Step();
        Move();
    }
}
