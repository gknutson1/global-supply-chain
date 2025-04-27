using System.Collections.Generic;
using UnityEngine;

public class PersistentVariables : MonoBehaviour
{
    public int level = 0; // Start on zero
    public int supply = 0; // XP
    public List<Ship> ships = new();

    void Start()
    {
        DontDestroyOnLoad(this);
    }
}
