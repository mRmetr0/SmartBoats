using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PopulationManager : GenerationManager
{
    [Header("Minimal Thresholds")]
    [SerializeField] private float minHerbThreshHold = 0f;
    [SerializeField] private float minCarnThreshHold = 0f;
    [SerializeField] private float minOmniThreshHold = 0f;
    
    [Space(5)]
    [Header("Minimal Growth")]
    [SerializeField] private int herbGrowth = 0;
    [SerializeField] private int carniGrowth = 0;
    [SerializeField] private int omniGrowth = 0;
    
    [Space(5)]
    [Header("Minimal Offsprings")]
    [SerializeField] private int minHerbOffspring = 0;
    [SerializeField] private int minCarnOffspring = 0;
    [SerializeField] private int minOmniOffspring = 0;
    
    [Space(5)]
    [Header("Apex threshold")]
    [SerializeField] private float apexHerbThreshold = 0;
    [SerializeField] private float apexCarnThreshold = 0;
    [SerializeField] private float apexOmniThreshold = 0;
    private float _minimalThreshold;
    private float _apexThreshold;
    private int _minimalOffspring;
    private int _constGrowth;
    
    void Awake()
    {
        base.Awake();
    }

    protected override void GeneratePirates(PirateLogic[] pirateParents)
    {
        if (firstRun)
        {
            base.GeneratePirates(pirateParents);
            return;
        }
        
        _minimalThreshold = minCarnThreshHold;
        _apexThreshold = apexCarnThreshold;
        _minimalOffspring = Mathf.Max(pirateParentSize, minCarnOffspring);
        _constGrowth = carniGrowth;
        
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
        if (firstRun)
        {
            base.GenerateOmnivores(omnivoreParents);
            return;
        }

        _minimalThreshold = minOmniThreshHold;
        _apexThreshold = apexOmniThreshold;
        _minimalOffspring = Mathf.Max(omnivoreParentSize, minOmniOffspring);
        _constGrowth = omniGrowth;
        
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
        if (firstRun)
        {
            base.GenerateBoats(boatParents);
            return;
        }

        _minimalThreshold = minHerbThreshHold;
        _minimalOffspring = Mathf.Max(boatParentSize, minHerbOffspring);
        _apexThreshold = apexHerbThreshold;
        _constGrowth = herbGrowth;
        
        _activeBoats = new List<BoatLogic>();
        int populationSize = ModulatePopulation(boatGenerator);
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
        if (oldCount == 0)
        {
            return _minimalOffspring;
        }
        int offspring = oldCount;
        for (int i = 0; i < oldCount; i++)
        {
            float agentPoints = generator.transform.GetChild(i).GetComponent<AgentLogic>().GetPoints();
            if (agentPoints < _minimalThreshold)
            {
                offspring = Mathf.Max(offspring - 1, _minimalOffspring);
            } else if (agentPoints >= _apexThreshold)
            {
                offspring += 1 * (int)(agentPoints/_apexThreshold);
            }
        }
        offspring = Mathf.Clamp(offspring, _minimalOffspring, 200);
        //Debug.Log("Generator: "+generator +"offspring:"+offspring);
        return offspring + _constGrowth;
    }
}
