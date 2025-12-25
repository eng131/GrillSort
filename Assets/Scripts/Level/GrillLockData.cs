using UnityEngine;
public enum GrillLockType
{
    None,           // bếp thường
    MergeCount,     // merge X lần thì mở
    ManualUnlock    // người chơi bấm unlock
}

[System.Serializable]
public class GrillLockData
{
    public GrillLockType lockType;

    // MergeCount
    public int needMergeCount;

    // ManualUnlock 
    public bool startLocked;
}
