using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Astabba : Agent
{

    float HungerDecayRate = 0.1f;
    float ThirstDecayRate = 0.1f;
    float DesireToReproduceRate = 0.05f;

    // Start is called before the first frame update
    public Astabba() : base(0, SpeciesT.Astabba, new Vector3(4, 4, 4), 7f) { }

    public override void Start()
    {
        base.Start();
        gameObject.tag = "Astabba";
        SetState(StateT.Hungry);
    }

    public void Move()
    {
        Vector2 dir2D = Random.insideUnitCircle.normalized;
        Vector3 dir = new Vector3(dir2D.x, 1, dir2D.y);
        if (!IsGrounded) return;

        Vector3 v;
        if (State == StateT.Hungry) {
            dir = DirToClosestEnemy;
            dir.y = 1;
            v = Vector3.Scale(Velocity, dir);
        } else if (State == StateT.Idle)
        {
            v = Vector3.Scale(Velocity, dir);
        } else
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

        if (Hunger < Thirst)
            SetState(StateT.Hungry);
        else
            SetState(StateT.Thirsty);
    }

    // Update is called once per frame
    public override void Update()
    {
        base.Update();
        Step();
        Move();
    }
}
