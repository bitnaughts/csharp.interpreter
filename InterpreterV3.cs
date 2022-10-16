using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System;

public class InterpreterV3
{
    List<ClassController> classes;
    ScopeController scope;
    HeapController heap;
    string target_class = "";
    int index = -1;
    string debug_output = "";
    // List<string> methods;
    public InterpreterV3(List<ClassController> classes) 
    {
        this.classes = classes;
        this.scope = new ScopeController();
        this.heap = new HeapController();
        foreach (var c in classes) 
        {
            foreach (var m in c.methods) 
            {
                if (m.name == "Main") 
                {
                    debug_output += "Found Main ()";
                    scope.push(new Scope(c.name, m, 0, m.lines.Count(), false));
                    SetTargetClass(c.name);
                }
            }
        }
    }
    /* Interpret executes the next operation found */
    string intermediate_line = "";
    public void Interpret() 
    {
        if (scope == null) 
        {
            debug_output += "No Main method found";
            return;
        }
        Scope s = scope.peek ();
        /* Initialize index to the top of the scope */
        if (index == -1) 
        {
            index = s.start_index;
        }
        /* Grab the current line if empty */
        if (intermediate_line == "") 
        {
            intermediate_line = s.method.lines[index];
            debug_output += "\nLine: \"" + intermediate_line + "\"";
        }
        /* Split the intermediate line into its parts */
        string[] line_parts = intermediate_line.Split (Operators.SPACE[0]);
        string[] line_parameters = {};
        if (line_parts == null) 
        {
            debug_output += "Line empty";
            return;
        }
        /* Determine operation */
        switch (line_parts[0]) {
            case Operators.EMPTY:
                debug_output += "Line part empty";
                break;
            case Operators.CLOSING_BRACKET:
                index = -1;
                if (!s.is_loop) 
                {
                    // scope.pop ();
                }
                break;
            case Keywords.Statement.Jump.BREAK:
                index = -1;
                // scope.pop ();
                break;
            case Keywords.Statement.Jump.CONTINUE:
                index = s.start_index;
                break;
            // case Keywords.Statement.Selection.IF:
            // case Keywords.Statement.Iteration.WHILE:
            //     /* For this case, use the example "if (i < 10) {" */
            //     /* e.g. ["if", "i < 10", "{"] */
            //     line_parameters = SubstringHandler.SplitFunction (intermediate_line, Operators.SPLIT_PARAMETERS);

            //     /* e.g. "5 < 10" if i = 5 */
            //     line_parameters[1] = Parser.step (line_parameters[1], getVariableHandler (), out line_simplified);

            //     /* e.g. "if (5 < 10) {" */
            //     script[getPointer ()] = line_parts[0] + " (" + line_parameters[1] + ") {";

            //     if (line_simplified) {
            //         // scope.push (RangeObject.getScopeRange (script, getPointer ()), line_parts[0] != Keywords.Statement.Selection.IF);
            //         evaluateCondition (line_parameters[1], line_parts[0]);
            //     }
            //     break;
            // case Keywords.Statement.Iteration.FOR:
            //     /* For this case, use the example "for (int i = 5; i < 10; i++) {" */

            //     /* e.g. ["for", "int i = 5", "i < 10", "i++", "{"] */
            //     line_parameters = SubstringHandler.SplitFunction (intermediate_line, Operators.SPLIT_PARAMETERS);

            //     /* Has the for loop been executed in scope before? If so, "i" must've been declared */
            //     if (scope.isVariableInScope (line_parameters[1].Split (' ') [1])) {
            //         /* For loop has run in scope before, modify the variable */

            //         /* e.g. "i++" goes to "i = i + 1" goes to "i = 5 + 1" goes to "i = 6", simplified = true */
            //         line_parameters[3] = Parser.step (line_parameters[3], getVariableHandler (), out line_simplified);

            //         if (line_simplified) {
            //             scope.setVariableInScope (line_parameters[3]);

            //             first_section_finished = true;
            //             script[getPointer()] = line_parts[0] + "ll (" + line_parameters[1] + "; " + line_parameters[2] + "; " + line_parameters[3] + ") {";

            //         }
            //         else {
            //             script[getPointer ()] = line_parts[0] + " (" + line_parameters[1] + "; " + line_parameters[2] + "; " + line_parameters[3] + ") {";
            //             break;
            //         }

            //     } else {
            //         /* For loop has not run in scope before, declare the variable */

            //         /* e.g. "int i = 5", simplified = true */
            //         line_parameters[1] = Parser.step (line_parameters[1], getVariableHandler (), out line_simplified);

            //         if (line_simplified) {
            //             Logger.Log (line_parameters[1]);
            //             scope.declareVariableInScope (line_parameters[1]);
            //             first_section_finished = true;
            //             script[getPointer()] = line_parts[0] + "ll (" + line_parameters[1] + "; " + line_parameters[2] + "; " + line_parameters[3] + ") {";

            //         }
            //         else {
            //             script[getPointer ()] = line_parts[0] + " (" + line_parameters[1] + "; " + line_parameters[2] + "; " + line_parameters[3] + ") {";
            //             break;
            //         }; //don't check conditional in same instant
            //     }
            //     line_simplified = false;
            //     break;

            // case Keywords.Statement.Iteration.FOR + "ll":
            //     line_parameters = SubstringHandler.SplitFunction(intermediate_line, Operators.SPLIT_PARAMETERS);

            //     //if (first_section_finished) {
            //     Logger.Log ("input:\n" + line_parameters[2]);
            //     line_parameters[2] = Parser.step (line_parameters[2], getVariableHandler (), out line_simplified);

            //     Logger.Log("output:\n" + line_parameters[2] + "\n" + script_saved[getPointer()]);

            //     if (line_simplified) {
            //         // scope.push (RangeObject.getScopeRange (script, getPointer ()), line_parts[0] != Keywords.Statement.Selection.IF);
            //         evaluateCondition (line_parameters[2], line_parts[0]);
            //         script[getPointer ()] = script_saved[getPointer ()];
            //     } else {
            //         script[getPointer ()] = line_parts[0] + " (" + line_parameters[1] + "; " + line_parameters[2] + "; " + line_parameters[3] + ") {";
            //     }

            //     break;

            // case Keywords.Type.Value.BOOLEAN:
            // case Keywords.Type.Value.INTEGER:
            // case Keywords.Type.Value.FLOAT:
            // case Keywords.Type.Reference.STRING:
            //     //Primitive Data Types
            //     scope.declareVariableInScope (intermediate_line);
            //     break;
            //     //case Console.NAME:
            //     //case Plotter.NAME:
            //     //Not possible (as far as I know) to hit a function header, so assume it is just a variable declaration
            //     //scope.declareVariableInScope (intermediate_line);
            //     //if (line_parts[0] == Console.NAME) {
            //     //issue with doing this here is that you still ned separate logic when garbage collecting to destroy window...
            //     //}
            //     //break;
            // default:
            //     VariableObject variable_reference;
            //     if (scope.isVariableInScope (line_parts[0])) {
            //         /* CHECK IF LINE REFERS TO A VARIABLE, e.g. "i = 10;" */
            //         scope.setVariableInScope (intermediate_line);

            //     } else if (scope.isVariableInScope (line_parts[0].Split ('.') [0], out variable_reference)) {

            //         /* e.g. "console1.Add(...)" */
            //         // listener_handler.callListener (line, variable_reference, obj);
            //         debugger += "YES\n\n";

            //     } else if (function_handler.isFunction (line_parts[0].Split ('(') [0])) {
            //         /* CHECK IF LINE REFERS TO A FUNCTION, e.g. "print(x);" */
            //         FunctionObject function = function_handler.getFunction (line_parts[0].Split ('(') [0]);
            //         scope.push (RangeObject.returnTo (function.range, getPointer ()), false);

            //     } else if (listener_handler.isFunction (line_parts[0].Split ('(') [0])) {

            //         //TODO:: Not sure if this is accomplishing intended functionality

            //         /* CHECK IF LINE REFERS TO A LISTENER, e.g. "Console.PrintLine("test");" */
            //         // listener_handler.callListener (line,  obj);

            //     } else {
            //         debugger += "Not able to process line" + intermediate_line;
            //     }
            //     break;
        }
        // if (scope.isFinished ()) return true;
        // else {
        //     if (line_simplified) {
        //         scope.step ();
        //         for (int i = 0; i < script.Count; i++)
        //         {
        //             script[i] = script_saved[i];
        //         }
        //     }
        //     // listener_handler.updateListeners (this);
        //     return false;
        // }
    } 
    public void SetTargetClass(string name) 
    {

    }
    // string.Join("\n", scope.peek().method.lines.ToArray())
    public override string ToString() {
        string output = "Target Class:\n";
        foreach (var c in classes) 
        {
            if (c.name == target_class)
            {
                output += c.ToString();
            }
        } 
        return output + "\n" + scope.ToString() + "\n" + heap.ToString() + "\nDebug:\n" + debug_output;
    }
}

