using System.Collections.Generic;
using UnityEngine;

public class ConstructionInputArea : MonoBehaviour
{
    [Header("Setup")]
    [SerializeField] private ConstructionSite constructionSite; 
    
    [Header("Settings")]
    [SerializeField] private int maxCapacity = 20;
    
    [Header("Stack Visuals")]
    [SerializeField] private Transform itemParent;
    
    [Header("Grid Settings")]
    [SerializeField] private int itemsPerColumn = 5;
    [SerializeField] private int columnsPerRow = 5;
    [SerializeField] private float yStackSpacing = 0.1f;
    [SerializeField] private float xColumnSpacing = 0.5f;
    [SerializeField] private float zRowSpacing = 0.5f;

    private List<GameObject> stackedItems = new();

    public bool IsFull => stackedItems.Count >= maxCapacity;
    public int CurrentItemCount => stackedItems.Count;

    public bool CanAcceptItem(ProductType productType)
    {
        if (IsFull || constructionSite == null || constructionSite.IsComplete)
        {
            return false;
        }
        
        return constructionSite.NeedsProduct(productType);
    }

    public void AddItemToStack(GameObject item)
    {
        if (IsFull) return;

        item.transform.SetParent(itemParent);
        item.transform.localPosition = CalculateGridPosition(stackedItems.Count);
        item.transform.localRotation = Quaternion.identity;
        item.SetActive(true);
        stackedItems.Add(item);
    }

    public bool HasItemOfType(ProductType type)
    {
        foreach (GameObject item in stackedItems)
        {
            if (item.GetComponent<Product>().Type == type)
            {
                return true;
            }
        }
        return false;
    }

    public GameObject TakeItemOfType(ProductType type)
    {
        for (int i = 0; i < stackedItems.Count; i++)
        {
            Product product = stackedItems[i].GetComponent<Product>();
            if (product != null && product.Type == type)
            {
                GameObject itemToTake = stackedItems[i];
                stackedItems.RemoveAt(i);
                RebuildStackVisuals();
                return itemToTake;
            }
        }
        return null;
    }
    
    private void RebuildStackVisuals()
    {
        for (int i = 0; i < stackedItems.Count; i++)
        {
            stackedItems[i].transform.localPosition = CalculateGridPosition(i);
        }
    }

    private Vector3 CalculateGridPosition(int count)
    {
        int yIndex = count % itemsPerColumn;
        float yPos = yIndex * yStackSpacing;

        int flatColumnIndex = count / itemsPerColumn;
        
        int xIndex = flatColumnIndex % columnsPerRow;
        float xPos = xIndex * xColumnSpacing;

        int zIndex = flatColumnIndex / columnsPerRow;
        float zPos = zIndex * zRowSpacing;

        return new Vector3(xPos, yPos, zPos);
    }
}