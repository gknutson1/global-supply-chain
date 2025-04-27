using System.Collections.Generic;
using UnityEngine;

public class PersistentVariables : MonoBehaviour
{
    public int level = 0;
    public int supply = 0;
    public List<Ship> ships = new();

    void Start()
    {
        DontDestroyOnLoad(this);
    }
}
