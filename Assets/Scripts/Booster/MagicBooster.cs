using UnityEngine;


public class MagicBooster : MonoBehaviour, IBooster
{
    public bool CanUse() => true;

    public void Execute()
    {
        var grills = GameManager.Instance.GetAllActiveGrills();

        foreach (var grill in grills)
        {
            if (grill.CanMergeForBooster())
            {
                grill.ForceMerge();
                return;
            }
        }

        Debug.Log("? Magic: Không có grill h?p l?");
    }
}
