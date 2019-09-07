![banner](https://raw.githubusercontent.com/bitnaughts/bitnaughts.assets/master/images/banner.png)

# BitNaughts' C# Intepreter: Interfaces

Interfaces are automatically inherited for a class if a module is placed on a specific ship part. Interfaces provide standardized functionalities for the module to augment and customize.  

## Examples

```cs 
interface Gimbal {
    void Rotate (int degrees); 
}
interface Gun {
    readonly int shots_left;
    void Fire ();
}

using TargettingComputer;
class GimbalGun : Gimbal, Gun {
    void Rotate(int degrees) {
        ...
    }


}

```