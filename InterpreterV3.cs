using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System;

public class InterpreterV3
{
    List<ClassObj> classes;
    ScopeObj scope;
    HeapObj heap;
    string target_class = "", intermediate_line = "", debug_output = "";
    int line;
    /* InterpreterV3 takes a list of classes, one of which must contain a "Main" method */
    public InterpreterV3(List<ClassObj> classes) 
    {
        this.classes = classes;
        this.scope = new ScopeObj();
        this.heap = new HeapObj();
        foreach (var c in classes) 
        {
            foreach (var m in c.methods) 
            {
                if (m.name == "Main") 
                {
                    debug_output += "Found Main ()";
                    scope.push(new Scope(m, 0, m.lines.Count(), false, new List<Field>()));
                    SetTargetClass(c.name);
                }
            }
        }
    }
    /* Step increments the line index after finishing operations for the current line */
    bool update_intermediate = false;
    public bool Step() 
    {
        update_intermediate = true;
        return true;
    }
    /* Interpret executes the next operation found */
    public bool Interpret() 
    {
        if (scope == null) 
        {
            debug_output += "No Main method found";
            return false;
        }
        /* s = local scope */
        Scope s = scope.peek ();
        if (s == null) 
        {
            debug_output += "No local scope";
            return false;
        }
        /* Initialize index to the top of the scope */
        if (line == -1) 
        {
            line = s.start_index;
            debug_output += "\nsetting index " + line;
        }
        if (line == s.end_index) 
        {
            if (s.is_loop) line = s.start_index;
            else scope.pop();
            debug_output += "\nending line " + line;
        }
        /* Grab the current line if empty */
        if (intermediate_line == "" || update_intermediate)  
        {
            line++;
            intermediate_line = s.method.lines[line].Trim();
            update_intermediate = false;
            debug_output += "\nLine " + line + ": \"" + intermediate_line + "\"";
        }
        /* Determine operation */
        switch (intermediate_line[0]) 
        {
            case Operators.DOLLAR_SIGN:
            case Operators.COMMENT_CHAR:
            case Operators.OPENING_BRACKET_CHAR:
            case Operators.CLOSING_BRACKET_CHAR:
                debug_output += "Skipping";
                return Step();
            default:
                debug_output += "Operatable";
                /* Split the intermediate line into its parts */
                string[] line_parts = intermediate_line.Split (Operators.SPACE[0]);
                string[] line_parameters = {};
                var line_simplified = false;
                switch (line_parts[0]) 
                {
                    /* Primitive Data Types (PDTs, Instantiated on Stack) */
                    case Keywords.Type.Value.BOOLEAN:
                    case Keywords.Type.Value.INTEGER:
                    case Keywords.Type.Value.FLOAT:
                        line_parameters = new string[]{Simplify (intermediate_line.Split('=')[1], s, out line_simplified)};
                        intermediate_line = line_parts[0] + " " + line_parts[1] + " = " + line_parameters[0];
                        if (line_simplified)
                        {
                            scope.setPrimitive(intermediate_line);
                            return Step();
                        }
                        return true;
                        // scope.declareVariableInScope (intermediate_line);
                    /* Objects (Instantiated on Heap) */
                    case Keywords.Type.Reference.STRING:
                    case Keywords.Type.Reference.OBJECT:
                        heap.add(intermediate_line); // TODO, generalize to support constructors/heap
                        return Step();
                        // if (scope.hasVariable(line_parts[1]) {
                            // index++; /* Already set, move on */
                        // }
                    case Keywords.Statement.Selection.IF:
                    case Keywords.Statement.Iteration.WHILE:
                        /* e.g. "if (i < 10)"    */
                        line_parameters = SubstringHandler.SplitFunction (intermediate_line, Operators.SPLIT_PARAMETERS);
                        line_parameters[1] = Simplify (line_parameters[1], s, out line_simplified);
                        intermediate_line = line_parts[0] + " (" + line_parameters[1] + ")";
                        var end_line = s.method.EndOfScope(line);
                        if (line_simplified) 
                        {
                            if (line_parameters[1] == "true")
                            {
                                scope.push (new Scope(s.method, line, end_line, false, s.variables));
                            }
                            else 
                            {
                                line = end_line;
                            }
                            return Step();
                        }
                        return true;            
                    default:
                        if (s.hasVariable (line_parts[0]) != -1) 
                        {
                            // debug_output += line_parts[0] + "is variable!\n";
                            
                            if (intermediate_line.Contains(Operators.EQUALS)) 
                            {
                                line_parameters = new string[]{Simplify (intermediate_line.Split('=')[1], s, out line_simplified)};
                                intermediate_line = line_parts[0] + " = " + line_parameters[0];
                                if (line_simplified)
                                {
                                    scope.setPrimitive(intermediate_line);
                                    return Step();
                                }
                                return true;            
                            }
                            /* CHECK IF LINE REFERS TO A VARIABLE, e.g. "i = 10;" */
                            // scope.setPrimitive(intermediate_line);
                        }
                        return Step();
                }
                return true;
            // case Keywords.Statement.Jump.CONTINUE:
            //     index = s.start_index;
            //     break;
            // case Keywords.Statement.Jump.BREAK:
            //     index = -1;
            //     // scope.pop ();
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
        return false;
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
        target_class = name;
    }
    /* Simplify complex statements into simple ones, managing PEMDAS, function calls, etc. */
    public static string Simplify (string line_in, Scope local_scope, out bool simplified) {

        /* Assume given line is not fully simplified until proven otherwise */
        simplified = false;

        string beginning_section = "";

        if (line_in.IndexOf (" = ") >= 0) {
            beginning_section = line_in.Substring (0, line_in.IndexOf (" = ") + 3);
            line_in = line_in.Substring (line_in.IndexOf (" = ") + 2);
        }

        //NOTE: LOOK ALWAYS TO THE RIGHT OF AN "=" SIGN... supports things like "int i = 5 + 10" cleanly with no edge cases, when simplfiied, set i = 15... 

        /* Handling parenthesis, whether for PEDMAS manipulation or function calls */
        if (line_in.Contains (Operators.OPENING_PARENTHESIS)) {

            int start_of_parenthesis = 0;
            string inner_snippet = line_in;

            while (inner_snippet.Contains (Operators.OPENING_PARENTHESIS)) {

                start_of_parenthesis += inner_snippet.IndexOf (Operators.OPENING_PARENTHESIS);
                Logger.Log (inner_snippet);
                inner_snippet = inner_snippet.Substring (inner_snippet.IndexOf (Operators.OPENING_PARENTHESIS) + 1);
                inner_snippet = inner_snippet.Substring (0, getLengthToClosingParenthesis (inner_snippet, 0));
            }

            return beginning_section + line_in.Remove (start_of_parenthesis, inner_snippet.Length + 2)
                .Insert (start_of_parenthesis, SimplifyOperation (inner_snippet, local_scope, out simplified));
        }
        return beginning_section + SimplifyOperation (line_in, local_scope, out simplified);
    }

    private static string SimplifyOperation (string input, Scope local_scope, out bool simplified) {

        /* Example input for this function: "4 + 4 * 4" */
        /* Note: Since step() manages any parenthesis-related PEDMAS, so that isn't handled in this function */

        /* Splits along spaces, e.g. ["4", "+", "4", "*", "4"] */
        List<string> parts = input.Split (Operators.SPLIT, StringSplitOptions.RemoveEmptyEntries).ToList ();

        /* If line is just "x", replace "x" with x's value */
        if (parts.Count () == 1) {
            simplified = true;
            return local_scope.getValue (parts[0]);
        }

        /* Otherwise, parts needs to be condensed before being fully simplified */
        simplified = false;

        /* PEDMAS, e.g. ["4", "+", 4, "*", "4"] ==> ["4", "+", "16"] */
        /* First Modulus, then Multiplication and Division together, then Addition and Subtraction together, etc. */
        for (int operation_set = 0; operation_set < Operators.PEMDAS.Length; operation_set++) {

            /* Look for operations (the odd indices of the parts array)  */
            for (int part = 1; part < parts.Count () - 1; part += 2) {

                /* Check if operation of input matches current order of PEMDAS */
                if (Operators.PEMDAS[operation_set].Contains (parts[part] + Operators.TAB)) {

                    /* Replace any variable's mnemonic name with it's value */
                    string left = local_scope.getValue (parts[part - 1]),
                        right = local_scope.getValue (parts[part + 1]);

                    /* Execute operation between two values, compresses three parts into one, e.g. ["4", "*", "4"] ==> ["16", "*", "4"] ==> ["16"] */
                    parts[part - 1] = Evaluator.simplify (left, parts[part], right);
                    parts.RemoveRange (part, 2);

                    /* Fully simplified */
                    if (parts.Count () == 1) {
                        simplified = true;
                    }

                    /* Recombine parts to resulting, simplified "4 + 16" */
                    /* Note: It chooses to evaluate multiplication before addition, correctly following PEMDAS */
                    return String.Join (Operators.SPACE, parts.ToArray ());
                }
            }
        }
        return "";
    }

    public static int getLengthToClosingParenthesis (string line_in, int start_index) {

        int count = 0;
        int parenthesis_count = 1;

        while (parenthesis_count > 0) {
            count++;
            if (line_in[start_index + count].ToString () == Operators.OPENING_PARENTHESIS) {
                parenthesis_count++;
            } else if (line_in[start_index + count].ToString () == Operators.CLOSING_PARENTHESIS) {
                parenthesis_count--;
            }
        }
        return count;
    }
    // string.Join("\n", scope.peek().method.lines.ToArray())
    public override string ToString() {
        string output = "";
        var s = scope.peek();
        foreach (var c in classes) 
        {
            if (s != null && s.method != null && s.method.class_obj != null && c.name == s.method.class_obj.name)
            {
                if (intermediate_line == "") 
                {
                    output += c.ToString();
                }
                else 
                {
                    output += c.ToString(s.method.name, line, intermediate_line);
                }
            }
        } 
        output += "\n\n" + scope.ToString() + "\n" + heap.ToString() + "\nDebug:\n" + debug_output + "\n\nClock: " + System.DateTime.Now.ToString("hh:mm:ss");
        debug_output = "";
        return output;
    }
}

public class ScopeObj 
{
    Stack<Scope> stack;
    public ScopeObj () {
        stack = new Stack<Scope>();
    }
    public void setPrimitive (string line) 
    {
        stack.Peek().setPrimitive(line);
    }
    public Scope peek() {
        if (stack.Count() == 0) return null;
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
        return output;
    }
}

public class Scope 
{
    public Method method;
    public List<Field> variables;
    public int start_index, end_index;
    public bool is_loop;
    public Scope (Method method, int start_index, int end_index, bool is_loop, List<Field> variables) 
    {
        this.method = method;
        this.variables = variables;
        this.start_index = start_index;
        this.end_index = end_index;
        this.is_loop = is_loop;
    }
    public string getValue (string name) 
    {  
        var i = hasVariable(name);
        if (i == -1) return name;
        return getVariable(i);
    }
    public string getVariable(int index) 
    {
        return variables[index].value;
    }
    public int hasVariable (string name) 
    {
        for (int i = 0; i < variables.Count(); i++)
        { 
            if (variables[i].name == name) return i;
        }
        return -1;
    }
    public void setPrimitive (string line) 
    {   /* e.g. "int i = 123;".Split(' ') */
        /*      ["int", "i", "=", "123;"] */
        /*      [l[0], l[1], l[2], l[3]"] */
        var l = line.Split (' ');
        var i = hasVariable(l[0]);
        if (i != -1) 
        {
            variables[i].value = l[2].Substring(0, l[2].Length - 1);
            return;
        }
        i = hasVariable(l[1]);
        if (i == -1) 
        {
            variables.Add(new Field(l[1], l[0], l[3].Substring(0, l[3].Length - 1)));
        }
        else 
        {
            variables[i].value = l[3].Substring(0, l[3].Length - 1);
        }
    }
    public override string ToString() 
    {
        string output = method.class_obj.name + "." + method.name + "<" + start_index + "," + end_index + ">";
        foreach (var v in variables) 
        {
            output += "\n- " + v.ToString();
        }
        return output;
    }
}

public class HeapObj 
{
    List<string> heap;
    public HeapObj () 
    {
        heap = new List<string>();
    }
    public void add(string input) 
    {
        heap.Add(input);
    }
    public override string ToString() 
    {
        string output = "Heap:\n";
        foreach (var h in heap.ToArray ()) 
        {
            output += h + "\n";
        }
        return output;
    }
}

public class ClassObj
{
    public string name = "";
    public string input = "";
    public List<Field> fields; // primitive data types, objects
    public List<Method> methods; // constructor (initializes fields), getter/setter (modifies fields), main (Processor's start method)

