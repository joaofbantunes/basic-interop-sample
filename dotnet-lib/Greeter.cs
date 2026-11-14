using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using System.Text;

namespace GreeterLib;

public static class Greeter
{
    private static readonly UTF8Encoding StrictUtf8 = new(false, true);

    // traditional C# greeting method
    private static string GreetManaged(string name) => $"Hello, {name}! .NET Native AOT here.";

    // C-compatible wrapper for the greeting method
    [UnmanagedCallersOnly(EntryPoint = "greet", CallConvs = [typeof(CallConvCdecl)])]
    public static unsafe IntPtr Greet(IntPtr name)
    {
        if (name == IntPtr.Zero)
            return IntPtr.Zero;

        try
        {
            ReadOnlySpan<byte> bytes = MemoryMarshal.CreateReadOnlySpanFromNullTerminated((byte*)name);
            string input = StrictUtf8.GetString(bytes);
            return Marshal.StringToCoTaskMemUTF8(GreetManaged(input));
        }
        catch (Exception)
        {
            return IntPtr.Zero;
        }
    }

    // C-compatible wrapper for freeing the greeting memory
    [UnmanagedCallersOnly(EntryPoint = "free_greeting", CallConvs = [typeof(CallConvCdecl)])]
    public static void FreeGreeting(IntPtr greeting) => Marshal.FreeCoTaskMem(greeting);
}
