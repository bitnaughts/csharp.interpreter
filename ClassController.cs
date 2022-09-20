using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ClassController
{
    List<Field> fields; // primitive data types, objects
    List<Method> methods; // constructor (initializes fields), getter/setter (modifies fields), main (Processor's start method)

    public ClassController(string input) {
        switch (input) {
            case "◎": //Booster
                fields.Add(new Field("throttle", "double", "0"));
                methods.Add(new Method("Boost ()", "Throttle control", "throttle = 100;"));
                methods.Add(new Method("Launch ()", "Torpedo control", "new Torpedo();"));
                methods.Add(new Method("Ping ()", "Called periodically", "throttle--;"));
                break;
            case "◍": //Thruster
                fields.Add(new Field("throttle", "double", "0"));
                methods.Add(new Method("MaxThrottle ()", "Throttle control (max)", "throttle = 100;"));
                methods.Add(new Method("MinThrottle ()", "Throttle control (min)", "throttle = 0;"));
                break;
            case "◍": //Cannon
                fields.Add(new Field("barrels", "double[]", "{}"));
                methods.Add(new Method("Fire ()", "Use weapon control ", "new Shell();"));
            case "▥": //Bulkhead
                fields.Add(new Field("heap", "Heap", "new Heap()"));
                methods.Add(new Method("New (string name)", "Memory Allocation", "heap.Add(name)"));
                methods.Add(new Method("Delete (string name)", "Memory Deallocation", "heap.Remove(name)"));
                break;
            case "▩": //Processor
                fields.Add(new Field("stack", "Stack", "new Stack()"));
                methods.Add(new Method("Main (string[] args)", "Main Method (entry point)", "$"));
                break;
            case "▣": //Gimbal
                methods.Add(new Method("RotateCW ()", "Rotation control (cw)", "r += 15;"));
                methods.Add(new Method("RotateCCW ()", "Rotation control (ccw)", "r -= 15;"));
                methods.Add(new Method("Rotate (double delta)", "Rotation control (ccw)", "r += delta;"));
                break;
            case "◌": //Sensor
                methods.Add(new Method("Scan ()", "Measure distance", "new Ray().Length();"));
                break;
            case "▦": //Printer
                break;
            default:
                // for field in fields, add
                // for method in method, add
                break;
        }
        // fields["test"] = component.GetTypeClass().ToString();
    }

    public override string ToString() {
        return fields.ToArray() + methods.ToArray(); 
    }

}


public class Field 
{
    public string type, name, value;
    public Field (string name, string type, string value) {
        this.name = name;
        this.type = type;
        this.value = value;
    }
}
public class Method 
{
    public string comment, name, code;
    public Method(string name, string comment, string code) {
        this.comment = comment;
        this.name = name;
        this.code = code;
    }
}