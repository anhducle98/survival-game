using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PlayerStats : MonoBehaviour
{
    [SerializeField]
    private Image healthStats, statminaStats;


    public void DisplayHealthStats(float healthValue) {
        healthValue /= 100f;
        healthStats.fillAmount = healthValue;
    }

    public void DisplayStaminaStats(float staminaValue) {
        staminaValue /= 100f;
        statminaStats.fillAmount = staminaValue;
    }
}
