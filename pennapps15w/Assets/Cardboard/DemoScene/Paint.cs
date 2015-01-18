using UnityEngine;
using System.Collections;

[RequireComponent(typeof(Collider))]
public class Paint : MonoBehaviour {
	private CardboardHead head;
	private Vector3 startingPosition;
	public Texture2D texture; 
	private GameObject plane;
	private MeshCollider meshCollider; 
	public int radius = 5;

	void Start() {
		// Create plane canvas programmatically.
		plane = GameObject.CreatePrimitive(PrimitiveType.Plane);
		plane.transform.position = new Vector3(0, 0, 8);
		plane.transform.Rotate(270, 0, 0);
		meshCollider = plane.AddComponent("MeshCollider") as MeshCollider;

		head = Camera.main.GetComponent<StereoController>().Head;
		startingPosition = transform.localPosition;
		CardboardGUI.IsGUIVisible = true;
		CardboardGUI.onGUICallback += this.OnGUI;
		
		// Apply texture to plane.
		plane.renderer.material.mainTexture = texture;
		texture.Apply ();
	}
	
	void Update() {
		// Detect whether user is looking at camera. 
		RaycastHit hit;
		bool isLookedAt = plane.GetComponent<Collider>().Raycast(head.Gaze, out hit, Mathf.Infinity);
		// If the plane is being looked out, determine exactly which pixel on the texture is being looked at.
		if (isLookedAt) {
			//Debug.Log ("Looked at!"); 
			Renderer renderer = hit.collider.renderer;
			MeshCollider meshCollider = hit.collider as MeshCollider;
			if (renderer == null || renderer.sharedMaterial == null || renderer.sharedMaterial.mainTexture == null || meshCollider == null)
				return;

			// "Paint" the pixel.
			Vector2 pixelUV = hit.textureCoord;
			//Debug.Log (pixelUV.x + " " + pixelUV.y); 
			pixelUV.x *= texture.width;
			pixelUV.y *= texture.height;

			GameObject ColorToolBar = GameObject.Find ("ColorToolBar");
			CreatePicker colorPicker = ColorToolBar.GetComponent<CreatePicker>();

			Circle(texture, (int)pixelUV.x, (int)pixelUV.y, radius, colorPicker.currentColor);
			texture.Apply();
		}

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


