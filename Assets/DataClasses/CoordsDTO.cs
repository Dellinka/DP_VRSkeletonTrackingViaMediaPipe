using System.Collections;
using System.Collections.Generic;
using UnityEngine;


[System.Serializable]
public class CoordsDTO
{
    public float x;
    public float y;
    public float z;

    public override string ToString()
    {
        return "(x = " + x + ", y = " + y + ", z = " + z + ")";
    }
}
