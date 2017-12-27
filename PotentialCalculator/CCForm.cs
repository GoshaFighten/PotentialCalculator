using PotentialCalculator.Helpers;
using PotentialCalculator.Models;
using System;
using System.Drawing;
using System.Linq;

namespace PotentialCalculator {
    public partial class CCForm : DevExpress.XtraEditors.XtraForm {
        public CCForm() {
            InitializeComponent();
            this.graphControl2.hidePs();
        }
        public void Calc() {
            var result = MyMath.CalcCC(Project.ProjectInstance.Criterias.ToList());
            this.graphControl1.AddVLine(Project.ProjectInstance.CCThreshold, "Порог", Color.Green, MyMath.GetMaxY(result.Item1, result.Item2));
            this.graphControl1.AddCurve(result.Item1, "КК источника", Color.Red);
            this.graphControl1.AddCurve(result.Item2, "КК помехи", Color.Blue);
            this.graphControl1.SetTitle("КК");
            double detectionP = MyMath.CalcRandomLeftProbability(result.Item1, Project.ProjectInstance.CCThreshold);
            double errorP = MyMath.CalcRandomLeftProbability(result.Item2, Project.ProjectInstance.CCThreshold);
            this.graphControl1.SetPs(detectionP, errorP, 1.0 - detectionP);
            CalcMeans(result.Item1, result.Item2, Project.ProjectInstance.CCThreshold);
        }
        void CalcMeans(MyPoint[] sourceDensity, MyPoint[] disturberDensity, double threshold) {
            var sourceMean = MyMath.CalcRandomMean(sourceDensity);
            var disturberMean = MyMath.CalcRandomMean(disturberDensity);
            this.graphControl2.AddPoint(sourceMean, disturberMean, threshold);
            this.graphControl2.SetTitle("Средние");
        }
    }
}