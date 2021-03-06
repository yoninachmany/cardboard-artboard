using UnityEngine;
using System.Collections;
using System.Threading;
using System.Text;
using System.IO; 

[RequireComponent(typeof(Collider))]
public class Paint : MonoBehaviour {
	private CardboardHead head;
	private Vector3 startingPosition;
	public Texture2D texture_o;
	public Texture2D texture0, texture1, texture2, texture3, texture4, texture5; 
	private GameObject[] planes;
	private MeshCollider meshCollider; 

	public int radius = 5;

	private bool isPenDown = false;
	private GameObject lightGameObject; 
	private GameObject cameraGameObject;

	private int NUM_PLANES = 6;
	private float PLANE_WIDTH = 15F; 
	public  static float DISTANCE_FROM_GROUND = 0.6F; 
	float OFFSET = 2.5f;
	float MULT = 0.55f;
	float SCALE = 0.055f;

	RaycastHit hit;
	
	GameObject ColorToolBar;
	CreatePicker colorPicker;
	
	GameObject size1;
	GameObject size2;
	GameObject size3;
	GameObject size4;
	GameObject size5;

	GameObject toggle;
	GameObject import;
	GameObject export;
	GameObject newDoc;
	
	public Texture2D texXS;
	public Texture2D texS;
	public Texture2D texM;
	public Texture2D texL;
	public Texture2D texXL;

	public Texture2D texEye;
	public Texture2D texMag;

	Color32[] togEye32;
	Color32[] togMag32;
	
	public Texture2D texImport;
	public Texture2D texExport;
	public Texture2D texNew;
	bool isEye;
	
	private bool clickedThisRound;
	private bool isClicked;

	public Texture2D texture; // palette texture
	public GameObject palette; 

	// Eye interaction. 
	private float timeLeft = 2.0f;
	private float thresh = .01f;
	private float stillTimeLeft = 2.0f;
	public AudioClip sound;
	private AudioSource source;
	public Vector3 lastVector;
	public Vector3 currVector;
	private bool isIControlOn = false; /* iControl is initially off. */ 
	
