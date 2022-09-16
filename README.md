# dotnet-global-funcs
A .Net global tool to identify global function calls in CalcEngine risk projects. By default a full list of call detail per C# (.cs) file will be returned. Optionally a list of distinct global function names can be returned, or a list of detailed usages for a given function name.

## Overview

When global functions are called from a risk solution, they are called using the built-in function **GlobalValueOf**. This tool allows easy identification of such calls, with generic and function argument information.

- [Installation](#installation)
- [Usage](#usage)
- [Specifying the path](#specifying-the-path)

## Installation

Download and install the [.NET 5 SDK](https://dotnet.microsoft.com/download/visual-studio-sdks) or newer. Once installed, run the following command:

```bash
dotnet tool install --global global-funcs
```

If you already have a previous version of **global-funcs** installed, you can upgrade to the latest version using the following command:

```bash
dotnet tool update --global global-funcs
```
## Usage

```text
Usage: dotnet global-funcs <Path>

Arguments:
  Path                                       The path to a directory containing C# files. If none is specified, the current directory will be used.

Options:
  --version                                  Show version information.
  -?|-h|--help                               Show this help message.
  -d|--distinct                              Will only list distinct functions called from risks without any detail. Useful for identifying global functions in use. Cannot be combined with the 'Filter' (-f) option.
  -f|--filter <FUNCTION>                     Only include function call detail for the specified function name. The filter value must match the function name exactly.
```

```bash
dotnet global-funcs C:\Outsurance\out-risks-commercial\src\cs
or
dotnet global-funcs C:\Outsurance\out-risks-commercial\src\cs\ -d
or
dotnet global-funcs C:\Outsurance\out-risks-commercial\src\cs\ -f SPP_Factors
```
## Specifying the path
You can run **global-funcs** without specifying the `Path` argument. In this case, it will look in the current directory and recursively scan for all C# (.cs) files.

You can also pass a directory in the `Path` argument, in which case the same logic described above will be used, but in the directory specified.
