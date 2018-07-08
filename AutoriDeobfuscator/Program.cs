using AutoriDeobfuscator.Modules;
using dnlib.DotNet;
using dnlib.DotNet.Writer;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace AutoriDeobfuscator
{
    class Program
    {

        private static List<AutoriDeobfuscator> autoriDeobfuscators;
        static void Main(string[] args)
        {
            Console.ForegroundColor = ConsoleColor.Green;
            Console.WriteLine("Autori Deobfuscator");
            var filePath = Console.ReadLine();
            AssemblyDef asm = AssemblyDef.Load(filePath);
            loadModules();
            Console.ForegroundColor = ConsoleColor.Yellow;
            executeModules(asm);
            var opts = new ModuleWriterOptions();
            opts.Logger = DummyLogger.NoThrowInstance;
            asm.Write(filePath.Replace(".exe", "-cleaned.exe"), opts);
            Console.ReadKey();

        }

        private static void loadModules()
        {
            autoriDeobfuscators = new List<AutoriDeobfuscator>()
            {
                new AntiDecompiler(),
                new ConstructorDecomposition(),
                new AntiDe4dot(),
                new EmptyTypes(),
                new IntegerEncryption(),
                new ConstantHider(),
                new StringEncryption(),
                new AntiTamper()
            };
        }

        private static void executeModules(AssemblyDef asm)
        {

            autoriDeobfuscators.ForEach(x => x.Deobfuscate(asm));

        }
    }
}
