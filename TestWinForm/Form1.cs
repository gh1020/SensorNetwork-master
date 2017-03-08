using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace TestWinForm
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
        }

        private void button2_Click(object sender, EventArgs e)
        {
            Application.Exit();
        }
        /// <summary>
        /// 16进制-》字符串
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void button1_Click(object sender, EventArgs e)
        {
            string strHex = "6E 00 8F D7 1D EC 00 00 00 00 00 00 00 00 33 32 31 34 30 39 30 30 30 30 30 30 02 00 01 00 20 00 00 00 00 00 33 32 31 34 6D 79 6E 61 6D 65 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 6D 79 70 61 73 73 77 6F 72 64 00 00 00 00 00 00 6D 79 41 70 70 55 73 65 72 49 44 00 00 00 00 00 01 00 00 00";
          //  string strHex = txt16Content.Text;
          string strResult= HexToStr(strHex);
            textBox1.Text =  strResult ;
            //Console.WriteLine(HexToStr(strHex));
        }
        /// <summary>
        /// 字符串转16进制
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void button3_Click(object sender, EventArgs e)
        {

        }
        public static string StrToHex(string mStr) //返回处理后的十六进制字符串
        {
            return BitConverter.ToString(
            ASCIIEncoding.Default.GetBytes(mStr)).Replace("-", " ");
        } /* StrToHex */
        public static string HexToStr(string mHex) // 返回十六进制代表的字符串
        {
            mHex = mHex.Replace(" ", "");
            if (mHex.Length <= 0) return "";
            byte[] vBytes = new byte[mHex.Length / 2];
            for (int i = 0; i < mHex.Length; i += 2)
                if (!byte.TryParse(mHex.Substring(i, 2), NumberStyles.HexNumber, null, out vBytes[i / 2]))
                    vBytes[i / 2] = 0;
            return ASCIIEncoding.Default.GetString(vBytes);
        } /* HexToStr */
    }
}
