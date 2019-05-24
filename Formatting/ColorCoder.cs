using System;
using System.Collections.Generic;
using System.Drawing;

public class ColorCoder {
    static string hex_values = "0123456789ABCDEF";
    //Color schemes:::
    public static Color Color_keyword = Color.FromArgb (88, 156, 214);
    public static Color Color_variable = Color.FromArgb (156, 220, 254);
    public static Color Color_object = Color.FromArgb (80, 200, 175);
    public static Color Color_function = Color.FromArgb (220, 220, 170);
    public static Color Color_number = Color.FromArgb (181, 206, 168);
    public static Color Color_comment = Color.FromArgb (88, 166, 77);
    public static Color Color_string = Color.FromArgb (210, 150, 100);
    public static Color Color_highlight = Color.FromArgb (1, 0, 0);

    public static Color Color_shadedScreen = Color.FromArgb (16, 46, 51);
    public static Color Color_screen = Color.FromArgb (25, 61, 65);
    public static Color Color_litScreen = Color.FromArgb (34, 76, 79);

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
        float red = color_in.R * 255f;
        float green = color_in.G * 255f;
        float blue = color_in.B * 255f;

        string a = GetHex (Math.Floor (red / 16));
        string b = GetHex (Math.Round (red % 16));
        string c = GetHex (Math.Floor (green / 16));
        string d = GetHex (Math.Round (green % 16));
        string e = GetHex (Math.Floor (blue / 16));
        string f = GetHex (Math.Round (blue % 16));

        string z = a + b + c + d + e + f;

        return z;
    }
    public static string GetHex (double value) {
        return hex_values[(int) value] + "";
    }

}