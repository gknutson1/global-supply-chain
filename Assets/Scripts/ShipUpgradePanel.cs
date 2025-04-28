using System;
using TMPro;
using UnityEngine;
using UnityEngine.UI;


public class ShipUpgradePanel : MonoBehaviour {
    public enum Direct {Inc, Dec}

    private PersistentVariables pv;
    
    public Ship target;

    private TMP_Text SupplyText;

    
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start() {}
    // We need to wait to start so we can do setup stuff - this will be called later
    public void ActualStart() {
        pv = GameObject.Find("PersistentVariables").GetComponent<PersistentVariables>();
        
        UpgradeManager manager = GameObject.Find("UpgradeManager").GetComponent<UpgradeManager>();
        
        GameObject image_object = transform.Find("image").gameObject;
        RectTransform image_transform = image_object.GetComponent<RectTransform>();
        Image image = image_object.GetComponent<Image>();
        
        image.sprite = target.shipSprite;
        float height = image_transform.rect.width * (target.shipSprite.rect.height / target.shipSprite.rect.width);
        image_transform.SetSizeWithCurrentAnchors(RectTransform.Axis.Vertical, height);

        Panel speed_panel = new Panel(
            gameObject.transform.Find("speed_panel"), "speed",
            target.SpeedMax, target.SpeedAccel,
            target.upgradeSpeedMaxSpeedStep, target.upgradeSpeedAccelStep,
            target.upgradeSpeedLevelCur, target.upgradeSpeedLevelMax,
            target.upgradeSpeedCost, pv, target, manager
        );
        
        manager.OnUpgrade.AddListener(speed_panel.disableIfNeededAuto);
        
        Panel turn_panel = new Panel(
            gameObject.transform.Find("turn_panel"), "turn",
            target.TurnMax, target.TurnAccel,
            target.upgradeTurnMaxTurnStep, target.upgradeTurnAccelStep,
            target.upgradeTurnLevelCur, target.upgradeTurnLevelMax,
            target.upgradeTurnCost, pv, target, manager
        );
        manager.OnUpgrade.AddListener(turn_panel.disableIfNeededAuto);
        
        Panel atk_panel = new Panel(
            gameObject.transform.Find("atk_panel"), "atk",
            target.Strength, target.Accuracy,
            target.upgradeAtkDmgStep, target.upgradeAtkAccStep,
            target.upgradeAtkLevelCur, target.upgradeAtkLevelMax,
            target.upgradeAtkCost, pv, target, manager
        );
        manager.OnUpgrade.AddListener(atk_panel.disableIfNeededAuto);
        
        Panel def_panel = new Panel(
            gameObject.transform.Find("def_panel"), "def",
            target.MaxHealth, target.Evasion,
            target.upgradeDefHpStep, target.upgradeDefEvasionStep,
            target.upgradeDefLevelCur, target.upgradeDefLevelMax,
            target.upgradeDefCost, pv, target, manager
        );
        manager.OnUpgrade.AddListener(def_panel.disableIfNeededAuto);
    }

    // Update is called once per frame
    void Update() {
    }
    private class Panel {
        public Panel(
            Transform panel, String name,
            float stat_1_init, float stat_2_init, 
            float stat_1_inc, float stat_2_inc,
            int cur_level, int max_level, 
            int cost_per_level,
            PersistentVariables pv, Ship ship, UpgradeManager manager) {
            this.name = name;
            this.ship = ship;
            
            up = panel.Find("stat_up").GetComponent<Button>();
            up.onClick.AddListener(() => Change(Direct.Inc));
                
            down = panel.Find("stat_down").GetComponent<Button>();
            down.onClick.AddListener(() => Change(Direct.Dec));
            
            cost = panel.Find("cost").GetComponent<TMP_Text>();
            stat_1_box = panel.Find("stat_1").GetComponent<TMP_Text>();
            stat_2_box = panel.Find("stat_2").GetComponent<TMP_Text>();
            
            this.stat_1_init = stat_1_init;
            this.stat_1_inc = stat_1_inc;
            
            this.stat_2_init = stat_2_init;
            this.stat_2_inc = stat_2_inc;
            
            this.min_level = cur_level;
            this.max_level = max_level;
            this.cost_per_level = cost_per_level;
            this.pv = pv;
            
            stat_1_box.SetText(stat_1_init.ToString());
            stat_2_box.SetText(stat_2_init.ToString());

            this.manager = manager;
            
            // This will disable the buttons if needed
            Change(Direct.Dec, true);
            Change(Direct.Inc, true);
        }

