# Basic Interop Sample

Just a pretty basic sample showing how to use native libraries from a C# application.

- `dotnet-app` - file-based C# application that calls the native libraries
- `dotnet-lib` - Native AOT C# library exposing the `greet` and `free_greeting` C ABI.
- `rust-lib` - Rust library exposing the `greet` and `free_greeting` C ABI.

Both Rust and Native AOT libraries expose the same `greet` and `free_greeting` C ABI.
The input and result are NUL-terminated UTF-8 strings.
`greet` returns a pointer to the greeting string, or returns null on failure.
`free_greeting` should be used to release the memory allocated by `greet` (the string returned by `greet`).

## Prerequisites

Run the commands below from the repository root. Build for the same OS and architecture as the .NET application; the examples use the native host rather than cross-compiling. Select the appropriate runtime identifier (RID):

| OS | x64 | ARM64 |
| --- | --- | --- |
| Linux | `linux-x64` | `linux-arm64` |
| macOS | `osx-x64` | `osx-arm64` |
| Windows | `win-x64` | `win-arm64` |

## Linux and macOS

Set `RID` for your OS and architecture, then build both libraries:

```bash
RID=linux-x64 # For example, use osx-arm64 on Apple Silicon.
cargo build --manifest-path rust-lib/Cargo.toml --release
dotnet publish dotnet-lib/GreeterLib.csproj -c Release -r "$RID"
```

On Linux, run the caller against each implementation:

```bash
# using rust library
LD_LIBRARY_PATH="$PWD/rust-lib/target/release" dotnet run dotnet-app/greeter.cs -- world

# using dotnet library
LD_LIBRARY_PATH="$PWD/dotnet-lib/bin/Release/net11.0/$RID/publish" dotnet run dotnet-app/greeter.cs -- world
```

On macOS, use `DYLD_LIBRARY_PATH` instead:

```bash
# using rust library
DYLD_LIBRARY_PATH="$PWD/rust-lib/target/release" dotnet run dotnet-app/greeter.cs -- world

# using dotnet library
DYLD_LIBRARY_PATH="$PWD/dotnet-lib/bin/Release/net11.0/$RID/publish" dotnet run dotnet-app/greeter.cs -- world
```

## Windows

Don't have a Windows machine, can't test.
