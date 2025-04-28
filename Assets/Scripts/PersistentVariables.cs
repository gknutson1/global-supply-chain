using System.Collections.Generic;
using UnityEngine;

public class PersistentVariables : MonoBehaviour
{
    public int level = 0; // Start on zero
    public int supply = 0; // XP
    public List<Ship> ships = new();

    public List<Ship> level0NewShips = new List<Ship>();
    public List<Ship> level1NewShips = new List<Ship>();
    public List<Ship> level2NewShips = new List<Ship>();
    public List<Ship> level3NewShips = new List<Ship>();
    public List<Ship> level4NewShips = new List<Ship>();
    public List<Ship> level5NewShips = new List<Ship>();

    void Start()
    {
        DontDestroyOnLoad(this);
    }
}
