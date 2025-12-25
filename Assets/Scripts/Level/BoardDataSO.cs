using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "GrillSort/Board Data")]

public class BoardDataSO : ScriptableObject
{
    public List<GrillLockData> grills;
}


//public class BoardDataSO : ScriptableObject
//{
//    public int width;
//    public int height;
//    public string boardId;

//    public List<TraySlotData> listTrayData;
//}