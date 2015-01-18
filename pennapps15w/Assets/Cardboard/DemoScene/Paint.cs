using UnityEngine;
using System.Collections;
using System.IO; 

[RequireComponent(typeof(Collider))]
public class Paint : MonoBehaviour {
	private CardboardHead head;
	private Vector3 startingPosition;
	public Texture2D texture0, texture1, texture2, texture3, texture4, texture5; 
	private GameObject[] planes;
	private MeshCollider meshCollider; 

	public int radius = 5;

	private Color penColor = Color.black;
	private bool isPenDown = false;
	private GameObject lightGameObject; 

	private int NUM_PLANES = 6;
	private float PLANE_WIDTH = 15F; 
	public  static float DISTANCE_FROM_GROUND = 0.6F; 

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
	
	public Texture2D texXS;
	public Texture2D texS;
	public Texture2D texM;
	public Texture2D texL;
	public Texture2D texXL;
	
	public Texture2D texImport;
	public Texture2D texExport;
	public Texture2D texNew;

	
	private bool clickedThisRound;
	private bool isClicked;

	public Texture2D texture; // palette texture
	public GameObject palette; 

	void Start() {
		AddButtons();
		planes = new GameObject[NUM_PLANES];
		clickedThisRound = false; 

		ClearTextures();
		PositionLight(); 
		StartColorPalette (); 

		// Create plane canvas programmatically.
		for (int i = 0; i < NUM_PLANES; i++) {
			planes[i] = GameObject.CreatePrimitive(PrimitiveType.Plane);
			planes[i].transform.localScale = new Vector3(3, 3, 3);
		}

		// Plane in front of camera.
		planes[0].transform.position = new Vector3(0, PLANE_WIDTH + DISTANCE_FROM_GROUND, PLANE_WIDTH);
		planes[0].transform.Rotate(270, 0, 0);
		meshCollider = planes[0].AddComponent("MeshCollider") as MeshCollider;
		 
		// Plane behind camera.
		planes[1].transform.position = new Vector3(0, PLANE_WIDTH + DISTANCE_FROM_GROUND, -PLANE_WIDTH);
		planes[1].transform.Rotate(90, 0, 0);
		meshCollider = planes[1].AddComponent("MeshCollider") as MeshCollider;
		// Planes on the side of the camera.
		planes[2].transform.position = new Vector3(-PLANE_WIDTH, PLANE_WIDTH + DISTANCE_FROM_GROUND, 0);
		planes[2].transform.Rotate(0, 0, 270);
		meshCollider = planes[2].AddComponent("MeshCollider") as MeshCollider;
		planes[3].transform.position = new Vector3(PLANE_WIDTH, PLANE_WIDTH + DISTANCE_FROM_GROUND, 0);
		planes[3].transform.Rotate(0, 0, 90);
		meshCollider = planes[3].AddComponent("MeshCollider") as MeshCollider;
		// Plane below camera.
		planes[4].transform.position = new Vector3(0, DISTANCE_FROM_GROUND, 0);
		planes[4].transform.Rotate(0, 0, 0);
		meshCollider = planes[4].AddComponent("MeshCollider") as MeshCollider;
		// Plane above camera.
		planes[5].transform.position = new Vector3(0, PLANE_WIDTH + DISTANCE_FROM_GROUND, 0);
		planes[5].transform.Rotate(0, 0, 180);
		meshCollider = planes[5].AddComponent("MeshCollider") as MeshCollider;

		head = Camera.main.GetComponent<StereoController>().Head;
		startingPosition = transform.localPosition;
		CardboardGUI.IsGUIVisible = true;
		CardboardGUI.onGUICallback += this.OnGUI;
		
		// Apply textures to planes.
		planes[0].renderer.material.mainTexture = texture0;
		planes[1].renderer.material.mainTexture = texture1;
		planes[2].renderer.material.mainTexture = texture2;
		planes[3].renderer.material.mainTexture = texture3;
		planes[4].renderer.material.mainTexture = texture4;
		planes[5].renderer.material.mainTexture = texture5;
		texture0.Apply();
		texture1.Apply();
		texture2.Apply();
		texture3.Apply();
		texture4.Apply();
		texture5.Apply();

		ColorToolBar = GameObject.Find ("ColorToolBar");
		colorPicker = ColorToolBar.GetComponent<CreatePicker>();

		//byte[] image = texture0.EncodeToPNG();
		//File.WriteAllBytes(Application.dataPath + "/../image.jpg", image);
	}
	
