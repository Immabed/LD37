using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public abstract class Subsystem : MonoBehaviour {

    public GameObject menu;
    public GameObject repairMenu;


    [SerializeField]
    Button[] powerBars;
    Image[] powerBarImages;

    [SerializeField]
	[Tooltip("Only applicable if a level 2 or better subsystem")]
	protected Vector2 costRange;
    protected int cost;

    [SerializeField]
    protected int maxPower;
	[SerializeField]
    protected int currentPower;
	[SerializeField]
    protected int currentPowerLimit;

    protected RepairRecipe currentRecipe;

    protected bool isDamaged;
    protected bool playerIsNearby;
    protected PlayerController pc;

    [SerializeField]
    protected GameManager gm;

    [SerializeField]
    float averageTimeUntilFailure;

	[SerializeField]
	protected string id;

	[SerializeField]
	[Tooltip("Used by vendors if offered as upgrade")]
	private string nameOfSystem;

	[SerializeField]
	[Tooltip("Used by vendors if subsystem is offered as upgrade")]
	private string upgradeDescription;

    [SerializeField]
    [Range(0, 1)]
    protected float timeBetweenUpdates;

	protected SubsystemType type;


    // Repair Recipes
    [SerializeField]
    protected RepairRecipe[] recipes;

    public bool IsDamaged { get { return isDamaged; } }
	public string ID { get { return id; } }
    public int CurrentPower { get { return currentPower; } set { currentPower = value; } }
    public int CurrentPowerLimit {  get { return currentPowerLimit; } }
    public int MaxPower {  get { return maxPower; } }
	public int Cost { get { return cost; } }
	public string Name { get { return nameOfSystem; }}
	public string Description { get { return upgradeDescription; }}
    public virtual float FailureChance { get { return 1 / averageTimeUntilFailure; } }
	public SubsystemType Type { get { return type; } }

    protected Coroutine co;

    protected abstract IEnumerator UpdateTimer();

	protected abstract void UpdateUI();

	public void Deactivate() {
		StopCoroutine(co);
		co = null;
		menu.SetActive(false);
		repairMenu.SetActive(false);
		gameObject.SetActive(false);
	}

    protected virtual void Start()
    {
        powerBarImages = new Image[powerBars.Length];
        for (int i = 0; i < powerBars.Length; i++)
        {
            powerBarImages[i] = powerBars[i].gameObject.GetComponent<Image>();
        }


        if (timeBetweenUpdates > 0)
            co = StartCoroutine(UpdateTimer());
        currentPower = maxPower;
        currentPowerLimit = maxPower;
        cost = Mathf.RoundToInt(Random.Range(costRange.x, costRange.y));
        if (powerBars.Length != maxPower)
        {
			Debug.LogWarning(string.Format("{0} does not have equal number of power bars({1}) and max power({2}). ", id, powerBars.Length, maxPower));
        }

        
    }

	public void ActivateCoroutine() {
		if (co == null && timeBetweenUpdates > 0) {
			co = StartCoroutine(UpdateTimer());
		}
	}


    public void GenerateCost()
    {
        cost = Mathf.RoundToInt(Random.Range(costRange.x, costRange.y));
    }

    protected void UpdatePowerBars()
    {
        for (int i = 0; i < maxPower; i++)
        {
            if (i < currentPower)
            {
				powerBarImages[i].sprite = gm.PowerIcons.InUse;
                powerBars[i].interactable = true;
            }
			else if (i < currentPowerLimit && i - currentPower + 1 <= gm.EnergyAvailable())
            {
				powerBarImages[i].sprite = gm.PowerIcons.Available;
                powerBars[i].interactable = true;
            }
			else if (i < currentPowerLimit && i - currentPower >= gm.EnergyAvailable())
			{
				powerBarImages[i].sprite = gm.PowerIcons.Unavailable;
				powerBars[i].interactable = false;
			}
            else if (i >= currentPowerLimit && i - currentPower + 1 <= gm.EnergyAvailable())
			{
				powerBarImages[i].sprite = gm.PowerIcons.AvailableDisabled;
				powerBars[i].interactable = false;
			}
			else if (i >= currentPowerLimit && i - currentPower >= gm.EnergyAvailable())
			{
				powerBarImages[i].sprite = gm.PowerIcons.UnavailableDisabled;
				powerBars[i].interactable = false;
			}
        }
    }

    public void ClickPower(int id)
    {
        if (id <= currentPowerLimit && id != currentPower)
        {
            currentPower = id;
            UpdatePowerBars();
        }
        else if (id == currentPower)
        {
            currentPower--;
            UpdatePowerBars();
        }
		UpdateUI();
    }

    protected virtual void UpdatePower()
    {
        if (currentPowerLimit < 0)
        {
            currentPowerLimit = 0;
        }
        else if (currentPowerLimit > maxPower)
        {
            currentPowerLimit = maxPower;
        }
        if (currentPower > currentPowerLimit)
        {
            currentPower = currentPowerLimit;
        }
        if (gm.EnergyAvailable() > 0)
        {
            currentPower += Mathf.Min(gm.EnergyAvailable(), currentPowerLimit - currentPower);
        } 
        //gm.UpdateSystems();
        //TODO - implement more power management
    }

    protected virtual void Repair(Resource res)
    {
        if (isDamaged) {
            if (res is SpareParts && currentRecipe.SparePartsNeeded > 0)
            {
                currentRecipe.UseSpareParts();
            }
            else if (res is PowerCell && currentRecipe.PowerCellsNeeded > 0)
            {
                currentRecipe.UsePowerCell();
            }
            else if (res is Computer && currentRecipe.ComputersNeeded > 0)
            {
                currentRecipe.UseComputer();
            }
        }
        if (currentRecipe.IsCompleted())
        {
            RepairSystem();
        }
        UpdatePower();
    }

    // Function for In Game Button to Repair System
    public void TryRepair()
    {
        if (playerIsNearby && pc != null)
        {
            Repair(pc.Item);
            pc.RemoveResource();
        }
    }

    protected virtual void RepairSystem()
    {
        isDamaged = false;
    }

    public abstract void DamageSystem();

    public bool PlayerCanRepair()
    {
        if (playerIsNearby && pc != null && pc.HasItem && isDamaged && currentRecipe.Needs(pc.Item))
            return true;
        else
            return false;
    }

    public RepairRecipe GetRecipe()
    {
        RepairRecipe recipe;
        if (isDamaged)
        {
            recipe = currentRecipe;
            return recipe;
        }
        else
        {
            return new RepairRecipe(0,0,0);
        }
    }
        
    protected virtual void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.tag == "Player")
        {
            playerIsNearby = true;
            pc = collision.gameObject.GetComponent<PlayerController>();
            if (pc.HasItem)
            {
                if (isDamaged)
                {
                    repairMenu.SetActive(true);
                }
            }
            else
            {
                menu.SetActive(true);
            }
        }
    }

    protected virtual void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.tag == "Player")
        {
            playerIsNearby = false;
            pc = null;
            menu.SetActive(false);
            repairMenu.SetActive(false);
        }
    }
}

