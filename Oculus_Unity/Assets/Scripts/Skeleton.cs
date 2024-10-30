using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Skeleton : MonoBehaviour
{
    [SerializeField] private Transform landmarkPrefab;
    [SerializeField] private int landmarksNum = 33;
    [SerializeField] private int scale = -3;


    private List<Transform> landmarksInstancesArray = new List<Transform>();
    private List<Vector3> landmarkCoordinates = new List<Vector3>();


    void Start()
    {
        for (int i = 0; i < landmarksNum; ++i)
        {
            var landmarkInstance = Instantiate(landmarkPrefab);
            landmarkInstance.transform.parent = gameObject.transform;
            landmarksInstancesArray.Add(landmarkInstance);

            // Initialize positions of landmarks
            landmarkInstance.transform.position = new Vector3(i, 0, 0);
        }
    }

    void Update()
    {
        int i = 0;
        foreach (Vector3 coords in landmarkCoordinates)
        {
            Vector3 newPosition = new Vector3(coords.x, coords.y, coords.z);

            landmarksInstancesArray[i].transform.position = scale*newPosition;

            i++;

            Debug.Log(i);
        }
    }

    public void UpdateLandmarkCoordinates(List<Vector3> newLandmarkCoordinates) {
        Debug.Log("Update landmarks");
        landmarkCoordinates = newLandmarkCoordinates;
    }

    // TODO FUTURE: "Calibrate" hands with trackers?
    // ... So far coords super small numbers (-1, 1) could get positions of trackers and calibrate to hands landmarks
    // ... Just get numebr to multipli te coords with
}
