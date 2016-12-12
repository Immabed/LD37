using System;
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
    [SerializeField]
    private CargoSystem cargo;
    [SerializeField]
    private ShipCommand cockpit;
    [SerializeField]
    private DataBank dataBank;
    [SerializeField]
    private LifeSupport lifeSupport;
	[SerializeField]
	Vendor[] vendorObjects;
	[SerializeField]
	UpgradeList upgrades;
	[SerializeField]
	CargoPool[] cargoPools;
	[SerializeField]
	VendorNamePool[] vendors;
	[SerializeField]
	ResourceAvailability[] resourcePrices;
	[SerializeField]
	Vector2 fuelPriceRange;
	[SerializeField]
	Vector2 fuelBaseAmountRange;



    int credits;
    int cargoDelivered;

    private float timeScale = 1;

    public float TimeScale { get { return timeScale; } }
    public int PowerUsed { get { return engine.CurrentPower + storage.CurrentPower + fuel.CurrentPower;  } }
    public float CurrentSpeed {  get { return engine.CurrentSpeed; } }
	public int Credits { get { return credits; } }
    public int CargoDelivered {  get { return cargoDelivered; } }



	private void Awake() {
		Array.Sort<CargoPool>(cargoPools, (x, y) => x.minimumDifficulty.CompareTo(y.minimumDifficulty));
	}




	public void ArrivedAtDestination()
	{
		if (cockpit.DistanceToDestination <= 0)
		{
			timeScale = 0;
			// Add to cargo deliverd
			credits += cargo.CollectCargo(); // Implement something interesting
			// DO SOMETHING
		}
	}

	public int EngineLevel 
	{ 
		get 
		{
			for (int i = 0; i < upgrades.engines.Length; i++)
			{
				if (upgrades.engines[i].ID == engine.ID)
				{
					return i;
				}
			}
			Debug.Log("Engine ID did not match an engine in upgrade list");
			return 0;
		} 
	}
	public int FuelTankLevel
	{
		get
		{
			for (int i = 0; i < upgrades.fuelTanks.Length; i++)
			{
				if (upgrades.fuelTanks[i].ID == fuel.ID)
				{
					return i;
				}
			}
			Debug.Log("FuelTank ID did not match an engine in upgrade list");
			return 0;
		}
	}
	public int PowerGeneratorLevel
	{
		get
		{
			for (int i = 0; i < upgrades.powerGenerators.Length; i++)
			{
				if (upgrades.powerGenerators[i].ID == power.ID)
				{
					return i;
				}
			}
			Debug.Log("PowerGenerator ID did not match an engine in upgrade list");
			return 0;
		}
	}
	public int CargoLevel
	{
		get
		{
			for (int i = 0; i < upgrades.cargoSystems.Length; i++)
			{
				if (upgrades.cargoSystems[i].ID == cargo.ID)
				{
					return i;
				}
			}
			Debug.Log("Cargo System ID did not match an engine in upgrade list");
			return 0;
		}
	}
	public int StorageLevel
	{
		get
		{
			for (int i = 0; i < upgrades.storages.Length; i++)
			{
				if (upgrades.storages[i].ID == storage.ID)
				{
					return i;
				}
			}
			Debug.Log("Engine ID did not match an engine in upgrade list");
			return 0;
		}
	}


	public float Difficulty() {
		return (EngineLevel * 5 + PowerGeneratorLevel * 3 + CargoLevel * 3 + cargoDelivered);
	}

	public ResourceForSale GenerateResourceSale() {
		float totalWeight = 0;
		float[] weights = new float[resourcePrices.Length];
		for (int i = 0; i < resourcePrices.Length; ) {
			weights[i] = resourcePrices[i].relativeWeight;
			totalWeight += resourcePrices[i].relativeWeight;
		}
		float randomizer = UnityEngine.Random.Range(0, totalWeight);
		ResourceAvailability res = new ResourceAvailability();
		for (int i = 0; i < weights.Length; i++) {
			if (randomizer < weights[i]) {
				res = resourcePrices[i];
				break;
			}
			randomizer -= weights[i];
		}
		ResourceForSale rfs = new ResourceForSale();
		rfs.amount = Mathf.RoundToInt(UnityEngine.Random.Range(res.amountForSale.x, res.amountForSale.y));
		rfs.cost = Mathf.RoundToInt(UnityEngine.Random.Range(res.costRange.x, res.costRange.y));
		rfs.type = res.type;
		return rfs;
	}
  
	public Cargo GenerateCargoContract() {
		int availablePools = 0;
		float difficulty = Difficulty();
		for (int i = 0; i < cargoPools.Length; i++) {
			if (cargoPools[i].minimumDifficulty < difficulty)
				availablePools++;
			else
				break;
		}
		// MODIFIER
		float weightDistibution = 1.21f;
		float chanceFor2if2 = 0.1f;
		float chanceFor2if3 = 0.25f;
		float chanceFor2if4 = 0.5f;
		// END MODIFIER
		int rand_num = (int)Mathf.Pow(UnityEngine.Random.Range(0, Mathf.Pow(availablePools, weightDistibution)), 1 / weightDistibution);
		CargoPool pool = cargoPools[rand_num];
		int size = 1;
		if (CargoLevel == 2 && UnityEngine.Random.value < chanceFor2if2)
			size = 2;
		else if (CargoLevel == 3 && UnityEngine.Random.value < chanceFor2if3)
			size = 2;
		else if (CargoLevel == 4 && UnityEngine.Random.value < chanceFor2if4)
			size = 2;

		Cargo newCargo = new Cargo(
			Mathf.RoundToInt(UnityEngine.Random.Range(pool.powerNeedRange.x, pool.powerNeedRange.y) * size),
			size,
			pool.isTimeSensitive,
			Mathf.RoundToInt(UnityEngine.Random.Range(pool.timeRange.x, pool.timeRange.y) * size),
			pool.names[(int)UnityEngine.Random.Range(0, pool.names.Length)],
			Mathf.RoundToInt(UnityEngine.Random.Range(pool.maxCreditRange.x, pool.maxCreditRange.y) * size),
			Mathf.RoundToInt(UnityEngine.Random.Range(pool.lateCreditFractionRange.x, pool.lateCreditFractionRange.y) * size),
			Mathf.RoundToInt(UnityEngine.Random.Range(pool.damagedCreditFractionRange.x, pool.damagedCreditFractionRange.y) * size),
			Mathf.RoundToInt(UnityEngine.Random.Range(pool.ruinedCreditFractionRange.x, pool.ruinedCreditFractionRange.y) * size),
			Mathf.RoundToInt(UnityEngine.Random.Range(pool.damagedAtFractionRange.x, pool.damagedAtFractionRange.y) * size),
			Mathf.RoundToInt(UnityEngine.Random.Range(pool.ruinedAtFractionRange.x, pool.ruinedAtFractionRange.y) * size),
			Mathf.RoundToInt(UnityEngine.Random.Range(pool.maxFractionalDegredationRateRange.x, pool.maxFractionalDegredationRateRange.y) * size)
		);
		return newCargo;

	}

	public Subsystem GenerateUpgrade() {
		Subsystem[] systems = new Subsystem[5]; // 5 is number of upgradable resources
		int numberOfUpgradesAvailable = 0;
		if (PowerGeneratorLevel < upgrades.powerGenerators.Length - 1) {
			systems[numberOfUpgradesAvailable] = upgrades.powerGenerators[PowerGeneratorLevel + 1];
			numberOfUpgradesAvailable++;
		}
		if (FuelTankLevel < upgrades.fuelTanks.Length - 1)
		{
			systems[numberOfUpgradesAvailable] = upgrades.fuelTanks[FuelTankLevel + 1];
			numberOfUpgradesAvailable++;
		}
		if (EngineLevel < upgrades.engines.Length - 1)
		{
			systems[numberOfUpgradesAvailable] = upgrades.engines[EngineLevel + 1];
			numberOfUpgradesAvailable++;
		}
		if (StorageLevel < upgrades.storages.Length - 1)
		{
			systems[numberOfUpgradesAvailable] = upgrades.storages[StorageLevel + 1];
			numberOfUpgradesAvailable++;
		}
		if (CargoLevel < upgrades.cargoSystems.Length - 1)
		{
			systems[numberOfUpgradesAvailable] = upgrades.cargoSystems[CargoLevel + 1];
			numberOfUpgradesAvailable++;
		}
		return systems[(int)UnityEngine.Random.Range(0, numberOfUpgradesAvailable)];
	}

	public VendorAndName GenerateVendor() {
		VendorAndName vn = new VendorAndName();
		VendorNamePool vnPool = vendors[(int)UnityEngine.Random.Range(0, vendors.Length)];
		vn.vendor = vnPool.sprite;
		vn.name = vnPool.names[(int)UnityEngine.Random.Range(0, vnPool.names.Length)];
		return vn;
	}

	// MODIFIERS AVAILABLE
	public FuelSale GenerateFuelSale() {
		FuelSale fs = new FuelSale();
		float unitCost = UnityEngine.Random.Range(fuelPriceRange.x, fuelPriceRange.y);
		fs.amount = Mathf.RoundToInt(UnityEngine.Random.Range(
			fuelBaseAmountRange.x + (FuelTankLevel * 0.2f) + (EngineLevel * 0.5f),
			fuelBaseAmountRange.y + (FuelTankLevel * 0.8f) + (EngineLevel * 1.2f)
		));
		fs.cost = Mathf.RoundToInt(fs.amount * unitCost);
		return fs;
	}



	// SUBSYSTEM CODE

    public void ReplaceSubsystem(Subsystem sys)
    {
		if (sys is Engine)
		{
			engine.Deactivate();
			engine = sys as Engine;
			engine.gameObject.SetActive(true);
		}
		else if (sys is CargoSystem)
		{
			cargo.Deactivate();
			cargo = sys as CargoSystem;
			cargo.gameObject.SetActive(true);
		}
		else if (sys is CargoSystem)
		{
			cargo = sys as CargoSystem;
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

[System.Serializable]
public struct UpgradeList
{
    public Engine[] engines;
    public ShipStorage[] storages;
    public FuelTank[] fuelTanks;
    public CargoSystem[] cargoSystems;
	public PowerGenerator[] powerGenerators;
}

[System.Serializable]
public struct VendorNamePool {
	public Sprite sprite;
	public string[] names;
}

[System.Serializable]
public struct ResourceAvailability {
	public ResourceType type;
	public Vector2 costRange;
	public Vector2 amountForSale;
	public float relativeWeight;
}

public struct ResourceForSale {
	public ResourceType type;
	public int cost;
	public int amount;
	public ResourceForSale(ResourceType resourceType, int creditCost, int numberOfResource) {
		type = resourceType;
		cost = creditCost;
		amount = numberOfResource;
	}
}

public struct VendorAndName {
	public Sprite vendor;
	public string name;
}

public struct FuelSale {
	public int amount;
	public int cost;

}
