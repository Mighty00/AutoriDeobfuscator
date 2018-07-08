using dnlib.DotNet;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace AutoriDeobfuscator
{
    public abstract class AutoriDeobfuscator
    {
        public abstract void Deobfuscate(AssemblyDef asm);
    }
}
