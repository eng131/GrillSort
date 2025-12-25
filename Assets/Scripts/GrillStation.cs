using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class GrillStation : MonoBehaviour
{
    [SerializeField] private Transform trayContainer;
    [SerializeField] private Transform slotContainer;

    private List<TrayItem> totalTrays;
    private List<FoodSlot> totalSlots;

    private Stack<TrayItem> stackTrays = new Stack<TrayItem>();

    public List<FoodSlot> TotalSlots => totalSlots;

    private void Awake()
    {
        totalTrays = Utils.GetListInChild<TrayItem>(trayContainer);
        totalSlots = Utils.GetListInChild<FoodSlot>(slotContainer);
    }

    public void OnInitGrillFromLevel( List<Sprite> foodsOnGrill , List<Sprite> foodsInTray , int trayCount)
    {
        // clear grill slots trước (để tránh dính level cũ)
        for (int i = 0; i < totalSlots.Count; i++)
            totalSlots[i].OnHideFood();

        // 1) Set food trên grill
        int fillCount = Mathf.Min(totalSlots.Count, foodsOnGrill.Count);
        for (int i = 0; i < fillCount; i++)
            totalSlots[i].OnSetSlot(foodsOnGrill[i]);

        // 2) Chia food vào từng tray
        stackTrays.Clear();

        
        List<List<Sprite>> trayFoods = new List<List<Sprite>>();
        for (int i = 0; i < trayCount; i++)
            trayFoods.Add(new List<Sprite>());

        int idx = 0;
        while (idx < foodsInTray.Count && trayFoods.Count > 0)
        {
            trayFoods[idx % trayFoods.Count].Add(foodsInTray[idx]);
            idx++;
        }

        //  Active tray nếu có food
        for (int i = 0; i < totalTrays.Count; i++)
        {
            bool active = i < trayFoods.Count && trayFoods[i].Count > 0;
            totalTrays[i].gameObject.SetActive(active);

            if (active)
            {
                totalTrays[i].OnSetFood(trayFoods[i]);
                stackTrays.Push(totalTrays[i]);
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
        FoodSlot tmp = null;

        for (int i = 0; i < totalSlots.Count; i++)
        {
            if (!totalSlots[i].HasFood)
            {
                if (tmp == null)
                {
                    tmp = totalSlots[i];
                }
                else
                {
                    Vector2 mousePos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
                    float x1 = Mathf.Abs(mousePos.x - tmp.transform.position.x);
                    float x2 = Mathf.Abs(mousePos.x - totalSlots[i].transform.position.x);

                    if (x2 < x1)
                        tmp = totalSlots[i];
                }
            }
        }

        return tmp;
    }

    private bool HasGrillEmpty()
    {
        for (int i = 0; i < totalSlots.Count; i++)
        {
            if (totalSlots[i].HasFood)
                return false;
        }

        return true;
    }

    //public void OnCheckMerge()
    //{
    //    if(this.GetSlotNull() == null)
    //    {
            
    //        if (this.CanMerge())
    //        {  
    //            for(int i = 0 ; i < totalSlots.Count; i++)
    //            {
    //                totalSlots[i].OnActiveFood(false);
    //            }

    //            this.OnPrepareTray();
    //            GameManager.Instance?.OnMinusFood();
    //        }                            
    //    }
    //}

    public void OnCheckMerge()
    {
        if (this.GetSlotNull() == null)
        {
            if (this.CanMerge())
            {
                Debug.Log("Complete Grill");

                //StartCoroutine(IEMerge());

                GameManager.Instance?.OnMergeReward(totalSlots);

                this.OnPrepareTray(false);
                GameManager.Instance?.OnMinusFood();

            }
        }

        IEnumerator IEMerge()
        {
            for (int i = 0; i < totalSlots.Count; i++)
            {
                totalSlots[i].OnFadeOut();
                yield return new WaitForSeconds(0.1f);
            }
        }
    }

    private void OnPrepareTray(bool isNow)
    {
        StartCoroutine(IEPrepare());

        IEnumerator IEPrepare()
        {
            if (!isNow)
                yield return new WaitForSeconds(0.95f);

            if (stackTrays.Count > 0)
            {
                Debug.Log("Prepare Tray");

                TrayItem item = stackTrays.Pop();

                for (int i = 0; i < item.FoodList.Count; i++)
                {
                    Image img = item.FoodList[i];
                    if (img.gameObject.activeInHierarchy)
                    {
                        totalSlots[i].OnPrepareItem(img);
                        img.gameObject.SetActive(false);
                        yield return new WaitForSeconds(0.1f);
                    }
                }


                if (item.IsEmpty())
                {
                    CanvasGroup canvas = item.GetComponent<CanvasGroup>();
                    if (canvas == null)
                        canvas = item.gameObject.AddComponent<CanvasGroup>();

                    canvas.DOFade(0f, 0.35f).OnComplete(() =>
                    {
                        item.gameObject.SetActive(false);
                        canvas.alpha = 1f;
                    });
                }
            }
        }
    }

    public void OnCheckPrepareTray()
    {
        if (this.HasGrillEmpty())
        {
            this.OnPrepareTray(true);
        }
    }

    public bool CanMergeForBooster()
    {
        if (GetSlotNull() != null) return false;
        return CanMerge();
    }

    public void ForceMerge()
    {
        GameManager.Instance.OnMergeReward(TotalSlots);
        OnPrepareTray(false);
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

    public List<TrayItem> GetActiveTrays()
    {
        List<TrayItem> result = new();

        foreach (var tray in totalTrays)
        {
            if (tray.gameObject.activeInHierarchy)
                result.Add(tray);
        }

        return result;
    }

}


//public void OnInitGrill(int totalTray, List<Sprite> listFood)
//{
//    int foodCount = Random.Range(1, totalSlots.Count + 1);
//    ///////////
//    List<Sprite> list = new List<Sprite>(listFood);
//    List<Sprite> listSlot = Utils.TakeAndRemoveRandom(list, foodCount);

//    for (int i = 0; i < listSlot.Count; i++)
//    {
//        FoodSlot slot = this.RandomSlot();
//        slot.OnSetSlot(listSlot[i]);
//    }

//    List<List<Sprite>> remainFood = new List<List<Sprite>>();

//    for (int i = 0; i < totalTray - 1; i++)
//    {
//        if (listFood.Count > 0)
//        {
//            remainFood.Add(new List<Sprite>());
//            remainFood[i].Add(listFood[0]);
//            listFood.RemoveAt(0);
//        }
//    }
//    // 111111
//    //while (listFood.Count > 0)
//    //{
//    //    int rans = Random.Range(0, remainFood.Count);
//    //    if (remainFood[rans].Count < 4)
//    //    {
//    //        remainFood[rans].Add(listFood[0]);
//    //        listFood.RemoveAt(0);
//    //    }
//    //}

//    // 22222
//    //for (int i = 0; i < remainFood.Count && listFood.Count > 0; i++)
//    //{
//    //    while (remainFood[i].Count < 4 && listFood.Count > 0)
//    //    {
//    //        remainFood[i].Add(listFood[0]);
//    //        listFood.RemoveAt(0);
//    //    }
//    //}


//    int safe = 0;
//    int maxLoop = 1000;

//    while (listFood.Count > 0 && safe < maxLoop)
//    {
//        int rans = Random.Range(0, remainFood.Count);

//        if (remainFood[rans].Count < 4)
//        {
//            remainFood[rans].Add(listFood[0]);
//            listFood.RemoveAt(0);
//        }

//        safe++;
//    }

//    if (safe >= maxLoop)
//    {
//        Debug.LogError("OnInitGrill bị kẹt vòng lặp!");
//    }

//    for (int i = 0; i < totalTrays.Count; i++)
//    {
//        bool active = i < remainFood.Count;
//        totalTrays[i].gameObject.SetActive(active);
//        if(active )
//        {
//            totalTrays[i].OnSetFood(remainFood[i]);
//            TrayItem item = totalTrays[i];
//            stackTrays.Push(item);
//        }
//    }
//}
