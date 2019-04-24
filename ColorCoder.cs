using System;
using System.Collections.Generic;
using UnityEngine;
public class ColorCoder {
    static string hex_values = "0123456789ABCDEF";
    //Color schemes:::
    public static Color Color_keyword = new Color (88 / 255f, 156 / 255f, 214 / 255f);
    public static Color Color_variable = new Color (156 / 255f, 220 / 255f, 254 / 255f);
    public static Color Color_object = new Color (80 / 255f, 200 / 255f, 175 / 255f);
    public static Color Color_function = new Color (220 / 255f, 220 / 255f, 170 / 255f);
    public static Color Color_number = new Color (181 / 255f, 206 / 255f, 168 / 255f);
    public static Color Color_comment = new Color (88 / 255f, 166 / 255f, 77 / 255f);
    public static Color Color_string = new Color (210 / 255f, 150 / 255f, 100 / 255f);
    public static Color Color_highlight = new Color (1, 0, 0);

    public static Color Color_shadedScreen = new Color (16 / 255f, 46 / 255f, 51 / 255f);
    public static Color Color_screen = new Color (25 / 255f, 61 / 255f, 65 / 255f);
    public static Color Color_litScreen = new Color (34 / 255f, 76 / 255f, 79 / 255f);

    public static string[] keyword_list = { "class", "using", "boolean", "char", "class", "const", "double", "else", "final", "float", "for", "if", "int", "new", "private", "public", "return", "static", "this", "void", "while" };
    public static string[] object_list = { "Console", "Plotter", "Grapher", "String", "System" };
    public static string[] function_list = { "WriteLine" };
    public static string[] variable_list = { "angle", "x", "y" };

    public static string colorize (string line) {
        List<int> indexes;
        //STRINGS
        line = colorizeStrings (line);
        //KEYWORDS
        // for (int i = 0; i < 10; i++) {
        //     if (line.Contains(i+"")) {
        //         line = line.Replace (i+"", "<color=#" + RGBToHex (Color_number) + ">" + i + "</color>");
        //     }
        // }
        for (int i = 0; i < keyword_list.Length; i++) {
            if (line.Contains (keyword_list[i])) {
                line = line.Replace (keyword_list[i], "<color=#" + RGBToHex (Color_keyword) + ">" + keyword_list[i] + "</color>");
            }
        }
        for (int i = 0; i < object_list.Length; i++) {
            if (line.Contains (object_list[i])) {
                line = line.Replace (object_list[i], "<color=#" + RGBToHex (Color_object) + ">" + object_list[i] + "</color>");
            }
        }
        for (int i = 0; i < function_list.Length; i++) {
            if (line.Contains (function_list[i])) {
                line = line.Replace (function_list[i], "<color=#" + RGBToHex (Color_function) + ">" + function_list[i] + "</color>");
            }
        }
        for (int i = 0; i < variable_list.Length; i++) {
            if (line.Contains (variable_list[i])) {
                line = line.Replace (variable_list[i], "<color=#" + RGBToHex (Color_variable) + ">" + variable_list[i] + "</color>");
            }
        }

        return line;
    }

    public static string colorizeStrings (string line) {
        if (!line.Contains ("\"")) {
            return line;
        } else {
            string output = "";
            bool open_string = false;
            for (int i = 0; i < line.Length; i++) {
                if (line[i] == '\"') {
                    if (!open_string) {
                        output += "<color=#" + getStringColor () + ">";
                    } else {
                        output += "\"" + "</color>";
                        i++;
                    }
                    open_string = !open_string;
                }
                output += line[i];
            }
            return output;
        }
    }

    public static string colorizeWords (string line, string word, List<int> indexes, Color color_in) {
        if (indexes.Count == 0) {
            return line;
        } else {
            string output = "";
            int shifter = 0;
            for (int i = 0; i < indexes.Count; i++) {
                output += line.Substring (shifter, indexes[i]) + "<color=#" + RGBToHex (color_in) + ">" + word + "</color>" + line.Substring (indexes[i] + word.Length);
                shifter = indexes[i];
            }
            return output;
        }
    }

    public static string highlight (string line) {
        return "<color=#" + ColorCoder.getHighlightColor () + "55>" + line + "</mark>";
    }
    public static string comment (string line) {
        return line.Substring (0, line.IndexOf ("//")) + "<color=#" + ColorCoder.getCommentColor () + ">" + line.Substring (line.IndexOf ("//")) + "</color>";
    }
    public static string getShadedScreenColor () {
        return RGBToHex (Color_shadedScreen);
    }
    public static string getScreenColor () {
        return RGBToHex (Color_screen);
    }
    public static string getLitScreenColor () {
        return RGBToHex (Color_litScreen);
    }
    public static string getKeywordColor () {
        return RGBToHex (Color_keyword);
    }
    public static string getCommentColor () {
        return RGBToHex (Color_comment);
    }
    public static string getHighlightColor () {
        return RGBToHex (Color_highlight);
    }
    public static string getColorObject () {
        return RGBToHex (Color_object);
    }
    public static string getStringColor () {
        return RGBToHex (Color_string);
    }
    /*  Code converted from Danny Lawrence's JavaScript implementation
        http://wiki.unity3d.com/index.php?title=HexConverter        */
    public static string RGBToHex (Color color_in) {
        float red = color_in.r * 255f;
        float green = color_in.g * 255f;
        float blue = color_in.b * 255f;

        string a = GetHex (Mathf.Floor (red / 16));
        string b = GetHex (Mathf.Round (red % 16));
        string c = GetHex (Mathf.Floor (green / 16));
        string d = GetHex (Mathf.Round (green % 16));
        string e = GetHex (Mathf.Floor (blue / 16));
        string f = GetHex (Mathf.Round (blue % 16));

        string z = a + b + c + d + e + f;

        return z;
    }
    public static string GetHex (float value) {
        return hex_values[(int) value] + "";
    }

}