	void Update() {
		UpdateClick (); 
		if (!HaveButtonsBeenClicked() && !HasColorPaletteBeenClicked() && HasBeenClicked()) {
			isPenDown = !isPenDown; 
			if (!isPenDown) {
				// Save textures as PNGs.
				CreatePng(texture0, 0); 
				CreatePng(texture1, 1); 
				CreatePng(texture2, 2); 
				CreatePng(texture3, 3); 
				CreatePng(texture4, 4);
				CreatePng(texture5, 5); 
			}
		}

		// Detect whether user is looking at camera. 
		DetectLookAt(0, texture0);
		DetectLookAt(1, texture1); 
		DetectLookAt(2, texture2); 
		DetectLookAt(3, texture3); 
		DetectLookAt(4, texture4); 
		DetectLookAt(5, texture5); 
	}

	void AddButtons() {
		size1 = GameObject.CreatePrimitive (PrimitiveType.Plane);
		size1.transform.localScale = new Vector3 (0.0625f, 0.0625f, 0.0625f);
		size1.transform.Translate(-3, DISTANCE_FROM_GROUND+0.625f*8, 7);
		size1.transform.Rotate (270, 0, 0);
		size1.renderer.material.mainTexture = texXS;
		
		size2 = GameObject.CreatePrimitive (PrimitiveType.Plane);
		size2.transform.localScale = new Vector3 (0.0625f, 0.0625f, 0.0625f);
		size2.transform.Translate(-3, DISTANCE_FROM_GROUND+0.625f*7, 7);
		size2.transform.Rotate (270, 0, 0);
		size2.renderer.material.mainTexture = texS;
		
		size3 = GameObject.CreatePrimitive (PrimitiveType.Plane);
		size3.transform.localScale = new Vector3 (0.0625f, 0.0625f, 0.0625f);
		size3.transform.Translate(-3, DISTANCE_FROM_GROUND+0.625f*6, 7);
		size3.transform.Rotate (270, 0, 0);
		size3.renderer.material.mainTexture = texM;
		
		size4 = GameObject.CreatePrimitive (PrimitiveType.Plane);
		size4.transform.localScale = new Vector3 (0.0625f, 0.0625f, 0.0625f);
		size4.transform.Translate(-3, DISTANCE_FROM_GROUND+0.625f*5, 7);
		size4.transform.Rotate (270, 0, 0);
		size4.renderer.material.mainTexture = texL;
		
		size5 = GameObject.CreatePrimitive (PrimitiveType.Plane);
		size5.transform.localScale = new Vector3 (0.0625f, 0.0625f, 0.0625f);
		size5.transform.Translate(-3, DISTANCE_FROM_GROUND+0.625f*4, 7);
		size5.transform.Rotate (270, 0, 0);
		size5.renderer.material.mainTexture = texXL;
		
		import = GameObject.CreatePrimitive (PrimitiveType.Plane);
		import.transform.localScale = new Vector3 (0.0625f, 0.0625f, 0.0625f);
		import.transform.Translate(-3, DISTANCE_FROM_GROUND+0.625f*3, 7);
		import.transform.Rotate (270, 0, 0);
		import.renderer.material.mainTexture = texImport;
		
		export = GameObject.CreatePrimitive (PrimitiveType.Plane);
		export.transform.localScale = new Vector3 (0.0625f, 0.0625f, 0.0625f);
		export.transform.Translate(-3, DISTANCE_FROM_GROUND+0.625f*2, 7);
		export.transform.Rotate (270, 0, 0);
		export.renderer.material.mainTexture = texExport;
		
		newDoc = GameObject.CreatePrimitive (PrimitiveType.Plane);
		newDoc.transform.localScale = new Vector3 (0.0625f, 0.0625f, 0.0625f);
		newDoc.transform.Translate(-3, DISTANCE_FROM_GROUND+0.625f*1, 7);
		newDoc.transform.Rotate (270, 0, 0);
		newDoc.renderer.material.mainTexture = texNew;
		
		texXS.Apply ();
		texS.Apply ();
		texM.Apply ();
		texL.Apply ();
		texXL.Apply ();
		
		texImport.Apply();
		texExport.Apply();
		texNew.Apply();
	}

