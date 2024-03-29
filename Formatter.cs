using System;
using System.Collections.Generic;
using UnityEngine;
public class Formatter {
    static string hex_values = "0123456789ABCDEF";
    //Color schemes:::
    public static Color Color_keyword = new Color (88 / 255f, 156 / 255f, 214 / 255f);
    public static Color Color_variable = new Color (156 / 255f, 220 / 255f, 254 / 255f);
    public static Color Color_object = new Color (80 / 255f, 200 / 255f, 175 / 255f);
    public static Color Color_red = new Color (1f, 0, 0);
    public static Color Color_green = new Color (0, 255f / 255f,  0);
    public static Color Color_yellow = new Color (1f, 1f,  0);
    public static Color Color_function = new Color (220 / 255f, 220 / 255f, 170 / 255f);
    public static Color Color_number = new Color (181 / 255f, 206 / 255f, 168 / 255f);
    public static Color Color_comment = new Color (88 / 255f, 166 / 255f, 77 / 255f);
    public static Color Color_string = new Color (210 / 255f, 150 / 255f, 100 / 255f);
    public static Color Color_highlight = new Color (1, 0, 0);

    public static Color Color_shadedScreen = new Color (16 / 255f, 46 / 255f, 51 / 255f);
    public static Color Color_screen = new Color (25 / 255f, 61 / 255f, 65 / 255f);
    public static Color Color_litScreen = new Color (34 / 255f, 76 / 255f, 79 / 255f);


    public static string stringify (string line) {
        return "<color=#" + getStringColor () + ">" + line + "</color>";
    }
    public static string highlight (string line) {
        return "<color=#" + getHighlightColor () + ">" + line + "</color>";
    }
    public static string comment (string line) {
        return"<color=#" + getCommentColor () + ">" + line + "</color>";
    }
    public static string keyword (string line) {
        return"<color=#" + getKeywordColor () + ">" + line + "</color>";
    }
    public static string ColorObject (string line) {
        return"<color=#" + getObjectColor () + ">" + line + "</color>";
    }
    public static string Red (string line) {
        return"<color=#" + getRedColor () + ">" + line + "</color>";
    }
    public static string Green (string line) {
        return"<color=#" + getGreenColor () + ">" + line + "</color>";
    }
    public static string Yellow (string line) {
        return"<color=#" + getYellowColor () + ">" + line + "</color>";
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
    public static string getObjectColor () {
        return RGBToHex (Color_object);
    }
    public static string getRedColor () {
        return RGBToHex (Color_red);
    }
    public static string getGreenColor () {
        return RGBToHex (Color_green);
    }
    public static string getYellowColor () {
        return RGBToHex (Color_yellow);
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