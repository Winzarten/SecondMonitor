namespace SecondMonitor.PluginManager.GameConnector.SharedMemory
{
    using System.IO;
    using System.IO.MemoryMappedFiles;
    using System.Runtime.InteropServices;

    public class MappedBuffer<MappedBufferT> where MappedBufferT : struct
    {
        /*readonly int RF2_BUFFER_VERSION_BLOCK_SIZE_BYTES = Marshal.SizeOf(typeof(rF2MappedBufferVersionBlock));
        readonly int RF2_BUFFER_VERSION_BLOCK_WITH_SIZE_SIZE_BYTES = Marshal.SizeOf(typeof(rF2MappedBufferVersionBlockWithSize));*/

        private readonly int _bufferSizeBytes;
        private readonly string _bufferName;

        /* Holds the entire byte array that can be marshalled to a MappedBufferT.  Partial updates
         only read changed part of buffer, ignoring trailing uninteresting bytes.  However,
         to marshal we still need to supply entire structure size.  So, on update new bytes are copied
         (outside of the mutex). */
        private MemoryMappedFile _memoryMappedFile;


        public MappedBuffer(string buffName)
        {
            _bufferSizeBytes = Marshal.SizeOf(typeof(MappedBufferT));
            _bufferName = buffName;

        }

        public void Connect()
        {
            _memoryMappedFile = MemoryMappedFile.OpenExisting(this._bufferName);
        }

        public void Disconnect()
        {
            _memoryMappedFile?.Dispose();
            _memoryMappedFile = null;
        }

        public MappedBufferT GetMappedDataUnSynchronized()
        {
            MappedBufferT mappedData = new MappedBufferT();
            using (var sharedMemoryStreamView = this._memoryMappedFile.CreateViewStream())
            {
                var sharedMemoryStream = new BinaryReader(sharedMemoryStreamView);
                var sharedMemoryReadBuffer = sharedMemoryStream.ReadBytes(this._bufferSizeBytes);

                var handleBuffer = GCHandle.Alloc(sharedMemoryReadBuffer, GCHandleType.Pinned);
                mappedData = (MappedBufferT)Marshal.PtrToStructure(handleBuffer.AddrOfPinnedObject(), typeof(MappedBufferT));
                handleBuffer.Free();
            }

            return mappedData;
        }
    }
}