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

    string variable_type, variable_name, variable_value, variable_modifier, variable_initialization, parameter, condition, debugger;

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

            switch (line_parts[0]) {
                case Operators.EMPTY:
                    break;
                case Operators.CLOSING_BRACKET:
                    if (scope.isLooping()) scope.back();
                    else scope.pop();
                    break;
                case Keywords.Statement.Jump.BREAK:
                    scope.pop ();
                    break;
                case Keywords.Statement.Jump.CONTINUE:
                    scope.back ();
                    break;
                case Keywords.Statement.Selection.IF:
                case Keywords.Statement.Iteration.WHILE:

                    string[] line_parameters = SubstringHandler.SplitFunction (current_line, Operators.SPLIT_PARAMETERS);
                    line_parameters[1] = Parser.step (line_parameters[1], getVariableHandler (), out line_simplified);

                    script[getPointer ()] = line_parts[0] + " (" + line_parameters[1] + ") {";

                    if (line_simplified) {

                        scope.push (RangeObject.getScopeRange (script, getPointer ()), line_parts[0] != Keywords.Statement.Selection.IF);
                        evaluateCondition (parameter, line_parts[0]);
                    }

                    break;
                case Keywords.Statement.Iteration.FOR:

                    /* Add to scope "if scope hasn't been pushed for this yet..." */
                    // scope.push (RangeObject.getScopeRange (script, getPointer ()), line_parts[0] != Keywords.IF);
                    // /* e.g. "while (i < 10) {" */
                    // parameter = Operators.EMPTY;
                    // for (int i = 1; i < line_parts.Length - 1; i++) parameter += line_parts[i] + " ";
                    // parameter = parameter.Substring (1, parameter.Length - 3);

                    // if (line_parts[0] == Keywords.FOR) {
                    //     /* e.g. "int i = 0; i < 10; i++" */
                    //     string[] parameters = parameter.Split (Operators.END_LINE_CHAR);
                    //     variable_initialization = parameters[0];
                    //     variable_modifier = parameters[2].Substring (1);
                    //     parameter = parameters[1].Substring (1);
                    //     /* e.g. ["int i = 0", "i < 10", "i++"] */
                    //     if (scope.isVariableInScope (variable_initialization.Split (' ') [1])) {
                    //         scope.setVariableInScope (variable_modifier); /* Is not the first time for loop has run, e.g. "i" exists*/
                    //     } else {
                    //         scope.declareVariableInScope (variable_initialization); /* Run first part of for loop for first iteration, e.g. "i" needs to be initialized*/
                    //     }
                    // }

                    // evaluateCondition (parameter, line_parts[0]);
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
        input = Evaluator.cast (scope.parseInScope (input), Keywords.Type.Value.BOOLEAN);

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