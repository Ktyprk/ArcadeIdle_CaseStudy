using UnityEngine;

public class Product : MonoBehaviour
{
    [SerializeField] private ProductType productType;

    public ProductType Type => productType;
}