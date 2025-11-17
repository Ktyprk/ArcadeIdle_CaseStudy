using System.Collections;
using UnityEngine;

public class PlayerTriggerManager : MonoBehaviour
{
    public delegate void CollectAreaAction(ProductSpawner spawner);
    public static event CollectAreaAction onCollectAreaStay;

    public delegate void DropAreaAction(FactoryInputArea inputArea);
    public static event DropAreaAction onDropAreaStay;
    
    public delegate void ConstructionAreaAction(ConstructionSite constructionSite);
    public static event ConstructionAreaAction onConstructionAreaStay;
    
    public delegate void TrashAreaAction(TrashArea trashArea);
    public static event TrashAreaAction onTrashAreaStay;

    [SerializeField] private float interactionTickRate = 0.25f;

    private ProductSpawner currentSpawner;
    private FactoryInputArea currentInputArea;
    private ConstructionSite currentConstructionSite; 
    private TrashArea currentTrashArea;
    
    private Coroutine interactionCoroutine;
    private WaitForSeconds interactionWait;

    private void OnEnable()
    {
        interactionWait = new WaitForSeconds(interactionTickRate);
        interactionCoroutine = StartCoroutine(InteractionCoroutine());
    }

    private void OnDisable()
    {
        if (interactionCoroutine != null)
        {
            StopCoroutine(interactionCoroutine);
            interactionCoroutine = null;
        }
    }

    private IEnumerator InteractionCoroutine()
    {
        while (true)
        {
            if (currentSpawner != null)
            {
                onCollectAreaStay?.Invoke(currentSpawner);
            }

            if (currentInputArea != null)
            {
                onDropAreaStay?.Invoke(currentInputArea);
            }

            if (currentConstructionSite != null)
            {
                onConstructionAreaStay?.Invoke(currentConstructionSite);
            }

            if (currentTrashArea != null)
            {
                onTrashAreaStay?.Invoke(currentTrashArea);
            }
            
            yield return interactionWait;
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("CollectArea"))
        {
            if (other.TryGetComponent<ProductSpawner>(out var spawner))
            {
                currentSpawner = spawner;
            }
        }

        if (other.CompareTag("DropArea"))
        {
            if (other.TryGetComponent<FactoryInputArea>(out var inputArea))
            {
                currentInputArea = inputArea;
            }
        }

        if (other.CompareTag("ConstructionArea"))
        {
            if (other.TryGetComponent<ConstructionSite>(out var constSite))
            {
                currentConstructionSite = constSite;
            }
        }

        if (other.CompareTag("TrashArea"))
        {
            if (other.TryGetComponent<TrashArea>(out var trashArea))
            {
                currentTrashArea = trashArea;
            }
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("CollectArea")) 
        {
            if (other.TryGetComponent<ProductSpawner>(out var spawner) && currentSpawner == spawner)
            {
                currentSpawner = null;
            }
        }

        if (other.CompareTag("DropArea"))
        {
            if (other.TryGetComponent<FactoryInputArea>(out var inputArea) && currentInputArea == inputArea)
            {
                currentInputArea = null;
            }
        }
        
        if (other.CompareTag("ConstructionArea"))
        {
            if (other.TryGetComponent<ConstructionSite>(out var constSite) && currentConstructionSite == constSite)
            {
                currentConstructionSite = null;
            }
        }
        
        if (other.CompareTag("TrashArea"))
        {
            if (other.TryGetComponent<TrashArea>(out var trashArea) && currentTrashArea == trashArea)
            {
                currentTrashArea = null;
            }
        }
    }
}