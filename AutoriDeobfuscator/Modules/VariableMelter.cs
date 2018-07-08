using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using dnlib.DotNet;
using dnlib.DotNet.Emit;

namespace AutoriDeobfuscator.Modules
{
    class VariableMelter : AutoriDeobfuscator
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
                            if (body.Instructions[i].OpCode == OpCodes.Stloc_S &&
                               body.Instructions[i + 2].OpCode == OpCodes.Stloc_S)
                            {
                                Console.WriteLine("[!] Detected Variable Melter in method: {0}; line: {1}", method.Name, i);
                                int loops = 0;
                                if (body.Instructions[i + 4].OpCode == OpCodes.Call)
                                    loops = 2;
                                else
                                    loops = 4;

                                i += 2; //second local init

                                for (int j = 0; j < loops; j++)
                                    body.Instructions.RemoveAt(i);

                                //There are some problems..
                            }


                        }
                    }
                }
            }
        }
    }
}