	bool HaveButtonsBeenClicked () {
		GameObject plane = GameObject.Find ("GroundPlane");
		Paint circle = plane.GetComponent<Paint>();
		
		bool size1LA = size1.GetComponent<Collider>().Raycast(head.Gaze, out hit, Mathf.Infinity);
		bool size2LA = size2.GetComponent<Collider>().Raycast(head.Gaze, out hit, Mathf.Infinity);
		bool size3LA = size3.GetComponent<Collider>().Raycast(head.Gaze, out hit, Mathf.Infinity);
		bool size4LA = size4.GetComponent<Collider>().Raycast(head.Gaze, out hit, Mathf.Infinity);
		bool size5LA = size5.GetComponent<Collider>().Raycast(head.Gaze, out hit, Mathf.Infinity);

		bool importLA = import.GetComponent<Collider>().Raycast(head.Gaze, out hit, Mathf.Infinity);
		bool exportLA = export.GetComponent<Collider>().Raycast(head.Gaze, out hit, Mathf.Infinity);
		bool newLA = newDoc.GetComponent<Collider>().Raycast(head.Gaze, out hit, Mathf.Infinity);

		if (HasBeenClicked() && size1LA) {
			circle.radius = 1;
			return true;
		}
		if (HasBeenClicked() && size2LA) {
			circle.radius = 3;
			return true;
		}
		if (HasBeenClicked() && size3LA) {
			circle.radius = 5;
			return true;
		}
		if (HasBeenClicked() && size4LA) {
			circle.radius = 10;
			return true;
		}
		if (HasBeenClicked() && size5LA) {
			circle.radius = 20;
			return true;
		}

		if (HasBeenClicked() && newLA) {
			ClearTextures();
			return true;
		}

		if (HasBeenClicked() && importLA) {
			// import images
			return true;
		}

		if (HasBeenClicked() && exportLA) {
			// export images
			return true;
		}

		return false;
	}

	void DetectLookAt(int index, Texture2D tex) {
		RaycastHit hit;
		bool isLookedAt = planes[index].GetComponent<Collider>().Raycast(head.Gaze, out hit, Mathf.Infinity);

		// If the plane is being looked out, determine exactly which pixel on the texture is being looked at.
		if (isLookedAt && isPenDown) {
			Renderer renderer = hit.collider.renderer;
			MeshCollider meshCollider = hit.collider as MeshCollider;
			if (renderer == null || renderer.sharedMaterial == null || renderer.sharedMaterial.mainTexture == null || meshCollider == null) {
				return;
			}

			// "Paint" the pixel.
			Vector2 pixelUV = hit.textureCoord;
			pixelUV.x *= tex.width;
			pixelUV.y *= tex.height;
			Circle(tex, (int)pixelUV.x, (int)pixelUV.y, radius, colorPicker.currentColor);
			tex.Apply();
		}
	}
	
	void OnGUI() {
		if (!CardboardGUI.OKToDraw(this)) {
			return;
		}
		if (GUI.Button(new Rect(50, 50, 200, 50), "Reset")) {
			transform.localPosition = startingPosition;
		}
	}

