using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DropItems : MonoBehaviour
{

    public GameObject ItemToDrop;

    public GameObject DropItem(Vector3 loc) {
        return Instantiate(ItemToDrop, loc, new Quaternion(0, 0, 0, 0));
    }
}
