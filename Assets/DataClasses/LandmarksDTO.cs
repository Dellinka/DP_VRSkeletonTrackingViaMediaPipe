using System.Collections;
using System.Collections.Generic;
using UnityEngine;


[System.Serializable]
public class LandmarksDTO
{
    public List<CoordsDTO> landmarks;

    public override string ToString()
    {
        string toPrint = "";

        int i = 0;
        foreach (CoordsDTO c in landmarks)
        {
            toPrint += i + ". " + c.ToString() + "\n";
            i++;
        }

        return toPrint;
    }
}
