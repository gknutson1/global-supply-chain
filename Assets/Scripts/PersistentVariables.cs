using UnityEngine;

public class PersistentVariables : MonoBehaviour
{
    public int level = 0;

    void Start()
    {
        DontDestroyOnLoad(this);
    }
}
