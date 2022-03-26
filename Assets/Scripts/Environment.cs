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
            Vector2 pos = Random.insideUnitCircle;
            GameObject astabbaGameObject= new GameObject("Astabba");
            Astabba astabba = astabbaGameObject.AddComponent<Astabba>();
            astabbas.Add(astabba);
            astabbaGameObject.transform.SetParent(astabbaPopulation.transform, false);
            astabba.Spawn(new Vector3(SimulationSettings.Instance().WorldRadius * pos.x, 1, SimulationSettings.Instance().WorldRadius * pos.y));
        }

        for (int i = 0; i < SimulationSettings.Instance().colokaiInitalPop; i++)
        {
            Vector2 pos = Random.insideUnitCircle;
            GameObject colokaiGameObject = new GameObject("Colokai");
            Colokai colokai = colokaiGameObject.AddComponent<Colokai>();
            colokais.Add(colokai);
            colokaiGameObject.transform.SetParent(colokaiPopulation.transform, false);
            colokai.Spawn(new Vector3(SimulationSettings.Instance().WorldRadius * pos.x, 1, SimulationSettings.Instance().WorldRadius * pos.y));
        }

    }

    // Update is called once per frame
    public void Update()
    {
    }

    public void FixedUpdate()
    {
        SimulationSettings.Instance().tick();
    }

    public void Reproduce(Agent.SpeciesT species, Agent parent1, Agent parent2)
    {
        Vector3 vel = Random.value <= 0.5 ? parent1.Velocity : parent2.Velocity;
        float fitness = Random.value <= 0.5 ? parent1.Fitness : parent2.Fitness;
        float trackingDist = Random.value <= 0.5 ? parent1.BaseTrackingDistance : parent2.BaseTrackingDistance;

        Vector3 velMutation = Random.value <= 0.5 ? vel : vel * Random.Range(0.7f, 1.3f);
        float fitnessMutation = Random.value <= 0.5 ? fitness : fitness * Random.Range(0.7f, 1.3f);
        float trackingDistMutation = Random.value <= 0.5 ? trackingDist : trackingDist * Random.Range(0.7f, 1.3f);

        if (species == Agent.SpeciesT.Astabba)
        {
            GameObject astabbaGameObject = new GameObject("Astabba");
            Astabba astabba = astabbaGameObject.AddComponent<Astabba>();
            astabba.Create(velMutation, trackingDistMutation, fitnessMutation);
            astabbas.Add(astabba);
            astabbaGameObject.transform.SetParent(astabbaPopulation.transform, false);
            astabba.Spawn(new Vector3(parent1.transform.position.x, 1, parent2.transform.position.z));
        } else
        {
            GameObject colokaiGameObject = new GameObject("Colokai");
            Colokai colokai = colokaiGameObject.AddComponent<Colokai>();
            colokai.Create(velMutation, trackingDistMutation, fitnessMutation);
            colokais.Add(colokai);
            colokaiGameObject.transform.SetParent(colokaiPopulation.transform, false);
            colokai.Spawn(new Vector3(parent1.transform.position.x, 1, parent2.transform.position.z));
        }
    }
}
