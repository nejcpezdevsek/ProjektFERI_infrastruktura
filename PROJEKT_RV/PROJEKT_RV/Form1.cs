using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace PROJEKT_RV
{
    public partial class Form1 : Form
    {
        List<int> testniPrimer;
        int[] razlike2 = new int[] { -2, -1, 1, 2 };
        int[] razlike3 = new int[] { -6, -5, -4, -3, 3, 4, 5, 6 };
        int[] razlike4 = new int[] { -14, -13, -12, -11, -10, -9, -8, -7, 7, 8, 9, 10, 11, 12, 13, 14 };
        int[] razlike5 = new int[] { -30, -29, -28, -27, -26, -25, -24, -23, -22, -21, -20, -19, -18, -17, -16, -15, 15, 16, 17, 18, 19, 20, 21, 22, 23, 24, 25, 26, 27, 28, 29, 30 };
        List<int> list;
        BinaryWriter binaryWriter;
        BinaryReader binaryReader;
        bool[] bitBuffer = new bool[8];
        short bitBuffCounter = 0;
        int bitIndex;
        int indx = -1;
        BitArray bitArray;
        List<int> prebranList;
        public Form1()
        {
            InitializeComponent();
        }

        private void btnOdpriDatoteko_Click(object sender, EventArgs e)
        {
            OpenFileDialog theDialog = new OpenFileDialog();
            theDialog.Title = "Odprite .txt datoteko";
            testniPrimer = new List<int>();
            using (OpenFileDialog ofd = new OpenFileDialog())
            {
                if (ofd.ShowDialog() == DialogResult.OK)
                {
                    string[] lines = File.ReadAllLines(ofd.FileName);
                    for (int i = 0; i < lines.Length; i++)
                    {
                        testniPrimer.Add(Convert.ToInt32(lines[i]));
                    }
                }
            }
        }

        private void btnOpenBinFile_Click(object sender, EventArgs e)
        {
            using (OpenFileDialog ofd = new OpenFileDialog())
            {
                ofd.Title = "Odpri binarno sliko";
                if (ofd.ShowDialog() == DialogResult.OK)
                {
                    binaryReader = new BinaryReader(File.Open(ofd.FileName, FileMode.Open));
                }
            }
            prebranList = new List<int>();
            beriBitov(10000000);
        }

        private void btnCompress_Click(object sender, EventArgs e)
        {
            binaryWriter = new BinaryWriter(File.Open("kompresiranBin.bin", FileMode.Create));
            list = new List<int>();
            //Izracunamo razlike
            int[] poljeRazlik = new int[testniPrimer.Count];
            poljeRazlik[0] = testniPrimer[0];
            for (int i = 1; i < testniPrimer.Count; i++)
            {
                poljeRazlik[i] = testniPrimer[i] - testniPrimer[i - 1];
            }

            decToBinary(poljeRazlik[0], 8);
            for (int i = 1; i < poljeRazlik.Length;)
            {
                //Gre za razliko ko je manjša od 30
                if (poljeRazlik[i] != 0 && poljeRazlik[i] >= -30 && poljeRazlik[i] <= 30)
                {
                    decToBinary(0, 2);
                    if (poljeRazlik[i] >= -2 && poljeRazlik[i] <= 2)
                    {
                        for (int j = 0; j < 4; j++)
                        {
                            if (poljeRazlik[i] == razlike2[j])
                            {
                                decToBinary(0, 2);
                                decToBinary(j, 2);
                                i++;
                                break;
                            }
                        }
                    }
                    else if (poljeRazlik[i] >= -6 && poljeRazlik[i] <= 6)
                    {
                        for (int j = 0; j < 8; j++)
                        {
                            if (poljeRazlik[i++] == razlike3[j])
                            {
                                decToBinary(1, 2);
                                decToBinary(j, 3);
                                break;
                            }
                        }
                    }
                    else if (poljeRazlik[i] >= -14 && poljeRazlik[i] <= 14)
                    {
                        for (int j = 0; j < 16; j++)
                        {
                            if (poljeRazlik[i] == razlike4[j])
                            {
                                decToBinary(2, 2);
                                decToBinary(j, 4);
                                i++;
                                break;
                            }
                        }
                    }
                    else if (poljeRazlik[i] >= -30 && poljeRazlik[i] <= 30)
                    {
                        for (int j = 0; j < 32; j++)
                        {
                            if (poljeRazlik[i] == razlike5[j])
                            {
                                decToBinary(3, 2);
                                decToBinary(j, 5);
                                i++;
                                break;
                            }
                        }
                    }

                }
                else if (poljeRazlik[i] == 0)
                {
                    decToBinary(1, 2);
                    int counterPonovitev = 0;
                    int x = i;
                    while (poljeRazlik[x + 1] == 0 && counterPonovitev < 8)
                    {
                        counterPonovitev++;
                        x++;
                        if (x + 1 == poljeRazlik.Length) break;
                    }
                    decToBinary(counterPonovitev, 3);
                    i = x + 1;
                }
                else
                {
                    decToBinary(2, 2);
                    if (poljeRazlik[i] < 0)
                    {
                        decToBinary(1, 1);
                    }
                    else
                    {
                        decToBinary(0, 1);
                    }

                    decToBinary(Math.Abs(poljeRazlik[i]), 8);
                    i++;
                }
            }
            decToBinary(3, 2);
            decToBinary(0, 20);


            for (int i = 0, bitBuffCounter = 0; i < list.Count; i++)
            {
                richTextBox1.Text += list[i].ToString();
                bitBuffer[bitBuffCounter++] = (list[i] == 1);
                if (bitBuffCounter == 8)
                {
                    binaryWriter.Write(ConvertToByte(bitBuffer));
                    bitBuffCounter = 0;
                    bitBuffer = new bool[8];
                }
            }
            binaryWriter.Close();
            //writingToFile();
        }

        private void btnDecompress_Click(object sender, EventArgs e)
        {
            //Prvo prvih 8
            List<int> current = new List<int>();
            List<int> result = new List<int>();
            bitBuffCounter = 0;
            int counterBit = 0;
            for (int i = 0; i < 8; i++)
            {
                current.Insert(0, prebranList[i]);
                counterBit++;
            }
            result.Add(BinaryToDec(current));
            current.Clear();
            for (int i = 8; i < prebranList.Count; i++)
            {
                if (prebranList[i] == 0 && prebranList[i + 1] == 0)
                {
                    i += 2;
                    if (prebranList[i] == 0 && prebranList[i + 1] == 0)
                    {
                        i += 2;
                        for (int j = i; j < i + 2; j++)
                        {
                            current.Insert(0, prebranList[j]);
                        }
                        int currentResult = BinaryToDec(current);
                        current.Clear();
                        result.Add(razlike2[currentResult]);
                        i++;
                    }
                    else if (prebranList[i] == 0 && prebranList[i + 1] == 1)
                    {
                        i += 2;
                        for (int j = i; j < i + 3; j++)
                        {
                            current.Insert(0, prebranList[j]);
                        }
                        int currentResult = BinaryToDec(current);
                        current.Clear();
                        result.Add(razlike3[currentResult]);
                        i += 2;
                    }
                    else if (prebranList[i] == 1 && prebranList[i + 1] == 0)
                    {
                        i += 2;
                        for (int j = i; j < i + 4; j++)
                        {
                            current.Insert(0, prebranList[j]);
                        }
                        int currentResult = BinaryToDec(current);
                        current.Clear();
                        result.Add(razlike4[currentResult]);
                        i += 3;
                    }
                    else if (prebranList[i] == 1 && prebranList[i + 1] == 1)
                    {
                        i += 2;
                        for (int j = i; j < i + 5; j++)
                        {
                            current.Insert(0, prebranList[j]);
                        }
                        int currentResult = BinaryToDec(current);
                        current.Clear();
                        result.Add(razlike5[currentResult]);
                        i += 4;
                    }

                }
                else if (prebranList[i] == 0 && prebranList[i + 1] == 1)
                {
                    i += 2;
                    for (int j = i; j < i + 3; j++)
                    {
                        current.Insert(0, prebranList[j]);
                    }
                    int currentResult = BinaryToDec(current);
                    current.Clear();
                    for (int j = 0; j <= currentResult; j++)
                    {
                        result.Add(0);
                    }
                    i += 2;
                }
                else if (prebranList[i] == 1 && prebranList[i + 1] == 0)
                {
                    i += 2;
                    // 1 je negativno stevilo
                    if (prebranList[i] == 1)
                    {
                        i++;
                        for (int j = i; j < i + 8; j++)
                        {
                            current.Insert(0, prebranList[j]);

                        }
                        int currentResult = BinaryToDec(current);
                        current.Clear();
                        currentResult = currentResult * -1;
                        i += 7;
                        result.Add(currentResult);
                    }
                    else
                    {
                        i++;
                        for (int j = i; j < i + 8; j++)
                        {
                            current.Insert(0, prebranList[j]);
                        }
                        int currentResult = BinaryToDec(current);
                        current.Clear();
                        i += 7;
                        result.Add(currentResult);

                    }
                }
                else if (prebranList[i] == 1 && prebranList[i + 1] == 1)
                {
                    break;
                }
            }
            richTextBox1.Text += result[0].ToString();
            richTextBox1.Text += " ";
            for (int i = 1; i < result.Count; i++)
            {
                result[i] = result[i] + result[i - 1];
                richTextBox1.Text += result[i].ToString();
                richTextBox1.Text += " ";
            }
        }

        public void decToBinary(int n, int stBitov)
        {
            int counter = 0;
            // array to store binary number 
            int[] binaryNum = new int[32];
            List<int> currentList = new List<int>();
            // counter for binary array 
            int i = 0;
            while (n > 0)
            {
                // storing remainder in 
                // binary array 
                binaryNum[i] = n % 2;
                n = n / 2;
                i++;
            }

            // printing binary array 
            // in reverse order 
            for (int j = i - 1; j >= 0; j--)
            {
                counter++;
                currentList.Add(binaryNum[j]);
            }
            while (counter != stBitov)
            {
                currentList.Insert(0, 0);
                counter++;
            }
            list.AddRange(currentList);
        }

        private void beriBitov(int stBitov)
        {
            int vrednost;
            bitIndex = stBitov - 1;
            for (int i = 0; i < stBitov; i++)
            {
                if (binaryReader.BaseStream.Position >= binaryReader.BaseStream.Length) { break; } //konec fajla
                vrednost = (ReadBoolean()) ? 1 : 0;
                prebranList.Add(vrednost);
                richTextBox2.Text += vrednost.ToString();
                bitIndex--;
            }
        }

        public bool ReadBoolean()
        {
            if(indx < 0)
            {
                bitArray = new BitArray(new byte[] { binaryReader.ReadByte() });
                bitArray.CopyTo(bitBuffer, 0);
                bitArray = null;
                this.bitBuffCounter = 0;
                indx = 7;
            }
            bitBuffCounter++;
            return bitBuffer[indx--];
        }
        private void zapisiBit(bool bit)
        {
            bitBuffer[bitBuffCounter++] = bit;
            if (bitBuffCounter == 8)
            {
                binaryWriter.Write(ConvertToByte(bitBuffer));
                bitBuffCounter = 0;
                bitBuffer = new bool[8];
            }

        }

        private byte ConvertToByte(bool[] arrBits) //pretvorba bitov(bool) v byte
        {
            byte result = 0;
            for (int i = 0; i < arrBits.Length; i++)
            {
                if (arrBits[i])
                    result |= (byte)(1 << 7 - i);
            }
            return result;
        }

        private int BinaryToDec(List<int> currentList)
        {
            int sum = 0;
            //int sum = 1 + (int)Math.Pow(2, currentList.GetRange(1, currentList.Count-1).Sum());
            for (int i = 0; i < currentList.Count; i++)
            {
                if (currentList[i] != 0)
                {
                    if (i != 0) 
                    {
                        sum += (int)Math.Pow(2, i);
                    }
                    else
                    {
                        sum += 1;
                    }
                }

            }
            return sum;
        }
    }
}
