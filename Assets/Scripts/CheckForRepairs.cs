using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class CheckForRepairs : MonoBehaviour {

    [SerializeField] Subsystem sys;
    private Button bt;

    private void Awake()
    {
        bt = gameObject.GetComponent<Button>();
    }

    private void OnEnable()
    {
        CheckIfRepairable();
        DisplayResourcesNeeded();
    }

    public void CheckIfRepairable()
    {
        if (sys.PlayerCanRepair())
        {
            bt.interactable = true;
        }
        else
        {
            bt.interactable = false;
        }
    }

    public void DisplayResourcesNeeded()
    {
        if (sys.IsDamaged)
        {
            RepairRecipe rc = sys.GetRecipe();
            // TODO - Display recipe
        }
        else
        {
            // TODO - Display no recipe
        }
    }
}
