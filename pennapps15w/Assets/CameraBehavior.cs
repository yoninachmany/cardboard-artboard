using UnityEngine;
using System.Collections;

public class CameraBehavior : MonoBehaviour {

	GameObject ColorToolBar;
	CreatePicker colorPicker;
	GameObject cyl;

	// Use this for initialization
	void Start () {
		cyl = GameObject.CreatePrimitive(PrimitiveType.Cylinder);
		cyl.transform.parent = transform;
		cyl.transform.localPosition = new Vector3(0f, 0f, 0.7f);
		cyl.transform.Rotate(90, 0, 0);
		cyl.transform.localScale = new Vector3(0.05f, .0000000007f, 0.05f);
	}
	
	// Update is called once per frame
	void Update () {
		ColorToolBar = GameObject.Find ("ColorToolBar");
		colorPicker = ColorToolBar.GetComponent<CreatePicker> ();
		cyl.renderer.material.color = colorPicker.currentColor;
	}
}
