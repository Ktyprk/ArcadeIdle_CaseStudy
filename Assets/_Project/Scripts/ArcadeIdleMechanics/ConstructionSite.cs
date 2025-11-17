using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;
using System;

public class ConstructionSite : MonoBehaviour
{
    [Header("Setup")]
    [SerializeField] private List<Product> buildParts; 

    [Header("Materials")]
    [SerializeField] private Material silhouetteMaterial;
    
    [Header("Settings")]
    [SerializeField] private float buildAnimationTime = 0.5f;
    [SerializeField] private float dropAnimYOffset = 2f; 

    private Dictionary<MeshRenderer, Material> originalMaterials = new();
    private int currentPartIndex = 0;
    
    private int itemsInTransit = 0;
    private HashSet<int> completedParts = new HashSet<int>();
    
    [SerializeField] private GameObject completionEffectPrefab;
    public event Action OnPartCompleted;

    public bool IsComplete { get; private set; } = false;

    private void Start()
    {
        InitializeSilhouettes();
    }

    private void InitializeSilhouettes()
    {
        originalMaterials.Clear();
        completedParts.Clear(); 
        
        foreach (var part in buildParts)
        {
            if (part == null) continue;

            if (part.TryGetComponent<MeshRenderer>(out var partRenderer))
            {
                if (!originalMaterials.ContainsKey(partRenderer))
                {
                    originalMaterials.Add(partRenderer, partRenderer.material);
                    partRenderer.material = silhouetteMaterial;
                }
            }
        }
    }

    public void ReceiveItem(GameObject item)
    {
        if (IsComplete)
        {
            Destroy(item);
            return;
        }

        int partIndexToBuild = currentPartIndex + itemsInTransit;

        if (partIndexToBuild >= buildParts.Count)
        {
            Destroy(item);
            return;
        }
        
        Product product = item.GetComponent<Product>();
        ProductType neededType = buildParts[partIndexToBuild].Type;

        if (product == null || product.Type != neededType)
        {
            Debug.LogWarning("Yanlış tipte item alındı.");
            Destroy(item);
            return;
        }

        itemsInTransit++;

        Product partToBuild = buildParts[partIndexToBuild];
        Transform finalTransform = partToBuild.transform;

        item.transform.SetParent(null);
        item.SetActive(true);

        Vector3 finalPos = finalTransform.position;
        Vector3 startPos = finalPos + (Vector3.up * dropAnimYOffset);
        
        item.transform.position = startPos;
        
        Quaternion finalRot = finalTransform.rotation;
        item.transform.rotation = finalRot; 

        item.transform.DOMove(finalPos, buildAnimationTime)
            .SetEase(Ease.OutBounce) 
            .OnComplete(() => 
            {
                Instantiate(completionEffectPrefab, finalPos, completionEffectPrefab.transform.rotation);
                Destroy(item); 
                
                itemsInTransit--;
                
                completedParts.Add(partIndexToBuild);
                
                while (currentPartIndex < buildParts.Count && completedParts.Contains(currentPartIndex))
                {
                    RestorePartMaterial(currentPartIndex);
                    currentPartIndex++;
                    
                    OnPartCompleted?.Invoke();
                }
                
                if (currentPartIndex >= buildParts.Count)
                {
                    IsComplete = true;
                    Debug.Log("İnşaat Tamamlandı!");
                }
                
            });
    }

    private void RestorePartMaterial(int index)
    {
        if (index < 0 || index >= buildParts.Count) return;

        Product part = buildParts[index];
        if (part == null) return;
        
        if (part.TryGetComponent<MeshRenderer>(out var partRenderer))
        {
            if (originalMaterials.TryGetValue(partRenderer, out Material originalMat))
            {
                partRenderer.material = originalMat;
            }
        }
    }

    public ProductType GetNextRequiredType()
    {
        if (IsComplete) return null;
    
        int nextPartIndex = currentPartIndex + itemsInTransit;

        if (nextPartIndex >= buildParts.Count) return null;
        
        return buildParts[nextPartIndex].Type;
    }

    public bool NeedsProduct(ProductType type)
    {
        if (IsComplete) return false;

        ProductType nextType = GetNextRequiredType();
        return (nextType != null && nextType == type);
    }
}