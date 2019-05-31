using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Globalization;
using System.Linq;
using System.Reflection;

public class Evaluator {

    private static int int_result;
    private static float float_result;
    private static bool bool_result;

    public static string simplify (string left, string arithmetic_operator, string right) {
        /* e.g. ["12", "*", "4"] ==> ["48"] */
        string left_type = getType (left), right_type = getType (right);
        if (left_type == right_type) {
            switch (left_type) {
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
        if ((left_type == Keywords.Type.Value.FLOAT && right_type == Keywords.Type.Value.INTEGER) || (right_type == Keywords.Type.Value.FLOAT && left_type == Keywords.Type.Value.INTEGER)) {
            /* AUTO TYPE CASTING INTEGERS IN FLOAT CALCULATIONS, e.g. "12 / 1.0" == "12.0", not "12" */
            return simplifyFloats (float.Parse (left), arithmetic_operator, float.Parse (right));
        }
        /* CASTS NOT HANDLED, e.g. "true + 1.0", TREAT AS STRINGS */
        return simplifyString (left, arithmetic_operator, right);
    }
    public static string simplifyBooleans (bool left, string arithmetic_operator, bool right) {
        switch (arithmetic_operator) {
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
    public static string simplifyIntegers (int left, string arithmetic_operator, int right) {
        switch (arithmetic_operator) {
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
    public static string simplifyFloats (float left, string arithmetic_operator, float right) {
        switch (arithmetic_operator) {
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

    public static string simplifyString (string left, string arithmetic_operator, string right) {
        if (left.IndexOf ("\"") == 0) left = left.Substring (1, left.Length - 2);
        if (right.IndexOf ("\"") == 0) right = right.Substring (1, right.Length - 2);
        switch (arithmetic_operator) {
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
    public static string getType (string input) {
        if (bool.TryParse (input, out bool_result)) return Keywords.Type.Value.BOOLEAN;
        if (int.TryParse (input, out int_result)) return Keywords.Type.Value.INTEGER;
        if (float.TryParse (input, out float_result)) return Keywords.Type.Value.FLOAT;
        return Keywords.Type.Reference.STRING;
    }
    public static string simplifyCondensedOperators (string variable_name, string arithmetic_operator) {
        switch (arithmetic_operator) {
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
    public static string scrubSymbols (string input) {
        string output = input;
        if (input.Contains (Operators.END_LINE)) output = input.Remove (input.IndexOf (Operators.END_LINE), 1);
        return output;
    }

    public static string[] splitIncrement (string input) {
        string variable_name, variable_operation;
        input = input.Split (Operators.END_LINE_CHAR) [0]; //remove ; if there

        variable_name = input.Substring (0, input.Length - 2);
        variable_operation = input.Substring (input.Length - 2);

        switch (variable_operation) {
            case Operators.Arithmetic.Unary.INCREMENT:
                return new string[] { variable_name, Operators.EQUALS, variable_name, Operators.Arithmetic.ADDITION, "1" };
            case Operators.Arithmetic.Unary.DECREMEMT:
                return new string[] { variable_name, Operators.EQUALS, variable_name, Operators.Arithmetic.SUBTRACTION, "1" };

        }
        return new string[] { };
    }
    public static string cast (string input, string cast_type) {
        if (input != Operators.EMPTY) { //is this necessary? probably...&& Evaluator.getType (getValue (input)) == cast_type) {
            switch (cast_type) {
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
}