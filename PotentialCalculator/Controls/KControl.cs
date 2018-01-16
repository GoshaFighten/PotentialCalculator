using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Text;
using System.Linq;
using System.Windows.Forms;
using DevExpress.XtraEditors;
using PotentialCalculator.Models;

namespace PotentialCalculator.Controls {
    public partial class KControl : DevExpress.XtraEditors.XtraUserControl {
        public KControl() {
            InitializeComponent();
        }
        public void Calc(Dictionary<Criteria, double> ks) {
            foreach (var k in ks) {
                this.AddRow(k.Key, k.Value);
            }
        }
        void AddRow(Criteria criteria, double k) {
            var index = layoutControl1.Root.OptionsTableLayoutGroup.RowDefinitions.Add(new DevExpress.XtraLayout.RowDefinition());
            var txt = new TextEdit();
            txt.EditValue = k;
            var item = layoutControl1.AddItem("K " + criteria.Name, txt);
            item.OptionsTableLayoutItem.RowIndex = index;
        }
    }
}
