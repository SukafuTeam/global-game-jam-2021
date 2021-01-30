using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PentagramController : MonoBehaviour
{
    public  Transform StartPoint;
    public Transform Endpoint; 
    
    public int NDivisions;

    public List<Vector3> BeatPositions;
    
    public List<GameObject> BeatIndicators;

    public GameObject IndicatorPrefab;

    public Transform IndicatorsParent;

    public Vector3 DefaultIndicatorScale;

    public Transform ArrowsParent;
    
    // Start is called before the first frame update
    void Start()
    {
        var dir = Endpoint.position - StartPoint.position;

        var distance = dir / (NDivisions + 2);

        BeatPositions = new List<Vector3>();
        BeatIndicators = new List<GameObject>();
        
        for (var i = 0; i < (NDivisions + 2); i++)
        {
            // We skip the first two half steps
            if(i == 0 || i == 1)
                continue;

            BeatPositions.Add(StartPoint.position + distance * i);
            BeatIndicators.Add(Instantiate(IndicatorPrefab, BeatPositions[i-2], Quaternion.identity, IndicatorsParent));
            BeatIndicators[i-2].SetActive(false);
        }
    }

    public void ResetIndicators()
    {
        for (var i = 0; i < NDivisions; i++)
        {
            BeatIndicators[i].transform.localScale = DefaultIndicatorScale;
            BeatIndicators[i].SetActive(false);
        }

        if (ArrowsParent == null)
            return;

        foreach (Transform child in ArrowsParent) {
            Destroy(child.gameObject);
        }
    }
}
