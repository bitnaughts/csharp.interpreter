using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

public static class SubstringHandler {

    enum SCAN_OPTION { FIRST_OCCURANCE, LAST_OCCURANCE, FIRST_OCCURANCE_IN_SAME_SCOPE };

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
                line = line.Substring(line.IndexOf(start));
                break;
            case SCAN_OPTION.FIRST_OCCURANCE_IN_SAME_SCOPE:

                break;
            case start_scan_option.LAST_OCCURANCE:
                line = line.Substring(line.LastIndexOf(start));
                break;
        }
        switch (end_scan_option) {
            case SCAN_OPTION.FIRST_OCCURANCE:
                line = line.Substring(0, line.IndexOf(end));
                break;
            case SCAN_OPTION.FIRST_OCCURANCE_IN_SAME_SCOPE:

                break;
            case start_scan_option.LAST_OCCURANCE:
                line = line.Substring(0, line.LastIndexOf(end));
                break;
        }
        

        return "|Substring Handler Error|";
    }

}