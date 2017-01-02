using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class GameManager : MonoBehaviour {

	#region fields
    //Subsystems
    [Header("Subsystems")]
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

    //Station menus
    [Header("Station Menu Items")]
    [SerializeField]
    Text stationNameTx;
    [SerializeField]
    Vendor[] vendorObjects;
    [SerializeField]
    GameObject vendorMenu;
    [SerializeField]
    Text stationCreditTx;
    [SerializeField]
    Text stationNextDestinationTx;
    [SerializeField]
    Text engineUpgradeLevelTx;
    [SerializeField]
    Text fuelUpgradeLevelTx;
    [SerializeField]
    Text storageLevelTx;
    [SerializeField]
    Text powerLevelTx;
    [SerializeField]
    Text cargoLevelTx;

    // Pools and info
    [Header("Random Generation Information")]
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
    [SerializeField]
    string[] stationNames;

    //Assorted Info
    [Header("Assorted other Info")]
    [SerializeField]
    int baseContractDistance;
    [SerializeField]
    [Tooltip("Distances are doubled by this difficulty level")]
    int contractDistanceDoubledAtDifficulty;
	[SerializeField]
	PowerLevelIcons powerIcons;
	[SerializeField]
	SaleIcons saleIcons;
	[SerializeField]
	CargoIcons cargoIcons;

	// Number of light years between current station and next station
    int distanceToNextStation;

	// Failure flags
    bool engineHasFailed;
    bool fuelHasFailed;
    bool powerHasFailed;
    bool cargoHasFailed;
    bool lifeSupportHasFailed;

	// Number of credits player has
    int credits;
	// Pieces of cargo a player has delivered, double cargo count for 2
    int cargoDelivered;

	// Current rate of time passage. 0: stopped; 1: normal.
    float timeScale = 1;

	#endregion

	// PROPERTIES //
	#region properties

	// Current rate of time passage. 0: stopped; 1: normal.
    public float TimeScale { get { return timeScale; } }
	// Current power being used by subsystems
    public int PowerUsed { get { return engine.CurrentPower + fuel.CurrentPower + cargo.CurrentPower + lifeSupport.CurrentPower; } }
	// Max amount of power drawable, given damage levels of subsystems.
	public int PowerDrawAvailable { get { return engine.CurrentPowerLimit + fuel.CurrentPowerLimit + LifeSupport.CurrentPowerLimit + cargo.CurrentPowerDrawLimit; } }
	// Current speed in light years per second
    public float CurrentSpeed { get { return engine.CurrentSpeed; } }
	// Number of credits player has
    public int Credits { get { return credits; } }
	// Pieces of cargo a player has delivered, double cargo count for 2
    public int CargoDelivered { get { return cargoDelivered; } }
	// Available room in storage locker
    public int StorageAvailable { get { return storage.RoomLeft; } }
	// Available (no cargo) cargo slots in current cargo system
    public int CargoRoomAvailable { get { return cargo.CargoRoomAvailable; } }
	// Available units of fuel that can be added to the fuel tank
    public float FuelTankRoom { get { return fuel.RoomInTank; } }
	// Get current fuel draw rate to engines in units per second
	public float FuelUse { get { return engine.CurrentFuelDraw; } }
	// Amount of surplus power
	public int PowerAvailable { get { return power.CurrentPowerGeneration - PowerUsed; } }

	// Icons
	public PowerLevelIcons PowerIcons { get { return powerIcons; } }
	public SaleIcons SaleIcons { get { return saleIcons; } }
	public CargoIcons CargoIcons { get { return cargoIcons; } }

	// Subsystems accessors (for power generator)
	public Engine Engine { get { return engine; } }
	public FuelTank FuelTank { get { return fuel; } }
	public LifeSupport LifeSupport { get { return lifeSupport; } }
	public CargoSystem Cargo { get { return cargo; } }

	// Current 'difficulty' for use in selecting cargo missions etc. Higher is more, units are meaningless
	// The difficulty function is itself arbitrary and can be adjusted to alter how the game progresses
	public float Difficulty {
		get
		{
			// Engine is favored highest, but cargoDelivered can also dominate
			return (EngineLevel * 5 + PowerGeneratorLevel * 3 + CargoLevel * 3 + cargoDelivered);
		}
	}

	#endregion

	// Upgrade levels based on upgrade pools
	#region upgrade level properties
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
	#endregion


	// Functions like Start, OnEnable, Awake etc.
	#region MonoBehaviour functions

    private void Awake() {
		// Sort cargo pools by minimum difficulty
		Array.Sort<CargoPool>(cargoPools, (x, y) => x.minimumDifficulty.CompareTo(y.minimumDifficulty));
        
    }

    private void OnEnable()
	{
		// Set up power and power displays
        CheckPowerDeficit();
		power.UpdateSystems();
		// Begin damage timer
		StartCoroutine(DamageTimer(0.1f));
    }

	#endregion


    #region vendorCode

	/// <summary>
	/// Starts the leave station sequence.
	/// </summary>
    public void LeaveStation()
    {
		// Enable time
        timeScale = 1;
		// Hide station menu
        vendorMenu.SetActive(false);
		// Set new destination distance
        cockpit.SetNewDestination(distanceToNextStation);
    }

	/// <summary>
	/// Updates the station user interface.
	/// </summary>
    public void UpdateStationUI()
    {
		// General info
        stationCreditTx.text = credits.ToString();
        stationNextDestinationTx.text = String.Format("{0} light years", distanceToNextStation);
        
		// Subsystem info
		engineUpgradeLevelTx.text = String.Format("{0}", engine.MaxPower);
        fuelUpgradeLevelTx.text = String.Format("{0:##0}/{1}", fuel.FuelLevel, fuel.FuelCapacity);
        cargoLevelTx.text = String.Format("{0}/{1}", cargo.CargoCapacity - cargo.CargoRoomAvailable, cargo.CargoCapacity);
		powerLevelTx.text = String.Format("{0}/{1}", PowerUsed, power.MaxPowerGeneration);
        storageLevelTx.text = String.Format("{0}/{1}", storage.MaxCapacity - storage.RoomLeft, storage.MaxCapacity);

		foreach (Vendor ven in vendorObjects) {
			ven.UpdateSelectable();
		}
    }

	/// <summary>
	/// Triggers arriving at a station.
	/// </summary>
    public void ArrivedAtDestination()
    {
        if (cockpit.DistanceToDestination <= 0)
			// Ensure that actually at destination
        {
			// Pause time
            timeScale = 0;

			// Collect Cargo TODO
			var cargoDelivery = cargo.CollectCargo(); // Implement something interesting
			cargoDelivered += (int)cargoDelivery.y;
			credits += (int)cargoDelivery.x;

			// Populate vendors
            foreach (Vendor vendor in vendorObjects)
            {
                vendor.PickSelection();
            }
			// Populate station name
            stationNameTx.text = stationNames[(int)UnityEngine.Random.Range(0, stationNames.Length)];                               

			// Determine next station (distance)
            distanceToNextStation = (int)(baseContractDistance * (Difficulty / contractDistanceDoubledAtDifficulty + 1));

            // Reset failures
            engineHasFailed = false;
            fuelHasFailed = false;
            powerHasFailed = false;
            cargoHasFailed = false;
            lifeSupportHasFailed = false;

            // Update and display UI
            UpdateStationUI();
            vendorMenu.SetActive(true);
        }
    }


	// GENERATION CODE //

	// Generate a sale of resources for a vendor
	/// <summary>
	/// Generates a resource sale for a vendor.
	/// </summary>
	/// <returns>The resource sale.</returns>
    public ResourceForSale GenerateResourceSale() {
		// Generate weighted distribution
        float totalWeight = 0;
        float[] weights = new float[resourcePrices.Length];
        for (int i = 0; i < resourcePrices.Length; i++) {
			weights[i] = resourcePrices[i].relativeWeight;
			totalWeight = weights[i];
        }

		// Generate random value in weight range
        float randomizer = UnityEngine.Random.Range(0, totalWeight);
        // Resource variable
		var res = new ResourceAvailability();
        
		// Determine which resourcePrices item was selected
		for (int i = 0; i < weights.Length; i++) {
            if (randomizer <= weights[i]) {
                res = resourcePrices[i];
                break;
            }
            randomizer -= weights[i];
        }

		// Instantiate and populate ResourceForSale
        ResourceForSale rfs = new ResourceForSale();
        rfs.amount = Mathf.RoundToInt(UnityEngine.Random.Range(res.amountForSale.x, res.amountForSale.y));
        rfs.cost = Mathf.RoundToInt(UnityEngine.Random.Range(res.costRange.x, res.costRange.y));
        rfs.type = res.type;
        return rfs;
    }

	// Generate a cargo contract for a vendor, modifiers available
	/// <summary>
	/// Generates a cargo contract for a vendor, modifiers available.
	/// </summary>
	/// <returns>The cargo contract.</returns>
    public Cargo GenerateCargoContract() {
        // MODIFIER
        float weightDistibution = 1.21f; //shape of distribution follows this root
		float chanceFor2if2 = 0.1f;  // Chance that a contract will be a double cargo if cargoSystem has 2 slots
		float chanceFor2if3 = 0.25f; // Chance that a contract will be a double cargo if cargoSystem has 3 slots
		float chanceFor2if4 = 0.5f;  // Chance that a contract will be a double cargo if cargoSystem has 4 slots
		// END MODIFIER

		// Determine which contract pools are available for current difficulty
		// availablePools is how many cargo pools can be pulled from
		int availablePools = 0;
        float difficulty = Difficulty;
        for (int i = 0; i < cargoPools.Length; i++) {
            if (cargoPools[i].minimumDifficulty <= difficulty)
                availablePools++;
            else
                break;
        }
        
		// Select a pool index based on distribution
		int rand_num = (int)Mathf.Pow(UnityEngine.Random.value * Mathf.Pow(availablePools, weightDistibution), 1 / weightDistibution);
		// Ensure that selection is not greater (does not mess with distribution, but Random.value is inclusive of 0 and 1)
		// If Random.value returns 1, rand_num = availablePools, and it must be less
		rand_num = rand_num < availablePools ? rand_num : availablePools - 1;

        // Determine if contract is for a double cargo
		int size = 1;
        if (CargoLevel == 2 && UnityEngine.Random.value < chanceFor2if2)
            size = 2;
        else if (CargoLevel == 3 && UnityEngine.Random.value < chanceFor2if3)
            size = 2;
        else if (CargoLevel == 4 && UnityEngine.Random.value < chanceFor2if4)
            size = 2;

		// Select pool
		CargoPool pool = cargoPools[rand_num];

		// SPRITE
		Sprite sp = null;
		// Determine which sprite to use
		switch (pool.type) {
			case CargoType.STANDARD:
				sp = cargoIcons.Standard;
				break;
			case CargoType.HAZARDOUS:
				sp = cargoIcons.Hazardous;
				break;
			case CargoType.PERISHABLE:
				sp = cargoIcons.Perishable;
				break;
		}

		// Generate cargo TODO replace with vars, and use vars to instantiate
		Cargo newCargo = new Cargo(
			pool.type,
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
			Mathf.RoundToInt(UnityEngine.Random.Range(pool.maxFractionalDegredationRateRange.x, pool.maxFractionalDegredationRateRange.y) * size),
			sp
        );
        return newCargo;
    }

	// Generate an upgrade sale for a vendor TODO check code and enable
	/// <summary>
	/// Generates an upgrade.
	/// </summary>
	/// <returns>An upgrade.</returns>
    public Subsystem GenerateUpgrade() {
        /* Subsystem[] systems = new Subsystem[5]; // 5 is number of upgradable resources
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
        var sys = systems[(int)UnityEngine.Random.Range(0, numberOfUpgradesAvailable)];
        sys.GenerateCost(); */
        return upgrades.engines[0];
    }

	/// <summary>
	/// Generates the vendor (name and image).
	/// </summary>
	/// <returns>The vendor.</returns>
    public VendorAndName GenerateVendor() {
        VendorAndName vn = new VendorAndName();
		// Select vendor image (with name pool)
        VendorNamePool vnPool = vendors[(int)UnityEngine.Random.Range(0, vendors.Length)];
        vn.vendor = vnPool.sprite;
		// Select vendor name
        vn.name = vnPool.names[(int)UnityEngine.Random.Range(0, vnPool.names.Length)];
        return vn;
    }

    // MODIFIERS AVAILABLE
	/// <summary>
	/// Generates a fuel sale for a vendor.
	/// </summary>
	/// <returns>The fuel sale.</returns>
    public FuelSale GenerateFuelSale() {
		// MODIFIERS
		// Low and High refer to modifying the lower and upper ranges of fuel amount
		// The amount is modified by adding the current subsystem level multiplied by the relevant modifier, for each
		// subsystem. Eg, add 0.2 of fuel system level and 0.5 of engine system level to lower bound.
		var fuelModLow = 0.2f;
		var fuelModHigh = 0.8f;
		var engineModLow = 0.5f;
		var engineModHigh = 1.5f;
		// END MODIFIERS


		FuelSale fs = new FuelSale();
		// Select cost per unit of fuel
        float unitCost = UnityEngine.Random.Range(fuelPriceRange.x, fuelPriceRange.y);
		// Select fuel amount based on engine and fuel tank level modifiers
        fs.amount = Mathf.RoundToInt(UnityEngine.Random.Range(
			fuelBaseAmountRange.x + (FuelTankLevel * fuelModLow) + (EngineLevel * engineModLow),
			fuelBaseAmountRange.y + (FuelTankLevel * fuelModHigh) + (EngineLevel * engineModHigh)
        ));
		// Determine total cost
        fs.cost = Mathf.RoundToInt(fs.amount * unitCost);
        return fs;
    }

	// END GENERATION CODE //



	/// <summary>
	/// Buys the fuel.
	/// </summary>
	/// <returns><c>true</c>, if fuel sale was possible and fuel was bought, <c>false</c> otherwise.</returns>
	/// <param name="fs">The fuel sale object, for getting amount and cost.</param>
    public bool BuyFuel(FuelSale fs)
    {
        if (fs.cost <= credits && fs.amount <= fuel.RoomInTank)
        {
            credits -= fs.cost;
            fuel.AddFuel(fs.amount);
            UpdateStationUI();
            return true;
        }
        else
            return false;
    }

	/// <summary>
	/// Buys the upgrade. Does not check that subsystem is actually and upgrade.
	/// </summary>
	/// <returns><c>true</c>, if upgrade purchase was possible and upgrade was bought, <c>false</c> otherwise.</returns>
	/// <param name="sys">The subsystem to upgrade to.</param>
    public bool BuyUpgrade(Subsystem sys)
    {
        if (sys != null && sys.Cost <= credits)
        {
            credits -= sys.Cost;
            ReplaceSubsystem(sys);
            UpdateStationUI();
            return true;
        }
        else
            return false;
    }

	/// <summary>
	/// Buys the resource.
	/// </summary>
	/// <returns><c>true</c>, if resource purchase possible and resource was bought, <c>false</c> otherwise.</returns>
	/// <param name="res">Resource sale, for type, amount, and cost.</param>
    public bool BuyResource(ResourceForSale res)
    {
        if (res.cost <= credits && res.amount <= storage.RoomLeft)
        {
            credits -= res.cost;
            storage.AddResource(res.type, res.amount);
            UpdateStationUI();
            return true;
        }
        else
        {
            return false;
        }
    }

	/// <summary>
	/// Accepts the cargo contract.
	/// </summary>
	/// <returns><c>true</c>, if room for cargo, and cargo was accepted, <c>false</c> otherwise.</returns>
	/// <param name="car">Car.</param>
    public bool AcceptCargo(Cargo car)
    {
        if (cargo.CargoRoomAvailable >= car.Size)
        {
            cargo.AddCargo(car);
            UpdateStationUI();
            return true;
        }
        else return false;
    }
    #endregion


    #region subsystemCode
    // SUBSYSTEM CODE

	/// <summary>
	/// Triggers DamageSystem at regular intervals, is how subsystems get damaged.
	/// </summary>
	/// <returns>The timer.</returns>
	/// <param name="timeStep">Time between checks.</param>
    private IEnumerator DamageTimer(float timeStep)
    {
        for (;;)
        {
            DamageSystem(timeStep);
            yield return new WaitForSeconds(timeStep);
        }
    }

	/// <summary>
	/// Damages subsystems based on failure chances. Subsystem can only be damaged once between stations.
	/// </summary>
	/// <param name="timeStep">Time between checks.</param>
    private void DamageSystem(float timeStep)
    {
        if (!engineHasFailed && UnityEngine.Random.value < engine.FailureChance * timeScale * timeStep)
			// Engine
        {
            engineHasFailed = true;
            engine.DamageSystem();
        }
        if (!fuelHasFailed && UnityEngine.Random.value < fuel.FailureChance * timeScale * timeStep)
			// Fuel Tank
        {
            fuelHasFailed = true;
            fuel.DamageSystem();
        }
        if (!cargoHasFailed && UnityEngine.Random.value < cargo.FailureChance * timeScale * timeStep)
			// Cargo System
        {
            cargoHasFailed = true;
            cargo.DamageSystem();
        }
        if (!powerHasFailed && UnityEngine.Random.value < power.FailureChance * timeScale * timeStep)
			// Power Generator
        {
            powerHasFailed = true;
            power.DamageSystem();
        }
        if (!lifeSupportHasFailed && UnityEngine.Random.value < lifeSupport.FailureChance * timeScale * timeStep)
			// Life Support
        {
            lifeSupportHasFailed = true;
            lifeSupport.DamageSystem();
        }
		// Since power levels may have changed, ensure there is no power deficit
        CheckPowerDeficit();
    }

	/// <summary>
	/// Replaces the subsystem.
	/// </summary>
	/// <param name="sys">The subsystem to replace current system with.</param>
    public void ReplaceSubsystem(Subsystem sys)
    {
        if (sys is Engine)
			// Engine
        {
            engine.Deactivate();
            engine = sys as Engine;
            engine.gameObject.SetActive(true);
			engine.ActivateCoroutine();
        }
        else if (sys is CargoSystem)
			// Cargo System
        {
			// Transfer cargo
            (sys as CargoSystem).GetCargoFromCargoSystem(cargo);
            
			cargo.Deactivate();
            cargo = sys as CargoSystem;
            cargo.gameObject.SetActive(true);
			cargo.ActivateCoroutine();
        }
        else if (sys is ShipStorage)
			// Ship Storage Lockers
        {
			// Transfer stored resources
            var sysStore = sys as ShipStorage;
            sysStore.AddResource(ResourceType.SPAREPARTS, storage.StoredSpareParts);
            sysStore.AddResource(ResourceType.COMPUTER, storage.StoredComputers);
            sysStore.AddResource(ResourceType.POWERCELL, storage.StoredPowerCells);

			storage.Deactivate();
            storage = sysStore;
			storage.gameObject.SetActive(true);
			storage.ActivateCoroutine();
        }
        else if (sys is FuelTank)
			// Fuel Tank
        {
            // Transfer fuel
			(sys as FuelTank).AddFuel(fuel.FuelLevel);
            
			fuel.Deactivate();
            fuel = sys as FuelTank;
            fuel.gameObject.SetActive(true);
			fuel.ActivateCoroutine();
        }
        else if (sys is PowerGenerator)
			// Power Generator
        {
            power.Deactivate();
            power = sys as PowerGenerator;
            power.gameObject.SetActive(true);
			// Update power systems
			power.UpdateSystems();
			power.ActivateCoroutine();
        }

		// Since subsystems have changed, update the power generator to recognize new systems.
		power.UpdateSystems();
    }

    /// <summary>
    /// Checks the power deficit and adjusts power use if necessary.
    /// </summary>
    public void CheckPowerDeficit()
    {
        if (PowerUsed > power.CurrentPowerGeneration)
			// There is a deficit
        {
			// Determine deficit
            int deficit = PowerUsed - power.CurrentPowerGeneration;
			// Determine power used by engine and fuel
			int engineFuelPower = engine.CurrentPower + fuel.CurrentPower;

            if (deficit > engineFuelPower)
				// Deficit greater that just fuel and engine use
            {
				// Remove all fuel and engine power
                engine.CurrentPower = 0;
                fuel.CurrentPower = 0;
				// Determin new deficit
                deficit = PowerUsed - power.CurrentPowerGeneration;
                if (deficit > cargo.CurrentPower)
					// Deficit still greater than cargo power
                {
					// Remove all cargo power
                    cargo.CurrentPower = 0;
					// Remaining deficit taken from life support
                    deficit = PowerUsed - power.CurrentPowerGeneration;
                    lifeSupport.CurrentPower -= deficit;
                }
                else
					// Deficit less than cargo power, take from cargo
                {
                    cargo.CurrentPower -= deficit;
                }
            }
            else
				// Deficit less that fuel and engine power (combined), power taken in equal proportion
            {
                engine.CurrentPower -= Mathf.CeilToInt(((float)engine.CurrentPower / engineFuelPower) * deficit);
                fuel.CurrentPower -= Mathf.FloorToInt(((float)fuel.CurrentPower / engineFuelPower) * deficit);
            }

            if (PowerUsed - power.CurrentPowerGeneration != 0)
				// Check that there is no power deficit or surplus anymore
            {
				Debug.LogWarning("Power deficit not dealt with properly.");
            }
        }
    }

	/// <summary>
	/// Uses surplus as possible.
	/// </summary>
	public void IncreasePowerUse() {
		if (PowerUsed < power.CurrentPowerGeneration) {
			lifeSupport.CurrentPower += PowerAvailable < lifeSupport.CurrentPowerLimit - lifeSupport.CurrentPower ?
				PowerAvailable : lifeSupport.CurrentPowerLimit - lifeSupport.CurrentPower;
			
			cargo.CurrentPower += PowerAvailable < cargo.CurrentPowerLimit - cargo.CurrentPower ?
				PowerAvailable : cargo.CurrentPowerLimit - cargo.CurrentPower;
			
			fuel.CurrentPower += PowerAvailable < fuel.CurrentPowerLimit - fuel.CurrentPower ?
				PowerAvailable : fuel.CurrentPowerLimit - fuel.CurrentPower;
			
			engine.CurrentPower += PowerAvailable < engine.CurrentPowerLimit - engine.CurrentPower ?
				PowerAvailable : engine.CurrentPowerLimit - engine.CurrentPower;
		}
	}

	/// <summary>
	/// Ends the game.
	/// </summary>
	/// <param name="endGameMessage">End game message.</param>
    public void EndGame(string endGameMessage)
    {
        Debug.Log("YOU LOSE" + endGameMessage);
        // TODO - Implement end game
    }

    // Determine if the ship has enough resources for a repair recipe
	/// <summary>
	/// Checks that ship has resources to fulfill a repair recipe.
	/// </summary>
	/// <returns><c>true</c>, if ship has enough resources, <c>false</c> otherwise.</returns>
	/// <param name="rec">Repair recipe to check.</param>
    public bool HasResources(RepairRecipe rec)
    {
		// TODO - also check if player has a resource
        return (storage.StoredSpareParts >= rec.SparePartsNeeded
            && storage.StoredPowerCells >= rec.PowerCellsNeeded
            && storage.StoredComputers >= rec.ComputersNeeded);
    }

    #endregion
}

