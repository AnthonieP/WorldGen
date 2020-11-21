using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ChunkPlacement : MonoBehaviour
{
    public GameObject chunk;
    public int worldSizeX;
    public int worldSizeZ;
    public float OffsetX;
    public float OffsetY;
    public bool randomWorld;
    public bool generateAnotherLayer;

    void Start()
    {
        if (randomWorld)
        {
            OffsetX = Random.Range(-999f, 999f);
            OffsetY = Random.Range(-999f, 999f);
        }

        GenerateWorld();
    }

    private void Update()
    {
        if (generateAnotherLayer)
        {
            generateAnotherLayer = false;
            for (int z = 0; z < worldSizeZ + 1; z++)
            {
                for (int x = 0; x < worldSizeX + 1; x++)
                {
                    if(x == worldSizeX || z == worldSizeZ)
                    {
                        ChunkGen tempChunk = Instantiate(chunk, transform.position, Quaternion.identity, transform).GetComponent<ChunkGen>();
                        tempChunk.offsetX = OffsetX;
                        tempChunk.offsetY = OffsetY;
                        tempChunk.CalculateStep();
                        tempChunk.transform.position = new Vector3(tempChunk.stepX * x, 0, tempChunk.stepY * z);
                        tempChunk.GenerateChunk();

                    }

                }
            }
            worldSizeX++;
            worldSizeZ++;
        }
    }

    void GenerateWorld()
    {
        for (int z = 0; z < worldSizeZ; z++)
        {
            for (int x = 0; x < worldSizeX; x++)
            {
                ChunkGen tempChunk = Instantiate(chunk,transform.position, Quaternion.identity, transform).GetComponent<ChunkGen>();
                tempChunk.offsetX = OffsetX;
                tempChunk.offsetY = OffsetY;
                tempChunk.CalculateStep();
                tempChunk.transform.position = new Vector3(tempChunk.stepX * x, 0, tempChunk.stepY * z);
                tempChunk.GenerateChunk();
            }
        }
    }
}
