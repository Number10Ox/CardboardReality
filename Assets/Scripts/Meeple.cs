using UnityEngine;

public class Meeple : MonoBehaviour {

    public GameObject[] AnimatedObjects;

    void Start () {
        for (int i = 0; i < AnimatedObjects.Length; i++)
        {
            var animator = AnimatedObjects[i].GetComponent<Animator>();
            if (animator)
            {
                animator.Play("Idle");
            }
        }
    }
	
	void Update () {
		
	}

    public void Attack()
    {
        for (int i = 0; i < AnimatedObjects.Length; i++)
        {
            var animator = AnimatedObjects[i].GetComponent<Animator>();
            if (animator)
            {
                animator.Play("Attack");
            }
        }
    }
}
