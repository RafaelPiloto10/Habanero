using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Astabba : Agent
{
    Vector3 vel = new Vector3(2, 7, 2);

    // Start is called before the first frame update
    public Astabba() : base(0, Species.Astabba) { }

    public override void Start()
    {
        base.Start();
        gameObject.tag = "Astabba";

    }

    public void move()
    {
        Vector2 dir2D = Random.insideUnitCircle.normalized;
        Vector3 dir = new Vector3(dir2D.x, 1, dir2D.y);

        Vector3 v = Vector3.Scale(vel, dir);

        if (!IsGrounded()) return;

        if (state == State.Idle)
        {
            gameObject.GetComponent<Rigidbody>().velocity = v ;
        }
    }

    // Update is called once per frame
    public override void Update()
    {
        base.Update();
        move();
    }
}
