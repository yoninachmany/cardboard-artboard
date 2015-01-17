using UnityEngine;
using System.Collections;

public class CameraBehavior : MonoBehaviour {

	// Use this for initialization
	void Start () {
//		GameObject cyl = GameObject.CreatePrimitive (PrimitiveType.Cylinder);
//		cyl.transform.Translate (0f, 0f, 0.7f);
//		cyl.tranform.Rotate (90, 0, 0);
//		cyl.transform.Scale (0.05f, .0000000007f, 0.05f);
//		cyl.renderer.material.color = Color.black;
		GameObject cyl = GameObject.CreatePrimitive(PrimitiveType.Cylinder);
//		plane.transform.Translate(0,0,8);
//		plane.transform.Rotate (270, 0, 0);
		cyl.transform.parent = transform;
		cyl.transform.localPosition = new Vector3(0f, 0f, 2.5f);
		cyl.transform.Rotate(90, 0, 0);
		cyl.transform.localScale = new Vector3(0.1f, .0000000007f, 0.1f);
		cyl.renderer.material.color = Color.black;
	}
	
	// Update is called once per frame
	void Update () {
	
	}
}
