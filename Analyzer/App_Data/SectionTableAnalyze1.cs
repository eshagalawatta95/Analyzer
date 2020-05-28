using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using System.IO;

namespace _Peanalyzer.Data
{
    public class PeHeaderReader
    {
        #region File Header Structures

        public struct IMAGE_DOS_HEADER
        {     
            public UInt16 e_magic;            
            public UInt16 e_cblp;             
            public UInt16 e_cp;               
            public UInt16 e_crlc;              
            public UInt16 e_cparhdr;           
            public UInt16 e_minalloc;         
            public UInt16 e_maxalloc;          
            public UInt16 e_ss;               
            public UInt16 e_sp;                
            public UInt16 e_csum;              
            public UInt16 e_ip;         
            public UInt16 e_cs;                 
            public UInt16 e_lfarlc;            
            public UInt16 e_ovno;              
            public UInt16 e_res_0;              
            public UInt16 e_res_1;            
            public UInt16 e_res_2;            
            public UInt16 e_res_3;           
            public UInt16 e_oemid;           
            public UInt16 e_oeminfo;       
            public UInt16 e_res2_0;          
            public UInt16 e_res2_1;          
            public UInt16 e_res2_2;            
            public UInt16 e_res2_3;           
            public UInt16 e_res2_4;            
            public UInt16 e_res2_5;          
            public UInt16 e_res2_6;             
            public UInt16 e_res2_7;           
            public UInt16 e_res2_8;           
            public UInt16 e_res2_9;             
            public UInt32 e_lfanew;            
        }

        [StructLayout(LayoutKind.Sequential)]
        public struct IMAGE_DATA_DIRECTORY
        {
            public UInt32 VirtualAddress;
            public UInt32 Size;
        }

        [StructLayout(LayoutKind.Sequential, Pack = 1)]
        public struct IMAGE_OPTIONAL_HEADER32
        {
            public UInt16 Magic;
            public Byte MajorLinkerVersion;
            public Byte MinorLinkerVersion;
            public UInt32 SizeOfCode;
            public UInt32 SizeOfInitializedData;
            public UInt32 SizeOfUninitializedData;
            public UInt32 AddressOfEntryPoint;
            public UInt32 BaseOfCode;
            public UInt32 BaseOfData;
            public UInt32 ImageBase;
            public UInt32 SectionAlignment;
            public UInt32 FileAlignment;
            public UInt16 MajorOperatingSystemVersion;
            public UInt16 MinorOperatingSystemVersion;
            public UInt16 MajorImageVersion;
            public UInt16 MinorImageVersion;
            public UInt16 MajorSubsystemVersion;
            public UInt16 MinorSubsystemVersion;
            public UInt32 Win32VersionValue;
            public UInt32 SizeOfImage;
            public UInt32 SizeOfHeaders;
            public UInt32 CheckSum;
            public UInt16 Subsystem;
            public UInt16 DllCharacteristics;
            public UInt32 SizeOfStackReserve;
            public UInt32 SizeOfStackCommit;
            public UInt32 SizeOfHeapReserve;
            public UInt32 SizeOfHeapCommit;
            public UInt32 LoaderFlags;
            public UInt32 NumberOfRvaAndSizes;

            public IMAGE_DATA_DIRECTORY ExportTable;
            public IMAGE_DATA_DIRECTORY ImportTable;
            public IMAGE_DATA_DIRECTORY ResourceTable;
            public IMAGE_DATA_DIRECTORY ExceptionTable;
            public IMAGE_DATA_DIRECTORY CertificateTable;
            public IMAGE_DATA_DIRECTORY BaseRelocationTable;
            public IMAGE_DATA_DIRECTORY Debug;
            public IMAGE_DATA_DIRECTORY Architecture;
            public IMAGE_DATA_DIRECTORY GlobalPtr;
            public IMAGE_DATA_DIRECTORY TLSTable;
            public IMAGE_DATA_DIRECTORY LoadConfigTable;
            public IMAGE_DATA_DIRECTORY BoundImport;
            public IMAGE_DATA_DIRECTORY IAT;
            public IMAGE_DATA_DIRECTORY DelayImportDescriptor;
            public IMAGE_DATA_DIRECTORY CLRRuntimeHeader;
            public IMAGE_DATA_DIRECTORY Reserved;
        }

