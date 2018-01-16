using PotentialCalculator.Controls;
using PotentialCalculator.Helpers;
using PotentialCalculator.Models;
using System;
using System.Drawing;
using System.Linq;

namespace PotentialCalculator {
    public partial class CCForm : DevExpress.XtraEditors.XtraForm {
        KControl kControl = new KControl();
        public CCForm() {
            InitializeComponent();
            this.graphControl2.SetNotificationControl(kControl);
            //this.graphControl2.hidePs();
        }
        public void Calc(bool newLogic = false) {
            kControl.Calc(MyMath.CalcKs(Project.ProjectInstance.Criterias, newLogic));
            var result = MyMath.CalcCCDensity(Project.ProjectInstance.Criterias.ToList(), Project.ProjectInstance.CCThreshold, newLogic);
            this.graphControl1.AddVLine(Project.ProjectInstance.CCThreshold, "Порог", Color.Green, MyMath.GetMaxY(result.Item1, result.Item2));
            this.graphControl1.AddCurve(result.Item1, "КК источника", Color.Red);
            this.graphControl1.AddCurve(result.Item2, "КК помехи", Color.Blue);
            this.graphControl1.SetTitle("КК");
            double detectionP = result.Item3[CCDecision.Hit];
            double errorP = result.Item3[CCDecision.FalseAlarm];
            double missP = result.Item3[CCDecision.Miss];
            this.graphControl1.SetPs(detectionP, errorP, missP);
            CalcMeans(result.Item1, result.Item2, Project.ProjectInstance.CCThreshold);
        }
        void CalcMeans(MyPoint[] sourceDensity, MyPoint[] disturberDensity, double threshold) {
            var sourceMean = MyMath.CalcRandomMean(sourceDensity);
            var disturberMean = MyMath.CalcRandomMean(disturberDensity);
            this.graphControl2.AddPoint(sourceMean, disturberMean, threshold);
            this.graphControl2.SetTitle("Средние");
        }

        private void barButtonItem1_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e) {
            var form = new CCForm();
            form.Calc(true);
            form.ShowDialog();
        }
    }
}