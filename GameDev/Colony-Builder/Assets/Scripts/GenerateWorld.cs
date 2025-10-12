using UnityEngine;

public class GenerateWorld : MonoBehaviour
{
    public float radius = 3;
    public float regionSize = 20;
    public float noiseScale = 1;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        PlaceTrees();        
    }


    public void PlaceTrees()
    {
        var treefab = Resources.Load("Prefabs/Tree");

        var trees = PoissonDiscSampling.GeneratePoints(radius, Vector2.one * regionSize);

        var offset = (new Vector2(.5f, .5f) * regionSize);

        foreach (var p in trees)
        {
            float n = Mathf.PerlinNoise(p.x * noiseScale, p.y * noiseScale);
            if(n > 0.5)
            {
                Instantiate(treefab, p - offset, Quaternion.identity);
            }
        }
    }


}
