using UnityEngine;
using UnityEngine.AI;

[RequireComponent(typeof(NavMeshAgent))]
public class AgentWander : MonoBehaviour
{
    [Header("Wander")]
    public float radius = 8f;
    public float waitTime = 1.5f;

    private NavMeshAgent agent;
    private float timer;

    void Awake()
    {
        agent = GetComponent<NavMeshAgent>();
    }

    void Start()
    {
        // Si el agente no está en NavMesh al arrancar, intenta ajustarlo al más cercano
        if (!agent.isOnNavMesh)
        {
            if (NavMesh.SamplePosition(transform.position, out NavMeshHit hit, 2f, NavMesh.AllAreas))
            {
                agent.Warp(hit.position);
            }
            else
            {
                // No hay NavMesh cerca: desactivo para evitar spam de errores
                enabled = false;
                Debug.LogWarning($"{name}: No NavMesh nearby. Disabling AgentWander.");
                return;
            }
        }

        SetNewDestination();
    }

    void Update()
    {
        // Guardas anti-spam: agente nulo o fuera de NavMesh
        if (agent == null || !agent.isOnNavMesh) return;

        // Evita consultar remainingDistance mientras calcula ruta
        if (!agent.pathPending && agent.remainingDistance <= agent.stoppingDistance)
        {
            timer += Time.deltaTime;
            if (timer >= waitTime)
            {
                SetNewDestination();
                timer = 0f;
            }
        }
    }

    bool TrySetDestination(Vector3 pos)
    {
        if (!agent.isOnNavMesh) return false;

        var path = new NavMeshPath();
        if (NavMesh.CalculatePath(agent.transform.position, pos, NavMesh.AllAreas, path) &&
            path.status == NavMeshPathStatus.PathComplete)
        {
            agent.SetPath(path);
            return true;
        }
        return false;
    }

    void SetNewDestination()
    {
        // Punto aleatorio dentro del radio
        var random = Random.insideUnitSphere * radius + transform.position;

        // Proyecta ese punto al NavMesh y valida la ruta
        if (NavMesh.SamplePosition(random, out NavMeshHit hit, radius, NavMesh.AllAreas))
        {
            if (!TrySetDestination(hit.position))
            {
                // Si falla, intenta un segundo muestreo rápido más cerca
                if (NavMesh.SamplePosition(transform.position, out NavMeshHit near, 1.5f, NavMesh.AllAreas))
                    TrySetDestination(near.position);
            }
        }
    }
}
