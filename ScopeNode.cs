using System.Collections.Generic;

public class ScopeNode {
    private Range range;

    private bool looping;

    public VariableHandler variable_handler;

    /* 
    1   :   if (true) {
    2   :       int var = 10;
    3   :       if (false) { 
    4   :           print(var);
    5   :       }
    6   :   }
    
    OUTER
    start line == 1
    end line == 6

    INNER
    start line == 3
    end line == 5

    when closing node, if condition if false, go to end_line + 1; if condition is true, go to start_line + 1
    */

    public ScopeNode (Range range, bool looping) {
        this.range = range;
        this.variable_handler = new VariableHandler ();
        this.looping = looping;

    }
    public ScopeNode (Range range, List<VariableObject> variables, bool looping) {
        this.range = range;
        this.variable_handler = new VariableHandler (variables);
        this.looping = looping;
    }

    public void setRange(Range range) {
        this.range = range;
    }

    public int getStartLine () {
        return this.range.start;
    }
    public int getEndLine () {
        return this.range.end;
    }
    public int getReturnToLine () {
        return this.range.return_to;
    }
    public bool isLooping () {
        return looping;
    }

    public void addVariableToScope (VariableObject variable) {
        variable_handler.add (variable);
    }
    public override string ToString () {
        string output = range.ToString() + "\t" + variable_handler.ToString ();
        return output;
    }
}