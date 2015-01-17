using UnityEngine;
using System.Collections;

[RequireComponent(typeof(Collider))]
public class Paint : MonoBehaviour {
	private CardboardHead head;
	private Vector3 startingPosition;
	public Texture2D texture, texture1, texture2, texture3, texture4, texture5; 
	//public Texture2D[] textures = new Texture2D[6]; 
	private GameObject[] planes;
	private int NUM_PLANES = 6; 
	private MeshCollider meshCollider; 
	private Color penColor = Color.black; 

	void Start() {
		planes = new GameObject[NUM_PLANES];
		//textures = new Texture2D[NUM_PLANES]; 

		// Create plane canvas programmatically.
		// Create 6 textures.
		for (int i = 0; i < NUM_PLANES; i++) {
			planes[i] = GameObject.CreatePrimitive(PrimitiveType.Plane);
			//textures[i] = Texture2D.blackTexture; 
		}
		// Plane in front of camera.
		planes[0].transform.position = new Vector3(0, 5.6F, 5F);
		planes[0].transform.Rotate(270, 0, 0);
		meshCollider = planes[0].AddComponent("MeshCollider") as MeshCollider;
		// Plane behind camera.
		planes[1].transform.position = new Vector3(0, 5.6F, -5F);
		planes[1].transform.Rotate(90, 0, 0);
		meshCollider = planes[1].AddComponent("MeshCollider") as MeshCollider;
		// Planes on the side of the camera.
		planes[2].transform.position = new Vector3(-5F, 5.6F, 0);
		planes[2].transform.Rotate(0, 0, 270);
		meshCollider = planes[2].AddComponent("MeshCollider") as MeshCollider;
		planes[3].transform.position = new Vector3(5F, 5.6F, 0);
		planes[3].transform.Rotate(0, 0, 90);
		meshCollider = planes[3].AddComponent("MeshCollider") as MeshCollider;
		// Plane below camera.
		planes[4].transform.position = new Vector3(0, 0.6F, 0);
		planes[4].transform.Rotate(0, 0, 0);
		meshCollider = planes[4].AddComponent("MeshCollider") as MeshCollider;
		// Plane above camera.
		planes[5].transform.position = new Vector3(0, 5.6F, 0);
		planes[5].transform.Rotate(0, 0, 180);
		meshCollider = planes[5].AddComponent("MeshCollider") as MeshCollider;

		head = Camera.main.GetComponent<StereoController>().Head;
		startingPosition = transform.localPosition;
		CardboardGUI.IsGUIVisible = true;
		CardboardGUI.onGUICallback += this.OnGUI;
		
		// Apply texture to plane.
		planes[0].renderer.material.mainTexture = texture;
		planes[1].renderer.material.mainTexture = texture1;
		planes[2].renderer.material.mainTexture = texture2;
		planes[3].renderer.material.mainTexture = texture3;
		planes[4].renderer.material.mainTexture = texture4;
		planes[5].renderer.material.mainTexture = texture5;

		texture.Apply();
		texture1.Apply();
		texture2.Apply();
		texture3.Apply();
		texture4.Apply();
		texture5.Apply();

		// Save file image.
		byte[] image = texture.EncodeToJPG();
		//File.WriteAllBytes("image.jpg", image);
	}
	
	void Update() {
		// Detect whether user is looking at camera. 
		RaycastHit hit;
		bool isLookedAt = planes[0].GetComponent<Collider>().Raycast(head.Gaze, out hit, Mathf.Infinity);
		// If the plane is being looked out, determine exactly which pixel on the texture is being looked at.
		if (isLookedAt) {
			Renderer renderer = hit.collider.renderer;
			MeshCollider meshCollider = hit.collider as MeshCollider;
			if (renderer == null || renderer.sharedMaterial == null || renderer.sharedMaterial.mainTexture == null || meshCollider == null)
				return;

			// "Paint" the pixel.
			Vector2 pixelUV = hit.textureCoord;
			pixelUV.x *= texture.width;
			pixelUV.y *= texture.height;
			Circle(texture, (int)pixelUV.x, (int)pixelUV.y, 5, penColor);
			texture.Apply();
		}

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
		if (isLookedAt) {
			Debug.Log ("Is looked at!"); 
			Renderer renderer = hit.collider.renderer;
			MeshCollider meshCollider = hit.collider as MeshCollider;
			if (renderer == null || renderer.sharedMaterial == null || renderer.sharedMaterial.mainTexture == null || meshCollider == null) {
				Debug.Log ("Not working");
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
}


