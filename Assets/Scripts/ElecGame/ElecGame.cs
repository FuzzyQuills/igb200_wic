using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class ElecNodeLayer {
    public List<ElecNode> nodes;
}

public class ElecGame : MonoBehaviour
{
    [Header("ElecGame dependencies.")]
    private TileInfoCollector c_codeman;
    private GameData c_gd;

    public GameObject RootTransform; // needed as otherwise ElecNodes spawn way out of bounds!
    public GameObject ElecNodePrefab;

    [Header("Electrical Game Parameters")]
    public float playTime;
    private float currentTime;
    public int stars = 5;
    public Vector2 playAreaDimensions;
    public List<ElecNodeLayer> layers;
    public int numberOfLayers;
    public int numberOfNodesPerLayer;

    [Header("Win Condition (Voltages)")]
    bool gameWon = false;
    public CircuitBreaker finalSwitch; // when active, switches on circuit breaker controlling power to the player's building. 
    public TMP_Text finalVoltageLabel;
    public TMP_Text startingVoltageLabel;
    public TMP_Text starText;
    public Slider timeSlider;
    public float startingVoltage;
    public float desiredVoltage;
    public float finalVoltage;
    public float voltageTolerance;
    private bool voltageUpOrDown;

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
        c_gd = FindObjectOfType<GameData>();
        c_codeman = FindObjectOfType<TileInfoCollector>();
        if (c_gd) {
            c_gd.expenditure = 0;
        }
        if (c_codeman) {
            playTime -= c_codeman.currentLevel;
        }

        desiredVoltage = Random.Range(5.0f, 12.0f);
        startingVoltage = Random.Range(desiredVoltage / 4, desiredVoltage / 2);
        startingVoltageLabel.text = string.Format("Starting Voltage: {0:0.00}", startingVoltage);

        // Compute voltage stride from desired voltage.
        float voltageStride = desiredVoltage / startingVoltage;

        // Generate voltage tolerance value.
        if (c_codeman.currentLevel > 0) {
            voltageTolerance = 4.0f; // TODO: change to difficulty based values later. 
        } else {
            voltageTolerance = 4.0f / c_codeman.currentLevel; // TODO: change to difficulty based values later. 
        }

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
                temp.GetComponent<ElecNode>().voltageDiff = Mathf.Round(Random.Range(0.0f, voltageStride));
                voltageUpOrDown = !voltageUpOrDown;
                newLayer.nodes.Add(temp.GetComponent<ElecNode>());
            }
            layers.Add(newLayer);
        }
        currentTime = playTime;
    }

    // Update is called once per frame
    void Update()
    {
        if (!gameWon) {
            currentTime -= Time.deltaTime;
            timeSlider.value = currentTime / playTime;
            Debug.Log(string.Format("{0}", currentTime / playTime));
            if (currentTime <= 0)
            {
                stars--;
                starText.text = $"{stars} stars";
                currentTime = playTime;
            }
        }
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
        
        if (finalSwitch.IsOn) {
            finalVoltage = accumulatedVoltage > 0.0f ? startingVoltage + accumulatedVoltage : 0.0f;
            if (finalVoltage > desiredVoltage + voltageTolerance) {
                Debug.Log("Blowout!!!");
                finalVoltageLabel.text = string.Format("Voltage: OVERLOAD\nDesired Voltage: {1:0.00}V\nTolerance: {2:0.00}V", finalVoltage, desiredVoltage, voltageTolerance);
                finalSwitch.IsOn = false;
            } else {
                finalVoltageLabel.text = string.Format("Voltage: {0:0.00}V\nDesired Voltage: {1:0.00}V\nTolerance: {2:0.00}V", finalVoltage, desiredVoltage, voltageTolerance);
            }

            // test for win condition.
            if (finalVoltage > desiredVoltage - voltageTolerance && finalVoltage < desiredVoltage + voltageTolerance) {
                gameWon = true;
                float voltageReward = 1.0f / (desiredVoltage - finalVoltage);
                if (voltageReward < 0) voltageReward = -voltageReward;
                voltageReward *= 2.0f;
                // Every 6 seconds, a star is lost, so it's safe to assume the reward would be 
                // stars * 6 multiplied by a constant. 
                int prizeMoney = Mathf.RoundToInt((stars * (playTime / 5.0f)) * voltageReward * 20.0f); 
                finalVoltageLabel.text = string.Format("SUCCESS!\nFinal Voltage: {0:0.00}V\n{1}K awarded!", 
                                                        finalVoltage, prizeMoney);
                finalSwitch.disableBreaker = true;
                if (c_gd) {
                    c_gd.expenditure += prizeMoney;
                }
                foreach (ElecNodeLayer layer in layers) {
                    foreach (ElecNode node in layer.nodes) {
                        node.breaker.disableBreaker = true;
                    }
                }
            }
        } else {
            finalVoltageLabel.text = string.Format("Voltage: {0:0.00}V\nDesired Voltage: {1:0.00}V\nTolerance: {2:0.00}V", finalVoltage, desiredVoltage, voltageTolerance);
        }
    }

    void OnDrawGizmos () {
        Gizmos.DrawWireCube(RootTransform.transform.position, new Vector3(playAreaDimensions.x * transform.localScale.x, playAreaDimensions.y * transform.localScale.y, 0.001f));
    }
}
