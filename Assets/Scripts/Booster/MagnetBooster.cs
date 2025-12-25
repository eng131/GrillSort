using System.Collections.Generic;
using UnityEngine;

public class MagnetBooster : MonoBehaviour, IBooster
{
    public bool CanUse()
    {
        return true; // sau này check s? l??ng booster
    }

    public void Execute()
    {
        var grills = GameManager.Instance.GetAllActiveGrills();

        Dictionary<Sprite, List<FoodSlot>> map = new();

        foreach (var grill in grills)
        {
            foreach (var slot in grill.TotalSlots)
            {
                if (!slot.HasFood) continue;

                var sp = slot.GetSpriteFood;
                if (!map.ContainsKey(sp))
                    map[sp] = new List<FoodSlot>();

                map[sp].Add(slot);
            }
        }

        foreach (var pair in map)
        {
            if (pair.Value.Count >= 3)
            {
                GameManager.Instance.OnMergeReward(pair.Value);
                return;
            }
        }

        Debug.Log("? Magnet: Không có b? 3 nào");
    }
}
