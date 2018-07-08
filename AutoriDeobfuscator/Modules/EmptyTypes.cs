using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using dnlib.DotNet;
using dnlib.DotNet.Emit;

namespace AutoriDeobfuscator.Modules
{
    class EmptyTypes : AutoriDeobfuscator
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


                        for(int i = 0; i < body.Instructions.Count; i++)
                        {
                            if(body.Instructions[i].OpCode == OpCodes.Ldsfld &&
                               body.Instructions[i].Operand.ToString().Contains("System.Type::EmptyTypes"))
                            {
                                Console.WriteLine("[!] Detected Empty Types in method: {0}; line: {1}", method.Name, i);

                                for (int j = 0; j < 3; j++)
                                    body.Instructions.RemoveAt(i);

                                Console.WriteLine("[+] Removed Empty Types in method: {0}; line: {1}", method.Name, i);

                            }
                        }

                        
                    }
                }
            }
        }
    }
}
