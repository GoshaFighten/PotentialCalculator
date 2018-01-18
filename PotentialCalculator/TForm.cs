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

namespace PotentialCalculator {
    public partial class TForm : DevExpress.XtraEditors.XtraForm {
        TextEdit txt = new TextEdit();
        public TForm() {
            InitializeComponent();

            graphControl1.SetNotificationControl(txt);
        }
        public void Render(Criteria criteria) {
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
    }
}