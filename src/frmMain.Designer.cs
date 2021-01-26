namespace Deconvolvulator
{
    partial class frmMain
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
            this.components = new System.ComponentModel.Container();
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(frmMain));
            this.btnDeblur = new System.Windows.Forms.Button();
            this.label3 = new System.Windows.Forms.Label();
            this.txtNSR = new System.Windows.Forms.TextBox();
            this.txtImage = new System.Windows.Forms.TextBox();
            this.chkRepairEdges = new System.Windows.Forms.CheckBox();
            this.picOut = new System.Windows.Forms.PictureBox();
            this.pnlOut = new System.Windows.Forms.Panel();
            this.label6 = new System.Windows.Forms.Label();
            this.txtPSFLineThickness = new System.Windows.Forms.TextBox();
            this.label7 = new System.Windows.Forms.Label();
            this.label8 = new System.Windows.Forms.Label();
            this.txtFeather = new System.Windows.Forms.TextBox();
            this.chkAntiAliasLine = new System.Windows.Forms.CheckBox();
            this.picFilterFT = new System.Windows.Forms.PictureBox();
            this.txtCLS_Y = new System.Windows.Forms.TextBox();
            this.txtIterations = new System.Windows.Forms.TextBox();
            this.btnStopIterations = new System.Windows.Forms.Button();
            this.lblIterationInfo = new System.Windows.Forms.Label();
            this.btnFFTW = new System.Windows.Forms.Button();
            this.grpMethod = new System.Windows.Forms.GroupBox();
            this.optRIF = new System.Windows.Forms.RadioButton();
            this.label32 = new System.Windows.Forms.Label();
            this.label25 = new System.Windows.Forms.Label();
            this.label20 = new System.Windows.Forms.Label();
            this.label19 = new System.Windows.Forms.Label();
            this.label18 = new System.Windows.Forms.Label();
            this.label17 = new System.Windows.Forms.Label();
            this.optLR = new System.Windows.Forms.RadioButton();
            this.trkBlur = new System.Windows.Forms.TrackBar();
            this.trkIterations = new System.Windows.Forms.TrackBar();
            this.optLandweber = new System.Windows.Forms.RadioButton();
            this.optTikhonov = new System.Windows.Forms.RadioButton();
            this.optWiener = new System.Windows.Forms.RadioButton();
            this.label12 = new System.Windows.Forms.Label();
            this.optCustomRepair = new System.Windows.Forms.RadioButton();
            this.grpPSF = new System.Windows.Forms.GroupBox();
            this.udWave = new System.Windows.Forms.NumericUpDown();
            this.label30 = new System.Windows.Forms.Label();
            this.optMTFPSF = new System.Windows.Forms.RadioButton();
            this.label15 = new System.Windows.Forms.Label();
            this.label23 = new System.Windows.Forms.Label();
            this.udGaussFraction = new System.Windows.Forms.NumericUpDown();
            this.optVoigt = new System.Windows.Forms.RadioButton();
            this.label22 = new System.Windows.Forms.Label();
            this.udMoffatPasses = new System.Windows.Forms.NumericUpDown();
            this.chkMoffatInPasses = new System.Windows.Forms.CheckBox();
            this.udCropPSF = new System.Windows.Forms.NumericUpDown();
            this.chkCropPSF = new System.Windows.Forms.CheckBox();
            this.udBrightness = new System.Windows.Forms.NumericUpDown();
            this.udFWHM = new System.Windows.Forms.NumericUpDown();
            this.udMoffatBeta = new System.Windows.Forms.NumericUpDown();
            this.label13 = new System.Windows.Forms.Label();
            this.optMoffat = new System.Windows.Forms.RadioButton();
            this.optGaussianDeblur = new System.Windows.Forms.RadioButton();
            this.optCameraCircleDeblur = new System.Windows.Forms.RadioButton();
            this.chkAutoDeblur = new System.Windows.Forms.CheckBox();
            this.grpEdgeRepair = new System.Windows.Forms.GroupBox();
            this.udDeringRepairStrength = new System.Windows.Forms.NumericUpDown();
            this.udDeringStarThreshold = new System.Windows.Forms.NumericUpDown();
            this.chkDeringing = new System.Windows.Forms.CheckBox();
            this.chkRepairTopBottom = new System.Windows.Forms.CheckBox();
            this.grpMotionBlurDetails = new System.Windows.Forms.GroupBox();
            this.chkRotateImage = new System.Windows.Forms.CheckBox();
            this.udCentreFieldRotationY = new System.Windows.Forms.NumericUpDown();
            this.udCentreFieldRotationX = new System.Windows.Forms.NumericUpDown();
            this.chkFiledRotationDeblur = new System.Windows.Forms.CheckBox();
            this.udRotationTiles = new System.Windows.Forms.NumericUpDown();
            this.udMotionBlurAngle = new System.Windows.Forms.NumericUpDown();
            this.udMotionBlurLength = new System.Windows.Forms.NumericUpDown();
            this.udFieldRotationAngle = new System.Windows.Forms.NumericUpDown();
            this.label2 = new System.Windows.Forms.Label();
            this.label27 = new System.Windows.Forms.Label();
            this.label1 = new System.Windows.Forms.Label();
            this.label14 = new System.Windows.Forms.Label();
            this.grpPSFModify = new System.Windows.Forms.GroupBox();
            this.txtPSFDump = new System.Windows.Forms.TextBox();
            this.btnCopyPSF = new System.Windows.Forms.Button();
            this.btnSave = new System.Windows.Forms.Button();
            this.cboHistory = new System.Windows.Forms.ComboBox();
            this.label16 = new System.Windows.Forms.Label();
            this.groupBox1 = new System.Windows.Forms.GroupBox();
            this.lblLastStored = new System.Windows.Forms.Label();
            this.optPrevious = new System.Windows.Forms.RadioButton();
            this.optOriginal = new System.Windows.Forms.RadioButton();
            this.btnConvolve = new System.Windows.Forms.Button();
            this.lblProcessing = new System.Windows.Forms.Label();
            this.label21 = new System.Windows.Forms.Label();
            this.cboCurrent = new System.Windows.Forms.ComboBox();
            this.grpPSFRepair = new System.Windows.Forms.GroupBox();
            this.chkProcessLuminance = new System.Windows.Forms.CheckBox();
            this.label31 = new System.Windows.Forms.Label();
            this.udUpscale = new System.Windows.Forms.NumericUpDown();
            this.optSharpeningLayers = new System.Windows.Forms.RadioButton();
            this.optCircularBlur = new System.Windows.Forms.RadioButton();
            this.optMotionBlur = new System.Windows.Forms.RadioButton();
            this.grpLayers = new System.Windows.Forms.GroupBox();
            this.btnShowLayers = new System.Windows.Forms.Button();
            this.udLayersImageScale = new System.Windows.Forms.NumericUpDown();
            this.label29 = new System.Windows.Forms.Label();
            this.trkLayers5 = new System.Windows.Forms.TrackBar();
            this.udLayersScale = new System.Windows.Forms.NumericUpDown();
            this.label9 = new System.Windows.Forms.Label();
            this.btnResetLayers = new System.Windows.Forms.Button();
            this.trkLayers2 = new System.Windows.Forms.TrackBar();
            this.label11 = new System.Windows.Forms.Label();
            this.cboLayerSettings = new System.Windows.Forms.ComboBox();
            this.txtSaveSettingsName = new System.Windows.Forms.TextBox();
            this.btnSaveLayersSettings = new System.Windows.Forms.Button();
            this.udLayersNoiseControl = new System.Windows.Forms.NumericUpDown();
            this.label10 = new System.Windows.Forms.Label();
            this.chkLayers5 = new System.Windows.Forms.CheckBox();
            this.chkLayers4 = new System.Windows.Forms.CheckBox();
            this.chkLayers3 = new System.Windows.Forms.CheckBox();
            this.chkLayers2 = new System.Windows.Forms.CheckBox();
            this.chkLayers1 = new System.Windows.Forms.CheckBox();
            this.chkLayers0 = new System.Windows.Forms.CheckBox();
            this.trkLayers1 = new System.Windows.Forms.TrackBar();
            this.trkLayers0 = new System.Windows.Forms.TrackBar();
            this.trkLayers4 = new System.Windows.Forms.TrackBar();
            this.trkLayers3 = new System.Windows.Forms.TrackBar();
            this.cboKernelSharpeningLayers = new System.Windows.Forms.ComboBox();
            this.picPSFProfile = new System.Windows.Forms.PictureBox();
            this.label24 = new System.Windows.Forms.Label();
            this.udPSFPlotWidth = new System.Windows.Forms.NumericUpDown();
            this.btnClearPSFPlot = new System.Windows.Forms.Button();
            this.label26 = new System.Windows.Forms.Label();
            this.udZoom = new System.Windows.Forms.NumericUpDown();
            this.trkGif = new System.Windows.Forms.TrackBar();
            this.chkClickCompareTo = new System.Windows.Forms.CheckBox();
            this.btnOriginal = new System.Windows.Forms.Button();
            this.btnLimb = new System.Windows.Forms.Button();
            this.grpAnalysis = new System.Windows.Forms.GroupBox();
            this.lblLaplacian = new System.Windows.Forms.Label();
            this.chkLaplacian = new System.Windows.Forms.CheckBox();
            this.btnStoreOutput = new System.Windows.Forms.Button();
            this.btnFileLocation = new System.Windows.Forms.Button();
            this.tip = new System.Windows.Forms.ToolTip(this.components);
            this.chkAutostretch = new System.Windows.Forms.CheckBox();
            this.cboZoomMode = new System.Windows.Forms.ComboBox();
            this.btnDetailMask = new System.Windows.Forms.Button();
            this.optFilterFourierTransform = new System.Windows.Forms.RadioButton();
            this.optMTF = new System.Windows.Forms.RadioButton();
            this.btnFourierTransform = new System.Windows.Forms.Button();
            this.chkPSF = new System.Windows.Forms.CheckBox();
            this.btnRotate = new System.Windows.Forms.Button();
            this.btnTest = new System.Windows.Forms.Button();
            this.lblPixelPos = new System.Windows.Forms.Label();
            this.chkTanhRepair = new System.Windows.Forms.CheckBox();
            this.txtGamma = new System.Windows.Forms.TextBox();
            this.label5 = new System.Windows.Forms.Label();
            this.txtBeta = new System.Windows.Forms.TextBox();
            this.label4 = new System.Windows.Forms.Label();
            this.label28 = new System.Windows.Forms.Label();
            this.grpPlotExtra = new System.Windows.Forms.GroupBox();
            this.mnuRightMain = new System.Windows.Forms.ContextMenuStrip(this.components);
            this.mnuSetArea = new System.Windows.Forms.ToolStripMenuItem();
            this.mnuSetWholeImage = new System.Windows.Forms.ToolStripMenuItem();
            this.btnSavePSF = new System.Windows.Forms.Button();
            this.picPSF = new Deconvolvulator.PixelBox();
            ((System.ComponentModel.ISupportInitialize)(this.picOut)).BeginInit();
            this.pnlOut.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.picFilterFT)).BeginInit();
            this.grpMethod.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.trkBlur)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.trkIterations)).BeginInit();
            this.grpPSF.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.udWave)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.udGaussFraction)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.udMoffatPasses)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.udCropPSF)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.udBrightness)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.udFWHM)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.udMoffatBeta)).BeginInit();
            this.grpEdgeRepair.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.udDeringRepairStrength)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.udDeringStarThreshold)).BeginInit();
            this.grpMotionBlurDetails.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.udCentreFieldRotationY)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.udCentreFieldRotationX)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.udRotationTiles)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.udMotionBlurAngle)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.udMotionBlurLength)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.udFieldRotationAngle)).BeginInit();
            this.grpPSFModify.SuspendLayout();
            this.groupBox1.SuspendLayout();
            this.grpPSFRepair.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.udUpscale)).BeginInit();
            this.grpLayers.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.udLayersImageScale)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.trkLayers5)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.udLayersScale)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.trkLayers2)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.udLayersNoiseControl)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.trkLayers1)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.trkLayers0)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.trkLayers4)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.trkLayers3)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.picPSFProfile)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.udPSFPlotWidth)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.udZoom)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.trkGif)).BeginInit();
            this.grpAnalysis.SuspendLayout();
            this.grpPlotExtra.SuspendLayout();
            this.mnuRightMain.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.picPSF)).BeginInit();
            this.SuspendLayout();
            // 
            // btnDeblur
            // 
            this.btnDeblur.BackColor = System.Drawing.Color.PaleGreen;
            this.btnDeblur.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.btnDeblur.Location = new System.Drawing.Point(1, 22);
            this.btnDeblur.Name = "btnDeblur";
            this.btnDeblur.Size = new System.Drawing.Size(98, 31);
            this.btnDeblur.TabIndex = 3;
            this.btnDeblur.Text = "REPAIR";
            this.btnDeblur.UseVisualStyleBackColor = false;
            this.btnDeblur.Click += new System.EventHandler(this.btnDeblur_Click);
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Location = new System.Drawing.Point(162, 15);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(33, 13);
            this.label3.TabIndex = 6;
            this.label3.Text = "NSR";
            // 
            // txtNSR
            // 
            this.txtNSR.Location = new System.Drawing.Point(195, 12);
            this.txtNSR.Name = "txtNSR";
            this.txtNSR.Size = new System.Drawing.Size(46, 20);
            this.txtNSR.TabIndex = 26;
            this.txtNSR.Text = "0.001";
            this.tip.SetToolTip(this.txtNSR, "Noise to signal ratio");
            // 
            // txtImage
            // 
            this.txtImage.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.txtImage.Location = new System.Drawing.Point(2, 2);
            this.txtImage.Name = "txtImage";
            this.txtImage.Size = new System.Drawing.Size(134, 20);
            this.txtImage.TabIndex = 1;
            this.txtImage.Text = "Jup_CloudyNights_Bird.tif";
            this.txtImage.TextChanged += new System.EventHandler(this.txtImage_TextChanged);
            // 
            // chkRepairEdges
            // 
            this.chkRepairEdges.AutoSize = true;
            this.chkRepairEdges.Font = new System.Drawing.Font("Microsoft Sans Serif", 6.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.chkRepairEdges.Location = new System.Drawing.Point(1, 14);
            this.chkRepairEdges.Name = "chkRepairEdges";
            this.chkRepairEdges.Size = new System.Drawing.Size(101, 16);
            this.chkRepairEdges.TabIndex = 150;
            this.chkRepairEdges.Text = "Fix border artifacts";
            this.tip.SetToolTip(this.chkRepairEdges, "Fix borders to avoid artifacts Wiener and Tikhonov FFT repairs");
            this.chkRepairEdges.UseVisualStyleBackColor = true;
            // 
            // picOut
            // 
            this.picOut.Location = new System.Drawing.Point(5, 7);
            this.picOut.Name = "picOut";
            this.picOut.Size = new System.Drawing.Size(510, 510);
            this.picOut.SizeMode = System.Windows.Forms.PictureBoxSizeMode.AutoSize;
            this.picOut.TabIndex = 15;
            this.picOut.TabStop = false;
            this.picOut.MouseDown += new System.Windows.Forms.MouseEventHandler(this.picOut_MouseDown);
            this.picOut.MouseEnter += new System.EventHandler(this.picOut_MouseEnter);
            this.picOut.MouseMove += new System.Windows.Forms.MouseEventHandler(this.picOut_MouseMove);
            this.picOut.MouseUp += new System.Windows.Forms.MouseEventHandler(this.picOut_MouseUp);
            // 
            // pnlOut
            // 
            this.pnlOut.AutoScroll = true;
            this.pnlOut.Controls.Add(this.picOut);
            this.pnlOut.Location = new System.Drawing.Point(288, 140);
            this.pnlOut.Name = "pnlOut";
            this.pnlOut.Size = new System.Drawing.Size(510, 510);
            this.pnlOut.TabIndex = 17;
            this.pnlOut.MouseUp += new System.Windows.Forms.MouseEventHandler(this.pnlOut_MouseUp);
            // 
            // label6
            // 
            this.label6.AutoSize = true;
            this.label6.Location = new System.Drawing.Point(132, 33);
            this.label6.Name = "label6";
            this.label6.Size = new System.Drawing.Size(75, 13);
            this.label6.TabIndex = 21;
            this.label6.Text = "Line thickness";
            // 
            // txtPSFLineThickness
            // 
            this.txtPSFLineThickness.Location = new System.Drawing.Point(211, 30);
            this.txtPSFLineThickness.Name = "txtPSFLineThickness";
            this.txtPSFLineThickness.Size = new System.Drawing.Size(14, 20);
            this.txtPSFLineThickness.TabIndex = 67;
            this.txtPSFLineThickness.Text = "1";
            // 
            // label7
            // 
            this.label7.AutoSize = true;
            this.label7.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label7.Location = new System.Drawing.Point(6, 17);
            this.label7.Name = "label7";
            this.label7.Size = new System.Drawing.Size(45, 13);
            this.label7.TabIndex = 25;
            this.label7.Text = "FWHM";
            // 
            // label8
            // 
            this.label8.AutoSize = true;
            this.label8.Location = new System.Drawing.Point(16, 15);
            this.label8.Name = "label8";
            this.label8.Size = new System.Drawing.Size(43, 13);
            this.label8.TabIndex = 27;
            this.label8.Text = "Feather";
            // 
            // txtFeather
            // 
            this.txtFeather.Location = new System.Drawing.Point(95, 9);
            this.txtFeather.Name = "txtFeather";
            this.txtFeather.Size = new System.Drawing.Size(17, 20);
            this.txtFeather.TabIndex = 154;
            this.txtFeather.Text = "0";
            this.txtFeather.TextChanged += new System.EventHandler(this.txtFeather_TextChanged);
            // 
            // chkAntiAliasLine
            // 
            this.chkAntiAliasLine.AutoSize = true;
            this.chkAntiAliasLine.Location = new System.Drawing.Point(6, 32);
            this.chkAntiAliasLine.Name = "chkAntiAliasLine";
            this.chkAntiAliasLine.Size = new System.Drawing.Size(128, 17);
            this.chkAntiAliasLine.TabIndex = 67;
            this.chkAntiAliasLine.Text = "Make line anti-aliased";
            this.chkAntiAliasLine.UseVisualStyleBackColor = true;
            // 
            // picFilterFT
            // 
            this.picFilterFT.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.picFilterFT.Location = new System.Drawing.Point(805, 512);
            this.picFilterFT.Name = "picFilterFT";
            this.picFilterFT.Size = new System.Drawing.Size(140, 140);
            this.picFilterFT.SizeMode = System.Windows.Forms.PictureBoxSizeMode.StretchImage;
            this.picFilterFT.TabIndex = 33;
            this.picFilterFT.TabStop = false;
            // 
            // txtCLS_Y
            // 
            this.txtCLS_Y.Location = new System.Drawing.Point(195, 32);
            this.txtCLS_Y.Name = "txtCLS_Y";
            this.txtCLS_Y.Size = new System.Drawing.Size(46, 20);
            this.txtCLS_Y.TabIndex = 27;
            this.txtCLS_Y.Text = "0.005";
            this.tip.SetToolTip(this.txtCLS_Y, "Regularisation parameter (similar value to NSR)");
            // 
            // txtIterations
            // 
            this.txtIterations.Location = new System.Drawing.Point(214, 55);
            this.txtIterations.Name = "txtIterations";
            this.txtIterations.Size = new System.Drawing.Size(27, 20);
            this.txtIterations.TabIndex = 28;
            this.txtIterations.Text = "100";
            this.tip.SetToolTip(this.txtIterations, "Number of iterations for Tot Variation and RL deconvolution");
            // 
            // btnStopIterations
            // 
            this.btnStopIterations.Location = new System.Drawing.Point(165, 78);
            this.btnStopIterations.Name = "btnStopIterations";
            this.btnStopIterations.Size = new System.Drawing.Size(78, 25);
            this.btnStopIterations.TabIndex = 28;
            this.btnStopIterations.Text = "Stop";
            this.btnStopIterations.UseVisualStyleBackColor = true;
            this.btnStopIterations.Click += new System.EventHandler(this.btnStopIterations_Click);
            // 
            // lblIterationInfo
            // 
            this.lblIterationInfo.AutoSize = true;
            this.lblIterationInfo.Location = new System.Drawing.Point(92, 93);
            this.lblIterationInfo.Name = "lblIterationInfo";
            this.lblIterationInfo.Size = new System.Drawing.Size(19, 13);
            this.lblIterationInfo.TabIndex = 43;
            this.lblIterationInfo.Text = "...";
            // 
            // btnFFTW
            // 
            this.btnFFTW.Location = new System.Drawing.Point(902, 149);
            this.btnFFTW.Name = "btnFFTW";
            this.btnFFTW.Size = new System.Drawing.Size(47, 36);
            this.btnFFTW.TabIndex = 44;
            this.btnFFTW.Text = "FFTW Test";
            this.btnFFTW.UseVisualStyleBackColor = true;
            this.btnFFTW.Visible = false;
            this.btnFFTW.Click += new System.EventHandler(this.btnFFTW_Click);
            // 
            // grpMethod
            // 
            this.grpMethod.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(192)))), ((int)(((byte)(255)))), ((int)(((byte)(192)))));
            this.grpMethod.Controls.Add(this.optRIF);
            this.grpMethod.Controls.Add(this.label32);
            this.grpMethod.Controls.Add(this.label25);
            this.grpMethod.Controls.Add(this.label20);
            this.grpMethod.Controls.Add(this.label19);
            this.grpMethod.Controls.Add(this.label18);
            this.grpMethod.Controls.Add(this.label17);
            this.grpMethod.Controls.Add(this.optLR);
            this.grpMethod.Controls.Add(this.trkBlur);
            this.grpMethod.Controls.Add(this.trkIterations);
            this.grpMethod.Controls.Add(this.optLandweber);
            this.grpMethod.Controls.Add(this.optTikhonov);
            this.grpMethod.Controls.Add(this.lblIterationInfo);
            this.grpMethod.Controls.Add(this.optWiener);
            this.grpMethod.Controls.Add(this.txtIterations);
            this.grpMethod.Controls.Add(this.btnStopIterations);
            this.grpMethod.Controls.Add(this.txtCLS_Y);
            this.grpMethod.Controls.Add(this.txtNSR);
            this.grpMethod.Controls.Add(this.label12);
            this.grpMethod.Controls.Add(this.label3);
            this.grpMethod.Controls.Add(this.optCustomRepair);
            this.grpMethod.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.grpMethod.ForeColor = System.Drawing.Color.DarkGreen;
            this.grpMethod.Location = new System.Drawing.Point(180, 0);
            this.grpMethod.Name = "grpMethod";
            this.grpMethod.Size = new System.Drawing.Size(302, 120);
            this.grpMethod.TabIndex = 45;
            this.grpMethod.TabStop = false;
            this.grpMethod.Text = "DECONVOLUTION REPAIR METHOD";
            // 
            // optRIF
            // 
            this.optRIF.AutoSize = true;
            this.optRIF.Font = new System.Drawing.Font("Microsoft Sans Serif", 6.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.optRIF.Location = new System.Drawing.Point(117, 14);
            this.optRIF.Name = "optRIF";
            this.optRIF.Size = new System.Drawing.Size(49, 16);
            this.optRIF.TabIndex = 25;
            this.optRIF.Text = "RIF ->";
            this.tip.SetToolTip(this.optRIF, "Regularised inverse filter");
            this.optRIF.UseVisualStyleBackColor = true;
            this.optRIF.CheckedChanged += new System.EventHandler(this.optRIF_CheckedChanged);
            // 
            // label32
            // 
            this.label32.AutoSize = true;
            this.label32.Font = new System.Drawing.Font("Microsoft Sans Serif", 7F, System.Drawing.FontStyle.Italic, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label32.Location = new System.Drawing.Point(56, 49);
            this.label32.Name = "label32";
            this.label32.Size = new System.Drawing.Size(90, 13);
            this.label32.TabIndex = 53;
            this.label32.Text = "Using iterations ...";
            // 
            // label25
            // 
            this.label25.AutoSize = true;
            this.label25.Location = new System.Drawing.Point(1, 13);
            this.label25.Name = "label25";
            this.label25.Size = new System.Drawing.Size(54, 13);
            this.label25.TabIndex = 52;
            this.label25.Text = "BLUR ▲";
            // 
            // label20
            // 
            this.label20.AutoSize = true;
            this.label20.Font = new System.Drawing.Font("Microsoft Sans Serif", 6.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label20.Location = new System.Drawing.Point(250, 11);
            this.label20.Name = "label20";
            this.label20.Size = new System.Drawing.Size(41, 24);
            this.label20.TabIndex = 51;
            this.label20.Text = "Iteration\r\nprogress";
            // 
            // label19
            // 
            this.label19.AutoSize = true;
            this.label19.Location = new System.Drawing.Point(151, 59);
            this.label19.Name = "label19";
            this.label19.Size = new System.Drawing.Size(60, 13);
            this.label19.TabIndex = 50;
            this.label19.Text = "Iterations";
            // 
            // label18
            // 
            this.label18.AutoSize = true;
            this.label18.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, ((System.Drawing.FontStyle)((System.Drawing.FontStyle.Bold | System.Drawing.FontStyle.Underline))), System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label18.Location = new System.Drawing.Point(3, 102);
            this.label18.Name = "label18";
            this.label18.Size = new System.Drawing.Size(52, 13);
            this.label18.TabIndex = 49;
            this.label18.Text = "Noisy ▼";
            this.tip.SetToolTip(this.label18, "More detail but more NOISE");
            // 
            // label17
            // 
            this.label17.AutoSize = true;
            this.label17.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, ((System.Drawing.FontStyle)((System.Drawing.FontStyle.Bold | System.Drawing.FontStyle.Underline))), System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label17.Location = new System.Drawing.Point(1, 25);
            this.label17.Name = "label17";
            this.label17.Size = new System.Drawing.Size(49, 13);
            this.label17.TabIndex = 48;
            this.label17.Text = "Smooth";
            this.tip.SetToolTip(this.label17, "SMOOTHER image but less detail");
            // 
            // optLR
            // 
            this.optLR.AutoSize = true;
            this.optLR.Font = new System.Drawing.Font("Microsoft Sans Serif", 6.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.optLR.Location = new System.Drawing.Point(55, 76);
            this.optLR.Name = "optLR";
            this.optLR.Size = new System.Drawing.Size(93, 16);
            this.optLR.TabIndex = 24;
            this.optLR.Text = "Richardson-Lucy";
            this.tip.SetToolTip(this.optLR, "Deconvolution by Richardson-Lucy algorithm");
            this.optLR.UseVisualStyleBackColor = true;
            // 
            // trkBlur
            // 
            this.trkBlur.LargeChange = 10;
            this.trkBlur.Location = new System.Drawing.Point(3, 33);
            this.trkBlur.Maximum = 100;
            this.trkBlur.Name = "trkBlur";
            this.trkBlur.Orientation = System.Windows.Forms.Orientation.Vertical;
            this.trkBlur.Size = new System.Drawing.Size(45, 71);
            this.trkBlur.TabIndex = 45;
            this.trkBlur.TickFrequency = 20;
            this.tip.SetToolTip(this.trkBlur, "Control noise (Wiener and Tikhonov only) - LOWER means sharper but noisier");
            this.trkBlur.Value = 40;
            this.trkBlur.Scroll += new System.EventHandler(this.trkBlur_Scroll);
            this.trkBlur.ValueChanged += new System.EventHandler(this.trkBlur_ValueChanged);
            this.trkBlur.KeyUp += new System.Windows.Forms.KeyEventHandler(this.trkBlur_KeyUp);
            this.trkBlur.MouseUp += new System.Windows.Forms.MouseEventHandler(this.trkBlur_MouseUp);
            // 
            // trkIterations
            // 
            this.trkIterations.Location = new System.Drawing.Point(254, 36);
            this.trkIterations.Name = "trkIterations";
            this.trkIterations.Orientation = System.Windows.Forms.Orientation.Vertical;
            this.trkIterations.Size = new System.Drawing.Size(45, 78);
            this.trkIterations.TabIndex = 29;
            this.tip.SetToolTip(this.trkIterations, "Display intermediate iterations for Tot variation and Richardson-Lucy");
            this.trkIterations.Scroll += new System.EventHandler(this.trkIterations_Scroll);
            this.trkIterations.KeyUp += new System.Windows.Forms.KeyEventHandler(this.trkIterations_KeyUp);
            this.trkIterations.MouseUp += new System.Windows.Forms.MouseEventHandler(this.trkIterations_MouseUp);
            // 
            // optLandweber
            // 
            this.optLandweber.AutoSize = true;
            this.optLandweber.Font = new System.Drawing.Font("Microsoft Sans Serif", 6.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.optLandweber.Location = new System.Drawing.Point(55, 62);
            this.optLandweber.Name = "optLandweber";
            this.optLandweber.Size = new System.Drawing.Size(69, 16);
            this.optLandweber.TabIndex = 23;
            this.optLandweber.Text = "Landweber";
            this.tip.SetToolTip(this.optLandweber, "Deconvolution by Landweber algorithm");
            this.optLandweber.UseVisualStyleBackColor = true;
            // 
            // optTikhonov
            // 
            this.optTikhonov.AutoSize = true;
            this.optTikhonov.Font = new System.Drawing.Font("Microsoft Sans Serif", 6.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.optTikhonov.Location = new System.Drawing.Point(87, 32);
            this.optTikhonov.Name = "optTikhonov";
            this.optTikhonov.Size = new System.Drawing.Size(91, 16);
            this.optTikhonov.TabIndex = 22;
            this.optTikhonov.Text = "Tikhonov - - - - ->";
            this.tip.SetToolTip(this.optTikhonov, "Tikhonov (Contrained least squares) inverse filter");
            this.optTikhonov.UseVisualStyleBackColor = true;
            this.optTikhonov.CheckedChanged += new System.EventHandler(this.optTikhonov_CheckedChanged);
            // 
            // optWiener
            // 
            this.optWiener.AutoSize = true;
            this.optWiener.Checked = true;
            this.optWiener.Font = new System.Drawing.Font("Microsoft Sans Serif", 6.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.optWiener.Location = new System.Drawing.Point(56, 14);
            this.optWiener.Name = "optWiener";
            this.optWiener.Size = new System.Drawing.Size(62, 16);
            this.optWiener.TabIndex = 21;
            this.optWiener.TabStop = true;
            this.optWiener.Text = "Wiener ->";
            this.tip.SetToolTip(this.optWiener, "Wiener inverse filter");
            this.optWiener.UseVisualStyleBackColor = true;
            this.optWiener.CheckedChanged += new System.EventHandler(this.optWiener_CheckedChanged);
            // 
            // label12
            // 
            this.label12.AutoSize = true;
            this.label12.Location = new System.Drawing.Point(179, 35);
            this.label12.Name = "label12";
            this.label12.Size = new System.Drawing.Size(15, 13);
            this.label12.TabIndex = 6;
            this.label12.Text = "Y";
            // 
            // optCustomRepair
            // 
            this.optCustomRepair.AutoSize = true;
            this.optCustomRepair.Font = new System.Drawing.Font("Microsoft Sans Serif", 6.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.optCustomRepair.Location = new System.Drawing.Point(56, 99);
            this.optCustomRepair.Name = "optCustomRepair";
            this.optCustomRepair.Size = new System.Drawing.Size(81, 16);
            this.optCustomRepair.TabIndex = 25;
            this.optCustomRepair.Text = "Custom repair";
            this.optCustomRepair.UseVisualStyleBackColor = true;
            this.optCustomRepair.Visible = false;
            // 
            // grpPSF
            // 
            this.grpPSF.BackColor = System.Drawing.SystemColors.ControlLight;
            this.grpPSF.Controls.Add(this.udWave);
            this.grpPSF.Controls.Add(this.label30);
            this.grpPSF.Controls.Add(this.optMTFPSF);
            this.grpPSF.Controls.Add(this.label15);
            this.grpPSF.Controls.Add(this.label23);
            this.grpPSF.Controls.Add(this.udGaussFraction);
            this.grpPSF.Controls.Add(this.optVoigt);
            this.grpPSF.Controls.Add(this.label22);
            this.grpPSF.Controls.Add(this.udMoffatPasses);
            this.grpPSF.Controls.Add(this.chkMoffatInPasses);
            this.grpPSF.Controls.Add(this.udCropPSF);
            this.grpPSF.Controls.Add(this.chkCropPSF);
            this.grpPSF.Controls.Add(this.udBrightness);
            this.grpPSF.Controls.Add(this.udFWHM);
            this.grpPSF.Controls.Add(this.udMoffatBeta);
            this.grpPSF.Controls.Add(this.label13);
            this.grpPSF.Controls.Add(this.optMoffat);
            this.grpPSF.Controls.Add(this.optGaussianDeblur);
            this.grpPSF.Controls.Add(this.optCameraCircleDeblur);
            this.grpPSF.Controls.Add(this.label7);
            this.grpPSF.ForeColor = System.Drawing.Color.DarkBlue;
            this.grpPSF.Location = new System.Drawing.Point(5, 33);
            this.grpPSF.Name = "grpPSF";
            this.grpPSF.Size = new System.Drawing.Size(275, 165);
            this.grpPSF.TabIndex = 47;
            this.grpPSF.TabStop = false;
            this.grpPSF.Text = "PSF Details";
            // 
            // udWave
            // 
            this.udWave.DecimalPlaces = 2;
            this.udWave.Increment = new decimal(new int[] {
            5,
            0,
            0,
            131072});
            this.udWave.Location = new System.Drawing.Point(223, 109);
            this.udWave.Maximum = new decimal(new int[] {
            1,
            0,
            0,
            0});
            this.udWave.Name = "udWave";
            this.udWave.Size = new System.Drawing.Size(45, 20);
            this.udWave.TabIndex = 68;
            this.tip.SetToolTip(this.udWave, "Wavefront error");
            this.udWave.Value = new decimal(new int[] {
            15,
            0,
            0,
            131072});
            this.udWave.ValueChanged += new System.EventHandler(this.udWave_ValueChanged);
            // 
            // label30
            // 
            this.label30.AutoSize = true;
            this.label30.Location = new System.Drawing.Point(184, 112);
            this.label30.Name = "label30";
            this.label30.Size = new System.Drawing.Size(36, 13);
            this.label30.TabIndex = 67;
            this.label30.Text = "Wave";
            // 
            // optMTFPSF
            // 
            this.optMTFPSF.AutoSize = true;
            this.optMTFPSF.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold);
            this.optMTFPSF.Location = new System.Drawing.Point(127, 109);
            this.optMTFPSF.Name = "optMTFPSF";
            this.optMTFPSF.Size = new System.Drawing.Size(50, 17);
            this.optMTFPSF.TabIndex = 59;
            this.optMTFPSF.TabStop = true;
            this.optMTFPSF.Text = "MTF";
            this.tip.SetToolTip(this.optMTFPSF, "Use Modulation Transfer Function");
            this.optMTFPSF.UseVisualStyleBackColor = true;
            this.optMTFPSF.CheckedChanged += new System.EventHandler(this.optMTFPSF_CheckedChanged);
            // 
            // label15
            // 
            this.label15.AutoSize = true;
            this.label15.Location = new System.Drawing.Point(5, 60);
            this.label15.Name = "label15";
            this.label15.Size = new System.Drawing.Size(126, 13);
            this.label15.TabIndex = 66;
            this.label15.Text = "CHOOSE PSF SHAPE ...";
            // 
            // label23
            // 
            this.label23.AutoSize = true;
            this.label23.Location = new System.Drawing.Point(149, 146);
            this.label23.Name = "label23";
            this.label23.Size = new System.Drawing.Size(75, 13);
            this.label23.TabIndex = 65;
            this.label23.Text = "Gauss fraction";
            // 
            // udGaussFraction
            // 
            this.udGaussFraction.DecimalPlaces = 2;
            this.udGaussFraction.Increment = new decimal(new int[] {
            1,
            0,
            0,
            65536});
            this.udGaussFraction.Location = new System.Drawing.Point(224, 142);
            this.udGaussFraction.Maximum = new decimal(new int[] {
            1,
            0,
            0,
            0});
            this.udGaussFraction.Name = "udGaussFraction";
            this.udGaussFraction.Size = new System.Drawing.Size(45, 20);
            this.udGaussFraction.TabIndex = 61;
            this.tip.SetToolTip(this.udGaussFraction, "Convolve a Lorentz with a Gaussian PSF (with the same overall FWHM)");
            this.udGaussFraction.Value = new decimal(new int[] {
            20,
            0,
            0,
            131072});
            this.udGaussFraction.ValueChanged += new System.EventHandler(this.udGaussFraction_ValueChanged);
            // 
            // optVoigt
            // 
            this.optVoigt.AutoSize = true;
            this.optVoigt.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.optVoigt.Location = new System.Drawing.Point(2, 144);
            this.optVoigt.Name = "optVoigt";
            this.optVoigt.Size = new System.Drawing.Size(153, 17);
            this.optVoigt.TabIndex = 60;
            this.optVoigt.Text = "Voigt (Lorentz**Guass)";
            this.tip.SetToolTip(this.optVoigt, "Lorentzian convolved with Guassian PSF");
            this.optVoigt.UseVisualStyleBackColor = true;
            this.optVoigt.CheckedChanged += new System.EventHandler(this.optVoigt_CheckedChanged);
            // 
            // label22
            // 
            this.label22.AutoSize = true;
            this.label22.Location = new System.Drawing.Point(162, 16);
            this.label22.Name = "label22";
            this.label22.Size = new System.Drawing.Size(56, 13);
            this.label22.TabIndex = 61;
            this.label22.Text = "Brightness";
            // 
            // udMoffatPasses
            // 
            this.udMoffatPasses.Location = new System.Drawing.Point(145, 89);
            this.udMoffatPasses.Maximum = new decimal(new int[] {
            5,
            0,
            0,
            0});
            this.udMoffatPasses.Minimum = new decimal(new int[] {
            1,
            0,
            0,
            0});
            this.udMoffatPasses.Name = "udMoffatPasses";
            this.udMoffatPasses.Size = new System.Drawing.Size(32, 20);
            this.udMoffatPasses.TabIndex = 57;
            this.udMoffatPasses.Value = new decimal(new int[] {
            2,
            0,
            0,
            0});
            this.udMoffatPasses.ValueChanged += new System.EventHandler(this.udMoffatPasses_ValueChanged);
            // 
            // chkMoffatInPasses
            // 
            this.chkMoffatInPasses.AutoSize = true;
            this.chkMoffatInPasses.Location = new System.Drawing.Point(29, 91);
            this.chkMoffatInPasses.Name = "chkMoffatInPasses";
            this.chkMoffatInPasses.Size = new System.Drawing.Size(114, 17);
            this.chkMoffatInPasses.TabIndex = 56;
            this.chkMoffatInPasses.Text = "Do multiple passes";
            this.tip.SetToolTip(this.chkMoffatInPasses, "Apply the PSF in multiple iterations (Moffat/Lorentz only)");
            this.chkMoffatInPasses.UseVisualStyleBackColor = true;
            this.chkMoffatInPasses.CheckedChanged += new System.EventHandler(this.chkMoffatInPasses_CheckedChanged);
            // 
            // udCropPSF
            // 
            this.udCropPSF.DecimalPlaces = 1;
            this.udCropPSF.Location = new System.Drawing.Point(225, 36);
            this.udCropPSF.Maximum = new decimal(new int[] {
            50,
            0,
            0,
            0});
            this.udCropPSF.Minimum = new decimal(new int[] {
            1,
            0,
            0,
            0});
            this.udCropPSF.Name = "udCropPSF";
            this.udCropPSF.Size = new System.Drawing.Size(45, 20);
            this.udCropPSF.TabIndex = 53;
            this.tip.SetToolTip(this.udCropPSF, "Taper down PSF tails quickly, increasing will increase contrast");
            this.udCropPSF.Value = new decimal(new int[] {
            100,
            0,
            0,
            65536});
            this.udCropPSF.ValueChanged += new System.EventHandler(this.udCropPSF_ValueChanged);
            // 
            // chkCropPSF
            // 
            this.chkCropPSF.AutoSize = true;
            this.chkCropPSF.Checked = true;
            this.chkCropPSF.CheckState = System.Windows.Forms.CheckState.Checked;
            this.chkCropPSF.Location = new System.Drawing.Point(5, 38);
            this.chkCropPSF.Name = "chkCropPSF";
            this.chkCropPSF.Size = new System.Drawing.Size(196, 17);
            this.chkCropPSF.TabIndex = 52;
            this.chkCropPSF.Text = "Reduce PSF tails at FWHM times ->";
            this.tip.SetToolTip(this.chkCropPSF, "Reduce tails of PSF function to zero, useful for Moffat/Lorentz");
            this.chkCropPSF.UseVisualStyleBackColor = true;
            this.chkCropPSF.CheckedChanged += new System.EventHandler(this.chkCropPSF_CheckedChanged);
            // 
            // udBrightness
            // 
            this.udBrightness.DecimalPlaces = 2;
            this.udBrightness.Increment = new decimal(new int[] {
            1,
            0,
            0,
            65536});
            this.udBrightness.Location = new System.Drawing.Point(225, 13);
            this.udBrightness.Maximum = new decimal(new int[] {
            3,
            0,
            0,
            0});
            this.udBrightness.Minimum = new decimal(new int[] {
            1,
            0,
            0,
            65536});
            this.udBrightness.Name = "udBrightness";
            this.udBrightness.Size = new System.Drawing.Size(45, 20);
            this.udBrightness.TabIndex = 51;
            this.tip.SetToolTip(this.udBrightness, "Decrease if any areas are over-exposed");
            this.udBrightness.Value = new decimal(new int[] {
            10,
            0,
            0,
            65536});
            this.udBrightness.ValueChanged += new System.EventHandler(this.udBrightness_ValueChanged);
            // 
            // udFWHM
            // 
            this.udFWHM.DecimalPlaces = 2;
            this.udFWHM.Font = new System.Drawing.Font("Microsoft Sans Serif", 9.75F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.udFWHM.Increment = new decimal(new int[] {
            5,
            0,
            0,
            65536});
            this.udFWHM.Location = new System.Drawing.Point(60, 13);
            this.udFWHM.Minimum = new decimal(new int[] {
            2,
            0,
            0,
            65536});
            this.udFWHM.Name = "udFWHM";
            this.udFWHM.Size = new System.Drawing.Size(50, 22);
            this.udFWHM.TabIndex = 50;
            this.tip.SetToolTip(this.udFWHM, "Full width at half maximum on PSF (in pixels)");
            this.udFWHM.Value = new decimal(new int[] {
            40,
            0,
            0,
            65536});
            this.udFWHM.ValueChanged += new System.EventHandler(this.udFWHM_ValueChanged);
            // 
            // udMoffatBeta
            // 
            this.udMoffatBeta.DecimalPlaces = 2;
            this.udMoffatBeta.Increment = new decimal(new int[] {
            1,
            0,
            0,
            65536});
            this.udMoffatBeta.Location = new System.Drawing.Point(226, 74);
            this.udMoffatBeta.Maximum = new decimal(new int[] {
            10,
            0,
            0,
            0});
            this.udMoffatBeta.Minimum = new decimal(new int[] {
            1,
            0,
            0,
            65536});
            this.udMoffatBeta.Name = "udMoffatBeta";
            this.udMoffatBeta.Size = new System.Drawing.Size(42, 20);
            this.udMoffatBeta.TabIndex = 55;
            this.tip.SetToolTip(this.udMoffatBeta, "Moffat curve fitting parameter ");
            this.udMoffatBeta.Value = new decimal(new int[] {
            1,
            0,
            0,
            0});
            this.udMoffatBeta.ValueChanged += new System.EventHandler(this.udMoffatBeta_ValueChanged);
            // 
            // label13
            // 
            this.label13.AutoSize = true;
            this.label13.Location = new System.Drawing.Point(211, 76);
            this.label13.Name = "label13";
            this.label13.Size = new System.Drawing.Size(13, 13);
            this.label13.TabIndex = 51;
            this.label13.Text = "β";
            // 
            // optMoffat
            // 
            this.optMoffat.AutoSize = true;
            this.optMoffat.Checked = true;
            this.optMoffat.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.optMoffat.Location = new System.Drawing.Point(2, 72);
            this.optMoffat.Name = "optMoffat";
            this.optMoffat.Size = new System.Drawing.Size(117, 17);
            this.optMoffat.TabIndex = 54;
            this.optMoffat.TabStop = true;
            this.optMoffat.Text = "Lorentz / Moffat";
            this.tip.SetToolTip(this.optMoffat, "Lorentz when β=1, otherwise a Moffat PSF");
            this.optMoffat.UseVisualStyleBackColor = true;
            this.optMoffat.CheckedChanged += new System.EventHandler(this.optMoffat_CheckedChanged);
            // 
            // optGaussianDeblur
            // 
            this.optGaussianDeblur.AutoSize = true;
            this.optGaussianDeblur.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.optGaussianDeblur.Location = new System.Drawing.Point(3, 109);
            this.optGaussianDeblur.Name = "optGaussianDeblur";
            this.optGaussianDeblur.Size = new System.Drawing.Size(77, 17);
            this.optGaussianDeblur.TabIndex = 58;
            this.optGaussianDeblur.Text = "Gaussian";
            this.optGaussianDeblur.UseVisualStyleBackColor = true;
            this.optGaussianDeblur.CheckedChanged += new System.EventHandler(this.optGaussianDeblur_CheckedChanged);
            // 
            // optCameraCircleDeblur
            // 
            this.optCameraCircleDeblur.AutoSize = true;
            this.optCameraCircleDeblur.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.optCameraCircleDeblur.Location = new System.Drawing.Point(3, 126);
            this.optCameraCircleDeblur.Name = "optCameraCircleDeblur";
            this.optCameraCircleDeblur.Size = new System.Drawing.Size(183, 17);
            this.optCameraCircleDeblur.TabIndex = 59;
            this.optCameraCircleDeblur.Text = "Camera incorrect lens focus";
            this.optCameraCircleDeblur.UseVisualStyleBackColor = true;
            this.optCameraCircleDeblur.CheckedChanged += new System.EventHandler(this.optCameraCircleDeblur_CheckedChanged);
            // 
            // chkAutoDeblur
            // 
            this.chkAutoDeblur.AutoSize = true;
            this.chkAutoDeblur.Checked = true;
            this.chkAutoDeblur.CheckState = System.Windows.Forms.CheckState.Checked;
            this.chkAutoDeblur.Location = new System.Drawing.Point(2, 52);
            this.chkAutoDeblur.Name = "chkAutoDeblur";
            this.chkAutoDeblur.Size = new System.Drawing.Size(103, 17);
            this.chkAutoDeblur.TabIndex = 4;
            this.chkAutoDeblur.Text = "Auto recalculate";
            this.tip.SetToolTip(this.chkAutoDeblur, "Update image every time something is changed");
            this.chkAutoDeblur.UseVisualStyleBackColor = true;
            // 
            // grpEdgeRepair
            // 
            this.grpEdgeRepair.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.grpEdgeRepair.Controls.Add(this.udDeringRepairStrength);
            this.grpEdgeRepair.Controls.Add(this.udDeringStarThreshold);
            this.grpEdgeRepair.Controls.Add(this.chkDeringing);
            this.grpEdgeRepair.Controls.Add(this.chkRepairTopBottom);
            this.grpEdgeRepair.Controls.Add(this.chkRepairEdges);
            this.grpEdgeRepair.ForeColor = System.Drawing.Color.Olive;
            this.grpEdgeRepair.Location = new System.Drawing.Point(748, 0);
            this.grpEdgeRepair.Name = "grpEdgeRepair";
            this.grpEdgeRepair.Size = new System.Drawing.Size(139, 86);
            this.grpEdgeRepair.TabIndex = 49;
            this.grpEdgeRepair.TabStop = false;
            this.grpEdgeRepair.Text = "COSMETIC REPAIRS";
            // 
            // udDeringRepairStrength
            // 
            this.udDeringRepairStrength.DecimalPlaces = 3;
            this.udDeringRepairStrength.Increment = new decimal(new int[] {
            2,
            0,
            0,
            196608});
            this.udDeringRepairStrength.Location = new System.Drawing.Point(74, 62);
            this.udDeringRepairStrength.Maximum = new decimal(new int[] {
            2,
            0,
            0,
            65536});
            this.udDeringRepairStrength.Name = "udDeringRepairStrength";
            this.udDeringRepairStrength.Size = new System.Drawing.Size(50, 20);
            this.udDeringRepairStrength.TabIndex = 156;
            this.tip.SetToolTip(this.udDeringRepairStrength, "Smaller = Repair further out from star centres");
            this.udDeringRepairStrength.Value = new decimal(new int[] {
            1,
            0,
            0,
            131072});
            // 
            // udDeringStarThreshold
            // 
            this.udDeringStarThreshold.DecimalPlaces = 2;
            this.udDeringStarThreshold.Increment = new decimal(new int[] {
            4,
            0,
            0,
            131072});
            this.udDeringStarThreshold.Location = new System.Drawing.Point(20, 61);
            this.udDeringStarThreshold.Maximum = new decimal(new int[] {
            1,
            0,
            0,
            0});
            this.udDeringStarThreshold.Name = "udDeringStarThreshold";
            this.udDeringStarThreshold.Size = new System.Drawing.Size(42, 20);
            this.udDeringStarThreshold.TabIndex = 155;
            this.tip.SetToolTip(this.udDeringStarThreshold, "Smaller = Repair more stars");
            this.udDeringStarThreshold.Value = new decimal(new int[] {
            6,
            0,
            0,
            131072});
            // 
            // chkDeringing
            // 
            this.chkDeringing.AutoSize = true;
            this.chkDeringing.Font = new System.Drawing.Font("Microsoft Sans Serif", 6.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.chkDeringing.Location = new System.Drawing.Point(1, 46);
            this.chkDeringing.Name = "chkDeringing";
            this.chkDeringing.Size = new System.Drawing.Size(109, 16);
            this.chkDeringing.TabIndex = 154;
            this.chkDeringing.Text = "Motion blur deringing";
            this.tip.SetToolTip(this.chkDeringing, "Repair light and dark restoration artifacts near brighter stars");
            this.chkDeringing.UseVisualStyleBackColor = true;
            // 
            // chkRepairTopBottom
            // 
            this.chkRepairTopBottom.AutoSize = true;
            this.chkRepairTopBottom.Font = new System.Drawing.Font("Microsoft Sans Serif", 6.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.chkRepairTopBottom.Location = new System.Drawing.Point(30, 30);
            this.chkRepairTopBottom.Name = "chkRepairTopBottom";
            this.chkRepairTopBottom.Size = new System.Drawing.Size(76, 16);
            this.chkRepairTopBottom.TabIndex = 153;
            this.chkRepairTopBottom.Text = "Inc top/botm";
            this.tip.SetToolTip(this.chkRepairTopBottom, "Include top and bottom borders");
            this.chkRepairTopBottom.UseVisualStyleBackColor = true;
            // 
            // grpMotionBlurDetails
            // 
            this.grpMotionBlurDetails.BackColor = System.Drawing.SystemColors.ControlLight;
            this.grpMotionBlurDetails.Controls.Add(this.chkRotateImage);
            this.grpMotionBlurDetails.Controls.Add(this.udCentreFieldRotationY);
            this.grpMotionBlurDetails.Controls.Add(this.udCentreFieldRotationX);
            this.grpMotionBlurDetails.Controls.Add(this.chkFiledRotationDeblur);
            this.grpMotionBlurDetails.Controls.Add(this.udRotationTiles);
            this.grpMotionBlurDetails.Controls.Add(this.udMotionBlurAngle);
            this.grpMotionBlurDetails.Controls.Add(this.udMotionBlurLength);
            this.grpMotionBlurDetails.Controls.Add(this.udFieldRotationAngle);
            this.grpMotionBlurDetails.Controls.Add(this.label2);
            this.grpMotionBlurDetails.Controls.Add(this.label27);
            this.grpMotionBlurDetails.Controls.Add(this.label1);
            this.grpMotionBlurDetails.Controls.Add(this.label6);
            this.grpMotionBlurDetails.Controls.Add(this.txtPSFLineThickness);
            this.grpMotionBlurDetails.Controls.Add(this.chkAntiAliasLine);
            this.grpMotionBlurDetails.Enabled = false;
            this.grpMotionBlurDetails.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(192)))), ((int)(((byte)(64)))), ((int)(((byte)(0)))));
            this.grpMotionBlurDetails.Location = new System.Drawing.Point(6, 405);
            this.grpMotionBlurDetails.Name = "grpMotionBlurDetails";
            this.grpMotionBlurDetails.Size = new System.Drawing.Size(273, 72);
            this.grpMotionBlurDetails.TabIndex = 50;
            this.grpMotionBlurDetails.TabStop = false;
            this.grpMotionBlurDetails.Text = "Motion blur details";
            // 
            // chkRotateImage
            // 
            this.chkRotateImage.AutoSize = true;
            this.chkRotateImage.Location = new System.Drawing.Point(185, 12);
            this.chkRotateImage.Name = "chkRotateImage";
            this.chkRotateImage.Size = new System.Drawing.Size(77, 17);
            this.chkRotateImage.TabIndex = 67;
            this.chkRotateImage.Text = "Rotate img";
            this.tip.SetToolTip(this.chkRotateImage, "Rotate image, then apply deconvolution");
            this.chkRotateImage.UseVisualStyleBackColor = true;
            // 
            // udCentreFieldRotationY
            // 
            this.udCentreFieldRotationY.Font = new System.Drawing.Font("Microsoft Sans Serif", 6.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.udCentreFieldRotationY.Increment = new decimal(new int[] {
            5,
            0,
            0,
            0});
            this.udCentreFieldRotationY.Location = new System.Drawing.Point(228, 51);
            this.udCentreFieldRotationY.Maximum = new decimal(new int[] {
            10000,
            0,
            0,
            0});
            this.udCentreFieldRotationY.Minimum = new decimal(new int[] {
            1,
            0,
            0,
            -2147483648});
            this.udCentreFieldRotationY.Name = "udCentreFieldRotationY";
            this.udCentreFieldRotationY.Size = new System.Drawing.Size(39, 18);
            this.udCentreFieldRotationY.TabIndex = 72;
            this.tip.SetToolTip(this.udCentreFieldRotationY, "Centre (Y) of field rotation");
            this.udCentreFieldRotationY.Value = new decimal(new int[] {
            1,
            0,
            0,
            -2147483648});
            // 
            // udCentreFieldRotationX
            // 
            this.udCentreFieldRotationX.Font = new System.Drawing.Font("Microsoft Sans Serif", 6.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.udCentreFieldRotationX.Increment = new decimal(new int[] {
            5,
            0,
            0,
            0});
            this.udCentreFieldRotationX.Location = new System.Drawing.Point(181, 51);
            this.udCentreFieldRotationX.Maximum = new decimal(new int[] {
            10000,
            0,
            0,
            0});
            this.udCentreFieldRotationX.Minimum = new decimal(new int[] {
            1,
            0,
            0,
            -2147483648});
            this.udCentreFieldRotationX.Name = "udCentreFieldRotationX";
            this.udCentreFieldRotationX.Size = new System.Drawing.Size(39, 18);
            this.udCentreFieldRotationX.TabIndex = 71;
            this.tip.SetToolTip(this.udCentreFieldRotationX, "Centre (X) of field rotation (-1 means centre)");
            this.udCentreFieldRotationX.Value = new decimal(new int[] {
            1,
            0,
            0,
            -2147483648});
            // 
            // chkFiledRotationDeblur
            // 
            this.chkFiledRotationDeblur.AutoSize = true;
            this.chkFiledRotationDeblur.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.chkFiledRotationDeblur.Location = new System.Drawing.Point(4, 51);
            this.chkFiledRotationDeblur.Name = "chkFiledRotationDeblur";
            this.chkFiledRotationDeblur.Size = new System.Drawing.Size(90, 17);
            this.chkFiledRotationDeblur.TabIndex = 68;
            this.chkFiledRotationDeblur.Text = "ROTATION";
            this.tip.SetToolTip(this.chkFiledRotationDeblur, "Remove field rotation");
            this.chkFiledRotationDeblur.UseVisualStyleBackColor = true;
            this.chkFiledRotationDeblur.CheckedChanged += new System.EventHandler(this.chkFiledRotationDeblur_CheckedChanged);
            // 
            // udRotationTiles
            // 
            this.udRotationTiles.Location = new System.Drawing.Point(234, 29);
            this.udRotationTiles.Minimum = new decimal(new int[] {
            2,
            0,
            0,
            0});
            this.udRotationTiles.Name = "udRotationTiles";
            this.udRotationTiles.Size = new System.Drawing.Size(33, 20);
            this.udRotationTiles.TabIndex = 70;
            this.udRotationTiles.Value = new decimal(new int[] {
            5,
            0,
            0,
            0});
            this.udRotationTiles.Visible = false;
            // 
            // udMotionBlurAngle
            // 
            this.udMotionBlurAngle.DecimalPlaces = 1;
            this.udMotionBlurAngle.Location = new System.Drawing.Point(133, 12);
            this.udMotionBlurAngle.Maximum = new decimal(new int[] {
            360,
            0,
            0,
            0});
            this.udMotionBlurAngle.Minimum = new decimal(new int[] {
            360,
            0,
            0,
            -2147483648});
            this.udMotionBlurAngle.Name = "udMotionBlurAngle";
            this.udMotionBlurAngle.Size = new System.Drawing.Size(45, 20);
            this.udMotionBlurAngle.TabIndex = 66;
            this.udMotionBlurAngle.ValueChanged += new System.EventHandler(this.udMotionBlurAngle_ValueChanged);
            // 
            // udMotionBlurLength
            // 
            this.udMotionBlurLength.DecimalPlaces = 1;
            this.udMotionBlurLength.Location = new System.Drawing.Point(41, 12);
            this.udMotionBlurLength.Maximum = new decimal(new int[] {
            1000,
            0,
            0,
            0});
            this.udMotionBlurLength.Minimum = new decimal(new int[] {
            1,
            0,
            0,
            65536});
            this.udMotionBlurLength.Name = "udMotionBlurLength";
            this.udMotionBlurLength.Size = new System.Drawing.Size(55, 20);
            this.udMotionBlurLength.TabIndex = 65;
            this.udMotionBlurLength.Value = new decimal(new int[] {
            100,
            0,
            0,
            65536});
            this.udMotionBlurLength.ValueChanged += new System.EventHandler(this.udMotionBlurLength_ValueChanged);
            // 
            // udFieldRotationAngle
            // 
            this.udFieldRotationAngle.DecimalPlaces = 2;
            this.udFieldRotationAngle.Increment = new decimal(new int[] {
            5,
            0,
            0,
            131072});
            this.udFieldRotationAngle.Location = new System.Drawing.Point(126, 50);
            this.udFieldRotationAngle.Maximum = new decimal(new int[] {
            360,
            0,
            0,
            0});
            this.udFieldRotationAngle.Name = "udFieldRotationAngle";
            this.udFieldRotationAngle.Size = new System.Drawing.Size(48, 20);
            this.udFieldRotationAngle.TabIndex = 69;
            this.tip.SetToolTip(this.udFieldRotationAngle, "Field rotated by degrees");
            this.udFieldRotationAngle.Value = new decimal(new int[] {
            1000,
            0,
            0,
            131072});
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(96, 15);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(34, 13);
            this.label2.TabIndex = 50;
            this.label2.Text = "Angle";
            // 
            // label27
            // 
            this.label27.AutoSize = true;
            this.label27.Location = new System.Drawing.Point(91, 52);
            this.label27.Name = "label27";
            this.label27.Size = new System.Drawing.Size(34, 13);
            this.label27.TabIndex = 67;
            this.label27.Text = "Angle";
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(4, 15);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(40, 13);
            this.label1.TabIndex = 51;
            this.label1.Text = "Length";
            // 
            // label14
            // 
            this.label14.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.label14.AutoSize = true;
            this.label14.Location = new System.Drawing.Point(809, 133);
            this.label14.Name = "label14";
            this.label14.Size = new System.Drawing.Size(86, 13);
            this.label14.TabIndex = 53;
            this.label14.Text = "PSF 2D Intensity";
            // 
            // grpPSFModify
            // 
            this.grpPSFModify.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.grpPSFModify.Controls.Add(this.label8);
            this.grpPSFModify.Controls.Add(this.txtFeather);
            this.grpPSFModify.ForeColor = System.Drawing.Color.Teal;
            this.grpPSFModify.Location = new System.Drawing.Point(750, 86);
            this.grpPSFModify.Name = "grpPSFModify";
            this.grpPSFModify.Size = new System.Drawing.Size(139, 33);
            this.grpPSFModify.TabIndex = 55;
            this.grpPSFModify.TabStop = false;
            this.grpPSFModify.Text = "PSF Smooth";
            // 
            // txtPSFDump
            // 
            this.txtPSFDump.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.txtPSFDump.Location = new System.Drawing.Point(807, 292);
            this.txtPSFDump.Multiline = true;
            this.txtPSFDump.Name = "txtPSFDump";
            this.txtPSFDump.ScrollBars = System.Windows.Forms.ScrollBars.Vertical;
            this.txtPSFDump.Size = new System.Drawing.Size(125, 128);
            this.txtPSFDump.TabIndex = 57;
            this.txtPSFDump.Visible = false;
            this.txtPSFDump.WordWrap = false;
            // 
            // btnCopyPSF
            // 
            this.btnCopyPSF.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.btnCopyPSF.Location = new System.Drawing.Point(810, 421);
            this.btnCopyPSF.Name = "btnCopyPSF";
            this.btnCopyPSF.Size = new System.Drawing.Size(125, 23);
            this.btnCopyPSF.TabIndex = 160;
            this.btnCopyPSF.Text = "Copy to clipboard";
            this.btnCopyPSF.UseVisualStyleBackColor = true;
            this.btnCopyPSF.Visible = false;
            this.btnCopyPSF.Click += new System.EventHandler(this.btnCopyPSF_Click);
            // 
            // btnSave
            // 
            this.btnSave.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.btnSave.BackColor = System.Drawing.Color.PaleGreen;
            this.btnSave.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.btnSave.Location = new System.Drawing.Point(4, 651);
            this.btnSave.Name = "btnSave";
            this.btnSave.Size = new System.Drawing.Size(75, 38);
            this.btnSave.TabIndex = 110;
            this.btnSave.Text = "SAVE";
            this.btnSave.UseVisualStyleBackColor = false;
            this.btnSave.Click += new System.EventHandler(this.btnSave_Click);
            // 
            // cboHistory
            // 
            this.cboHistory.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.cboHistory.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cboHistory.DropDownWidth = 800;
            this.cboHistory.Font = new System.Drawing.Font("Microsoft Sans Serif", 9.75F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.cboHistory.FormattingEnabled = true;
            this.cboHistory.Location = new System.Drawing.Point(583, 660);
            this.cboHistory.Name = "cboHistory";
            this.cboHistory.Size = new System.Drawing.Size(279, 24);
            this.cboHistory.TabIndex = 113;
            // 
            // label16
            // 
            this.label16.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.label16.AutoSize = true;
            this.label16.Location = new System.Drawing.Point(809, 276);
            this.label16.Name = "label16";
            this.label16.Size = new System.Drawing.Size(94, 13);
            this.label16.TabIndex = 62;
            this.label16.Text = "PSF Radial values";
            this.label16.Visible = false;
            // 
            // groupBox1
            // 
            this.groupBox1.Controls.Add(this.lblLastStored);
            this.groupBox1.Controls.Add(this.optPrevious);
            this.groupBox1.Controls.Add(this.optOriginal);
            this.groupBox1.Location = new System.Drawing.Point(99, 55);
            this.groupBox1.Name = "groupBox1";
            this.groupBox1.Size = new System.Drawing.Size(75, 61);
            this.groupBox1.TabIndex = 63;
            this.groupBox1.TabStop = false;
            this.groupBox1.Text = "Repair image";
            // 
            // lblLastStored
            // 
            this.lblLastStored.AutoSize = true;
            this.lblLastStored.Location = new System.Drawing.Point(1, 37);
            this.lblLastStored.Name = "lblLastStored";
            this.lblLastStored.Size = new System.Drawing.Size(16, 13);
            this.lblLastStored.TabIndex = 10;
            this.lblLastStored.Text = "<-";
            this.lblLastStored.Visible = false;
            // 
            // optPrevious
            // 
            this.optPrevious.AutoSize = true;
            this.optPrevious.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.optPrevious.ForeColor = System.Drawing.SystemColors.ControlText;
            this.optPrevious.Location = new System.Drawing.Point(17, 29);
            this.optPrevious.Name = "optPrevious";
            this.optPrevious.Size = new System.Drawing.Size(54, 30);
            this.optPrevious.TabIndex = 9;
            this.optPrevious.Text = "Last \r\nstored";
            this.tip.SetToolTip(this.optPrevious, "Apply repair processing to last stored image, rather than the original loaded ima" +
        "ge");
            this.optPrevious.UseVisualStyleBackColor = true;
            this.optPrevious.Visible = false;
            // 
            // optOriginal
            // 
            this.optOriginal.AutoSize = true;
            this.optOriginal.Checked = true;
            this.optOriginal.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.optOriginal.Location = new System.Drawing.Point(3, 15);
            this.optOriginal.Name = "optOriginal";
            this.optOriginal.Size = new System.Drawing.Size(60, 17);
            this.optOriginal.TabIndex = 7;
            this.optOriginal.TabStop = true;
            this.optOriginal.Text = "Original";
            this.tip.SetToolTip(this.optOriginal, "Apply processing to original loaded image");
            this.optOriginal.UseVisualStyleBackColor = true;
            // 
            // btnConvolve
            // 
            this.btnConvolve.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.btnConvolve.Location = new System.Drawing.Point(860, 441);
            this.btnConvolve.Name = "btnConvolve";
            this.btnConvolve.Size = new System.Drawing.Size(75, 23);
            this.btnConvolve.TabIndex = 6;
            this.btnConvolve.Text = "Convolve";
            this.tip.SetToolTip(this.btnConvolve, "Use PSF to blur the image");
            this.btnConvolve.UseVisualStyleBackColor = true;
            this.btnConvolve.Click += new System.EventHandler(this.btnConvolve_Click);
            // 
            // lblProcessing
            // 
            this.lblProcessing.AutoSize = true;
            this.lblProcessing.Font = new System.Drawing.Font("Microsoft Sans Serif", 10F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lblProcessing.Location = new System.Drawing.Point(6, 121);
            this.lblProcessing.Name = "lblProcessing";
            this.lblProcessing.Size = new System.Drawing.Size(0, 17);
            this.lblProcessing.TabIndex = 65;
            // 
            // label21
            // 
            this.label21.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.label21.AutoSize = true;
            this.label21.Font = new System.Drawing.Font("Microsoft Sans Serif", 10F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label21.ForeColor = System.Drawing.SystemColors.ControlText;
            this.label21.Location = new System.Drawing.Point(82, 656);
            this.label21.Name = "label21";
            this.label21.Size = new System.Drawing.Size(79, 34);
            this.label21.TabIndex = 66;
            this.label21.Text = "Currently \r\ndisplayed";
            // 
            // cboCurrent
            // 
            this.cboCurrent.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.cboCurrent.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cboCurrent.DropDownWidth = 800;
            this.cboCurrent.Font = new System.Drawing.Font("Microsoft Sans Serif", 9.75F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.cboCurrent.FormattingEnabled = true;
            this.cboCurrent.Location = new System.Drawing.Point(160, 660);
            this.cboCurrent.Name = "cboCurrent";
            this.cboCurrent.Size = new System.Drawing.Size(299, 24);
            this.cboCurrent.TabIndex = 111;
            this.cboCurrent.SelectedIndexChanged += new System.EventHandler(this.cboCurrent_SelectedIndexChanged);
            // 
            // grpPSFRepair
            // 
            this.grpPSFRepair.Controls.Add(this.chkProcessLuminance);
            this.grpPSFRepair.Controls.Add(this.label31);
            this.grpPSFRepair.Controls.Add(this.udUpscale);
            this.grpPSFRepair.Controls.Add(this.optSharpeningLayers);
            this.grpPSFRepair.Controls.Add(this.optCircularBlur);
            this.grpPSFRepair.Controls.Add(this.optMotionBlur);
            this.grpPSFRepair.Controls.Add(this.grpLayers);
            this.grpPSFRepair.Controls.Add(this.grpMotionBlurDetails);
            this.grpPSFRepair.Controls.Add(this.cboKernelSharpeningLayers);
            this.grpPSFRepair.Controls.Add(this.grpPSF);
            this.grpPSFRepair.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(0)))), ((int)(((byte)(0)))), ((int)(((byte)(192)))));
            this.grpPSFRepair.Location = new System.Drawing.Point(0, 139);
            this.grpPSFRepair.Name = "grpPSFRepair";
            this.grpPSFRepair.Size = new System.Drawing.Size(286, 482);
            this.grpPSFRepair.TabIndex = 68;
            this.grpPSFRepair.TabStop = false;
            this.grpPSFRepair.Text = "POINT SPREAD FUNCTION (PSF) DEFINITION";
            // 
            // chkProcessLuminance
            // 
            this.chkProcessLuminance.AutoSize = true;
            this.chkProcessLuminance.Location = new System.Drawing.Point(194, 10);
            this.chkProcessLuminance.Name = "chkProcessLuminance";
            this.chkProcessLuminance.Size = new System.Drawing.Size(87, 17);
            this.chkProcessLuminance.TabIndex = 85;
            this.chkProcessLuminance.Text = "Process Lum";
            this.tip.SetToolTip(this.chkProcessLuminance, "Convert RGB image to Grayscale to deconvolve, then recombine the repaired luminan" +
        "ce layer with the colours (to use higher SNR of luminance)");
            this.chkProcessLuminance.UseVisualStyleBackColor = true;
            this.chkProcessLuminance.Visible = false;
            // 
            // label31
            // 
            this.label31.AutoSize = true;
            this.label31.Location = new System.Drawing.Point(199, 26);
            this.label31.Name = "label31";
            this.label31.Size = new System.Drawing.Size(49, 13);
            this.label31.TabIndex = 84;
            this.label31.Text = "Up-scale";
            this.tip.SetToolTip(this.label31, "Enlarge image before deconvolving");
            this.label31.Visible = false;
            // 
            // udUpscale
            // 
            this.udUpscale.Location = new System.Drawing.Point(251, 23);
            this.udUpscale.Maximum = new decimal(new int[] {
            4,
            0,
            0,
            0});
            this.udUpscale.Minimum = new decimal(new int[] {
            1,
            0,
            0,
            0});
            this.udUpscale.Name = "udUpscale";
            this.udUpscale.Size = new System.Drawing.Size(27, 20);
            this.udUpscale.TabIndex = 40;
            this.udUpscale.Value = new decimal(new int[] {
            1,
            0,
            0,
            0});
            this.udUpscale.Visible = false;
            // 
            // optSharpeningLayers
            // 
            this.optSharpeningLayers.AutoSize = true;
            this.optSharpeningLayers.Font = new System.Drawing.Font("Microsoft Sans Serif", 9F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.optSharpeningLayers.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(192)))), ((int)(((byte)(0)))), ((int)(((byte)(192)))));
            this.optSharpeningLayers.Location = new System.Drawing.Point(7, 202);
            this.optSharpeningLayers.Name = "optSharpeningLayers";
            this.optSharpeningLayers.Size = new System.Drawing.Size(170, 19);
            this.optSharpeningLayers.TabIndex = 41;
            this.optSharpeningLayers.TabStop = true;
            this.optSharpeningLayers.Text = "SHARPENING LAYERS";
            this.tip.SetToolTip(this.optSharpeningLayers, "Laplacian sharpening where each layer is the the image convolved with a Laplacian" +
        " 3x3 kernel (-1,-1,-1;-1,8,-1;-1,-1,-1), at different multiples of the image siz" +
        "e");
            this.optSharpeningLayers.UseVisualStyleBackColor = true;
            this.optSharpeningLayers.CheckedChanged += new System.EventHandler(this.optSharpeningLayers_CheckedChanged);
            // 
            // optCircularBlur
            // 
            this.optCircularBlur.AutoSize = true;
            this.optCircularBlur.Checked = true;
            this.optCircularBlur.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.optCircularBlur.ForeColor = System.Drawing.Color.DarkBlue;
            this.optCircularBlur.Location = new System.Drawing.Point(5, 15);
            this.optCircularBlur.Name = "optCircularBlur";
            this.optCircularBlur.Size = new System.Drawing.Size(172, 17);
            this.optCircularBlur.TabIndex = 39;
            this.optCircularBlur.TabStop = true;
            this.optCircularBlur.Text = "DECONVOLVE WITH PSF";
            this.optCircularBlur.UseVisualStyleBackColor = true;
            this.optCircularBlur.CheckedChanged += new System.EventHandler(this.optCircularBlur_CheckedChanged);
            // 
            // optMotionBlur
            // 
            this.optMotionBlur.AutoSize = true;
            this.optMotionBlur.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.optMotionBlur.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(192)))), ((int)(((byte)(64)))), ((int)(((byte)(0)))));
            this.optMotionBlur.Location = new System.Drawing.Point(5, 385);
            this.optMotionBlur.Name = "optMotionBlur";
            this.optMotionBlur.Size = new System.Drawing.Size(128, 17);
            this.optMotionBlur.TabIndex = 45;
            this.optMotionBlur.Text = "MOTION DEBLUR";
            this.optMotionBlur.UseVisualStyleBackColor = true;
            this.optMotionBlur.CheckedChanged += new System.EventHandler(this.optMotionBlur_CheckedChanged);
            // 
            // grpLayers
            // 
            this.grpLayers.BackColor = System.Drawing.SystemColors.ControlLight;
            this.grpLayers.Controls.Add(this.btnShowLayers);
            this.grpLayers.Controls.Add(this.udLayersImageScale);
            this.grpLayers.Controls.Add(this.label29);
            this.grpLayers.Controls.Add(this.trkLayers5);
            this.grpLayers.Controls.Add(this.udLayersScale);
            this.grpLayers.Controls.Add(this.label9);
            this.grpLayers.Controls.Add(this.btnResetLayers);
            this.grpLayers.Controls.Add(this.trkLayers2);
            this.grpLayers.Controls.Add(this.label11);
            this.grpLayers.Controls.Add(this.cboLayerSettings);
            this.grpLayers.Controls.Add(this.txtSaveSettingsName);
            this.grpLayers.Controls.Add(this.btnSaveLayersSettings);
            this.grpLayers.Controls.Add(this.udLayersNoiseControl);
            this.grpLayers.Controls.Add(this.label10);
            this.grpLayers.Controls.Add(this.chkLayers5);
            this.grpLayers.Controls.Add(this.chkLayers4);
            this.grpLayers.Controls.Add(this.chkLayers3);
            this.grpLayers.Controls.Add(this.chkLayers2);
            this.grpLayers.Controls.Add(this.chkLayers1);
            this.grpLayers.Controls.Add(this.chkLayers0);
            this.grpLayers.Controls.Add(this.trkLayers1);
            this.grpLayers.Controls.Add(this.trkLayers0);
            this.grpLayers.Controls.Add(this.trkLayers4);
            this.grpLayers.Controls.Add(this.trkLayers3);
            this.grpLayers.Enabled = false;
            this.grpLayers.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(192)))), ((int)(((byte)(0)))), ((int)(((byte)(192)))));
            this.grpLayers.Location = new System.Drawing.Point(7, 223);
            this.grpLayers.Name = "grpLayers";
            this.grpLayers.Size = new System.Drawing.Size(274, 163);
            this.grpLayers.TabIndex = 82;
            this.grpLayers.TabStop = false;
            this.grpLayers.Text = "LAYERS (pixels)";
            // 
            // btnShowLayers
            // 
            this.btnShowLayers.Location = new System.Drawing.Point(197, 119);
            this.btnShowLayers.Name = "btnShowLayers";
            this.btnShowLayers.Size = new System.Drawing.Size(22, 22);
            this.btnShowLayers.TabIndex = 104;
            this.btnShowLayers.Text = "S";
            this.tip.SetToolTip(this.btnShowLayers, "Display sharpening layers");
            this.btnShowLayers.UseVisualStyleBackColor = true;
            this.btnShowLayers.Click += new System.EventHandler(this.btnShowLayers_Click);
            // 
            // udLayersImageScale
            // 
            this.udLayersImageScale.DecimalPlaces = 2;
            this.udLayersImageScale.Increment = new decimal(new int[] {
            1,
            0,
            0,
            65536});
            this.udLayersImageScale.Location = new System.Drawing.Point(148, 111);
            this.udLayersImageScale.Maximum = new decimal(new int[] {
            10,
            0,
            0,
            0});
            this.udLayersImageScale.Minimum = new decimal(new int[] {
            1,
            0,
            0,
            65536});
            this.udLayersImageScale.Name = "udLayersImageScale";
            this.udLayersImageScale.Size = new System.Drawing.Size(45, 20);
            this.udLayersImageScale.TabIndex = 97;
            this.tip.SetToolTip(this.udLayersImageScale, "Instead of calculating layers at 0.5px, 1px, 2px etc, multiply the layer scales o" +
        "f 0.5px, 1px, 2px, etc by this factor");
            this.udLayersImageScale.Value = new decimal(new int[] {
            1,
            0,
            0,
            0});
            this.udLayersImageScale.ValueChanged += new System.EventHandler(this.udLayersImageScale_ValueChanged);
            // 
            // label29
            // 
            this.label29.AutoSize = true;
            this.label29.Location = new System.Drawing.Point(105, 107);
            this.label29.Name = "label29";
            this.label29.Size = new System.Drawing.Size(43, 26);
            this.label29.TabIndex = 103;
            this.label29.Text = "Layer \r\nscale  x";
            // 
            // trkLayers5
            // 
            this.trkLayers5.Location = new System.Drawing.Point(173, 74);
            this.trkLayers5.Maximum = 100;
            this.trkLayers5.Minimum = -10;
            this.trkLayers5.Name = "trkLayers5";
            this.trkLayers5.Size = new System.Drawing.Size(90, 45);
            this.trkLayers5.TabIndex = 102;
            this.trkLayers5.TickFrequency = 5;
            this.trkLayers5.Value = 1;
            this.trkLayers5.Visible = false;
            this.trkLayers5.Scroll += new System.EventHandler(this.trkLayers_Scroll);
            this.trkLayers5.KeyDown += new System.Windows.Forms.KeyEventHandler(this.trkLayers_KeyDown);
            this.trkLayers5.KeyUp += new System.Windows.Forms.KeyEventHandler(this.trkLayers_KeyUp);
            this.trkLayers5.MouseDown += new System.Windows.Forms.MouseEventHandler(this.trkLayers_MouseDown);
            this.trkLayers5.MouseUp += new System.Windows.Forms.MouseEventHandler(this.trkLayers_MouseUp);
            // 
            // udLayersScale
            // 
            this.udLayersScale.DecimalPlaces = 2;
            this.udLayersScale.Increment = new decimal(new int[] {
            5,
            0,
            0,
            131072});
            this.udLayersScale.Location = new System.Drawing.Point(54, 104);
            this.udLayersScale.Minimum = new decimal(new int[] {
            1,
            0,
            0,
            131072});
            this.udLayersScale.Name = "udLayersScale";
            this.udLayersScale.Size = new System.Drawing.Size(45, 20);
            this.udLayersScale.TabIndex = 93;
            this.tip.SetToolTip(this.udLayersScale, "CONTRAST Level: Layer by layer corrections are all multiplied by this");
            this.udLayersScale.Value = new decimal(new int[] {
            300,
            0,
            0,
            131072});
            this.udLayersScale.ValueChanged += new System.EventHandler(this.udLayersScale_ValueChanged);
            // 
            // label9
            // 
            this.label9.AutoSize = true;
            this.label9.Location = new System.Drawing.Point(3, 107);
            this.label9.Name = "label9";
            this.label9.Size = new System.Drawing.Size(46, 13);
            this.label9.TabIndex = 60;
            this.label9.Text = "Contrast";
            // 
            // btnResetLayers
            // 
            this.btnResetLayers.Location = new System.Drawing.Point(219, 119);
            this.btnResetLayers.Name = "btnResetLayers";
            this.btnResetLayers.Size = new System.Drawing.Size(54, 22);
            this.btnResetLayers.TabIndex = 94;
            this.btnResetLayers.Text = "Reset";
            this.btnResetLayers.UseVisualStyleBackColor = true;
            this.btnResetLayers.Click += new System.EventHandler(this.btnResetLayers_Click);
            // 
            // trkLayers2
            // 
            this.trkLayers2.Location = new System.Drawing.Point(38, 75);
            this.trkLayers2.Maximum = 100;
            this.trkLayers2.Minimum = -10;
            this.trkLayers2.Name = "trkLayers2";
            this.trkLayers2.Size = new System.Drawing.Size(90, 45);
            this.trkLayers2.TabIndex = 85;
            this.trkLayers2.TickFrequency = 5;
            this.trkLayers2.Value = 1;
            this.trkLayers2.Scroll += new System.EventHandler(this.trkLayers_Scroll);
            this.trkLayers2.KeyDown += new System.Windows.Forms.KeyEventHandler(this.trkLayers_KeyDown);
            this.trkLayers2.KeyUp += new System.Windows.Forms.KeyEventHandler(this.trkLayers_KeyUp);
            this.trkLayers2.MouseDown += new System.Windows.Forms.MouseEventHandler(this.trkLayers_MouseDown);
            this.trkLayers2.MouseUp += new System.Windows.Forms.MouseEventHandler(this.trkLayers_MouseUp);
            // 
            // label11
            // 
            this.label11.AutoSize = true;
            this.label11.Location = new System.Drawing.Point(4, 144);
            this.label11.Name = "label11";
            this.label11.Size = new System.Drawing.Size(38, 13);
            this.label11.TabIndex = 101;
            this.label11.Text = "Saved";
            // 
            // cboLayerSettings
            // 
            this.cboLayerSettings.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cboLayerSettings.DropDownWidth = 140;
            this.cboLayerSettings.FormattingEnabled = true;
            this.cboLayerSettings.Location = new System.Drawing.Point(47, 141);
            this.cboLayerSettings.Name = "cboLayerSettings";
            this.cboLayerSettings.Size = new System.Drawing.Size(88, 21);
            this.cboLayerSettings.TabIndex = 98;
            this.cboLayerSettings.SelectedIndexChanged += new System.EventHandler(this.cboLayerSettings_SelectedIndexChanged);
            // 
            // txtSaveSettingsName
            // 
            this.txtSaveSettingsName.Location = new System.Drawing.Point(140, 142);
            this.txtSaveSettingsName.Name = "txtSaveSettingsName";
            this.txtSaveSettingsName.Size = new System.Drawing.Size(74, 20);
            this.txtSaveSettingsName.TabIndex = 99;
            this.txtSaveSettingsName.TextChanged += new System.EventHandler(this.txtSaveSettingsName_TextChanged);
            // 
            // btnSaveLayersSettings
            // 
            this.btnSaveLayersSettings.Enabled = false;
            this.btnSaveLayersSettings.Location = new System.Drawing.Point(219, 142);
            this.btnSaveLayersSettings.Name = "btnSaveLayersSettings";
            this.btnSaveLayersSettings.Size = new System.Drawing.Size(55, 21);
            this.btnSaveLayersSettings.TabIndex = 100;
            this.btnSaveLayersSettings.Text = "Save";
            this.btnSaveLayersSettings.UseVisualStyleBackColor = true;
            this.btnSaveLayersSettings.Click += new System.EventHandler(this.btnSaveLayersSettings_Click);
            // 
            // udLayersNoiseControl
            // 
            this.udLayersNoiseControl.DecimalPlaces = 2;
            this.udLayersNoiseControl.Increment = new decimal(new int[] {
            1,
            0,
            0,
            65536});
            this.udLayersNoiseControl.Location = new System.Drawing.Point(54, 122);
            this.udLayersNoiseControl.Maximum = new decimal(new int[] {
            5,
            0,
            0,
            0});
            this.udLayersNoiseControl.Name = "udLayersNoiseControl";
            this.udLayersNoiseControl.Size = new System.Drawing.Size(45, 20);
            this.udLayersNoiseControl.TabIndex = 96;
            this.tip.SetToolTip(this.udLayersNoiseControl, "Control noise in the sharpening layers (do a Gaussian blur with this sigma pixels" +
        " at the 1px level, \r\nhalf this sigma pixels at the 2px level, etc)");
            this.udLayersNoiseControl.Value = new decimal(new int[] {
            10,
            0,
            0,
            65536});
            this.udLayersNoiseControl.ValueChanged += new System.EventHandler(this.udLayersNoiseControl_ValueChanged);
            // 
            // label10
            // 
            this.label10.AutoSize = true;
            this.label10.Location = new System.Drawing.Point(3, 124);
            this.label10.Name = "label10";
            this.label10.Size = new System.Drawing.Size(43, 13);
            this.label10.TabIndex = 95;
            this.label10.Text = "Smooth";
            // 
            // chkLayers5
            // 
            this.chkLayers5.AutoSize = true;
            this.chkLayers5.Location = new System.Drawing.Point(139, 78);
            this.chkLayers5.Name = "chkLayers5";
            this.chkLayers5.Size = new System.Drawing.Size(38, 17);
            this.chkLayers5.TabIndex = 90;
            this.chkLayers5.Text = "16";
            this.tip.SetToolTip(this.chkLayers5, "Detail at 16 pixel scale");
            this.chkLayers5.UseVisualStyleBackColor = true;
            this.chkLayers5.CheckedChanged += new System.EventHandler(this.chkLayers_CheckedChanged);
            // 
            // chkLayers4
            // 
            this.chkLayers4.AutoSize = true;
            this.chkLayers4.Location = new System.Drawing.Point(139, 46);
            this.chkLayers4.Name = "chkLayers4";
            this.chkLayers4.Size = new System.Drawing.Size(32, 17);
            this.chkLayers4.TabIndex = 88;
            this.chkLayers4.Text = "8";
            this.tip.SetToolTip(this.chkLayers4, "Detail at 8 pixel scale");
            this.chkLayers4.UseVisualStyleBackColor = true;
            this.chkLayers4.CheckedChanged += new System.EventHandler(this.chkLayers_CheckedChanged);
            // 
            // chkLayers3
            // 
            this.chkLayers3.AutoSize = true;
            this.chkLayers3.Location = new System.Drawing.Point(140, 16);
            this.chkLayers3.Name = "chkLayers3";
            this.chkLayers3.Size = new System.Drawing.Size(32, 17);
            this.chkLayers3.TabIndex = 86;
            this.chkLayers3.Text = "4";
            this.tip.SetToolTip(this.chkLayers3, "Detail at 4 pixel scale");
            this.chkLayers3.UseVisualStyleBackColor = true;
            this.chkLayers3.CheckedChanged += new System.EventHandler(this.chkLayers_CheckedChanged);
            // 
            // chkLayers2
            // 
            this.chkLayers2.AutoSize = true;
            this.chkLayers2.Checked = true;
            this.chkLayers2.CheckState = System.Windows.Forms.CheckState.Checked;
            this.chkLayers2.Location = new System.Drawing.Point(3, 78);
            this.chkLayers2.Name = "chkLayers2";
            this.chkLayers2.Size = new System.Drawing.Size(32, 17);
            this.chkLayers2.TabIndex = 84;
            this.chkLayers2.Text = "2";
            this.tip.SetToolTip(this.chkLayers2, "Detail at 2 pixel scale");
            this.chkLayers2.UseVisualStyleBackColor = true;
            this.chkLayers2.CheckedChanged += new System.EventHandler(this.chkLayers_CheckedChanged);
            // 
            // chkLayers1
            // 
            this.chkLayers1.AutoSize = true;
            this.chkLayers1.Location = new System.Drawing.Point(4, 46);
            this.chkLayers1.Name = "chkLayers1";
            this.chkLayers1.Size = new System.Drawing.Size(32, 17);
            this.chkLayers1.TabIndex = 82;
            this.chkLayers1.Text = "1";
            this.tip.SetToolTip(this.chkLayers1, "Detail at 1.0 pixels (Original size)");
            this.chkLayers1.UseVisualStyleBackColor = true;
            this.chkLayers1.CheckedChanged += new System.EventHandler(this.chkLayers_CheckedChanged);
            // 
            // chkLayers0
            // 
            this.chkLayers0.AutoSize = true;
            this.chkLayers0.Location = new System.Drawing.Point(4, 16);
            this.chkLayers0.Name = "chkLayers0";
            this.chkLayers0.Size = new System.Drawing.Size(41, 17);
            this.chkLayers0.TabIndex = 80;
            this.chkLayers0.Text = "0.5";
            this.tip.SetToolTip(this.chkLayers0, "Detail at 0.5 pixels, ie details in enlarged x2 image");
            this.chkLayers0.UseVisualStyleBackColor = true;
            this.chkLayers0.CheckedChanged += new System.EventHandler(this.chkLayers_CheckedChanged);
            // 
            // trkLayers1
            // 
            this.trkLayers1.Location = new System.Drawing.Point(38, 44);
            this.trkLayers1.Maximum = 100;
            this.trkLayers1.Minimum = -10;
            this.trkLayers1.Name = "trkLayers1";
            this.trkLayers1.Size = new System.Drawing.Size(90, 45);
            this.trkLayers1.TabIndex = 83;
            this.trkLayers1.TickFrequency = 5;
            this.trkLayers1.Value = 1;
            this.trkLayers1.Visible = false;
            this.trkLayers1.Scroll += new System.EventHandler(this.trkLayers_Scroll);
            this.trkLayers1.KeyDown += new System.Windows.Forms.KeyEventHandler(this.trkLayers_KeyDown);
            this.trkLayers1.KeyUp += new System.Windows.Forms.KeyEventHandler(this.trkLayers_KeyUp);
            this.trkLayers1.MouseDown += new System.Windows.Forms.MouseEventHandler(this.trkLayers_MouseDown);
            this.trkLayers1.MouseUp += new System.Windows.Forms.MouseEventHandler(this.trkLayers_MouseUp);
            // 
            // trkLayers0
            // 
            this.trkLayers0.Location = new System.Drawing.Point(38, 13);
            this.trkLayers0.Maximum = 100;
            this.trkLayers0.Minimum = -10;
            this.trkLayers0.Name = "trkLayers0";
            this.trkLayers0.Size = new System.Drawing.Size(90, 45);
            this.trkLayers0.TabIndex = 81;
            this.trkLayers0.TickFrequency = 5;
            this.trkLayers0.Value = 1;
            this.trkLayers0.Visible = false;
            this.trkLayers0.Scroll += new System.EventHandler(this.trkLayers_Scroll);
            this.trkLayers0.KeyDown += new System.Windows.Forms.KeyEventHandler(this.trkLayers_KeyDown);
            this.trkLayers0.KeyUp += new System.Windows.Forms.KeyEventHandler(this.trkLayers_KeyUp);
            this.trkLayers0.MouseDown += new System.Windows.Forms.MouseEventHandler(this.trkLayers_MouseDown);
            this.trkLayers0.MouseUp += new System.Windows.Forms.MouseEventHandler(this.trkLayers_MouseUp);
            // 
            // trkLayers4
            // 
            this.trkLayers4.Location = new System.Drawing.Point(173, 44);
            this.trkLayers4.Maximum = 100;
            this.trkLayers4.Minimum = -10;
            this.trkLayers4.Name = "trkLayers4";
            this.trkLayers4.Size = new System.Drawing.Size(90, 45);
            this.trkLayers4.TabIndex = 89;
            this.trkLayers4.TickFrequency = 5;
            this.trkLayers4.Value = 1;
            this.trkLayers4.Visible = false;
            this.trkLayers4.Scroll += new System.EventHandler(this.trkLayers_Scroll);
            this.trkLayers4.KeyDown += new System.Windows.Forms.KeyEventHandler(this.trkLayers_KeyDown);
            this.trkLayers4.KeyUp += new System.Windows.Forms.KeyEventHandler(this.trkLayers_KeyUp);
            this.trkLayers4.MouseDown += new System.Windows.Forms.MouseEventHandler(this.trkLayers_MouseDown);
            this.trkLayers4.MouseUp += new System.Windows.Forms.MouseEventHandler(this.trkLayers_MouseUp);
            // 
            // trkLayers3
            // 
            this.trkLayers3.Location = new System.Drawing.Point(173, 13);
            this.trkLayers3.Maximum = 100;
            this.trkLayers3.Minimum = -10;
            this.trkLayers3.Name = "trkLayers3";
            this.trkLayers3.Size = new System.Drawing.Size(90, 45);
            this.trkLayers3.TabIndex = 87;
            this.trkLayers3.TickFrequency = 5;
            this.trkLayers3.Value = 1;
            this.trkLayers3.Visible = false;
            this.trkLayers3.Scroll += new System.EventHandler(this.trkLayers_Scroll);
            this.trkLayers3.KeyDown += new System.Windows.Forms.KeyEventHandler(this.trkLayers_KeyDown);
            this.trkLayers3.KeyUp += new System.Windows.Forms.KeyEventHandler(this.trkLayers_KeyUp);
            this.trkLayers3.MouseDown += new System.Windows.Forms.MouseEventHandler(this.trkLayers_MouseDown);
            this.trkLayers3.MouseUp += new System.Windows.Forms.MouseEventHandler(this.trkLayers_MouseUp);
            // 
            // cboKernelSharpeningLayers
            // 
            this.cboKernelSharpeningLayers.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cboKernelSharpeningLayers.DropDownWidth = 300;
            this.cboKernelSharpeningLayers.FormattingEnabled = true;
            this.cboKernelSharpeningLayers.Items.AddRange(new object[] {
            "Laplacian (-1,-1,-1;-1,8,-1;-1,-1,-1)",
            "Lorentz (-0.67,-1,-0.67;-1,6.68,-1;-0.67,-1,-0.67)",
            "Lap_NoDiag (0,-1,0; -1,4,-1; 0,-1,0)",
            "Gaussian (-0.135,-0.37,-0.135; -0.37, 2.02, -0.37; -0.135,-0.37,-0.135)",
            "5x5 Gaussian",
            "5x5 Lorentz"});
            this.cboKernelSharpeningLayers.Location = new System.Drawing.Point(179, 202);
            this.cboKernelSharpeningLayers.Name = "cboKernelSharpeningLayers";
            this.cboKernelSharpeningLayers.Size = new System.Drawing.Size(97, 21);
            this.cboKernelSharpeningLayers.TabIndex = 42;
            this.cboKernelSharpeningLayers.SelectedIndexChanged += new System.EventHandler(this.cboKernelSharpeningLayers_SelectedIndexChanged);
            // 
            // picPSFProfile
            // 
            this.picPSFProfile.Location = new System.Drawing.Point(487, 24);
            this.picPSFProfile.Name = "picPSFProfile";
            this.picPSFProfile.Size = new System.Drawing.Size(259, 96);
            this.picPSFProfile.TabIndex = 69;
            this.picPSFProfile.TabStop = false;
            // 
            // label24
            // 
            this.label24.AutoSize = true;
            this.label24.Location = new System.Drawing.Point(490, 7);
            this.label24.Name = "label24";
            this.label24.Size = new System.Drawing.Size(115, 13);
            this.label24.TabIndex = 70;
            this.label24.Text = "PSF display width in px";
            // 
            // udPSFPlotWidth
            // 
            this.udPSFPlotWidth.Increment = new decimal(new int[] {
            4,
            0,
            0,
            0});
            this.udPSFPlotWidth.Location = new System.Drawing.Point(621, 2);
            this.udPSFPlotWidth.Maximum = new decimal(new int[] {
            200,
            0,
            0,
            0});
            this.udPSFPlotWidth.Minimum = new decimal(new int[] {
            4,
            0,
            0,
            0});
            this.udPSFPlotWidth.Name = "udPSFPlotWidth";
            this.udPSFPlotWidth.Size = new System.Drawing.Size(37, 20);
            this.udPSFPlotWidth.TabIndex = 71;
            this.udPSFPlotWidth.Value = new decimal(new int[] {
            40,
            0,
            0,
            0});
            this.udPSFPlotWidth.ValueChanged += new System.EventHandler(this.udPSFPlotWidth_ValueChanged);
            // 
            // btnClearPSFPlot
            // 
            this.btnClearPSFPlot.Location = new System.Drawing.Point(671, 1);
            this.btnClearPSFPlot.Name = "btnClearPSFPlot";
            this.btnClearPSFPlot.Size = new System.Drawing.Size(75, 23);
            this.btnClearPSFPlot.TabIndex = 72;
            this.btnClearPSFPlot.Text = "Clear";
            this.btnClearPSFPlot.UseVisualStyleBackColor = true;
            this.btnClearPSFPlot.Click += new System.EventHandler(this.btnClearPSFPlot_Click);
            // 
            // label26
            // 
            this.label26.AutoSize = true;
            this.label26.Location = new System.Drawing.Point(6, 71);
            this.label26.Name = "label26";
            this.label26.Size = new System.Drawing.Size(34, 13);
            this.label26.TabIndex = 73;
            this.label26.Text = "Zoom";
            // 
            // udZoom
            // 
            this.udZoom.DecimalPlaces = 2;
            this.udZoom.Increment = new decimal(new int[] {
            2,
            0,
            0,
            65536});
            this.udZoom.Location = new System.Drawing.Point(46, 67);
            this.udZoom.Minimum = new decimal(new int[] {
            1,
            0,
            0,
            131072});
            this.udZoom.Name = "udZoom";
            this.udZoom.Size = new System.Drawing.Size(50, 20);
            this.udZoom.TabIndex = 5;
            this.udZoom.Value = new decimal(new int[] {
            1,
            0,
            0,
            0});
            this.udZoom.ValueChanged += new System.EventHandler(this.udZoom_ValueChanged);
            // 
            // trkGif
            // 
            this.trkGif.Location = new System.Drawing.Point(852, 11);
            this.trkGif.Name = "trkGif";
            this.trkGif.Orientation = System.Windows.Forms.Orientation.Vertical;
            this.trkGif.Size = new System.Drawing.Size(45, 104);
            this.trkGif.TabIndex = 75;
            this.trkGif.Visible = false;
            this.trkGif.Scroll += new System.EventHandler(this.trkGif_Scroll);
            // 
            // chkClickCompareTo
            // 
            this.chkClickCompareTo.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.chkClickCompareTo.AutoSize = true;
            this.chkClickCompareTo.Checked = true;
            this.chkClickCompareTo.CheckState = System.Windows.Forms.CheckState.Checked;
            this.chkClickCompareTo.Font = new System.Drawing.Font("Microsoft Sans Serif", 10F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.chkClickCompareTo.ForeColor = System.Drawing.Color.DarkGreen;
            this.chkClickCompareTo.Location = new System.Drawing.Point(472, 663);
            this.chkClickCompareTo.Name = "chkClickCompareTo";
            this.chkClickCompareTo.Size = new System.Drawing.Size(105, 21);
            this.chkClickCompareTo.TabIndex = 112;
            this.chkClickCompareTo.Text = "Click to blink";
            this.chkClickCompareTo.UseVisualStyleBackColor = true;
            // 
            // btnOriginal
            // 
            this.btnOriginal.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.btnOriginal.Location = new System.Drawing.Point(885, 654);
            this.btnOriginal.Name = "btnOriginal";
            this.btnOriginal.Size = new System.Drawing.Size(37, 23);
            this.btnOriginal.TabIndex = 114;
            this.btnOriginal.Text = "Original";
            this.btnOriginal.UseVisualStyleBackColor = true;
            this.btnOriginal.Visible = false;
            this.btnOriginal.Click += new System.EventHandler(this.btnOriginal_Click);
            // 
            // btnLimb
            // 
            this.btnLimb.Location = new System.Drawing.Point(896, 58);
            this.btnLimb.Name = "btnLimb";
            this.btnLimb.Size = new System.Drawing.Size(47, 23);
            this.btnLimb.TabIndex = 80;
            this.btnLimb.Text = "Debug";
            this.btnLimb.UseVisualStyleBackColor = true;
            this.btnLimb.Visible = false;
            this.btnLimb.Click += new System.EventHandler(this.btnLimb_Click);
            // 
            // grpAnalysis
            // 
            this.grpAnalysis.Controls.Add(this.lblLaplacian);
            this.grpAnalysis.Controls.Add(this.chkLaplacian);
            this.grpAnalysis.Location = new System.Drawing.Point(890, 2);
            this.grpAnalysis.Name = "grpAnalysis";
            this.grpAnalysis.Size = new System.Drawing.Size(95, 34);
            this.grpAnalysis.TabIndex = 81;
            this.grpAnalysis.TabStop = false;
            this.grpAnalysis.Text = "Analysis";
            this.grpAnalysis.Visible = false;
            // 
            // lblLaplacian
            // 
            this.lblLaplacian.AutoSize = true;
            this.lblLaplacian.Location = new System.Drawing.Point(92, 15);
            this.lblLaplacian.Name = "lblLaplacian";
            this.lblLaplacian.Size = new System.Drawing.Size(0, 13);
            this.lblLaplacian.TabIndex = 1;
            // 
            // chkLaplacian
            // 
            this.chkLaplacian.AutoSize = true;
            this.chkLaplacian.Location = new System.Drawing.Point(9, 15);
            this.chkLaplacian.Name = "chkLaplacian";
            this.chkLaplacian.Size = new System.Drawing.Size(72, 17);
            this.chkLaplacian.TabIndex = 100;
            this.chkLaplacian.Text = "Laplacian";
            this.tip.SetToolTip(this.chkLaplacian, "Calculate the divergence (Laplacian) of the restored image");
            this.chkLaplacian.UseVisualStyleBackColor = true;
            // 
            // btnStoreOutput
            // 
            this.btnStoreOutput.BackColor = System.Drawing.SystemColors.ControlDark;
            this.btnStoreOutput.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, ((System.Drawing.FontStyle)((System.Drawing.FontStyle.Bold | System.Drawing.FontStyle.Italic))), System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.btnStoreOutput.Location = new System.Drawing.Point(2, 84);
            this.btnStoreOutput.Name = "btnStoreOutput";
            this.btnStoreOutput.Size = new System.Drawing.Size(94, 36);
            this.btnStoreOutput.TabIndex = 6;
            this.btnStoreOutput.Text = "STORE repaired";
            this.tip.SetToolTip(this.btnStoreOutput, "Store this repaired image (then click mouse on image to compare to)");
            this.btnStoreOutput.UseVisualStyleBackColor = false;
            this.btnStoreOutput.Click += new System.EventHandler(this.btnStoreOutput_Click);
            // 
            // btnFileLocation
            // 
            this.btnFileLocation.BackColor = System.Drawing.SystemColors.ControlDark;
            this.btnFileLocation.Location = new System.Drawing.Point(140, 0);
            this.btnFileLocation.Name = "btnFileLocation";
            this.btnFileLocation.Size = new System.Drawing.Size(33, 28);
            this.btnFileLocation.TabIndex = 2;
            this.btnFileLocation.Text = "...";
            this.btnFileLocation.UseVisualStyleBackColor = false;
            this.btnFileLocation.Click += new System.EventHandler(this.btnFileLocation_Click);
            // 
            // chkAutostretch
            // 
            this.chkAutostretch.AutoSize = true;
            this.chkAutostretch.Font = new System.Drawing.Font("Microsoft Sans Serif", 6.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.chkAutostretch.Location = new System.Drawing.Point(102, 33);
            this.chkAutostretch.Name = "chkAutostretch";
            this.chkAutostretch.Size = new System.Drawing.Size(73, 16);
            this.chkAutostretch.TabIndex = 4;
            this.chkAutostretch.Text = "Autostretch";
            this.tip.SetToolTip(this.chkAutostretch, "Linear stretch on repaired image between 0.25 and 99.9 percentiles");
            this.chkAutostretch.UseVisualStyleBackColor = true;
            this.chkAutostretch.CheckedChanged += new System.EventHandler(this.chkAutostretch_CheckedChanged);
            // 
            // cboZoomMode
            // 
            this.cboZoomMode.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cboZoomMode.DropDownWidth = 140;
            this.cboZoomMode.FormattingEnabled = true;
            this.cboZoomMode.Items.AddRange(new object[] {
            "Linear",
            "Cubic",
            "Nearest",
            "Lanczos4"});
            this.cboZoomMode.Location = new System.Drawing.Point(182, 627);
            this.cboZoomMode.Name = "cboZoomMode";
            this.cboZoomMode.Size = new System.Drawing.Size(100, 21);
            this.cboZoomMode.TabIndex = 82;
            this.tip.SetToolTip(this.cboZoomMode, "Interpolation mode when zooming (use Nearest to view actual pixels)");
            this.cboZoomMode.SelectedIndexChanged += new System.EventHandler(this.cboZoomMode_SelectedIndexChanged);
            // 
            // btnDetailMask
            // 
            this.btnDetailMask.Location = new System.Drawing.Point(7, 624);
            this.btnDetailMask.Name = "btnDetailMask";
            this.btnDetailMask.Size = new System.Drawing.Size(75, 23);
            this.btnDetailMask.TabIndex = 81;
            this.btnDetailMask.Text = "Detail mask";
            this.tip.SetToolTip(this.btnDetailMask, "Make mask where most image detail present (Gaussian blurred Lapacian)");
            this.btnDetailMask.UseVisualStyleBackColor = true;
            this.btnDetailMask.Click += new System.EventHandler(this.btnDetailMask_Click);
            // 
            // optFilterFourierTransform
            // 
            this.optFilterFourierTransform.AutoSize = true;
            this.optFilterFourierTransform.Checked = true;
            this.optFilterFourierTransform.Location = new System.Drawing.Point(6, 12);
            this.optFilterFourierTransform.Name = "optFilterFourierTransform";
            this.optFilterFourierTransform.Size = new System.Drawing.Size(112, 17);
            this.optFilterFourierTransform.TabIndex = 162;
            this.optFilterFourierTransform.TabStop = true;
            this.optFilterFourierTransform.Text = "Filter Fourier Trans";
            this.tip.SetToolTip(this.optFilterFourierTransform, "Display Fourier Transform of PSF");
            this.optFilterFourierTransform.UseVisualStyleBackColor = true;
            // 
            // optMTF
            // 
            this.optMTF.AutoSize = true;
            this.optMTF.Location = new System.Drawing.Point(6, 27);
            this.optMTF.Name = "optMTF";
            this.optMTF.Size = new System.Drawing.Size(82, 17);
            this.optMTF.TabIndex = 163;
            this.optMTF.Text = "MTF of PSF";
            this.tip.SetToolTip(this.optMTF, "Plot modulation transfer function (MTF), scaled to current FWHM");
            this.optMTF.UseVisualStyleBackColor = true;
            // 
            // btnFourierTransform
            // 
            this.btnFourierTransform.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.btnFourierTransform.Location = new System.Drawing.Point(915, 659);
            this.btnFourierTransform.Name = "btnFourierTransform";
            this.btnFourierTransform.Size = new System.Drawing.Size(34, 23);
            this.btnFourierTransform.TabIndex = 171;
            this.btnFourierTransform.Text = "FT";
            this.tip.SetToolTip(this.btnFourierTransform, "Display Fourier transform of image");
            this.btnFourierTransform.UseVisualStyleBackColor = true;
            this.btnFourierTransform.Click += new System.EventHandler(this.btnFourierTransform_Click);
            // 
            // chkPSF
            // 
            this.chkPSF.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.chkPSF.AutoSize = true;
            this.chkPSF.Location = new System.Drawing.Point(867, 662);
            this.chkPSF.Name = "chkPSF";
            this.chkPSF.Size = new System.Drawing.Size(46, 17);
            this.chkPSF.TabIndex = 170;
            this.chkPSF.Text = "PSF";
            this.tip.SetToolTip(this.chkPSF, "Display FT of PSF");
            this.chkPSF.UseVisualStyleBackColor = true;
            // 
            // btnRotate
            // 
            this.btnRotate.Location = new System.Drawing.Point(890, 36);
            this.btnRotate.Name = "btnRotate";
            this.btnRotate.Size = new System.Drawing.Size(53, 23);
            this.btnRotate.TabIndex = 162;
            this.btnRotate.Text = "Rotate";
            this.btnRotate.UseVisualStyleBackColor = true;
            this.btnRotate.Visible = false;
            this.btnRotate.Click += new System.EventHandler(this.btnRotate_Click);
            // 
            // btnTest
            // 
            this.btnTest.Location = new System.Drawing.Point(896, 83);
            this.btnTest.Name = "btnTest";
            this.btnTest.Size = new System.Drawing.Size(48, 23);
            this.btnTest.TabIndex = 163;
            this.btnTest.Text = "Test";
            this.btnTest.UseVisualStyleBackColor = true;
            this.btnTest.Visible = false;
            this.btnTest.Click += new System.EventHandler(this.btnTest_Click);
            // 
            // lblPixelPos
            // 
            this.lblPixelPos.AutoSize = true;
            this.lblPixelPos.Dock = System.Windows.Forms.DockStyle.Right;
            this.lblPixelPos.Location = new System.Drawing.Point(949, 0);
            this.lblPixelPos.Name = "lblPixelPos";
            this.lblPixelPos.Size = new System.Drawing.Size(0, 13);
            this.lblPixelPos.TabIndex = 164;
            // 
            // chkTanhRepair
            // 
            this.chkTanhRepair.AutoSize = true;
            this.chkTanhRepair.Location = new System.Drawing.Point(644, 80);
            this.chkTanhRepair.Name = "chkTanhRepair";
            this.chkTanhRepair.Size = new System.Drawing.Size(82, 17);
            this.chkTanhRepair.TabIndex = 167;
            this.chkTanhRepair.Text = "Fade edges";
            this.chkTanhRepair.UseVisualStyleBackColor = true;
            this.chkTanhRepair.Visible = false;
            // 
            // txtGamma
            // 
            this.txtGamma.Location = new System.Drawing.Point(672, 97);
            this.txtGamma.Name = "txtGamma";
            this.txtGamma.Size = new System.Drawing.Size(23, 20);
            this.txtGamma.TabIndex = 168;
            this.txtGamma.Text = "4.8";
            this.txtGamma.Visible = false;
            // 
            // label5
            // 
            this.label5.AutoSize = true;
            this.label5.Location = new System.Drawing.Point(654, 97);
            this.label5.Name = "label5";
            this.label5.Size = new System.Drawing.Size(13, 13);
            this.label5.TabIndex = 165;
            this.label5.Text = "γ";
            this.label5.Visible = false;
            // 
            // txtBeta
            // 
            this.txtBeta.Location = new System.Drawing.Point(720, 97);
            this.txtBeta.Name = "txtBeta";
            this.txtBeta.Size = new System.Drawing.Size(21, 20);
            this.txtBeta.TabIndex = 169;
            this.txtBeta.Text = "0.2";
            this.txtBeta.Visible = false;
            // 
            // label4
            // 
            this.label4.AutoSize = true;
            this.label4.Location = new System.Drawing.Point(701, 100);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(13, 13);
            this.label4.TabIndex = 166;
            this.label4.Text = "β";
            this.label4.Visible = false;
            // 
            // label28
            // 
            this.label28.AutoSize = true;
            this.label28.Location = new System.Drawing.Point(110, 631);
            this.label28.Name = "label28";
            this.label28.Size = new System.Drawing.Size(70, 13);
            this.label28.TabIndex = 103;
            this.label28.Text = "Display mode";
            // 
            // grpPlotExtra
            // 
            this.grpPlotExtra.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.grpPlotExtra.Controls.Add(this.optMTF);
            this.grpPlotExtra.Controls.Add(this.optFilterFourierTransform);
            this.grpPlotExtra.Location = new System.Drawing.Point(804, 464);
            this.grpPlotExtra.Name = "grpPlotExtra";
            this.grpPlotExtra.Size = new System.Drawing.Size(139, 46);
            this.grpPlotExtra.TabIndex = 161;
            this.grpPlotExtra.TabStop = false;
            this.grpPlotExtra.Text = "Plot";
            // 
            // mnuRightMain
            // 
            this.mnuRightMain.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.mnuSetArea,
            this.mnuSetWholeImage});
            this.mnuRightMain.Name = "mnuRightMain";
            this.mnuRightMain.Size = new System.Drawing.Size(188, 48);
            // 
            // mnuSetArea
            // 
            this.mnuSetArea.Name = "mnuSetArea";
            this.mnuSetArea.Size = new System.Drawing.Size(187, 22);
            this.mnuSetArea.Text = "Select area to process";
            this.mnuSetArea.Click += new System.EventHandler(this.mnuSetArea_Click);
            // 
            // mnuSetWholeImage
            // 
            this.mnuSetWholeImage.Name = "mnuSetWholeImage";
            this.mnuSetWholeImage.Size = new System.Drawing.Size(187, 22);
            this.mnuSetWholeImage.Text = "Process whole image";
            this.mnuSetWholeImage.Click += new System.EventHandler(this.mnuSetWholeImage_Click);
            // 
            // btnSavePSF
            // 
            this.btnSavePSF.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.btnSavePSF.Location = new System.Drawing.Point(895, 124);
            this.btnSavePSF.Name = "btnSavePSF";
            this.btnSavePSF.Size = new System.Drawing.Size(41, 23);
            this.btnSavePSF.TabIndex = 172;
            this.btnSavePSF.Text = "Save";
            this.btnSavePSF.UseVisualStyleBackColor = true;
            this.btnSavePSF.Click += new System.EventHandler(this.btnSavePSF_Click);
            // 
            // picPSF
            // 
            this.picPSF.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.picPSF.InterpolationMode = System.Drawing.Drawing2D.InterpolationMode.NearestNeighbor;
            this.picPSF.Location = new System.Drawing.Point(807, 149);
            this.picPSF.Name = "picPSF";
            this.picPSF.Size = new System.Drawing.Size(125, 124);
            this.picPSF.SizeMode = System.Windows.Forms.PictureBoxSizeMode.StretchImage;
            this.picPSF.TabIndex = 54;
            this.picPSF.TabStop = false;
            // 
            // frmMain
            // 
            this.AcceptButton = this.btnStoreOutput;
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(949, 689);
            this.Controls.Add(this.btnSavePSF);
            this.Controls.Add(this.chkPSF);
            this.Controls.Add(this.btnFourierTransform);
            this.Controls.Add(this.grpPlotExtra);
            this.Controls.Add(this.btnDetailMask);
            this.Controls.Add(this.groupBox1);
            this.Controls.Add(this.chkAutostretch);
            this.Controls.Add(this.cboZoomMode);
            this.Controls.Add(this.label28);
            this.Controls.Add(this.chkTanhRepair);
            this.Controls.Add(this.txtGamma);
            this.Controls.Add(this.label5);
            this.Controls.Add(this.txtBeta);
            this.Controls.Add(this.label4);
            this.Controls.Add(this.lblPixelPos);
            this.Controls.Add(this.btnTest);
            this.Controls.Add(this.btnRotate);
            this.Controls.Add(this.btnFileLocation);
            this.Controls.Add(this.btnStoreOutput);
            this.Controls.Add(this.grpAnalysis);
            this.Controls.Add(this.btnLimb);
            this.Controls.Add(this.btnOriginal);
            this.Controls.Add(this.chkClickCompareTo);
            this.Controls.Add(this.trkGif);
            this.Controls.Add(this.udZoom);
            this.Controls.Add(this.label26);
            this.Controls.Add(this.btnClearPSFPlot);
            this.Controls.Add(this.udPSFPlotWidth);
            this.Controls.Add(this.label24);
            this.Controls.Add(this.picPSFProfile);
            this.Controls.Add(this.grpPSFRepair);
            this.Controls.Add(this.cboCurrent);
            this.Controls.Add(this.label21);
            this.Controls.Add(this.lblProcessing);
            this.Controls.Add(this.btnConvolve);
            this.Controls.Add(this.label16);
            this.Controls.Add(this.cboHistory);
            this.Controls.Add(this.btnSave);
            this.Controls.Add(this.btnCopyPSF);
            this.Controls.Add(this.txtPSFDump);
            this.Controls.Add(this.grpPSFModify);
            this.Controls.Add(this.chkAutoDeblur);
            this.Controls.Add(this.picPSF);
            this.Controls.Add(this.label14);
            this.Controls.Add(this.grpEdgeRepair);
            this.Controls.Add(this.grpMethod);
            this.Controls.Add(this.btnFFTW);
            this.Controls.Add(this.picFilterFT);
            this.Controls.Add(this.pnlOut);
            this.Controls.Add(this.txtImage);
            this.Controls.Add(this.btnDeblur);
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.MinimumSize = new System.Drawing.Size(965, 718);
            this.Name = "frmMain";
            this.Text = "The Deconvolvulator v1.0";
            this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.frmMain_FormClosing);
            this.Load += new System.EventHandler(this.frmMain_Load);
            this.Shown += new System.EventHandler(this.frmMain_Shown);
            this.ResizeEnd += new System.EventHandler(this.frmMain_ResizeEnd);
            this.Resize += new System.EventHandler(this.frmMain_Resize);
            ((System.ComponentModel.ISupportInitialize)(this.picOut)).EndInit();
            this.pnlOut.ResumeLayout(false);
            this.pnlOut.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.picFilterFT)).EndInit();
            this.grpMethod.ResumeLayout(false);
            this.grpMethod.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.trkBlur)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.trkIterations)).EndInit();
            this.grpPSF.ResumeLayout(false);
            this.grpPSF.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.udWave)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.udGaussFraction)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.udMoffatPasses)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.udCropPSF)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.udBrightness)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.udFWHM)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.udMoffatBeta)).EndInit();
            this.grpEdgeRepair.ResumeLayout(false);
            this.grpEdgeRepair.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.udDeringRepairStrength)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.udDeringStarThreshold)).EndInit();
            this.grpMotionBlurDetails.ResumeLayout(false);
            this.grpMotionBlurDetails.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.udCentreFieldRotationY)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.udCentreFieldRotationX)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.udRotationTiles)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.udMotionBlurAngle)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.udMotionBlurLength)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.udFieldRotationAngle)).EndInit();
            this.grpPSFModify.ResumeLayout(false);
            this.grpPSFModify.PerformLayout();
            this.groupBox1.ResumeLayout(false);
            this.groupBox1.PerformLayout();
            this.grpPSFRepair.ResumeLayout(false);
            this.grpPSFRepair.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.udUpscale)).EndInit();
            this.grpLayers.ResumeLayout(false);
            this.grpLayers.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.udLayersImageScale)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.trkLayers5)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.udLayersScale)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.trkLayers2)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.udLayersNoiseControl)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.trkLayers1)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.trkLayers0)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.trkLayers4)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.trkLayers3)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.picPSFProfile)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.udPSFPlotWidth)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.udZoom)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.trkGif)).EndInit();
            this.grpAnalysis.ResumeLayout(false);
            this.grpAnalysis.PerformLayout();
            this.grpPlotExtra.ResumeLayout(false);
            this.grpPlotExtra.PerformLayout();
            this.mnuRightMain.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.picPSF)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion
        private System.Windows.Forms.Button btnDeblur;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.TextBox txtNSR;
        private System.Windows.Forms.TextBox txtImage;
        private System.Windows.Forms.CheckBox chkRepairEdges;
        private System.Windows.Forms.PictureBox picOut;
        private System.Windows.Forms.Panel pnlOut;
        private System.Windows.Forms.Label label6;
        private System.Windows.Forms.TextBox txtPSFLineThickness;
        private System.Windows.Forms.Label label7;
        private System.Windows.Forms.Label label8;
        private System.Windows.Forms.TextBox txtFeather;
        private System.Windows.Forms.CheckBox chkAntiAliasLine;
        private System.Windows.Forms.PictureBox picFilterFT;
        private System.Windows.Forms.TextBox txtCLS_Y;
        private System.Windows.Forms.TextBox txtIterations;
        private System.Windows.Forms.Button btnStopIterations;
        private System.Windows.Forms.Label lblIterationInfo;
        private System.Windows.Forms.Button btnFFTW;
        private System.Windows.Forms.GroupBox grpMethod;
        private System.Windows.Forms.RadioButton optLandweber;
        private System.Windows.Forms.RadioButton optTikhonov;
        private System.Windows.Forms.RadioButton optWiener;
        private System.Windows.Forms.GroupBox grpPSF;
        private System.Windows.Forms.RadioButton optGaussianDeblur;
        private System.Windows.Forms.RadioButton optCameraCircleDeblur;
        private System.Windows.Forms.Label label12;
        private System.Windows.Forms.TrackBar trkIterations;
        private System.Windows.Forms.TrackBar trkBlur;
        private System.Windows.Forms.GroupBox grpEdgeRepair;
        private System.Windows.Forms.GroupBox grpMotionBlurDetails;
        private System.Windows.Forms.CheckBox chkRepairTopBottom;
        private System.Windows.Forms.RadioButton optMoffat;
        private System.Windows.Forms.Label label13;
        private System.Windows.Forms.NumericUpDown udMoffatBeta;
        private System.Windows.Forms.NumericUpDown udFWHM;
        private System.Windows.Forms.CheckBox chkAutoDeblur;
        private System.Windows.Forms.RadioButton optLR;
        private System.Windows.Forms.Label label14;
        private PixelBox picPSF;
        private System.Windows.Forms.GroupBox grpPSFModify;
        private System.Windows.Forms.RadioButton optCustomRepair;
        private System.Windows.Forms.TextBox txtPSFDump;
        private System.Windows.Forms.Button btnCopyPSF;
        private System.Windows.Forms.NumericUpDown udBrightness;
        private System.Windows.Forms.Button btnSave;
        private System.Windows.Forms.ComboBox cboHistory;
        private System.Windows.Forms.Label label16;
        private System.Windows.Forms.GroupBox groupBox1;
        private System.Windows.Forms.RadioButton optPrevious;
        private System.Windows.Forms.RadioButton optOriginal;
        private System.Windows.Forms.Label label20;
        private System.Windows.Forms.Label label19;
        private System.Windows.Forms.Label label18;
        private System.Windows.Forms.Label label17;
        private System.Windows.Forms.Button btnConvolve;
        private System.Windows.Forms.NumericUpDown udCropPSF;
        private System.Windows.Forms.CheckBox chkCropPSF;
        private System.Windows.Forms.Label lblProcessing;
        private System.Windows.Forms.Label label21;
        private System.Windows.Forms.ComboBox cboCurrent;
        private System.Windows.Forms.NumericUpDown udMoffatPasses;
        private System.Windows.Forms.CheckBox chkMoffatInPasses;
        private System.Windows.Forms.NumericUpDown udGaussFraction;
        private System.Windows.Forms.RadioButton optVoigt;
        private System.Windows.Forms.Label label22;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.GroupBox grpPSFRepair;
        private System.Windows.Forms.RadioButton optCircularBlur;
        private System.Windows.Forms.RadioButton optMotionBlur;
        private System.Windows.Forms.PictureBox picPSFProfile;
        private System.Windows.Forms.Label label24;
        private System.Windows.Forms.NumericUpDown udPSFPlotWidth;
        private System.Windows.Forms.Button btnClearPSFPlot;
        private System.Windows.Forms.Label label25;
        private System.Windows.Forms.Label label26;
        private System.Windows.Forms.NumericUpDown udZoom;
        private System.Windows.Forms.Label label23;
        private System.Windows.Forms.TrackBar trkGif;
        private System.Windows.Forms.CheckBox chkClickCompareTo;
        private System.Windows.Forms.Button btnOriginal;
        private System.Windows.Forms.Label label15;
        private System.Windows.Forms.Button btnLimb;
        private System.Windows.Forms.GroupBox grpAnalysis;
        private System.Windows.Forms.Label lblLaplacian;
        private System.Windows.Forms.CheckBox chkLaplacian;
        private System.Windows.Forms.GroupBox grpLayers;
        private System.Windows.Forms.CheckBox chkLayers5;
        private System.Windows.Forms.CheckBox chkLayers4;
        private System.Windows.Forms.CheckBox chkLayers3;
        private System.Windows.Forms.CheckBox chkLayers2;
        private System.Windows.Forms.CheckBox chkLayers1;
        private System.Windows.Forms.CheckBox chkLayers0;
        private System.Windows.Forms.TrackBar trkLayers4;
        private System.Windows.Forms.TrackBar trkLayers2;
        private System.Windows.Forms.TrackBar trkLayers1;
        private System.Windows.Forms.TrackBar trkLayers3;
        private System.Windows.Forms.TrackBar trkLayers0;
        private System.Windows.Forms.Button btnStoreOutput;
        private System.Windows.Forms.Label label9;
        private System.Windows.Forms.NumericUpDown udLayersScale;
        private System.Windows.Forms.Button btnFileLocation;
        private System.Windows.Forms.NumericUpDown udMotionBlurAngle;
        private System.Windows.Forms.NumericUpDown udMotionBlurLength;
        private System.Windows.Forms.Button btnResetLayers;
        private System.Windows.Forms.NumericUpDown udLayersNoiseControl;
        private System.Windows.Forms.Label label10;
        private System.Windows.Forms.ComboBox cboLayerSettings;
        private System.Windows.Forms.TextBox txtSaveSettingsName;
        private System.Windows.Forms.Button btnSaveLayersSettings;
        private System.Windows.Forms.Label label11;
        private System.Windows.Forms.ToolTip tip;
        private System.Windows.Forms.TrackBar trkLayers5;
        private System.Windows.Forms.NumericUpDown udFieldRotationAngle;
        private System.Windows.Forms.Label label27;
        private System.Windows.Forms.NumericUpDown udRotationTiles;
        private System.Windows.Forms.CheckBox chkFiledRotationDeblur;
        private System.Windows.Forms.Button btnRotate;
        private System.Windows.Forms.Button btnTest;
        private System.Windows.Forms.NumericUpDown udCentreFieldRotationY;
        private System.Windows.Forms.NumericUpDown udCentreFieldRotationX;
        private System.Windows.Forms.ComboBox cboKernelSharpeningLayers;
        private System.Windows.Forms.Label lblPixelPos;
        private System.Windows.Forms.NumericUpDown udDeringRepairStrength;
        private System.Windows.Forms.NumericUpDown udDeringStarThreshold;
        private System.Windows.Forms.CheckBox chkDeringing;
        private System.Windows.Forms.CheckBox chkTanhRepair;
        private System.Windows.Forms.TextBox txtGamma;
        private System.Windows.Forms.Label label5;
        private System.Windows.Forms.TextBox txtBeta;
        private System.Windows.Forms.Label label4;
        private System.Windows.Forms.ComboBox cboZoomMode;
        private System.Windows.Forms.Label label28;
        private System.Windows.Forms.CheckBox chkRotateImage;
        private System.Windows.Forms.CheckBox chkAutostretch;
        private System.Windows.Forms.RadioButton optSharpeningLayers;
        private System.Windows.Forms.Button btnDetailMask;
        private System.Windows.Forms.NumericUpDown udLayersImageScale;
        private System.Windows.Forms.Label label29;
        private System.Windows.Forms.GroupBox grpPlotExtra;
        private System.Windows.Forms.RadioButton optMTF;
        private System.Windows.Forms.RadioButton optFilterFourierTransform;
        private System.Windows.Forms.Label lblLastStored;
        private System.Windows.Forms.Button btnShowLayers;
        private System.Windows.Forms.NumericUpDown udWave;
        private System.Windows.Forms.Label label30;
        private System.Windows.Forms.RadioButton optMTFPSF;
        private System.Windows.Forms.Label label31;
        private System.Windows.Forms.NumericUpDown udUpscale;
        private System.Windows.Forms.CheckBox chkProcessLuminance;
        private System.Windows.Forms.ContextMenuStrip mnuRightMain;
        private System.Windows.Forms.ToolStripMenuItem mnuSetArea;
        private System.Windows.Forms.ToolStripMenuItem mnuSetWholeImage;
        private System.Windows.Forms.Button btnFourierTransform;
        private System.Windows.Forms.CheckBox chkPSF;
        private System.Windows.Forms.Label label32;
        private System.Windows.Forms.Button btnSavePSF;
        private System.Windows.Forms.RadioButton optRIF;
    }
}

