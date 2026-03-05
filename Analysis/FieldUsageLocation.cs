using System;
using System.Reflection;

namespace RimWorldModDevProbe.Analysis
{
    public enum FieldAccessType
    {
        Read,
        Write,
        ReadWrite
    }

    public class FieldUsageLocation
    {
        public string TypeName { get; set; }
        public string MethodName { get; set; }
        public string MethodSignature { get; set; }
        public FieldAccessType AccessType { get; set; }
        public int ILOffset { get; set; }
        public MethodInfo MethodInfo { get; set; }

        public override string ToString()
        {
            var access = AccessType == FieldAccessType.Read ? "Read" :
                         AccessType == FieldAccessType.Write ? "Write" : "ReadWrite";
            return $"{TypeName}.{MethodName} [{access}] @ IL_{ILOffset:X4}";
        }
    }
}
