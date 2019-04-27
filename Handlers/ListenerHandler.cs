using System.Collections.Generic;
using UnityEngine;

public class ListenerHandler {

    private List<string> listeners;
    private GameObject obj;

    private bool has_been_added = false;

    public ListenerHandler (List<string> listeners) {
        this.listeners = listeners;
        // this.obj = obj;

    }
    public void addListeners (GameObject obj) {
        for (int listener = 0; listener < listeners.Count; listener++) {
            addListener (listeners[listener], obj);
        }
    }

    public void addListener (string class_name, GameObject obj) {
        switch (class_name) {
            case Classes.CONSOLE:
                // Referencer.consoleManager.execute (Console.NAME, Console.OPEN, obj);
                break;
            case Classes.PLOTTER:

                break;
        }
    }
    public bool isFunction (string function_name) {
        string class_name = function_name.Split ('.') [0];
        /* Isn't a function in scope */
        for (int i = 0; i < listeners.Count; i++) {
            if (class_name == listeners[i]) { //.name) {
                return true;
            }
        }

        /* Is it a static function? */
        return false;
    }
    public void callListener (string line, VariableObject variable_reference, GameObject obj) {
        string[] line_parts = line.Split (' ');
        string class_name = line_parts[0].Split ('.') [0];
        string function_name = line_parts[0].Split ('.') [1].Split ('(') [0];
        string function_parameters = line.Substring (line.IndexOf ("(") + 1);
        function_parameters = function_parameters.Substring (0, function_parameters.Length - 2);

        string[] function_parameters_arr = function_parameters.Split (',');
        function_parameters_arr[1] = function_parameters_arr[1].Split('\"')[1]; // remove "quotes" from string

        //TODO:
        //Will want to pass variable type (or entire variable obj) to this function to know what referencer connection to make

        //switch (class_name) {
          //  case Classes.CONSOLE:
          if (class_name == "console1")
                Referencer.consoleManager.execute (variable_reference, function_name, function_parameters_arr, obj);
            //    break;
            //case Classes.PLOTTER:
                // Referencer.plotterManager.execute (command, "", obj);
              //  break;
        //}
    }
    public void updateListeners (ScopeHandler scope, GameObject obj) {
        if (has_been_added) {
            Referencer.consoleManager.execute (Console.NAME, Console.UPDATE, scope.getVariablesInScope (Console.NAME).ToArray(), obj);
            // for (int listener = 0; listener < listeners.Count; listener++) {
            //     switch (listeners[listener]) {
            //         case Classes.CONSOLE:
            //             Referencer.consoleManager.execute (Console.NAME, Console.UPDATE, scope, obj);
            //             break;
            //         case Classes.PLOTTER:
            //             break;
            //     }
            // }
        } else {
            addListeners (obj);
            has_been_added = true;
        }
    }
}