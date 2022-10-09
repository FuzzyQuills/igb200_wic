using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class ElecNodeLayer {
    public List<ElecNode> nodes;
}

public class ElecGame : MonoBehaviour
{
    [Header("ElecGame dependencies.")]
    public GameObject RootTransform; // needed as otherwise ElecNodes spawn way out of bounds!
    public GameObject ElecNodePrefab;

    [Header("Electrical Game Parameters")]
    public Vector2 playAreaDimensions;
    public List<ElecNodeLayer> layers;
    public int numberOfLayers;
    public int numberOfNodesPerLayer;

    [Header("Win Condition (Voltages)")]
    public CircuitBreaker finalSwitch; // when active, switches on circuit breaker controlling power to the player's building. 
    public TMP_Text finalVoltageLabel;
    public float startingVoltage;
    public float desiredVoltage;
    public float finalVoltage;
    public float voltageTolerance;

    float ComputeLayerVoltage (ElecNodeLayer layer) {
        float accumulatedNodeVoltages = 0.0f;
        foreach (ElecNode node in layer.nodes) {
            accumulatedNodeVoltages += node.breaker.IsOn ? node.voltageDiff : 0.0f;
        }
        float result = accumulatedNodeVoltages;
        return float.IsNaN(result) ? 0.0f : result;
    }

    // Start is called before the first frame update
    void Start()
    {
        startingVoltage = Random.Range(desiredVoltage / 4, desiredVoltage / 2);
        // Compute voltage stride from desired voltage.
        float voltageStride = desiredVoltage / numberOfNodesPerLayer;

        // Generate voltage tolerance value.
        voltageTolerance = Random.Range(2.0f, 4.0f); // TODO: change to difficulty based values later. 

        // Compute grid offsets for the loops.
        int xstride, ystride;
        xstride = Mathf.RoundToInt(playAreaDimensions.x / (float)(numberOfNodesPerLayer + 1));
        ystride = Mathf.RoundToInt(playAreaDimensions.y / (float)(numberOfLayers + 1));

        // Generate grid. 
        layers = new List<ElecNodeLayer>(numberOfLayers);
        for (int y = 1; y <= numberOfLayers; ++y) {
            ElecNodeLayer newLayer = new ElecNodeLayer();
            newLayer.nodes = new List<ElecNode>(numberOfNodesPerLayer);
            for (int x = 1; x <= numberOfNodesPerLayer; ++x) {
                Vector3 newPosition = new Vector3((xstride * x) - (playAreaDimensions.x / 2), (ystride * y) - (playAreaDimensions.y / 2), 0);
                GameObject temp = Instantiate<GameObject>(ElecNodePrefab, newPosition, ElecNodePrefab.transform.rotation);
                temp.transform.SetParent(RootTransform.transform, false);
                temp.GetComponent<ElecNode>().voltageDiff = Random.Range(-voltageStride, voltageStride);
                newLayer.nodes.Add(temp.GetComponent<ElecNode>());
            }
            layers.Add(newLayer);
        }
    }

    // Update is called once per frame
    void Update()
    {
        // Sum all layers, based on ElecNode states in each layer.
        float accumulatedVoltage = 0.0f;
        foreach (ElecNodeLayer layer in layers) {
            float layerVoltage = ComputeLayerVoltage(layer);
            
            if (layerVoltage != 0.0f) {
                accumulatedVoltage += layerVoltage;
            } else {
                accumulatedVoltage = 0.0f;
                break;
            }
        }

        finalVoltage = accumulatedVoltage != 0.0f && finalSwitch.IsOn ? startingVoltage + accumulatedVoltage : 0.0f;
        finalVoltageLabel.text = string.Format("Voltage: {0:0.00}V\nDesired Voltage: {1:0.00}V\nTolerance: {2:0.00}V", finalVoltage, desiredVoltage, voltageTolerance);
    }

    void OnDrawGizmos () {
        Gizmos.DrawWireCube(RootTransform.transform.position, new Vector3(playAreaDimensions.x * transform.localScale.x, playAreaDimensions.y * transform.localScale.y, 0.001f));
    }
}
