using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System;


public class InterpreterInput {
    public float x, y;
    public bool fire;
    public Dictionary<string, ComponentController> components;
    public InterpreterInput (float x, float y, bool fire, Dictionary<string, ComponentController> components) {
        this.x = x;
        this.y = y;
        this.fire = fire;
        this.components = components;
    }


}

// public InterpreterComponents
// {
        
// }

public class InterpreterV3
{
    public List<ClassObj> classes;
    ScopeObj scope;
    Scope next_scope; // On the next tick, push to the scope stack
    bool previous_scope = false; // On the next tick, pop the scope stack
    HeapObj heap;
    string target_class = "", intermediate_line = "", debug_output = "";
    string[] split_line;
    int line = -1, update_line = -1, end_line = -1;
    bool update_intermediate = false;
    private static int int_result;
    private static float float_result;
    private static bool bool_result;
    // TODO- update clickable button names to be able to track where to make edits (Which class/method/line)
    public InterpreterV3(List<ClassObj> classes) /* InterpreterV3 takes a list of classes, one of which must contain a "Main" method */
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
                    scope.push(new Scope(m, 0, m.lines.Count, false, new List<Field>()));
                    SetTargetClass(c.name);
                }
            }
        }
    }
    public void ResetLine() {
        line = -1;
        this.scope = new ScopeObj();
        this.heap = new HeapObj();
        foreach (var c in classes) 
        {
            foreach (var m in c.methods) 
            {
                if (m.name == "Main") 
                {
                    debug_output += "Found Main ()";
                    scope.push(new Scope(m, 0, m.lines.Count, false, new List<Field>()));
                    SetTargetClass(c.name);
                }
            }
        }
    }
    public string simplifyBooleans (bool left, string arithmetic_operator, bool right)
    {
        switch (arithmetic_operator)
        {
            case Operators.Boolean.EQUALITY:
                return (left == right).ToString ().ToLower();
            case Operators.Boolean.AND:
                return (left && right).ToString ().ToLower();
            case Operators.Boolean.OR:
                return (left || right).ToString ().ToLower();
            default:
                return "";
        }
    }
    public string simplifyIntegers (int left, string arithmetic_operator, int right)
    {
        switch (arithmetic_operator)
        {
            case Operators.Arithmetic.REMAINDER:
                return (left % right).ToString ();
            case Operators.Arithmetic.MULTIPLICATION:
                return (left * right).ToString ();
            case Operators.Arithmetic.DIVISION:
                if (right == 0) return 0.ToString();
                return (left / right).ToString ();
            case Operators.Arithmetic.ADDITION:
                return (left + right).ToString ();
            case Operators.Arithmetic.SUBTRACTION:
                return (left - right).ToString ();
            case Operators.Boolean.EQUALITY:
                return (left == right).ToString ().ToLower();
            case Operators.Boolean.INEQUALITY:
                return (left != right).ToString().ToLower();
            case Operators.Comparison.GREATER_THAN:
                return (left > right).ToString ().ToLower();
            case Operators.Comparison.GREATER_THAN_OR_EQUAL:
                return (left >= right).ToString ().ToLower();
            case Operators.Comparison.LESS_THAN:
                return (left < right).ToString ().ToLower();
            case Operators.Comparison.LESS_THAN_OR_EQUAL:
                return (left <= right).ToString ().ToLower();
            default:
                return "";
        }
    }
    public string simplifyFloats (float left, string arithmetic_operator, float right)
    {
        switch (arithmetic_operator)
        {
            case Operators.Arithmetic.REMAINDER:
                return (left % right).ToString ();
            case Operators.Arithmetic.MULTIPLICATION:
                return (left * right).ToString ();
            case Operators.Arithmetic.DIVISION:
                return (left / right).ToString ();
            case Operators.Arithmetic.ADDITION:
                return (left + right).ToString ();
            case Operators.Arithmetic.SUBTRACTION:
                return (left - right).ToString ();
            case Operators.Boolean.EQUALITY:
                return (left == right).ToString ().ToLower();
            case Operators.Boolean.INEQUALITY:
                return (left != right).ToString().ToLower();
            case Operators.Comparison.GREATER_THAN:
                return (left > right).ToString ().ToLower();
            case Operators.Comparison.GREATER_THAN_OR_EQUAL:
                return (left >= right).ToString ().ToLower();
            case Operators.Comparison.LESS_THAN:
                return (left < right).ToString ().ToLower();
            case Operators.Comparison.LESS_THAN_OR_EQUAL:
                return (left <= right).ToString ().ToLower();
            default:
                return "";
        }
    }
    public string simplifyString (string left, string arithmetic_operator, string right)
    {
        if (left.IndexOf ("\"") == 0) left = left.Substring (1, left.Length - 2);
        if (right.IndexOf ("\"") == 0) right = right.Substring (1, right.Length - 2);
        switch (arithmetic_operator)
        {
            case Operators.Arithmetic.ADDITION:
                return left + right;
            case Operators.Boolean.EQUALITY:
                return (left == right).ToString ().ToLower();
            case Operators.Boolean.INEQUALITY:
                return (left != right).ToString().ToLower();
            default:
                return "";
        }
    }
    public string getType (string input)
    {
        if (bool.TryParse (input, out bool_result)) return Keywords.Type.Value.BOOLEAN;
        if (int.TryParse (input, out int_result)) return Keywords.Type.Value.INTEGER;
        if (float.TryParse (input, out float_result)) return Keywords.Type.Value.FLOAT;
        return Keywords.Type.Reference.STRING;
    }
    public string simplifyCondensedOperators (string variable_name, string arithmetic_operator)
    {
        switch (arithmetic_operator)
        {
            case Operators.EQUALS:
                return Operators.EMPTY;
            case Operators.Arithmetic.Compound.ADDITION:
                return variable_name + " " + Operators.Arithmetic.ADDITION + " " + Operators.OPENING_PARENTHESIS;
            case Operators.Arithmetic.Compound.SUBTRACTION:
                return variable_name + " " + Operators.Arithmetic.SUBTRACTION + " " + Operators.OPENING_PARENTHESIS;
            case Operators.Arithmetic.Compound.MULTIPLICATION:
                return variable_name + " " + Operators.Arithmetic.MULTIPLICATION + " " + Operators.OPENING_PARENTHESIS;
            case Operators.Arithmetic.Compound.DIVISION:
                return variable_name + " " + Operators.Arithmetic.DIVISION + " " + Operators.OPENING_PARENTHESIS;
            case Operators.Arithmetic.Compound.REMAINDER:
                return variable_name + " " + Operators.Arithmetic.REMAINDER + " " + Operators.OPENING_PARENTHESIS;
            default:
                return Operators.EMPTY;
        }

    }
    public string[] splitIncrement (string input)
    {
        string variable_name, variable_operation;
        input = input.Split (Operators.END_LINE_CHAR) [0]; //remove ; if there

        variable_name = input.Substring (0, input.Length - 2);
        variable_operation = input.Substring (input.Length - 2);

        switch (variable_operation)
        {
            case Operators.Arithmetic.Unary.INCREMENT:
                return new string[] { variable_name, Operators.EQUALS, variable_name, Operators.Arithmetic.ADDITION, "1" };
            case Operators.Arithmetic.Unary.DECREMEMT:
                return new string[] { variable_name, Operators.EQUALS, variable_name, Operators.Arithmetic.SUBTRACTION, "1" };

        }
        return new string[] { };
    }
    public string cast (string input, string cast_type)
    {
        if (input != Operators.EMPTY)
        { //is this necessary? probably...&& Evaluator.getType (getValue (input)) == cast_type)
            switch (cast_type)
            {
                case Keywords.Type.Value.BOOLEAN:
                    return bool.Parse (input).ToString ();
                case Keywords.Type.Value.INTEGER:
                    return int.Parse (input).ToString ();
                case Keywords.Type.Value.FLOAT:
                    return float.Parse (input).ToString ();
                case Keywords.Type.Reference.STRING:
                    return input;
            }
        }
        //add cases to convert floats to ints, etc? 
        //but, preferrably implement casting (int.Parse...)
        return Operators.EMPTY;
    }
    
    public bool Step() /* Step increments the line index after finishing operations for the current line */
    {
        update_intermediate = true;
        return true;
    }
    public bool Interpret(InterpreterInput input) /* Interpret executes the next operation found */
    {
        if (scope == null) 
        {
            debug_output += "No Main method found";
            return false;
        }
        Scope s = scope.peek (); /* s = local scope */
        if (s == null) 
        {
            debug_output += "No local scope";
            return false;
        }
        if (previous_scope) 
        {
            previous_scope = false;
            line = scope.pop();
            s = scope.peek ();
            intermediate_line = "void";//s.method.lines[line].Trim().TrimEnd(';');
            update_intermediate = true;
            return true;
        }
        if (next_scope != null) 
        {
            scope.push (next_scope);
            // intermediate_line = "";
            line = next_scope.start_index - 1;
            next_scope = null;
            update_intermediate = true;
            return true;
        }
        if (intermediate_line == "" || update_intermediate) /* Grab the current line if empty */
        {
            if (update_line != -1) 
            {
                line = update_line;
                update_line = -1;
            }
            else 
            {
                line++;
            }
            if (line >= s.method.lines.Count) 
            {
                update_line = scope.pop();
                return true;
                // line = 0;
            }
            intermediate_line = s.method.lines[line].Trim();//.TrimEnd(';');
            update_intermediate = false;
        }
        if (intermediate_line.Contains("System.in.X")) {
            intermediate_line = intermediate_line.Replace("System.in.X", input.x.ToString("0.00"));
            input.x = 0;
            // return true;
        }
        if (intermediate_line.Contains("System.in.Y")) {
            intermediate_line = intermediate_line.Replace("System.in.Y", input.y.ToString("0.00"));
            input.y = 0;
            // return true;
        }
        if (intermediate_line.Contains("System.in.Button")) {
            intermediate_line = intermediate_line.Replace("System.in.Button", input.fire.ToString().ToLower());
            input.fire = false;
            // return true;
        }
        debug_output += line + ": \"" + intermediate_line + "\"\n";
        /* Determine operation */
        switch (intermediate_line[0]) 
        {
            case Operators.DOLLAR_SIGN:
            case Operators.COMMENT_CHAR:
            case Operators.OPENING_BRACKET_CHAR:
                return Step();
            case Operators.CLOSING_BRACKET_CHAR:
                if (s.is_loop) update_line = s.start_index;
                else if (scope.count() > 1) update_line = scope.pop();
                return Step();
            default:
                /* Split the intermediate line into its parts */
                string[] line_parts = intermediate_line.Split (Operators.SPACE[0]);
                string[] line_parameters = {};
                var line_simplified = false;
                /* Determine operation */
                switch (line_parts[0]) 
                {
                    case Keywords.Statement.Jump.RETURN:
                        previous_scope = true;
                        // update_line = scope.pop ();
                        // update_intermediate = true;
                        return true;
                    case Keywords.Statement.Jump.CONTINUE:
                        line = s.start_index;
                        return true;
                    case Keywords.Statement.Jump.BREAK:
                        line = s.end_index;
                        update_line = scope.pop ();
                        return true;
                    /* Primitive Data Types (PDTs, Instantiated on Stack) */
                    case Keywords.Type.Value.BOOLEAN:
                    case Keywords.Type.Value.INTEGER:
                    case Keywords.Type.Value.DOUBLE:
                        line_parameters = new string[]{Simplify (intermediate_line.Split(Operators.EQUALS[0])[1], s, input, out line_simplified)};
                        intermediate_line = line_parts[0] + " " + line_parts[1] + " = " + line_parameters[0];
                        if (line_simplified)
                        {
                            scope.setPrimitive(intermediate_line.TrimEnd(';'));
                            return Step();
                        }
                        return true;
                        // scope.declareVariableInScope (intermediate_line);
                    /* Objects (Instantiated on Heap) */
                    case Keywords.Type.Reference.STRING:
                    case Keywords.Type.Reference.OBJECT:
                        // heap.add(new Field(intermediate_line)); // TODO, generalize to support constructors/heap
                        return Step();
                        // if (scope.hasVariable(line_parts[1])
                            // index++; /* Already set, move on */
                        // }
                    case Keywords.Statement.Selection.IF:
                    case Keywords.Statement.Iteration.WHILE:
                        /* e.g. "if (i < 10)"    */
                        line_parameters = SubstringHandler.SplitFunction (intermediate_line, Operators.SPLIT_PARAMETERS);
                        line_parameters[1] = Simplify (line_parameters[1], s, input, out line_simplified);
                        intermediate_line = line_parts[0] + " (" + line_parameters[1] + ")";
                        if (line_simplified) 
                        {
                            end_line = s.method.EndOfScope(line);
                            if (line_parameters[1] == "true")
                            {
                                // next_scope = new Scope(s.method, line, end_line, false, s.variables.ToList());
                                scope.push (new Scope(s.method, line, end_line, false, s.variables.ToList())); //.ToList() clones via shallow copy
                            }
                            else 
                            {
                                update_line = end_line;
                            }
                            return Step();
                        }
                        return true;
                    case Keywords.Statement.Iteration.FOR:
                        /* e.g. "for (int i = 5; i < 10; i++)" */
                        line_parameters = SubstringHandler.SplitFunction (intermediate_line, Operators.SPLIT_PARAMETERS);
                        if (line == s.start_index)
                        {
                            /* For loop has run in scope before, modify the variable */
                            line_parameters[3] = Simplify (line_parameters[3], s, input, out line_simplified);
                            if (line_simplified)
                            {
                                scope.setPrimitive (line_parameters[3]);
                                line_parameters[2] = Simplify (line_parameters[2], s, input, out line_simplified);

                                if (line_simplified) 
                                {
                                    end_line = s.method.EndOfScope(line);
                                    intermediate_line = line_parts[0] + " (" + line_parameters[1] + "; " + line_parameters[2] + "; " + line_parameters[3] + ")";
                                    if (line_parameters[2] == "true")
                                    {
                                        return Step();
                                    }
                                    else 
                                    {
                                        update_line = scope.pop();//end_line + 1;
                                        return true;
                                    }
                                }
                                else 
                                {
                                    intermediate_line = line_parts[0] + " (" + line_parameters[1] + "; " + line_parameters[2] + "; " + line_parameters[3] + ")";
                                    return true;
                                }
                            }
                            else 
                            {
                                intermediate_line = line_parts[0] + " (" + line_parameters[1] + "; " + line_parameters[2] + "; " + line_parameters[3] + ")";
                                return true;
                            }
                        } 
                        else 
                        {
                            /* For loop has not run in scope before, declare the variable */
                            line_parameters[1] = Simplify (line_parameters[1], s, input, out line_simplified);
                            if (line_simplified)
                            {
                                scope.setPrimitive (line_parameters[1]);
                                line_parameters[2] = Simplify (line_parameters[2], s, input, out line_simplified);
                                if (line_simplified) 
                                {
                                    end_line = s.method.EndOfScope(line);
                                    intermediate_line = line_parts[0] + " (" + line_parameters[1] + "; " + line_parameters[2] + "; " + line_parameters[3] + ")";
                                    if (line_parameters[2] == "true")
                                    {
                                        scope.push (new Scope(s.method, line, end_line, true, s.variables.ToList())); //.ToList() clones via shallow copy

                                        // next_scope = new Scope(s.method, line, end_line, true, s.variables.ToList());
                                        return Step();
                                        // scope.push ()); //.ToList() clones via shallow copy
                                    }
                                    else 
                                    {
                                        update_line = end_line + 1;
                                        return Step();
                                    }
                                }
                                else 
                                {
                                    intermediate_line = line_parts[0] + " (" + line_parameters[1] + "; " + line_parameters[2] + "; " + line_parameters[3] + ")";
                                    return true;
                                }
                            }
                            else 
                            {
                                intermediate_line = line_parts[0] + " (" + line_parameters[1] + "; " + line_parameters[2] + "; " + line_parameters[3] + ")";
                                return true;
                            }
                        }
                        line_simplified = false;
                        break;
                    default:
                        if (s.hasVariable (line_parts[0]) != -1) 
                        {
                            if (intermediate_line.Contains(Operators.EQUALS)) 
                            {
                                line_parameters = new string[]{Simplify (intermediate_line, s, input, out line_simplified)};
                                // intermediate_line = line_parts[0] + " = " + line_parameters[0];
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
                        else
                        {
                            foreach (var m in s.method.class_obj.methods)
                            {
                                if (line_parts[0] == m.name)
                                {
                                    if (s.method.name == m.name) 
                                    {
                                        // update_line = -1;
                                        // return Step();
                                        return true;
                                    }
                                    else 
                                    {
                                        next_scope = new Scope(m, 0, m.lines.Count, false, s.method.class_obj.fields.ToList()); //.ToList() shallow-copies
                                        s.return_index = line;
                                        return true;
                                    }
                                }
                            }
                            foreach (var c in classes)
                            {
                                if (line_parts[0] == c.name)
                                {
                                    // heap.add(intermediate_line);
                                    return Step();
                                }
                                // foreach (var h in heap)
                                // {
                                    

                                // }
                                foreach (var m in c.methods)
                                {
                                    if (line_parts[0] == c.name + "." + m.name)
                                    {
                                        line_parameters = SubstringHandler.SplitFunction (intermediate_line, Operators.SPLIT_PARAMETERS);
                                        if (line_parameters[1] == "") {
                                            input.components[c.name].Action();
                                            return Step();
                                        }
                                        line_parameters[1] = Simplify (line_parameters[1], s, input, out line_simplified);
                                        intermediate_line = line_parts[0] + " (" + line_parameters[1] + ");";

                                        // next_scope = new Scope(m, 0, m.lines.Count, false, s.method.class_obj.fields.ToList());
                                        // s.return_index = line;
                                        // line_parts[1] = Simplify (line_parts[1], s, out line_simplified);
                                        // intermediate_line = line_parts[0] + " (" + line_parts[1].TrimEnd(';') + ");";
                                        if (line_simplified && input.components.ContainsKey(c.name)) {
                                            input.components[c.name].Action(float.Parse(line_parameters[1]));
                                            return Step();
                                        }
                                        return true;
                                    }
                                }
                            }
                            debug_output += "\nNot able to process";
                            return Step();
                        }
                        return Step();
                }
                return true;
        }
        return false;
    } 
    public void SetTargetClass(string name) 
    {
        target_class = name;
    }
    /* Simplify complex statements into simple ones, managing PEMDAS, function calls, etc. */
    public string Simplify (string line_in, Scope local_scope, InterpreterInput input, out bool simplified)
    {
        if (line_in == "true" || line_in == "false")  {
            simplified = true;
            return line_in;
        }
        // var prefix = "";
        // if (line_in.Contains(Operators.EQUALS)) {
        //     split_line = line_in.Split(Operators.EQUALS_CHAR);
        //     prefix = split_line[0] + "= ";
        //     line_in = split_line[1];
        // }
        // /* Assume given line is not fully simplified until proven otherwise */
        // simplified = false;
        
        // /* Handling parenthesis, whether for PEDMAS manipulation or function calls */
        // if (line_in.Contains (Operators.OPENING_PARENTHESIS))
        // {
        //     int start_of_parenthesis = 0;
        //     string inner_snippet = line_in;
        //     while (inner_snippet.Contains (Operators.OPENING_PARENTHESIS))
        //     {
        //         start_of_parenthesis += inner_snippet.IndexOf (Operators.OPENING_PARENTHESIS);
        //         inner_snippet = inner_snippet.Substring (inner_snippet.IndexOf (Operators.OPENING_PARENTHESIS) + 1);
        //         inner_snippet = inner_snippet.Substring (0, getLengthToClosingParenthesis (inner_snippet, 0));
        //     }
        //     return prefix + line_in.Remove (start_of_parenthesis, inner_snippet.Length + 2)
        //         .Insert (start_of_parenthesis, SimplifyOperation (inner_snippet, local_scope, input, out simplified));
        // }
        // return prefix + 
        return SimplifyOperation (line_in, local_scope, input, out simplified);
    }

    private string SimplifyOperation (string line_in, Scope local_scope, InterpreterInput input, out bool simplified)
    {
        /* e.g. "4 + 4 * 4" */
        List<string> parts = line_in.Split (Operators.SPLIT, StringSplitOptions.RemoveEmptyEntries).ToList ();
        
        
        /* Function Calls first */
        foreach (var c in classes) {
            foreach (var m in c.methods) {
                for (int part = 0; part < parts.Count - 1; part ++) {
                    if (parts[part] == c.name + "." + m.name)
                    {
                        if (parts[part+1] == "()") {
                            parts[part] = input.components[c.name].Action().ToString("0.00");
                            parts.RemoveRange (part + 1, 1);
                            simplified = false;
                            return String.Join (Operators.SPACE, parts.ToArray ());
                        } else {
                            parts[part] = input.components[c.name].Action( float.Parse(parts[part + 1].Substring(1, parts[part + 1].Length - 2))).ToString("0.00");
                            parts.RemoveRange (part + 1, 1);
                            simplified = false;
                            return String.Join (Operators.SPACE, parts.ToArray ());
                        }
                    }
                }
            }
        }
        
        /* ["4", "+", "4", "*", "4"] */
        if (parts.Count == 1)
        {
            simplified = true;
            /* If a variable, convert to temporary value */
            return local_scope.getValue (parts[0]); 
        }


        simplified = false;
        /* PEDMAS, e.g. ["4", "+", 4, "*", "4"] ==> ["4", "+", "16"] */
        /* First Modulus, then Multiplication and Division together, then Addition and Subtraction together, etc. */
        for (int operation_set = 0; operation_set < Operators.PEMDAS.Length; operation_set++)
        {
            /* Look for operations (the odd indices of the parts array)  */
            for (int part = 1; part < parts.Count - 1; part += 2)
            {
                /* Check if operation of line_in matches current order of PEMDAS */
                if (Operators.PEMDAS[operation_set].Contains (parts[part] + Operators.TAB))
                {
                    /* Replace any variable's mnemonic name with it's value */
                    string left = local_scope.getValue (parts[part - 1]),
                        right = local_scope.getValue (parts[part + 1]);
                    /* Execute operation between two values, compresses three parts into one */
                    /* ["4", "*", "4"] --> ["16", "*", "4"] --> ["16"] */
                    parts[part - 1] = Equate (left, parts[part], right);
                    parts.RemoveRange (part, 2);
                    /* Fully simplified */
                    if (parts.Count == 1)
                    {
                        simplified = true;
                        /* If a variable, convert to temporary value */
                        return local_scope.getValue (parts[0]);
                    }
                    /* Recombine parts to resulting, simplified "4 + 16" */
                    return String.Join (Operators.SPACE, parts.ToArray ());
                }
            }
        }
        return "";
    }
    public static int getLengthToClosingParenthesis (string line_in, int start_index)
    {

        int count = 0;
        int parenthesis_count = 1;

        while (parenthesis_count > 0)
        {
            count++;
            if (line_in[start_index + count].ToString () == Operators.OPENING_PARENTHESIS)
            {
                parenthesis_count++;
            } else if (line_in[start_index + count].ToString () == Operators.CLOSING_PARENTHESIS)
            {
                parenthesis_count--;
            }
        }
        return count;
    }
    public string Equate (string left, string arithmetic_operator, string right)
    {
        /* e.g. ["12", "*", "4"] ==> ["48"] */
        string left_type = getType (left), right_type = getType (right);
        if (left_type == right_type)
        {
            switch (left_type)
            {
                case Keywords.Type.Value.BOOLEAN:
                    return simplifyBooleans (bool.Parse (left), arithmetic_operator, bool.Parse (right));
                case Keywords.Type.Value.INTEGER:
                    return simplifyIntegers (int.Parse (left), arithmetic_operator, int.Parse (right));
                case Keywords.Type.Value.FLOAT:
                    return simplifyFloats (float.Parse (left), arithmetic_operator, float.Parse (right));
                case Keywords.Type.Reference.STRING:
                    return simplifyString (left, arithmetic_operator, right);
            }
        }
        if ((left_type == Keywords.Type.Value.FLOAT && right_type == Keywords.Type.Value.INTEGER) || (right_type == Keywords.Type.Value.FLOAT && left_type == Keywords.Type.Value.INTEGER))
        {
            /* AUTO TYPE CASTING INTEGERS IN FLOAT CALCULATIONS, e.g. "12 / 1.0" == "12.0", not "12" */
            return simplifyFloats (float.Parse (left), arithmetic_operator, float.Parse (right));
        }
        /* CASTS NOT HANDLED, e.g. "true + 1.0", TREAT AS STRINGS */
        return simplifyString (left, arithmetic_operator, right);
    }
    public string ToDebug()
    {
        var output = "\nStack" + scope.ToString() + "\n" + heap.ToString() + "\n" + debug_output;
        debug_output = "";
        return output;
    }
    public string[] GetMethods(string class_name) {
        foreach (var c in classes) {
            if (c.name == class_name) {
                List<string> method_names = new List<string>();
                foreach (var method_instance in c.methods) {
                    method_names.Add(method_instance.name);
                }
                return method_names.ToArray();
            }
        }
        return null;
    }
    public string GetMethodParameter(string class_name, string method_name) {
        foreach (var c in classes) {
            if (c.name == class_name) {
                List<string> method_names = new List<string>();
                foreach (var method_instance in c.methods) {
                    if (method_instance.name == method_name && method_instance.parameters.Count >= 1) {
                        return method_instance.parameters[0].type;
                    }
                }
            }
        }
        return "";
    }
    public string ToString(string class_name)
    {
        string output = "";
        foreach (var c in classes) 
        {
            if (c.name == class_name) {
                if (scope != null) 
                {
                    var s = scope.peek();
                    if (s != null && s.method != null && s.method.class_obj != null && c.name == s.method.class_obj.name && intermediate_line != "") 
                    {
                        output += c.ToString(s.method.name, line, intermediate_line);
                    }
                    else 
                    {
                        output += c.ToString();
                    }
                }
                else 
                {
                    output += c.ToString();
                }
            }
        }         
        // output += "\n\n" + scope.ToString() + "\n" + heap.ToString() + "\nDebug:\n" + debug_output + "\n\nClock: " + System.DateTime.Now.ToString("hh:mm:ss")+ "\n";
        return output;
    }
    public override string ToString()
    {
        string output = "";
        foreach (var c in classes) 
        {
            if (scope != null) 
            {
                var s = scope.peek();
                if (s != null && s.method != null && s.method.class_obj != null && c.name == s.method.class_obj.name && intermediate_line != "") 
                {
                    output += c.ToString(s.method.name, line, intermediate_line);
                }
                else 
                {
                    output += c.ToString();
                }
            }
            else 
            {
                output += c.ToString();
            }
        } 
        // output += "\n\n" + scope.ToString() + "\n" + heap.ToString() + "\nDebug:\n" + debug_output + "\n\nClock: " + System.DateTime.Now.ToString("hh:mm:ss") + "\n";
        // debug_output = "";
        return output;
    }
}

public class ScopeObj 
{
    Stack<Scope> stack;
    public ScopeObj ()
    {
        stack = new Stack<Scope>();
    }
    public void setPrimitive (string line) 
    {
        stack.Peek().setPrimitive(line);
    }
    public int count()
    {
        return stack.Count;
    }
    public Scope peek()
    {
        if (count() == 0) return null;
        return stack.Peek();
    }
    public void push (Scope scope)
    {
        stack.Push (scope);
    }
    public int pop ()
    {
        if (stack.Count == 0) return -1; 
        stack.Pop();
        if (stack.Count == 0) return -1; 
        return stack.Peek().return_index;
    }
    public override string ToString() 
    {
        string output = "";
        if (stack.Count > 0) {
            output += stack.Peek().VariableString();
            foreach (var s in stack.ToArray ()) 
            {
                output += s.ToString () + "\n";
            }
        }
        return output;
    }
}

public class Scope 
{
    public Method method;
    public List<Field> variables;
    public int start_index, end_index, return_index;
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
        for (int i = 0; i < variables.Count; i++)
        { 
            if (variables[i].name == name) return i;
        }
        return -1;
    }
    public void setPrimitive (string line) 
    {
        var split_line = line.Split (' ');
        int variable_index;
        switch (split_line[0]) {
            case Keywords.Type.Value.BOOLEAN:
            case Keywords.Type.Value.INTEGER:
            case Keywords.Type.Value.DOUBLE:
                /* ["int", "i", "=", "123"] */
                variable_index = hasVariable(split_line[1]);
                if (variable_index == -1) 
                {
                    variables.Add(new Field(split_line[1], split_line[0], split_line[3]));
                }
                else 
                {
                    variables[variable_index].value = split_line[3];
                }
                return;
            default:
                /* ["i", "=", "0"] */
                variable_index = hasVariable(split_line[0]);
                variables[variable_index].value = split_line[2];
                return;
        }
    }
    public void setObject (string line) 
    {
        var split_line = line.Split (' ');
        int variable_index;
        switch (split_line[0]) {
            case Keywords.Type.Reference.OBJECT:
            case Keywords.Type.Reference.STRING:
                /* ["Object", "o", "=", "new Object ()"] */
                variable_index = hasVariable(split_line[1]);
                if (variable_index == -1) 
                {
                    // heap.Add(new Field(split_line[1], split_line[0], split_line[3]));
                }
                else 
                {
                    // heap[variable_index].value = split_line[3];
                }
                return;
            default:
                /* ["i", "=", "0"] */
                // variable_index = hasVariable(split_line[0]);
                // heap[variable_index].value = split_line[2];
                return;
        }
    }
    public string VariableString()
    {
        string output = "";
        foreach (var v in variables) 
        {
            output += "- " + v.ToString() + "\n";
        }
        return output;
    }
    public override string ToString() 
    {
        return method.class_obj.name + "." + method.name + "<" + start_index + "," + end_index + "> " + return_index;
    }
}
public class HeapObj 
{
    List<Field> heap;
    public HeapObj () 
    {
        heap = new List<Field>();
    }
    public void add (Field f) 
    {
        foreach (var h in heap) 
        {
            if (h.name == f.name) 
            {
                heap.Remove(h);
            }
        }
        heap.Add(f);
    }
    public override string ToString() 
    {
        string output = "Heap:\n";
        foreach (var h in heap.ToArray ()) 
        {
            output += h + ";\n";
        }
        return output;
    }
}
public class ClassObj
{
    public string name = "", parent_name = "";
    public string input = "";
    public List<Field> fields; // primitive data types, objects
    public List<Method> methods; // constructor (initializes fields), getter/setter (modifies fields), main (Processor's start method)

