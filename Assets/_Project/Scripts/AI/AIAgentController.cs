using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.AI;

[RequireComponent(typeof(NavMeshAgent), typeof(AgentInventory), typeof(Animator))]
public class AIAgentController : MonoBehaviour
{
    private enum AgentState
    {
        Deciding,
        MovingToCollect,
        Collecting,
        MovingToDrop,
        Dropping,
        MovingToWait,
        Waiting
    }

    [Header("AI Settings")]
    [SerializeField] private Transform waitingPoint;
    [SerializeField] private float interactionInterval = 0.2f;
    [SerializeField] private float waitCheckInterval = 1.0f;

    private NavMeshAgent agent;
    private AgentInventory inventory;
    private Animator animator;
    
    private AgentState currentState;
    private ProductSpawner targetSpawner;
    private FactoryInputArea targetInputArea;

    private List<ProductSpawner> allSpawners;
    private List<FactoryInputArea> allInputAreas;

    private float timer;

    private readonly int isWalkingHash = Animator.StringToHash("IsWalking");

    private void Awake()
    {
        agent = GetComponent<NavMeshAgent>();
        inventory = GetComponent<AgentInventory>();
        animator = GetComponent<Animator>();
    }

    private void Start()
    {
        CacheWorldTargets();
        SetState(AgentState.Deciding);
    }

    private void CacheWorldTargets()
    {
        allSpawners = new List<ProductSpawner>(FindObjectsOfType<ProductSpawner>());
        allInputAreas = new List<FactoryInputArea>(FindObjectsOfType<FactoryInputArea>());
    }

    private void Update()
    {
        timer -= Time.deltaTime;
        
        HandleStateMachine();
        UpdateAnimations();
    }

    private void HandleStateMachine()
    {
        switch (currentState)
        {
            case AgentState.Deciding:
                DecideNextAction();
                break;
            
            case AgentState.MovingToCollect:
                if (HasReachedDestination())
                    SetState(AgentState.Collecting);
                break;
            
            case AgentState.Collecting:
                HandleCollecting();
                break;
            
            case AgentState.MovingToDrop:
                if (HasReachedDestination())
                    SetState(AgentState.Dropping);
                break;
            
            case AgentState.Dropping:
                HandleDropping();
                break;
            
            case AgentState.MovingToWait:
                if (HasReachedDestination())
                    SetState(AgentState.Waiting);
                break;
            
            case AgentState.Waiting:
                HandleWaiting();
                break;
        }
    }

    private void UpdateAnimations()
    {
        bool isMoving = agent.velocity.magnitude > 0.1f;
        animator.SetBool(isWalkingHash, isMoving);
    }

    private void SetState(AgentState newState)
    {
        currentState = newState;
        timer = 0f;
    }

    private void DecideNextAction()
    {
        if (inventory.CurrentItemCount == 0)
        {
            targetSpawner = FindAvailableSpawner();
            if (targetSpawner != null)
            {
                agent.SetDestination(targetSpawner.transform.position);
                SetState(AgentState.MovingToCollect);
            }
            else
            {
                MoveToWaitPoint();
            }
        }
        else 
        {
            targetInputArea = FindAvailableInputArea();
            if (targetInputArea != null)
            {
                agent.SetDestination(targetInputArea.transform.position);
                SetState(AgentState.MovingToDrop);
            }
            else
            {
                MoveToWaitPoint();
            }
        }
    }

    private void HandleCollecting()
    {
        if (inventory.IsFull || targetSpawner.CurrentProductCount == 0)
        {
            SetState(AgentState.Deciding);
            return;
        }

        if (timer <= 0f)
        {
            GameObject item = targetSpawner.TakeLastProduct();
            if (item != null)
            {
                ProductType itemType = targetSpawner.GetProductType();
                inventory.AddItemToStack(item, itemType);
            }
            timer = interactionInterval;
        }
    }

    private void HandleDropping()
    {
        if (inventory.CurrentItemCount == 0)
        {
            SetState(AgentState.Deciding);
            return;
        }

        if (targetInputArea.IsFull)
        {
            SetState(AgentState.Deciding);
            return;
        }

        if (timer <= 0f)
        {
            GameObject item = inventory.RemoveLastItemFromStack();
            if (item != null)
            {
                targetInputArea.AddItemToStack(item);
            }
            timer = interactionInterval;
        }
    }

    private void HandleWaiting()
    {
        if (timer <= 0f)
        {
            SetState(AgentState.Deciding);
            timer = waitCheckInterval;
        }
    }

    private ProductSpawner FindAvailableSpawner()
    {
        ProductType holdingType = inventory.CurrentHoldingType;
        
        return allSpawners.FirstOrDefault(s => 
            s.CurrentProductCount > 0 && 
            (holdingType == null || s.GetProductType() == holdingType)
        );
    }

    private FactoryInputArea FindAvailableInputArea()
    {
        ProductType holdingType = inventory.CurrentHoldingType;
        if (holdingType == null) return null;

        return allInputAreas.FirstOrDefault(i => 
            !i.IsFull && 
            i.RequiredInputType == holdingType
        );
    }
    
    private void MoveToWaitPoint()
    {
        if (waitingPoint != null)
        {
            agent.SetDestination(waitingPoint.position);
            SetState(AgentState.MovingToWait);
        }
        else
        {
            SetState(AgentState.Waiting);
        }
    }

    private bool HasReachedDestination()
    {
        return !agent.pathPending && agent.remainingDistance <= agent.stoppingDistance;
    }
}