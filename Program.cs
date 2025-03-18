using CsLLVM;

namespace llvm
{
    internal class Program
    {
        static void Main(string[] args)
        {
            var module = new LLVMModule("TestModule");

         
            module.AddGlobal("@.str = private unnamed_addr constant [10 x i8] c\"Result: %d\\0A\\00\"");

        
            var funcAdd = new LLVMFunction("add", "i32", "i32 %a", "i32 %b");
            funcAdd.AddInstruction("%sum = add i32 %a, %b");
            funcAdd.AddInstruction("ret i32 %sum");

   
            var funcMain = new LLVMFunction("main", "i32");
            funcMain.AddInstruction("%result = call i32 @add(i32 5, i32 7)");
            funcMain.AddInstruction("%formatPtr = bitcast [10 x i8]* @.str to i8*");
            funcMain.AddInstruction("call i32 (i8*, ...) @printf(i8* %formatPtr, i32 %result)");
            funcMain.AddInstruction("ret i32 0");


            module.AddFunction(funcAdd.GenerateIR());
            module.AddFunction(funcMain.GenerateIR());

       
            string llvmIR = module.GenerateIR();
            Console.WriteLine(llvmIR);
        }
    }
}
