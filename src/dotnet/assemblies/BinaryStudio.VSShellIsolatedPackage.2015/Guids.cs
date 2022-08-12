// Guids.cs
// MUST match guids.h
using System;

namespace BinaryStudio.VSShellIsolatedPackage2015.AboutBoxPackage
{
    static class GuidList
    {
        public const string guidAboutBoxPackagePkgString = "52891e72-35f2-446c-8cfc-4bcb4c417ba6";
        public const string guidAboutBoxPackageCmdSetString = "6187d09a-aae7-4f8e-8326-5a25a77adbf1";

        public static readonly Guid guidAboutBoxPackageCmdSet = new Guid(guidAboutBoxPackageCmdSetString);
    };
}