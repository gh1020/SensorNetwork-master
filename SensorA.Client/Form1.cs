using SensorNetwork.Protocol;
using SensorNetwork.Protocol.Packet.PQRY;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace SensorA.Client
{
    public partial class Form1 : Form
    {
        private Random rnd = new Random();
        TcpClient client;
        private string uid = "32140900000000000000000000000000";
        public Form1()
        {
            InitializeComponent();
            txtId.Text = uid;

            var packet = new TPKGHead()
            {
                Term_Code = uid,
                Data = new SensorNetwork.Protocol.Packet.PQRY.SensPacketData() { Equip_Id = "Equip_Id", Temperature = 200, Humidity = 300 },

            };
            packet.SetNextSessionId();

            var b = packet.ToBytes();
            var r = string.Join(Environment.NewLine, BitConverter.ToString(b).Split('-')
                .Select((o, i) => new { o, i = i / 16 })
                .GroupBy(o => o.i).Select(o => string.Join(" ", o.Select(v => v.o))));



            var x = packet.Parse(b);
            int b2 = x ? 1 : 0;
        }

        private void timer1_Tick(object sender, EventArgs e)
        {
            var value = Math.Round((rnd.NextDouble() * 10 + 20), 2);
            numTemp.Value = (decimal)value;

            try
            {
                if (bw != null)
                {
                    var packet = new TPKGHead()
                    {
                        Term_Code = uid,
                        Data = new SensorNetwork.Protocol.Packet.PQRY.SensPacketData()
                        {
                            Temperature = (ushort)(value * 10),
                            Humidity = 971,
                        },
                    };
                    packet.ToBytes(bw);
                }
            }
            catch { }
        }

        private void chkRandom_CheckedChanged(object sender, EventArgs e)
        {
            timer1_Tick(this, e);
            timer1.Enabled = chkRandom.Checked;
            numTemp.Enabled = !chkRandom.Checked;
        }
        Thread th;
        private void btnConnect_Click(object sender, EventArgs e)
        {
            if (client == null)
            {
                try
                {
                    client = new TcpClient(txtIPAddress.Text, (int)numPort.Value);
                    th = new Thread(Working);
                    th.Start();

                    var packet = new TPKGHead()
                    {
                        Term_Code = uid,
                        Data = new SensorNetwork.Protocol.Packet.LINK.LoginPacketData() { Cloud_Code = uid, Username = "myname", Password = "mypassword" },
                    };
                    packet.SetNextSessionId();

                    var bytes = packet.ToBytes();
                    client.GetStream().Write(bytes, 0, bytes.Length);

                    packet = new TPKGHead()
                    {
                        Term_Code = uid,
                        Data = new SensorNetwork.Protocol.Packet.PQRY.SensPacketData() { Temperature = (ushort)(numTemp.Value / 10) },
                    };
                    packet.SetNextSessionId();

                    bytes = packet.ToBytes();
                    client.GetStream().Write(bytes, 0, bytes.Length);
                    txtIPAddress.Enabled = numPort.Enabled = false;
                    btnConnect.Text = "断开(&D)";
                }
                catch (Exception ex)
                {
                    client = null;
                    MessageBox.Show(ex.Message, "错误", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
                finally
                {

                }
            }
            else
            {
                isWorking = false;
                txtIPAddress.Enabled = numPort.Enabled = true;
                btnConnect.Text = "连接(&C)";
            }
        }

        private bool isWorking = false;

        BinaryWriter bw;
        public void Working()
        {
            Func<double> getTemp = () => (double)numTemp.Value;
            Func<double, double> setTemp = (v) =>
               {
                   if (v != 0)
                   {
                       if (chkRandom.Checked)
                           chkRandom.Checked = false;
                       numTemp.Value = (decimal)v;
                   }
                   else
                   {
                       if (chkRandom.Checked == false)
                           chkRandom.Checked = true;
                   }
                   return v;
               };
            isWorking = true;
            TPKGHead packet;
            try
            {
                var ns = client.GetStream();
                BinaryReader br = new BinaryReader(ns);
                bw = new BinaryWriter(ns);

                while (isWorking)
                {
                    if (client.Available == 0)
                    {
                        Thread.Sleep(100);
                        continue;
                    }
                    packet = new TPKGHead();
                    if (packet.Parse(br))
                    {
                        if ((packet.Data as SensPacketData)?.Temperature > 0)
                        {
                            try
                            {
                                var value = (packet.Data as SensPacketData)?.Temperature / 10.0;
                                this.Invoke(setTemp, value);
                            }
                            catch { }
                        }
                        var tmp = (double)this.Invoke(getTemp);
                        packet.Data = new SensPacketData() { Temperature = (ushort)(tmp * 10) };
                        packet.ToBytes(bw);
                    }

                };
            }
            catch { }

            try
            {
                client.Close();
                client = null;
                bw = null;
            }
            catch { }
        }

        private void txtId_TextChanged(object sender, EventArgs e)
        {
            uid = txtId.Text;
        }
    }

}
