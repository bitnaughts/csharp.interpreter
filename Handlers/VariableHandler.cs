using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

public class VariableHandler {
    string variable_type, variable_name, variable_value, variable_modifier, variable_initialization, parameter, condition, debugger;

    //this should probably become a dictionary...
    public List<VariableObject> variables;

    public VariableHandler () {
        variables = new List<VariableObject> ();
    }
    public VariableHandler (List<VariableObject> variables) {
        if (variables == null) this.variables = new List<VariableObject> ();
        else this.variables = variables;
    }
    public void add (VariableObject variable) {
        variables.Add (variable);
    }

    public bool isVariable (string name) {
        return getIndexOfVariable (name) != -1;
    }
    public bool isVariable (string name, out int index) {
        index = getIndexOfVariable (name);
        return index != -1;
    }
    public int getIndexOfVariable (string name) {
        for (int i = 0; i < variables.Count; i++)
            if (variables[i].name == name) return i;
        return -1;
    }
    public string getValue (string input) {
        int index;
        if (isVariable (input, out index)) return variables[index].value;
        return input;
    }
    public VariableObject getVariable (int index) {
        return variables[index];
    }
    

    public override string ToString () {
        string output = "";
        for (int i = 0; i < variables.Count; i++) {
            output += variables[i].ToString () + ", ";
        }
        return output;
    }
}