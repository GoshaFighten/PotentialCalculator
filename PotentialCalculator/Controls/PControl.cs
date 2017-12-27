using System;
using System.Linq;

namespace PotentialCalculator.Controls {
    public partial class PControl : DevExpress.XtraEditors.XtraUserControl {
        public PControl() {
            InitializeComponent();
        }
        public void setPs(double detectionP, double errorP, double missP) {
            textEdit1.EditValue = detectionP;
            textEdit2.EditValue = errorP;
            textEdit3.EditValue = missP;
        }
    }
}
