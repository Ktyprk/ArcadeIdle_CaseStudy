using UnityEngine;

public class TrashArea : MonoBehaviour
{
    public void TrashItem(GameObject itemToTrash)
    {
        Destroy(itemToTrash);
    }
}