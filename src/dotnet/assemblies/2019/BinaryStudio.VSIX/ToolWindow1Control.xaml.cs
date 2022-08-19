using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;
using System.Windows;
using System.Windows.Controls;
using EnvDTE;
using Microsoft.VisualStudio;
using Microsoft.VisualStudio.Shell;
using Microsoft.VisualStudio.Shell.Interop;

namespace BinaryStudio.VSShellPackage
    {
    /// <summary>
    /// Interaction logic for ToolWindow1Control.
    /// </summary>
    public partial class ToolWindow1Control : UserControl
        {
        /// <summary>
        /// Initializes a new instance of the <see cref="ToolWindow1Control"/> class.
        /// </summary>
        public ToolWindow1Control()
            {
            this.InitializeComponent();
            }

        /// <summary>
        /// Handles click on the button by displaying a message box.
        /// </summary>
        /// <param name="sender">The event sender.</param>
        /// <param name="e">The event args.</param>
        [SuppressMessage("Microsoft.Globalization", "CA1300:SpecifyMessageBoxOptions", Justification = "Sample code")]
        [SuppressMessage("StyleCop.CSharp.NamingRules", "SA1300:ElementMustBeginWithUpperCaseLetter", Justification = "Default event handler naming pattern")]
        private void button1_Click(object sender, RoutedEventArgs e)
            {
            ThreadHelper.ThrowIfNotOnUIThread();
            if (Package.GetGlobalService(typeof(IVsShell)) is IVsShell Shell) {
                Shell.GetPackageEnum(out var EnumPackage);
                if (EnumPackage != null) {
                    var Packages = new IVsPackage[1];
                    while (EnumPackage.Next(1,Packages,out var Fetched) == VSConstants.S_OK) {
                        if ((Fetched == 1) && (Packages[0] is Package Package)) {
                            Debug.Print("Package:{{{0}}}", Package.GetType().AssemblyQualifiedName);
                            var Attributes = Package.GetType().GetCustomAttributes(true);
                            foreach (var attribute in Attributes) {
                                Debug.Print("  Attribute:{{{0}}}", attribute.GetType().AssemblyQualifiedName);
                                }
                            }
                        }
                    }
                }
            MessageBox.Show(
                string.Format(System.Globalization.CultureInfo.CurrentUICulture, "Invoked '{0}'", this.ToString()),
                "ToolWindow1");
            }
        }
    }