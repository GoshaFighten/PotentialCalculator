using PotentialCalculator.Controls;
using PotentialCalculator.Models;
using System;
using System.Collections.Generic;
using System.Linq;

namespace PotentialCalculator {
    public partial class ResultForm : DevExpress.XtraEditors.XtraForm {
        public ResultForm() {
            InitializeComponent();
        }
        public void Render() {
            var project = Project.ProjectInstance;
            foreach (var criteria in project.Criterias) {
                var index = layoutControl1.Root.OptionsTableLayoutGroup.RowDefinitions.Add(new DevExpress.XtraLayout.RowDefinition());
                GraphControl graph = GraphControl.CreateGraphControlForCriteria(criteria);
                var item = layoutControl1.AddItem(criteria.Name, graph);
                item.OptionsTableLayoutItem.RowIndex = index;
            }
        }

        private void barButtonItem1_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e) {
            Calc();
        }

        private void Calc() {
            var graphs = layoutControl1.Controls.OfType<GraphControl>();
            var calculation = new List<Tuple<double, double, double>>();
            foreach (var graph in graphs) {
                var result = graph.CalcPs();
                calculation.Add(result);
            }
            double detectionP = 1;
            double errorP = 1;
            double missP;
            foreach (var item in calculation) {
                detectionP = detectionP * item.Item1;
                errorP = errorP * item.Item2;
            }
            missP = 1 - detectionP;
            pControl1.setPs(detectionP, errorP, missP);
        }

        private void barButtonItem2_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e) {
            var form = new CCForm();
            this.Calc();
            form.Calc();
            form.ShowDialog();
        }
    }
}