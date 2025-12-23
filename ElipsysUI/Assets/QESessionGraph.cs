using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class QESessionGraph : MonoBehaviour
{

    [SerializeField]
    GameObject QEShotGraphPrefab;

    [SerializeField]
    List<Vector2> QEDurations;
    [SerializeField]
    List<bool> makes;

    [SerializeField]
    List<QEShotGraph> shotGraphs;

    [ContextMenu("Update Graphs")]
    public void UpdateGraphs()
    {
        while (shotGraphs.Count > 0)
        {
            var removeGraph = shotGraphs[0];
            shotGraphs.Remove(shotGraphs[0]);
            Destroy(removeGraph.gameObject);
        }
        for (int i = 0; i < QEDurations.Count; i++)
        {
            var newGraph = Instantiate(QEShotGraphPrefab, transform).GetComponent<QEShotGraph>();
            newGraph.SetValues(QEDurations[i], makes[i],i+1);
            shotGraphs.Add(newGraph);
        }

    }

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
