using UnityEngine;
using System.Collections;


public class CreatePicker : MonoBehaviour {
	public Color currentColor = Color.red;
	public GameObject cubeCurr;

	// Use this for initialization
	void Start () {

		// color picker plane
		GameObject palette = GameObject.CreatePrimitive(PrimitiveType.Plane);
		palette.transform.Translate(0,5,8);
		palette.transform.Rotate(270, 0, 0);
		var paletteScript = palette.AddComponent<Palette>();

		// cube shows current color
		cubeCurr = GameObject.CreatePrimitive(PrimitiveType.Cube);
		cubeCurr.transform.Translate(-2, 1, 5);
		cubeCurr.renderer.material.color = Color.red;

		// blue cube for color picking
		GameObject cubeBlue = GameObject.CreatePrimitive(PrimitiveType.Cube);
		cubeBlue.transform.Translate(-2, 2, 5);
		cubeBlue.renderer.material.color = Color.blue;
		var cubeScript = cubeBlue.AddComponent<Teleport>();

		/*
		GameObject cubeGreen = GameObject.CreatePrimitive(PrimitiveType.Cube);
		cubeGreen.transform.Translate(-2, 3, 5);
		cubeGreen.renderer.material.color = Color.green;
		cubeScript = cubeGreen.AddComponent<Teleport>();

		GameObject cubeRed = GameObject.CreatePrimitive(PrimitiveType.Cube);
		cubeRed.transform.Translate(-2, 4, 5);
		cubeRed.renderer.material.color = Color.red;
		cubeScript = cubeRed.AddComponent<Teleport>();*/
	}
	
	// Update is called once per frame
	void Update () {
		cubeCurr.renderer.material.color = currentColor;
	}
}
