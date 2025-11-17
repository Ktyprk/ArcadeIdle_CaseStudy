using System.Collections.Generic;
using UnityEngine;

public class AgentInventory : MonoBehaviour
{
    [Header("Settings")]
    [SerializeField] private Transform itemParent;
    [SerializeField] private int maxCapacity = 20;
    [SerializeField] private float yStackSpacing = 0.1f;

    private List<GameObject> collectedItems = new();
    private ProductType currentHoldingType = null;

    public bool IsFull => collectedItems.Count >= maxCapacity;
    public int CurrentItemCount => collectedItems.Count;
    public ProductType CurrentHoldingType => currentHoldingType;

    public bool CanHoldMore(ProductType type)
    {
        if (IsFull) return false;
        
        bool typeMatches = (currentHoldingType == null || currentHoldingType == type);
        return typeMatches;
    }
    
    public void AddItemToStack(GameObject item, ProductType productType)
    {
        if (IsFull) return;

        if (currentHoldingType == null)
        {
            currentHoldingType = productType;
        }
        
        item.transform.SetParent(itemParent);
        item.transform.localPosition = new Vector3(0, (float)collectedItems.Count * yStackSpacing, 0);
        item.transform.localRotation = Quaternion.identity;
        collectedItems.Add(item);
    }

    public GameObject RemoveLastItemFromStack()
    {
        if (collectedItems.Count == 0) return null;

        int lastIndex = collectedItems.Count - 1;
        GameObject item = collectedItems[lastIndex];
        collectedItems.RemoveAt(lastIndex);
        
        if (collectedItems.Count == 0)
        {
            currentHoldingType = null;
        }

        return item;
    }
}