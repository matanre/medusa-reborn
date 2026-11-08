using UnityEngine;

public static class Assert
{
    public static void NotNull(Component obj, string name)
    {
        string caller = obj.gameObject.name;
        NotNull(obj, caller, name);
    }

    public static void NotNull(object obj, string caller, string name)
    {
        UnityEngine.Assertions.Assert.IsNotNull(obj, $"{caller}: {name} cannot be null!");
    }
}
