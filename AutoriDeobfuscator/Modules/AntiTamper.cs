using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using dnlib.DotNet;
using dnlib.DotNet.Emit;

namespace AutoriDeobfuscator.Modules
{
    class AntiTamper : AutoriDeobfuscator
    {
        public override void Deobfuscate(AssemblyDef asm)
        {
            foreach (var mod in asm.Modules)
            {
                foreach (var type in mod.Types)
                {

                    for (int i = 0; i < type.Methods.Count; i++)
                    {
                        var method = type.Methods[i];
                        if (!method.HasBody) continue;
                        if (method.HasBody) if (!method.Body.HasInstructions) continue;
                        
                        var body = method.Body;


                        for (int j = 0; j < body.Instructions.Count; j++)
                        {
                            if(body.Instructions[j].OpCode == OpCodes.Call&&
                               body.Instructions[j].Operand.ToString().Contains("GetExecutingAssembly") &&
                               body.Instructions[j + 1].OpCode == OpCodes.Callvirt &&
                               body.Instructions[j + 1].Operand.ToString().Contains("get_Location"))
                            {

                                Console.WriteLine("[!] Detected AntiTamper in method: {0};", method.Name);

                                Utils.RemoveCall(asm, method);
                                type.Methods.RemoveAt(i);

                                Console.WriteLine("[+] Removed AntiTamper in method: {0};", method.Name);


                            }
                        }
                    }
                }
            }
        }
    }
}

