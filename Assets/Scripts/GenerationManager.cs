using System;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using Random = UnityEngine.Random;

public class GenerationManager : MonoBehaviour
{
    [Header("Generators")]
    [SerializeField]
    protected GenerateObjectsInArea[] boxGenerators;
    [SerializeField]
    protected GenerateObjectsInArea boatGenerator;
    [SerializeField]
    protected GenerateObjectsInArea pirateGenerator;
    [SerializeField] 
    protected GenerateObjectsInArea omnivoreGenerator;


    [Space(10)]
    [Header("Parenting and Mutation")]
    [SerializeField]
    protected float mutationFactor;
    [SerializeField] 
    protected float mutationChance;
    [SerializeField] 
    protected int boatParentSize;
    [SerializeField] 
    protected int pirateParentSize;
    [SerializeField] 
    protected int omnivoreParentSize;

    [Space(10)] 
    [Header("Simulation Controls")]
    [SerializeField, Tooltip("Time per simulation (in seconds).")]
    private float simulationTimer;
    [SerializeField, Tooltip("Current time spent on this simulation.")]
    private float simulationCount;
    [SerializeField, Tooltip("Automatically starts the simulation on Play.")]
    private bool runOnStart;
    [SerializeField, Tooltip("Initial count for the simulation. Used for the Prefabs naming.")]
    private int generationCount;

    [Space(10)] 
    [Header("Prefab Saving")]
    [SerializeField] 
    private bool savePrefabs;
    [SerializeField] 
    private bool runTill100;
    [SerializeField]
    private string savePrefabsAt;
    
    /// <summary>
    /// Those variables are used mostly for debugging in the inspector.
    /// </summary>
    [Header("Former winners")]
    [SerializeField]
    private AgentData lastBoatWinnerData;
    [SerializeField]
    private AgentData lastPirateWinnerData;
    [SerializeField] 
    private AgentData lastOmnivoreWinnerData;

    private bool _runningSimulation;
    protected List<BoatLogic> _activeBoats;
    protected List<PirateLogic> _activePirates;
    protected List<OmnivoreScript> _activeOmnivores;
    private BoatLogic[] _boatParents;
    private PirateLogic[] _pirateParents;
    private OmnivoreScript[] _omnivoreParents;

    protected bool firstRun = true;
    
    protected void Awake()
    {
        Random.InitState(6);
    }

    private void Start()
    {
        if (runOnStart)
        {
            StartSimulation();
        }
    }
    
    private void Update()
    {
        if (_runningSimulation)
        {
            //Creates a new generation.
            if (simulationCount >= simulationTimer)
            {
                ++generationCount;
                MakeNewGeneration();
                simulationCount = -Time.deltaTime;
            } 
            simulationCount += Time.deltaTime;
        }
    }

     
    /// <summary>
    /// Generates the boxes on all box areas.
    /// </summary>
    public void GenerateBoxes()
    {
        foreach (GenerateObjectsInArea generateObjectsInArea in boxGenerators)
        {
            generateObjectsInArea.RegenerateObjects();
        }
    }

    /// <summary>
    /// Generates boats and pirates using the parents list.
    /// If no parents are used, then they are ignored and the boats/pirates are generated using the default prefab
    /// specified in their areas.
    /// </summary>
    /// <param name="boatParents"></param>
    /// <param name="pirateParents"></param>
    /// <param name="omnivoreParents"></param>
    public void GenerateObjects(BoatLogic[] boatParents = null, PirateLogic[] pirateParents = null, OmnivoreScript[] omnivoreParents = null)
    {
        GenerateBoats(boatParents);
        GeneratePirates(pirateParents);
        GenerateOmnivores(omnivoreParents);
        firstRun = false;
    }

