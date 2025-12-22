using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;

public class DropDragCtrl : MonoBehaviour
{
    [SerializeField] private Image imgFoodDrag;
    [SerializeField] private float timeCheckSuggest;
    private FoodSlot currentFood;
    private FoodSlot cacheFood;
    private bool hasDrag;
    private Vector3 offset;
    private float countTime;
    private float timeAtClick;


    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        countTime += Time.deltaTime;
        if(countTime >= timeCheckSuggest)
        {
            countTime = 0;
            GameManager.Instance?.OnCheckAndShake();
        }

        if (Input.GetMouseButtonDown(0))
        {
            countTime = 0;
            FoodSlot tapSlot = Utils.GetRayCastUI<FoodSlot>(Input.mousePosition);

            if (tapSlot != null)
            {
                if (tapSlot.HasFood)
                {
                    hasDrag = true;
                    currentFood?.OnActiveFood(true);
                    cacheFood = currentFood = tapSlot;
                    // sprite food cho dummy 
                    imgFoodDrag.gameObject.SetActive(true);
                    imgFoodDrag.sprite = currentFood.GetSpriteFood;
                    imgFoodDrag.SetNativeSize();
                    imgFoodDrag.transform.position = currentFood.transform.position;            

                    //tinh offset
                    Vector3 mouseWordPos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
                    offset = currentFood.transform.position - mouseWordPos;
                    offset.z = 0f;

                    currentFood.OnActiveFood(false);
                    imgFoodDrag.transform.DOScale(Vector3.one * 1.2f, 0.2f);
                }
                else
                {
                    if (currentFood != null) // di chuyen item vua slect toi vi tri nay
                    {
                        imgFoodDrag.transform.DOMove(tapSlot.transform.position, 0.4f).OnComplete(() => {
                            tapSlot.OnSetSlot(currentFood.GetSpriteFood);
                            tapSlot.OnActiveFood(true);
                            tapSlot.OnCheckMerge();
                            currentFood?.OnCheckPrepareTray();
                            currentFood = null;
                            imgFoodDrag.gameObject.SetActive(false);
                        });
                        imgFoodDrag.transform.DOScale(Vector3.one, 0.4f);
                    }
                }
            }
            else
            {
                currentFood?.OnActiveFood(true);
                currentFood = null;
                imgFoodDrag.gameObject.SetActive(false);
            }

            timeAtClick = Time.time;
        }
    

        if (hasDrag)
        {
            
            Vector3 mouseWordPos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
            Vector3 foodPos = mouseWordPos + offset;
            foodPos.z = 0;
            imgFoodDrag.transform.position = foodPos;
            countTime = 0;

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
            if (Time.time - timeAtClick < 0.15f) //click
            {

            }
            else
            {
                if (cacheFood != null) //fill item
                {
                    imgFoodDrag.transform.DOMove(cacheFood.transform.position, 0.2f).OnComplete(() =>
                    {
                        imgFoodDrag.gameObject.SetActive(false);
                        cacheFood.OnSetSlot(currentFood.GetSpriteFood);
                        cacheFood.OnActiveFood(true);
                        cacheFood.OnCheckMerge();
                        currentFood?.OnCheckPrepareTray();
                        cacheFood = null;
                        currentFood = null;
                    });

                    imgFoodDrag.transform.DOScale(Vector3.one, 0.22f);
                }
                else //tro ve vi tri ban dau
                {
                    imgFoodDrag.transform.DOMove(currentFood.transform.position, 0.3f).OnComplete(() =>
                    {
                        imgFoodDrag.gameObject.SetActive(false);
                        currentFood.OnActiveFood(true);
                    });

                    imgFoodDrag.transform.DOScale(Vector3.one, 0.3f);
                }
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
