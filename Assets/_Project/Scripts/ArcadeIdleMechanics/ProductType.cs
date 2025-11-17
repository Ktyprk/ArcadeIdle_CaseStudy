using UnityEngine;

[CreateAssetMenu(fileName = "ProductType_", menuName = "Game/Product Type")]
public class ProductType : ScriptableObject
{
    [Tooltip("Bu ürün stack'teyken bir öncekinden ne kadar uzakta duracak?")]
    [SerializeField] private Vector3 stackOffset = new Vector3(0, 0.1f, 0);

    [Tooltip("Bu ürün stack'e eklendiğinde hangi lokal rotasyonda duracak?")]
    [SerializeField] private Vector3 stackRotation = Vector3.zero;

    public Vector3 StackOffset => stackOffset;
    public Vector3 StackRotation => stackRotation;
}