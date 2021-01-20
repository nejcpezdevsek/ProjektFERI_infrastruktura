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
        List<int> buffer;
        int[] r1 = new int[] { -2, -1, 1, 2 };
        int[] r2 = new int[] { -6, -5, -4, -3, 3, 4, 5, 6 };
        int[] r3 = new int[] { -14, -13, -12, -11, -10, -9, -8, -7, 7, 8, 9, 10, 11, 12, 13, 14 };
        int[] r4 = new int[] { -30, -29, -28, -27, -26, -25, -24, -23, -22, -21, -20, -19, -18, -17, -16, -15, 15, 16, 17, 18, 19, 20, 21, 22, 23, 24, 25, 26, 27, 28, 29, 30 };
        List<int> temp;
        BinaryReader binaryReader;
        bool[] bufferOfBits = new bool[8];
        int bitIndex;
        int polozaj = -1;
        BitArray bitArray;
        List<int> binaryList;
        public Form1()
        {
            InitializeComponent();
        }

        private void btnOdpriDatoteko_Click(object sender, EventArgs e)
        {
            OpenFileDialog theDialog = new OpenFileDialog();
            theDialog.Title = "Odprite .txt datoteko";
            buffer = new List<int>();
            using (OpenFileDialog ofd = new OpenFileDialog())
            {
                if (ofd.ShowDialog() == DialogResult.OK)
                {
                    string[] lines = File.ReadAllLines(ofd.FileName);
                    for (int i = 0; i < lines.Length; i++)
                    {
                        buffer.Add(Convert.ToInt32(lines[i]));
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
            binaryList = new List<int>();
            beriBitov(10000);
            binaryReader.Close();
        }

        bool TestRange(int numberToCheck, int bottom, int top)
        {
            return (numberToCheck >= bottom && numberToCheck <= top);
        }
        private void btnCompress_Click(object sender, EventArgs e)
        {
            temp = new List<int>();
            //Izracunamo razlike
            int[] razlike = new int[buffer.Count];
            razlike[0] = buffer[0];
            for (int i = 1; i < buffer.Count; i++)
            {
                razlike[i] = buffer[i] - buffer[i - 1];
            }

            //zapišemo prvo vrednost
            writeBinary(razlike[0], 8);
            for (int i = 1; i < razlike.Length;)
            {
                if (razlike[i] == 0)
                {
                    writeBinary(1, 2);
                    int stNul = 0;
                    int x = i;
                    while (razlike[x + 1] == 0)
                    {
                        if (stNul++ < 8)
                        {
                            if (++x + 1 == razlike.Length) break;
                        }
                    }
                    writeBinary(stNul, 3);
                    i = x + 1;
                }

                //Gre za razliko ko je manjša od 30
                else if (TestRange(razlike[i], -30, 30))
                {
                    writeBinary(0, 2);
                    if (TestRange(razlike[i], -2, 2))
                    {
                        if(r1.Contains(razlike[i]))
                            writeBinary(0, 2);
                            writeBinary(Array.IndexOf(r1, razlike[i++]), 2);
                    }
                    else if (TestRange(razlike[i], -6, 6))
                    {
                        if (r2.Contains(razlike[i]))
                            writeBinary(1, 2);
                            writeBinary(Array.IndexOf(r2, razlike[i++]), 3);
                    }
                    else if (TestRange(razlike[i], -14, 14))
                    {
                        if (r3.Contains(razlike[i]))
                            writeBinary(2, 2);
                            writeBinary(Array.IndexOf(r3, razlike[i++]), 4);
                    }
                    else if (TestRange(razlike[i], -30, 30))
                    {
                        if (r4.Contains(razlike[i]))
                            writeBinary(3, 2);
                            writeBinary(Array.IndexOf(r4, razlike[i++]), 5);
                    }

                }
                //v primeru če je večja od 30 in manjša od -30
                else
                {
                    writeBinary(2, 2);
                    //če je negativna, zapiši 1 čene 0
                    if (razlike[i] < 0)
                        writeBinary(1, 1);
                    else
                        writeBinary(0, 1);

                    writeBinary(Math.Abs(razlike[i++]), 8);
                }
            }

            writeBinary(3, 2);
            writeBinary(0, 16);

            richTextBox1.Text = "";
            using (BinaryWriter binaryWriter = new BinaryWriter(File.Open("compressed.bin", FileMode.Create)))
            {
                for (int i = 0, bufferOfBitsCounter = 0; i < temp.Count; i++)
                {
                    richTextBox1.Text += temp[i].ToString();
                    bufferOfBits[bufferOfBitsCounter++] = (temp[i] == 1);
                    if (bufferOfBitsCounter == 8)
                    {
                        binaryWriter.Write(ConvertToByte(bufferOfBits));
                        bufferOfBitsCounter = 0;
                        bufferOfBits = new bool[8];
                    }
                }

                binaryWriter.Close();
            }
        }

        private void btnDecompress_Click(object sender, EventArgs e)
        {
            List<int> temp = new List<int>();
            List<int> vrni = new List<int>();
            List<int> neki = new List<int>();

            //neki = binaryList.GetRange(0, 7);
            //binaryList.Reverse();

            //najprej preberem prvih 8
            temp.AddRange(binaryList.GetRange(0, 8));
            temp.Reverse();
            vrni.Add(ConvertToDecimal(temp));
            temp.Clear();
            for (int i = 8; i < binaryList.Count; i++)
            {
                if (binaryList[i] == 0 && binaryList[i + 1] == 0)
                {
                    i += 2;
                    if (binaryList[i] == 0 && binaryList[i + 1] == 0)
                    {
                        i += 2;
                        temp.AddRange(binaryList.Skip(i++).Take(2));
                        temp.Reverse();
                        vrni.Add(r1[ConvertToDecimal(temp)]);
                        temp.Clear();
                    }
                    else if (binaryList[i] == 0 && binaryList[i + 1] == 1)
                    {
                        i += 2;
                        temp.AddRange(binaryList.Skip(i+=2).Take(3));
                        temp.Reverse();
                        vrni.Add(r2[ConvertToDecimal(temp)]);
                        temp.Clear();
                    }
                    else if (binaryList[i] == 1 && binaryList[i + 1] == 0)
                    {
                        i += 2;
                        temp.AddRange(binaryList.Skip(i += 3).Take(4));
                        temp.Reverse();
                        vrni.Add(r2[ConvertToDecimal(temp)]);
                        temp.Clear();
                    }
                    else if (binaryList[i] == 1 && binaryList[i + 1] == 1)
                    {
                        i += 2;
                        temp.AddRange(binaryList.Skip(i += 4).Take(5));
                        temp.Reverse();
                        vrni.Add(r2[ConvertToDecimal(temp)]);
                        temp.Clear();
                    }

                }
                else if (binaryList[i] == 0 && binaryList[i + 1] == 1)
                {
                    i += 2;
                    temp.AddRange(binaryList.Skip(i).Take(3));
                    temp.Reverse();

                    for (int j = 0; j <= ConvertToDecimal(temp); j++)
                    {
                        vrni.Add(0);
                    }
                    temp.Clear();
                    i += 2;
                }
                else if (binaryList[i] == 1 && binaryList[i + 1] == 0)
                {
                    i += 2;
                    if (binaryList[i++] == 1)
                    {
                        temp.AddRange(binaryList.Skip(i).Take(8));
                        temp.Reverse();
                        vrni.Add(ConvertToDecimal(temp) * -1);
                    }
                    else
                    {
                        temp.AddRange(binaryList.Skip(i).Take(8));
                        temp.Reverse();
                        vrni.Add(ConvertToDecimal(temp));
                    }
                    temp.Clear();
                    i += 7;
                }

                //ko dosežemo konec
                else if (binaryList[i] == 1 && binaryList[i + 1] == 1)
                {
                    break;
                }
            }
            richTextBox1.Text = vrni[0].ToString();
            richTextBox1.Text += " ";
            for (int i = 1; i < vrni.Count; i++)
            {
                vrni[i] = vrni[i] + vrni[i - 1];
                richTextBox1.Text += vrni[i].ToString();
                richTextBox1.Text += " ";
            }
        }

        public void writeBinary(int n, int stBitov)
        {
            int counter = 0;
            int[] binaryNum = new int[32];
            List<int> temp = new List<int>();
            int i = 0;
            while (n > 0)
            {
                binaryNum[i] = n % 2;
                n = n / 2;
                i++;
            }
            for (int j = i - 1; j >= 0; j--)
            {
                counter++;
                temp.Add(binaryNum[j]);
            }
            while (counter != stBitov)
            {
                temp.Insert(0, 0);
                counter++;
            }
            this.temp.AddRange(temp);
        }

        private void beriBitov(int stBitov)
        {
            int vrednost;
            bitIndex = stBitov - 1;
            richTextBox2.Text = "";
            for (int i = 0; i < stBitov; i++)
            {
                if (binaryReader.BaseStream.Position >= binaryReader.BaseStream.Length) { break; }
                vrednost = (ReadBoolean()) ? 1 : 0;
                binaryList.Add(vrednost);
                richTextBox2.Text += vrednost.ToString();
                bitIndex--;
            }
        }

        public bool ReadBoolean()
        {
            if(polozaj < 0)
            {
                bitArray = new BitArray(new byte[] { binaryReader.ReadByte() });
                bitArray.CopyTo(bufferOfBits, 0);
                bitArray = null;
                polozaj = 7;
            }
            return bufferOfBits[polozaj--];
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

        private int ConvertToDecimal(List<int> temp)
        {
            int sum = 0;
            //int sum = 1 + (int)Math.Pow(2, temp.GetRange(1, currentList.Count-1).Sum());
            for (int i = 0; i < temp.Count; i++)
            {
                if (temp[i] != 0)
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
