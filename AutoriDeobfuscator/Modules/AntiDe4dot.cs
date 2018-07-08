using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using dnlib.DotNet;

namespace AutoriDeobfuscator.Modules
{
    class AntiDe4dot : AutoriDeobfuscator
    {
        public override void Deobfuscate(AssemblyDef asm)
        {
            foreach(var mod in asm.Modules)
            {
                for(int i = 0; i < mod.Types.Count; i++)
                {
                    TypeDef type = mod.Types[i];
                    if (type.HasInterfaces)
                    {
                        for (int j = 0; j < type.Interfaces.Count; j++)
                        {
                            if (type.Interfaces[j].Interface.Name.Contains(type.Name) || 
                                type.Name.Contains(type.Interfaces[j].Interface.Name) )
                            {
                                Console.WriteLine("[!] Detected AntiDe4dot in type {0}", type.Name);
                                mod.Types.RemoveAt(i);
                                Console.WriteLine("[+] Removed AntiDe4dot in type {0}", type.Name);


                            }
                        }
                    }
                }
            }
        }
    }
}
