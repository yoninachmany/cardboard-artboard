using UnityEngine;
using System.Collections;
using System.Text;

public class CameraBehavior : MonoBehaviour {
	WWW get;
	WWW poster;

	GameObject ColorToolBar;
	CreatePicker colorPicker;
	GameObject cyl;

	// Use this for initialization
	void Start () {
		GameObject cyl = GameObject.CreatePrimitive(PrimitiveType.Cylinder);
		cyl.transform.parent = transform;
		cyl.transform.localPosition = new Vector3(0f, 0f, 2.5f);
		cyl.transform.Rotate(90, 0, 0);
		cyl.transform.localScale = new Vector3(0.1f, .0000000007f, 0.1f);
	}
	
	// Update is called once per frame
	void Update () {
		ColorToolBar = GameObject.Find ("ColorToolBar");
		colorPicker = ColorToolBar.GetComponent<CreatePicker> ();
		//cyl.renderer.material.color = colorPicker.currentColor;
	}
}
