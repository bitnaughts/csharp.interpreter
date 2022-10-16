using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ClassController
{
    public string name = "";
    public string input = "";
    public List<Field> fields; // primitive data types, objects
    public List<Method> methods; // constructor (initializes fields), getter/setter (modifies fields), main (Processor's start method)

    public ClassController(string input) {
        this.input = input;
        fields = new List<Field>();
        methods = new List<Method>();
        switch (input) {
            // Iterate️ class
            case "ℹ":
                name = "Iterate";
                methods.Add(new Method("Main", "Entry Point", "void", new List<string>(){"$"}));
                // methods.Add(new Method("SumArray (int[] input)", "Return_sum_of_input_array", "int", "  int total = 0;\n  for (int i = 0; i < input.Length(); i++)\n  {\n    total += input[i];\n  }\n  return total;"));
                break;
            // BitNaughts constant class
            case "◎": //Booster
                name = "Booster";
                fields.Add(new Field("throttle", "double", "0"));
                // methods.Add(new Method("Boost ()", "Throttle_control", "void", "throttle = 100;"));
                // methods.Add(new Method("Launch ()", "Torpedo_control", "Torpedo", "return new Torpedo( GetLoadedBarrel() );"));
                // methods.Add(new Method("Ping ()", "Called_periodically", "void", "throttle--;"));
                break;
            case "◉": //Thruster
                name = "Thruster";
                fields.Add(new Field("throttle", "double", "0"));
                // methods.Add(new Method("MaxThrottle ()", "Throttle_control_(max)", "void", "throttle = 100;"));
                // methods.Add(new Method("MinThrottle ()", "Throttle_control_(min)", "void", "throttle = 0;"));
                break;
            case "◍": //Cannon
                name = "Cannon";
                fields.Add(new Field("barrels", "double[]", "{}"));
                // methods.Add(new Method("Fire ()", "Use_weapon_control ", "Shell", "return new Shell( GetLoadedBarrel() );"));
                break;
            case "▥": //Bulkhead
                name = "Bulkhead";
                fields.Add(new Field("heap", "Heap", "new Heap()"));
                // methods.Add(new Method("New (string name)", "Memory_Allocation", "void", "heap.Add( name )"));
                // methods.Add(new Method("Delete (string name)", "Memory_Deallocation", "void", "heap.Remove( name )"));
                break;
            case "▩": //Processor
                name = "Processor";
                fields.Add(new Field("stack", "Stack", "new Stack()"));
                // methods.Add(new Method("Main (string[] args)", "Main_Method_(entry_point)", "void", "$"));
                break;
            case "▣": //Gimbal
                name = "Gimbal";
                // methods.Add(new Method("RotateCW ()", "Rotation_control_(cw)", "void", "rot += 15;"));
                // methods.Add(new Method("RotateCCW ()", "Rotation_control_(ccw)", "void", "rot -= 15;"));
                // methods.Add(new Method("Rotate (double delta)", "Rotation_control_(ccw)", "void", "rot += delta;"));
                break;
            case "◌": //Sensor
                name = "Sensor";
                // methods.Add(new Method("Scan ()", "Measure_distance", "double", "new Ray().Length();"));
                break;
            case "▦": //Printer
                fields.Add(new Field("nozzel", "Nozzel", "new Nozzel()"));
                // methods.Add(new Method("Main (string[] args)", "Main_Method_(entry_point)", "void", "$"));
                break;
            default:
                // general classes
                // for field in fields, add
                // for method in method, add
                break;
        }
        // fields["test"] = component.GetTypeClass().ToString();
    }

    public override string ToString() {
        string output = "";//♘";
        // if (input.Length == 1) {
        //     return output + "c" + input;
        // }
        output += "class " + name + "\n{\n";
        foreach (var f in fields.ToArray()) {
            output += f.ToString() + "\n";
        }
        foreach (var m in methods.ToArray()) {
            output += m.ToString() + "\n";
        }
        return output + "\n}";
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
    public string comment, name, return_type;
    public List<string> lines;
    public Method(string name, string comment, string return_type, List<string> lines) {
        this.comment = comment;
        this.name = name;
        this.return_type = return_type;
        this.lines = lines;
    }
    public override string ToString () {
        return "  /*" + comment + "*/\n  " + return_type + " " + name + "\n{\n  " + string.Join("\n", lines.ToArray()) + "\n}";
    }
}