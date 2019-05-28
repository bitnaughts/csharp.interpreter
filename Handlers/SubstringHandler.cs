using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

public static class SubstringHandler {

    public static string[] Split (string line, string delimiter) {
        return Split (line, new string[] { delimiter });
    }
    public static string[] Split (string line, string[] delimiters) {

        List<string> line_parameters = new List<string> ();
        line_parameters.Add ("");
        int index = 0, parenthesis_level = 0;

        for (int i = 0; i < line.Length; i++) {

            int delimiter_index = -1;
            for (int j = 0; j < delimiters.Length; j++) {
                //Note: delimiters are assumed to be one character only (see 537ae28270ead0b9bef6e80730f26db90ff1df21 for extended verison)
                if (line[i] == delimiters[j][0]) {
                    delimiter_index = j;
                    if (delimiters[j] == Operators.OPENING_PARENTHESIS) {
                        parenthesis_level++;
                    } else if (delimiters[j] == Operators.CLOSING_PARENTHESIS) {
                        parenthesis_level--;
                    }
                }
            }

            //if not in correct parenthesis scope, check for change in parenthesis scope and continue
            if (delimiter_index == -1) {
                line_parameters[index] += line[i];
            } else {

                bool is_valid_delimiter = (parenthesis_level <= 1) && !(parenthesis_level == 1 && line[i].ToString() == Operators.CLOSING_PARENTHESIS);
               
                if (is_valid_delimiter) {
                    line_parameters.Add (""); 
                    index++;
                }
                else 
                {
                    line_parameters[index] += line[i];
                }
            }
        }
        for (int i = 0; i <= index; i++) {
            line_parameters[i] = line_parameters[i].Trim ();
        }
        return line_parameters.ToArray ();
    }
}