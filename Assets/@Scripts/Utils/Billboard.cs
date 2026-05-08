using UnityEngine;

public class Billboard : MonoBehaviour
{
	void LateUpdate()
	{
		// Vector3 dir = transform.position - Camera.main.transform.position;
		// transform.LookAt(transform.position + dir);
		transform.rotation = Camera.main.transform.rotation;
	}
}
