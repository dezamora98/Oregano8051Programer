using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Oregano8051Programer.Model
{
    internal class IntelHexFile
    {
        public IntelHexFile()
        {
            CodeForMem = new List<byte>();
            StrIntelHexCode = "";
        }
        public IntelHexFile(string AddrFile, string Backfill) => Load(AddrFile, Backfill);

        public void Load(string AddrFile, string Backfill)
        {
            StreamReader HexFile;
            try
            {
                HexFile = new StreamReader(AddrFile);
            }
            catch (Exception e)
            {
                Console.WriteLine("Exception " + e.Message);
                throw;
            }

            string line = HexFile.ReadLine();
            CodeForMem = new List<byte>();
            uint addr = 0;
            while (!HexFile.EndOfStream)
            {
                if (line.StartsWith(":"))
                {
                    //Opteniendo tipo de línea
                    byte lineType = Convert.ToByte(line.Substring(7, 2), 16);
                    //Identificando tamaño de línea
                    byte lineSize = Convert.ToByte(line.Substring(1, 2), 16);
                    //Identificando dirección
                    UInt16 lineAddr = Convert.ToUInt16(line.Substring(3, 4), 16);

                    if (lineType == 0x00)
                    {
                        // Si la dirección base es menor que la actual, aplicar el relleno a la lista
                        while (addr != lineAddr)
                        {
                            CodeForMem.Add(Convert.ToByte(Backfill, 16));
                            ++addr;
                        }

                        // Cargar nuevos datos a la lista
                        for (int i = 0; i < lineSize; ++i)
                        {
                            if (i != lineSize)
                                CodeForMem.Add((byte)Convert.ToByte(line.Substring((i * 2) + 9, 2), 16));
                            ++addr;
                        }

                    }
                }
                line = HexFile.ReadLine();
                StrIntelHexCode += line + "\n";
            }
            HexFile.Close();
        }

        public List<byte> CodeForMem { get; private set; }
        public string StrIntelHexCode { get; private set; }
    }
}
