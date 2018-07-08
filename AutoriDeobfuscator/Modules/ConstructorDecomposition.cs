using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using dnlib.DotNet;
using dnlib.DotNet.Emit;

namespace AutoriDeobfuscator.Modules
{
    class ConstructorDecomposition : AutoriDeobfuscator
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

                        for (int i = 0; i < body.Instructions.Count; i++)
                        {
                            if (body.Instructions[i].IsLdcI4() &&
                               body.Instructions[i + 1].OpCode == OpCodes.Stloc_S &&
                               body.Instructions[i + 2].OpCode == OpCodes.Ldloc_S &&
                               body.Instructions[i + 3].IsLdcI4() &&
                               body.Instructions[i + 11].OpCode == OpCodes.Call &&
                               body.Instructions[i + 11].Operand.ToString().Contains(".ctor"))
                            {
                                Console.WriteLine("[!] Detected Constructor Decomposition in method: {0}; line: {1}", method.Name, i);
                                for (int j = 0; j < 12; j++)
                                    body.Instructions.RemoveAt(i);

                                Console.WriteLine("[+] Removed Constructor Decomposition in method: {0}; line: {1}", method.Name, i);


                            }
                        }

                    }
                }
            }
        }
    }
}
