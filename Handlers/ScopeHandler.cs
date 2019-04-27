using System.Collections.Generic;

public class ScopeHandler {

    private int pointer;

    private Stack<ScopeObject> scope;

    //Available Functions to Call
    //might need to make a "FunctionObject" for this to keep track of potential parameters/return types....

    private bool hasAlreadyStepped;
    private bool hasFinished;

    public ScopeHandler (CompilerHandler compiled_script) {
        scope = new Stack<ScopeObject> ();
        scope.Push (compiled_script.base_scope);

        pointer = compiled_script.main_function_line;

        hasAlreadyStepped = false;
        hasFinished = false;
    }
    public bool isVariableInScope (string name) {
        if (scope.Count == 0) return false;
        return scope.Peek ().variable_handler.isVariable (name);
    }
    public bool isVariableInScope (string name, out VariableObject variable_reference) {
        
        variable_reference = null;
        if (scope.Count == 0) return false;
        int index;
        if (scope.Peek ().variable_handler.isVariable (name, out index))
        {
            variable_reference = scope.Peek ().variable_handler.getVariable(index);
            return true;
        }
        return false;
    }
    public void setVariableInScope (string line) {
        if (scope.Count == 0) return;
        scope.Peek ().variable_handler.setVariable (line);
    }
    public void declareVariableInScope (string line) {
        if (scope.Count == 0) return;
        scope.Peek ().variable_handler.declareVariable (line);
    }
    public string parseInScope (string line) {
        if (scope.Count == 0) return null;
        return scope.Peek ().variable_handler.parse (line);
    }
    public List<VariableObject> getVariablesInScope () {
        if (scope.Count == 0) return null;
        List<VariableObject> variables_copy = new List<VariableObject> ();
        foreach (VariableObject variable in scope.Peek ().variable_handler.variables) {
            variables_copy.Add (variable);
        }
        return variables_copy;
    }
    public List<VariableObject> getVariablesInScope (string type) {
        if (scope.Count == 0) return null;
        List<VariableObject> variables_copy = new List<VariableObject> ();
        foreach (VariableObject variable in scope.Peek ().variable_handler.variables) {
            if (type == variable.type) variables_copy.Add (variable);
        }
        return variables_copy;
    }

    public void step () {
        if (hasAlreadyStepped == true) {
            hasAlreadyStepped = false;
        } else {
            pointer++;
        }
    }
    public void push (RangeObject range) {
        push (range, false);
    }
    public void push (RangeObject range, bool isLooping) {
        //Add all existing variables to new scope
        if (scope.Count == 0 || scope.Peek ().getStartLine () != range.start) {
            ScopeObject node = new ScopeObject (range, getVariablesInScope (), isLooping);
            scope.Push (node);

            pointer = range.start;
            hasAlreadyStepped = true;
        }

    }
    public void pop () {
        pointer = scope.Peek ().getReturnToLine ();
        hasAlreadyStepped = true;

        scope.Pop ();
        if (scope.Count == 0) hasFinished = true;
    }
    public void back () { //continue keyword
        pointer = scope.Peek ().getStartLine ();
        hasAlreadyStepped = true;
    }

    public bool isLooping () {
        if (scope.Count == 0) return false;
        return scope.Peek ().isLooping ();
    }
    public bool isFinished () {
        return hasFinished;
    }
    // public void skipScope () {
    //     pointer = scope.Peek ().getEndLine (); //getPointerTo (pointer, Operators.CLOSING_BRACKET); //will get "End line" value here
    //     //   variables = GarbageCollector.removeBetween (getPointerTo (pointer, Operators.OPENING_BRACKET), pointer, variables);
    // }

    public int getPointer () {
        return pointer;
    }
    // public int getPointerTo (int starting_pointer, string target) {
    //     int bracket_counter = 1;
    //     switch (target) {
    //         case Operators.CLOSING_BRACKET:
    //             while (bracket_counter > 0) {
    //                 starting_pointer++;
    //                 if (script[starting_pointer].Contains (Operators.OPENING_BRACKET)) bracket_counter++;
    //                 else if (script[starting_pointer].Contains (Operators.CLOSING_BRACKET)) bracket_counter--;
    //             }
    //             return starting_pointer;
    //         case Operators.OPENING_BRACKET:
    //             while (bracket_counter > 0) {
    //                 starting_pointer--;
    //                 if (script[starting_pointer].Contains (Operators.OPENING_BRACKET)) bracket_counter--;
    //                 else if (script[starting_pointer].Contains (Operators.CLOSING_BRACKET)) bracket_counter++;
    //             }
    //             return starting_pointer;
    //     }
    //     return starting_pointer;
    // }
    public override string ToString () {
        string output = "\n" + pointer + "\n";
        ScopeObject[] scope_array = scope.ToArray ();
        for (int i = 0; i < scope_array.Length; i++) {
            output += i + ":\t" + scope_array[i].ToString () + "\n";
        }
        return output;
    }
}