public class ScopeController {
    Stack<Scope> stack;
    public ScopeController () {
        stack = new Stack<Scope>();
    }
    public Scope peek() {
        return stack.Peek();
    }
    public void push (Scope scope) {
        stack.Push (scope);
    }
    public void pop () {
        stack.Pop();
    }
    public override string ToString() 
    {
        string output = "Stack:\n";
        foreach (var s in stack.ToArray ()) 
        {
            output += s.ToString () + "\n";
        }
        return output + stack.ToString();
    }
}
public class Scope {
    public string class_name;
    public Method method;
    public int start_index, end_index;
    public bool is_loop;
    public Scope (string class_name, Method method, int start_index, int end_index, bool is_loop) 
    {
        this.class_name = class_name;
        this.method = method;
        this.start_index = start_index;
        this.end_index = end_index;
        this.is_loop = is_loop;
    }
    public override string ToString() 
    {
        return class_name + "." + method.name + "<" + start_index + "," + end_index + ">";
    }
}

public class HeapController 
{
    List<string> heap;
    public HeapController () 
    {
        heap = new List<string>();
    }
    public override string ToString() 
    {
        string output = "Heap:\n";
        foreach (var h in heap.ToArray ()) 
        {
            output += h.ToString () + "\n";
        }
        return output + heap.ToString();
    }
}