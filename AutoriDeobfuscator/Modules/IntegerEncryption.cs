using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using dnlib.DotNet;
using dnlib.DotNet.Emit;

namespace AutoriDeobfuscator.Modules
{
    class IntegerEncryption : AutoriDeobfuscator
    {

        private MethodDef getCollatz(AssemblyDef asm)
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

                        for (int i = 0; i < body.Instructions.Count; i++)
                        {
                            if (body.Instructions[i].IsLdarg() &&
                               body.Instructions[i + 1].IsLdcI4() && body.Instructions[i + 1].GetLdcI4Value() == 1 &&
                               body.Instructions[i + 2].OpCode == OpCodes.Ceq)
                                return method;
                        }
                    }
                }
            }
            return null;
        }

        public override void Deobfuscate(AssemblyDef asm)
        {
            MethodDef collatzConjecture = getCollatz(asm);
            foreach (var mod in asm.Modules)
            {
                foreach (var type in mod.Types)
                {
                    foreach (var method in type.Methods)
                    {
                        if (!method.HasBody) continue;
                        if (method.HasBody) if (!method.Body.HasInstructions) continue;
                        var body = method.Body;

                        for (int i = 0; i < body.Instructions.Count; i++)
                        {
                            if(body.Instructions[i].OpCode == OpCodes.Call &&
                               body.Instructions[i].Operand as MethodDef == collatzConjecture)
                            {
                                Console.WriteLine("[!] Detected CollatzConjecture Call in method: {0}; line: {1}", method.Name, i);
                                body.Instructions.RemoveAt(i);
                                body.Instructions[i - 1] = OpCodes.Ldc_I4_1.ToInstruction();
                                Console.WriteLine("[!] Removed CollatzConjecture Call in method: {0}; line: {1}", method.Name, i);


                            }

                        }
                    }
                }
            }
        }
    }
}
