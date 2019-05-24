using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Globalization;
using System.Linq;
using System.Reflection;

public class VariableObject {

    public string type;
    public string name;
    /* For Primitive Data Types, value == value, otherwise value holds ToString() */
    public string value;
    public VariableObject[] fields;

    public VariableObject (string type) {
        init (type, "", "");
    }
    public VariableObject (string type, string name, string value) {
        init (type, name, value);
    }

    public void init (string type, string name, string value) {
        this.type = type;
        this.name = name;
        this.value = value;
        if (name == "") {

            switch (type) {

            case "Vector2":
            fields = new VariableObject[] {
            new VariableObject (Variables.FLOAT, "x", value.Split (',') [0]),
            new VariableObject (Variables.FLOAT, "y", value.Split (',') [1])
                    };
                    break;
                case Console.NAME:
                    fields = new VariableObject[] {
                        new VariableObject (Variables.FLOAT, "x", ""),
                        new VariableObject (Variables.FLOAT, "y", "")
                    };
                    break;
                case Plotter.NAME:
                    fields = new VariableObject[] {
                        new VariableObject (Variables.STRING, "value", "")
                    };
                    break;
                default:
                    fields = null;
                    break;
            }
        }
    }

    public static VariableObject getTemplate (string type) {
        return new VariableObject (type);
    }

    public override string ToString () {
        string output = type + " " + name + " = " + value;
        if (fields != null) {

            for (int i = 0; i < fields.Length; i++) {
                output += "-> -> Field(" + fields[i].ToString () + ")\n";
            }
        }
        return output;
    }
}

// public class VariableTemplates {
//     public static VariableObject VECTOR2;

// }