using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "GrillSort/Spawn Ware Data")]
public class SpawnWareDataSO : ScriptableObject
{
    //public int totalWare;
    //public int totalWarePattern;

    public int uniqueFoodCount;
    public int totalPattern;

    public List<string> listWareSet;

    public List<LayerSpawnData> listLayerData;

    public int numberIce;
    public int maxNumberIceInTray;
    public int numberTrayNoIce;

    public int numberHidden;
    public int maxNumberHiddenInTray;
}
