# BitNaughts' C# Intepreter

A glorified string parser, the Interpreter reads heavily-linted C# syntax and triggers events in BitNaughts based on what it reads. It current supports:

  - Global and Local Variables (primitive data types only)
  - Variable Manipulation (PEMDAS, modulo)
  - Garbage Collection
  - Conditional Flow Control (if, for, while)
  - Function Calls (recursive functions to be tested)

Future additions include:
  - More Data Types (objects, e.g. Vector2)
  - Code Editor/Writer
  - ... and much more

BitNaughts is a true open-source project. If you'd like to contribute to the interpreter, feel free to make a pull request! 

## Examples

```
using Console;
class ExampleClass {
    int tester = 10;
    static void Main() {
            int angle = 1;
            for (int x = 0; x < 10; x++) {
                for (int y = 0; y < 10; y++) {
                    for (int z = 0; z < 10; z++) {
                        angle = angle + 1;
                        Print();
                }
            }
        }
    }
    void Print() {
        Console.WriteLine("hello");
    }
}
```