using UnityEngine;
using System.Collections.Generic;

public class GrillStation : MonoBehaviour
{
    [SerializeField] private Transform trayContainer;
    [SerializeField] private Transform slotContainer;

    private List<TrayItem> totalTrays;
    private List<FoodSlot> totalSlots;


    private void Awake()
    {
        totalTrays = Utils.GetListInChild<TrayItem>(trayContainer);
        totalSlots = Utils.GetListInChild<FoodSlot>(slotContainer);
    }

    public void OnInitGrill(int totalTray, List<Sprite> listFood)
    {
        int foodCount = Random.Range(1, totalSlots.Count + 1);
        ///////////
        List<Sprite> list = new List<Sprite>(listFood);
        List<Sprite> listSlot = Utils.TakeAndRemoveRandom(list, foodCount);

        for (int i = 0; i < listSlot.Count; i++)
        {
            FoodSlot slot = this.RandomSlot();
            slot.OnSetSlot(listSlot[i]);
        }

        List<List<Sprite>> remainFood = new List<List<Sprite>>();

        for (int i = 0; i < totalTray - 1; i++)
        {
            remainFood.Add(new List<Sprite>());
            int n = Random.Range(0, listFood.Count);
            remainFood[i].Add(listFood[n]);
            listFood.RemoveAt(n);
        }

        while (listFood.Count > 0)
        {
            int rans = Random.Range(0, remainFood.Count);
            if (remainFood[rans].Count < 4)
            {
                int n = Random.Range(0, listFood.Count);
                remainFood[rans].Add(listFood[n]);
                listFood.RemoveAt(n);
            }
        }


        for (int i = 0; i < totalTrays.Count; i++)
        {
            bool active = i < remainFood.Count;
            totalTrays[i].gameObject.SetActive(active);
            if(active )
            {
                totalTrays[i].OnSetFood(remainFood[i]);
            }
        }
    }

    private FoodSlot RandomSlot()
    {
        var free = totalSlots.FindAll(x => !x.HasFood);
        if (free.Count == 0) return null;
        return free[Random.Range(0, free.Count)];
    }

    public FoodSlot GetSlotNull()
    {
        for(int i = 0; i< totalSlots.Count; i++)
        {
            if (!totalSlots[i].HasFood) return totalSlots[i];
        }
        return null;
    }

    public void OnCheckMerge()
    {
        if(this.GetSlotNull() == null)
        {
            Debug.Log("merge");
            if (this.CanMerge())
            {
                for(int i = 0 ; i< totalSlots.Count; i++)
                {
                    totalSlots[i].onActiveFood(false);
                }
            }                            
        }
    }

    private bool CanMerge()
    {
        string name = totalSlots[0].GetSpriteFood.name;

        for (int i = 0 ; i<totalSlots.Count; i++)
        {
            if(totalSlots[i].GetSpriteFood.name != name)
            {
                return false;
            }
        }

        return true;
    }

}
