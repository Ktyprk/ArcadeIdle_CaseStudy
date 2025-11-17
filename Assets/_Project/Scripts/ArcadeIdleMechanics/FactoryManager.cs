using System.Collections;
using UnityEngine;

public class FactoryManager : MonoBehaviour
{
    [Header("Factory I/O")]
    [SerializeField] private FactoryInputArea inputArea;
    [SerializeField] private ProductSpawner outputProductSpawner;

    [Header("Processing")]
    [SerializeField] private float processingTime = 1.0f;

    [Header("Animation")]
    [SerializeField] private Animator factoryAnimator;
    
    private readonly int isProcessingHash = Animator.StringToHash("isProcessing");

    private Coroutine processingCoroutine;
    private WaitForSeconds processWait;

    private void Awake()
    {
        processWait = new WaitForSeconds(processingTime);
    }

    private void Start()
    {
        processingCoroutine = StartCoroutine(ProcessQueue());
    }

    private void OnDisable()
    {
        if (processingCoroutine != null)
        {
            StopCoroutine(processingCoroutine);
            processingCoroutine = null;
        }
        
        SetAnimationState(false);
    }

    private IEnumerator ProcessQueue()
    {
        while (true)
        {
            bool outputIsFree = (outputProductSpawner == null || !outputProductSpawner.IsFull);
            bool inputHasItems = (inputArea != null && inputArea.CurrentItemCount > 0);

            if (outputIsFree && inputHasItems)
            {
                GameObject itemToProcess = inputArea.TakeLastItem();

                if (itemToProcess != null)
                {
                    SetAnimationState(true);
                    yield return processWait;

                    Destroy(itemToProcess);

                    if (outputProductSpawner != null)
                    {
                        outputProductSpawner.SpawnProduct();
                    }

                    SetAnimationState(false);
                }
                else
                {
                    SetAnimationState(false);
                    yield return null;
                }
            }
            else
            {
                SetAnimationState(false);
                yield return null;
            }
        }
    }
    
    private void SetAnimationState(bool isProcessing)
    {
        if (factoryAnimator != null)
        {
            factoryAnimator.SetBool(isProcessingHash, isProcessing);
        }
    }
}