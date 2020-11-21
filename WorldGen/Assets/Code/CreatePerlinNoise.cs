using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CreatePerlinNoise : MonoBehaviour
{
    public Texture2D GeneratePerlinNoiseTexture(int width, int height, float scale, float offsetX, float offsetY)
    {
        Texture2D texture = new Texture2D(width, height);


        for (int x = 0; x < width; x++)
        {
            for (int y = 0; y < height; y++)
            {
                Color color = CalculatePerlinColor(x, y,width, height,scale, offsetX, offsetY);
                print(CalculatePerlinFloat(x, y, width, height, scale, offsetX, offsetY));
                texture.SetPixel(x, y, color);
            }
        }

        texture.Apply();
        return texture;
    }

    public Color CalculatePerlinColor(int x, int y, int width, int height, float scale, float offsetX, float offsetY)
    {
        float xCoord = (float)x / width * scale + offsetX;
        float yCoord = (float)y / height * scale + offsetY;

        float noicePixel = Mathf.PerlinNoise(xCoord, yCoord);
        return new Color(noicePixel, noicePixel, noicePixel);
    }

    public float CalculatePerlinFloat(int x, int y, int width, int height, float scale, float offsetX, float offsetY)
    {
        float xCoord = (float)x / width * scale + offsetX;
        float yCoord = (float)y / height * scale + offsetY;

        float noicePixel = Mathf.PerlinNoise(xCoord, yCoord);
        return noicePixel;
    }
}
