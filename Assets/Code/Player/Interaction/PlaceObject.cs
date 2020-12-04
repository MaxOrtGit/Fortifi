using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlaceObject : MonoBehaviour
{
    [System.Serializable]
    public struct Objects
    {
        public GameObject gameObject;
    }

    public MapGenerator map;


    public Objects[] objects;

    public Vector3 oldLoc;
    public bool placing = false;
    GameObject placedTransparent;
    

    void Update() {
        /*
        if (Input.GetKeyDown(KeyCode.R)) {
            placing = true;
        }

        if (Input.GetKeyUp(KeyCode.R)) {
            placing = false;
            Destroy(placedTransparent);
            GameObject placedObj = objects[0].gameObject;
            if (PlaceGameObject(placedObj, this.transform, map)) {
                Instantiate(objects[0].gameObject, placedObj.transform.localPosition, new Quaternion(0, 0, 0, 0));
            }
        }
         
        if (placing) {
            Destroy(placedTransparent);
            PlaceTransparentObject(objects[0].gameObject, this.transform);
        }
        */
    }

    public void PlaceTransparentObject(GameObject ObjToPlace, Transform startTransform) {
        Vector3 startLoc = new Vector3(Mathf.Round((startTransform.transform.position.x + 0.4f) / 0.8f) * 0.8f - 0.4f, 0, Mathf.Round((startTransform.transform.position.z + 0.4f) / 0.8f) * 0.8f - 0.4f);
        float offset = 0f;
        while (true) {
            offset += 0.1f;
            float offsetX = Mathf.Sin(Mathf.PI * startTransform.transform.parent.rotation.eulerAngles.y / 180.0f) * offset;
            float offsetZ = Mathf.Cos(Mathf.PI * startTransform.transform.parent.rotation.eulerAngles.y / 180.0f) * offset;
            Vector3 nextLoc = new Vector3(Mathf.Round((startTransform.transform.position.x + offsetX + 0.4f) / 0.8f) * 0.8f - 0.4f, 0, Mathf.Round((startTransform.transform.position.z + offsetZ + 0.4f) / 0.8f) * 0.8f - 0.4f);
            if (!startLoc.Equals(nextLoc)) {
                if (oldLoc == null || !oldLoc.Equals(nextLoc)) {
                    Destroy(placedTransparent);
                    int az = (int)Mathf.Round((nextLoc.z + ((map.zLength / 2) - (map.size / 2))) / (map.size + map.spread));
                    int ax = (int)Mathf.Round((nextLoc.x + ((map.xLength / 2) - (map.size / 2))) / (map.size + map.spread));
                    if (az >= map.gameObjectMap.GetUpperBound(0) || az < 0 || ax >= map.gameObjectMap.GetUpperBound(1) || ax < 0) {
                        Debug.Log("out of bounds " + az + " " + ax);
                        break;
                    }
                    if (map.gameObjectMap[az, ax] == null) {
                        placedTransparent = Instantiate(ObjToPlace, nextLoc, new Quaternion(0, 0, 0, 0));
                        placedTransparent.transform.GetComponent<BoxCollider>().isTrigger = true;

                        Material newMat = placedTransparent.transform.GetChild(0).GetComponent<Renderer>().material;
                        newMat.SetInt("_SrcBlend", (int)UnityEngine.Rendering.BlendMode.One);
                        newMat.SetInt("_DstBlend", (int)UnityEngine.Rendering.BlendMode.OneMinusSrcAlpha);
                        newMat.SetInt("_ZWrite", 0);
                        newMat.DisableKeyword("_ALPHATEST_ON");
                        newMat.DisableKeyword("_ALPHABLEND_ON");
                        newMat.EnableKeyword("_ALPHAPREMULTIPLY_ON");
                        newMat.renderQueue = 3000;
                        newMat.SetColor("_Color", new Color(127 / 255f, 127 / 255f, 255 / 255f, 127 / 255f));
                        placedTransparent.transform.GetChild(0).GetComponent<Renderer>().material = newMat;

                    } else {
                        placedTransparent = null;
                    }
                }
                break;
            }
        }
    }

    public static GameObject PlaceGameObject(GameObject ObjToPlace, Transform startTransform, MapGenerator map) {
        Vector3 startLoc = new Vector3(Mathf.Round((startTransform.transform.position.x + 0.4f) / 0.8f) * 0.8f - 0.4f, 0, Mathf.Round((startTransform.transform.position.z + 0.4f) / 0.8f) * 0.8f - 0.4f);
        float offset = 0f;
        while (true) {
            offset += 0.1f;
            float offsetX = Mathf.Sin(Mathf.PI * startTransform.transform.parent.rotation.eulerAngles.y / 180.0f) * offset;
            float offsetZ = Mathf.Cos(Mathf.PI * startTransform.transform.parent.rotation.eulerAngles.y / 180.0f) * offset;
            Vector3 nextLoc = new Vector3(Mathf.Round((startTransform.transform.position.x + offsetX + 0.4f) / 0.8f) * 0.8f - 0.4f, 0, Mathf.Round((startTransform.transform.position.z + offsetZ + 0.4f) / 0.8f) * 0.8f - 0.4f);
            if (!startLoc.Equals(nextLoc)) {
                int az = (int)Mathf.Round((nextLoc.z + ((map.zLength / 2) - (map.size / 2))) / (map.size + map.spread));
                int ax = (int)Mathf.Round((nextLoc.x + ((map.xLength / 2) - (map.size / 2))) / (map.size + map.spread));
                if (az >= map.gameObjectMap.GetUpperBound(0) || az < 0 || ax >= map.gameObjectMap.GetUpperBound(1) || ax < 0) {
                    Debug.Log("out of bounds" + az + " " + ax);
                    break;
                }
                if (map.gameObjectMap[az, ax] == null) {
                    ObjToPlace.transform.localPosition = nextLoc;
                    return ObjToPlace;
                } else {
                    if ((map.gameObjectMap[az, ax].GetComponent<Item>() != null) && (ObjToPlace.GetComponent<Item>() != null)) {
                        if (map.gameObjectMap[az, ax].GetComponent<Item>().id.Equals("stone") && ObjToPlace.GetComponent<Item>().id.Equals("wood")) {
                            ObjToPlace.transform.localPosition = nextLoc;
                            return (map.craftables[0]);
                        }
                    }
                }
                return null;
            }
        }
        return null;
    }
}
