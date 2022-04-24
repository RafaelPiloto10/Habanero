using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Agent : MonoBehaviour
{

    public enum SpeciesT { Astabba, Colokai };
    public SpeciesT Species { get; private set; }

    public enum StateT { Idle, Hungry, Fleeing, Thirsty, Flirty };
    public StateT State { get; private set; }

    public int Age { get; private set; }
    public Vector3 Velocity { get; private set; }

    public float Fitness { get; set; }
    public float Hunger { get; private set; } = 1;
    public float Thirst { get; private set; } = 1;
    public float DesireToReproduce { get; private set; } = 0;

    public float TrackingDistance { get; private set; }
    public float BaseTrackingDistance { get; set; }
    public List<Agent> FriendlyNear { get; private set; }
    public List<Agent> EnemyNear { get; private set; }

    public bool IsNearEnemy { get; private set; }

    public Vector3 DirToClosestEnemy { get; private set; }
    public Vector3 DirToClosestFlirtyFriendly { get; private set; }

    public bool IsGrounded { get; private set; }
    private string groundedName = "";

    Mesh mesh;
    Color color;

    public Agent(int age, SpeciesT species, Vector3 velocity, float trackingDistance)
    {
        Age = age;
        Species = species;
        State = StateT.Idle;
        Velocity = velocity;
        BaseTrackingDistance = trackingDistance;
        TrackingDistance = BaseTrackingDistance * Mathf.Pow(2, Hunger);

    }

    public void Create(Vector3 velocity, float trackingDistance)
    {
        Velocity = velocity;
        BaseTrackingDistance = trackingDistance;
    }

    public void Eat(float food)
    {
        Hunger = Mathf.Min(food + Hunger, 1);
    }

    public void Drink(float fluid)
    {
        Debug.Log("Drinking!");
        Thirst = Mathf.Min(1, Thirst + fluid);
    }

    public void Reproduce(Agent other)
    {
        if (Age > 18) {
            SimulationSettings.Instance().GetEnvironment().Reproduce(Species, this, other);
        }
        DesireToReproduce = 0;
    }

    public void DecayHunger(float decay)
    {
        Hunger = Mathf.Max(0, Hunger - decay);
    }

    public void DecayThirst(float thirst)
    {
        Thirst = Mathf.Max(0, Thirst - thirst);
    }

    public void IncreaseDesireToReproduce(float desire)
    {
        DesireToReproduce = Mathf.Min(1, DesireToReproduce + desire);
    }

    public Collider[] Track()
    {
        Collider[] nearBy = Physics.OverlapSphere(gameObject.transform.position, TrackingDistance);

        FriendlyNear = new List<Agent>();
        EnemyNear = new List<Agent>();

        DirToClosestEnemy = Vector3.zero;
        DirToClosestFlirtyFriendly = Vector3.zero;

        float distanceToClosestEnemy = float.PositiveInfinity;
        float distanceToClosestFlirtyFriendly = float.PositiveInfinity;


        foreach (Collider other in nearBy)
        {
            Agent agent;
            agent = other.gameObject.GetComponent<Astabba>();
            if (agent == null) agent = other.gameObject.GetComponent<Colokai>();
            if (agent != null)
            {
                if (Species != agent.Species)
                {
                    EnemyNear.Add(agent);
                    float dist = Mathf.Abs(Vector3.Distance(agent.gameObject.transform.position, gameObject.transform.position));
                    if (distanceToClosestEnemy > dist)
                    {
                        distanceToClosestEnemy = dist;
                        DirToClosestEnemy = (agent.gameObject.transform.position - gameObject.transform.position).normalized;
                    }
                }
                else if (agent != this)
                {
                    FriendlyNear.Add(agent);
                    if (agent.State == StateT.Flirty)
                    {
                        float dist = Mathf.Abs(Vector3.Distance(agent.gameObject.transform.position, gameObject.transform.position));
                        if (distanceToClosestFlirtyFriendly > dist)
                        {
                            distanceToClosestFlirtyFriendly = dist;
                            DirToClosestFlirtyFriendly = (agent.transform.position - gameObject.transform.position).normalized;
                        }
                    }
                }
            }
        }

        IsNearEnemy = EnemyNear.Count > 0;

        return nearBy;
    }

    public void Spawn(Vector3 loc)
    {
        gameObject.transform.position = loc;
    }

    // Start is called before the first frame update
    public virtual void Start()
    {
    }

    public void FixedUpdate()
    {
        if(SimulationSettings.Instance().ticks % SimulationSettings.Instance().TicksPerDay == 0)
        {
            Age++;
            // TODO: Write age death
        }
    }

    // Update is called once per frame
    public virtual void Update()
    {
        TrackingDistance = BaseTrackingDistance * Mathf.Pow(2, Hunger);
        Track();
        DrawDebugGizmos();
    }

    public virtual void OnCollisionEnter(Collision collision)
    {
        if ((gameObject.transform.position - collision.collider.gameObject.transform.position).y > 0)
        {
            gameObject.GetComponent<Rigidbody>().velocity = Vector3.zero;
            IsGrounded = true;
            groundedName = collision.collider.gameObject.name;
        }
        else if (collision.gameObject.name == "Floor")
        {
            gameObject.GetComponent<Rigidbody>().velocity = Vector3.zero;
            IsGrounded = true;
        }
    }

    public void OnCollisionExit(Collision collision)
    {
        if (collision.gameObject.name == "Floor")
        {
            IsGrounded = false;
        }
        else if (collision.collider.gameObject.name == groundedName)
        {
            IsGrounded = false;
            groundedName = "";
        }
    }

    public void DrawDebugGizmos()
    {
        if (SimulationSettings.Instance().IsDebug())
        {
            DrawDebugAxis();
        }

        DrawDebugRadius();
    }

    public void DrawDebugAxis()
    {
        // Draw axis -- ensure that editor gizmos are set
        float showTime = 0.01f;
        Color color = Color.green;

        if (State == StateT.Fleeing)
            color = Color.yellow;
        else if (State == StateT.Flirty)
            color = Color.magenta;
        else if (State == StateT.Hungry)
            color = Color.red;
        else if (State == StateT.Idle)
            color = Color.white;
        else if (State == StateT.Thirsty)
            color = Color.blue;

        Debug.DrawLine(gameObject.transform.position, gameObject.transform.position + Vector3.forward, color, showTime, true);
        Debug.DrawLine(gameObject.transform.position, gameObject.transform.position + Vector3.back, color, showTime, true);
        Debug.DrawLine(gameObject.transform.position, gameObject.transform.position + Vector3.right, color, showTime, true);
        Debug.DrawLine(gameObject.transform.position, gameObject.transform.position + Vector3.left, color, showTime, true);

        Vector3 end = Vector3.zero;

        if (State == StateT.Hungry && IsNearEnemy)
            end = TrackingDistance / 2 * DirToClosestEnemy;

        else if (State == StateT.Flirty)
            end = (TrackingDistance / 2 * DirToClosestFlirtyFriendly);

        Debug.DrawLine(gameObject.transform.position, gameObject.transform.position + end, color, showTime, true);
    }

    public void DrawDebugRadius()
    {
        int segments = 128;
        LineRenderer line;

        if (gameObject.GetComponent<LineRenderer>() == null)
        {
            line = gameObject.AddComponent<LineRenderer>();
            line.positionCount = segments + 1;
            line.useWorldSpace = false;
            line.startWidth = 0.1f;
            line.endWidth = 0.1f;
            line.material = new Material(Shader.Find("Mobile/Particles/Additive"));

            float x;
            float z;

            float angle = 20f;

            for (int i = 0; i < (segments + 1); i++)
            {
                x = Mathf.Sin(Mathf.Deg2Rad * angle) * TrackingDistance;
                z = Mathf.Cos(Mathf.Deg2Rad * angle) * TrackingDistance;

                line.SetPosition(i, new Vector3(x, 0, z));

                angle += (360f / segments);
            }

        }

        line = gameObject.GetComponent<LineRenderer>();

        line.enabled = SimulationSettings.Instance().IsDebug();

        if (IsNearEnemy)
        {
            line.startColor = Color.red;
            line.endColor = Color.red;
        }
        else
        {
            Color color = Terrain.WhereAmI(transform.position.y);
            line.startColor = color;
            line.endColor = color;
        }
    }

    public void SetState(StateT state)
    {
        State = state;
    }

    public void caughtFlying()
    {
        IsGrounded = false;
        gameObject.GetComponent<Rigidbody>().velocity = Vector3.down;
    }
}
