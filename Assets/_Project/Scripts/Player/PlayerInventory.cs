using System.Collections.Generic;
using UnityEngine;

public class PlayerInventory : MonoBehaviour
{
    [Header("Stack Visuals")] 
    [SerializeField] private Transform itemParent;
    [SerializeField] private int maxCapacity = 20;

    private readonly List<Product> collectedItems = new();

    public bool IsFull => collectedItems.Count >= maxCapacity;

    private void OnEnable()
    {
        PlayerTriggerManager.onCollectAreaStay += TryCollectFromSpawner;
        PlayerTriggerManager.onDropAreaStay += TryDropToInputArea;
        PlayerTriggerManager.onConstructionAreaStay += TryDropToConstructionArea;
        PlayerTriggerManager.onTrashAreaStay += TryTrashItem;
    }

    private void OnDisable()
    {
        PlayerTriggerManager.onCollectAreaStay -= TryCollectFromSpawner;
        PlayerTriggerManager.onDropAreaStay -= TryDropToInputArea;
        PlayerTriggerManager.onConstructionAreaStay -= TryDropToConstructionArea;
        PlayerTriggerManager.onTrashAreaStay -= TryTrashItem;
    }

    private void TryCollectFromSpawner(ProductSpawner spawner)
    {
        if (IsFull || spawner.CurrentProductCount == 0) return;

        GameObject item = spawner.TakeLastProduct();
        if (item == null) return;

        Product product = item.GetComponent<Product>();
        if (product == null)
        {
            Destroy(item);
            return;
        }

        AddItemToStack(product);
    }

    private void AddItemToStack(Product product)
    {
        product.transform.SetParent(itemParent);
        
        Vector3 newPosition = Vector3.zero;
        if (collectedItems.Count > 0)
        {
            Transform previousItemTransform = collectedItems[collectedItems.Count - 1].transform;
            newPosition = previousItemTransform.localPosition + product.Type.StackOffset;
        }

        product.transform.localPosition = newPosition;
        product.transform.localRotation = Quaternion.Euler(product.Type.StackRotation);

        collectedItems.Add(product);
    }

    private void TryDropToInputArea(FactoryInputArea inputArea)
    {
        if (collectedItems.Count == 0) return;

        ProductType requiredType = inputArea.RequiredInputType;
        bool canDrop = inputArea.CanAcceptItem(requiredType);
        if (!canDrop) return;

        Product itemToDrop = RemoveItemOfType(requiredType);
        if (itemToDrop == null) return;

        inputArea.AddItemToStack(itemToDrop.gameObject);
        RebuildStackVisuals();
    }

    private void TryDropToConstructionArea(ConstructionSite constructionSite)
    {
        if (collectedItems.Count == 0) return;

        ProductType typeToDrop = FindFirstDroppableType(constructionSite);
        if (typeToDrop == null) return;

        Product itemToDrop = RemoveItemOfType(typeToDrop);
        if (itemToDrop == null) return;

        constructionSite.ReceiveItem(itemToDrop.gameObject);
        RebuildStackVisuals();
    }

    private void TryTrashItem(TrashArea trashArea)
    {
        if (collectedItems.Count == 0) return;

        int lastIndex = collectedItems.Count - 1;
        Product itemToTrash = collectedItems[lastIndex];

        collectedItems.RemoveAt(lastIndex);

        trashArea.TrashItem(itemToTrash.gameObject);
    }

    private ProductType FindFirstDroppableType(ConstructionSite constructionSite)
    {
        ProductType nextNeeded = constructionSite.GetNextRequiredType();
        if (nextNeeded == null) return null;

        for (int i = collectedItems.Count - 1; i >= 0; i--)
        {
            if (collectedItems[i].Type == nextNeeded)
            {
                return nextNeeded;
            }
        }
        return null;
    }

    private Product RemoveItemOfType(ProductType type)
    {
        for (int i = collectedItems.Count - 1; i >= 0; i--)
        {
            if (collectedItems[i].Type == type)
            {
                Product itemToTake = collectedItems[i];
                collectedItems.RemoveAt(i);
                return itemToTake;
            }
        }
        return null;
    }

    private void RebuildStackVisuals()
    {
        Vector3 currentPosition = Vector3.zero;
        for (int i = 0; i < collectedItems.Count; i++)
        {
            Product product = collectedItems[i];
            product.transform.localPosition = currentPosition;
            product.transform.localRotation = Quaternion.Euler(product.Type.StackRotation);
            
            if(i < collectedItems.Count - 1)
            {
                currentPosition += collectedItems[i + 1].Type.StackOffset;
            }
        }
    }
}