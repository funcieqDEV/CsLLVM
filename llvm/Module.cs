using System;
using System.Collections.Generic;
using System.Text;

class LLVMModule
{
    private string moduleName;
    private List<string> functions = new List<string>();
    private List<string> globalDecls = new List<string>();

    public LLVMModule(string name)
    {
        moduleName = name;
    }

    public void AddFunction(string functionIR)
    {
        functions.Add(functionIR);
    }

    public void AddGlobal(string globalIR)
    {
        globalDecls.Add(globalIR);
    }

    public string GenerateIR()
    {
        var sb = new StringBuilder();
        sb.AppendLine($"; ModuleID = \"{moduleName}\"");
        sb.AppendLine("target triple = \"x86_64-pc-linux-gnu\"");
        sb.AppendLine();

  
        sb.AppendLine("declare i32 @printf(i8*, ...)");

    
        foreach (var global in globalDecls)
        {
            sb.AppendLine(global);
        }

        sb.AppendLine();

     
        foreach (var func in functions)
        {
            sb.AppendLine(func);
        }

        return sb.ToString();
    }
}

class LLVMFunction
{
    private string functionName;
    private string returnType;
    private List<string> parameters;
    private List<string> body = new List<string>();

    public LLVMFunction(string name, string returnType, params string[] parameters)
    {
        this.functionName = name;
        this.returnType = returnType;
        this.parameters = new List<string>(parameters);
    }

    public void AddInstruction(string instruction)
    {
        body.Add(instruction);
    }

    public string GenerateIR()
    {
        var sb = new StringBuilder();
        sb.Append($"define {returnType} @{functionName}(");
        sb.Append(string.Join(", ", parameters));
        sb.AppendLine(") {");
        sb.AppendLine("entry:");
        foreach (var instr in body)
        {
            sb.AppendLine("  " + instr);
        }
        sb.AppendLine("}");
        return sb.ToString();
    }
}

class LLVMBuilder
{
    private int tempCounter = 0;
    private LLVMFunction function;

    public LLVMBuilder(LLVMFunction function)
    {
        this.function = function;
    }

    private string NewTemp() => $"%t{tempCounter++}";

    public string Alloca(string type)
    {
        string varName = NewTemp();
        function.AddInstruction($"{varName} = alloca {type}");
        return varName;
    }

    public void Store(string value, string variable)
    {
        function.AddInstruction($"store i32 {value}, i32* {variable}");
    }

    public string Load(string variable)
    {
        string temp = NewTemp();
        function.AddInstruction($"{temp} = load i32, i32* {variable}");
        return temp;
    }

    public string Add(string a, string b)
    {
        string temp = NewTemp();
        function.AddInstruction($"{temp} = add i32 {a}, {b}");
        return temp;
    }

    public void Print(string value)
    {
        function.AddInstruction($"%formatPtr = bitcast [12 x i8]* @.str to i8*");
        function.AddInstruction($"call i32 (i8*, ...) @printf(i8* %formatPtr, i32 {value})");
    }
}

