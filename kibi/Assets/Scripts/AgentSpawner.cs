using UnityEngine;
using UnityEngine.AI;

public class AgentSpawner : MonoBehaviour
{
    public GameObject agentPrefab;
    public int count = 10;
    public Vector3 areaCenter = Vector3.zero;
    public Vector3 areaSize = new Vector3(9f, 0f, 9f);

    void Start()
    {
        for (int i = 0; i < count; i++)
        {
            var rnd = areaCenter + new Vector3(
                Random.Range(-areaSize.x * 0.5f, areaSize.x * 0.5f),
                0f,
                Random.Range(-areaSize.z * 0.5f, areaSize.z * 0.5f)
            );

            if (NavMesh.SamplePosition(rnd, out NavMeshHit hit, 3f, NavMesh.AllAreas))
            {
                var go = Instantiate(agentPrefab, hit.position, Quaternion.identity);
                var nav = go.GetComponent<NavMeshAgent>();
                if (nav != null) nav.Warp(hit.position); // clávalo al NavMesh
            }
        }
    }

    void OnDrawGizmosSelected()
    {
        Gizmos.color = new Color(0f, 1f, 0f, 0.2f);
        Gizmos.DrawCube(areaCenter, areaSize);
        Gizmos.color = Color.green;
        Gizmos.DrawWireCube(areaCenter, areaSize);
    }
}
