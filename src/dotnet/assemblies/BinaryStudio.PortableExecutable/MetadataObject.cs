using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Threading;
using System.Threading.Tasks;
using BinaryStudio.IO;

namespace BinaryStudio.PortableExecutable
    {
    public class MetadataObject : IDisposable, IServiceProvider
        {
        public virtual MetadataObjectState State { get { return state; }}
        public MetadataScope Scope { get; }
        public MetadataObjectIdentity Identity { get; }

        protected MetadataObject(MetadataScope scope, MetadataObjectIdentity identity)
            {
            if (scope == null)    { throw new ArgumentNullException(nameof(scope));    }
            if (identity == null) { throw new ArgumentNullException(nameof(identity)); }
            Scope = scope;
            Identity = identity;
            }

        #region M:Dispose(Boolean)
        /// <summary>Releases the unmanaged resources used by the <see cref="MetadataObject"/> and optionally releases the managed resources.</summary>
        /// <param name="disposing">true to release both managed and unmanaged resources; false to release only unmanaged resources.</param>
        protected virtual void Dispose(Boolean disposing) {
            state = MetadataObjectState.Disposed;
            if (!Disposed) {
                try
                    {
                    if (disposing) {
                        mappings.Clear();
                        }
                    }
                finally
                    {
                    Disposed = true;
                    }
                }
            }
        #endregion
        #region M:Finalize
        ~MetadataObject() {
            Dispose(false);
            }
        #endregion
        #region M:IDisposable.Dispose
        /// <summary>Performs application-defined tasks associated with freeing, releasing, or resetting unmanaged resources.</summary>
        public void Dispose()
            {
            Dispose(true);
            GC.SuppressFinalize(this);
            }
        #endregion
        #region M:IServiceProvider.GetService(Type):Object
        /// <summary>Gets the service object of the specified type.</summary>
        /// <param name="type">An object that specifies the type of service object to get.</param>
        /// <returns>A service object of type <paramref name="type"/>. -or- <see langword="null" /> if there is no service object of type <paramref name="type"/>.</returns>
        public virtual Object GetService(Type type)
            {
            if (type == null) { throw new ArgumentNullException(nameof(type)); }
            if (type == GetType()) { return this; }
            return null;
            }
        #endregion

        #region M:LoadAsync(FileMappingMemory,Int64,Int64,CancellationToken):Task
        public Task LoadAsync(FileMappingMemory source,Int64 offset,Int64 length, CancellationToken cancellationToken = default(CancellationToken))
            {
            if (source == null) { throw new ArgumentNullException(nameof(source)); }
            if (length < 0) { throw new ArgumentOutOfRangeException(nameof(length)); }
            if (offset < 0) { throw new ArgumentOutOfRangeException(nameof(offset)); }
            if (State != MetadataObjectState.Pending) { throw new InvalidOperationException(); }
            #if NET40
            return Task.Factory.StartNew(delegate{ Load(source,offset,length); },cancellationToken);
            #else
            return Task.Run(delegate{ Load(source,offset,length); },cancellationToken);
            #endif
            }
        #endregion
        #region M:Load(FileMappingMemory,Int64,Int64)
        public unsafe void Load(FileMappingMemory source,Int64 offset, Int64 length)
            {
            if (source == null) { throw new ArgumentNullException(nameof(source)); }
            if (length < 0) { throw new ArgumentOutOfRangeException(nameof(length)); }
            if (offset < 0) { throw new ArgumentOutOfRangeException(nameof(offset)); }
            if (State != MetadataObjectState.Pending) { throw new InvalidOperationException(); }
            Debug.Print("Loading {{{0}}}:{{{1}}}...", Identity,GetType().Name);
            try
                {
                state = MetadataObjectState.Loading;
                mappings.Push(source);
                LoadCore(((Byte*)(void*)source) + offset, length);
                state = MetadataObjectState.Loaded;
                }
            catch
                {
                mappings.Pop();
                state = MetadataObjectState.Failed;
                }
            Debug.Print("Completed {{{0}}}:{{{1}}}...", Identity,GetType().Name);
            }
        #endregion
        #region M:LoadCore(Byte*,Int64)
        protected virtual unsafe void LoadCore(Byte* source,Int64 length)
            {
            if (source == null) { throw new ArgumentNullException(nameof(source)); }
            if (length < 0) { throw new ArgumentOutOfRangeException(nameof(length)); }
            }
        #endregion

        private MetadataObjectState state = MetadataObjectState.Pending;
        private readonly Stack<FileMappingMemory> mappings = new Stack<FileMappingMemory>();
        private Boolean Disposed;
        }
    }