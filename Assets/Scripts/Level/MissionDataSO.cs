using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "GrillSort/Mission Data")]
public class MissionDataSO : ScriptableObject
{
    public int start;
    public int seconds;

    public List<MissionRequiredData> listRequiredData;

    public bool hasMixRequired;
    public bool hasSpicyRequired;
}
