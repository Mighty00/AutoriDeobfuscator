using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using dnlib.DotNet;
using dnlib.DotNet.Emit;

namespace AutoriDeobfuscator.Modules
{
    class AntiDecompiler : AutoriDeobfuscator
    {
        public override void Deobfuscate(AssemblyDef asm)
        {
            foreach (var mod in asm.Modules)
            {
                foreach (var type in mod.Types)
                {
                    foreach (var method in type.Methods)
                    {
                        if (!method.HasBody) continue;
                        if (method.HasBody) if (!method.Body.HasInstructions) continue;
                        var body = method.Body;

                        for (int i = 0; i < body.ExceptionHandlers.Count; i++)
                        {
                            if (body.ExceptionHandlers[i].HandlerType == ExceptionHandlerType.Finally)
                            {
                                for (int j = 0; j < body.Instructions.Count; j++)
                                {
                                    if(body.Instructions[j].OpCode == OpCodes.Calli &&
                                       body.Instructions[j].Operand == null &&
                                       body.Instructions[j + 1].OpCode == OpCodes.Sizeof)
                                    {
                                        Console.WriteLine("[!] Detected Anti Decompiler in method: {0}", method.Name);
                                        body.ExceptionHandlers.RemoveAt(i);
                                        body.Instructions.RemoveAt(j);
                                        body.Instructions.RemoveAt(j + 1);
                                        Console.WriteLine("[+] Removed Anti Decompiler in method: {0}; line: {1}", method.Name, i);
                                    }
                                }


                        
                            }
                        }
                    }
                }
            }
        }
    }
}
