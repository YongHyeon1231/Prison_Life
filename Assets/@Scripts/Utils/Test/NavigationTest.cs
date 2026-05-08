using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class NavigationTest : MonoBehaviour
{
    NavMeshAgent _navMeshAgent;

    void Start()
    {
        _navMeshAgent = GetComponent<NavMeshAgent>();
        _navMeshAgent.SetDestination(new Vector3(-2.4f, 0, -12.0f));
    }

    void Update()
    {
        transform.position = new Vector3(transform.position.x, 0.38f, transform.position.z);
    }
}
