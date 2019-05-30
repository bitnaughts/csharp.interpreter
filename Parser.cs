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
        
        
        //This might be more appropriate in Interpreter.cs, as parser should be specialized only for shortening expressions/functions...
        /* If there is a variable declaration in the current line, e.g. "i = 10 + 20" or "int i = 10 + 20" */
        if (line_in.Contains(Operators.SPACE + Operators.EQUALS + Operators.SPACE)) { //to avoid being confused with ==, etc.

            string[] line_parts = line_in.Split(Operators.SPACE + Operators.EQUALS + Operators.SPACE);
            string variable_information = line_parts[0]; // "i"
            string value_information = line_parts[1]; // "10 + 20" ==> "30"...
            
            //line_in = line_in.Substring(line_in.IndexOf(Operators.SPACE + Operators.EQUALS + Operators.SPACE) + 3);
            //Might need some sort of promise to set variable to fully simplified value...
            //Also for rebuilding full line back to "i = 30" at the end...
        }

        /* Handling parenthesis, whether for PEDMAS manipulation or function calls */
        if (line_in.Contains (Operators.OPENING_PARENTHESIS)) {

            int start_of_parenthesis = 0;
            string inner_snippet = line_in;


            while (inner_snippet.Contains (Operators.OPENING_PARENTHESIS)) {

                start_of_parenthesis += inner_snippet.IndexOf (Operators.OPENING_PARENTHESIS);
                Logger.Log(inner_snippet);
                inner_snippet = inner_snippet.Substring(inner_snippet.IndexOf(Operators.OPENING_PARENTHESIS) + 1);
                inner_snippet = inner_snippet.Substring(0, getLengthToClosingParenthesis(inner_snippet, 0));
            }
            
            return line_in.Remove (start_of_parenthesis, inner_snippet.Length + 2)
                .Insert (start_of_parenthesis, simplify (inner_snippet, variable_handler, out simplified));
        }
        return simplify (line_in, variable_handler, out simplified);
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
                for (int operation = 0; operation < Operators.PEMDAS[operation_set].Length; operation++) {

                    if (parts[part] == Operators.PEMDAS[operation_set][operation]) {

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
