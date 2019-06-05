using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

public class Interpreter {

    private string[] script;
    private string current_line;

    private ScopeHandler scope;
    private ListenerHandler listener_handler;
    private TemplateHandler template_handler;
    public FunctionHandler function_handler;

    bool line_simplified;
    bool first_section_finished = false;

    string debugger;

    public Interpreter (string[] script) {
        if (script == null) script = new string[] { };
        this.script = script;

        CompilerHandler compiler = new CompilerHandler (script);

        scope = new ScopeHandler (compiler);
        listener_handler = new ListenerHandler (compiler.handlers);

        template_handler = new TemplateHandler (compiler.template_list); //Will want to include information from compiler: build templates for all functions in scope

        function_handler = compiler.function_handler;
    }

    /* ... to be described...  (a.k.a. where the magic happens) */
    public bool step () {

        debugger += "LINE: " + script[getPointer ()] + "\n";

        current_line = script[getPointer ()];

        //An important missing step is to keep a backup copy this line to revert to when the line is done...
        string line_saved = current_line;

        /* ... */

        //if (test < 10) {
        //for (int i = 0; i < 10; i++) {
        // string[] line_parameters = SubstringHandler.Split(current_line, Operators.SPLIT_PARAMETERS);
        //"if", "test < 10", "{"
        //"for", "int i = 0", "i < 10", "i++", "{"

        // debugger += "\n" + current_line;
        // for (int i = 0; i < line_parameters.Length; i++) {
        //     debugger += "\n|" + line_parameters[i] + "|";
        // }

        // current_line = Parser.step (current_line, getVariableHandler (), out line_simplified);

        //For visualization
        // script[getPointer ()] = current_line;

        //This might be more appropriate in Interpreter.cs, as parser should be specialized only for shortening expressions/functions...
        /* If there is a variable declaration in the current line, e.g. "i = 10 + 20" or "int i = 10 + 20" */
        //if (line_in.Contains(Operators.SPACE + Operators.EQUALS + Operators.SPACE))
        //{ //to avoid being confused with ==, etc.

        //string[] line_parts = line_in.Split(Operators.SPACE + Operators.EQUALS + Operators.SPACE);
        //string variable_information = line_parts[0]; // "i"
        //string value_information = line_parts[1]; // "10 + 20" ==> "30"...

        //line_in = line_in.Substring(line_in.IndexOf(Operators.SPACE + Operators.EQUALS + Operators.SPACE) + 3);
        //Might need some sort of promise to set variable to fully simplified value...
        //Also for rebuilding full line back to "i = 30" at the end...
        //}

        if (line_simplified || true) {

            string[] line_parts = current_line.Split (Operators.SPACE[0]);
            string[] line_parameters;
            switch (line_parts[0]) {
                case Operators.EMPTY:
                    break;
                case Operators.CLOSING_BRACKET:
                    if (scope.isLooping ()) scope.back ();
                    else scope.pop ();
                    break;
                case Keywords.Statement.Jump.BREAK:
                    scope.pop ();
                    break;
                case Keywords.Statement.Jump.CONTINUE:
                    scope.back ();
                    break;
                case Keywords.Statement.Selection.IF:
                case Keywords.Statement.Iteration.WHILE:
                    /* For this case, use the example "if (i < 10) {" */

                    /* e.g. ["if", "i < 10", "{"] */
                   line_parameters = SubstringHandler.SplitFunction (current_line, Operators.SPLIT_PARAMETERS);

                    /* e.g. "5 < 10" if i = 5 */
                    line_parameters[1] = Parser.step (line_parameters[1], getVariableHandler (), out line_simplified);

                    /* e.g. "if (5 < 10) {" */
                    script[getPointer ()] = line_parts[0] + " (" + line_parameters[1] + ") {";

                    if (line_simplified) {
                        scope.push (RangeObject.getScopeRange (script, getPointer ()), line_parts[0] != Keywords.Statement.Selection.IF);
                        evaluateCondition (line_parameters[1], line_parts[0]);
                    }
                    break;
                case Keywords.Statement.Iteration.FOR:
                    /* For this case, use the example "for (int i = 5; i < 10; i++) {" */

                    /* e.g. ["for", "int i = 5", "i < 10", "i++", "{"] */
                    line_parameters = SubstringHandler.SplitFunction (current_line, Operators.SPLIT_PARAMETERS);

                    

                    /* Has the for loop been executed in scope before? If so, "i" must've been declared */
                    if (scope.isVariableInScope (line_parameters[1].Split (' ') [1])) {
                        /* For loop has run in scope before, modify the variable */

                        /* e.g. "i++" goes to "i = i + 1" goes to "i = 5 + 1" goes to "i = 6", simplified = true */
                        line_parameters[3] = Parser.step (line_parameters[3], getVariableHandler (), out line_simplified);

                        if (line_simplified) {
                            scope.setVariableInScope (line_parameters[3]);
                            first_section_finished = true;
                            
                        }
                        else {
                            script[getPointer()] = line_parts[0] + " (" + line_parameters[1] + "; " + line_parameters[2] + "; " + line_parameters[3] + ") {";
                            break; }

                    } else {
                        /* For loop has not run in scope before, declare the variable */

                        /* e.g. "int i = 5", simplified = true */
                        line_parameters[1] = Parser.step (line_parameters[1], getVariableHandler (), out line_simplified);

                        if (line_simplified) {
                            Logger.Log(line_parameters[1]);
                            scope.declareVariableInScope(line_parameters[1]);
                            first_section_finished = true;
                        }
                        else {
                            script[getPointer()] = line_parts[0] + " (" + line_parameters[1] + "; " + line_parameters[2] + "; " + line_parameters[3] + ") {";
                            break;
                        }; //don't check conditional in same instant
                    }

                    //if (first_section_finished) {
                    Logger.Log(line_parameters[2]);
                    line_parameters[2] = Parser.step (line_parameters[2], getVariableHandler (), out line_simplified);
                    debugger += "\ntwo: " + line_parameters[2];
                        if (line_simplified)
                        { 
                            scope.push (RangeObject.getScopeRange (script, getPointer ()), line_parts[0] != Keywords.Statement.Selection.IF);
                            evaluateCondition (line_parameters[2], line_parts[0]);
                        }
                    //}
                    script[getPointer()] = line_parts[0] + " (" + line_parameters[1] + "; " + line_parameters[2] + "; " + line_parameters[3] + ") {";


                    break;
                case Keywords.Type.Value.BOOLEAN:
                case Keywords.Type.Value.INTEGER:
                case Keywords.Type.Value.FLOAT:
                case Keywords.Type.Reference.STRING:
                    //Primitive Data Types
                    scope.declareVariableInScope (current_line);
                    break;
                    //case Console.NAME:
                    //case Plotter.NAME:
                    //Not possible (as far as I know) to hit a function header, so assume it is just a variable declaration
                    //scope.declareVariableInScope (current_line);
                    //if (line_parts[0] == Console.NAME) {
                    //issue with doing this here is that you still ned separate logic when garbage collecting to destroy window...
                    //}
                    //break;
                default:
                    VariableObject variable_reference;
                    if (scope.isVariableInScope (line_parts[0])) {
                        /* CHECK IF LINE REFERS TO A VARIABLE, e.g. "i = 10;" */
                        scope.setVariableInScope (current_line);

                    } else if (scope.isVariableInScope (line_parts[0].Split ('.') [0], out variable_reference)) {

                        /* e.g. "console1.Add(...)" */
                        // listener_handler.callListener (line, variable_reference, obj);
                        debugger += "YES\n\n";

                    } else if (function_handler.isFunction (line_parts[0].Split ('(') [0])) {
                        /* CHECK IF LINE REFERS TO A FUNCTION, e.g. "print(x);" */
                        FunctionObject function = function_handler.getFunction (line_parts[0].Split ('(') [0]);
                        scope.push (RangeObject.returnTo (function.range, getPointer ()), false);

                    } else if (listener_handler.isFunction (line_parts[0].Split ('(') [0])) {

                        //TODO:: Not sure if this is accomplishing intended functionality

                        /* CHECK IF LINE REFERS TO A LISTENER, e.g. "Console.PrintLine("test");" */
                        // listener_handler.callListener (line,  obj);

                    } else {
                        debugger += "Not able to process line" + current_line;
                    }
                    break;
            }
            if (scope.isFinished ()) return true;
            else {
                if (line_simplified) {
                    scope.step ();
                }
                // listener_handler.updateListeners (this);
                return false;
            }
        } else {
            return false;
        }
    }

    private void evaluateCondition (string input, string type) {
        //Logger.Log(input);
        //input = Evaluator.cast (scope.parseInScope (input), Keywords.Type.Value.BOOLEAN);
        //Logger.Log(input);
        if (bool.Parse (input) == true) {
            //...
        } else { scope.pop (); }

    }

    private int getMatchingEndBracket (int start_line) {
        int bracket_count = 1;
        while (bracket_count > 0) {
            start_line++;
            if (script[start_line].Contains (Operators.OPENING_BRACKET)) {
                bracket_count++;
            } else if (script[start_line] == Operators.CLOSING_BRACKET) {
                bracket_count--;
            }
        }
        return start_line + 1;
    }

    /* Scope Handler Helper Functions */
    public int getPointer () {
        return scope.getPointer ();
    }
    public VariableHandler getVariableHandler () {
        return scope.getVariableHandler ();
    }

    /* Visualization Helper Functions */
    public string getScript () {
        return String.Join (Operators.NEW_LINE, script);
    }

    public override string ToString () {
        string output = debugger + "\n\n";
        // output += compiler.ToString ();
        debugger = Operators.EMPTY;
        output += scope.ToString ();
        return output;
    }
}