using QFSW.QC.Actions;
using System.Collections;
using UnityEngine;

public class Ragdoll : MonoBehaviour 
{
    public float fadeDuration;

    
    bool destroying = false;

    public void BeginDeath()
    {
        StartCoroutine(Death());
    }

    IEnumerator Death()
    {
        destroying = true;

        var sr = GetComponent<SpriteRenderer>();

        float elapsedTime = 0f;


        int i = 0;


        while (elapsedTime < fadeDuration)
        {
            elapsedTime += Time.deltaTime;
            float fade = Mathf.Lerp(1f, 0f, elapsedTime / fadeDuration);
            sr.material.SetFloat("_Fade", fade);

            yield return null; // Wait for the next frame
        }

        Destroy(gameObject);
    }

}