[System.Serializable]
public struct RepairRecipe
{
    [SerializeField] int sparePartsNeeded;
    [SerializeField] int computersNeeded;
    [SerializeField] int powerCellsNeeded;

    public int SparePartsNeeded { get { return sparePartsNeeded; } }
    public int ComputersNeeded { get { return computersNeeded; } }
    public int PowerCellsNeeded { get { return powerCellsNeeded; } }

    public RepairRecipe(int spareParts, int computers, int powerCells)
    {
        sparePartsNeeded = spareParts;
        computersNeeded = computers;
        powerCellsNeeded = powerCells;
    }

    public void UseSpareParts()
    {
        if (sparePartsNeeded > 0)
            sparePartsNeeded--;
    }

    public void UseComputer()
    {
        if (computersNeeded > 0)
            computersNeeded--;
    }
    public void UsePowerCell()
    {
        if (powerCellsNeeded > 0)
            powerCellsNeeded--;
    }
    public bool Needs(Resource res)
    {
        if (res is SpareParts && sparePartsNeeded > 0)
        {
            return true;
        }
        else if (res is Computer && computersNeeded > 0)
        {
            return true;
        }
        else if (res is PowerCell && powerCellsNeeded > 0)
        {
            return true;
        }
        else
        {
            return false;
        }
    }
    public bool IsCompleted()
    {
        return sparePartsNeeded <= 0 && computersNeeded <= 0 && powerCellsNeeded <= 0;
    }
}

public enum SubsystemType {NONETYPE, ENGINE, FUEL, POWER, CARGO, LIFESUPPORT, STORAGE, COCKPIT, DATABANK}
