using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ChangeMesh : MonoBehaviour
{

    [System.Serializable]
    public struct Meshes {
        public Mesh mesh;
        public float waitFor;
    }

    public MapGenerator from;

    public bool destroy = false;
    public bool globalWait = true;
    public float GWaitFor = 0.5f;
    public bool alwaysOn = true;
    public Meshes[] meshes;

    [HideInInspector]
    public bool differentStart = false;

    [HideInInspector]
    public int on = 0;
    float elapsed = 0f;
    bool animates = false;
    bool active = false;

    Infrount sentFrom;

    // Start is called before the first frame update
    void Start()
    {
        if (!differentStart) {
            GetComponent<MeshFilter>().sharedMesh = meshes[0].mesh;
        }
        if (globalWait) {
            for(int i = 0; i < meshes.Length; i++) {
                meshes[i].waitFor = GWaitFor;
            }
        }
    }
    
    public bool IfAllWaysOn() {
        return alwaysOn;
    }

    public void DoAnimate(Infrount sender) {
        sentFrom = sender;
        animates = sentFrom.animate;
    }

    public void SetActive(bool isActive) {
        active = isActive;
    }

    void Update() {
        if ((animates && active) || alwaysOn) {
            elapsed += Time.deltaTime;
            GetComponent<MeshFilter>().sharedMesh = meshes[on].mesh;
            while (elapsed >= meshes[on].waitFor) {
                if (meshes[on].waitFor >= 0) {
                    elapsed -= meshes[on].waitFor;
                    on++;
                } else {
                    elapsed += meshes[on].waitFor;
                    on--;
                }
                if (meshes.Length == on || on == -1) {
                    if (destroy) {
                        if (sentFrom != null) {
                            if (transform.parent.GetComponent<DropItems>() != null) {
                                if (sentFrom != null) {
                                    int z = Mathf.RoundToInt((this.transform.position.z + from.zLength / 2) * (from.size + from.spread));
                                    int x = Mathf.RoundToInt((this.transform.position.x + from.xLength / 2) * (from.size + from.spread));
                                    from.gameObjectMap[z, x] = transform.parent.GetComponent<DropItems>().DropItem(transform.parent.localPosition);
                                }
                            }
                            sentFrom.Next();
                        }
                        Destroy(this.transform.parent.gameObject);
                    }
                    on = 0;
                }
            }
        }
    }
}
