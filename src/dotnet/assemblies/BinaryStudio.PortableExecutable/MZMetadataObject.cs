using System;
using System.IO;
using BinaryStudio.PortableExecutable.Win32;

namespace BinaryStudio.PortableExecutable
    {
    public class MZMetadataObject : MetadataObject
        {
        internal MZMetadataObject(MetadataScope scope, MetadataObjectIdentity identity)
            : base(scope, identity)
            {
            }

        protected override unsafe void LoadCore(Byte* source, Int64 length)
            {
            if (source == null) { throw new ArgumentNullException(nameof(source)); }
            if (length < 0) { throw new ArgumentOutOfRangeException(nameof(length)); }
            if (length < sizeof(IMAGE_DOS_HEADER)) { throw new InvalidDataException(); }
            var header = (IMAGE_DOS_HEADER*)source;
            }
        }
    }