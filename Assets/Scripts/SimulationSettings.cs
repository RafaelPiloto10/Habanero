using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SimulationSettings : MonoBehaviour
{
    private static SimulationSettings m_Instance;

    public bool debug = true;

    [Range(1, 2)]
    public int astabbasInitalPop = 1;

    [Range(1, 2)]
    public int colokaiInitalPop = 1;

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

        }

        return m_Instance;
    }
}
