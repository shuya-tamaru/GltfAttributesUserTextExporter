using System;
using System.Drawing;
using System.Windows.Forms;

namespace GltfAttributesExporter.Export
{
    public class ExportOptionsDialog : Form
    {
        public bool GroupByLayer { get; private set; }
        public bool UseDracoCompression { get; private set; }
        public int CompressionLevel { get; private set; }

        private CheckBox groupByLayerCheckBox;
        private CheckBox useDracoCompressionCheckBox;
        private NumericUpDown compressionLevelNumericUpDown;

        public ExportOptionsDialog()
        {
            Text = "Export Options";
            Width = 400;
            Height = 250;
            FormBorderStyle = FormBorderStyle.FixedDialog;

            var mainPanel = new TableLayoutPanel
            {
                Dock = DockStyle.Fill,
                ColumnCount = 2,
                RowCount = 4,
                Padding = new Padding(10),
                AutoSize = true,
                AutoSizeMode = AutoSizeMode.GrowAndShrink
            };
            Controls.Add(mainPanel);

            groupByLayerCheckBox = new CheckBox
            {
                Text = "Group by Layer",
                Anchor = AnchorStyles.Left,
                Checked = true,
                AutoSize = true
            };
            mainPanel.Controls.Add(groupByLayerCheckBox, 0, 0);
            mainPanel.SetColumnSpan(groupByLayerCheckBox, 2);

            useDracoCompressionCheckBox = new CheckBox
            {
                Text = "Use Draco Compression",
                Anchor = AnchorStyles.Left,
                Checked = false,
                AutoSize = true
            };
            mainPanel.Controls.Add(useDracoCompressionCheckBox, 0, 1);
            mainPanel.SetColumnSpan(useDracoCompressionCheckBox, 2);

            var compressionLevelLabel = new Label
            {
                Text = "Compression Level:",
                Anchor = AnchorStyles.Left,
                TextAlign = ContentAlignment.MiddleLeft,
                AutoSize = true,
                Width = 150 // ラベルの幅を設定
            };
            mainPanel.Controls.Add(compressionLevelLabel, 0, 2);

            compressionLevelNumericUpDown = new NumericUpDown
            {
                Minimum = 0,
                Maximum = 10,
                Value = 5,
                Enabled = false,
                Anchor = AnchorStyles.Left,
                Width = 80,
                AutoSize = true
            };
            mainPanel.Controls.Add(compressionLevelNumericUpDown, 1, 2);

            useDracoCompressionCheckBox.CheckedChanged += (sender, e) =>
            {
                compressionLevelNumericUpDown.Enabled = useDracoCompressionCheckBox.Checked;
            };

            var buttonPanel = new FlowLayoutPanel
            {
                FlowDirection = FlowDirection.RightToLeft,
                Dock = DockStyle.Fill,
                AutoSize = true,
                AutoSizeMode = AutoSizeMode.GrowAndShrink
            };
            mainPanel.Controls.Add(buttonPanel, 0, 3);
            mainPanel.SetColumnSpan(buttonPanel, 2);

            var okButton = new Button
            {
                Text = "OK",
                DialogResult = DialogResult.OK,
                Width = 100,
                Height = 30,
                Margin = new Padding(10)
            };
            buttonPanel.Controls.Add(okButton);

            var cancelButton = new Button
            {
                Text = "Cancel",
                DialogResult = DialogResult.Cancel,
                Width = 100,
                Height = 30,
                Margin = new Padding(10)
            };
            buttonPanel.Controls.Add(cancelButton);

            AcceptButton = okButton;
            CancelButton = cancelButton;
        }

        protected override void OnFormClosing(FormClosingEventArgs e)
        {
            base.OnFormClosing(e);
            if (DialogResult == DialogResult.OK)
            {
                GroupByLayer = groupByLayerCheckBox.Checked;
                UseDracoCompression = useDracoCompressionCheckBox.Checked;
                CompressionLevel = (int)compressionLevelNumericUpDown.Value;
            }
        }
    }
}
