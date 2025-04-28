using UnityEngine;
using System.Collections.Generic;
using UnityEngine.Events;
using TMPro;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using System.Runtime.CompilerServices;

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
        pv.supply = 400;
        
        // Add any new ships for this level
        switch (++pv.level) {
            case 1:
                pv.ships = pv.level1NewShips;
                break;
            case 2:
                pv.ships = pv.level2NewShips;
                break;
            case 3:
                pv.ships = pv.level3NewShips;
                break;
            case 4:
                pv.ships = pv.level4NewShips;
                break;
            case 5:
                pv.ships = pv.level5NewShips;
                break;
        }

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
        pv.level++;
        SceneManager.LoadScene($"Story");
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
            TogglePause();
    }

    void UpdateSupplyText() {
        SupplyText.SetText($"Supply: {pv.supply}");
    }

    public Canvas UpgradeManagerCanvas;
    public Canvas PauseMenu;
    public Canvas OptionsMenu;

    bool isPaused = false;

    public void TogglePause()
    {
        isPaused ^= true;
        UpgradeManagerCanvas.enabled = !isPaused;
        PauseMenu.enabled = isPaused;
        OptionsMenu.enabled = false;
    }

    public void LoadShipBuilder()
    {
        SceneManager.LoadScene("ShipBuilder");
    }
}
