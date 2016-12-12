using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class Cargo {

    [SerializeField]
    Sprite sprite;


	[SerializeField]
	int powerNeed;
    int currentPower;
	[SerializeField]
	int size;
	[SerializeField]
	bool isTimeSensitive;
	[SerializeField]
	float timeLimit;
	[SerializeField]
	string nameOfCargo;
    [SerializeField]
    int maxCredits;
    [SerializeField]
    [Range(0,1)]
    float lateCreditsPercent;
    // Fraction of credits recieved for damaged
    [SerializeField]
    [Range(0,1)]
    float damagedCreditsPercent;
    // Fraction of credits recieved for ruined product
    [SerializeField]
    float ruinedCreditsPercent;
    [SerializeField]
    float damagedAtPercent;
    [SerializeField]
    float ruinedAtPercent;
    // Fraction per second
    [SerializeField]
    float maxDegredationRate;
    float currentStatus;

    float timeOfLastUpdate;

    

	public Cargo (int powerNeed, int size, bool isTimeSensitive, float timeLimit, string nameOfCargo, 
	              int maxCredits, float lateCreditsPercent, float damagedCreditsPercent, float ruinedCreditsPercent, 
	              float damagedAtPercent, float ruinedAtPercent, float maxDegredationRate) {
		this.powerNeed = powerNeed;
		this.size = size;
		this.isTimeSensitive = isTimeSensitive;
		this.timeLimit = timeLimit;
		this.nameOfCargo = nameOfCargo;
		this.maxCredits = maxCredits;
		this.lateCreditsPercent = lateCreditsPercent;
		this.damagedCreditsPercent = damagedCreditsPercent;
		this.ruinedCreditsPercent = ruinedCreditsPercent;
		this.damagedAtPercent = damagedAtPercent;
		this.ruinedAtPercent = ruinedAtPercent;
		this.maxDegredationRate = maxDegredationRate;
	}



	IEnumerator CheckPower(float timeBetweenUpdates, GameManager gm) 
    {
        timeOfLastUpdate = Time.time;
        for (;;)
        {
            currentStatus -= (powerNeed - currentPower) * (maxDegredationRate / powerNeed) * (Time.time - timeOfLastUpdate) * gm.TimeScale;
            if (currentStatus < 0) {
                maxDegredationRate = 0;
                currentStatus = 0;
            }
            if (isTimeSensitive && timeLimit > 0)
            {
                timeLimit -= Mathf.Max(Time.time - timeOfLastUpdate, 0);
            }
			yield return new WaitForSeconds(timeBetweenUpdates);
        }
    }

	public int PowerNeed { get { return powerNeed; } }
	public int Size { get { return size; } }
    public bool IsTimeSensitive {  get { return isTimeSensitive; } }
    public float TimeRemaining { get { return timeLimit; } }
    public int CurrentPower { get { return currentPower; } set { currentPower = value; } }
    public string Name {  get { return nameOfCargo; } }
    public int MaxCreditValue {  get { return maxCredits; } }
    public int CurrentCreditValue {
        get
        {
            float creditValue = maxCredits;
            if (currentStatus > ruinedAtPercent && currentStatus < damagedAtPercent)
                creditValue *= damagedCreditsPercent;
            else if (currentStatus < ruinedAtPercent)
                creditValue *= ruinedCreditsPercent;

            if (isTimeSensitive && timeLimit <= 0)
            {
                creditValue *= lateCreditsPercent;
            }

            return Mathf.RoundToInt(creditValue);
        }
    }
    public Sprite Sprite { get { return sprite; } }
}


[System.Serializable]
public struct CargoPool 
{
	public CargoType type;
	public string[] names;
	public bool isTimeSensitive;
	public Vector2 timeRange;
	public float minimumDifficulty;
	public Vector2 powerNeedRange;
	public Vector2 maxCreditRange;
	public Vector2 lateCreditFractionRange;
	public Vector2 damagedCreditFractionRange;
	public Vector2 ruinedCreditFractionRange;
	public Vector2 damagedAtFractionRange;
	public Vector2 ruinedAtFractionRange;
	public Vector2 maxFractionalDegredationRateRange;
}

public enum CargoType { HAZARDOUS, PERISHABLE, STANDARD}
