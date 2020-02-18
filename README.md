# SerializedFunc

SerializedFunc allows you to define C# `func<TReturnValue>` and assign them in the Unity Editor.

This package is available as package manager package. Please see [here](https://docs.unity3d.com/Manual/upm-git.html) for installation instructions.

![Showcase of editor integration](https://raw.githubusercontent.com/JurjenBiewenga/SerializedFunc/master/Images/Assignment.gif)

# Usage

To allow serialization of your SerializedFunc you have to subclass from the generic class.

To do so you have create a new class similar to:
```cs
[Serializable]
private class ClassName : SerializedFunc<ParameterType, ReturnType>
{

}
```
If you'd like you can add more parameters (up to 7) by just adding more generic arguments.
Note: The last argument will always be the return type

This is what a class would look like for a SerializedFunc that accepts 4 integers and returns a string:
```cs
[Serializable]
private class CombineIntegers : SerializedFunc<int, int, int, int, string>
{

}
```

Once you've created subclass you can simply create a serialized field as follows:
```cs
private class Player : MonoBehaviour
{
    public CombineIntegers scoreCombiners;
}
```
Now it will appear in the inspector!
