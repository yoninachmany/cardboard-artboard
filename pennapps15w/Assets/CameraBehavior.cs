﻿using UnityEngine;
using System.Collections;
using System.Text;

public class CameraBehavior : MonoBehaviour {
	WWW get;
	WWW poster;

	GameObject ColorToolBar;
	CreatePicker colorPicker;
	GameObject cyl;
	public static float cylRadius;

	// Use this for initialization
	void Start () {
		cyl = GameObject.CreatePrimitive(PrimitiveType.Cylinder);
		cyl.transform.parent = transform;
		cyl.transform.localPosition = new Vector3(0f, 0f, 3.9f);
		cyl.transform.Rotate(90, 0, 0);
		cyl.transform.localScale = new Vector3(0.2f, .0000000007f, 0.2f);
		cylRadius = 0.2f;
	}
	
	// Update is called once per frame
	void Update () {
		ColorToolBar = GameObject.Find ("ColorToolBar");
		colorPicker = ColorToolBar.GetComponent<CreatePicker> ();
		cyl.renderer.material.color = colorPicker.currentColor;

		GameObject plane = GameObject.Find ("GroundPlane");
		Paint circle = plane.GetComponent<Paint>();
		cylRadius = circle.radius / 35.0f;
		cyl.transform.localScale = new Vector3(cylRadius, .0000000007f, cylRadius);
	}
}
