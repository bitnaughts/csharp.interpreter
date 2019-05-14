using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class Interpreter {

    private GameObject obj;
    private string[] script;

    private ScopeHandler scope;
    private ListenerHandler listener_handler;
    public FunctionHandler function_handler;

    string variable_type, variable_name, variable_value, variable_modifier, variable_initialization, parameter, condition, debugger;

    public Interpreter (string[] script, GameObject obj) {
        if (script == null) script = new string[] { };
        this.script = script;
        this.obj = obj;

        CompilerHandler compiler = new CompilerHandler (script);

        scope = new ScopeHandler (compiler);
        listener_handler = new ListenerHandler (compiler.handlers);
        function_handler = compiler.function_handler;
    }

    /* ... to be described...  (a.k.a. where the magic happens) */
    public bool step () {
        debugger += "LINE: " + script[scope.getPointer ()] + "\n";
        
        string line = script[scope.getPointer ()];

        if (line.isSimplified()) {

        }
        

        string[] line_parts = line.Split (' ');

        switch (line_parts[0]) {
            case Operators.EMPTY:
                break;
            case Keywords.BREAK:
                scope.pop ();
                break;
            case Operators.CLOSING_BRACKET:
                if (scope.isLooping ()) scope.back ();
                else scope.pop ();
                break;
            case Keywords.CONTINUE:
                scope.back ();
                break;
            case Keywords.IF:
            case Keywords.WHILE:
            case Keywords.FOR:
                /* Add to scope "if scope hasn't been pushed for this yet..." */
                scope.push (RangeObject.getScopeRange (script, getPointer ()), line_parts[0] != Keywords.IF);
                /* e.g. "while (i < 10) {" */
                parameter = Operators.EMPTY;
                for (int i = 1; i < line_parts.Length - 1; i++) parameter += line_parts[i] + " ";
                parameter = parameter.Substring (1, parameter.Length - 3);

                if (line_parts[0] == Keywords.FOR) {
                    /* e.g. "int i = 0; i < 10; i++" */
                    string[] parameters = parameter.Split (Operators.END_LINE_CHAR);
                    variable_initialization = parameters[0];
                    variable_modifier = parameters[2].Substring (1);
                    parameter = parameters[1].Substring (1);
                    /* e.g. ["int i = 0", "i < 10", "i++"] */
                    if (scope.isVariableInScope (variable_initialization.Split (' ') [1])) {
                        scope.setVariableInScope (variable_modifier); /* Is not the first time for loop has run, e.g. "i" exists*/
                    } else {
                        scope.declareVariableInScope (variable_initialization); /* Run first part of for loop for first iteration, e.g. "i" needs to be initialized*/
                    }
                }

                evaluateCondition (parameter, line_parts[0]);
                break;
            case Variables.BOOLEAN:
            case Variables.INTEGER:
            case Variables.FLOAT:
            case Variables.STRING:
                //Primitive Data Types
                scope.declareVariableInScope (line);
                break;
            case Console.NAME:
            case Plotter.NAME:
                //Not possible (as far as I know) to hit a function header, so assume it is just a variable declaration
                scope.declareVariableInScope (line);
                if (line_parts[0] == Console.NAME) {
                    //issue with doing this here is that you still ned separate logic when garbage collecting to destroy window...
                }
                break;
            default:
                VariableObject variable_reference;
                if (scope.isVariableInScope (line_parts[0])) {
                    /* CHECK IF LINE REFERS TO A VARIABLE, e.g. "i = 10;" */
                    scope.setVariableInScope (line);

                } else if (scope.isVariableInScope (line_parts[0].Split('.')[0], out variable_reference)) {


                    /* e.g. "console1.Add(...)" */
                    listener_handler.callListener (line, variable_reference, obj);
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
                    debugger += "Not able to process line" + line;
                }
                break;
        }
        if (scope.isFinished ()) return true;
        else {
            scope.step ();
            listener_handler.updateListeners (this);
            return false;
        }
    }

    private void evaluateCondition (string input, string type) {
        input = Evaluator.cast (scope.parseInScope (input), Variables.BOOLEAN);

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

    public int getPointer () {
        return scope.getPointer ();
    }
    public override string ToString () {
        string output = debugger + "\n\n";
        // output += compiler.ToString ();
        debugger = Operators.EMPTY;
        output += scope.ToString ();
        return output;
    }
}