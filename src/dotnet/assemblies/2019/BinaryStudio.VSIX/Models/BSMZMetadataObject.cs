using BinaryStudio.PortableExecutable;

namespace BinaryStudio.VSShellPackage.Models
    {
    [Model(typeof(MZMetadataObject))]
    public class BSMZMetadataObject : NotifyPropertyChangedDispatcherObject<MZMetadataObject>
        {
        public BSMZMetadataObject(MZMetadataObject source)
            : base(source)
            {
            }
        }
    }