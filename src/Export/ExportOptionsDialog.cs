using System;
using System.Drawing;
using System.Windows.Forms;

namespace GltfAttributesExporter.Export
{
    public class ExportOptionsDialog : Form
    {
        public bool GroupByLayer { get; private set; }
        public bool VertexColorExport { get; private set; }

        private CheckBox groupByLayerCheckBox;
        private CheckBox vertecColorCheckBox;

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

            vertecColorCheckBox = new CheckBox
            {
                Text = "Export VertexColor",
                Anchor = AnchorStyles.Left,
                Checked = false,
                AutoSize = true
            };
            mainPanel.Controls.Add(vertecColorCheckBox, 0, 1);
            mainPanel.SetColumnSpan(vertecColorCheckBox, 2);

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
                VertexColorExport = vertecColorCheckBox.Checked;
            }
        }
    }
}
