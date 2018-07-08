using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using dnlib.DotNet;
using dnlib.DotNet.Emit;

namespace AutoriDeobfuscator.Modules
{
    class StringEncryption : AutoriDeobfuscator
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
                            if (body.Instructions[i].OpCode == OpCodes.Ldstr&&
                                body.Instructions[i + 1].OpCode == OpCodes.Ldstr&&
                                body.Instructions[i + 2].OpCode == OpCodes.Call)
                            {
                                Console.WriteLine("[!] Detected String Encryption in method: {0}; line: {1}", method.Name, i);
                                string xored = body.Instructions[i].Operand.ToString();
                                string key = body.Instructions[i + 1].Operand.ToString();
                                try
                                {
                                    body.Instructions[i] = OpCodes.Ldstr.ToInstruction(Utils.DecryptXorString(xored, key));
                                    body.Instructions.RemoveAt(i + 1);
                                    body.Instructions.RemoveAt(i + 2);
                                    Console.WriteLine("[+] Removed String Encryption in method: {0}; line: {1}", method.Name, i);


                                    if (body.Instructions[i + 1].OpCode == OpCodes.Call)
                                    {
                                        Console.WriteLine("[!] Detected String Encryption (2) in method: {0}; line: {1}", method.Name, i);
                                        string rijndael = body.Instructions[i].Operand.ToString();
                                        body.Instructions[i] = OpCodes.Ldstr.ToInstruction(Utils.DecryptRijndaelManagedString(rijndael));
                                        body.Instructions.RemoveAt(i + 1);
                                        Console.WriteLine("[+] Detected String Encryption (2) in method: {0}; line: {1}", method.Name, i);

                                    }
                                }
                                catch
                                {
                                    continue;
                                }

                            }

                        }
                    }
                }
            }
        }
    }
 }

