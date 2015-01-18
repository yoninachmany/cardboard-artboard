using UnityEngine;
using System.Collections;
using System.Text;

public class CameraBehavior : MonoBehaviour {
	WWW get;
	WWW poster;

	// Use this for initialization
	void Start () {
		GameObject cyl = GameObject.CreatePrimitive(PrimitiveType.Cylinder);
		cyl.transform.parent = transform;
		cyl.transform.localPosition = new Vector3(0f, 0f, 0.7f);
		cyl.transform.Rotate(90, 0, 0);
		cyl.transform.localScale = new Vector3(0.05f, .0000000007f, 0.05f);
		cyl.renderer.material.color = Color.black;
	}
	
	// Update is called once per frame
	void Update () {

	}
}
