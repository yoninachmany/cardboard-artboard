using UnityEngine;
using System.Collections;

public class PalettePanels : MonoBehaviour {
	private CardboardHead head;
	private Vector3 startingPosition;
	public GameObject palette; 
	private MeshCollider meshCollider; 
	RaycastHit hit;
	
	GameObject ColorToolBar;
	CreatePicker colorPicker;

	GameObject size1;
	GameObject size2;
	GameObject size3;
	GameObject size4;
	GameObject size5;

	GameObject import;
	GameObject export;
	GameObject newDoc;

	public Texture2D tex1;
	public Texture2D tex2;
	public Texture2D tex3;
	public Texture2D tex4;
	public Texture2D tex5;

	public Texture2D texImport;
	public Texture2D texExport;
	public Texture2D texNew;

	void AddButtons() {
		size1 = GameObject.CreatePrimitive (PrimitiveType.Plane);
		size1.transform.localScale = new Vector3 (0.0625f, 0.0625f, 0.0625f);
		size1.transform.Translate(-3, 8, 7);
		size1.transform.Rotate (270, 0, 0);
		size1.renderer.material.mainTexture = tex1;

		size2 = GameObject.CreatePrimitive (PrimitiveType.Plane);
		size2.transform.localScale = new Vector3 (0.0625f, 0.0625f, 0.0625f);
		size2.transform.Translate(-3, 7, 7);
		size2.transform.Rotate (270, 0, 0);
		size2.renderer.material.mainTexture = tex2;

		size3 = GameObject.CreatePrimitive (PrimitiveType.Plane);
		size3.transform.localScale = new Vector3 (0.0625f, 0.0625f, 0.0625f);
		size3.transform.Translate(-3, 6, 7);
		size3.transform.Rotate (270, 0, 0);
		size3.renderer.material.mainTexture = tex3;

		size4 = GameObject.CreatePrimitive (PrimitiveType.Plane);
		size4.transform.localScale = new Vector3 (0.0625f, 0.0625f, 0.0625f);
		size4.transform.Translate(-3, 5, 7);
		size4.transform.Rotate (270, 0, 0);
		size4.renderer.material.mainTexture = tex4;

		size5 = GameObject.CreatePrimitive (PrimitiveType.Plane);
		size5.transform.localScale = new Vector3 (0.0625f, 0.0625f, 0.0625f);
		size5.transform.Translate(-3, 4, 7);
		size5.transform.Rotate (270, 0, 0);
		size5.renderer.material.mainTexture = tex5;

		import = GameObject.CreatePrimitive (PrimitiveType.Plane);
		import.transform.localScale = new Vector3 (0.0625f, 0.0625f, 0.0625f);
		import.transform.Translate(-3, 3, 7);
		import.transform.Rotate (270, 0, 0);
		import.renderer.material.mainTexture = texImport;
		
		export = GameObject.CreatePrimitive (PrimitiveType.Plane);
		export.transform.localScale = new Vector3 (0.0625f, 0.0625f, 0.0625f);
		export.transform.Translate(-3, 2, 7);
		export.transform.Rotate (270, 0, 0);
		export.renderer.material.mainTexture = texExport;

		newDoc = GameObject.CreatePrimitive (PrimitiveType.Plane);
		newDoc.transform.localScale = new Vector3 (0.0625f, 0.0625f, 0.0625f);
		newDoc.transform.Translate(-3, 1, 7);
		newDoc.transform.Rotate (270, 0, 0);
		newDoc.renderer.material.mainTexture = texNew;

		tex1.Apply ();
		tex2.Apply ();
		tex3.Apply ();
		tex4.Apply ();
		tex5.Apply ();

		texImport.Apply();
		texExport.Apply();
		texNew.Apply();
	}
	// Use this for initialization
	void Start () {
		AddButtons();

		head = Camera.main.GetComponent<StereoController>().Head;
		startingPosition = transform.localPosition;
		CardboardGUI.IsGUIVisible = true;

		ColorToolBar = GameObject.Find ("ColorToolBar");
		colorPicker = ColorToolBar.GetComponent<CreatePicker>();
	}
	
	// Update is called once per frame
	void Update () {
		GameObject plane = GameObject.Find ("Plane");
		Paint circle = plane.GetComponent<Paint>();

		bool size1LA = size1.GetComponent<Collider>().Raycast(head.Gaze, out hit, Mathf.Infinity);
		bool size2LA = size2.GetComponent<Collider>().Raycast(head.Gaze, out hit, Mathf.Infinity);
		bool size3LA = size3.GetComponent<Collider>().Raycast(head.Gaze, out hit, Mathf.Infinity);
		bool size4LA = size4.GetComponent<Collider>().Raycast(head.Gaze, out hit, Mathf.Infinity);
		bool size5LA = size5.GetComponent<Collider>().Raycast(head.Gaze, out hit, Mathf.Infinity);

		if (Cardboard.SDK.CardboardTriggered && size1LA) {
			circle.radius = 1;
			Debug.Log("smamlsls");
		}
		if (Cardboard.SDK.CardboardTriggered && size2LA) {
			circle.radius = 3;
		}
		if (Cardboard.SDK.CardboardTriggered && size3LA) {
			circle.radius = 5;
		}
		if (Cardboard.SDK.CardboardTriggered && size4LA) {
			circle.radius = 10;
		}
		if (Cardboard.SDK.CardboardTriggered && size5LA) {
			circle.radius = 20;
		}
	}
}
