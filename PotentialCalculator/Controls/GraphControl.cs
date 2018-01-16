using PotentialCalculator.Helpers;
using PotentialCalculator.Models;
using System;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Windows.Forms;
using ZedGraph;

namespace PotentialCalculator.Controls {
    public partial class GraphControl : DevExpress.XtraEditors.XtraUserControl {
        Criteria criteria;
        public GraphControl() {
            InitializeComponent();
            zedGraphControl.GraphPane.XAxis.Title.IsVisible = false;
            zedGraphControl.GraphPane.YAxis.Title.IsVisible = false;
        }

        public void SetNotificationControl(Control control) {
            layoutControlItem2.BeginInit();
            Control tempC = layoutControlItem2.Control;
            layoutControlItem2.Control = control;
            tempC.Parent = null;
            layoutControlItem2.EndInit();
        }

        public GraphControl(string title, Criteria criteria) : this() {
            this.SetTitle(title);
            this.criteria = criteria;
        }
        public void SetTitle(string title) {
            zedGraphControl.GraphPane.Title.Text = title;
        }
        public static GraphControl CreateGraphControlForCriteria(Criteria criteria) {
            var graph = new GraphControl(criteria.Name, criteria);
            var sourceData = MyMath.GetGraphDataForNormalDistribution(criteria.SourceMean, criteria.SourceSigma);
            graph.AddCurve(sourceData, "Источник", Color.Red);
            var disturberData = MyMath.GetGraphDataForNormalDistribution(criteria.DisturberMean, criteria.DisturberSigma);
            graph.AddCurve(disturberData, "Помеха", Color.Blue);
            graph.AddVLine(criteria.Threshold, "Порог", Color.Green, MyMath.GetMaxY(sourceData, disturberData));
            return graph;
        }
        public void AddCurve(MyPoint[] list, string name, Color color) {
            GraphPane pane = zedGraphControl.GraphPane;
            LineItem curve = pane.AddCurve(
                name,
                list.Select(p => Convert.ToDouble(p.X)).ToArray(),
                list.Select(p => Convert.ToDouble(p.Y)).ToArray(),
                color,
                SymbolType.None
            );
            zedGraphControl.AxisChange();
        }

        public void AddVLine(double x, string name, Color color, double maxY) {
            GraphPane pane = zedGraphControl.GraphPane;
            LineItem line = new LineItem(
                name, 
                new[] { x, x },
                new[] { pane.YAxis.Scale.Min, maxY },
                color,
                SymbolType.None
            );
            line.Line.Style = System.Drawing.Drawing2D.DashStyle.Solid;
            line.Line.Width = 1f;
            pane.CurveList.Add(line);
            zedGraphControl.AxisChange();
        }
        public Tuple<double, double, double> CalcPs() {
            var result = new Tuple<double, double, double>(MyMath.CalcDetectionP(criteria), MyMath.CalcErrorP(criteria), MyMath.CalcMissP(criteria));
            SetPs(result.Item1, result.Item2, result.Item3);
            return result;
        }
        public void SetPs(double d, double e, double miss) {
            pControl1.setPs(d, e, miss);
        }
        public void hidePs() {
            layoutControlItem2.Visibility = DevExpress.XtraLayout.Utils.LayoutVisibility.Never;
        }
        public void AddPoint(double sourceMean, double disturberMean, double threshold) {
            GraphPane pane = zedGraphControl.GraphPane;
            pane.AddCurve("Источник", new double[] { 1 }, new double[] { sourceMean }, Color.Red, SymbolType.Circle);
            pane.AddCurve("Помеха", new double[] { 1 }, new double[] { disturberMean }, Color.Blue, SymbolType.Circle);
            pane.AddCurve("Порог", new double[] { 1 }, new double[] { threshold }, Color.Green, SymbolType.Circle);
            zedGraphControl.AxisChange();
        }
    }
}
