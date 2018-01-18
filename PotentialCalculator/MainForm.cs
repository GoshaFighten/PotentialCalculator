using DevExpress.XtraEditors;
using PotentialCalculator.Helpers;
using PotentialCalculator.Models;
using System;
using System.Windows.Forms;

namespace PotentialCalculator {
    public partial class MainForm : XtraForm {
        Project project = Project.ProjectInstance;
        public MainForm() {
            InitializeComponent();
            projectBindingSource.DataSource = project;
        }

        private void simpleButton1_Click(object sender, EventArgs e) {
            gridView1.AddNewRow();
        }

        private void simpleButton2_Click(object sender, EventArgs e) {
            gridView1.DeleteRow(gridView1.FocusedRowHandle);
        }

        private void simpleButton3_Click(object sender, EventArgs e) {
            var form = new ResultForm();
            form.Render();
            form.ShowDialog();
        }

        private void simpleButton4_Click(object sender, EventArgs e) {
            using (SaveFileDialog dialog = GetSaveFileDialog()) {
                if (dialog.ShowDialog() != DialogResult.OK) {
                    return;
                }
                ProjectHelper.Save(project, dialog.FileName);
            }
        }

        private void simpleButton5_Click(object sender, EventArgs e) {
            using (OpenFileDialog dialog = GetOpenFileDialog()) {
                if (dialog.ShowDialog() != DialogResult.OK) {
                    return;
                }
                ProjectHelper.Open(project, dialog.FileName);
            }
        }
        private static OpenFileDialog GetOpenFileDialog() {
            OpenFileDialog dialog = new OpenFileDialog();
            return dialog;
        }
        private static SaveFileDialog GetSaveFileDialog() {
            SaveFileDialog dialog = new SaveFileDialog();
            return dialog;
        }

        private void repositoryItemButtonEdit1_ButtonClick(object sender, DevExpress.XtraEditors.Controls.ButtonPressedEventArgs e) {
            Criteria criteria = (Criteria)gridView1.GetFocusedRow();
            var form = new TForm();
            form.Render(criteria);
            form.ShowDialog();
        }
    }
}
