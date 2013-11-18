using UnityEngine;
using System.Collections;

public class CameraFollowObject : MonoBehaviour {

	public GameObject objectToFollow = null;

	void Update() {
		transform.position = new Vector3(objectToFollow.transform.position.x, objectToFollow.transform.position.y, transform.position.z);
	}
}
