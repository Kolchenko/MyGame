using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class UnitInfoPrint : MonoBehaviour {
    private static bool isDisplayInfo = false;
    private static Unit unit;
    public TextMeshProUGUI UnitName;
    public GameObject UnitInfoPanel;
    public TextMeshProUGUI AttackValue;
    public TextMeshProUGUI DefenseValue;
    public TextMeshProUGUI DamageValue;
    public TextMeshProUGUI HealthValue;
    public TextMeshProUGUI SpeedValue;
    
    private void Update () {
        if (Input.GetMouseButtonDown((int)MouseButton.RIGHT) && !isDisplayInfo)
        {
            RaycastHit hitInfo = new RaycastHit();
            bool hit = Physics.Raycast(Camera.main.ScreenPointToRay(Input.mousePosition), out hitInfo);
            if (hit)
            {
                unit = hitInfo.transform.gameObject.GetComponent<Unit>();
                if (unit != null)
                {
                    DisplayUnitInfo();
                }
            }
        }
        else if (Input.GetMouseButtonDown((int)MouseButton.LEFT) && isDisplayInfo)
        {
            HideUnitInfo();
        }
    }

    private void DisplayUnitInfo()
    {
        UnitInfoPanel.SetActive(true);
        isDisplayInfo = true;
        UnitName.text = unit.LocalizeTag();
        AttackValue.text = unit.attackSkill.ToString();
        DefenseValue.text = unit.defenseSkill.ToString();
        DamageValue.text = unit.damageMin + "-" + unit.damageMax;
        HealthValue.text = unit.health.ToString();
        SpeedValue.text = unit.distance.ToString();
        UnitInfoPanel.transform.position = Input.mousePosition;
    }

    private void HideUnitInfo()
    {
        UnitInfoPanel.SetActive(false);
        isDisplayInfo = false;
    }
}