        [StructLayout(LayoutKind.Sequential, Pack = 1)]
        public struct IMAGE_OPTIONAL_HEADER64
        {
            public UInt16 Magic;
            public Byte MajorLinkerVersion;
            public Byte MinorLinkerVersion;
            public UInt32 SizeOfCode;
            public UInt32 SizeOfInitializedData;
            public UInt32 SizeOfUninitializedData;
            public UInt32 AddressOfEntryPoint;
            public UInt32 BaseOfCode;
            public UInt64 ImageBase;
            public UInt32 SectionAlignment;
            public UInt32 FileAlignment;
            public UInt16 MajorOperatingSystemVersion;
            public UInt16 MinorOperatingSystemVersion;
            public UInt16 MajorImageVersion;
            public UInt16 MinorImageVersion;
            public UInt16 MajorSubsystemVersion;
            public UInt16 MinorSubsystemVersion;
            public UInt32 Win32VersionValue;
            public UInt32 SizeOfImage;
            public UInt32 SizeOfHeaders;
            public UInt32 CheckSum;
            public UInt16 Subsystem;
            public UInt16 DllCharacteristics;
            public UInt64 SizeOfStackReserve;
            public UInt64 SizeOfStackCommit;
            public UInt64 SizeOfHeapReserve;
            public UInt64 SizeOfHeapCommit;
            public UInt32 LoaderFlags;
            public UInt32 NumberOfRvaAndSizes;

            public IMAGE_DATA_DIRECTORY ExportTable;
            public IMAGE_DATA_DIRECTORY ImportTable;
            public IMAGE_DATA_DIRECTORY ResourceTable;
            public IMAGE_DATA_DIRECTORY ExceptionTable;
            public IMAGE_DATA_DIRECTORY CertificateTable;
            public IMAGE_DATA_DIRECTORY BaseRelocationTable;
            public IMAGE_DATA_DIRECTORY Debug;
            public IMAGE_DATA_DIRECTORY Architecture;
            public IMAGE_DATA_DIRECTORY GlobalPtr;
            public IMAGE_DATA_DIRECTORY TLSTable;
            public IMAGE_DATA_DIRECTORY LoadConfigTable;
            public IMAGE_DATA_DIRECTORY BoundImport;
            public IMAGE_DATA_DIRECTORY IAT;
            public IMAGE_DATA_DIRECTORY DelayImportDescriptor;
            public IMAGE_DATA_DIRECTORY CLRRuntimeHeader;
            public IMAGE_DATA_DIRECTORY Reserved;
        }

        [StructLayout(LayoutKind.Sequential, Pack = 1)]
        public struct IMAGE_FILE_HEADER
        {
            public UInt16 Machine;
            public UInt16 NumberOfSections;
            public UInt32 TimeDateStamp;
            public UInt32 PointerToSymbolTable;
            public UInt32 NumberOfSymbols;
            public UInt16 SizeOfOptionalHeader;
            public UInt16 Characteristics;
        }

        [StructLayout(LayoutKind.Explicit)]
        public struct IMAGE_SECTION_HEADER
        {
            [FieldOffset(0)]
            [MarshalAs(UnmanagedType.ByValArray, SizeConst = 8)]
            public char[] Name;
            [FieldOffset(8)]
            public UInt32 VirtualSize;
            [FieldOffset(12)]
            public UInt32 VirtualAddress;
            [FieldOffset(16)]
            public UInt32 SizeOfRawData;
            [FieldOffset(20)]
            public UInt32 PointerToRawData;
            [FieldOffset(24)]
            public UInt32 PointerToRelocations;
            [FieldOffset(28)]
            public UInt32 PointerToLinenumbers;
            [FieldOffset(32)]
            public UInt16 NumberOfRelocations;
            [FieldOffset(34)]
            public UInt16 NumberOfLinenumbers;
            [FieldOffset(36)]
            public DataSectionFlags Characteristics;

            public string Section
            {
                get { return new string(Name); }
            }
        }

        [Flags]
        public enum DataSectionFlags : uint
        {
            
            TypeReg = 0x00000000,           
            TypeDsect = 0x00000001,           
            TypeNoLoad = 0x00000002,          
            TypeGroup = 0x00000004,           
            TypeNoPadded = 0x00000008,          
            TypeCopy = 0x00000010,          
            ContentCode = 0x00000020,         
            ContentInitializedData = 0x00000040,        
            ContentUninitializedData = 0x00000080,           
            LinkOther = 0x00000100,           
            LinkInfo = 0x00000200,           
            TypeOver = 0x00000400,          
            LinkRemove = 0x00000800,          
            LinkComDat = 0x00001000,      
            NoDeferSpecExceptions = 0x00004000,          
            RelativeGP = 0x00008000,           
            MemPurgeable = 0x00020000,         
            Memory16Bit = 0x00020000,           
            MemoryLocked = 0x00040000,           
            MemoryPreload = 0x00080000,          
            Align1Bytes = 0x00100000,          
            Align2Bytes = 0x00200000,          
            Align4Bytes = 0x00300000,          
            Align8Bytes = 0x00400000,           
            Align16Bytes = 0x00500000,         
            Align32Bytes = 0x00600000,            
            Align64Bytes = 0x00700000,          
            Align128Bytes = 0x00800000,           
            Align256Bytes = 0x00900000,            
            Align512Bytes = 0x00A00000,            
            Align1024Bytes = 0x00B00000,          
            Align2048Bytes = 0x00C00000,         
            Align4096Bytes = 0x00D00000,          
            Align8192Bytes = 0x00E00000,          
            LinkExtendedRelocationOverflow = 0x01000000,           
            MemoryDiscardable = 0x02000000,           
            MemoryNotCached = 0x04000000,           
            MemoryNotPaged = 0x08000000,         
            MemoryShared = 0x10000000,         
            MemoryExecute = 0x20000000,           
            MemoryRead = 0x40000000,           
            MemoryWrite = 0x80000000
        }

