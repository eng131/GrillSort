using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;

public class TrayItem : MonoBehaviour
{
    private List<Image> foodList ;
    public List<Image> FoodList => foodList;

    private void Awake()
    {
        foodList = Utils.GetListInChild<Image>(this.transform);
        for (int i = 0; i < foodList.Count; i++)
        {
            foodList[i].gameObject.SetActive(false);
        }
    }

    public void OnSetFood(List<Sprite> items)
    {
        //if(items.Count < foodList.Count)
        //{
        //    for(int i = 0; i < items.Count; i++)
        //    {
        //        Image slot = this.RandomSlot();
        //        slot.gameObject.SetActive(true);
        //        slot.sprite = items[i];
        //        slot.SetNativeSize();
        //    }
        //}

        int count = Mathf.Min(items.Count, foodList.Count);

        for (int i = 0; i < count; i++)
        {
            Image slot = RandomSlot();
            if (slot == null) return;

            slot.gameObject.SetActive(true);
            slot.sprite = items[i];
            slot.SetNativeSize();
        }
    }

    public bool IsEmpty()
    {
        for (int i = 0; i < foodList.Count; i++)
        {
            if (foodList[i].gameObject.activeInHierarchy)
                return false;
        }
        return true;
    }


    private Image RandomSlot()
    {
        List<Image> freeSlots = foodList.FindAll(x => !x.gameObject.activeInHierarchy);
        if (freeSlots.Count == 0) return null;

        return freeSlots[Random.Range(0, freeSlots.Count)];
    }
}
