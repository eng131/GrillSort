using UnityEngine;

[System.Serializable]
public class LayerSpawnData
{
    public int numberEmptySlot;

    [Range(0, 100)]
    public float match3Ratio;
    [Range(0, 100)]
    public float match2Ratio;
    [Range(0, 100)]
    public float match1Ratio;
}
