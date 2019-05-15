using System.Collections;
using System.Collections.Generic;
using UnityEngine;
public class ScriptObject {

	public GameObject obj;
	private string[] script;

	private ProcessorObject processor;
	private Interpreter interpreter;
	float time = 0;

	bool ran_this_tick = false;
	bool finished = false;
	int line = 0;
	bool new_line = true;

	public ScriptObject (GameObject obj, string text) {
		init (obj, text.Split ('\n'));
	}
	public ScriptObject (GameObject obj, string[] script) {
		init (obj, script);
	}
	public void init (GameObject obj, string[] script) {
		this.obj = obj;
		this.script = script;
		interpreter = new Interpreter (script, obj);
		processor = new ProcessorObject ();
	}
	public void setScript (string text) {
		setScript (text.Split ('\n'));
	}
	public void setScript (string[] script) {
		interpreter = new Interpreter (script, obj);
	}
	public void setProcessor (ProcessorObject processor) {
		this.processor = processor;
	}
	public bool tick (float delta_time, float tick_speed) {
		processor.setSpeed (tick_speed);
		if (finished) return false;
		ran_this_tick = false;
		time += delta_time;
		while (time >= processor.tick_speed && !finished) {
			time -= processor.tick_speed;
			/* Execute a line */
			finished = interpreter.step ();
			ran_this_tick = true;
		}
		return ran_this_tick;
	}
	public int getCurrentLine () {
		return interpreter.getPointer ();
	}
	public string[] getScript () {
		return script;
	}
	public string getFormattedScript () {
		string indent_string = "<color=#" + ColorCoder.getLitScreenColor () + ">" + "|" + "</color>\t";
		string output = "";
		string indents = "";//"\t";
		for (int line = 0; line < script.Length; line++) {
			if (script[line].Contains ("}")) indents = indents.Substring (indent_string.Length);
			if (line < 10) output += "0" + line + "\t" + indents + ColorCoder.colorize(script[line]) + "\n";
			else output += line + "\t" + indents + ColorCoder.colorize(script[line]) + "\n";
			if (script[line].Contains ("{")) indents += indent_string;
		}
		return output;
	}

	public override string ToString () {
		return interpreter.ToString ();
	}
}