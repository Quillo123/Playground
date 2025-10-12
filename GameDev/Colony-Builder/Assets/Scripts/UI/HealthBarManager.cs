using System.Collections;
using System.Collections.Generic;
using UnityEngine;


/// <summary>
/// Manages Unit HealthBars
/// Press CTRL+J to show all health bars (not just selected)
/// Press CTRL+H to hide all health bars (even if selected)
/// </summary>
public class HealthBarManager : MonoBehaviour
{
    public static HealthBarManager Instance;

    public bool showHealthBars = true;
    public bool showOnlySelected = true;
    public bool showOnlyDamaged = true; 
    public GameObject HealthBar;

    Dictionary<Health, HealthBar> healthBars = new Dictionary<Health, HealthBar>();

    private void Awake()
    {
        // Singleton setup: Ensure only one instance exists
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject); // Persist across scenes
        }
        else
        {
            Destroy(gameObject); // Destroy duplicate instances
        }

    }

    private void Update()
    {
        if (Input.GetKey(KeyCode.LeftControl))        
        {
            if (Input.GetKeyDown(KeyCode.H))
            {
                ShowHealthBars(!showHealthBars);
            }
            if (Input.GetKeyDown(KeyCode.J))
            {
                showOnlySelected = !showOnlySelected;
            }
        }
    }

    public void ShowHealthBars(bool showBars)
    {
        if(showBars == showHealthBars)
        {
            return;
        }

        showHealthBars = showBars;

        foreach (var bar in healthBars)
        {
            bar.Value.gameObject.SetActive(showBars);
        }
    }

    public void CreateHealthBar(Health entity)
    {
        var h = Instantiate(HealthBar, this.transform).GetComponent<HealthBar>();
        h.entity = entity;
        healthBars.Add(entity, h);
    }

    public void RemoveHealthBar(Health entity)
    {
        Destroy(healthBars[entity]);
        healthBars.Remove(entity);
    }
}