        #endregion File Header Structures

        #region Private Fields
       
        private IMAGE_DOS_HEADER dosHeader;
     
        private IMAGE_FILE_HEADER fileHeader;
       
        private IMAGE_OPTIONAL_HEADER32 optionalHeader32;
       
        private IMAGE_OPTIONAL_HEADER64 optionalHeader64;
       
        private IMAGE_SECTION_HEADER[] imageSectionHeaders;

        #endregion Private Fields

        #region Public Methods

        public PeHeaderReader(string filePath)
        {
            
            using (FileStream stream = new FileStream(filePath, System.IO.FileMode.Open, System.IO.FileAccess.Read))
            {
                BinaryReader reader = new BinaryReader(stream);
                dosHeader = FromBinaryReader<IMAGE_DOS_HEADER>(reader);

            
                stream.Seek(dosHeader.e_lfanew, SeekOrigin.Begin);

                UInt32 ntHeadersSignature = reader.ReadUInt32();
                fileHeader = FromBinaryReader<IMAGE_FILE_HEADER>(reader);
                if (this.Is32BitHeader)
                {
                    optionalHeader32 = FromBinaryReader<IMAGE_OPTIONAL_HEADER32>(reader);
                }
                else
                {
                    optionalHeader64 = FromBinaryReader<IMAGE_OPTIONAL_HEADER64>(reader);
                }

                imageSectionHeaders = new IMAGE_SECTION_HEADER[fileHeader.NumberOfSections];
                for (int headerNo = 0; headerNo < imageSectionHeaders.Length; ++headerNo)
                {
                    imageSectionHeaders[headerNo] = FromBinaryReader<IMAGE_SECTION_HEADER>(reader);
                }

            }
        }

        
        public static PeHeaderReader GetCallingAssemblyHeader()
        {
           
            string filePath = System.Reflection.Assembly.GetCallingAssembly().Location;           
            return new PeHeaderReader(filePath);
        }

        
        public static PeHeaderReader GetAssemblyHeader()
        {
           
            string filePath = System.Reflection.Assembly.GetAssembly(typeof(PeHeaderReader)).Location;
            return new PeHeaderReader(filePath);
        }

        
        /// <typeparam name="T"></typeparam>
        /// <param name="reader"></param>
        /// <returns></returns>
        public static T FromBinaryReader<T>(BinaryReader reader)
        {
          
            byte[] bytes = reader.ReadBytes(Marshal.SizeOf(typeof(T)));
           
            GCHandle handle = GCHandle.Alloc(bytes, GCHandleType.Pinned);
            T theStructure = (T)Marshal.PtrToStructure(handle.AddrOfPinnedObject(), typeof(T));
            handle.Free();

            return theStructure;
        }

        #endregion Public Methods

        #region Properties

        
        public bool Is32BitHeader
        {
            get
            {
                UInt16 IMAGE_FILE_32BIT_MACHINE = 0x0100;
                return (IMAGE_FILE_32BIT_MACHINE & FileHeader.Characteristics) == IMAGE_FILE_32BIT_MACHINE;
            }
        }

       
        public IMAGE_FILE_HEADER FileHeader
        {
            get
            {
                return fileHeader;
            }
        }

        
        public IMAGE_OPTIONAL_HEADER32 OptionalHeader32
        {
            get
            {
                return optionalHeader32;
            }
        }

        
        public IMAGE_OPTIONAL_HEADER64 OptionalHeader64
        {
            get
            {
                return optionalHeader64;
            }
        }

        public IMAGE_SECTION_HEADER[] ImageSectionHeaders
        {
            get
            {
                return imageSectionHeaders;
            }
        }

       
        public DateTime TimeStamp
        {
            get
            {
              
                DateTime returnValue = new DateTime(1970, 1, 1, 0, 0, 0);                
                returnValue = returnValue.AddSeconds(fileHeader.TimeDateStamp);
               
                returnValue += TimeZone.CurrentTimeZone.GetUtcOffset(returnValue);

                return returnValue;
            }
        }

        #endregion Properties
    }
}