// POOLS AND PROBABILITIES //

/// <summary>
/// Lists of upgrade levels for upgradeable subystems.
/// </summary>
[System.Serializable]
public struct UpgradeList
{
    public Engine[] engines;
    public ShipStorage[] storages;
    public FuelTank[] fuelTanks;
    public CargoSystem[] cargoSystems;
	public PowerGenerator[] powerGenerators;
}

/// <summary>
/// A pool of vendor names and a vendor sprite
/// </summary>
[System.Serializable]
public struct VendorNamePool {
	public Sprite sprite;
	public string[] names;
}

/// <summary>
/// Description of resource sale and likelihood of appearance
/// </summary>
[System.Serializable]
public struct ResourceAvailability {
	public ResourceType type;
	public Vector2 costRange;
	public Vector2 amountForSale;
	public float relativeWeight;
}

// SALES AND VENDORS //

/// <summary>
/// An actual sale of a Resource, with amount, cost, type.
/// </summary>
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

/// <summary>
/// Vendor image and name.
/// </summary>
public struct VendorAndName {
	public Sprite vendor;
	public string name;
}

/// <summary>
/// An actual fuel sale, with amount and cost
/// </summary>
public struct FuelSale {
	public int amount;
	public int cost;

}


// ICON STRUCTS //

