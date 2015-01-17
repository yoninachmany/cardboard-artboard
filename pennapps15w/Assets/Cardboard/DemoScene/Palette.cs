using UnityEngine;
using System.Collections;

public class Palette : MonoBehaviour {

	// Use this for initialization
	void Start () {

	}
	
	// Update is called once per frame
	void Update () {
		RaycastHit hit;
		//Debug.Log (GetComponent<Collider>()); 
		bool isLookedAt = GetComponent<Collider>().Raycast(head.Gaze, out hit, Mathf.Infinity);
		if (isLookedAt) {
			//Debug.Log ("Looked at!"); 
			Renderer renderer = hit.collider.renderer;
			MeshCollider meshCollider = hit.collider as MeshCollider;
			if (renderer == null || renderer.sharedMaterial == null || renderer.sharedMaterial.mainTexture == null || meshCollider == null)
				return;
			
			Vector2 pixelUV = hit.textureCoord;
			//Debug.Log (pixelUV.x + " " + pixelUV.y); 
			pixelUV.x *= texture.width;
			pixelUV.y *= texture.height;
			Circle(texture, (int)pixelUV.x, (int)pixelUV.y, 5, Color.black);
			//texture.SetPixel((int)pixelUV.x, (int)pixelUV.y, Color.white);
			texture.Apply();
		}

	}
}