    public ClassObj(string input)
    {
        this.input = input;
        fields = new List<Field>();
        methods = new List<Method>();
        switch (input)
        {

                // new ClassObj("▩ Process"),
                // new ClassObj("▥ Bulk"),
                // new ClassObj("◉ Engine"),
                // new ClassObj("◎ Right"),
                // new ClassObj("◎ Left")


            // Iterate️ class
            case "ℹ":
                name = "Iterate";
                methods.Add(new Method(this, "Main", "_Entry_point_", "void", new List<Field>(){new Field("args", "String[]")}, new List<string>(){"A a = new A ();", "for (int i = 0; i < 10; i = i + 1)", "{", "a.Test ();", "}"}));
                // methods.Add(new Method(this, "One", "_Example_method_", "void", new List<Field>(), new List<string>(){"int i = 0;", "for (int j = -2; j < 2; j = j + 1)", "{", "Two ();", "}", "return;"}));
                // methods.Add(new Method(this, "Two", "_Another_method_", "void", new List<Field>(), new List<string>(){"Object obj = new Object ();", "for (int z = 4; z > -4; z = z - 2)", "{", "Three ();", "}", "return;"}));
                 // methods.Add(new Method("SumArray (int[] input)", "Return_sum_of_input_array", "int", "  int total = 0;\n  for (int i = 0; i < input.Length(); i++)\n  {\n    total += input[i];\n  }\n  return total;"));
                break;
            case "a": //Test class "a"
                name = "A";
                methods.Add(new Method(this, "A", "_Constructor_", "void", new List<Field>(), new List<string>(){"return;"}));
                methods.Add(new Method(this, "Test", "_Test_method_", "void", new List<Field>(), new List<string>(){"String word = \"apple\";", "return;"}));
                break;
            // BitNaughts constant class
            case "▩ Process": //Booster
            // "Booster l;", "Booster r;", "Thruster t;", "while (true) {", "l.Fire(Input.X);", "r.Fire(-Input.X);", "t.Fire(Input.Y);", "if (Input.UseWeapon()) {", "l.Fire(-1);", "r.Fire(-1);", "}", "}"
                parent_name = "Processor";
                name = "Process";
                // fields.AddRange(new List<Field>(){
                //     new Field("Thrust = new Thruster (3, 4, 5, 6);", "Thruster"), 
                //     new Field("Right = new Booster (4, 3, 2, 1);", "Booster"), 
                //     new Field("Left = new Booster (1, 2, 3, 4);", "Booster")
                // });
                methods.Add(new Method(this, "Main", "_Entry_point_", "void", new List<Field>(){new Field("args", "String[]")}, new List<string>(){"while (true)", "{", "double y = Input.Y;", "double x = Input.X;", "Engine.Throttle (y * 50);", "Right.Boost (x * -50);", "Left.Boost (x * 50);", "if (Input.Fire)", "{", "Right.Launch ();", "Left.Launch ();", "}", "$", "}"})); //  "}", 
                // methods.Add(new Method("Boost ()", "Throttle_control", "void", "throttle = 100;"));
                // methods.Add(new Method("Launch ()", "Torpedo_control", "Torpedo", "return new Torpedo( GetLoadedBarrel() );"));
                // methods.Add(new Method("Ping ()", "Called_periodically", "void", "throttle--;"));
                break;
            case "▩ ScanProcess":

            // "Booster l;", "Booster r;", "Thruster t;", "while (true) {", "l.Fire(Input.X);", "r.Fire(-Input.X);", "t.Fire(Input.Y);", "if (Input.UseWeapon()) {", "l.Fire(-1);", "r.Fire(-1);", "}", "}"
                parent_name = "Processor";
                name = "Process";
                // fields.AddRange(new List<Field>(){
                //     new Field("Thrust = new Thruster (3, 4, 5, 6);", "Thruster"), 
                //     new Field("Right = new Booster (4, 3, 2, 1);", "Booster"), 
                //     new Field("Left = new Booster (1, 2, 3, 4);", "Booster")
                // });
                methods.Add(new Method(this, "Main", "_Entry_point_", "void", new List<Field>(){new Field("args", "String[]")}, new List<string>(){ "$" })); //"if (r.GetLength () > 0)", "{", "print (r.GetLength ());", "}", "}"})); //  "}", 
                // EXAMPLE IMPLEMENTATION
                // methods.Add(new Method(this, "Main", "_Entry_point_", "void", new List<Field>(){new Field("args", "String[]")}, new List<string>(){"while (true)", "{", "Engine.Throttle (Input.Y * 10);", "Turret.Rotate (Input.X * 10);", "if (Scanner.Scan () != 0)", "{", "System.out.Println(\"Detected Martian!\");", "}", "$", "}" })); //"if (r.GetLength () > 0)", "{", "print (r.GetLength ());", "}", "}"})); //  "}", 
               
                break;
            case "▩ GroverProcess":

            // "Booster l;", "Booster r;", "Thruster t;", "while (true) {", "l.Fire(Input.X);", "r.Fire(-Input.X);", "t.Fire(Input.Y);", "if (Input.UseWeapon()) {", "l.Fire(-1);", "r.Fire(-1);", "}", "}"
                parent_name = "Processor";
                name = "Process";
                // fields.AddRange(new List<Field>(){
                //     new Field("Thrust = new Thruster (3, 4, 5, 6);", "Thruster"), 
                //     new Field("Right = new Booster (4, 3, 2, 1);", "Booster"), 
                //     new Field("Left = new Booster (1, 2, 3, 4);", "Booster")
                // });
                methods.Add(new Method(this, "Main", "_Entry_point_", "void", new List<Field>(){new Field("args", "String[]")}, new List<string>(){"while (true)", "{", "RightEngine.Throttle (Input.Y * 50);", "RightEngine.Throttle (Input.X * -25);", "if (Scanner.Scan () != 0)", "{", "RightCannon.Fire ();", "LeftCannon.Fire ();", "}", "LeftEngine.Throttle (Input.Y * 50);",  "LeftEngine.Throttle (Input.X * 25);", "$", "}" })); //"if (r.GetLength () > 0)", "{", "print (r.GetLength ());", "}", "}"})); //  "}",  
                break;
            case "◉ Engine": //Thruster
                parent_name = "Thruster";
                name = "Engine";
                fields.Add(new Field("throttle", "double", "0"));
                methods.Add(new Method(this, "Throttle", "_Throttle_Control_", "void", new List<Field>(){new Field("input", "double")}, new List<string>(){ "throttle += input;" }));
                break;
            case "◉ RightEngine":
                parent_name = "Thruster";
                name = "RightEngine";
                fields.Add(new Field("throttle", "double", "0"));
                methods.Add(new Method(this, "Throttle", "_Throttle_Control_", "void", new List<Field>(){new Field("input", "double")}, new List<string>(){ "throttle += input;" }));
                break;
            case "◉ LeftEngine":
                parent_name = "Thruster";
                name = "LeftEngine";
                fields.Add(new Field("throttle", "double", "0"));
                methods.Add(new Method(this, "Throttle", "_Throttle_Control_", "void", new List<Field>(){new Field("input", "double")}, new List<string>(){ "throttle += input;" }));
                break;
            case "◎ Right":
                parent_name = "Booster";
                name = "Right";
                fields.Add(new Field("throttle", "double", "0"));
                methods.Add(new Method(this, "Boost", "_Boost_Control_", "void", new List<Field>(){new Field("input", "double")}, new List<string>(){ "throttle += input;" }));
                methods.Add(new Method(this, "Launch", "_Fire_Torpedo_", "void", new List<Field>(), new List<string>(){"new Torpedo();"}));
                break;
            case "◎ Left":
                parent_name = "Booster";
                name = "Left";
                fields.Add(new Field("throttle", "double", "0"));
                methods.Add(new Method(this, "Boost", "_Boost_Control_", "void", new List<Field>(){new Field("input", "double")}, new List<string>(){ "throttle += input;" }));
                methods.Add(new Method(this, "Launch", "_Fire_Torpedo_", "void", new List<Field>(), new List<string>(){"new Torpedo();"}));
                break;
            // case "◍": //Cannon
            //     name = "Processor";
            //     methods.Add(new Method(this, "Main", "_Entry_point_", "void", new List<Field>(){new Field("args", "String[]")}, new List<string>(){"Cannon cl;", "Cannon cr;", "Thruster tl;", "Thruster tr;", "while (true) {", "tl.Fire (Input.X);", "tr.Fire (-Input.X);", "tl.Fire (Input.Y);", "tr.Fire (Input.Y);", "if (Input.UseWeapon ()) {", "cl.Fire(-1);", "cr.Fire(-1);", "}", "}"}));
            //     // methods.Add(new Method("Fire ()", "Use_weapon_control ", "Shell", "return new Shell( GetLoadedBarrel() );"));
            //     break;
            case "▥ Bulk": //Bulkhead
                parent_name = "Bulkhead";
                name = "Bulk";
                fields.Add(new Field("heap", "Heap", "new Heap()"));
                methods.Add(new Method(this, "New", "_Memory_Allocation_", "void", new List<Field>(){new Field("name", "string")}, new List<string>(){"heap.Add (name);"}));
                methods.Add(new Method(this, "Delete", "_Memory_Deallocation_", "void", new List<Field>(){new Field("name", "string")}, new List<string>(){"heap.Remove (name);"}));
                break;
            case "▩": //Processor
                name = "Processor";
                fields.Add(new Field("stack", "Stack", "new Stack()"));
                // methods.Add(new Method("Main (string[] args)", "Main_Method_(entry_point)", "void", "$"));
                break;
            case "▣ Turret": //Gimbal
                parent_name = "Gimbal";
                name = "Turret";
                // methods.Add(new Method("RotateCW ()", "Rotation_control_(cw)", "void", "rot += 15;"));
                // methods.Add(new Method("RotateCCW ()", "Rotation_control_(ccw)", "void", "rot -= 15;"));
                methods.Add(new Method(this, "Rotate", "_Gimbal_Control_", "void", new List<Field>(), new List<string>(){"rot += delta;"}));
                break;
            case "◍ RightCannon":
                parent_name = "Cannon";
                name = "RightCannon";                
                methods.Add(new Method(this, "Fire", "_Shoot_Shell_", "void", new List<Field>(), new List<string>(){"new Shell ();"}));
                    break;
            case "◍ LeftCannon":
                parent_name = "Cannon";
                name = "LeftCannon";                
                methods.Add(new Method(this, "Fire", "_Shoot_Shell_", "void", new List<Field>(), new List<string>(){"new Shell ();"}));
                break;
            case "◌ Scanner": //Sensor
                parent_name = "Sensor";
                name = "Scanner";                
                methods.Add(new Method(this, "Scan", "_Cast_Ray_", "double", new List<Field>(), new List<string>(){"return new Ray ().GetLength ();"}));
                // methods.Add(new Method("Scan ()", "Measure_distance", "double", "new Ray().Length();"));
                break;
            case "▦": //Printer
                fields.Add(new Field("nozzel", "Nozzel", "new Nozzel()"));
                // methods.Add(new Method("Main (string[] args)", "Main_Method_(entry_point)", "void", "$"));
                break;
            case "▨ Antenna":
                parent_name = "Girder";
                name = "Antenna";
                break;
            default:
                // general classes
                // for field in fields, add
                // for method in method, add
                break;
        }
        // fields["test"] = component.GetTypeClass().ToString();
    }

