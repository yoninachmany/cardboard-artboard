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
	private Color penColor = Color.black;
	private bool isPenDown = false;
	private GameObject lightGameObject; 

	private int NUM_PLANES = 6;
	private float PLANE_WIDTH = 15F; 
	private float DISTANCE_FROM_GROUND = 0.6F; 

	void Start() {
		planes = new GameObject[NUM_PLANES];

		ClearTextures();
		PositionLight(); 

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

		//byte[] image = texture0.EncodeToPNG();
		//File.WriteAllBytes(Application.dataPath + "/../image.jpg", image);
	}
	
	void Update() {
		if (Cardboard.SDK.CardboardTriggered) {
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

		/*RaycastHit hit;
		bool isLookedAt = GetComponent<Collider>().Raycast(head.Gaze, out hit, Mathf.Infinity);*/
		/*GetComponent<Renderer>().material.color = isLookedAt ? Color.green : Color.red;
		if (Cardboard.SDK.CardboardTriggered && isLookedAt) {
			// Teleport randomly.
			Vector3 direction = Random.onUnitSphere;
			direction.y = Mathf.Clamp(direction.y, 0.5f, 1f);
			float distance = 2 * Random.value + 1.5f;
			transform.localPosition = direction * distance;
		}*/
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
			Circle(tex, (int)pixelUV.x, (int)pixelUV.y, 5, penColor);
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
			lightGameObject.transform.position = new Vector3(0, 10.5F, 0); 
			lightGameObject.light.range = 78.3F; 
			lightGameObject.light.intensity = 0.81F; 
			//lightGameObject.light.color = Color.blue;
		}
	}
}