        public String name;
        public Ship ship;
        
        public Button up;
        public Button down;
        public TMP_Text cost;
        public TMP_Text stat_1_box;
        public TMP_Text stat_2_box;

        public float stat_1_init;
        public float stat_1_inc;
        
        public float stat_2_init;
        public float stat_2_inc;

        public int cost_per_level;
        public int min_level;
        public int max_level;

        public PersistentVariables pv;
        public UpgradeManager manager;
        
        void Change(Direct direction, bool only_verify = false) {
            switch (name) {
                case "speed":
                    ChangeNoLambda(direction, ref ship.upgradeSpeedLevelCur, ref ship.SpeedMax, ref ship.SpeedAccel, only_verify);
                    break;
                case "turn":
                    ChangeNoLambda(direction, ref ship.upgradeTurnLevelCur, ref ship.TurnMax, ref ship.TurnAccel, only_verify);
                    break;
                case "atk":
                    ChangeNoLambda(direction, ref ship.upgradeAtkLevelCur, ref ship.Strength, ref ship.Accuracy, only_verify);
                    break;
                case "def":
                    ChangeNoLambda(direction, ref ship.upgradeDefLevelCur, ref ship.MaxHealth, ref ship.Evasion, only_verify);
                    break;
            }
        }
        
        private void ChangeNoLambda(Direct direction, ref int level, ref float stat_1, ref float stat_2, bool only_verify = false) {
            // Make sure we can actually preform the action
            if (!this.check(direction, level)) return;
            // If we only want to check if we can continue (e.g. when setting up the buttons) exit here.
            if (only_verify) return;
            level += (direction == Direct.Inc ? 1 : -1);
            Debug.Log($"level is {level}");
            pv.supply -= cost_per_level * (direction == Direct.Inc ? 1 : -1);
            manager.OnUpgrade.Invoke();
            cost.SetText($"Cost: {cost_per_level * (level - min_level)}");
            
            if (level > this.min_level) {
                stat_1 = stat_1_init + (stat_1_inc * (level - min_level));
                stat_2 = stat_2_init + (stat_2_inc * (level - min_level));
                

                stat_1_box.SetText($"{stat_1_init} → {stat_1}");
                stat_2_box.SetText($"{stat_2_init} → {stat_2}");
            }
            else {
                stat_1 = stat_1_init;
                stat_2 = stat_2_init;
                
                stat_1_box.SetText(stat_1.ToString());
                stat_2_box.SetText(stat_2.ToString());
            }

            // Disable the button if we can't upgrade any more
            this.check(direction, level);
        }

        public void disableIfNeededAuto() {
            switch (name) {
                case "speed":
                    disableIfNeeded(ship.upgradeSpeedLevelCur);
                    break;
                case "turn":
                    disableIfNeeded(ship.upgradeTurnLevelCur);
                    break;
                case "atk":
                    disableIfNeeded(ship.upgradeAtkLevelCur);
                    break;
                case "def":
                    disableIfNeeded(ship.upgradeDefLevelCur);
                    break;
            }
        }

        private void disableIfNeeded(int level) {
            // Enable up button if we are below max_level and we have enough supply
            this.up.interactable = (level < this.max_level && (pv.supply - cost_per_level) >= 0);
            
            // Enable down button if we are above min_level
            this.down.interactable = (level > this.min_level);
            
        }

        private bool check(Direct direction, int level) {
            disableIfNeeded(level);

            switch (direction) {
                case Direct.Inc:
                    return this.up.interactable;
                case Direct.Dec:
                    return this.down.interactable;
            }

            // Should not be accessable
            return false;
        }
    }

}
