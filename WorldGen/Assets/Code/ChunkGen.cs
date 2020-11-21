using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ChunkGen : MonoBehaviour
{
    [System.Serializable]
    public class NoisePass
    {
        public float scale;
        public float scaleMultiplier;
        [Range(0,1)]
        public float minHeight;
        [Range(0, 10)]
        public float angleMultiplier;
    }
    [System.Serializable]
    public class Art
    {
        public GameObject prefab;
        public float amoundPerChunk;
        public Vector2 scaleVeriation;
        [Range(0, 1)]
        public float propAngleSpanw;

    }

    public MeshFilter world;
    public MeshCollider worldCollider;
    [Header("Props")]
    public Art[] props;
    [Header("WorldGenSettings")]
    public float vertDis;
    public float heightMultiplier;
    public int chunkSizeX;
    public int chunkSizeY;
    [Header("PerlinNoise")]
    public NoisePass[] noisePasses;
    [Header("Debug")]
    public bool resetChunk;
    public Vector3[] locs;
    public float offsetX;
    public float offsetY;
    public float stepX = 0;
    public float stepY = 0;

    private void Update()
    {
        if (resetChunk)
        {
            resetChunk = false;
            GenerateChunk();
        }
    }

    public void CalculateStep()
    {
        stepX = chunkSizeX * vertDis - vertDis;
        stepY = chunkSizeY * vertDis - vertDis;
    }

    
    public void GenerateChunk()
    {
        Vector3[] verteciLoc = CreateVertGrid().ToArray();
        locs = verteciLoc;
        int[] triangles = triangelOrder();

        //Create mesh
        world.mesh = new Mesh();
        world.mesh.vertices = verteciLoc;
        world.mesh.triangles = triangles;
        world.mesh.RecalculateNormals();
        worldCollider.sharedMesh = world.mesh;

        //Put down clutter
        PutDownProps();
    }

    List<Vector3> CreateVertGrid()
    {
        
        
        List<Vector3> grid = new List<Vector3>();
        for (int z = 0; z < chunkSizeY; z++)
        {
            for (int x = 0; x < chunkSizeX; x++)
            {
                float finalNoise = 0;
                float tempFinalNoise = 0;
                for (int i = 0; i < noisePasses.Length; i++)
                {
                    float offSetCalculatedX = (transform.position.x / (chunkSizeX * vertDis) * noisePasses[i].scale) + offsetX;
                    float offSetCalculatedY = (transform.position.z / (chunkSizeY * vertDis) * noisePasses[i].scale) + offsetY;
                    tempFinalNoise = CalculatePerlinFloat(x, z, chunkSizeX, chunkSizeY, noisePasses[i].scale, offSetCalculatedX, offSetCalculatedY);
                    if(tempFinalNoise < noisePasses[i].minHeight)
                    {
                        tempFinalNoise = noisePasses[i].minHeight + (noisePasses[i].angleMultiplier * (tempFinalNoise * tempFinalNoise));
                    }
                    else
                    {
                        tempFinalNoise += noisePasses[i].angleMultiplier * (tempFinalNoise * tempFinalNoise);
                    }
                    finalNoise += tempFinalNoise;
                    finalNoise *= noisePasses[i].scaleMultiplier;
                }
                grid.Add(new Vector3(x * vertDis, finalNoise * heightMultiplier, z * vertDis));
            }
        }

        return grid;
    }

    int[] triangelOrder()
    {
        List<int> intList = new List<int>();

        for (int y = 0; y < chunkSizeY - 1; y++)
        {
            for (int x = 0; x < chunkSizeX - 1; x++)
            {
                int i = x + (chunkSizeX * y);
                intList.Add(i);
                intList.Add(i + chunkSizeX);
                intList.Add(i + 1);

                intList.Add(i + chunkSizeX);
                intList.Add(i + chunkSizeX + 1);
                intList.Add(i + 1);
            }
        }

        

        return intList.ToArray();
    }

    void PutDownProps()
    {
        RaycastHit hit;
        for (int i = 0; i < props.Length; i++)
        {
            for (int j = 0; j < props[i].amoundPerChunk; j++)
            {
                Vector3 rayPos= new Vector3(Random.Range(transform.position.x, transform.position.x + stepX), locs[1].y + + 100, Random.Range(transform.position.z, transform.position.z + stepY));
                if (Physics.Raycast(rayPos, -transform.up, out hit))
                {
                    if(hit.normal.y > props[i].propAngleSpanw)
                    {
                        if(hit.transform.tag == "Ground")
                        {
                            GameObject spawn = Instantiate(props[i].prefab, hit.point, Quaternion.identity, transform);
                            spawn.transform.Rotate(0, Random.Range(0f, 360f), 0);
                            float randomScale = spawn.transform.localScale.x + Random.Range(props[i].scaleVeriation.x, props[i].scaleVeriation.y);
                            spawn.transform.localScale = new Vector3(randomScale, randomScale, randomScale);
                        }

                    }
                }

            }

        }
    }

    float CalculatePerlinFloat(int x, int y, int width, int height, float scale, float offsetX, float offsetY)
    {
        float xCoord = (float)x / width * scale + offsetX;
        float yCoord = (float)y / height * scale + offsetY;

        float noicePixel = Mathf.PerlinNoise(xCoord, yCoord);
        return noicePixel;
    }
}
