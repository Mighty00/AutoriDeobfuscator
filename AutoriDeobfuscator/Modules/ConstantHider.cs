using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using dnlib.DotNet;
using dnlib.DotNet.Emit;

namespace AutoriDeobfuscator.Modules
{
    class ConstantHider : AutoriDeobfuscator
    {

        private static Dictionary<FieldDef, string> proxyString = new Dictionary<FieldDef, string>();
        private void getFields(AssemblyDef asm)
        {
            var staticConstructor = asm.ManifestModule.GlobalType.FindOrCreateStaticConstructor();
            var body = staticConstructor.Body;
            for(int i = 0; i< body.Instructions.Count; i++)
            {
                if (body.Instructions[i].OpCode == OpCodes.Ldstr && body.Instructions[i + 1].OpCode == OpCodes.Stsfld)
                {
                    string strOperand = body.Instructions[i].Operand.ToString();
                    FieldDef strField = body.Instructions[i + 1].Operand as FieldDef;
                    proxyString.Add(strField, strOperand);
                }
            }
        }
        public override void Deobfuscate(AssemblyDef asm)
        {
            getFields(asm);

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
                            if(body.Instructions[i].OpCode == OpCodes.Ldsfld)
                            {
                                Console.WriteLine("[!] Detected Constant Hider in method: {0}; line: {1}", method.Name, i);
                                FieldDef strField = body.Instructions[i].Operand as FieldDef;
                                Console.WriteLine(strField.Name);
                                string extracted = null;
                             
                                 if (proxyString.TryGetValue(strField, out extracted))
                                   {
                                        body.Instructions[i] = OpCodes.Ldstr.ToInstruction(extracted);
                                        Console.WriteLine("[+] Replaced field in method: {0}; line: {1} with string value: {2}", method.Name, i, extracted);
                                    }
                            }
                        }

                    }
                }
            }
        }
    }
}
