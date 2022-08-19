using Microsoft.VisualStudio.Shell;
using System;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;
using System.Threading;
using BinaryStudio.PortableExecutable;
using Microsoft.VisualStudio;
using Microsoft.VisualStudio.Shell.Interop;
using Task = System.Threading.Tasks.Task;

namespace BinaryStudio.VSShellPackage
    {
    /// <summary>
    /// This is the class that implements the package exposed by this assembly.
    /// </summary>
    /// <remarks>
    /// <para>
    /// The minimum requirement for a class to be considered a valid package for Visual Studio
    /// is to implement the IVsPackage interface and register itself with the shell.
    /// This package uses the helper classes defined inside the Managed Package Framework (MPF)
    /// to do it: it derives from the Package class that provides the implementation of the
    /// IVsPackage interface and uses the registration attributes defined in the framework to
    /// register itself and its components with the shell. These attributes tell the pkgdef creation
    /// utility what data to put into .pkgdef file.
    /// </para>
    /// <para>
    /// To get loaded into VS, the package must be referred by &lt;Asset Type="Microsoft.VisualStudio.VsPackage" ...&gt; in .vsixmanifest file.
    /// </para>
    /// </remarks>
    [PackageRegistration(UseManagedResourcesOnly = true, AllowsBackgroundLoading = true)]
    [Guid(PackageGuidString)]
    [ProvideMenuResource("Menus.ctmenu", 1)]
    [ProvideToolWindow(typeof(ToolWindow1))]
    [ProvideEditorExtension(typeof(EditorFactory), ".exe", 32000, EditorFactoryNotify = true, ProjectGuid = "{980ecd8c-7958-432c-9853-42926e4262ab}")]
    [ProvideEditorExtension(typeof(EditorFactory), ".dll", 32000, EditorFactoryNotify = true, ProjectGuid = "{980ecd8c-7958-432c-9853-42926e4262ab}")]
    [ProvideEditorExtension(typeof(EditorFactory), ".ocx", 32000, EditorFactoryNotify = true, ProjectGuid = "{980ecd8c-7958-432c-9853-42926e4262ab}")]
    [ProvideEditorLogicalView(typeof(EditorFactory), "{e08b9e5e-3e89-47d3-8650-d9c9e3dbb5d0}")]
    public sealed class VSPackage : AsyncPackage
        {
        /// <summary>
        /// BinaryStudio.VSIXPackage GUID string.
        /// </summary>
        public const String PackageGuidString = "8dda4a86-d75f-4234-b023-d9f956e1557b";
        private EditorFactory EditorFactoryInstance;
        public static MetadataScope MetadataScope { get;private set; }

        #region Package Members
        /// <summary>
        /// Initialization of the package; this method is called right after the package is sited, so this is the place
        /// where you can put all the initialization code that rely on services provided by VisualStudio.
        /// </summary>
        /// <param name="cancellationToken">A cancellation token to monitor for initialization cancellation, which can occur when VS is shutting down.</param>
        /// <param name="progress">A provider for progress updates.</param>
        /// <returns>A task representing the async work of package initialization, or an already completed task if there is none. Do not return null from this method.</returns>
        protected override async Task InitializeAsync(CancellationToken cancellationToken, IProgress<ServiceProgressData> progress) {
            EditorFactoryInstance = new EditorFactory();
            RegisterEditorFactory(EditorFactoryInstance);
            await JoinableTaskFactory.SwitchToMainThreadAsync(cancellationToken);
            await UpdateMRUCommandsAsync(cancellationToken,GetGlobalService(typeof(SVsMRUItemsStore)) as IVsMRUItemsStore);
            await ToolWindow1Command.InitializeAsync(this);
            }
        #endregion
        #region M:UpdateMRUCommandsAsync(CancellationToken,IVsMRUItemsStore):Task
        private async Task UpdateMRUCommandsAsync(CancellationToken cancellationToken,IVsMRUItemsStore MRUItemsStore) {
            await JoinableTaskFactory.SwitchToMainThreadAsync(cancellationToken);
            if (MRUItemsStore != null) {
                var EditorFactoryGuid = typeof(EditorFactory).GUID.ToString("B");
                var MruFiles = new String[VSConstants.VSStd97CmdID.MRUFile25 - VSConstants.VSStd97CmdID.MRUFile1 + 1];
                var MruFilesGuid = VSConstants.MruList.Files;
                MRUItemsStore.GetMRUItems(ref MruFilesGuid,String.Empty,(UInt32)MruFiles.Length,MruFiles);
                for (var i = 0; i < MruFiles.Length; i++) {
                    if (MruFiles[i] != null) {
                        var Entries = MruFiles[i].Split(new []{ '|' }, StringSplitOptions.None);
                        if (Entries.Length >= 2) {
                            switch (Path.GetExtension(Entries[0]).ToLowerInvariant()) {
                                case ".exe":
                                case ".dll":
                                case ".ocx":
                                    {
                                    Entries[1] = EditorFactoryGuid;
                                    }
                                    break;
                                }
                            MruFiles[i] = String.Join("|", Entries);
                            }
                        }
                    }
                MRUItemsStore.DeleteMRUItems(ref MruFilesGuid);
                foreach (var i in MruFiles.Where(i => i != null)) {
                    MRUItemsStore.AddMRUItem(ref MruFilesGuid, i);
                    }
                }
            }
        #endregion

        static VSPackage()
            {
            MetadataScope = new MetadataScope();
            }
        }
    }
