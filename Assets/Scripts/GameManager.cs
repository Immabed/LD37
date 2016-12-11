using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour {

    [SerializeField]
    private Engine engine;
    [SerializeField]
    private ShipStorage storage;
    [SerializeField]
    private FuelTank fuel;
    [SerializeField]
    private PowerGenerator power;

    private float timeScale = 1;

    public float TimeScale { get { return timeScale; } }

    public int PowerUsed { get { return engine.CurrentPower + storage.CurrentPower + fuel.CurrentPower;  } }

    int credits;

    public int Credits { get { return credits; } }

    public void ReplaceSubsystem(Subsystem sys)
    {
        if (sys is Engine)
        {
            engine = sys as Engine;
        }
    }

    public float GetFuelUse()
    {
        return engine.CurrentFuelDraw;
    }

    public void UpdateSystems()
    {
        // TODO - Check fuel/engine
    }

    public void EndGame(string endGameMessage)
    {
        // TODO - Implement end game
    }

    // Determine if the ship has enough resources for a repair recipe
    public bool HasResources(RepairRecipe rec)
    {
        return (storage.StoredSpareParts >= rec.SparePartsNeeded
            && storage.StoredPowerCells >= rec.PowerCellsNeeded
            && storage.StoredComputers >= rec.ComputersNeeded);
    }

    public int EnergyAvailable()
    {
        return power.CurrentPowerGeneration - PowerUsed;
    }
}
