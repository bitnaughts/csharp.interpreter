using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Globalization;
using System.Linq;
using System.Reflection;
using UnityEngine;

public static class Parser {

    /* Parser simplifies complex statements into simple ones, managing PEMDAS, function calls, etc. */
    public static string step (string line_in, VariableObject[] variables) {
        string line_out = line_in;

        //If ()s exist
        //  Go to deepest ()s
        //      if parts[] of deepest ()s > 1
        //          simplify parts[] 1 level
        //          if parts[] length == 1
        //              if function(value)
        //                  manage function calls
        //              else 
        //                  replace (value) with value
        //else
        //  simplify parts[] 1 level

        return line_out;
    }

    private static string simplify (string input) {

        /* Example input for this function: "4 + 4 * 4" */
        /* Of note: since step() manages any parenthesis-related PEDMAS, so that isn't handled in this function */

        /* e.g. ["4", "+", "4", "*", "4"] */
        string[] parts = input.Split (' ');
        
        /* Stop simplifying after one step of PEDMAS */
        bool processed = false;

        /* PEDMAS, e.g. ["4", "+", 4, "*", "4"] ==> ["4", "+", "16"] ==> ["20"] */

        /* First Modulus, then Multiplication and Division together, then Addition and Subtraction together, etc. */
        for (int operation_set = 0; operation_set < Operators.PEMDAS.Length && !processed; operation_set++) {

            /* Look for operations (the odd indices of the parts array)  */
            for (int part = 1; part < parts.Count - 1 && !processed; part += 2) {

                /* Check if operation of input matches current order of PEMDAS */
                for (int operation = 0; operation < Operators.PEMDAS[operation_set].Length && !processed; operation++) {
                    
                    if (parts[part] == Operators.PEMDAS[operation_set][operation]) {

                        string left = getValue (parts[part - 1]), right = getValue (parts[part + 1]);

                        parts[part - 1] = Evaluator.simplify (left, operation_type, right);

                        parts.RemoveRange (part, 2);

                        break;
                    }
                }
            }
        }
        /* RETURN FULLY SIMPLIFIED VALUE, e.g. "28" */
        return parts[0];

    }
}

// public string parse (string input) {
//     if (input != Operators.EMPTY) {
//         List<string> parts = input.Split (' ').ToList<string> ();
//         /* EVALUATE PARENTHESIS AND FUNCTIONS RECURSIVELY, e.g. "12 + function(2) * 4" ==> "12 + 4 * 4" */

//         //this section is still untested and requires thorough testing
//         //for edge cases with parenthesis, e.g. "Mathf.Abs((2 + 1) * 2)"
//         //
//         //I think this logic isn't the best suited for parenthesis
//         //Fix:
//         //find deepest set of parenthesis, combine spaces to parameters[], parse each parameter in parameters (for ","s in functions)
//         //in the end, "2 * (2 + 2)" = "2 * 4"
//         //or with functions, "2 * Mathf.Abs(2 + 2)" = "2 * Mathf.Abs(4)"...
//         //If function preceeds ()s, keep parenthesis, else return parse(parameter)

//         for (int part = 0; part < parts.Count; part++) {
//             if (parts[part].Contains (Operators.OPENING_PARENTHESIS)) {
//                 string parts_to_be_condensed = parts[part];
//                 while (parts[part].Contains (Operators.CLOSING_PARENTHESIS) == false) {
//                     part++;
//                     parts_to_be_condensed += " " + parts[part];
//                 }
//                 if (parts[part].IndexOf (Operators.OPENING_PARENTHESIS) == 0) {
//                     parts[part] = parse (parts_to_be_condensed);
//                 } else {
//                     //to support user-made functions, or functions that require interpreted lines of code to be executed first, will require logic here to allow for putting this parse in a stack to be popped on the "return" of said function
//                     string function = parts[part].Substring (0, parts[part].IndexOf (Operators.OPENING_PARENTHESIS));
//                     /* e.g. Mathf.Abs(-10) == 10 */
//                     // parts[part] = Evaluator.simplifyFunction (function, parse (parts_to_be_condensed));
//                 }
//             }
//         }

//     }
//     return Operators.EMPTY;
// }