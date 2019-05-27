using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

public static class SubstringHandler {

    enum SCAN_OPTION { FIRST_OCCURANCE, LAST_OCCURANCE, NEXT_OCCURANCE_IN_SCOPE };

    /*
    String Class Overloads:
    ~~~~~~~~~~~~~~~~~~~~~~~
    Substring(int): Retrieves a substring starting at a specified character position and continuing to the end of the string.
    Substring(int, int): Retrieves a substring starting at a specified character position with a specified length.
 
    SubstringHandler Class Overloads:
    ~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
    Substring(string, string): Retrives a substring starting and ending at a given string
    */
    public static string Substring (string line, string start, Enum start_scan_option, string end, Enum end_scan_option) {

        switch (start_scan_option) {
            case SCAN_OPTION.FIRST_OCCURANCE:
                line = line.Substring (line.IndexOf (start));
                break;
            case SCAN_OPTION.NEXT_OCCURANCE_IN_SCOPE:

                break;
            case start_scan_option.LAST_OCCURANCE:
                line = line.Substring (line.LastIndexOf (start));
                break;
        }
        switch (end_scan_option) {
            case SCAN_OPTION.FIRST_OCCURANCE:
                line = line.Substring (0, line.IndexOf (end));
                break;
            case SCAN_OPTION.NEXT_OCCURANCE_IN_SCOPE:
                line = line.Substring (0, IndexInScopeOf (line, end));
                break;
            case start_scan_option.LAST_OCCURANCE:
                line = line.Substring (0, line.LastIndexOf (end));
                break;
        }

        return line; //"|Substring Handler Error|";
    }

    public static string[] Split (string line, string delimiter) {
        return Split (line, new string[] { delimiter });
    }
    public static string[] Split (string line, string[] delimiters) {

        List<string> line_parameters = new List<string> ();
        int index = 0;

        for (int i = 0; i < line.Length; i++) {

            int delimiter_index = -1;

            for (int j = 0; j < delimiters.Length; j++) {
                //Note: delimiters are assumed to be one character only (see 537ae28270ead0b9bef6e80730f26db90ff1df21 for extended verison)
                if (line[i] == delimiters[j][0]) {
                    delimiter_index = j;
                }
            }

            if (delimiter_index == -1) {

                line_parameters[index] += line[i];
            } else {

                line_parameters.Add ("");
                index++;
            }
        }
    }

    return line_parameters.ToArray ();
}

public static int IndexInScopeOf (string line, string end) {

    int count = 0;
    int parenthesis_count = 1;

    while (count < line.Length && parenthesis_count > 0) {
        count++;
        if (line_in[start_index + count].ToString () == Operators.OPENING_PARENTHESIS) {
            parenthesis_count++;
        } else if (line_in[start_index + count].ToString () == Operators.CLOSING_PARENTHESIS) {
            parenthesis_count--;
        }
    }
    return count;
}

}