﻿// Copyright 2014 Google Inc. All rights reserved.
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
	float timeLeft = 2.0f;

	public AudioClip sound;
	private AudioSource source;

	float stillTimeLeft = 2.0f;
	public Vector3 lastVector;
	public Vector3 currVector;
	float thresh = .01f;

  void Start() {
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

  void Update() {
	lastVector = currVector;
	currVector = head.Gaze.direction;
	if (isStill()) {
		audio.Play();
		Debug.Log("isStill!");
	}

//    RaycastHit hit;
//
//    if (Cardboard.SDK.CardboardTriggered && isLookedAt) {
//      // Teleport randomly.
//      Vector3 direction = Random.onUnitSphere;
//      direction.y = Mathf.Clamp(direction.y, 0.5f, 1f);
//      float distance = 2 * Random.value + 1.5f;
//      transform.localPosition = direction * distance;
  }
	
  void OnGUI() {
    if (!CardboardGUI.OKToDraw(this)) {
      return;
    }
  }
}
//    if (GUI.Button(new Rect(50, 50, 200, 50), "Reset")) {
//      transform.localPosition = startingPosition;
//    }
//	var guiTime = Time.time - startTime;
//	//var seconds : int = guiTime % 60;
//		Debug.Log(guiTime);

			//	//Debug.Log (GetComponent<Collider>().Raycast(head.Gaze, out hit, Mathf.Infinity));
			//    bool isLookedAt = GetComponent<Collider>().Raycast(head.Gaze, out hit, Mathf.Infinity);
			//	if (isLookedAt) {
			////		Debug.Log("isLookedAt");
			////		Debug.Log(timeLeft);
			//		if (timeLeft > 0) {
			//			timeLeft -= Time.deltaTime;
			//		}
			//		if (timeLeft < 0) {
			////			Debug.Log("Color selected!");
			//			GetComponent<Renderer>().material.color = Color.blue;
			//		}
			//	}
			//	if (!isLookedAt) {
			//
			////		Debug.Log("not isLookedAt");
			//		timeLeft = 2.0f;
			//	}	
			
			//    GetComponent<Renderer>().material.color = isLookedAt ? Color.white : Color.grey;
	
	//			source.PlayOneShot(sound, 1F);
	//			GameObject cyl = GameObject.CreatePrimitive(PrimitiveType.Cylinder);
	//			//		plane.transform.Translate(0,0,8);
	//			//		plane.transform.Rotate (270, 0, 0);
	//			cyl.transform.parent = transform;
	//			cyl.transform.localPosition = new Vector3(0f, 0f, 2.5f);
	//			cyl.transform.Rotate(90, 0, 0);
	//			cyl.transform.localScale = new Vector3(0.1f, .0000000007f, 0.1f);
	//			cyl.renderer.material.color = Color.black;	