     /// <summary>
     /// Generates the list of pirates using the parents list. The parent list can be null and, if so, it will be ignored.
     /// Newly created pirates will go under mutation (MutationChances and MutationFactor will be applied).
     /// Newly create agents will be Awaken (calling AwakeUp()).
     /// </summary>
     /// <param name="pirateParents"></param>
     protected virtual void GeneratePirates(PirateLogic[] pirateParents)
    {
        _activePirates = new List<PirateLogic>();
        List<GameObject> objects = pirateGenerator.RegenerateObjects();
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
     
     protected virtual void GenerateOmnivores(OmnivoreScript[] omnivoreParents)
     {
         _activeOmnivores = new List<OmnivoreScript>();
         List<GameObject> objects = omnivoreGenerator.RegenerateObjects();
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

     /// <summary>
     /// Generates the list of boats using the parents list. The parent list can be null and, if so, it will be ignored.
     /// Newly created boats will go under mutation (MutationChances and MutationFactor will be applied).
     /// /// Newly create agents will be Awaken (calling AwakeUp()).
     /// </summary>
     /// <param name="boatParents"></param>
    protected virtual void GenerateBoats(BoatLogic[] boatParents)
    {
        _activeBoats = new List<BoatLogic>();
        List<GameObject> objects = boatGenerator.RegenerateObjects();
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

     /// <summary>
     /// Creates a new generation by using GenerateBoxes and GenerateBoats/Pirates.
     /// Previous generations will be removed and the best parents will be selected and used to create the new generation.
     /// The best parents (top 1) of the generation will be stored as a Prefab in the [savePrefabsAt] folder. Their name
     /// will use the [generationCount] as an identifier.
     /// </summary>
    public void MakeNewGeneration()
    {
        Random.InitState(6);

        GenerateBoxes();
        
        //Fetch parents
        _activeBoats.RemoveAll(item => item == null);
        _activeBoats.Sort();
        if (_activeBoats.Count < boatParentSize)
        {
            GenerateBoats(_boatParents);
        }
        _boatParents = new BoatLogic[boatParentSize];
        for (int i = 0; i < boatParentSize; i++)
        {
            _boatParents[i] = _activeBoats[i];
        }

        BoatLogic lastBoatWinner = _activeBoats[0];
        lastBoatWinner.name += "Gen-" + generationCount; 
        lastBoatWinnerData = lastBoatWinner.GetData();
        if (savePrefabs) PrefabUtility.SaveAsPrefabAsset(lastBoatWinner.gameObject, savePrefabsAt + lastBoatWinner.name + ".prefab");
        
        //Carnivores (Pirates)
        _activePirates.RemoveAll(item => item == null);
        _activePirates.Sort();
        if (_activePirates.Count < pirateParentSize)
        {
            GeneratePirates(_pirateParents);
        }
        _pirateParents = new PirateLogic[pirateParentSize];
        for (int i = 0; i < pirateParentSize; i++)
        {
            _pirateParents[i] = _activePirates[i];
        }

        PirateLogic lastPirateWinner = _activePirates[0];
        lastPirateWinner.name += "Gen-" + generationCount; 
        lastPirateWinnerData = lastPirateWinner.GetData();
        if (savePrefabs) PrefabUtility.SaveAsPrefabAsset(lastPirateWinner.gameObject, savePrefabsAt + lastPirateWinner.name + ".prefab");
        
        //Omnivores
        _activeOmnivores.RemoveAll(item => item == null);
        _activeOmnivores.Sort();
        if (_activeOmnivores.Count < omnivoreParentSize)
        {
            GenerateOmnivores(_omnivoreParents);
        }
        _omnivoreParents = new OmnivoreScript[omnivoreParentSize];
        for (int i = 0; i < omnivoreParentSize; i++)
        {
            _omnivoreParents[i] = _activeOmnivores[i];
        }

        OmnivoreScript lastOmnivoreWinner = _activeOmnivores[0];
        lastOmnivoreWinner.name += "Gen-" + generationCount; 
        lastOmnivoreWinnerData = lastOmnivoreWinner.GetData();
        if (savePrefabs) PrefabUtility.SaveAsPrefabAsset(lastOmnivoreWinner.gameObject, savePrefabsAt + lastOmnivoreWinner.name + ".prefab");
        if (runTill100 && generationCount == 100)
        {
            PrefabUtility.SaveAsPrefabAsset(lastBoatWinner.gameObject, savePrefabsAt + lastBoatWinner.name + ".prefab");
            PrefabUtility.SaveAsPrefabAsset(lastPirateWinner.gameObject, savePrefabsAt + lastPirateWinner.name + ".prefab");
            PrefabUtility.SaveAsPrefabAsset(lastOmnivoreWinner.gameObject, savePrefabsAt + lastOmnivoreWinner.name + ".prefab");
            EditorApplication.isPlaying = false;
        }
        //Winners:
        // Debug.Log("Last winner boat had: " + lastBoatWinner.GetPoints() + " points!" + " Last winner pirate had: " + lastPirateWinner.GetPoints() + " points!" 
        //           + "Last winner Omni had: " + lastOmnivoreWinner.GetPoints() + " points!" );
        
        GenerateObjects(_boatParents, _pirateParents, _omnivoreParents);
        
        Logger.instance.SaveData(generationCount, lastBoatWinner.GetPoints(), lastPirateWinner.GetPoints(), lastOmnivoreWinner.GetPoints(), 
            boatGenerator.transform.childCount, pirateGenerator.transform.childCount, omnivoreGenerator.transform.childCount);
    }

     /// <summary>
     /// Starts a new simulation. It does not call MakeNewGeneration. It calls both GenerateBoxes and GenerateObjects and
     /// then sets the _runningSimulation flag to true.
     /// </summary>
    public void StartSimulation()
    {
        Random.InitState(6);

        GenerateBoxes();
        GenerateObjects();
        _runningSimulation = true;
    }

     /// <summary>
     /// Continues the simulation. It calls MakeNewGeneration to use the previous state of the simulation and continue it.
     /// It sets the _runningSimulation flag to true.
     /// </summary>
     public void ContinueSimulation()
     {
         MakeNewGeneration();
         _runningSimulation = true;
     }
     
     /// <summary>
     /// Stops the count for the simulation. It also removes null (Destroyed) boats from the _activeBoats list and sets
     /// all boats and pirates to Sleep.
     /// </summary>
    public void StopSimulation()
    {
        _runningSimulation = false;
        _activeBoats.RemoveAll(item => item == null);
        _activeBoats.ForEach(boat => boat.Sleep());
        _activePirates.ForEach(pirate => pirate.Sleep());
    }
}
