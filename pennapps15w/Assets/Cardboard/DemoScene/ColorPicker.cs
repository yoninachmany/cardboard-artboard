// Copyright 2014 Google Inc. All rights reserved.
//
// Licensed under the Apache License, Version 2.0 (the "License");
// you may not use this file except in compliance with the License.
// You may obtain a copy of the License at
//
//     http://www.apache.org/licenses/LICENSE-2.0
//
// Unless required by applicable law or agreed to in writing, software
// distributed under the License is distributed on an "AS IS" BASIS,
// WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
// See the License for the specific language governing permissions and
// limitations under the License.

using UnityEngine;
using System.Collections;

[RequireComponent(typeof(Collider))]
public class ColorPicker : MonoBehaviour {
  	private CardboardHead head;
  	private Vector3 startingPosition;
	public Texture2D texture; 
	public GameObject palette; 
	private MeshCollider meshCollider; 
	RaycastHit hit;
	
	GameObject ColorToolBar;
	CreatePicker colorPicker;

  void Start() {
	// color palette plane
	palette = GameObject.CreatePrimitive(PrimitiveType.Plane);
	palette.transform.localScale = new Vector3 (0.5f, 0.5f, 0.5f);
	palette.transform.Translate(0, 2.5f, 7.01f);
	palette.transform.Rotate(270, 0, 0);
	meshCollider = palette.AddComponent("MeshCollider") as MeshCollider;

	head = Camera.main.GetComponent<StereoController>().Head;
    startingPosition = transform.localPosition;
    CardboardGUI.IsGUIVisible = true;
    CardboardGUI.onGUICallback += this.OnGUI;

	// Apply texture to palette plane
	palette.renderer.material.mainTexture = texture;
	texture.Apply();

	ColorToolBar = GameObject.Find ("ColorToolBar");
	colorPicker = ColorToolBar.GetComponent<CreatePicker>();
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

  void Update() {
	bool paletteLookedAt = palette.GetComponent<Collider>().Raycast(head.Gaze, out hit, Mathf.Infinity);
	Debug.Log ("palette seen " + paletteLookedAt);
	if (Cardboard.SDK.CardboardTriggered && paletteLookedAt) {
		Debug.Log("HERE " + colorPicker.currentColor);
		PickColor();
	}

	//colorPicker.cubeCurr.renderer.material.color = colorPicker.currentColor;
  }

  void OnGUI() {
    if (!CardboardGUI.OKToDraw(this)) {
      return;
    }
  }
}
