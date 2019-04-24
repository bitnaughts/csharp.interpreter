using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
public class ScriptEditor : MonoBehaviour {

	public ScriptObject script;

	// public ScriptObject getScript () {
	// 	return script;
	// }
	void Update() {
		if (script != null && script.tick (Time.deltaTime, Referencer.codeSpeedTester.value)) {
			GameObject.Find("Text 1").GetComponent<Text>().text = script.ToString();
			//Any visualization/updating while script executes
		}
	}
}