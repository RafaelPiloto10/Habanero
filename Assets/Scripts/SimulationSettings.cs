using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SimulationSettings : MonoBehaviour
{
    private static SimulationSettings m_Instance;

    public bool debug = true;

    [Range(1, 10)]
    public int astabbasInitalPop = 1;

    [Range(1, 10)]
    public int colokaiInitalPop = 1;

    [Range(0, 1)]
    public float ReproductionSuccessRate = 0.15f;

    [Range(0, 30)]
    public int WorldRadius = 10;

    public static Environment environment;

    [Range(50, 200)]
    public ulong TicksPerDay = 100;

    public ulong ticks = 0;


    public void ToggleDebug()
    {
        debug = !debug;
    }

    public bool IsDebug()
    {
        return debug;
    }

    public static SimulationSettings Instance()
    {
        if (m_Instance == null)
        {
            m_Instance = GameObject.FindGameObjectWithTag("World").GetComponent<SimulationSettings>();

            environment = m_Instance.GetComponent<Environment>();
        }

        return m_Instance;
    }

    public Environment GetEnvironment()
    {
        return environment;
    }

    public void tick()
    {
        ticks += 1;
    }
}
