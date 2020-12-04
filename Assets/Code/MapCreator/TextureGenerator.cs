using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class TextureGenerator {

    public static Texture2D TextureFromMap(int[,] ColorObjMap) {
        int width = ColorObjMap.GetLength(1);
        int height = ColorObjMap.GetLength(0);

        Color[] colorMap = new Color[width * height];
        for (int y = 0; y < height; y++) {
            for (int x = 0; x < width; x++) {
                switch (ColorObjMap[y, x]) {
                    case (1):
                        colorMap[y * width + x] = new Color(133 / 256f, 211 / 256f, 67 / 256f);
                        break;

                    case (2):
                        colorMap[y * width + x] = new Color(133 / 256f, 94 / 256f, 52 / 256f);
                        break;

                    case (3):
                        colorMap[y * width + x] = new Color(46 / 256f, 51 / 256f, 58 / 256f);
                        break;


                }
            }
        }
        return TextureFromColorMap(colorMap, width, height);
    }

    public static Texture2D TextureFromColorMap(Color[] colorMap, int width, int height) {
        Texture2D texture = new Texture2D(width, height);
        texture.filterMode = FilterMode.Point;
        texture.wrapMode = TextureWrapMode.Clamp;
        texture.SetPixels(colorMap);
        texture.Apply();
        return texture;
    }
    
}
