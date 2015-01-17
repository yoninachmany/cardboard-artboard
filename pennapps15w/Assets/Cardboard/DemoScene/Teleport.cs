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
public class Teleport : MonoBehaviour {
  	private CardboardHead head;
  	private Vector3 startingPosition;
	public Texture2D texture; 
	public GameObject plane; 

  void Start() {
    head = Camera.main.GetComponent<StereoController>().Head;
    startingPosition = transform.localPosition;
    CardboardGUI.IsGUIVisible = true;
    CardboardGUI.onGUICallback += this.OnGUI;
  }

  void Update() {
	RaycastHit hit;
	GameObject ColorToolBar = GameObject.Find ("ColorToolBar");
	CreatePicker colorPicker = ColorToolBar.GetComponent<CreatePicker>();

	bool isLookedAt = GetComponent<Collider>().Raycast(head.Gaze, out hit, Mathf.Infinity);
		Debug.Log (isLookedAt);
	if (Cardboard.SDK.CardboardTriggered && isLookedAt) {
			Debug.Log("HERE" + colorPicker.currentColor);
		colorPicker.currentColor = GetComponent<Renderer>().material.color;
	}
		
	colorPicker.cubeCurr.renderer.material.color = colorPicker.currentColor;
  }

  void OnGUI() {
    if (!CardboardGUI.OKToDraw(this)) {
      return;
    }
  }
}
