using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;

public class DropDragCtrl : MonoBehaviour
{
    [SerializeField] private Image imgFoodDrag;
    private FoodSlot currentFood;
    private FoodSlot cacheFood;
    private bool hasDrag;
    private Vector3 offset;


    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetMouseButtonDown(0))
        {
            currentFood = Utils.GetRayCastUI<FoodSlot>(Input.mousePosition);

            if(currentFood != null && currentFood.HasFood)
            {
                hasDrag = true;
                cacheFood = currentFood;
                imgFoodDrag.gameObject.SetActive(true);
                imgFoodDrag.sprite = currentFood.GetSpriteFood;
                imgFoodDrag.SetNativeSize();
                imgFoodDrag.transform.position = currentFood.transform.position;

                // offset
                Vector3 mouseWordPos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
                offset = mouseWordPos - imgFoodDrag.transform.position ;
                
                currentFood.onActiveFood(false);
            }
        }

        if (hasDrag)
        {
            Vector3 mouseWordPos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
            Vector3 foodPos = mouseWordPos + offset;
            foodPos.z = 0;
            imgFoodDrag.transform.position = foodPos;

            FoodSlot slot = Utils.GetRayCastUI<FoodSlot>(Input.mousePosition);
            if (slot != null)
            {
                if (!slot.HasFood)
                {
                    if(cacheFood == null || cacheFood.GetInstanceID() != slot.GetInstanceID() )
                    {
                        cacheFood?.OnHideFood();
                        cacheFood = slot;
                        cacheFood.OnFadeFood();
                        cacheFood.OnSetSlot(currentFood.GetSpriteFood);
                    }
                }
                else
                {
                    FoodSlot slotAvailable = slot.GetSlotNull;
                    if(slotAvailable != null )
                    {
                        cacheFood?.OnHideFood();
                        cacheFood = slotAvailable;
                        cacheFood.OnFadeFood();
                        cacheFood.OnSetSlot(currentFood.GetSpriteFood);
                    }
                    else 
                    {
                        this.OnClearCacheSlot();
                    }

                }
                
            }
            else
            {
                if (cacheFood != null)
                {
                    cacheFood.OnHideFood();
                    cacheFood = null;
                }
            }

        }


        if (Input.GetMouseButtonUp(0) && hasDrag)
        {
            if(cacheFood != null ) // fill item
            {
                imgFoodDrag.transform.DOMove(cacheFood.transform.position, 0.2f).OnComplete( ()=>
                {
                    imgFoodDrag.gameObject.SetActive(false);
                    cacheFood.OnSetSlot(currentFood.GetSpriteFood);
                    cacheFood.onActiveFood(true);
                    cacheFood.OnCheckMerge();
                    cacheFood = null;
                    currentFood = null;
                });
            }
            else
            {
                imgFoodDrag.transform.DOMove(currentFood.transform.position, 0.3f).OnComplete(() =>
                {
                    imgFoodDrag.gameObject.SetActive(false);
                    currentFood.onActiveFood(true);
                });
            }
            hasDrag = false;

        }
    }

    private void OnClearCacheSlot()
    {
        if (cacheFood != null && cacheFood.GetInstanceID() != currentFood.GetInstanceID() )
        {
            cacheFood.OnHideFood();
            cacheFood = null;
        }
    }
}
