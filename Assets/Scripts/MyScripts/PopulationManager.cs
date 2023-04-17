using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PopulationManager : GenerationManager
{
    [SerializeField] private float minimalPointsThreshHold = 0.5f;
    [SerializeField] private int minHerbOffspring = 0;
    [SerializeField] private int minCarnOffspring = 0;
    [SerializeField] private int minOmniOffspring = 0;
    private int _minimalOffspring;
    
    void Awake()
    {
        base.Awake();
    }

    protected override void GeneratePirates(PirateLogic[] pirateParents)
    {
         _minimalOffspring = Mathf.Max(pirateParentSize, minCarnOffspring);
         _activePirates = new List<PirateLogic>();
        
         int populationSize = ModulatePopulation(pirateGenerator);
         List<GameObject> objects = pirateGenerator.RegenerateObjects(populationSize);
         foreach (GameObject obj in objects)
         {
             PirateLogic pirate = obj.GetComponent<PirateLogic>();
             if (pirate != null)
             {
                 _activePirates.Add(pirate);
                 if (pirateParents != null)
                 {
                     PirateLogic pirateParent = pirateParents[Random.Range(0, pirateParents.Length)];
                     pirate.Birth(pirateParent.GetData());
                 }

                 pirate.Mutate(mutationFactor, mutationChance);
                 pirate.AwakeUp();
             }
         }
    }
     
    protected override void GenerateOmnivores(OmnivoreScript[] omnivoreParents)
    {
        _minimalOffspring = Mathf.Max(omnivoreParentSize, minOmniOffspring) ;
        _activeOmnivores = new List<OmnivoreScript>();
         
        int populationSize = ModulatePopulation(omnivoreGenerator);
        List<GameObject> objects = omnivoreGenerator.RegenerateObjects(populationSize);
        foreach (GameObject obj in objects)
        {
            OmnivoreScript omnivore = obj.GetComponent<OmnivoreScript>();
            if (omnivore != null)
            {
                _activeOmnivores.Add(omnivore);
                if (omnivoreParents != null)
                {
                    OmnivoreScript omnivoreParent = omnivoreParents[Random.Range(0, omnivoreParents.Length)];
                    omnivore.Birth(omnivoreParent.GetData());
                }

                omnivore.Mutate(mutationFactor, mutationChance);
                omnivore.AwakeUp();
            }
        }
    }
    
    protected override void GenerateBoats(BoatLogic[] boatParents)
    {
        _minimalOffspring = Mathf.Max(boatParentSize, minHerbOffspring);
        _activeBoats = new List<BoatLogic>();

        int populationSize = ModulatePopulation(boatGenerator);
        Debug.Log(populationSize);
        List<GameObject> objects = boatGenerator.RegenerateObjects(populationSize);
        foreach (GameObject obj in objects)
        {
            BoatLogic boat = obj.GetComponent<BoatLogic>();
            if (boat != null)
            {
                _activeBoats.Add(boat);
                if (boatParents != null)
                {
                    BoatLogic boatParent = boatParents[Random.Range(0, boatParents.Length)];
                    boat.Birth(boatParent.GetData());
                }

                boat.Mutate(mutationFactor, mutationChance);
                boat.AwakeUp();
            }
        }
    }

    private int ModulatePopulation(GenerateObjectsInArea generator)
    {
        int oldCount = generator.transform.childCount;
        int offspring = oldCount;
        if (offspring == 0) return _minimalOffspring;
        for (int i = oldCount; i <= 0; i--)
        {
            AgentLogic agent = generator.transform.GetChild(i).GetComponent<AgentLogic>();
            if (agent.GetPoints() <= minimalPointsThreshHold)
            {
                offspring--;
            }
        }
        offspring = Mathf.Max(offspring, _minimalOffspring);
        Debug.Log("Generator: "+generator +"offspring:"+offspring);
        return offspring;
    }
}
