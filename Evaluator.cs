using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Globalization;
using System.Linq;
using System.Reflection;
using UnityEngine;

public class Evaluator {

    private static int int_result;
    private static float float_result;
    private static bool bool_result;

    public static string simplify (string left, string arithmetic_operator, string right) {
        /* e.g. ["12", "*", "4"] ==> ["48"] */
        string left_type = getType (left), right_type = getType (right);
        if (left_type == right_type) {
            switch (left_type) {
                case Variables.BOOLEAN:
                    return simplifyBooleans (bool.Parse (left), arithmetic_operator, bool.Parse (right));
                case Variables.INTEGER:
                    return simplifyIntegers (int.Parse (left), arithmetic_operator, int.Parse (right));
                case Variables.FLOAT:
                    return simplifyFloats (float.Parse (left), arithmetic_operator, float.Parse (right));
                case Variables.STRING:
                    return simplifyString (left, arithmetic_operator, right);
            }
        }
        if ((left_type == Variables.FLOAT && right_type == Variables.INTEGER) || (right_type == Variables.FLOAT && left_type == Variables.INTEGER)) {
            /* AUTO TYPE CASTING INTEGERS IN FLOAT CALCULATIONS, e.g. "12 / 1.0" == "12.0", not "12" */
            return simplifyFloats (float.Parse (left), arithmetic_operator, float.Parse (right));
        }
        /* CASTS NOT HANDLED, e.g. "true + 1.0", TREAT AS STRINGS */
        return simplifyString (left, arithmetic_operator, right);
    }
    public static string simplifyBooleans (bool left, string arithmetic_operator, bool right) {
        switch (arithmetic_operator) {
            case Operators.EQUAL_TO:
                return (left == right).ToString ().ToLower();
            case Operators.AND:
                return (left && right).ToString ().ToLower();
            case Operators.OR:
                return (left || right).ToString ().ToLower();
            default:
                return "";
        }
    }
    public static string simplifyIntegers (int left, string arithmetic_operator, int right) {
        switch (arithmetic_operator) {
            case Operators.MODULUS:
                return (left % right).ToString ();
            case Operators.TIMES:
                return (left * right).ToString ();
            case Operators.DIVIDE:
                return (left / right).ToString ();
            case Operators.ADD:
                return (left + right).ToString ();
            case Operators.SUBTRACT:
                return (left - right).ToString ();
            case Operators.EQUAL_TO:
                return (left == right).ToString ().ToLower();
            case Operators.GREATER_THAN:
                return (left > right).ToString ().ToLower();
            case Operators.GREATER_THAN_EQUAL:
                return (left >= right).ToString ().ToLower();
            case Operators.LESS_THAN:
                return (left < right).ToString ().ToLower();
            case Operators.LESS_THAN_EQUAL:
                return (left <= right).ToString ().ToLower();
            default:
                return "";
        }
    }
    public static string simplifyFloats (float left, string arithmetic_operator, float right) {
        switch (arithmetic_operator) {
            case Operators.MODULUS:
                return (left % right).ToString ();
            case Operators.TIMES:
                return (left * right).ToString ();
            case Operators.DIVIDE:
                return (left / right).ToString ();
            case Operators.ADD:
                return (left + right).ToString ();
            case Operators.SUBTRACT:
                return (left - right).ToString ();
            case Operators.EQUAL_TO:
                return (left == right).ToString ().ToLower();
            case Operators.GREATER_THAN:
                return (left > right).ToString ().ToLower();
            case Operators.GREATER_THAN_EQUAL:
                return (left >= right).ToString ().ToLower();
            case Operators.LESS_THAN:
                return (left < right).ToString ().ToLower();
            case Operators.LESS_THAN_EQUAL:
                return (left <= right).ToString ().ToLower();
            default:
                return "";
        }
    }

    public static string simplifyString (string left, string arithmetic_operator, string right) {
        if (left.IndexOf ("\"") == 0) left = left.Substring (1, left.Length - 2);
        if (right.IndexOf ("\"") == 0) right = right.Substring (1, right.Length - 2);
        switch (arithmetic_operator) {
            case Operators.ADD:
                return left + right;
            case Operators.EQUAL_TO:
                return (left == right).ToString ();
            default:
                return "";
        }
    }
    public static string getType (string input) {
        if (bool.TryParse (input, out bool_result)) return Variables.BOOLEAN;
        if (int.TryParse (input, out int_result)) return Variables.INTEGER;
        if (float.TryParse (input, out float_result)) return Variables.FLOAT;
        return Variables.STRING;
    }
    public static string simplifyCondensedOperators (string variable_name, string arithmetic_operator) {
        switch (arithmetic_operator) {
            case Operators.EQUALS:
                return Operators.EMPTY;
            case Operators.ADDITION:
                return variable_name + " " + Operators.ADD + " " + Operators.OPENING_PARENTHESIS;
            case Operators.SUBTRACTION:
                return variable_name + " " + Operators.SUBTRACT + " " + Operators.OPENING_PARENTHESIS;
            case Operators.MULTIPLICATION:
                return variable_name + " " + Operators.TIMES + " " + Operators.OPENING_PARENTHESIS;
            case Operators.DIVISION:
                return variable_name + " " + Operators.DIVIDE + " " + Operators.OPENING_PARENTHESIS;
            case Operators.MODULO:
                return variable_name + " " + Operators.MODULUS + " " + Operators.OPENING_PARENTHESIS;
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
            case Operators.INCREMENT:
                return new string[] { variable_name, Operators.EQUALS, variable_name, Operators.ADD, "1" };
            case Operators.DECREMENT:
                return new string[] { variable_name, Operators.EQUALS, variable_name, Operators.SUBTRACT, "1" };

        }
        return new string[] { };
    }
    public static string cast (string input, string cast_type) {
        if (input != Operators.EMPTY) { //is this necessary? probably...&& Evaluator.getType (getValue (input)) == cast_type) {
            switch (cast_type) {
                case Variables.BOOLEAN:
                    return bool.Parse (input).ToString ();
                case Variables.INTEGER:
                    return int.Parse (input).ToString ();
                case Variables.FLOAT:
                    return float.Parse (input).ToString ();
                case Variables.STRING:
                    return input;
            }
        }
        //add cases to convert floats to ints, etc? 
        //but, preferrably implement casting (int.Parse...)
        return Operators.EMPTY;
    }
}