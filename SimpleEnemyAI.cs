using UnityEngine;
using UnityEngine.AI;

public class SimpleEnemyAI : MonoBehaviour
{
    public float moveSpeed = 3f;
    public float stoppingDistance = 2f;
    public float rotationSpeed = 5f;
    public float updateTargetInterval = 0.5f; // Как часто ищем нового игрока

    private Transform playerTarget;
    private NavMeshAgent navAgent;
    private float lastUpdateTime = 0f;

    void Start()
    {
        navAgent = GetComponent<NavMeshAgent>();
        if (navAgent == null)
        {
            navAgent = gameObject.AddComponent<NavMeshAgent>();
        }

        navAgent.speed = moveSpeed;
        navAgent.stoppingDistance = stoppingDistance;
        navAgent.angularSpeed = rotationSpeed * 100;
        
        FindNearestPlayer(); // Находим игрока при старте
    }

    void Update()
    {
        // Периодически обновляем цель (чтобы не делать это каждый кадр)
        if (Time.time - lastUpdateTime > updateTargetInterval)
        {
            FindNearestPlayer();
            lastUpdateTime = Time.time;
        }

        // Если есть цель и она активна - преследуем
        if (playerTarget != null && playerTarget.gameObject.activeSelf && navAgent != null && navAgent.isActiveAndEnabled)
        {
            navAgent.SetDestination(playerTarget.position);

            // Поворачиваемся к цели
            Vector3 lookPos = playerTarget.position - transform.position;
            lookPos.y = 0;
            if (lookPos != Vector3.zero)
            {
                Quaternion targetRotation = Quaternion.LookRotation(lookPos);
                transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, Time.deltaTime * rotationSpeed);
            }
        }
        else
        {
            // Если цель не активна или null, останавливаемся
            if (navAgent != null && navAgent.hasPath)
            {
                navAgent.ResetPath();
            }
        }
    }

    void FindNearestPlayer()
    {
        // Находим ВСЕХ игроков на сцене
        GameObject[] players = GameObject.FindGameObjectsWithTag("Player");
        
        if (players.Length == 0)
        {
            playerTarget = null;
            return;
        }

        // Ищем ближайшего активного игрока
        GameObject nearestPlayer = null;
        float nearestDistance = Mathf.Infinity;
        
        foreach (GameObject player in players)
        {
            // Проверяем, активен ли игрок
            if (player.activeSelf)
            {
                float distance = Vector3.Distance(transform.position, player.transform.position);
                if (distance < nearestDistance)
                {
                    nearestDistance = distance;
                    nearestPlayer = player;
                }
            }
        }

        if (nearestPlayer != null)
        {
            playerTarget = nearestPlayer.transform;
        }
        else
        {
            playerTarget = null;
        }
    }

    // Метод для принудительной смены цели (можно вызвать из MissionManager)
    public void ForceUpdateTarget()
    {
        FindNearestPlayer();
    }
}
