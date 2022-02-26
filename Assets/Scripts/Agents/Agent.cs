using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Agent : MonoBehaviour
{

    public enum Species { Astabba, Colokai };

    public enum State { Idle };

    protected State state;

    int age;
    Species species;
    Mesh mesh;
    bool isGrounded;
    Color color;

    public Agent(int age, Species species)
    {
        this.age = age;
        this.species = species;
        this.state = State.Idle;
    }


    public int GetAge()
    {
        return age;
    }

    public Species GetSpecies()
    {
        return species;
    }

    public bool IsGrounded()
    {
        return isGrounded;
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

        if (species == Species.Astabba)
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
        if (SimulationSettings.Instance().IsDebug())
        {
            float showTime = 0.01f;
            Debug.DrawLine(gameObject.transform.position, gameObject.transform.position + Vector3.forward, Color.green, showTime, true);
            Debug.DrawLine(gameObject.transform.position, gameObject.transform.position + Vector3.back, Color.green, showTime, true);
            Debug.DrawLine(gameObject.transform.position, gameObject.transform.position + Vector3.right, Color.green, showTime, true);
            Debug.DrawLine(gameObject.transform.position, gameObject.transform.position + Vector3.left, Color.green, showTime, true);
        }
    }

    public void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.name == "Floor")
        {
            gameObject.GetComponent<Rigidbody>().velocity = Vector3.zero;
            isGrounded = true;
        }
    }

    public void OnCollisionExit(Collision collision)
    {
        if (collision.gameObject.name == "Floor")
        {
            isGrounded = false;
        }
    }
}
