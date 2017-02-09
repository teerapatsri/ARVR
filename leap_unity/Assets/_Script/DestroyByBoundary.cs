using UnityEngine;
using System.Collections;

public class DestroyByBoundary : MonoBehaviour
{
	void OnTriggerExit(Collider other)
	{
		Destroy(other.gameObject,0.5f);
	}
}