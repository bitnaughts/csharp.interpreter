using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ClassController
{
    string name = "";
    List<Field> fields; // primitive data types, objects
    List<Method> methods; // constructor (initializes fields), getter/setter (modifies fields), main (Processor's start method)

    public ClassController(string input) {
        fields = new List<Field>();
        methods = new List<Method>();
        
        switch (input) {
            case "◎": //Booster
                name = "Booster";
                fields.Add(new Field("throttle", "double", "0"));
                methods.Add(new Method("Boost ()", "Throttle_control", "void", "throttle = 100;"));
                methods.Add(new Method("Launch ()", "Torpedo_control", "Torpedo", "return new Torpedo( GetLoadedBarrel() );"));
                methods.Add(new Method("Ping ()", "Called_periodically", "void", "throttle--;"));
                break;
            case "◉": //Thruster
                name = "Thruster";
                fields.Add(new Field("throttle", "double", "0"));
                methods.Add(new Method("MaxThrottle ()", "Throttle_control_(max)", "void", "throttle = 100;"));
                methods.Add(new Method("MinThrottle ()", "Throttle_control_(min)", "void", "throttle = 0;"));
                break;
            case "◍": //Cannon
                name = "Cannon";
                fields.Add(new Field("barrels", "double[]", "{}"));
                methods.Add(new Method("Fire ()", "Use_weapon_control ", "Shell", "return new Shell( GetLoadedBarrel() );"));
                break;
            case "▥": //Bulkhead
                name = "Bulkhead";
                fields.Add(new Field("heap", "Heap", "new Heap()"));
                methods.Add(new Method("New (string name)", "Memory_Allocation", "void", "heap.Add( name )"));
                methods.Add(new Method("Delete (string name)", "Memory_Deallocation", "void", "heap.Remove( name )"));
                break;
            case "▩": //Processor
                name = "Processor";
                fields.Add(new Field("stack", "Stack", "new Stack()"));
                methods.Add(new Method("Main (string[] args)", "Main_Method_(entry_point)", "void", "$"));
                break;
            case "▣": //Gimbal
                name = "Gimbal";
                methods.Add(new Method("RotateCW ()", "Rotation_control_(cw)", "void", "r += 15;"));
                methods.Add(new Method("RotateCCW ()", "Rotation_control_(ccw)", "void", "r -= 15;"));
                methods.Add(new Method("Rotate (double delta)", "Rotation_control_(ccw)", "void", "r += delta;"));
                break;
            case "◌": //Sensor
                name = "Sensor";
                methods.Add(new Method("Scan ()", "Measure_distance", "double", "new Ray().Length();"));
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
        string output = "class " + name + " : Component {\n";
        foreach (var f in fields.ToArray()) {
            output += f.ToString() + "\n";
        }
        foreach (var m in methods.ToArray()) {
            output += m.ToString() + "\n";
        }
        return output + "}";
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
    public override string ToString () {
        return type + " " + name + " = " + value + ";";
    }
}
public class Method 
{
    public string comment, name, return_type, code;
    public Method(string name, string comment, string return_type, string code) {
        this.comment = comment;
        this.name = name;
        this.return_type = return_type;
        this.code = code;
    }
    public override string ToString () {
        return "/*" + comment + "*/\n" + return_type + " " + name + " { " + code + " }";
    }
}