    public override string ToString()
    {
        string output = "";
        foreach (var f in fields.ToArray())
        {
            output += " " + f.ToString() + "\n";
        }
        foreach (var m in methods.ToArray())
        {
            output += m.ToString() + "\n";
        }
        return output;
    }
    public string ToString(string method_name, int index, string intermediate_line)
    {
        string output = ""; //"class " + name + " : " + parent_name + " {";
        //if (parent_name == "") output = "class " + name + " {";
        foreach (var f in fields.ToArray())
        {
            output += " " + f.ToString() + "\n";
        }
        foreach (var m in methods.ToArray())
        {
            output += " /*" + m.comment + "*/\n " + m.return_type + " " + m.name + " (" + string.Join(", ", m.parameters) + ")\n {\n";
            int indent_count = 1;
            for (int i = 0; i < m.lines.Count; i++) 
            {
                if (m.lines[i].Contains(Operators.CLOSING_BRACKET))
                {
                    indent_count--;
                }
                if (m.name == method_name && i == index) 
                {
                    output += new string(' ', 1 + indent_count * 1) + Formatter.Red(intermediate_line.Replace(" ", "_")) + "\n";
                }
                else 
                {
                    output += new string(' ', 1 + indent_count * 1) + m.lines[i].Trim() + "\n";
                }
                if (m.lines[i].Contains(Operators.OPENING_BRACKET)) 
                {
                    indent_count++;
                }
            }
            output += " }\n";
        }
        return output;
    }
}

