using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MapGenerator : MonoBehaviour
{

    public GameObject player;

    public bool center = false;

    public Transform FloorTrans;
    public Renderer FloorRend;

    public GameObject boulder;
    public GameObject tree;
    public GameObject grass;
    
    [Range(0, 1f)]
    public float offsetMapCenterPow = 0.375f;
    [Range(0, 5f)]
    public float offsetMapCenterSub = 1f;

    [Range(0, 1.5f)]
    public float offsetMapOuterPow = 0.5f;
    [Range(0, 5f)]
    public float offsetMapOuterSub = 3.25f;

    public float zLength = 25.6f;
    public float xLength = 38.4f;
    public float size = 0.8f;
    public float spread = 0;

    public float noiseScale = 50;
    public int octaves = 5;
    [Range(0, 1)]
    public float persistance = 0.5f;
    public float lacunarity = 2;
    
    public Noise.NormalizeMode normalizeMode;

    GameObject floorObj = null;

    public GameObject[] craftables;

    [HideInInspector]
    public GameObject[,] gameObjectMap;
    [HideInInspector]
    public bool[,] openMap;
    int[,] colorMap;

    // Start is called before the first frame update
    void Start() {
        player = Instantiate(player, new Vector3(0.4f, 0f, 0.4f), new Quaternion(0, 0, 0, 0));
        player.transform.GetChild(2).GetComponent<PlaceObject>().map = this;
        player.transform.GetChild(1).GetComponent<Infrount>().map = this;
        int seed = Random.Range(int.MinValue, int.MaxValue);
        CreateMap(seed);
        ColorFloor();
    }

    void CreateMap(int seed) {
        GameObject obj = null;
        gameObjectMap = new GameObject[((int)System.Math.Round(zLength / size)), (int)System.Math.Round(xLength / size)];
        openMap = new bool[((int)System.Math.Round(zLength / size)), (int)System.Math.Round(xLength / size)];
        colorMap = new int[((int)System.Math.Round(zLength / size)), (int)System.Math.Round(xLength / size)];

        for (int x = 0; x <= gameObjectMap.GetUpperBound(0); x++) {
            for (int y = 0; y <= gameObjectMap.GetUpperBound(1); y++) {
                openMap[x, y] = false;
            }
        }

        int az = 0;
        int ax = 0;
        float[,] noiseMap = Noise.GenerateNoiseMap((int)System.Math.Round(zLength / size), (int)System.Math.Round(xLength / size), seed, noiseScale, octaves, persistance, lacunarity, Vector2.zero, normalizeMode);
        float[,] offsetMap = CreateCenterMapOffset();
        
        float[,] finalMap = noiseMap;
        if (center) {
            finalMap = CombineMaps(noiseMap, offsetMap);
        }

        for (float z = -((zLength / 2) - (size / 2)); z <= ((zLength / 2) - (size / 2)) + 0.0001f; z += 0.8f + spread) {
            for (float x = -((xLength / 2) - (size / 2)); x <= ((xLength / 2) - (size / 2)) + 0.0001f; x += 0.8f + spread) {
                obj = null;
                colorMap[az, ax] = 1;
                if (finalMap[az, ax] >= 0.7f) {
                    obj = Instantiate(boulder, new Vector3(x, 0, z), new Quaternion(0, 0, 0, 0));
                    colorMap[az, ax] = 3;
                } else if (finalMap[az, ax] >= 0.5f) {
                    obj = Instantiate(tree, new Vector3(x, 0, z), new Quaternion(0, 0, 0, 0));
                    colorMap[az, ax] = 2;
                } else if (finalMap[az, ax] >= 0.45f) {
                    obj = Instantiate(grass, new Vector3(x, 0, z), new Quaternion(0, 0, 0, 0));
                    obj.transform.GetChild(0).SendMessage("ChangeMesh", 4, SendMessageOptions.RequireReceiver);
                } else if (finalMap[az, ax] >= 0.4f) {
                    obj = Instantiate(grass, new Vector3(x, 0, z), new Quaternion(0, 0, 0, 0));
                    obj.transform.GetChild(0).SendMessage("ChangeMesh", 3, SendMessageOptions.RequireReceiver);
                } else if (finalMap[az, ax] >= 0.35f) {
                    obj = Instantiate(grass, new Vector3(x, 0, z), new Quaternion(0, 0, 0, 0));
                    obj.transform.GetChild(0).SendMessage("ChangeMesh", 2, SendMessageOptions.RequireReceiver);
                } else if (finalMap[az, ax] >= 0.3f) {
                    obj = Instantiate(grass, new Vector3(x, 0, z), new Quaternion(0, 0, 0, 0));
                    obj.transform.GetChild(0).SendMessage("ChangeMesh", 1, SendMessageOptions.RequireReceiver);
                } else if (finalMap[az, ax] >= 0.2f) {
                    obj = Instantiate(grass, new Vector3(x, 0, z), new Quaternion(0, 0, 0, 0));
                    obj.transform.GetChild(0).SendMessage("ChangeMesh", 0, SendMessageOptions.RequireReceiver);
                }
                gameObjectMap[az, ax] = obj;
                openMap[az, ax] = true;
                if (obj != null) {
                    if (obj.transform.GetChild(0).GetComponent<ChangeMesh>() != null) {
                        obj.transform.GetChild(0).GetComponent<ChangeMesh>().from = this;
                    }
                }
                ax++;
            }
            ax = 0;
            az++;
        }
    }

    void ColorFloor() {
        FloorTrans.localScale = new Vector3(xLength / 10f, 1, zLength / 10f);
        FloorRend.sharedMaterial.mainTexture = TextureGenerator.TextureFromMap(colorMap);
    }

    float[,] CombineMaps(float[,] map1, float[,] map2) {

        for (int z = 0; z <= gameObjectMap.GetUpperBound(0); z++) {
            for (int x = 0; x <= gameObjectMap.GetUpperBound(1); x++) {
                map1[z, x] = Mathf.Clamp01(map1[z, x] + map2[z, x]);

            }
        }
        return map1;
    }

    float[,] CreateCenterMapOffset() {
        float[,] offsetMap = new float[((int)System.Math.Round(zLength / size)), (int)System.Math.Round(xLength / size)];
        int az = 0;
        int ax = 0;
        for (float z = -((zLength / 2) - (size / 2)); z <= ((zLength / 2) - (size / 2)) + 0.0001f; z += 0.8f + spread) {
            for (float x = -((xLength / 2) - (size / 2)); x <= ((xLength / 2) - (size / 2)) + 0.0001f; x += 0.8f + spread) {
                offsetMap[az, ax] = Mathf.Clamp01(Mathf.Pow(Mathf.Sqrt((z * z) + (x * x * 0.4f)), offsetMapOuterPow) - offsetMapOuterSub);
                offsetMap[az, ax] += Mathf.Clamp01(Mathf.Pow(Mathf.Sqrt((z * z) + (x * x * 0.75f)), offsetMapCenterPow) - offsetMapCenterSub)-1;
                //offsetMap[az, ax] = Mathf.Clamp01(offsetMap[az, ax]);
                ax++;
            }
            ax = 0;
            az++;
        }
        return offsetMap;
    }
    
    void OnValidate() {
        if (lacunarity < 1) {
            lacunarity = 1;
        }
        if (octaves < 0) {
            octaves = 0;
        }
    }

    // Update is called once per frame

    public void Remove(int x, int y) {
        Destroy(gameObjectMap[x, y]);
        gameObjectMap[x, y] = null;
    }

    public void ClearMap() {
        Destroy(floorObj);
        for (int x = 0; x <= gameObjectMap.GetUpperBound(0); x++) {
            for (int y = 0; y <= gameObjectMap.GetUpperBound(1); y++) {
                if (gameObjectMap[x, y] != null) {
                    Remove(x, y);
                }
            }
        }
    }

    void Update() {
        if (Input.GetKeyDown(KeyCode.P)) {
            for (int x = 0; x <= gameObjectMap.GetUpperBound(0); x++) {
                for (int y = 0; y <= gameObjectMap.GetUpperBound(1); y++) {
                    Debug.Log(gameObjectMap[x, y] + " " + x + " " + y);
                }
            }
        }

        if (Input.GetKeyDown(KeyCode.C)) {
            ClearMap();
        }

        if (Input.GetKeyDown(KeyCode.O)) {
            ClearMap();
            int seed = Random.Range(int.MinValue, int.MaxValue);
            CreateMap(seed);
            ColorFloor();
        }
    }
}
