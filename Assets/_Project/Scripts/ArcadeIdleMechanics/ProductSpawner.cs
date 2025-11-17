using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

public class ProductSpawner : MonoBehaviour
{
    [Header("Product")]
    [SerializeField] private GameObject productPrefab;
    [SerializeField] private ProductType productType;

    [Header("Settings")]
    [SerializeField] private int maxCapacity = 20;
    [SerializeField] private float spawnInterval = 1.0f;
    [SerializeField] private bool autoSpawn = true;

    [Header("Stack Visuals")]
    [SerializeField] private Transform itemParent;
    [SerializeField] private float yStackSpacing = 0.1f;
    
    [Header("Grid Settings")]
    [SerializeField] private int itemsPerColumn = 5;
    [SerializeField] private int columnsPerRow = 5;
    [SerializeField] private float xColumnSpacing = 0.5f;
    [SerializeField] private float zRowSpacing = 0.5f;
    
    [Header("Animation")]
    [SerializeField] private float jumpPower = 1.0f;
    [SerializeField] private float jumpDuration = 0.5f;

    private List<GameObject> spawnedProducts = new();
    private Coroutine spawnCoroutine;
    private WaitForSeconds spawnWait;

    public ProductType GetProductType() => productType;
    public int CurrentProductCount => spawnedProducts.Count;
    public bool IsFull => spawnedProducts.Count >= maxCapacity;

    private void Start()
    {
        spawnWait = new WaitForSeconds(spawnInterval);

        if (autoSpawn)
        {
            spawnCoroutine = StartCoroutine(SpawnLoop());
        }
    }

    private void OnDisable()
    {
        if (spawnCoroutine != null)
        {
            StopCoroutine(spawnCoroutine);
            spawnCoroutine = null;
        }
    }

    private IEnumerator SpawnLoop()
    {
        while (true)
        {
            yield return spawnWait;

            if (!IsFull)
            {
                SpawnProduct();
            }
        }
    }
    public void SpawnProduct()
    {
        if (IsFull) return;
        if (productPrefab == null) return;

        GameObject newProduct = Instantiate(productPrefab, itemParent);
        Vector3 targetPosition = CalculateGridPosition(spawnedProducts.Count);
        
        newProduct.transform.localPosition = Vector3.zero;
        newProduct.transform.localRotation = Quaternion.identity;

        newProduct.transform.DOLocalJump(
            targetPosition, 
            jumpPower, 
            1, 
            jumpDuration
        ).SetEase(Ease.OutQuad);
        
        spawnedProducts.Add(newProduct);
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

    public GameObject TakeLastProduct()
    {
        if (spawnedProducts.Count == 0) return null;

        int lastIndex = spawnedProducts.Count - 1;
        GameObject item = spawnedProducts[lastIndex];
        spawnedProducts.RemoveAt(lastIndex);
        item.transform.DOKill();
        return item;
    }
}