	void Start() {
		AddButtons();
		planes = new GameObject[NUM_PLANES];
		clickedThisRound = false; 
		isEye = false;

		Color32[] texO = texture_o.GetPixels32 ();
		texture2 = new Texture2D (texture_o.width, texture_o.height);
		ClearTextures();
		texture2.SetPixels32(texO);
		texture2.Apply ();

		PositionLight(); 
		PositionCamera();
		StartColorPalette(); 
		StartAudio(); 

		// Create plane canvas programmatically.
		for (int i = 0; i < NUM_PLANES; i++) {
			planes[i] = GameObject.CreatePrimitive(PrimitiveType.Plane);
			planes[i].transform.localScale = new Vector3(3, 3, 3);
		}

		togEye32 = texEye.GetPixels32 ();
		togMag32 = texMag.GetPixels32 ();



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
		UpdateClick();
		UpdateAudio(); 
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
		size1.transform.localScale = new Vector3 (SCALE, SCALE, SCALE);
		size1.transform.Translate(-3, DISTANCE_FROM_GROUND+OFFSET+MULT*9, 7.5f);
		size1.transform.Rotate (270, 0, 0);
		size1.renderer.material.mainTexture = texXS;
		
		size2 = GameObject.CreatePrimitive (PrimitiveType.Plane);
		size2.transform.localScale = new Vector3 (SCALE, SCALE, SCALE);
		size2.transform.Translate(-3, DISTANCE_FROM_GROUND+OFFSET+MULT*8, 7.5f);
		size2.transform.Rotate (270, 0, 0);
		size2.renderer.material.mainTexture = texS;
		
		size3 = GameObject.CreatePrimitive (PrimitiveType.Plane);
		size3.transform.localScale = new Vector3 (SCALE, SCALE, SCALE);
		size3.transform.Translate(-3, DISTANCE_FROM_GROUND+OFFSET+MULT*7, 7.5f);
		size3.transform.Rotate (270, 0, 0);
		size3.renderer.material.mainTexture = texM;
		
		size4 = GameObject.CreatePrimitive (PrimitiveType.Plane);
		size4.transform.localScale = new Vector3 (SCALE, SCALE, SCALE);
		size4.transform.Translate(-3, DISTANCE_FROM_GROUND+OFFSET+MULT*6, 7.5f);
		size4.transform.Rotate (270, 0, 0);
		size4.renderer.material.mainTexture = texL;
		
		size5 = GameObject.CreatePrimitive (PrimitiveType.Plane);
		size5.transform.localScale = new Vector3 (SCALE, SCALE, SCALE);
		size5.transform.Translate(-3, DISTANCE_FROM_GROUND+OFFSET+MULT*5, 7.5f);
		size5.transform.Rotate (270, 0, 0);
		size5.renderer.material.mainTexture = texXL;

		toggle = GameObject.CreatePrimitive (PrimitiveType.Plane);
		toggle.transform.localScale = new Vector3 (SCALE, SCALE, SCALE);
		toggle.transform.Translate(-3, DISTANCE_FROM_GROUND+OFFSET+MULT*4, 7.5f);
		toggle.transform.Rotate (270, 0, 0);
		toggle.renderer.material.mainTexture = texMag;

		import = GameObject.CreatePrimitive (PrimitiveType.Plane);
		import.transform.localScale = new Vector3 (SCALE, SCALE, SCALE);
		import.transform.Translate(-3, DISTANCE_FROM_GROUND+OFFSET+MULT*3, 7.5f);
		import.transform.Rotate (270, 0, 0);
		import.renderer.material.mainTexture = texImport;
		
		export = GameObject.CreatePrimitive (PrimitiveType.Plane);
		export.transform.localScale = new Vector3 (SCALE, SCALE, SCALE);
		export.transform.Translate(-3, DISTANCE_FROM_GROUND+OFFSET+MULT*2, 7.5f);
		export.transform.Rotate (270, 0, 0);
		export.renderer.material.mainTexture = texExport;
		
		newDoc = GameObject.CreatePrimitive (PrimitiveType.Plane);
		newDoc.transform.localScale = new Vector3 (SCALE, SCALE, SCALE);
		newDoc.transform.Translate(-3, DISTANCE_FROM_GROUND+OFFSET+MULT*1, 7.5f);
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

		bool toggleLA = toggle.GetComponent<Collider>().Raycast(head.Gaze, out hit, Mathf.Infinity);
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

		if (HasBeenClicked() && toggleLA) {
			if(isEye) {
				toggle.renderer.material.mainTexture = texMag;
				texMag.Apply();
				isEye = false;
				isIControlOn = false; 
			} else {
				toggle.renderer.material.mainTexture = texEye;
				texEye.Apply();
				isEye = true;
				isIControlOn = true; 
			}
			return true;
		}
		
		if (HasBeenClicked() && newLA) {
			ClearTextures();
			return true;
		}

		if (HasBeenClicked() && importLA) {
			DownloadRandomAndSet();
			return true;
		}

		if (HasBeenClicked() && exportLA) {
			Upload();
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
	}

	void ClearTextures() {
		for (int i = 0; i < texture0.width; i++) {
			for (int j = 0; j < texture0.height; j++) {
				texture0.SetPixel(i, j, Color.white);
				texture1.SetPixel(i, j, Color.white);
				texture3.SetPixel(i, j, Color.white);
				texture4.SetPixel(i, j, Color.white);
				texture5.SetPixel(i, j, Color.white);
			}
		}

		for (int i = 0; i < texture2.width; i++) {
			for (int j = 0; j < texture2.height; j++) {
				texture2.SetPixel(i, j, Color.white);
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
		File.WriteAllBytes(Application.dataPath + "/../image" + i + ".png", image);
	}

	// Draw a filled circle on a texture.
	// http://answers.unity3d.com/questions/590469/drawing-a-solid-circle-onto-texture.html
	void Circle(Texture2D tex, int cx, int cy, int r, Color col) {
		int x, y, px, nx, py, ny, d;
		
		for (x = 0; x <= r; x++) {
			d = (int)Mathf.Ceil(Mathf.Sqrt(r * r - x * x));
			for (y = 0; y <= d; y++) {
				nx = cx - x;
				px = cx + x;
				py = cy + y;
				ny = cy - y;
				
				tex.SetPixel(px, py, col);
				tex.SetPixel(nx, py, col);
				
				tex.SetPixel(px, ny, col);
				tex.SetPixel(nx, ny, col);
			}
		}    
	}

	void Upload() {
		JSONObject parentBody = new JSONObject (JSONObject.Type.ARRAY);
		//JSONObject body0 = new JSONObject (JSONObject.Type.OBJECT);
		//body0.AddField("key0", System.Convert.ToBase64String(texture0.EncodeToPNG()));
		parentBody.Add (System.Convert.ToBase64String(texture0.EncodeToPNG()));
		parentBody.Add (System.Convert.ToBase64String(texture1.EncodeToPNG()));
		parentBody.Add (System.Convert.ToBase64String(texture2.EncodeToPNG()));
		parentBody.Add (System.Convert.ToBase64String(texture3.EncodeToPNG()));
		parentBody.Add (System.Convert.ToBase64String(texture4.EncodeToPNG()));
		parentBody.Add (System.Convert.ToBase64String(texture5.EncodeToPNG()));
//		Debug.Log (System.Convert.ToBase64String (texture0.EncodeToPNG ()));
//		Debug.Log (System.Convert.ToBase64String (texture1.EncodeToPNG ()));
//		Debug.Log (System.Convert.ToBase64String (texture2.EncodeToPNG ()));
//		Debug.Log (System.Convert.ToBase64String (texture3.EncodeToPNG ()));
//		Debug.Log (System.Convert.ToBase64String (texture4.EncodeToPNG ()));
//		Debug.Log (System.Convert.ToBase64String (texture5.EncodeToPNG ()));
//		Debug.Log ("separate");

		//string s = body.print();
		string s = parentBody.print ();
		//Debug.Log (s);
		
		Encoding encoding = new System.Text.UTF8Encoding();
		Hashtable postHeader = new Hashtable();
		
		postHeader.Add("Content-Type", "text/json");
		postHeader.Add("Content-Length", s.Length);
		
		WWW poster = new WWW("https://cardboardartboardimg.firebaseio.com/.json", encoding.GetBytes(s), postHeader);
		while (poster.uploadProgress != 1) {
			Thread.Sleep (1);
		}
		//Debug.Log(poster.error);
	}

	void DownloadRandomAndSet() {
		WWW get = new WWW("https://cardboardartboardimg.firebaseio.com/.json");
		while (!get.isDone) {
			Thread.Sleep(1);
		}
		JSONObject json = new JSONObject(get.text);
		int index = Random.Range(0,json.list.Count);
		JSONObject child = (JSONObject)json.list[index];
//		Encoding encoding = new System.Text.UTF8Encoding();
//		encoding.GetBytes(child.list [0].print());

		string str1 = child.list [0].print ();
		str1 = str1.Substring (1,str1.Length-2);
		string str2 = child.list [1].print ();
		str2 = str2.Substring (1,str2.Length-2);
		string str3 = child.list [2].print ();
		str3 = str3.Substring (1,str3.Length-2);
		string str4 = child.list [3].print ();
		str4 = str4.Substring (1,str4.Length-2);
		string str5 = child.list [4].print ();
		str5 = str5.Substring (1,str5.Length-2);
		string str6 = child.list [5].print ();
		str6 = str6.Substring (1,str6.Length-2);
//		Debug.Log (str1);
//		Debug.Log (str2);
//		Debug.Log (str3);
//		Debug.Log (str4);
//		Debug.Log (str5);
//		Debug.Log (str6);

		texture0.LoadImage (System.Convert.FromBase64String (str1));
		texture1.LoadImage (System.Convert.FromBase64String (str2));
		texture2.LoadImage (System.Convert.FromBase64String (str3));
		texture3.LoadImage (System.Convert.FromBase64String (str4));
		texture4.LoadImage (System.Convert.FromBase64String (str5));
		texture5.LoadImage (System.Convert.FromBase64String (str6));

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

	void PositionCamera() {
		cameraGameObject = GameObject.Find ("CardboardMain");
		if (cameraGameObject) {
			cameraGameObject.transform.position = new Vector3(0, 4.0f, 0);
		}
		cameraGameObject.transform.Rotate (0, -90, 0);
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
		colorPicker.currentColor = texture.GetPixel((int)pixelUV.x, (int)pixelUV.y);
	}

	void StartColorPalette() {
		// color palette plane
		palette = GameObject.CreatePrimitive(PrimitiveType.Plane);
		palette.transform.localScale = new Vector3 (0.5f, 0.5f, 0.5f);
		palette.transform.Translate(0, OFFSET+3.2f, 7.5f);
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
		if (HasBeenClicked() && paletteLookedAt) {
			PickColor();
		}
		return paletteLookedAt; 
	}

	void StartAudio() {
		source = GetComponent<AudioSource> ();
		head = Camera.main.GetComponent<StereoController>().Head;
		startingPosition = transform.localPosition;
		CardboardGUI.IsGUIVisible = true;
		CardboardGUI.onGUICallback += this.OnGUI;
		lastVector = head.Gaze.direction;
		currVector = head.Gaze.direction;
	}

	bool isStill() {
		if (Vector3.Distance(currVector, lastVector) < thresh) {
			if (stillTimeLeft > 0) {
				stillTimeLeft -= Time.deltaTime;
			}
			if (stillTimeLeft < 0) {
				stillTimeLeft = 2.0f;
				return true;
			}
			return false;
		}
		stillTimeLeft = 2.0f;
		return false;
	}

	void UpdateAudio() {
		lastVector = currVector;
		currVector = head.Gaze.direction;
		if (isStill()) {
			if (isIControlOn) {
				audio.Play();
				isClicked = true;
				Debug.Log("isStill!");
			}
		}
		else {
			if (isIControlOn) {
				isClicked = false;
			}
		}
	}
}


