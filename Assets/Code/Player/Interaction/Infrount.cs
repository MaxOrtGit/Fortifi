using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Infrount : MonoBehaviour
{
    public bool animate = false;
    public bool activeMining = false;
    GameObject holding = null;
    public MapGenerator map;
    public bool holdingE = false;
    List<GameObject> queue = new List<GameObject>();
    GameObject activeOn;
    Collider toAdd = null;

    [HideInInspector]
    public bool isAlwaysOn = false;

    GameObject transparent = null;

    void OnTriggerEnter(Collider other) {
        if (other.gameObject.transform.childCount != 0 && other.gameObject.transform.GetChild(0).GetComponent<ChangeMesh>() != null) {
            toAdd = other;
            if (!other.gameObject.transform.GetChild(0).GetComponent<ChangeMesh>().IfAllWaysOn()) {
                queue.Add(toAdd.gameObject);
                if (queue.Count == 1) {
                    activeOn = toAdd.gameObject;
                    animate = true;
                    if (other.gameObject.transform.GetChild(0).GetComponent<ChangeMesh>() != null) {
                        other.gameObject.transform.GetChild(0).GetComponent<ChangeMesh>().SetActive(activeMining);
                        other.gameObject.transform.GetChild(0).GetComponent<ChangeMesh>().DoAnimate(this);
                    }
                }
            }
        }
    }

    private void OnTriggerStay(Collider other) {
        if (holding == null && other.gameObject.transform.GetComponent<Item>() != null && Input.GetKeyDown(KeyCode.E)) {
            holdingE = true;
            holding = other.gameObject;
            int pz = (int)Mathf.Round((holding.transform.position.z + ((map.zLength / 2) - (map.size / 2))) / (map.size + map.spread));
            int px = (int)Mathf.Round((holding.transform.position.x + ((map.xLength / 2) - (map.size / 2))) / (map.size + map.spread));
            map.gameObjectMap[pz, px] = null;
            other.gameObject.transform.parent = this.transform;
            other.gameObject.transform.localPosition = new Vector3(0, -0.3f, 0.07f);
            other.gameObject.transform.localRotation = Quaternion.Euler(0, 0, 0);
        }
    }

    void OnTriggerExit(Collider other) {
        queue.Remove(other.gameObject);
        animate = false;
        if (other.gameObject.transform.childCount != 0 && other.gameObject.transform.GetChild(0).GetComponent<ChangeMesh>() != null) {
            other.gameObject.transform.GetChild(0).GetComponent<ChangeMesh>().SetActive(false);
            other.gameObject.transform.GetChild(0).GetComponent<ChangeMesh>().DoAnimate(this);
        }
        if (activeOn == other.gameObject) {
            Next();
        }
    }

    public void Next() {
        queue.Remove(activeOn);
        if (queue.Count != 0) {
            activeOn = queue[0];
            animate = true;
            if (activeOn.transform.childCount != 0 && activeOn.transform.GetChild(0).GetComponent<ChangeMesh>() != null) {
                activeOn.gameObject.transform.GetChild(0).GetComponent<ChangeMesh>().SetActive(activeMining);
                activeOn.gameObject.transform.GetChild(0).GetComponent<ChangeMesh>().DoAnimate(this);
            }
        } else {
            activeOn = null;
        }
    }

    void Update() {

        if (Input.GetKeyDown(KeyCode.E) && !holdingE && holding != null && false) {
            Debug.Log("never");
            Destroy(transparent);
            switch (PlaceObject.PlaceGameObject(holding, this.transform, map)) {
                case null:
                    transparent = Instantiate(holding);
                    Vector3 pos = transparent.transform.localPosition;
                    transparent.transform.parent = this.transform.parent.parent;
                    int pz = (int)Mathf.Round((this.transform.position.z + ((map.zLength / 2) - (map.size / 2))) / (map.size + map.spread));
                    int px = (int)Mathf.Round((this.transform.position.x + ((map.xLength / 2) - (map.size / 2))) / (map.size + map.spread));
                    transparent.transform.localPosition = pos;
                    transparent.transform.localRotation = new Quaternion(0, 0, 0, 0);

                    Material newMat = transparent.transform.GetChild(0).GetComponent<Renderer>().material;
                    newMat.SetInt("_SrcBlend", (int)UnityEngine.Rendering.BlendMode.One);
                    newMat.SetInt("_DstBlend", (int)UnityEngine.Rendering.BlendMode.OneMinusSrcAlpha);
                    newMat.SetInt("_ZWrite", 0);
                    newMat.DisableKeyword("_ALPHATEST_ON");
                    newMat.DisableKeyword("_ALPHABLEND_ON");
                    newMat.EnableKeyword("_ALPHAPREMULTIPLY_ON");
                    newMat.renderQueue = 3000;
                    newMat.SetColor("_Color", new Color(127 / 255f, 127 / 255f, 255 / 255f, 127 / 255f));
                    transparent.transform.GetChild(0).GetComponent<Renderer>().material = newMat;
                    break;
                default:
                    break;
            }
        }

        if (Input.GetKeyUp(KeyCode.E) && !holdingE && holding != null) {
            holdingE = true;
            GameObject placedObject = PlaceObject.PlaceGameObject(holding, this.transform, map);
            Debug.Log(placedObject);
            if (placedObject == holding) {
                Vector3 pos = holding.transform.localPosition;
                holding.transform.parent = this.transform.parent.parent;
                int pz = (int)Mathf.Round((pos.z + ((map.zLength / 2) - (map.size / 2))) / (map.size + map.spread));
                int px = (int)Mathf.Round((pos.x + ((map.xLength / 2) - (map.size / 2))) / (map.size + map.spread));
                holding.transform.localPosition = pos;
                map.gameObjectMap[pz, px] = holding;
                holding.transform.localRotation = new Quaternion(0, 0, 0, 0);
                holding = null;

            } else if (placedObject != null) {
                Vector3 pos = holding.transform.localPosition;
                int pz = (int)Mathf.Round((pos.z + ((map.zLength / 2) - (map.size / 2))) / (map.size + map.spread));
                int px = (int)Mathf.Round((pos.x + ((map.xLength / 2) - (map.size / 2))) / (map.size + map.spread));

                placedObject = Instantiate(placedObject, pos, new Quaternion(0, 0, 0, 0));

                Destroy(map.gameObjectMap[pz, px]);
                Destroy(holding);

                holding = null;
                map.gameObjectMap[pz, px] = placedObject;
            }
        }

        if (Input.GetKeyUp(KeyCode.E)) {
            holdingE = false;
        }

        if (Input.GetMouseButtonDown(0)) {
            activeMining = true;
            for (int i = 0; i < queue.Count; i++) {
                if(queue[i] != null) {
                    if (queue[i].transform.GetChild(0).GetComponent<ChangeMesh>() != null) {
                        queue[i].transform.GetChild(0).GetComponent<ChangeMesh>().SetActive(true);
                    }
                } else {
                    Next();
                }
            }
        }

        if (Input.GetMouseButtonUp(0)) {
            activeMining = false;
            for (int i = 0; i < queue.Count; i++) {
                if (queue[i].transform.GetChild(0).GetComponent<ChangeMesh>() != null) {
                    queue[i].transform.GetChild(0).GetComponent<ChangeMesh>().SetActive(false);
                }
            }
        }
        
    }
}
