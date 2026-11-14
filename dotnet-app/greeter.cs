#:property TargetFramework=net11.0

using System;
using System.Runtime.InteropServices;

string name = args.Length > 0 ? args[0] : "world";
Console.WriteLine(Greeter.Greet(name));

static class Greeter
{
    [DllImport("greeter_lib", EntryPoint = "greet", CallingConvention = CallingConvention.Cdecl)]
    private static extern IntPtr GreetNative([MarshalAs(UnmanagedType.LPUTF8Str)] string name);

    [DllImport("greeter_lib", EntryPoint = "free_greeting", CallingConvention = CallingConvention.Cdecl)]
    private static extern void FreeGreeting(IntPtr greeting);

    public static string Greet(string name)
    {
        ArgumentNullException.ThrowIfNull(name);
        if (name.Contains('\0'))
            throw new ArgumentException("The name cannot contain NUL characters.", nameof(name));

        IntPtr greeting = GreetNative(name);
        if (greeting == IntPtr.Zero)
            throw new InvalidOperationException("The native greeting failed.");

        try
        {
            return Marshal.PtrToStringUTF8(greeting)
                ?? throw new InvalidOperationException("The native greeting was null.");
        }
        finally
        {
            FreeGreeting(greeting);
        }
    }
}