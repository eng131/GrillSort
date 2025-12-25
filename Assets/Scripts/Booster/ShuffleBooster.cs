using UnityEngine;
using System.Collections.Generic;

public class ShuffleBooster : MonoBehaviour, IBooster
{
    public bool CanUse() => true;

    public void Execute()
    {
        var grills = GameManager.Instance.GetAllActiveGrills();

        // 1️⃣ Thu thập tất cả slot chứa food (grill + tray)
        List<System.Action<Sprite>> setActions = new();
        List<Sprite> allFoods = new();

        foreach (var grill in grills)
        {
            // 🔥 SLOT TRÊN GRILL
            foreach (var slot in grill.TotalSlots)
            {
                if (!slot.HasFood) continue;

                Sprite sp = slot.GetSpriteFood;
                allFoods.Add(sp);

                // lưu action để set lại đúng chỗ
                setActions.Add(sprite =>
                {
                    slot.OnSetSlot(sprite);
                });

                slot.OnHideFood();
            }

            // 🔥 SLOT TRONG TRAY
            foreach (var tray in grill.GetActiveTrays())
            {
                foreach (var img in tray.FoodList)
                {
                    if (!img.gameObject.activeInHierarchy) continue;

                    Sprite sp = img.sprite;
                    allFoods.Add(sp);

                    setActions.Add(sprite =>
                    {
                        img.sprite = sprite;
                        img.gameObject.SetActive(true);
                        img.SetNativeSize();
                    });

                    img.gameObject.SetActive(false);
                }
            }
        }

        // 2️⃣ Shuffle sprite
        for (int i = 0; i < allFoods.Count; i++)
        {
            int r = Random.Range(i, allFoods.Count);
            (allFoods[i], allFoods[r]) = (allFoods[r], allFoods[i]);
        }

        // 3️⃣ Gán lại sprite đúng số lượng ban đầu
        for (int i = 0; i < setActions.Count; i++)
        {
            setActions[i].Invoke(allFoods[i]);
        }
    }
}
