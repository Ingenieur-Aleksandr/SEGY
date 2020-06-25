using System;
using System.IO;
using ZedGraph;
using System.Drawing;// библиотеки базовых методов, открытия файлов и визуализации

namespace SEGY
{
    public class FileEater
    {
        public FileStream fs;
        public BinaryReader br;
        public short NTraces, LTraces, STraces;

        public FileEater (FileStream _fs)
        {
            fs = _fs;
            br = new BinaryReader(fs);
            byte[] bytearr = new byte[SEGYConstants.Two];

            fs.Seek(SEGYConstants.Seek1, SeekOrigin.Begin);
            fs.Read(bytearr, SEGYConstants.Zero, SEGYConstants.Two);
            Array.Reverse(bytearr);
            NTraces = BitConverter.ToInt16(bytearr, SEGYConstants.Zero);//получение номера трасс

            fs.Seek(SEGYConstants.Seek2, SeekOrigin.Begin);
            fs.Read(bytearr, SEGYConstants.Zero, SEGYConstants.Two);
            Array.Reverse(bytearr);
            LTraces = BitConverter.ToInt16(bytearr, SEGYConstants.Zero);//получение длины трасс

            fs.Seek(SEGYConstants.Seek3, SeekOrigin.Begin);
            fs.Read(bytearr, SEGYConstants.Zero, SEGYConstants.Two);
            Array.Reverse(bytearr);
            STraces = BitConverter.ToInt16(bytearr, SEGYConstants.Zero);//получение шага дискретизации
        }

        public void TraceGraph(ZedGraphControl ZGC, int TraceN)
        {
            TraceN = TraceN - 1;
            if (NTraces < TraceN)
            {
                throw new Exception("ошибка в открытии файла");
             }//обработка открытия неверного формата файла
            GraphPane pane = ZGC.GraphPane;
            pane.CurveList.Clear();
            PointPairList list = new PointPairList();
            int beg = SEGYConstants.HeaderTxt + SEGYConstants.HeaderDyn;
            int ntb = TraceN * (SEGYConstants.TraceHeader + LTraces * 4);
            fs.Seek(beg + ntb + SEGYConstants.TraceHeader, SeekOrigin.Begin);
            //fs.Seek((SEGYConstants.HeaderTxt + (NTraces *(LTraces *4 +SEGYConstants.TraceHeader))+ SEGYConstants.HeaderDyn), SeekOrigin.Begin);
            byte[] buf = new byte[4];
            for (int x = 1; x <= LTraces; x++)
            {
                fs.Read(buf, 0, 4);
                list.Add(x, SEGYFile.ReadWordIBM(buf));
            }
            pane.XAxis.Title.Text = "время";
            pane.YAxis.Title.Text = "амплитуда";
            Color curveColor = Color.Red;
            LineItem curve = pane.AddCurve("SEGY", list, curveColor, SymbolType.None);
            pane.XAxis.Type = AxisType.Linear;
            curve.Line.IsSmooth = true;
            pane.YAxis.MajorGrid.IsVisible = true;
            ZGC.AxisChange();
            ZGC.Invalidate();
        }//построение графика трассы
    }
}
