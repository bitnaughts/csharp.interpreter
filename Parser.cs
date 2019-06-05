using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Globalization;
using System.Linq;
using System.Reflection;

public static class Parser {

    /* Parser simplifies complex statements into simple ones, managing PEMDAS, function calls, etc. */
    public static string step (string line_in, VariableHandler variable_handler, out bool simplified) {

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
                .Insert (start_of_parenthesis, simplify (inner_snippet, variable_handler, out simplified));
        }
        return beginning_section + simplify (line_in, variable_handler, out simplified);
    }

    private static string simplify (string input, VariableHandler variable_handler, out bool simplified) {

        /* Example input for this function: "4 + 4 * 4" */
        /* Note: Since step() manages any parenthesis-related PEDMAS, so that isn't handled in this function */

        /* Splits along spaces, e.g. ["4", "+", "4", "*", "4"] */
        List<string> parts = input.Split (Operators.SPLIT, StringSplitOptions.RemoveEmptyEntries).ToList ();

        /* If line is just "x", replace "x" with x's value */
        if (parts.Count () == 1) {
            simplified = true;
            return variable_handler.getValue (parts[0]);
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
                    string left = variable_handler.getValue (parts[part - 1]),
                        right = variable_handler.getValue (parts[part + 1]);

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
        return getErrorMessage ();
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

    public static string getErrorMessage () {
        return "Parsing Error in this line.";
    }
}