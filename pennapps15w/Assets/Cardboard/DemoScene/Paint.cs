using UnityEngine;
using System.Collections;
using System.Threading;
using System.Text;

[RequireComponent(typeof(Collider))]
public class Paint : MonoBehaviour {
	private CardboardHead head;
	private Vector3 startingPosition;
	public Texture2D texture; 
	private GameObject plane;
	private MeshCollider meshCollider; 

	void Start() {
		// Create plane canvas programmatically.
		plane = GameObject.CreatePrimitive(PrimitiveType.Plane);
		plane.transform.position = new Vector3(0, 0.5F, 3F);
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
			Renderer renderer = hit.collider.renderer;
			MeshCollider meshCollider = hit.collider as MeshCollider;
			if (renderer == null || renderer.sharedMaterial == null || renderer.sharedMaterial.mainTexture == null || meshCollider == null)
				return;

			// "Paint" the pixel if button is down.
//			if (Cardboard.SDK.CardboardTriggered) {
				Vector2 pixelUV = hit.textureCoord;
				pixelUV.x *= texture.width;
				pixelUV.y *= texture.height;
				Circle(texture, (int)pixelUV.x, (int)pixelUV.y, 5, Color.green);
				texture.Apply();
//			}
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

	void Upload(byte[] arr) {
		JSONObject body = new JSONObject (JSONObject.Type.OBJECT);

		body.AddField("test3key", System.Convert.ToBase64String(arr));
		string s = body.print();
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

	byte[] DownloadRandom() {
		WWW get = new WWW("https://cardboardartboardimg.firebaseio.com/.json");
		while (!get.isDone) {
			Thread.Sleep(1);
		}
		JSONObject json = new JSONObject(get.text);
		int index = Random.Range(0,json.list.Count);
		JSONObject child = (JSONObject)json.list[index];
		Encoding encoding = new System.Text.UTF8Encoding();
		return encoding.GetBytes(child.list [0].print());
	}
}


