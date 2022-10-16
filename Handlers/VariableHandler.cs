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
    public void declareVariable (string line) {

        /* e.g. "Vector2 vect = new Vector2(20, 10);" */
        declareVariable (line.Split (' '));
    }
    public void declareVariable (string[] parts) {
        /* e.g. ["int", "i", "=", "123;"] */

        /* e.g. ["Vector2", "vect", "=", "new", "Vector2(20,", "10);"] */
        variable_type = parts[0];
        variable_name = parts[1];
        variable_value = parts[3];



        if (parts[3] == Keywords.Operator.NEW) {
            for (int i = 4; i < parts.Length; i++) {
                variable_value += parts[i] + " ";
            }

            string[] variable_values = variable_value.Split (',');
            for (int i = 0; i < variable_values.Length; i++) {
                variable_values[i] = variable_values[i].Substring (1);
            }
            variable_values[0] = variable_values[0].Split ('(') [1];
            variable_values[variable_values.Length - 1].Substring (0, variable_values[variable_values.Length - 1].Length - 2);

            //might want a "splitAtLevel" to delimiter "," but ignoring them if in functions, e.g.::
            // new Vector2 (getValue(10, 20), 20);
            // should have 2 fields, not 3.

            setVariable (variable_type, variable_name, variable_values);
        } else {
            //for (int i = 3; i < parts.Length; i++) {
            //variable_value += Evaluator.scrubSymbols (parts[i]) + " ";
            //}
//            Logger.Log(line_parameters[1]);
            setVariable (variable_type, variable_name, variable_value);
        }
    }
    public void setVariable (string line) {
        setVariable (line.Split (' '));
    }
    public void setVariable (string[] parts) {
        if (parts.Length == 1) {
            /* e.g. "i++;" */
            parts = Evaluator.splitIncrement (parts[0]);
        }
        string variable_name = parts[0];
        string variable_operator = parts[1];
        int index;
        if (isVariable (variable_name, out index)) {
            variable_value = Evaluator.simplifyCondensedOperators (variable_name, variable_operator);
            for (int i = 2; i < parts.Length; i++) {
                variable_value += Evaluator.scrubSymbols (parts[i]);
                if (i != parts.Length - 1) variable_value += " ";
                else if (variable_operator != Operators.EQUALS) variable_value += Operators.CLOSING_PARENTHESIS;
            }
            setVariable (index, variable_value);
        }
    }
    public void setVariable (int index, string value) {
        if (value != Operators.EMPTY) {
            variables[index].value = value;
        }
    }
    public void setVariable (string type, string name, string value) {
        /* VARIABLE DOES NOT EXIST, INITIALIZE IT, e.g. "int i = 122;" */
        variables.Add (new VariableObject (type, name, value));
    }
    public void setVariable (string type, string name, string[] values) {
        /* OBJECT DOES NOT EXIST, INITIALIZE IT */
        variables.Add (new VariableObject (type, name, "")); //HashManager.getNewHash ().ToString() Hash used to connect it to its UI counterpart
        VariableObject template = VariableObject.getTemplate (type);
        for (int i = 0; i < template.fields.Length; i++) {
            //Logger.Log (values[i]);
            //variables.Add (new VariableObject (template.fields[i].type, name + Operators.DOT + template.fields[i].name, Evaluator.cast (parse (values[i]), template.fields[i].type)));
        }
    }

    public override string ToString () {
        string output = "";
        for (int i = 0; i < variables.Count; i++) {
            output += variables[i].ToString () + ", ";
        }
        return output;
    }
}