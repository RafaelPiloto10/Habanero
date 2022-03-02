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

    public float Hunger { get; private set; } = 1;
    public float Thirst { get; private set; } = 1;
    public float DesireToReproduce { get; private set; } = 0;

    private class StateP
    {
        public StateT State { get; }
        public ref float Val { get { return ref Val; } }

        StateP(StateT state, ref float val)
        {
            State = state;
            Val = val;
        }
    }

    public float TrackingDistance { get; private set; }
    public List<Agent> FriendlyNear { get; private set; }
    public List<Agent> EnemyNear { get; private set; }

    public bool IsNearEnemy { get; private set; }
    public Vector3 DirToClosestEnemy { get; private set; }

    public bool IsGrounded { get; private set; }
    public string groundedName = "";
    

    Mesh mesh;
    Color color;

    public Agent(int age, SpeciesT species, Vector3 velocity, float trackingDistance)
    {
        Age = age;
        Species = species;
        State = StateT.Idle;
        Velocity = velocity;
        TrackingDistance = trackingDistance;
    }

    public void Eat(float food)
    {
        Hunger = Mathf.Min(food + Hunger, 1);
    }

    public void Drink(float fluid)
    {
        Thirst = Mathf.Min(1, Thirst + fluid);
    }

    public void Reproduce()
    {
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
        float distanceToClosestEnemy = float.PositiveInfinity;
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
                } else
                {
                    FriendlyNear.Add(agent);
                }
            }
        }

        IsNearEnemy = EnemyNear.Count > 0;

        return nearBy;
    }

    // Start is called before the first frame update
    public virtual void Start()
    {
        gameObject.AddComponent<MeshFilter>();
        gameObject.AddComponent<MeshRenderer>();
        gameObject.AddComponent<BoxCollider>();
        gameObject.AddComponent<Rigidbody>();
        gameObject.AddComponent<MeshCollider>();   

        Material material = new Material(Shader.Find("Standard"));
        gameObject.GetComponent<Rigidbody>().freezeRotation = true;

        if (Species == SpeciesT.Astabba)
        {
            color = Color.red;
            
            gameObject.transform.position = new Vector3(1, 3, 0);
        }
        else
        {
            color = Color.blue;
            gameObject.transform.position = new Vector3(0, 3, 1);
        }

        material.color = color;

        mesh = Resources.GetBuiltinResource<Mesh>("Cube.fbx");
        gameObject.GetComponent<MeshFilter>().mesh = mesh;
        gameObject.GetComponent<Renderer>().material = material;
    }

    // Update is called once per frame
    public virtual void Update()
    {
        Track();
        if (SimulationSettings.Instance().IsDebug())
        {
            DrawDebugGizmos();
        }
    }

    public void OnCollisionEnter(Collision collision)
    {
        if ((gameObject.transform.position - collision.collider.gameObject.transform.position).y > 0)
        {
            gameObject.GetComponent<Rigidbody>().velocity = Vector3.zero;
            IsGrounded = true;
            groundedName = collision.collider.gameObject.name;
        } else if (collision.gameObject.name == "Floor")
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
        } else if(collision.collider.gameObject.name == groundedName)
        {
            IsGrounded = false;
            groundedName = "";
        }
    }

    public void DrawDebugGizmos()
    {
        DrawDebugAxis();
        DrawDebugRadius();
    }

    public void DrawDebugAxis()
    {
        // Draw axis -- ensure that editor gizmos are set
        float showTime = 0.01f;
        Debug.DrawLine(gameObject.transform.position, gameObject.transform.position + Vector3.forward, Color.green, showTime, true);
        Debug.DrawLine(gameObject.transform.position, gameObject.transform.position + Vector3.back, Color.green, showTime, true);
        Debug.DrawLine(gameObject.transform.position, gameObject.transform.position + Vector3.right, Color.green, showTime, true);
        Debug.DrawLine(gameObject.transform.position, gameObject.transform.position + Vector3.left, Color.green, showTime, true);
        if (IsNearEnemy)
            Debug.DrawLine(gameObject.transform.position, gameObject.transform.position + (TrackingDistance / 2 * DirToClosestEnemy), Color.red, showTime, true);
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
            
        }

        line = gameObject.GetComponent<LineRenderer>();
        if (IsNearEnemy)
        {
            line.startColor = Color.red;
            line.endColor = Color.red;
        } else
        {
            line.startColor = Color.white;
            line.endColor = Color.white;
        }

        float x;
        float y;
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

    public void SetState(StateT state)
    {
        State = state;
    }
}