    public ClassObj(string input) {
        this.input = input;
        fields = new List<Field>();
        methods = new List<Method>();
        switch (input) {
            // Iterate️ class
            case "ℹ":
                name = "Iterate";
                methods.Add(new Method(this, "Main", "_Entry_point_", "void", new List<Field>(){new Field("args", "String[]")}, new List<string>(){"//_New_line", "$", "int i = 2;", "if (i + 1 < 5 + 2 * 4 + 10)", "{", "int j = 4;", "i = i + j + 1 + 2 + 3 + i + j;", "j = i + j + 1 + 2 + 3 + i + j;","Object obj = new Object();", "int k = 23 + 54 + 2342;", "}", "String test = \"Hello\";", "Object other = new Object();", "int abc = 123;", "double xyz = 987;", "bool primitive = false;"}));
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
        string output = "class " + name + "\n{\n  //_New_field\n  $\n";
        foreach (var f in fields.ToArray()) {
            output += f.ToString() + "\n";
        }
        foreach (var m in methods.ToArray()) {
            output += m.ToString() + "\n";
        }
        return output + "  //_New_method\n  $\n}";
    }
    public string ToString(string method_name, int index, string intermediate_line) {
        string output = "class " + name + "\n{\n  //_New_field\n  $\n";
        foreach (var f in fields.ToArray())
        {
            output += f.ToString() + "\n";
        }
        foreach (var m in methods.ToArray())
        {
            output += "  /*" + m.comment + "*/\n  " + m.return_type + " " + m.name + " (" + string.Join(", ", m.parameters) + ")\n  {\n";
            if (m.name == method_name) 
            {
                int indent_count = 2;
                for (int i = 0; i < m.lines.Count(); i++) 
                {
                    if (m.lines[i].Contains(Operators.CLOSING_BRACKET))
                    {
                        indent_count--;
                    }
                    if (i == index) 
                    {
                        output += new string(' ', indent_count * 2) + Formatter.Red(intermediate_line.Replace(" ", "_")) + "\n";
                    }
                    else 
                    {
                        output += new string(' ', indent_count * 2) + m.lines[i].Trim() + "\n";
                    }
                    if (m.lines[i].Contains(Operators.OPENING_BRACKET)) 
                    {
                        indent_count++;
                    }
                }
            }
            output += "  }";
        }
        return output + "\n  //_New_method\n  $\n}";
    }
}

public class Field 
{
    public string type, name, value;
    public Field (string name, string type) {
        this.name = name;
        this.type = type;
        this.value = "";
    }
    public Field (string name, string type, string value) {
        this.name = name;
        this.type = type;
        this.value = value;
    }
    public override string ToString () {
        if (value == "") return type + " " + name;
        return type + " " + name + " = " + value + ";";
    }
}

public class Method 
{
    public ClassObj class_obj;
    public string comment, name, return_type;
    public List<Field> parameters;
    public List<string> lines;
    public Method(ClassObj class_obj, string name, string comment, string return_type, List<Field> parameters, List<string> lines) {
        this.class_obj = class_obj;
        this.comment = comment;
        this.name = name;
        this.return_type = return_type;
        this.parameters = parameters;
        this.lines = lines;
    }
    public int EndOfScope (int index) {
        int indent_count = 0;
        for (int i = index + 1; i < lines.Count(); i++)
        {
            if (lines[i].Contains(Operators.OPENING_BRACKET)) 
            {
                indent_count++;
            }
            else if (lines[i].Contains(Operators.CLOSING_BRACKET))
            {
                indent_count--;
                if (indent_count <= 0) 
                {
                    return i;
                }
            }
        }
        return index;
    }
    public string IndentedLines() {
        string output = "";
        int indent_count = 2;
        for (int i = 0; i < lines.Count(); i++)
        {
            if (lines[i].Contains(Operators.CLOSING_BRACKET))
            {
                indent_count--;
            }
            output += new string(' ', indent_count * 2) + lines[i].Trim() + "\n";
            if (lines[i].Contains(Operators.OPENING_BRACKET)) 
            {
                indent_count++;
            }
        }
        return output;
    }
    public override string ToString () {
        return "  /*" + comment + "*/\n  " + return_type + " " + name + " (" + string.Join(", ", parameters) + ")\n  {\n" + IndentedLines() + "  }";
    }
}