/// <summary>
/// Sale icons.
/// </summary>
[System.Serializable]
public struct SaleIcons {
	[SerializeField]
	Sprite fuel;
	[SerializeField]
	Sprite cargo;
	[SerializeField]
	Sprite spareParts;
	[SerializeField]
	Sprite computer;
	[SerializeField]
	Sprite powerCell;
	[SerializeField]
	Sprite upgradeEngine;
	[SerializeField]
	Sprite upgradeCargo;
	[SerializeField]
	Sprite upgradeFuel;
	[SerializeField]
	Sprite upgradeStorage;
	[SerializeField]
	Sprite upgradePower;

	public Sprite Fuel { get { return fuel; } }
	public Sprite UpgradeEngine { get { return upgradeEngine; } }
	public Sprite UpgradeCargo { get { return upgradeCargo; } }
	public Sprite UpgradeFuel { get { return upgradeFuel; } }
	public Sprite UpgradeStorage { get { return upgradeStorage; } }
	public Sprite UpgradePower { get { return upgradePower; } }
	public Sprite Cargo { get { return cargo; } }
	public Sprite SpareParts { get { return spareParts; } }
	public Sprite Computer { get { return computer; } }
	public Sprite PowerCell { get { return powerCell; } }
}

/// <summary>
/// Power level icons.
/// </summary>
[System.Serializable]
public struct PowerLevelIcons
{
	[SerializeField]
	Sprite inUse;
	[SerializeField]
	Sprite available;
	[SerializeField]
	Sprite availableDisabled;
	[SerializeField]
	Sprite unavailable;
	[SerializeField]
	Sprite unavailableDisabled;

	public Sprite InUse { get { return inUse; } }
	public Sprite Available { get { return available; } }
	public Sprite AvailableDisabled { get { return availableDisabled; } }
	public Sprite Unavailable { get { return unavailable; } }
	public Sprite UnavailableDisabled { get { return unavailableDisabled; } }
}

/// <summary>
/// Cargo icons.
/// </summary>
[System.Serializable]
public struct CargoIcons
{
	[SerializeField]
	Sprite standard;
	[SerializeField]
	Sprite perishable;
	[SerializeField]
	Sprite hazardous;

	public Sprite Standard { get { return standard; } }
	public Sprite Perishable { get { return perishable; } }
	public Sprite Hazardous { get { return hazardous; } }
}

