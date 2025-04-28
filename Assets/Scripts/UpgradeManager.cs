using UnityEngine;
using System.Collections.Generic;
using UnityEngine.Events;
using TMPro;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class UpgradeManager : MonoBehaviour
{
    public UnityEvent OnUpgrade;
    
    public TMP_Text SupplyText;
    public PersistentVariables pv;
    public GameObject shipContainer;

	public GameObject ContainerPrefab;
    
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        pv = GameObject.Find("PersistentVariables").GetComponent<PersistentVariables>();
        shipContainer = GameObject.Find("ship_container");
        float width = 10;
        
        foreach (Ship ship in pv.ships) {
            GameObject container = (GameObject) GameObject.Instantiate(ContainerPrefab, shipContainer.transform);
            container.GetComponent<ShipUpgradePanel>().target = ship;
            container.GetComponent<ShipUpgradePanel>().ActualStart();
            container.transform.SetParent(shipContainer.transform);
            container.GetComponent<RectTransform>().anchoredPosition = new Vector2(width, 0);
            width += container.GetComponent<RectTransform>().rect.width + 10;
        }

        Debug.Log(width);
        
        SupplyText = GameObject.Find("supply_indicator").GetComponent<TMP_Text>();
        OnUpgrade.AddListener(UpdateSupplyText);
        UpdateSupplyText();
        GameObject.Find("level_indicator").GetComponent<TMP_Text>().SetText($"Level {pv.level}");
    }
    
    public void Continue() {
        SceneManager.LoadScene($"Level{pv.level}");
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    void UpdateSupplyText() {
        SupplyText.SetText($"Supply: {pv.supply}");
    }
}
