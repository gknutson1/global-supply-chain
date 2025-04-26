using System.Collections.Generic;
using UnityEngine;

public class PersistentVariables : MonoBehaviour
{
    public int level = 0;
    public int supply = 0;
    public List<Ship> ships = new List<Ship>();

    void Start()
    {
        DontDestroyOnLoad(this);
    }
}
