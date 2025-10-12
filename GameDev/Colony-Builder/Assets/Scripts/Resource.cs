using NUnit.Framework;
using System.Collections;
using System.Collections.Generic;
using System.Xml.Serialization;
using UnityEngine;
using UnityEngine.UIElements;

public class Resource : MonoBehaviour
{
    public float breakTime = 2f;

    private float startTime = 0;

    public List<Item> drops;

    public Vector2 spawnOffset = Vector2.zero;

    public float spawnForce = 1;

    private bool destroying = false;

    SpriteRenderer sr;
    Health health;

    public Material defaultMat;
    public Material highlight;
    public Material dissolve;

    


    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.blue;
        Gizmos.DrawWireSphere((Vector2)transform.position + spawnOffset, 0.5f);
    }

    private void Start()
    {
        sr = GetComponent<SpriteRenderer>();
        if(sr == null)
        {
            sr = GetComponentInChildren<SpriteRenderer>();
        }
        health = GetComponent<Health>();
        health.OnDeath += DropItems;
    }

    private void OnMouseEnter()
    {
        if (!destroying)
        {
            sr.material.SetInt("_Highlight", 1);
        }
    }

    private void OnMouseExit()
    {
        if (!destroying)
        {
            sr.material.SetInt("_Highlight", 0);
        }
    }

    private void OnMouseOver()
    { 
        if(Input.GetMouseButton(0) && !destroying)
        {
            sr.material.SetFloat("_Radius", 1 - health.ratio);
            
            if(Time.time - startTime > 0.5)
            {
                startTime = Time.time;
                health.Damage(5);
            }
        }
    }

    public void DropItems(GameObject obj)
    {
        GameController.Instance.StartCoroutine(
            DropItemsOverTime(
                transform.position + spawnOffset.ToVector3(),
                drops,
                health.ragdoll.fadeDuration,
                spawnForce
                )
            );
    }

    private static IEnumerator DropItemsOverTime(Vector3 pos, List<Item> drops, float duration, float force)
    {

        float dropwait = (duration * .5f) / drops.Count;
        float droptime = dropwait;

        for(int i = 0; i < drops.Count; i++) {

            SpawnDrop(drops[i], pos, force);

            yield return new WaitForSeconds(dropwait); // Wait for the next frame
        }
    }

    static void SpawnDrop(Item drop, Vector3 spawnpos, float spawnForce)
    {
        var itemE = Instantiate(ItemDatabase.Instance.itemPrefab, spawnpos, Quaternion.identity);
        itemE.item = drop;

        var itemRB = itemE.GetComponent<Rigidbody2D>();
        if (itemRB != null)
        {
            itemRB.AddForce(Random.insideUnitCircle * spawnForce);
        }
    }



}

