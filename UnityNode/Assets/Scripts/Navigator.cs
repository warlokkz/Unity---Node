using UnityEngine;
using System.Collections;

public class Navigator : MonoBehaviour {

    NavMeshAgent agent;
    Animator animator;
    Targeter targeter;

	// Use this for initialization
	void Awake () {
        agent = GetComponent<NavMeshAgent>();
        animator = GetComponent<Animator>();
        targeter = GetComponent<Targeter>();
	}
	
	// Update is called once per frame
	public void NavigateTo (Vector3 position) {
        agent.SetDestination(position);
        targeter.target = null;
    }

    void Update() {
        animator.SetFloat("Distance", agent.remainingDistance);
    }
}
