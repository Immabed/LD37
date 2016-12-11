using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour {

    [SerializeField]
    private Engine engine;

    int credits;

    public int Credits { get { return credits; } }

    public void ReplaceSubsystem(Subsystem sys)
    {
        if (sys is Engine)
        {
            engine = sys as Engine;
        }
    }
}
