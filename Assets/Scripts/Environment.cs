using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Environment : MonoBehaviour
{

    GameObject astabbaPopulation;
    GameObject colokaiPopulation;

    List<Astabba> astabbas;
    List<Colokai> colokais;

    // Create Colokai
    // Create Astabbas
    // Create world

    // Game loop:
    // - Update terrain
    // - Update Colokai
    // - Update Astabbas

    // Start is called before the first frame update
    public void Start()
    {

        astabbas = new List<Astabba>();
        colokais = new List<Colokai>();

        astabbaPopulation = GameObject.FindWithTag("AstabbaPopulation");
        colokaiPopulation = GameObject.FindWithTag("ColokaiPopulation");

        for (int i = 0; i < SimulationSettings.Instance().astabbasInitalPop; i++)
        {
            GameObject astabba = new GameObject("Astabba");
            astabba.gameObject.transform.parent = astabbaPopulation.gameObject.transform;

            astabbas.Add(astabba.AddComponent<Astabba>());
        }

        for (int i = 0; i < SimulationSettings.Instance().colokaiInitalPop; i++)
        {
            GameObject colokai = new GameObject("Colokai");
            colokai.gameObject.transform.parent = colokaiPopulation.gameObject.transform;
            colokais.Add(colokai.AddComponent<Colokai>());
        }

    }

    // Update is called once per frame
    public void Update()
    {
    }
}
