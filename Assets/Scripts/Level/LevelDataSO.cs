using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "GrillSort/Level Data")]
public class LevelDataSO : ScriptableObject
{
    public int difficulty;
    public int levelSeconds;
    public int levelIndex;

    public BoardDataSO boardData;
    public SpawnWareDataSO spawnWareData;
    public List<MissionDataSO> missions;
}
