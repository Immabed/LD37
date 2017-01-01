using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PowerBarSelector : MonoBehaviour {

	[SerializeField]
	Text systemNameTx;
	[SerializeField]
	Button[] powerBars;
	[SerializeField]
	Text powerAmountTx;
	Image[] powerBarImages;

	public Subsystem sys { get; set;}

	[SerializeField]
	PowerGenerator power;

	[SerializeField]
	GameManager gm;

	float xPos;

	float xSep = 100f;


	public void UpdateNumberOfPowerBars() 
	{
		if (powerBarImages == null || powerBarImages.Length != powerBars.Length) {
			powerBarImages = new Image[powerBars.Length];
			for (int i = 0; i < powerBars.Length; i++)
			{
				powerBarImages[i] = powerBars[i].gameObject.GetComponent<Image>();
				if (powerBarImages[i] == null)
					Debug.Log("powerBarImages null " + gameObject.name + i);
			}
		}

		if (sys != null) 
		{
			systemNameTx.text = sys.Name;
			for (int i = 0; i < powerBars.Length; i++) {
				if (i < sys.MaxPower) {
					powerBars[i].enabled = true;
					powerBarImages[i].enabled = true;
				}
				else {
					powerBars[i].enabled = false;
					powerBarImages[i].enabled = false;
				}
			}
			powerAmountTx.gameObject.transform.localPosition = new Vector3(powerBars[sys.MaxPower-1].gameObject.transform.localPosition.x + xSep, 
			                                                          powerAmountTx.gameObject.transform.localPosition.y, 
			                                                          powerAmountTx.gameObject.transform.localPosition.z);
			UpdateUI();
		}
		else {
			// DO SOMETHING
		}
	}

	public void UpdateUI() {
		if (sys != null)
		{
			for (int i = 0; i < sys.MaxPower; i++)
			{
				if (i < sys.CurrentPower)
				{
					powerBarImages[i].sprite = gm.PowerIcons.InUse;
					powerBars[i].interactable = true;
				}
				else if (i < sys.CurrentPowerLimit && i - sys.CurrentPower + 1 <= gm.PowerAvailable)
				{
					powerBarImages[i].sprite = gm.PowerIcons.Available;
					powerBars[i].interactable = true;
				}
				else if (i < sys.CurrentPowerLimit && i - sys.CurrentPower >= gm.PowerAvailable)
				{
					powerBarImages[i].sprite = gm.PowerIcons.Unavailable;
					powerBars[i].interactable = false;
				}
				else if (i >= sys.CurrentPowerLimit && i - sys.CurrentPower + 1 <= gm.PowerAvailable)
				{
					powerBarImages[i].sprite = gm.PowerIcons.AvailableDisabled;
					powerBars[i].interactable = false;
				}
				else if (i >= sys.CurrentPowerLimit && i - sys.CurrentPower >= gm.PowerAvailable)
				{
					powerBarImages[i].sprite = gm.PowerIcons.UnavailableDisabled;
					powerBars[i].interactable = false;
				}
			}
			powerAmountTx.text = string.Format("{0}/{1}", sys.CurrentPower, sys.MaxPower);
		}
	}


	// Update System
	public void ClickPower(int id) {
		sys.ClickPower(id);
		UpdateUI();
	}




}