public class Field 
{
    public string type, name, value;
    public Field (string name, string type)
    {
        this.name = name;
        this.type = type;
        this.value = "";
    }
    public Field (string name, string type, string value)
    {
        this.name = name;
        this.type = type;
        this.value = value;
    }
    public override string ToString ()
    {
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
    public Method(ClassObj class_obj, string name, string comment, string return_type, List<Field> parameters, List<string> lines)
    {
        this.class_obj = class_obj;
        this.comment = comment;
        this.name = name;
        this.return_type = return_type;
        this.parameters = parameters;
        this.lines = lines;
    }
    public int EndOfScope (int index)
    {
        int indent_count = 0;
        for (int i = index + 1; i < lines.Count; i++)
        {
            if (lines[i].Contains(Operators.OPENING_BRACKET)) 
            {
                indent_count++;
            }
            else if (lines[i].Contains(Operators.CLOSING_BRACKET))
            {
                indent_count--;
                if (indent_count == 0) 
                {
                    return i + 1;
                }
            }
        }
        return index;
    }
    public string IndentedLines()
    {
        string output = "";
        int indent_count = 2;
        for (int i = 0; i < lines.Count; i++)
        {
            if (lines[i].Contains(Operators.CLOSING_BRACKET))
            {
                indent_count--;
            }
            output += new string(' ', indent_count * 1) + lines[i].Trim() + "\n";
            if (lines[i].Contains(Operators.OPENING_BRACKET)) 
            {
                indent_count++;
            }
        }
        return output;
    }
    public override string ToString ()
    {
        return " /*" + comment + "*/\n " + return_type + " " + name + " (" + string.Join(", ", parameters) + ")\n {\n" + IndentedLines() + " }";
    }
}
