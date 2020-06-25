using System;
using System.ComponentModel;
using System.Windows.Forms;
using System.IO;

namespace SEGY
{
    public partial class Form1 : Form
    {
        int TraceN;
        FileEater SF;// глобальные переменные

        public Form1()
        {
            InitializeComponent();
        }

        private void tableLayoutPanel1_Click_1(object sender, EventArgs e)
        {
            openFileDialog1.ShowDialog();
        }

        private void openFileDialog1_FileOk(object sender, CancelEventArgs e)
        {
            FileStream FS = new FileStream(openFileDialog1.FileName, FileMode.Open, FileAccess.Read);
            SF = new FileEater(FS);
            textBox1.Text = SF.NTraces.ToString();
            textBox2.Text = SF.LTraces.ToString();
            textBox3.Text = SF.STraces.ToString();
        }

        private void zedGraphControl1_Click(object sender, EventArgs e)
        {
            try
            {
                TraceN = Convert.ToInt16(textBox4.Text);

                SF.TraceGraph(zedGraphControl1, TraceN);
            }
            catch
            {
                MessageBox.Show("Ошибка в открытии файла", "ОШИБКА", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }
    }
}
