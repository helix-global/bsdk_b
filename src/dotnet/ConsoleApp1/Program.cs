using System;
using BinaryStudio.PortableExecutable;

namespace ConsoleApp1
    {
    class Program
        {
        static void Main(string[] args)
            {
            using (var scope = new MetadataScope())
                {
                var r = scope.Load(@"C:\TFS\rtEditor.exe");
                }
            }
        }
    }
