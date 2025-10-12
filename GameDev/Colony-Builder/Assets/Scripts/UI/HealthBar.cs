using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class HealthBar : MonoBehaviour
{
    public float defaultWidth = 80;
    public Health entity;
    [SerializeField] Image healthBarImage;
    [SerializeField] Image background;

    float maxHealth;

    private void Start()
    {
        entity.OnDeath += RemoveHealthBar;
    }

    void Update()
    {
        if (entity == null)
        {
            return;
        }

        //if (HealthBarManager.main.showOnlySelected)
        //{
        //    healthBarImage.gameObject.SetActive(entity.select);
        //    background.gameObject.SetActive(entity.select);
        //}
        //else
        //{
        //    healthBarImage.gameObject.SetActive(true);
        //    background.gameObject.SetActive(true);
        //}



        if(maxHealth != entity.maxHealth)
        {
            maxHealth = entity.maxHealth;

            float width = defaultWidth + (maxHealth / 30);

            healthBarImage.rectTransform.SetSizeWithCurrentAnchors(RectTransform.Axis.Horizontal, width);
            background.rectTransform.SetSizeWithCurrentAnchors(RectTransform.Axis.Horizontal, width + 10);
        }

        if (HealthBarManager.Instance.showOnlyDamaged)
        {
            bool enable = entity.health != maxHealth;
            healthBarImage.gameObject.SetActive(enable);
            background.gameObject.SetActive(enable);
        }

        var pos = Camera.main.WorldToScreenPoint(entity.transform.position + entity.healthBarPosition);
        transform.position = pos;

        float fillamount = (float)entity.health / (float)entity.maxHealth;

        healthBarImage.fillAmount = fillamount;
        healthBarImage.color = Color.Lerp(Color.red, Color.green, fillamount);
    }

    public void RemoveHealthBar(GameObject obj)
    {
        HealthBarManager.Instance.RemoveHealthBar(entity);
        Destroy(gameObject);
    }
}
