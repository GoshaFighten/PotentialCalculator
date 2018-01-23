using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Linq;
using System.Windows.Forms;
using DevExpress.XtraEditors;
using PotentialCalculator.Models;
using PotentialCalculator.Helpers;
using DevExpress.XtraLayout;

namespace PotentialCalculator {
    public partial class TForm : DevExpress.XtraEditors.XtraForm {
        TextEdit txt = new TextEdit();
        public TForm() {
            InitializeComponent();
        }
        public void Render(Criteria criteria) {
            graphControl1.SetNotificationControl(txt);
            var list = new List<MyPoint>();
            var min = criteria.SourceMean - 3 * criteria.SourceSigma;
            var max = criteria.SourceMean + 3 * criteria.SourceSigma;
            var step = (max - min) / 1000 > 0.1 ? 0.1 : (max - min) / 1000;
            for (double i = min; i < max; i += step) {
                list.Add(new MyPoint(i, MyMath.CalcErrorP(criteria, i) + MyMath.CalcMissP(criteria, i)));
            }
            var data = list.ToArray();
            txt.EditValue = list.Aggregate((p1, p2) => p1.Y < p2.Y ? p1 : p2).X;
            graphControl1.AddCurve(data, "Ошибки", Color.Red);
        }
        public void RenderCC(bool newLogic) {
            var ctrl = new LayoutControl();
            ctrl.Root.OptionsTableLayoutGroup.ColumnDefinitions.Add(new ColumnDefinition() { SizeType = SizeType.Percent, Width = 100 });
            var index = ctrl.Root.OptionsTableLayoutGroup.RowDefinitions.Add(new DevExpress.XtraLayout.RowDefinition());
            var item = ctrl.AddItem("Min", txt);
            item.OptionsTableLayoutItem.RowIndex = index;
            var list = new List<MyPoint>();
            var result = MyMath.CalcCCDensity(Project.ProjectInstance.Criterias.ToList(), Project.ProjectInstance.CCThreshold, newLogic);
            var min = Math.Min(result.Item1.Min(p => p.X), result.Item2.Min(p => p.X));
            var max = Math.Max(result.Item1.Max(p => p.X), result.Item2.Max(p => p.X));
            var step = (max - min) / 1000 > 0.1 ? 0.1 : (max - min) / 1000;
            for (double i = min; i < max; i += step) {
                var current = MyMath.CalcCCDensity(Project.ProjectInstance.Criterias.ToList(), i, newLogic);
                list.Add(new MyPoint(i, current.Item3[CCDecision.FalseAlarm] + current.Item3[CCDecision.Miss]));
            }
            var data = list.ToArray();
            var btn = new SimpleButton();
            btn.Text = "Фурье";
            btn.Click += (s, e) => {
                var form = new TForm();
                form.RenderFourier(data);
                form.ShowDialog();
            };
            index = ctrl.Root.OptionsTableLayoutGroup.RowDefinitions.Add(new DevExpress.XtraLayout.RowDefinition());
            item = ctrl.AddItem("Фурье", btn);
            item.OptionsTableLayoutItem.RowIndex = index;
            btn = new SimpleButton();
            btn.Text = "Фильтр";
            btn.Click += (s, e) => {
                var form = new TForm();
                form.RenderFilter(data);
                form.ShowDialog();
            };
            index = ctrl.Root.OptionsTableLayoutGroup.RowDefinitions.Add(new DevExpress.XtraLayout.RowDefinition());
            item = ctrl.AddItem("Фурье", btn);
            item.OptionsTableLayoutItem.RowIndex = index;
            graphControl1.SetNotificationControl(ctrl);
            txt.EditValue = list.Aggregate((p1, p2) => p1.Y < p2.Y ? p1 : p2).X;
            graphControl1.AddCurve(data, "Ошибки", Color.Red);
        }

        private void RenderFilter(MyPoint[] data) {
            var source = data.ToList();
            //var size = data.Length % 2 == 1 ? data.Length : data.Length + 1;
            var size = 33;
            if (size % 2 != 1) {
                throw new Exception("WTF?");
            }
            for (int i = 0; i <= size / 2; i++) {
                source.Insert(0, data[0]);
            }
            for (int i = 0; i <= size / 2; i++) {
                source.Add(source[data.Length - 1]);
            }
            var result = new List<MyPoint>();
            for (int i = 0; i < data.Length; i++) {
                var mean = 0.0;
                for (int j = -size / 2; j <= size / 2; j++) {
                    mean += source[i + j + size / 2].Y;
                }
                result.Add(new MyPoint() { X = source[i].X, Y = mean / size });
            }
            var ctrl = new LayoutControl();
            ctrl.Root.OptionsTableLayoutGroup.ColumnDefinitions.Add(new ColumnDefinition() { SizeType = SizeType.Percent, Width = 100 });
            var index = ctrl.Root.OptionsTableLayoutGroup.RowDefinitions.Add(new DevExpress.XtraLayout.RowDefinition());
            var item = ctrl.AddItem("Min", txt);
            item.OptionsTableLayoutItem.RowIndex = index;
            var btn = new SimpleButton();
            btn.Text = "Фурье";
            btn.Click += (s, e) => {
                var form = new TForm();
                form.RenderFourier(result.ToArray());
                form.ShowDialog();
            };
            index = ctrl.Root.OptionsTableLayoutGroup.RowDefinitions.Add(new DevExpress.XtraLayout.RowDefinition());
            item = ctrl.AddItem("Фурье", btn);
            item.OptionsTableLayoutItem.RowIndex = index;
            graphControl1.SetNotificationControl(ctrl);
            txt.EditValue = result.Aggregate((p1, p2) => p1.Y < p2.Y ? p1 : p2).X;
            graphControl1.AddCurve(result.ToArray(), "Отфильтрованно", Color.Red);
        }

        private void RenderFourier(MyPoint[] data) {
            graphControl1.hidePs();
            graphControl1.AddCurve(MyMath.FourierTransform(data), "Фурье", Color.Red);
        }
    }
}