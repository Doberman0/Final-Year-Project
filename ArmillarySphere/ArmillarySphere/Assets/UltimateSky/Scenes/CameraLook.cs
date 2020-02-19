using UnityEngine;
using System.Collections;


namespace UltimateSky.Demo {

	[RequireComponent(typeof(Camera))]
	public class CameraLook : MonoBehaviour {

		protected virtual void Update () {
			Camera cam = this.GetComponent<Camera> ();
			if (Input.GetMouseButton (1)) {
				Vector3 mousePos = Input.mousePosition;
				mousePos.z = 1.0f;
				Vector3 lookPos = Vector3.Slerp(this.transform.TransformPoint(Vector3.forward), cam.ScreenToWorldPoint(mousePos), Time.deltaTime);
				this.transform.LookAt (lookPos);
			}
		}
	}
}