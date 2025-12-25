using UnityEngine;

public class SimpleEnemyAI : MonoBehaviour
{
    public float moveSpeed = 3f;
    public float stoppingDistance = 2f; // На каком расстоянии останавливаться от игрока
    public float rotationSpeed = 5f;

    private Transform playerTarget;
    private UnityEngine.AI.NavMeshAgent navAgent; 

    void Start()
    {
        // Находим игрока по тегу 
        playerTarget = GameObject.FindGameObjectWithTag("Player").transform;

        // Настраиваем NavMeshAgent для простого ИИ пути
        navAgent = GetComponent<UnityEngine.AI.NavMeshAgent>();
        if (navAgent == null)
        {
            navAgent = gameObject.AddComponent<UnityEngine.AI.NavMeshAgent>();
        }

        navAgent.speed = moveSpeed;
        navAgent.stoppingDistance = stoppingDistance;
        navAgent.angularSpeed = rotationSpeed * 100; 
    }

    void Update()
    {
        if (playerTarget != null && navAgent != null && navAgent.isActiveAndEnabled)
        {
            // Устанавливаем точку назначения - позицию игрока
            navAgent.SetDestination(playerTarget.position);

            // Визуальное "наведение" - враг смотрит на игрока
            Vector3 lookPos = playerTarget.position - transform.position;
            lookPos.y = 0; 
            if (lookPos != Vector3.zero)
            {
                Quaternion targetRotation = Quaternion.LookRotation(lookPos);
                transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, Time.deltaTime * rotationSpeed);
            }
        }
    }
}