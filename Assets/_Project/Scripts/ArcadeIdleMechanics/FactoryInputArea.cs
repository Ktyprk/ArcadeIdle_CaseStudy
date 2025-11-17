using System.Collections.Generic;
using UnityEngine;

public class FactoryInputArea : MonoBehaviour
{
    [Header("Settings")]
    [SerializeField] private ProductType requiredInputType;
    [SerializeField] private int maxCapacity = 20;
    
    [Header("Stack Visuals")]
    [SerializeField] private Transform itemParent;
    [SerializeField] private float yStackSpacing = 0.1f;
    
    [Header("Grid Settings")]
    [SerializeField] private int itemsPerColumn = 5;
    [SerializeField] private int columnsPerRow = 5;
    [SerializeField] private float xColumnSpacing = 0.5f;
    [SerializeField] private float zRowSpacing = 0.5f;

    private List<GameObject> stackedItems = new();

    public ProductType RequiredInputType => requiredInputType;
    public bool IsFull => stackedItems.Count >= maxCapacity;
    public int CurrentItemCount => stackedItems.Count;

    public bool CanAcceptItem(ProductType productType)
    {
        return productType == requiredInputType && !IsFull;
    }

    public void AddItemToStack(GameObject item)
    {
        if (IsFull) return;
        
        Product product = item.GetComponent<Product>();
        if (product == null || product.Type != requiredInputType)
        {
            return;
        }

        item.transform.SetParent(itemParent);
        item.transform.localPosition = CalculateGridPosition(stackedItems.Count);
        item.transform.localRotation = Quaternion.identity;
        item.SetActive(true);
        stackedItems.Add(item);
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

    public GameObject TakeLastItem()
    {
        if (stackedItems.Count == 0) return null;

        int lastIndex = stackedItems.Count - 1;
        GameObject item = stackedItems[lastIndex];
        stackedItems.RemoveAt(lastIndex);
        return item;
    }
}