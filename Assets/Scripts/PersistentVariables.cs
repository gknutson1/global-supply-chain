using System.Collections.Generic;
using UnityEngine;

public class PersistentVariables : MonoBehaviour
{
<<<<<<< HEAD
    public int level = 0;  // start on zero
    public int supply = 0; // xp
    public List<Ship> ships = new List<Ship>();
=======
    public int level = 0;
    public int supply = 0;
    public List<Ship> ships = new();
>>>>>>> 59e9c4cba23319cd6bc69e6f540a5b717e5ec746

    void Start()
    {
        DontDestroyOnLoad(this);
    }
}