	void ClearTextures() {
		for (int i = 0; i < texture0.width; i++) {
			for (int j = 0; j < texture0.height; j++) {
				texture0.SetPixel(i, j, Color.white);
				texture1.SetPixel(i, j, Color.white);
				texture2.SetPixel(i, j, Color.white);
				texture3.SetPixel(i, j, Color.white);
				texture4.SetPixel(i, j, Color.white);
				texture5.SetPixel(i, j, Color.white);
			}
		}

		texture0.Apply ();
		texture1.Apply ();
		texture2.Apply ();
		texture3.Apply ();
		texture4.Apply ();
		texture5.Apply ();
	}

	void CreatePng(Texture2D texture, int i) {
		byte[] image = texture.EncodeToPNG();
		File.WriteAllBytes(Application.dataPath + "/../image" + i + ".jpg", image);
	}

	// Draw a filled circle on a texture.
	// http://answers.unity3d.com/questions/590469/drawing-a-solid-circle-onto-texture.html
	void Circle(Texture2D tex, int cx, int cy, int r, Color col) {
		int x, y, px, nx, py, ny, d;
		
		for (x = 0; x <= r; x++) {
			d = (int)Mathf.Ceil(Mathf.Sqrt(r * r - x * x));
			for (y = 0; y <= d; y++) {
				px = cx + x;
				nx = cx - x;
				py = cy + y;
				ny = cy - y;
				
				tex.SetPixel(px, py, col);
				tex.SetPixel(nx, py, col);
				
				tex.SetPixel(px, ny, col);
				tex.SetPixel(nx, ny, col);
			}
		}    
	}

	void PositionLight() {
		lightGameObject = GameObject.Find("Point light");
		if (lightGameObject) {
			lightGameObject.transform.position = new Vector3(0, 7.5F, 0); 
			lightGameObject.light.range = 1200F; 
			lightGameObject.light.intensity = 0.5F; 
			//lightGameObject.light.color = Color.blue;
		}
	}
	
	void UpdateClick() {
		if (Cardboard.SDK.CardboardTriggered) {
			if (!clickedThisRound) {
				clickedThisRound = true;
				isClicked = true;
			} 
			else {
				isClicked = false;
			}
		} 
		else {
			clickedThisRound = false;
			isClicked = false;
		}
	}

	bool HasBeenClicked() {
		return isClicked; 
	}

	void PickColor() {
		Renderer renderer = hit.collider.renderer;
		MeshCollider meshCollider = hit.collider as MeshCollider;
		if (renderer == null || renderer.sharedMaterial == null || renderer.sharedMaterial.mainTexture == null || meshCollider == null)
			return;
		
		Vector2 pixelUV = hit.textureCoord;
		pixelUV.x *= texture.width;
		pixelUV.y *= texture.height;
		
		Debug.Log("pixel color " + colorPicker.currentColor);
		colorPicker.currentColor = texture.GetPixel((int)pixelUV.x, (int)pixelUV.y);
	}

	void StartColorPalette() {
		// color palette plane
		palette = GameObject.CreatePrimitive(PrimitiveType.Plane);
		palette.transform.localScale = new Vector3 (0.5f, 0.5f, 0.5f);
		palette.transform.Translate(0, 2.5f, 7.01f);
		palette.transform.Rotate(270, 0, 0);
		meshCollider = palette.AddComponent("MeshCollider") as MeshCollider;

		// Apply texture to palette plane
		palette.renderer.material.mainTexture = texture;
		texture.Apply();
		
		ColorToolBar = GameObject.Find ("ColorToolBar");
		colorPicker = ColorToolBar.GetComponent<CreatePicker>();
	}

	bool HasColorPaletteBeenClicked() {
		bool paletteLookedAt = palette.GetComponent<Collider>().Raycast(head.Gaze, out hit, Mathf.Infinity);
		Debug.Log ("palette seen " + paletteLookedAt);
		if (HasBeenClicked() && paletteLookedAt) {
			PickColor();
		}
		return paletteLookedAt; 
	}

}


