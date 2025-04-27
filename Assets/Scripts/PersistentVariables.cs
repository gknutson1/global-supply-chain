using System.Collections.Generic;
using UnityEngine;

public class PersistentVariables : MonoBehaviour
{
    public int level = 0;  // start on zero
    public int supply = 0; // xp
    public List<Ship> ships = new List<Ship>();

    void Start()
    {
        DontDestroyOnLoad(this);
    }
}
