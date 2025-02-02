namespace TakeoutWranglerUI
{
    partial class NewPrintDialog
    {
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            labelPrinter = new Label();
            printPreviewControl = new PrintPreviewControl();
            comboBoxPrinters = new ComboBox();
            labelOrientation = new Label();
            comboBoxOrientation = new ComboBox();
            labelCopies = new Label();
            numericUpDownCopies = new NumericUpDown();
            labelColorMode = new Label();
            comboBoxColorMode = new ComboBox();
            labelBothSides = new Label();
            comboBoxBothSides = new ComboBox();
            buttonCancel = new Button();
            buttonPrint = new Button();
            buttonNext = new Button();
            buttonPrev = new Button();
            numericUpDownDisplayPageCount = new NumericUpDown();
            labelViewCount = new Label();
            groupBoxPageRange = new GroupBox();
            textBoxToPage = new TextBox();
            labelPageTo = new Label();
            textBoxFromPage = new TextBox();
            radioButtonPages = new RadioButton();
            radioButtonSelection = new RadioButton();
            radioButtonAll = new RadioButton();
            labelPage = new Label();
            textBoxPage = new TextBox();
            ((System.ComponentModel.ISupportInitialize)numericUpDownCopies).BeginInit();
            ((System.ComponentModel.ISupportInitialize)numericUpDownDisplayPageCount).BeginInit();
            groupBoxPageRange.SuspendLayout();
            SuspendLayout();
            // 
            // labelPrinter
            // 
            labelPrinter.AutoSize = true;
            labelPrinter.Location = new Point(14, 13);
            labelPrinter.Name = "labelPrinter";
            labelPrinter.Size = new Size(42, 15);
            labelPrinter.TabIndex = 0;
            labelPrinter.Text = "Printer";
            // 
            // printPreviewControl
            // 
            printPreviewControl.Anchor = AnchorStyles.Top | AnchorStyles.Bottom | AnchorStyles.Left | AnchorStyles.Right;
            printPreviewControl.Location = new Point(265, 32);
            printPreviewControl.Name = "printPreviewControl";
            printPreviewControl.Size = new Size(383, 365);
            printPreviewControl.TabIndex = 1;
            // 
            // comboBoxPrinters
            // 
            comboBoxPrinters.FormattingEnabled = true;
            comboBoxPrinters.Location = new Point(14, 31);
            comboBoxPrinters.Name = "comboBoxPrinters";
            comboBoxPrinters.Size = new Size(238, 23);
            comboBoxPrinters.TabIndex = 2;
            comboBoxPrinters.SelectedIndexChanged += comboBoxPrinters_SelectedIndexChanged;
            // 
            // labelOrientation
            // 
            labelOrientation.AutoSize = true;
            labelOrientation.Location = new Point(14, 61);
            labelOrientation.Name = "labelOrientation";
            labelOrientation.Size = new Size(67, 15);
            labelOrientation.TabIndex = 4;
            labelOrientation.Text = "Orientation";
            // 
            // comboBoxOrientation
            // 
            comboBoxOrientation.FormattingEnabled = true;
            comboBoxOrientation.Location = new Point(14, 79);
            comboBoxOrientation.Name = "comboBoxOrientation";
            comboBoxOrientation.Size = new Size(235, 23);
            comboBoxOrientation.TabIndex = 5;
            // 
            // labelCopies
            // 
            labelCopies.AutoSize = true;
            labelCopies.Location = new Point(14, 110);
            labelCopies.Name = "labelCopies";
            labelCopies.Size = new Size(43, 15);
            labelCopies.TabIndex = 6;
            labelCopies.Text = "Copies";
            // 
            // numericUpDownCopies
            // 
            numericUpDownCopies.Location = new Point(14, 128);
            numericUpDownCopies.Name = "numericUpDownCopies";
            numericUpDownCopies.Size = new Size(77, 23);
            numericUpDownCopies.TabIndex = 7;
            // 
            // labelColorMode
            // 
            labelColorMode.AutoSize = true;
            labelColorMode.Location = new Point(14, 159);
            labelColorMode.Name = "labelColorMode";
            labelColorMode.Size = new Size(70, 15);
            labelColorMode.TabIndex = 8;
            labelColorMode.Text = "Color mode";
            // 
            // comboBoxColorMode
            // 
            comboBoxColorMode.FormattingEnabled = true;
            comboBoxColorMode.Location = new Point(14, 177);
            comboBoxColorMode.Name = "comboBoxColorMode";
            comboBoxColorMode.Size = new Size(235, 23);
            comboBoxColorMode.TabIndex = 9;
            // 
            // labelBothSides
            // 
            labelBothSides.AutoSize = true;
            labelBothSides.Location = new Point(14, 209);
            labelBothSides.Name = "labelBothSides";
            labelBothSides.Size = new Size(106, 15);
            labelBothSides.TabIndex = 10;
            labelBothSides.Text = "Print on both sides";
            // 
            // comboBoxBothSides
            // 
            comboBoxBothSides.FormattingEnabled = true;
            comboBoxBothSides.Location = new Point(14, 227);
            comboBoxBothSides.Name = "comboBoxBothSides";
            comboBoxBothSides.Size = new Size(235, 23);
            comboBoxBothSides.TabIndex = 11;
            // 
            // buttonCancel
            // 
            buttonCancel.Anchor = AnchorStyles.Bottom | AnchorStyles.Right;
            buttonCancel.DialogResult = DialogResult.Cancel;
            buttonCancel.Location = new Point(573, 405);
            buttonCancel.Name = "buttonCancel";
            buttonCancel.Size = new Size(75, 23);
            buttonCancel.TabIndex = 14;
            buttonCancel.Text = "Cancel";
            buttonCancel.UseVisualStyleBackColor = true;
            // 
            // buttonPrint
            // 
            buttonPrint.Anchor = AnchorStyles.Bottom | AnchorStyles.Right;
            buttonPrint.Location = new Point(495, 405);
            buttonPrint.Name = "buttonPrint";
            buttonPrint.Size = new Size(75, 23);
            buttonPrint.TabIndex = 15;
            buttonPrint.Text = "Print";
            buttonPrint.UseVisualStyleBackColor = true;
            // 
            // buttonNext
            // 
            buttonNext.Location = new Point(300, 6);
            buttonNext.Name = "buttonNext";
            buttonNext.Size = new Size(28, 23);
            buttonNext.TabIndex = 17;
            buttonNext.Text = ">";
            buttonNext.UseVisualStyleBackColor = true;
            buttonNext.Click += buttonNext_Click;
            // 
            // buttonPrev
            // 
            buttonPrev.Location = new Point(268, 6);
            buttonPrev.Name = "buttonPrev";
            buttonPrev.Size = new Size(28, 23);
            buttonPrev.TabIndex = 18;
            buttonPrev.Text = "<";
            buttonPrev.UseVisualStyleBackColor = true;
            buttonPrev.Click += buttonPrev_Click;
            // 
            // numericUpDownDisplayPageCount
            // 
            numericUpDownDisplayPageCount.Location = new Point(557, 7);
            numericUpDownDisplayPageCount.Maximum = new decimal(new int[] { 3, 0, 0, 0 });
            numericUpDownDisplayPageCount.Minimum = new decimal(new int[] { 1, 0, 0, 0 });
            numericUpDownDisplayPageCount.Name = "numericUpDownDisplayPageCount";
            numericUpDownDisplayPageCount.ReadOnly = true;
            numericUpDownDisplayPageCount.Size = new Size(33, 23);
            numericUpDownDisplayPageCount.TabIndex = 19;
            numericUpDownDisplayPageCount.TextAlign = HorizontalAlignment.Center;
            numericUpDownDisplayPageCount.Value = new decimal(new int[] { 1, 0, 0, 0 });
            numericUpDownDisplayPageCount.ValueChanged += numericUpDownDisplayPageCount_ValueChanged;
            // 
            // labelViewCount
            // 
            labelViewCount.AutoSize = true;
            labelViewCount.Location = new Point(484, 9);
            labelViewCount.Name = "labelViewCount";
            labelViewCount.Size = new Size(69, 15);
            labelViewCount.TabIndex = 20;
            labelViewCount.Text = "View count:";
            // 
            // groupBoxPageRange
            // 
            groupBoxPageRange.Controls.Add(textBoxToPage);
            groupBoxPageRange.Controls.Add(labelPageTo);
            groupBoxPageRange.Controls.Add(textBoxFromPage);
            groupBoxPageRange.Controls.Add(radioButtonPages);
            groupBoxPageRange.Controls.Add(radioButtonSelection);
            groupBoxPageRange.Controls.Add(radioButtonAll);
            groupBoxPageRange.Location = new Point(14, 259);
            groupBoxPageRange.Name = "groupBoxPageRange";
            groupBoxPageRange.Size = new Size(235, 106);
            groupBoxPageRange.TabIndex = 21;
            groupBoxPageRange.TabStop = false;
            groupBoxPageRange.Text = "Page range";
            // 
            // textBoxToPage
            // 
            textBoxToPage.Location = new Point(179, 72);
            textBoxToPage.Name = "textBoxToPage";
            textBoxToPage.Size = new Size(49, 23);
            textBoxToPage.TabIndex = 23;
            textBoxToPage.TextAlign = HorizontalAlignment.Center;
            textBoxToPage.Validating += textBoxFromPage_Validating;
            // 
            // labelPageTo
            // 
            labelPageTo.AutoSize = true;
            labelPageTo.Location = new Point(155, 77);
            labelPageTo.Name = "labelPageTo";
            labelPageTo.Size = new Size(18, 15);
            labelPageTo.TabIndex = 22;
            labelPageTo.Text = "to";
            // 
            // textBoxFromPage
            // 
            textBoxFromPage.Location = new Point(98, 72);
            textBoxFromPage.Name = "textBoxFromPage";
            textBoxFromPage.Size = new Size(50, 23);
            textBoxFromPage.TabIndex = 3;
            textBoxFromPage.TextAlign = HorizontalAlignment.Center;
            textBoxFromPage.Validating += textBoxFromPage_Validating;
            // 
            // radioButtonPages
            // 
            radioButtonPages.AutoSize = true;
            radioButtonPages.Location = new Point(7, 73);
            radioButtonPages.Name = "radioButtonPages";
            radioButtonPages.Size = new Size(88, 19);
            radioButtonPages.TabIndex = 2;
            radioButtonPages.TabStop = true;
            radioButtonPages.Text = "Pages from:";
            radioButtonPages.UseVisualStyleBackColor = true;
            radioButtonPages.CheckedChanged += radioButtonPages_CheckedChanged;
            // 
            // radioButtonSelection
            // 
            radioButtonSelection.AutoSize = true;
            radioButtonSelection.Location = new Point(7, 48);
            radioButtonSelection.Name = "radioButtonSelection";
            radioButtonSelection.Size = new Size(73, 19);
            radioButtonSelection.TabIndex = 1;
            radioButtonSelection.TabStop = true;
            radioButtonSelection.Text = "Selection";
            radioButtonSelection.UseVisualStyleBackColor = true;
            radioButtonSelection.CheckedChanged += radioButtonSelection_CheckedChanged;
            // 
            // radioButtonAll
            // 
            radioButtonAll.AutoSize = true;
            radioButtonAll.Location = new Point(7, 23);
            radioButtonAll.Name = "radioButtonAll";
            radioButtonAll.Size = new Size(39, 19);
            radioButtonAll.TabIndex = 0;
            radioButtonAll.TabStop = true;
            radioButtonAll.Text = "All";
            radioButtonAll.UseVisualStyleBackColor = true;
            radioButtonAll.CheckedChanged += radioButtonAll_CheckedChanged;
            // 
            // labelPage
            // 
            labelPage.AutoSize = true;
            labelPage.Location = new Point(340, 9);
            labelPage.Name = "labelPage";
            labelPage.Size = new Size(36, 15);
            labelPage.TabIndex = 22;
            labelPage.Text = "Page:";
            // 
            // textBoxPage
            // 
            textBoxPage.Location = new Point(378, 6);
            textBoxPage.Name = "textBoxPage";
            textBoxPage.ReadOnly = true;
            textBoxPage.Size = new Size(100, 23);
            textBoxPage.TabIndex = 23;
            textBoxPage.TextAlign = HorizontalAlignment.Center;
            textBoxPage.WordWrap = false;
            // 
            // NewPrintDialog
            // 
            AutoScaleDimensions = new SizeF(7F, 15F);
            AutoScaleMode = AutoScaleMode.Font;
            ClientSize = new Size(660, 437);
            Controls.Add(textBoxPage);
            Controls.Add(labelPage);
            Controls.Add(groupBoxPageRange);
            Controls.Add(labelViewCount);
            Controls.Add(numericUpDownDisplayPageCount);
            Controls.Add(buttonPrev);
            Controls.Add(buttonNext);
            Controls.Add(buttonPrint);
            Controls.Add(buttonCancel);
            Controls.Add(comboBoxBothSides);
            Controls.Add(labelBothSides);
            Controls.Add(comboBoxColorMode);
            Controls.Add(labelColorMode);
            Controls.Add(numericUpDownCopies);
            Controls.Add(labelCopies);
            Controls.Add(comboBoxOrientation);
            Controls.Add(labelOrientation);
            Controls.Add(comboBoxPrinters);
            Controls.Add(printPreviewControl);
            Controls.Add(labelPrinter);
            MaximizeBox = false;
            MinimizeBox = false;
            MinimumSize = new Size(676, 476);
            Name = "NewPrintDialog";
            ShowIcon = false;
            Text = "Print";
            ((System.ComponentModel.ISupportInitialize)numericUpDownCopies).EndInit();
            ((System.ComponentModel.ISupportInitialize)numericUpDownDisplayPageCount).EndInit();
            groupBoxPageRange.ResumeLayout(false);
            groupBoxPageRange.PerformLayout();
            ResumeLayout(false);
            PerformLayout();
        }

        #endregion

        private Label labelPrinter;
        private PrintPreviewControl printPreviewControl;
        private ComboBox comboBoxPrinters;
        private Label labelOrientation;
        private ComboBox comboBoxOrientation;
        private Label labelCopies;
        private NumericUpDown numericUpDownCopies;
        private Label labelColorMode;
        private ComboBox comboBoxColorMode;
        private Label labelBothSides;
        private ComboBox comboBoxBothSides;
        private Button buttonCancel;
        private Button buttonPrint;
        private Button buttonNext;
        private Button buttonPrev;
        private NumericUpDown numericUpDownDisplayPageCount;
        private Label labelViewCount;
        private GroupBox groupBoxPageRange;
        private Label labelPageTo;
        private TextBox textBoxFromPage;
        private RadioButton radioButtonPages;
        private RadioButton radioButtonSelection;
        private RadioButton radioButtonAll;
        private TextBox textBoxToPage;
        private Label labelPage;
        private TextBox textBoxPage;
    }
}