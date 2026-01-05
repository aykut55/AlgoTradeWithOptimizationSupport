namespace AlgoTradeWithOptimizationSupportWinFormsApp
{
    partial class Form1
    {
        /// <summary>
        ///  Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        ///  Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }

            if (disposing && statusTimer != null)
            {
                statusTimer.Stop();
                statusTimer.Dispose();
            }

            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code

        /// <summary>
        ///  Required method for Designer support - do not modify
        ///  the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            mainMenuStrip = new MenuStrip();
            fileToolStripMenuItem = new ToolStripMenuItem();
            newToolStripMenuItem = new ToolStripMenuItem();
            openToolStripMenuItem = new ToolStripMenuItem();
            saveToolStripMenuItem = new ToolStripMenuItem();
            saveAsToolStripMenuItem = new ToolStripMenuItem();
            exitToolStripMenuItem = new ToolStripMenuItem();
            editToolStripMenuItem = new ToolStripMenuItem();
            undoToolStripMenuItem = new ToolStripMenuItem();
            redoToolStripMenuItem = new ToolStripMenuItem();
            cutToolStripMenuItem = new ToolStripMenuItem();
            copyToolStripMenuItem = new ToolStripMenuItem();
            pasteToolStripMenuItem = new ToolStripMenuItem();
            selectAllToolStripMenuItem = new ToolStripMenuItem();
            viewToolStripMenuItem = new ToolStripMenuItem();
            toolbarsToolStripMenuItem = new ToolStripMenuItem();
            defaultToolbarsToolStripMenuItem = new ToolStripMenuItem();
            hideAllToolbarsToolStripMenuItem = new ToolStripMenuItem();
            showAllToolbarsToolStripMenuItem = new ToolStripMenuItem();
            toolStripSeparator4 = new ToolStripSeparator();
            mainToolStrip1ToolStripMenuItem = new ToolStripMenuItem();
            mainToolStrip2ToolStripMenuItem = new ToolStripMenuItem();
            statusBarToolStripMenuItem = new ToolStripMenuItem();
            panelsToolStripMenuItem = new ToolStripMenuItem();
            defaultPanelsToolStripMenuItem = new ToolStripMenuItem();
            hideAllPanelsToolStripMenuItem = new ToolStripMenuItem();
            showAllPanelsToolStripMenuItem = new ToolStripMenuItem();
            toolStripSeparator3 = new ToolStripSeparator();
            topPanelToolStripMenuItem = new ToolStripMenuItem();
            leftPanelToolStripMenuItem = new ToolStripMenuItem();
            rightPanelToolStripMenuItem = new ToolStripMenuItem();
            bottomPanelToolStripMenuItem = new ToolStripMenuItem();
            fullScreenToolStripMenuItem = new ToolStripMenuItem();
            toolsToolStripMenuItem = new ToolStripMenuItem();
            optionsToolStripMenuItem = new ToolStripMenuItem();
            preferencesToolStripMenuItem = new ToolStripMenuItem();
            helpToolStripMenuItem = new ToolStripMenuItem();
            documentationToolStripMenuItem = new ToolStripMenuItem();
            aboutToolStripMenuItem = new ToolStripMenuItem();
            mainToolStrip1 = new ToolStrip();
            newToolStripButton = new ToolStripButton();
            openToolStripButton = new ToolStripButton();
            saveToolStripButton = new ToolStripButton();
            toolStripSeparator1 = new ToolStripSeparator();
            cutToolStripButton = new ToolStripButton();
            copyToolStripButton = new ToolStripButton();
            pasteToolStripButton = new ToolStripButton();
            toolStripSeparator2 = new ToolStripSeparator();
            helpToolStripButton = new ToolStripButton();
            mainToolStrip2 = new ToolStrip();
            runStrategyButton = new ToolStripButton();
            stopStrategyButton = new ToolStripButton();
            toolStripSeparator5 = new ToolStripSeparator();
            optimizeButton = new ToolStripButton();
            exportResultsButton = new ToolStripButton();
            toolStripSeparator6 = new ToolStripSeparator();
            settingsButton = new ToolStripButton();
            statusStrip = new StatusStrip();
            statusLabel = new ToolStripStatusLabel();
            progressBar = new ToolStripProgressBar();
            spacerLabel = new ToolStripStatusLabel();
            timeLabel = new ToolStripStatusLabel();
            topPanel = new Panel();
            leftPanel = new Panel();
            rightPanel = new Panel();
            bottomPanel = new Panel();
            richTextBox1 = new RichTextBox();
            centerPanel = new Panel();
            mainTabControl = new TabControl();
            tabPageStockDataReader = new TabPage();
            panel3 = new Panel();
            panel2 = new Panel();
            panel1 = new Panel();
            lblReadStockDataTime = new Label();
            btnLastRow = new Button();
            btnNextRow = new Button();
            btnPrevRow = new Button();
            btnFirstRow = new Button();
            stockDataGridViewLabel = new Label();
            stockDataGridView = new DataGridView();
            textBoxMetaData = new TextBox();
            btnFirstPage = new Button();
            btnPreviousPage = new Button();
            btnNextPage = new Button();
            btnLastPage = new Button();
            cmbPageSize = new ComboBox();
            lblPageSize = new Label();
            groupBox1 = new GroupBox();
            btnClearStockData = new Button();
            btnClearLogFiles = new Button();
            btnSaveFile2 = new Button();
            btnBrowseFile2 = new Button();
            txtConfigFileName = new TextBox();
            lblConfigFileName = new Label();
            btnUpdateFilters = new Button();
            btnSaveConfigFile = new Button();
            btnReadConfigFile = new Button();
            btnClear = new Button();
            dtpFilterDateTime2 = new DateTimePicker();
            dtpFilterDateTime1 = new DateTimePicker();
            txtFilterValue2 = new TextBox();
            lblFilterValue2 = new Label();
            txtFilterValue1 = new TextBox();
            lblFilterValue1 = new Label();
            cmbFilterMode = new ComboBox();
            lblFilterMode = new Label();
            btnReadStockData = new Button();
            btnReadMetaData = new Button();
            btnSaveFile1 = new Button();
            btnBrowseFile1 = new Button();
            txtDataFileName = new TextBox();
            lblDataFileName = new Label();
            tabPageSingleTrader = new TabPage();
            panel4 = new Panel();
            btnStopSingleTrader = new Button();
            btnStartMultipleTrader = new Button();
            btnStartSingleTrader = new Button();
            progressBarSingleTrader = new ProgressBar();
            lblSingleTraderProgress = new Label();
            button1 = new Button();
            richTextBoxSingleTrader = new RichTextBox();
            btnTestAlgoTrader = new Button();
            tabPageMultipleTraders = new TabPage();
            tabPageSingleTraderOptimization = new TabPage();
            panel5 = new Panel();
            groupBox2 = new GroupBox();
            label4 = new Label();
            btnStopSingleTraderOpt = new Button();
            btnStartSingleTraderOpt = new Button();
            label2 = new Label();
            label1 = new Label();
            lblSingleTraderProgress2 = new Label();
            lblOptimizationProgress = new Label();
            lblSkipIteration = new Label();
            txtSkipIteration = new TextBox();
            lblMaxIteration = new Label();
            txtMaxIteration = new TextBox();
            progressBarSingleTraderProgress = new ProgressBar();
            progressBarOptimizationProgress = new ProgressBar();
            richTextBoxSingleTraderOptimization = new RichTextBox();
            lblOptimizationResult = new Label();
            label3 = new Label();
            lblOptimizationResults = new Label();
            dataGridViewOptimizationResults = new DataGridView();
            openFileDialog1 = new OpenFileDialog();
            saveFileDialog1 = new SaveFileDialog();
            openFileDialog2 = new OpenFileDialog();
            saveFileDialog2 = new SaveFileDialog();
            btnStopMultipleTrader = new Button();
            mainMenuStrip.SuspendLayout();
            mainToolStrip1.SuspendLayout();
            mainToolStrip2.SuspendLayout();
            statusStrip.SuspendLayout();
            bottomPanel.SuspendLayout();
            centerPanel.SuspendLayout();
            mainTabControl.SuspendLayout();
            tabPageStockDataReader.SuspendLayout();
            panel2.SuspendLayout();
            panel1.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)stockDataGridView).BeginInit();
            groupBox1.SuspendLayout();
            tabPageSingleTrader.SuspendLayout();
            panel4.SuspendLayout();
            tabPageSingleTraderOptimization.SuspendLayout();
            panel5.SuspendLayout();
            groupBox2.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)dataGridViewOptimizationResults).BeginInit();
            SuspendLayout();
            // 
            // mainMenuStrip
            // 
            mainMenuStrip.Items.AddRange(new ToolStripItem[] { fileToolStripMenuItem, editToolStripMenuItem, viewToolStripMenuItem, toolsToolStripMenuItem, helpToolStripMenuItem });
            mainMenuStrip.Location = new Point(0, 0);
            mainMenuStrip.Name = "mainMenuStrip";
            mainMenuStrip.Size = new Size(1599, 24);
            mainMenuStrip.TabIndex = 0;
            mainMenuStrip.Text = "mainMenuStrip";
            // 
            // fileToolStripMenuItem
            // 
            fileToolStripMenuItem.DropDownItems.AddRange(new ToolStripItem[] { newToolStripMenuItem, openToolStripMenuItem, saveToolStripMenuItem, saveAsToolStripMenuItem, exitToolStripMenuItem });
            fileToolStripMenuItem.Name = "fileToolStripMenuItem";
            fileToolStripMenuItem.Size = new Size(37, 20);
            fileToolStripMenuItem.Text = "&File";
            // 
            // newToolStripMenuItem
            // 
            newToolStripMenuItem.Name = "newToolStripMenuItem";
            newToolStripMenuItem.ShortcutKeys = Keys.Control | Keys.N;
            newToolStripMenuItem.Size = new Size(146, 22);
            newToolStripMenuItem.Text = "&New";
            newToolStripMenuItem.Click += newToolStripMenuItem_Click;
            // 
            // openToolStripMenuItem
            // 
            openToolStripMenuItem.Name = "openToolStripMenuItem";
            openToolStripMenuItem.ShortcutKeys = Keys.Control | Keys.O;
            openToolStripMenuItem.Size = new Size(146, 22);
            openToolStripMenuItem.Text = "&Open";
            openToolStripMenuItem.Click += openToolStripMenuItem_Click;
            // 
            // saveToolStripMenuItem
            // 
            saveToolStripMenuItem.Name = "saveToolStripMenuItem";
            saveToolStripMenuItem.ShortcutKeys = Keys.Control | Keys.S;
            saveToolStripMenuItem.Size = new Size(146, 22);
            saveToolStripMenuItem.Text = "&Save";
            saveToolStripMenuItem.Click += saveToolStripMenuItem_Click;
            // 
            // saveAsToolStripMenuItem
            // 
            saveAsToolStripMenuItem.Name = "saveAsToolStripMenuItem";
            saveAsToolStripMenuItem.Size = new Size(146, 22);
            saveAsToolStripMenuItem.Text = "Save &As...";
            saveAsToolStripMenuItem.Click += saveAsToolStripMenuItem_Click;
            // 
            // exitToolStripMenuItem
            // 
            exitToolStripMenuItem.Name = "exitToolStripMenuItem";
            exitToolStripMenuItem.Size = new Size(146, 22);
            exitToolStripMenuItem.Text = "E&xit";
            exitToolStripMenuItem.Click += exitToolStripMenuItem_Click;
            // 
            // editToolStripMenuItem
            // 
            editToolStripMenuItem.DropDownItems.AddRange(new ToolStripItem[] { undoToolStripMenuItem, redoToolStripMenuItem, cutToolStripMenuItem, copyToolStripMenuItem, pasteToolStripMenuItem, selectAllToolStripMenuItem });
            editToolStripMenuItem.Name = "editToolStripMenuItem";
            editToolStripMenuItem.Size = new Size(39, 20);
            editToolStripMenuItem.Text = "&Edit";
            // 
            // undoToolStripMenuItem
            // 
            undoToolStripMenuItem.Name = "undoToolStripMenuItem";
            undoToolStripMenuItem.ShortcutKeys = Keys.Control | Keys.Z;
            undoToolStripMenuItem.Size = new Size(164, 22);
            undoToolStripMenuItem.Text = "&Undo";
            undoToolStripMenuItem.Click += undoToolStripMenuItem_Click;
            // 
            // redoToolStripMenuItem
            // 
            redoToolStripMenuItem.Name = "redoToolStripMenuItem";
            redoToolStripMenuItem.ShortcutKeys = Keys.Control | Keys.Y;
            redoToolStripMenuItem.Size = new Size(164, 22);
            redoToolStripMenuItem.Text = "&Redo";
            redoToolStripMenuItem.Click += redoToolStripMenuItem_Click;
            // 
            // cutToolStripMenuItem
            // 
            cutToolStripMenuItem.Name = "cutToolStripMenuItem";
            cutToolStripMenuItem.ShortcutKeys = Keys.Control | Keys.X;
            cutToolStripMenuItem.Size = new Size(164, 22);
            cutToolStripMenuItem.Text = "Cu&t";
            cutToolStripMenuItem.Click += cutToolStripMenuItem_Click;
            // 
            // copyToolStripMenuItem
            // 
            copyToolStripMenuItem.Name = "copyToolStripMenuItem";
            copyToolStripMenuItem.ShortcutKeys = Keys.Control | Keys.C;
            copyToolStripMenuItem.Size = new Size(164, 22);
            copyToolStripMenuItem.Text = "&Copy";
            copyToolStripMenuItem.Click += copyToolStripMenuItem_Click;
            // 
            // pasteToolStripMenuItem
            // 
            pasteToolStripMenuItem.Name = "pasteToolStripMenuItem";
            pasteToolStripMenuItem.ShortcutKeys = Keys.Control | Keys.V;
            pasteToolStripMenuItem.Size = new Size(164, 22);
            pasteToolStripMenuItem.Text = "&Paste";
            pasteToolStripMenuItem.Click += pasteToolStripMenuItem_Click;
            // 
            // selectAllToolStripMenuItem
            // 
            selectAllToolStripMenuItem.Name = "selectAllToolStripMenuItem";
            selectAllToolStripMenuItem.ShortcutKeys = Keys.Control | Keys.A;
            selectAllToolStripMenuItem.Size = new Size(164, 22);
            selectAllToolStripMenuItem.Text = "Select &All";
            selectAllToolStripMenuItem.Click += selectAllToolStripMenuItem_Click;
            // 
            // viewToolStripMenuItem
            // 
            viewToolStripMenuItem.DropDownItems.AddRange(new ToolStripItem[] { toolbarsToolStripMenuItem, statusBarToolStripMenuItem, panelsToolStripMenuItem, fullScreenToolStripMenuItem });
            viewToolStripMenuItem.Name = "viewToolStripMenuItem";
            viewToolStripMenuItem.Size = new Size(44, 20);
            viewToolStripMenuItem.Text = "&View";
            // 
            // toolbarsToolStripMenuItem
            // 
            toolbarsToolStripMenuItem.DropDownItems.AddRange(new ToolStripItem[] { defaultToolbarsToolStripMenuItem, hideAllToolbarsToolStripMenuItem, showAllToolbarsToolStripMenuItem, toolStripSeparator4, mainToolStrip1ToolStripMenuItem, mainToolStrip2ToolStripMenuItem });
            toolbarsToolStripMenuItem.Name = "toolbarsToolStripMenuItem";
            toolbarsToolStripMenuItem.Size = new Size(156, 22);
            toolbarsToolStripMenuItem.Text = "&Toolbars";
            // 
            // defaultToolbarsToolStripMenuItem
            // 
            defaultToolbarsToolStripMenuItem.Name = "defaultToolbarsToolStripMenuItem";
            defaultToolbarsToolStripMenuItem.Size = new Size(160, 22);
            defaultToolbarsToolStripMenuItem.Text = "&Default";
            defaultToolbarsToolStripMenuItem.Click += defaultToolbarsToolStripMenuItem_Click;
            // 
            // hideAllToolbarsToolStripMenuItem
            // 
            hideAllToolbarsToolStripMenuItem.Name = "hideAllToolbarsToolStripMenuItem";
            hideAllToolbarsToolStripMenuItem.Size = new Size(160, 22);
            hideAllToolbarsToolStripMenuItem.Text = "&Hide All";
            hideAllToolbarsToolStripMenuItem.Click += hideAllToolbarsToolStripMenuItem_Click;
            // 
            // showAllToolbarsToolStripMenuItem
            // 
            showAllToolbarsToolStripMenuItem.Name = "showAllToolbarsToolStripMenuItem";
            showAllToolbarsToolStripMenuItem.Size = new Size(160, 22);
            showAllToolbarsToolStripMenuItem.Text = "&Show All";
            showAllToolbarsToolStripMenuItem.Click += showAllToolbarsToolStripMenuItem_Click;
            // 
            // toolStripSeparator4
            // 
            toolStripSeparator4.Name = "toolStripSeparator4";
            toolStripSeparator4.Size = new Size(157, 6);
            // 
            // mainToolStrip1ToolStripMenuItem
            // 
            mainToolStrip1ToolStripMenuItem.Checked = true;
            mainToolStrip1ToolStripMenuItem.CheckState = CheckState.Checked;
            mainToolStrip1ToolStripMenuItem.Name = "mainToolStrip1ToolStripMenuItem";
            mainToolStrip1ToolStripMenuItem.Size = new Size(160, 22);
            mainToolStrip1ToolStripMenuItem.Text = "Main ToolStrip &1";
            mainToolStrip1ToolStripMenuItem.Click += mainToolStrip1ToolStripMenuItem_Click;
            // 
            // mainToolStrip2ToolStripMenuItem
            // 
            mainToolStrip2ToolStripMenuItem.Name = "mainToolStrip2ToolStripMenuItem";
            mainToolStrip2ToolStripMenuItem.Size = new Size(160, 22);
            mainToolStrip2ToolStripMenuItem.Text = "Main ToolStrip &2";
            mainToolStrip2ToolStripMenuItem.Click += mainToolStrip2ToolStripMenuItem_Click;
            // 
            // statusBarToolStripMenuItem
            // 
            statusBarToolStripMenuItem.Checked = true;
            statusBarToolStripMenuItem.CheckState = CheckState.Checked;
            statusBarToolStripMenuItem.Name = "statusBarToolStripMenuItem";
            statusBarToolStripMenuItem.Size = new Size(156, 22);
            statusBarToolStripMenuItem.Text = "&Status Bar";
            statusBarToolStripMenuItem.Click += statusBarToolStripMenuItem_Click;
            // 
            // panelsToolStripMenuItem
            // 
            panelsToolStripMenuItem.DropDownItems.AddRange(new ToolStripItem[] { defaultPanelsToolStripMenuItem, hideAllPanelsToolStripMenuItem, showAllPanelsToolStripMenuItem, toolStripSeparator3, topPanelToolStripMenuItem, leftPanelToolStripMenuItem, rightPanelToolStripMenuItem, bottomPanelToolStripMenuItem });
            panelsToolStripMenuItem.Name = "panelsToolStripMenuItem";
            panelsToolStripMenuItem.Size = new Size(156, 22);
            panelsToolStripMenuItem.Text = "&Panels";
            // 
            // defaultPanelsToolStripMenuItem
            // 
            defaultPanelsToolStripMenuItem.Name = "defaultPanelsToolStripMenuItem";
            defaultPanelsToolStripMenuItem.Size = new Size(146, 22);
            defaultPanelsToolStripMenuItem.Text = "&Default";
            defaultPanelsToolStripMenuItem.Click += defaultPanelsToolStripMenuItem_Click;
            // 
            // hideAllPanelsToolStripMenuItem
            // 
            hideAllPanelsToolStripMenuItem.Name = "hideAllPanelsToolStripMenuItem";
            hideAllPanelsToolStripMenuItem.Size = new Size(146, 22);
            hideAllPanelsToolStripMenuItem.Text = "&Hide All";
            hideAllPanelsToolStripMenuItem.Click += hideAllPanelsToolStripMenuItem_Click;
            // 
            // showAllPanelsToolStripMenuItem
            // 
            showAllPanelsToolStripMenuItem.Name = "showAllPanelsToolStripMenuItem";
            showAllPanelsToolStripMenuItem.Size = new Size(146, 22);
            showAllPanelsToolStripMenuItem.Text = "&Show All";
            showAllPanelsToolStripMenuItem.Click += showAllPanelsToolStripMenuItem_Click;
            // 
            // toolStripSeparator3
            // 
            toolStripSeparator3.Name = "toolStripSeparator3";
            toolStripSeparator3.Size = new Size(143, 6);
            // 
            // topPanelToolStripMenuItem
            // 
            topPanelToolStripMenuItem.Name = "topPanelToolStripMenuItem";
            topPanelToolStripMenuItem.Size = new Size(146, 22);
            topPanelToolStripMenuItem.Text = "&Top Panel";
            topPanelToolStripMenuItem.Click += topPanelToolStripMenuItem_Click;
            // 
            // leftPanelToolStripMenuItem
            // 
            leftPanelToolStripMenuItem.Checked = true;
            leftPanelToolStripMenuItem.CheckState = CheckState.Checked;
            leftPanelToolStripMenuItem.Name = "leftPanelToolStripMenuItem";
            leftPanelToolStripMenuItem.Size = new Size(146, 22);
            leftPanelToolStripMenuItem.Text = "&Left Panel";
            leftPanelToolStripMenuItem.Click += leftPanelToolStripMenuItem_Click;
            // 
            // rightPanelToolStripMenuItem
            // 
            rightPanelToolStripMenuItem.Name = "rightPanelToolStripMenuItem";
            rightPanelToolStripMenuItem.Size = new Size(146, 22);
            rightPanelToolStripMenuItem.Text = "&Right Panel";
            rightPanelToolStripMenuItem.Click += rightPanelToolStripMenuItem_Click;
            // 
            // bottomPanelToolStripMenuItem
            // 
            bottomPanelToolStripMenuItem.Checked = true;
            bottomPanelToolStripMenuItem.CheckState = CheckState.Checked;
            bottomPanelToolStripMenuItem.Name = "bottomPanelToolStripMenuItem";
            bottomPanelToolStripMenuItem.Size = new Size(146, 22);
            bottomPanelToolStripMenuItem.Text = "&Bottom Panel";
            bottomPanelToolStripMenuItem.Click += bottomPanelToolStripMenuItem_Click;
            // 
            // fullScreenToolStripMenuItem
            // 
            fullScreenToolStripMenuItem.Name = "fullScreenToolStripMenuItem";
            fullScreenToolStripMenuItem.ShortcutKeys = Keys.F11;
            fullScreenToolStripMenuItem.Size = new Size(156, 22);
            fullScreenToolStripMenuItem.Text = "&Full Screen";
            fullScreenToolStripMenuItem.Click += fullScreenToolStripMenuItem_Click;
            // 
            // toolsToolStripMenuItem
            // 
            toolsToolStripMenuItem.DropDownItems.AddRange(new ToolStripItem[] { optionsToolStripMenuItem, preferencesToolStripMenuItem });
            toolsToolStripMenuItem.Name = "toolsToolStripMenuItem";
            toolsToolStripMenuItem.Size = new Size(47, 20);
            toolsToolStripMenuItem.Text = "&Tools";
            // 
            // optionsToolStripMenuItem
            // 
            optionsToolStripMenuItem.Name = "optionsToolStripMenuItem";
            optionsToolStripMenuItem.Size = new Size(135, 22);
            optionsToolStripMenuItem.Text = "&Options";
            optionsToolStripMenuItem.Click += optionsToolStripMenuItem_Click;
            // 
            // preferencesToolStripMenuItem
            // 
            preferencesToolStripMenuItem.Name = "preferencesToolStripMenuItem";
            preferencesToolStripMenuItem.Size = new Size(135, 22);
            preferencesToolStripMenuItem.Text = "&Preferences";
            preferencesToolStripMenuItem.Click += preferencesToolStripMenuItem_Click;
            // 
            // helpToolStripMenuItem
            // 
            helpToolStripMenuItem.DropDownItems.AddRange(new ToolStripItem[] { documentationToolStripMenuItem, aboutToolStripMenuItem });
            helpToolStripMenuItem.Name = "helpToolStripMenuItem";
            helpToolStripMenuItem.Size = new Size(44, 20);
            helpToolStripMenuItem.Text = "&Help";
            // 
            // documentationToolStripMenuItem
            // 
            documentationToolStripMenuItem.Name = "documentationToolStripMenuItem";
            documentationToolStripMenuItem.ShortcutKeys = Keys.F1;
            documentationToolStripMenuItem.Size = new Size(176, 22);
            documentationToolStripMenuItem.Text = "&Documentation";
            documentationToolStripMenuItem.Click += documentationToolStripMenuItem_Click;
            // 
            // aboutToolStripMenuItem
            // 
            aboutToolStripMenuItem.Name = "aboutToolStripMenuItem";
            aboutToolStripMenuItem.Size = new Size(176, 22);
            aboutToolStripMenuItem.Text = "&About";
            aboutToolStripMenuItem.Click += aboutToolStripMenuItem_Click;
            // 
            // mainToolStrip1
            // 
            mainToolStrip1.Items.AddRange(new ToolStripItem[] { newToolStripButton, openToolStripButton, saveToolStripButton, toolStripSeparator1, cutToolStripButton, copyToolStripButton, pasteToolStripButton, toolStripSeparator2, helpToolStripButton });
            mainToolStrip1.Location = new Point(0, 24);
            mainToolStrip1.Name = "mainToolStrip1";
            mainToolStrip1.Size = new Size(1599, 25);
            mainToolStrip1.TabIndex = 1;
            mainToolStrip1.ItemClicked += mainToolStrip_ItemClicked;
            // 
            // newToolStripButton
            // 
            newToolStripButton.DisplayStyle = ToolStripItemDisplayStyle.Text;
            newToolStripButton.Name = "newToolStripButton";
            newToolStripButton.Size = new Size(50, 22);
            newToolStripButton.Text = "📄 New";
            newToolStripButton.ToolTipText = "Create a new file (Ctrl+N)";
            // 
            // openToolStripButton
            // 
            openToolStripButton.DisplayStyle = ToolStripItemDisplayStyle.Text;
            openToolStripButton.Name = "openToolStripButton";
            openToolStripButton.Size = new Size(55, 22);
            openToolStripButton.Text = "📁 Open";
            openToolStripButton.ToolTipText = "Open an existing file (Ctrl+O)";
            // 
            // saveToolStripButton
            // 
            saveToolStripButton.DisplayStyle = ToolStripItemDisplayStyle.Text;
            saveToolStripButton.Name = "saveToolStripButton";
            saveToolStripButton.Size = new Size(50, 22);
            saveToolStripButton.Text = "💾 Save";
            saveToolStripButton.ToolTipText = "Save the current file (Ctrl+S)";
            // 
            // toolStripSeparator1
            // 
            toolStripSeparator1.Name = "toolStripSeparator1";
            toolStripSeparator1.Size = new Size(6, 25);
            // 
            // cutToolStripButton
            // 
            cutToolStripButton.DisplayStyle = ToolStripItemDisplayStyle.Text;
            cutToolStripButton.Name = "cutToolStripButton";
            cutToolStripButton.Size = new Size(45, 22);
            cutToolStripButton.Text = "✂️ Cut";
            cutToolStripButton.ToolTipText = "Cut selected content (Ctrl+X)";
            // 
            // copyToolStripButton
            // 
            copyToolStripButton.DisplayStyle = ToolStripItemDisplayStyle.Text;
            copyToolStripButton.Name = "copyToolStripButton";
            copyToolStripButton.Size = new Size(54, 22);
            copyToolStripButton.Text = "📋 Copy";
            copyToolStripButton.ToolTipText = "Copy selected content (Ctrl+C)";
            // 
            // pasteToolStripButton
            // 
            pasteToolStripButton.DisplayStyle = ToolStripItemDisplayStyle.Text;
            pasteToolStripButton.Name = "pasteToolStripButton";
            pasteToolStripButton.Size = new Size(54, 22);
            pasteToolStripButton.Text = "📌 Paste";
            pasteToolStripButton.ToolTipText = "Paste clipboard content (Ctrl+V)";
            // 
            // toolStripSeparator2
            // 
            toolStripSeparator2.Name = "toolStripSeparator2";
            toolStripSeparator2.Size = new Size(6, 25);
            // 
            // helpToolStripButton
            // 
            helpToolStripButton.DisplayStyle = ToolStripItemDisplayStyle.Text;
            helpToolStripButton.Name = "helpToolStripButton";
            helpToolStripButton.Size = new Size(51, 22);
            helpToolStripButton.Text = "❓ Help";
            helpToolStripButton.ToolTipText = "Show help documentation (F1)";
            // 
            // mainToolStrip2
            // 
            mainToolStrip2.Items.AddRange(new ToolStripItem[] { runStrategyButton, stopStrategyButton, toolStripSeparator5, optimizeButton, exportResultsButton, toolStripSeparator6, settingsButton });
            mainToolStrip2.Location = new Point(0, 49);
            mainToolStrip2.Name = "mainToolStrip2";
            mainToolStrip2.Size = new Size(1200, 25);
            mainToolStrip2.TabIndex = 8;
            mainToolStrip2.Text = "mainToolStrip2";
            mainToolStrip2.Visible = false;
            mainToolStrip2.ItemClicked += mainToolStrip2_ItemClicked;
            // 
            // runStrategyButton
            // 
            runStrategyButton.DisplayStyle = ToolStripItemDisplayStyle.Text;
            runStrategyButton.Name = "runStrategyButton";
            runStrategyButton.Size = new Size(45, 22);
            runStrategyButton.Text = "▶️ Run";
            runStrategyButton.ToolTipText = "Run selected strategy";
            // 
            // stopStrategyButton
            // 
            stopStrategyButton.DisplayStyle = ToolStripItemDisplayStyle.Text;
            stopStrategyButton.Name = "stopStrategyButton";
            stopStrategyButton.Size = new Size(50, 22);
            stopStrategyButton.Text = "⏹️ Stop";
            stopStrategyButton.ToolTipText = "Stop running strategy";
            // 
            // toolStripSeparator5
            // 
            toolStripSeparator5.Name = "toolStripSeparator5";
            toolStripSeparator5.Size = new Size(6, 25);
            // 
            // optimizeButton
            // 
            optimizeButton.DisplayStyle = ToolStripItemDisplayStyle.Text;
            optimizeButton.Name = "optimizeButton";
            optimizeButton.Size = new Size(74, 22);
            optimizeButton.Text = "🎯 Optimize";
            optimizeButton.ToolTipText = "Optimize strategy parameters";
            // 
            // exportResultsButton
            // 
            exportResultsButton.DisplayStyle = ToolStripItemDisplayStyle.Text;
            exportResultsButton.Name = "exportResultsButton";
            exportResultsButton.Size = new Size(59, 22);
            exportResultsButton.Text = "📊 Export";
            exportResultsButton.ToolTipText = "Export results to file";
            // 
            // toolStripSeparator6
            // 
            toolStripSeparator6.Name = "toolStripSeparator6";
            toolStripSeparator6.Size = new Size(6, 25);
            // 
            // settingsButton
            // 
            settingsButton.DisplayStyle = ToolStripItemDisplayStyle.Text;
            settingsButton.Name = "settingsButton";
            settingsButton.Size = new Size(68, 22);
            settingsButton.Text = "⚙️ Settings";
            settingsButton.ToolTipText = "Strategy settings";
            // 
            // statusStrip
            // 
            statusStrip.Items.AddRange(new ToolStripItem[] { statusLabel, progressBar, spacerLabel, timeLabel });
            statusStrip.Location = new Point(0, 1036);
            statusStrip.Name = "statusStrip";
            statusStrip.Size = new Size(1599, 25);
            statusStrip.TabIndex = 2;
            // 
            // statusLabel
            // 
            statusLabel.Name = "statusLabel";
            statusLabel.Size = new Size(39, 20);
            statusLabel.Text = "Ready";
            // 
            // progressBar
            // 
            progressBar.Name = "progressBar";
            progressBar.Size = new Size(100, 19);
            // 
            // spacerLabel
            // 
            spacerLabel.Name = "spacerLabel";
            spacerLabel.Size = new Size(1394, 20);
            spacerLabel.Spring = true;
            // 
            // timeLabel
            // 
            timeLabel.Name = "timeLabel";
            timeLabel.Size = new Size(49, 20);
            timeLabel.Text = "00:00:00";
            timeLabel.TextAlign = ContentAlignment.MiddleRight;
            // 
            // topPanel
            // 
            topPanel.BorderStyle = BorderStyle.FixedSingle;
            topPanel.Dock = DockStyle.Top;
            topPanel.Location = new Point(0, 49);
            topPanel.Name = "topPanel";
            topPanel.Size = new Size(1599, 27);
            topPanel.TabIndex = 3;
            topPanel.Visible = false;
            // 
            // leftPanel
            // 
            leftPanel.BorderStyle = BorderStyle.FixedSingle;
            leftPanel.Dock = DockStyle.Left;
            leftPanel.Location = new Point(0, 76);
            leftPanel.Name = "leftPanel";
            leftPanel.Size = new Size(103, 823);
            leftPanel.TabIndex = 5;
            // 
            // rightPanel
            // 
            rightPanel.BorderStyle = BorderStyle.FixedSingle;
            rightPanel.Dock = DockStyle.Right;
            rightPanel.Location = new Point(1528, 76);
            rightPanel.Name = "rightPanel";
            rightPanel.Size = new Size(71, 823);
            rightPanel.TabIndex = 6;
            rightPanel.Visible = false;
            // 
            // bottomPanel
            // 
            bottomPanel.BorderStyle = BorderStyle.FixedSingle;
            bottomPanel.Controls.Add(richTextBox1);
            bottomPanel.Dock = DockStyle.Bottom;
            bottomPanel.Location = new Point(0, 899);
            bottomPanel.Name = "bottomPanel";
            bottomPanel.Size = new Size(1599, 137);
            bottomPanel.TabIndex = 4;
            // 
            // richTextBox1
            // 
            richTextBox1.Dock = DockStyle.Fill;
            richTextBox1.Location = new Point(0, 0);
            richTextBox1.Name = "richTextBox1";
            richTextBox1.Size = new Size(1597, 135);
            richTextBox1.TabIndex = 0;
            richTextBox1.Text = "";
            // 
            // centerPanel
            // 
            centerPanel.BorderStyle = BorderStyle.FixedSingle;
            centerPanel.Controls.Add(mainTabControl);
            centerPanel.Dock = DockStyle.Fill;
            centerPanel.Location = new Point(103, 76);
            centerPanel.Name = "centerPanel";
            centerPanel.Size = new Size(1425, 823);
            centerPanel.TabIndex = 7;
            // 
            // mainTabControl
            // 
            mainTabControl.Controls.Add(tabPageStockDataReader);
            mainTabControl.Controls.Add(tabPageSingleTrader);
            mainTabControl.Controls.Add(tabPageMultipleTraders);
            mainTabControl.Controls.Add(tabPageSingleTraderOptimization);
            mainTabControl.Dock = DockStyle.Fill;
            mainTabControl.Location = new Point(0, 0);
            mainTabControl.Name = "mainTabControl";
            mainTabControl.SelectedIndex = 0;
            mainTabControl.Size = new Size(1423, 821);
            mainTabControl.TabIndex = 0;
            // 
            // tabPageStockDataReader
            // 
            tabPageStockDataReader.BackColor = Color.White;
            tabPageStockDataReader.Controls.Add(panel3);
            tabPageStockDataReader.Controls.Add(panel2);
            tabPageStockDataReader.Location = new Point(4, 24);
            tabPageStockDataReader.Name = "tabPageStockDataReader";
            tabPageStockDataReader.Padding = new Padding(3);
            tabPageStockDataReader.Size = new Size(1415, 793);
            tabPageStockDataReader.TabIndex = 0;
            tabPageStockDataReader.Text = "StockDataReader";
            // 
            // panel3
            // 
            panel3.Dock = DockStyle.Fill;
            panel3.Location = new Point(1093, 3);
            panel3.Name = "panel3";
            panel3.Size = new Size(319, 787);
            panel3.TabIndex = 3;
            // 
            // panel2
            // 
            panel2.Controls.Add(panel1);
            panel2.Controls.Add(groupBox1);
            panel2.Dock = DockStyle.Left;
            panel2.Location = new Point(3, 3);
            panel2.Name = "panel2";
            panel2.Size = new Size(1090, 787);
            panel2.TabIndex = 2;
            // 
            // panel1
            // 
            panel1.Controls.Add(lblReadStockDataTime);
            panel1.Controls.Add(btnLastRow);
            panel1.Controls.Add(btnNextRow);
            panel1.Controls.Add(btnPrevRow);
            panel1.Controls.Add(btnFirstRow);
            panel1.Controls.Add(stockDataGridViewLabel);
            panel1.Controls.Add(stockDataGridView);
            panel1.Controls.Add(textBoxMetaData);
            panel1.Controls.Add(btnFirstPage);
            panel1.Controls.Add(btnPreviousPage);
            panel1.Controls.Add(btnNextPage);
            panel1.Controls.Add(btnLastPage);
            panel1.Controls.Add(cmbPageSize);
            panel1.Controls.Add(lblPageSize);
            panel1.Dock = DockStyle.Fill;
            panel1.Location = new Point(0, 219);
            panel1.Name = "panel1";
            panel1.Size = new Size(1090, 568);
            panel1.TabIndex = 2;
            // 
            // lblReadStockDataTime
            // 
            lblReadStockDataTime.AutoSize = true;
            lblReadStockDataTime.Location = new Point(832, 11);
            lblReadStockDataTime.Name = "lblReadStockDataTime";
            lblReadStockDataTime.Size = new Size(152, 15);
            lblReadStockDataTime.TabIndex = 29;
            lblReadStockDataTime.Text = "Last StockData Read Time : ";
            // 
            // btnLastRow
            // 
            btnLastRow.Location = new Point(1001, 128);
            btnLastRow.Name = "btnLastRow";
            btnLastRow.Size = new Size(75, 23);
            btnLastRow.TabIndex = 22;
            btnLastRow.Text = "Last ▶|";
            btnLastRow.UseVisualStyleBackColor = true;
            btnLastRow.Click += BtnLastRow_Click;
            // 
            // btnNextRow
            // 
            btnNextRow.Location = new Point(920, 128);
            btnNextRow.Name = "btnNextRow";
            btnNextRow.Size = new Size(75, 23);
            btnNextRow.TabIndex = 21;
            btnNextRow.Text = "Next ▶";
            btnNextRow.UseVisualStyleBackColor = true;
            btnNextRow.Click += BtnNextRow_Click;
            // 
            // btnPrevRow
            // 
            btnPrevRow.Location = new Point(832, 128);
            btnPrevRow.Name = "btnPrevRow";
            btnPrevRow.Size = new Size(82, 23);
            btnPrevRow.TabIndex = 20;
            btnPrevRow.Text = "◀ Previous";
            btnPrevRow.UseVisualStyleBackColor = true;
            btnPrevRow.Click += BtnPrevRow_Click;
            // 
            // btnFirstRow
            // 
            btnFirstRow.Location = new Point(751, 128);
            btnFirstRow.Name = "btnFirstRow";
            btnFirstRow.Size = new Size(75, 23);
            btnFirstRow.TabIndex = 19;
            btnFirstRow.Text = "|◀ First";
            btnFirstRow.UseVisualStyleBackColor = true;
            btnFirstRow.Click += BtnFirstRow_Click;
            // 
            // stockDataGridViewLabel
            // 
            stockDataGridViewLabel.AutoSize = true;
            stockDataGridViewLabel.Location = new Point(832, 95);
            stockDataGridViewLabel.Name = "stockDataGridViewLabel";
            stockDataGridViewLabel.Size = new Size(158, 15);
            stockDataGridViewLabel.TabIndex = 18;
            stockDataGridViewLabel.Text = "Row: 0/0 | Page: 0/0 | Total: 0";
            // 
            // stockDataGridView
            // 
            stockDataGridView.ColumnHeadersHeightSizeMode = DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            stockDataGridView.Location = new Point(15, 157);
            stockDataGridView.Name = "stockDataGridView";
            stockDataGridView.Size = new Size(1086, 374);
            stockDataGridView.TabIndex = 17;
            // 
            // textBoxMetaData
            // 
            textBoxMetaData.Font = new Font("Consolas", 9F);
            textBoxMetaData.Location = new Point(15, 9);
            textBoxMetaData.Multiline = true;
            textBoxMetaData.Name = "textBoxMetaData";
            textBoxMetaData.ReadOnly = true;
            textBoxMetaData.ScrollBars = ScrollBars.Vertical;
            textBoxMetaData.Size = new Size(533, 112);
            textBoxMetaData.TabIndex = 16;
            textBoxMetaData.Text = "Kayit Zamani     : \r\nGrafikSembol     : \r\nGrafikPeriyot    : \r\nBarCount         : \r\nBaşlangiç Tarihi : \r\nBitiş Tarihi     : \r\nFormat           :\r\n";
            // 
            // btnFirstPage
            // 
            btnFirstPage.Location = new Point(220, 128);
            btnFirstPage.Name = "btnFirstPage";
            btnFirstPage.Size = new Size(75, 23);
            btnFirstPage.TabIndex = 23;
            btnFirstPage.Text = "|◀ First";
            btnFirstPage.UseVisualStyleBackColor = true;
            btnFirstPage.Click += BtnFirstPage_Click;
            // 
            // btnPreviousPage
            // 
            btnPreviousPage.Location = new Point(301, 128);
            btnPreviousPage.Name = "btnPreviousPage";
            btnPreviousPage.Size = new Size(85, 23);
            btnPreviousPage.TabIndex = 24;
            btnPreviousPage.Text = "◀ Previous";
            btnPreviousPage.UseVisualStyleBackColor = true;
            btnPreviousPage.Click += BtnPreviousPage_Click;
            // 
            // btnNextPage
            // 
            btnNextPage.Location = new Point(392, 128);
            btnNextPage.Name = "btnNextPage";
            btnNextPage.Size = new Size(75, 23);
            btnNextPage.TabIndex = 25;
            btnNextPage.Text = "Next ▶";
            btnNextPage.UseVisualStyleBackColor = true;
            btnNextPage.Click += BtnNextPage_Click;
            // 
            // btnLastPage
            // 
            btnLastPage.Location = new Point(473, 128);
            btnLastPage.Name = "btnLastPage";
            btnLastPage.Size = new Size(75, 23);
            btnLastPage.TabIndex = 26;
            btnLastPage.Text = "Last ▶|";
            btnLastPage.UseVisualStyleBackColor = true;
            btnLastPage.Click += BtnLastPage_Click;
            // 
            // cmbPageSize
            // 
            cmbPageSize.DropDownStyle = ComboBoxStyle.DropDownList;
            cmbPageSize.FormattingEnabled = true;
            cmbPageSize.Items.AddRange(new object[] { "100", "500", "1000", "5000", "10000" });
            cmbPageSize.Location = new Point(85, 128);
            cmbPageSize.Name = "cmbPageSize";
            cmbPageSize.Size = new Size(80, 23);
            cmbPageSize.TabIndex = 27;
            cmbPageSize.SelectedIndexChanged += CmbPageSize_SelectedIndexChanged;
            // 
            // lblPageSize
            // 
            lblPageSize.AutoSize = true;
            lblPageSize.Location = new Point(20, 132);
            lblPageSize.Name = "lblPageSize";
            lblPageSize.Size = new Size(59, 15);
            lblPageSize.TabIndex = 28;
            lblPageSize.Text = "Page Size:";
            // 
            // groupBox1
            // 
            groupBox1.Controls.Add(btnClearStockData);
            groupBox1.Controls.Add(btnClearLogFiles);
            groupBox1.Controls.Add(btnSaveFile2);
            groupBox1.Controls.Add(btnBrowseFile2);
            groupBox1.Controls.Add(txtConfigFileName);
            groupBox1.Controls.Add(lblConfigFileName);
            groupBox1.Controls.Add(btnUpdateFilters);
            groupBox1.Controls.Add(btnSaveConfigFile);
            groupBox1.Controls.Add(btnReadConfigFile);
            groupBox1.Controls.Add(btnClear);
            groupBox1.Controls.Add(dtpFilterDateTime2);
            groupBox1.Controls.Add(dtpFilterDateTime1);
            groupBox1.Controls.Add(txtFilterValue2);
            groupBox1.Controls.Add(lblFilterValue2);
            groupBox1.Controls.Add(txtFilterValue1);
            groupBox1.Controls.Add(lblFilterValue1);
            groupBox1.Controls.Add(cmbFilterMode);
            groupBox1.Controls.Add(lblFilterMode);
            groupBox1.Controls.Add(btnReadStockData);
            groupBox1.Controls.Add(btnReadMetaData);
            groupBox1.Controls.Add(btnSaveFile1);
            groupBox1.Controls.Add(btnBrowseFile1);
            groupBox1.Controls.Add(txtDataFileName);
            groupBox1.Controls.Add(lblDataFileName);
            groupBox1.Dock = DockStyle.Top;
            groupBox1.Location = new Point(0, 0);
            groupBox1.Name = "groupBox1";
            groupBox1.Size = new Size(1090, 219);
            groupBox1.TabIndex = 1;
            groupBox1.TabStop = false;
            groupBox1.Text = "Stock Data Reader";
            // 
            // btnClearStockData
            // 
            btnClearStockData.Location = new Point(823, 173);
            btnClearStockData.Name = "btnClearStockData";
            btnClearStockData.Size = new Size(100, 25);
            btnClearStockData.TabIndex = 22;
            btnClearStockData.Text = "Clear StockData";
            btnClearStockData.UseVisualStyleBackColor = true;
            btnClearStockData.Click += btnClearStockData_Click;
            // 
            // btnClearLogFiles
            // 
            btnClearLogFiles.Location = new Point(976, 45);
            btnClearLogFiles.Name = "btnClearLogFiles";
            btnClearLogFiles.Size = new Size(100, 25);
            btnClearLogFiles.TabIndex = 23;
            btnClearLogFiles.Text = "Clear Log Files";
            btnClearLogFiles.UseVisualStyleBackColor = true;
            btnClearLogFiles.Click += btnClearLogFiles_Click;
            // 
            // btnSaveFile2
            // 
            btnSaveFile2.Location = new Point(671, 45);
            btnSaveFile2.Name = "btnSaveFile2";
            btnSaveFile2.Size = new Size(30, 25);
            btnSaveFile2.TabIndex = 21;
            btnSaveFile2.Text = "💾";
            btnSaveFile2.UseVisualStyleBackColor = true;
            btnSaveFile2.Click += BtnSaveFile2_Click;
            // 
            // btnBrowseFile2
            // 
            btnBrowseFile2.Location = new Point(635, 45);
            btnBrowseFile2.Name = "btnBrowseFile2";
            btnBrowseFile2.Size = new Size(30, 25);
            btnBrowseFile2.TabIndex = 20;
            btnBrowseFile2.Text = "📁";
            btnBrowseFile2.UseVisualStyleBackColor = true;
            btnBrowseFile2.Click += BtnBrowseFile2_Click;
            // 
            // txtConfigFileName
            // 
            txtConfigFileName.Location = new Point(15, 45);
            txtConfigFileName.Name = "txtConfigFileName";
            txtConfigFileName.Size = new Size(614, 23);
            txtConfigFileName.TabIndex = 19;
            // 
            // lblConfigFileName
            // 
            lblConfigFileName.AutoSize = true;
            lblConfigFileName.Location = new Point(15, 27);
            lblConfigFileName.Name = "lblConfigFileName";
            lblConfigFileName.Size = new Size(96, 15);
            lblConfigFileName.TabIndex = 18;
            lblConfigFileName.Text = "ConfigFile Name";
            // 
            // btnUpdateFilters
            // 
            btnUpdateFilters.Location = new Point(823, 142);
            btnUpdateFilters.Name = "btnUpdateFilters";
            btnUpdateFilters.Size = new Size(100, 25);
            btnUpdateFilters.TabIndex = 17;
            btnUpdateFilters.Text = "Update Filters";
            btnUpdateFilters.UseVisualStyleBackColor = true;
            btnUpdateFilters.Click += btnUpdateFilters_Click;
            // 
            // btnSaveConfigFile
            // 
            btnSaveConfigFile.Location = new Point(823, 45);
            btnSaveConfigFile.Name = "btnSaveConfigFile";
            btnSaveConfigFile.Size = new Size(100, 25);
            btnSaveConfigFile.TabIndex = 16;
            btnSaveConfigFile.Text = "Save Config File";
            btnSaveConfigFile.UseVisualStyleBackColor = true;
            // 
            // btnReadConfigFile
            // 
            btnReadConfigFile.Location = new Point(717, 45);
            btnReadConfigFile.Name = "btnReadConfigFile";
            btnReadConfigFile.Size = new Size(100, 25);
            btnReadConfigFile.TabIndex = 15;
            btnReadConfigFile.Text = "Read Config File";
            btnReadConfigFile.UseVisualStyleBackColor = true;
            // 
            // btnClear
            // 
            btnClear.Location = new Point(717, 114);
            btnClear.Name = "btnClear";
            btnClear.Size = new Size(206, 25);
            btnClear.TabIndex = 14;
            btnClear.Text = "Clear";
            btnClear.UseVisualStyleBackColor = true;
            btnClear.Click += BtnClear_Click;
            // 
            // dtpFilterDateTime2
            // 
            dtpFilterDateTime2.Format = DateTimePickerFormat.Short;
            dtpFilterDateTime2.Location = new Point(531, 175);
            dtpFilterDateTime2.Name = "dtpFilterDateTime2";
            dtpFilterDateTime2.Size = new Size(142, 23);
            dtpFilterDateTime2.TabIndex = 11;
            // 
            // dtpFilterDateTime1
            // 
            dtpFilterDateTime1.Format = DateTimePickerFormat.Short;
            dtpFilterDateTime1.Location = new Point(383, 175);
            dtpFilterDateTime1.Name = "dtpFilterDateTime1";
            dtpFilterDateTime1.Size = new Size(142, 23);
            dtpFilterDateTime1.TabIndex = 10;
            // 
            // txtFilterValue2
            // 
            txtFilterValue2.Location = new Point(277, 175);
            txtFilterValue2.Name = "txtFilterValue2";
            txtFilterValue2.Size = new Size(100, 23);
            txtFilterValue2.TabIndex = 9;
            // 
            // lblFilterValue2
            // 
            lblFilterValue2.AutoSize = true;
            lblFilterValue2.Location = new Point(277, 157);
            lblFilterValue2.Name = "lblFilterValue2";
            lblFilterValue2.Size = new Size(44, 15);
            lblFilterValue2.TabIndex = 8;
            lblFilterValue2.Text = "Value 2";
            // 
            // txtFilterValue1
            // 
            txtFilterValue1.Location = new Point(171, 175);
            txtFilterValue1.Name = "txtFilterValue1";
            txtFilterValue1.Size = new Size(100, 23);
            txtFilterValue1.TabIndex = 7;
            // 
            // lblFilterValue1
            // 
            lblFilterValue1.AutoSize = true;
            lblFilterValue1.Location = new Point(171, 157);
            lblFilterValue1.Name = "lblFilterValue1";
            lblFilterValue1.Size = new Size(44, 15);
            lblFilterValue1.TabIndex = 6;
            lblFilterValue1.Text = "Value 1";
            // 
            // cmbFilterMode
            // 
            cmbFilterMode.DropDownStyle = ComboBoxStyle.DropDownList;
            cmbFilterMode.FormattingEnabled = true;
            cmbFilterMode.Location = new Point(15, 175);
            cmbFilterMode.Name = "cmbFilterMode";
            cmbFilterMode.Size = new Size(150, 23);
            cmbFilterMode.TabIndex = 5;
            // 
            // lblFilterMode
            // 
            lblFilterMode.AutoSize = true;
            lblFilterMode.Location = new Point(15, 157);
            lblFilterMode.Name = "lblFilterMode";
            lblFilterMode.Size = new Size(67, 15);
            lblFilterMode.TabIndex = 4;
            lblFilterMode.Text = "Filter Mode";
            // 
            // btnReadStockData
            // 
            btnReadStockData.Location = new Point(717, 173);
            btnReadStockData.Name = "btnReadStockData";
            btnReadStockData.Size = new Size(100, 25);
            btnReadStockData.TabIndex = 3;
            btnReadStockData.Text = "Read StockData";
            btnReadStockData.UseVisualStyleBackColor = true;
            btnReadStockData.Click += BtnReadStockData_Click;
            // 
            // btnReadMetaData
            // 
            btnReadMetaData.Location = new Point(717, 142);
            btnReadMetaData.Name = "btnReadMetaData";
            btnReadMetaData.Size = new Size(100, 25);
            btnReadMetaData.TabIndex = 2;
            btnReadMetaData.Text = "Read MetaData";
            btnReadMetaData.UseVisualStyleBackColor = true;
            btnReadMetaData.Click += BtnReadMetaData_Click;
            // 
            // btnSaveFile1
            // 
            btnSaveFile1.Location = new Point(671, 114);
            btnSaveFile1.Name = "btnSaveFile1";
            btnSaveFile1.Size = new Size(30, 25);
            btnSaveFile1.TabIndex = 13;
            btnSaveFile1.Text = "💾";
            btnSaveFile1.UseVisualStyleBackColor = true;
            btnSaveFile1.Click += BtnSaveFile1_Click;
            // 
            // btnBrowseFile1
            // 
            btnBrowseFile1.Location = new Point(635, 114);
            btnBrowseFile1.Name = "btnBrowseFile1";
            btnBrowseFile1.Size = new Size(30, 25);
            btnBrowseFile1.TabIndex = 12;
            btnBrowseFile1.Text = "📁";
            btnBrowseFile1.UseVisualStyleBackColor = true;
            btnBrowseFile1.Click += BtnBrowseFile1_Click;
            // 
            // txtDataFileName
            // 
            txtDataFileName.Location = new Point(15, 114);
            txtDataFileName.Name = "txtDataFileName";
            txtDataFileName.Size = new Size(614, 23);
            txtDataFileName.TabIndex = 1;
            // 
            // lblDataFileName
            // 
            lblDataFileName.AutoSize = true;
            lblDataFileName.Location = new Point(15, 96);
            lblDataFileName.Name = "lblDataFileName";
            lblDataFileName.Size = new Size(84, 15);
            lblDataFileName.TabIndex = 0;
            lblDataFileName.Text = "DataFile Name";
            // 
            // tabPageSingleTrader
            // 
            tabPageSingleTrader.BackColor = Color.White;
            tabPageSingleTrader.Controls.Add(panel4);
            tabPageSingleTrader.Location = new Point(4, 24);
            tabPageSingleTrader.Name = "tabPageSingleTrader";
            tabPageSingleTrader.Padding = new Padding(3);
            tabPageSingleTrader.Size = new Size(1415, 793);
            tabPageSingleTrader.TabIndex = 1;
            tabPageSingleTrader.Text = "SingleTrader";
            // 
            // panel4
            // 
            panel4.Controls.Add(btnStopMultipleTrader);
            panel4.Controls.Add(btnStopSingleTrader);
            panel4.Controls.Add(btnStartMultipleTrader);
            panel4.Controls.Add(btnStartSingleTrader);
            panel4.Controls.Add(progressBarSingleTrader);
            panel4.Controls.Add(lblSingleTraderProgress);
            panel4.Controls.Add(button1);
            panel4.Controls.Add(richTextBoxSingleTrader);
            panel4.Controls.Add(btnTestAlgoTrader);
            panel4.Location = new Point(32, 26);
            panel4.Name = "panel4";
            panel4.Size = new Size(1222, 717);
            panel4.TabIndex = 0;
            // 
            // btnStopSingleTrader
            // 
            btnStopSingleTrader.Location = new Point(917, 103);
            btnStopSingleTrader.Name = "btnStopSingleTrader";
            btnStopSingleTrader.Size = new Size(131, 23);
            btnStopSingleTrader.TabIndex = 7;
            btnStopSingleTrader.Text = "Stop Single Trader";
            btnStopSingleTrader.UseVisualStyleBackColor = true;
            btnStopSingleTrader.Click += btnStopSingleTrader_Click;
            // 
            // btnStartMultipleTrader
            // 
            btnStartMultipleTrader.Location = new Point(780, 221);
            btnStartMultipleTrader.Name = "btnStartMultipleTrader";
            btnStartMultipleTrader.Size = new Size(131, 23);
            btnStartMultipleTrader.TabIndex = 6;
            btnStartMultipleTrader.Text = "Start Multiple Trader";
            btnStartMultipleTrader.UseVisualStyleBackColor = true;
            btnStartMultipleTrader.Click += btnStartMultipleTrader_Click;
            // 
            // btnStartSingleTrader
            // 
            btnStartSingleTrader.Location = new Point(780, 103);
            btnStartSingleTrader.Name = "btnStartSingleTrader";
            btnStartSingleTrader.Size = new Size(131, 23);
            btnStartSingleTrader.TabIndex = 5;
            btnStartSingleTrader.Text = "Start Single Trader";
            btnStartSingleTrader.UseVisualStyleBackColor = true;
            btnStartSingleTrader.Click += btnStartSingleTrader_Click;
            // 
            // progressBarSingleTrader
            // 
            progressBarSingleTrader.Location = new Point(780, 74);
            progressBarSingleTrader.Name = "progressBarSingleTrader";
            progressBarSingleTrader.Size = new Size(268, 23);
            progressBarSingleTrader.Style = ProgressBarStyle.Continuous;
            progressBarSingleTrader.TabIndex = 4;
            // 
            // lblSingleTraderProgress
            // 
            lblSingleTraderProgress.AutoSize = true;
            lblSingleTraderProgress.Location = new Point(1047, 82);
            lblSingleTraderProgress.Name = "lblSingleTraderProgress";
            lblSingleTraderProgress.Size = new Size(48, 15);
            lblSingleTraderProgress.TabIndex = 3;
            lblSingleTraderProgress.Text = "Ready...";
            // 
            // button1
            // 
            button1.Location = new Point(780, 16);
            button1.Name = "button1";
            button1.Size = new Size(131, 23);
            button1.TabIndex = 2;
            button1.Text = "Read StockData";
            button1.UseVisualStyleBackColor = true;
            button1.Click += BtnReadStockData_Click;
            // 
            // richTextBoxSingleTrader
            // 
            richTextBoxSingleTrader.Location = new Point(24, 16);
            richTextBoxSingleTrader.Name = "richTextBoxSingleTrader";
            richTextBoxSingleTrader.Size = new Size(750, 668);
            richTextBoxSingleTrader.TabIndex = 1;
            richTextBoxSingleTrader.Text = "";
            // 
            // btnTestAlgoTrader
            // 
            btnTestAlgoTrader.Location = new Point(780, 45);
            btnTestAlgoTrader.Name = "btnTestAlgoTrader";
            btnTestAlgoTrader.Size = new Size(131, 23);
            btnTestAlgoTrader.TabIndex = 0;
            btnTestAlgoTrader.Text = "Test AlgoTrader";
            btnTestAlgoTrader.UseVisualStyleBackColor = true;
            btnTestAlgoTrader.Click += btnTestAlgoTrader_Click;
            // 
            // tabPageMultipleTraders
            // 
            tabPageMultipleTraders.BackColor = Color.White;
            tabPageMultipleTraders.Location = new Point(4, 24);
            tabPageMultipleTraders.Name = "tabPageMultipleTraders";
            tabPageMultipleTraders.Padding = new Padding(3);
            tabPageMultipleTraders.Size = new Size(1415, 793);
            tabPageMultipleTraders.TabIndex = 2;
            tabPageMultipleTraders.Text = "MultipleTraders";
            // 
            // tabPageSingleTraderOptimization
            // 
            tabPageSingleTraderOptimization.BackColor = Color.White;
            tabPageSingleTraderOptimization.Controls.Add(panel5);
            tabPageSingleTraderOptimization.Location = new Point(4, 24);
            tabPageSingleTraderOptimization.Name = "tabPageSingleTraderOptimization";
            tabPageSingleTraderOptimization.Padding = new Padding(3);
            tabPageSingleTraderOptimization.Size = new Size(1415, 793);
            tabPageSingleTraderOptimization.TabIndex = 3;
            tabPageSingleTraderOptimization.Text = "SingleTraderOptimization";
            // 
            // panel5
            // 
            panel5.Controls.Add(groupBox2);
            panel5.Controls.Add(btnStopSingleTraderOpt);
            panel5.Controls.Add(btnStartSingleTraderOpt);
            panel5.Controls.Add(label2);
            panel5.Controls.Add(label1);
            panel5.Controls.Add(lblSingleTraderProgress2);
            panel5.Controls.Add(lblOptimizationProgress);
            panel5.Controls.Add(lblSkipIteration);
            panel5.Controls.Add(txtSkipIteration);
            panel5.Controls.Add(lblMaxIteration);
            panel5.Controls.Add(txtMaxIteration);
            panel5.Controls.Add(progressBarSingleTraderProgress);
            panel5.Controls.Add(progressBarOptimizationProgress);
            panel5.Controls.Add(richTextBoxSingleTraderOptimization);
            panel5.Controls.Add(lblOptimizationResult);
            panel5.Controls.Add(label3);
            panel5.Controls.Add(lblOptimizationResults);
            panel5.Controls.Add(dataGridViewOptimizationResults);
            panel5.Location = new Point(16, 16);
            panel5.Name = "panel5";
            panel5.Size = new Size(1330, 841);
            panel5.TabIndex = 0;
            // 
            // groupBox2
            // 
            groupBox2.Controls.Add(label4);
            groupBox2.Location = new Point(991, 18);
            groupBox2.Name = "groupBox2";
            groupBox2.Size = new Size(313, 91);
            groupBox2.TabIndex = 20;
            groupBox2.TabStop = false;
            // 
            // label4
            // 
            label4.Location = new Point(19, 29);
            label4.Name = "label4";
            label4.Size = new Size(275, 40);
            label4.TabIndex = 0;
            label4.Text = "Opt sonucları log sonuc dosyasının sonuna eklenir! Gerekiyorsa dosyayı sil!";
            // 
            // btnStopSingleTraderOpt
            // 
            btnStopSingleTraderOpt.Location = new Point(208, 86);
            btnStopSingleTraderOpt.Name = "btnStopSingleTraderOpt";
            btnStopSingleTraderOpt.Size = new Size(147, 23);
            btnStopSingleTraderOpt.TabIndex = 14;
            btnStopSingleTraderOpt.Text = "Stop Single Trader Opt";
            btnStopSingleTraderOpt.UseVisualStyleBackColor = true;
            btnStopSingleTraderOpt.Click += btnStopSingleTraderOpt_Click;
            // 
            // btnStartSingleTraderOpt
            // 
            btnStartSingleTraderOpt.Location = new Point(40, 86);
            btnStartSingleTraderOpt.Name = "btnStartSingleTraderOpt";
            btnStartSingleTraderOpt.Size = new Size(162, 23);
            btnStartSingleTraderOpt.TabIndex = 9;
            btnStartSingleTraderOpt.Text = "Start Single Trader Opt";
            btnStartSingleTraderOpt.UseVisualStyleBackColor = true;
            btnStartSingleTraderOpt.Click += btnStartSingleTraderOpt_Click;
            // 
            // label2
            // 
            label2.AutoSize = true;
            label2.Location = new Point(65, 55);
            label2.Name = "label2";
            label2.Size = new Size(40, 15);
            label2.TabIndex = 19;
            label2.Text = "Trader";
            // 
            // label1
            // 
            label1.AutoSize = true;
            label1.Location = new Point(28, 26);
            label1.Name = "label1";
            label1.Size = new Size(77, 15);
            label1.TabIndex = 18;
            label1.Text = "Combination";
            // 
            // lblSingleTraderProgress2
            // 
            lblSingleTraderProgress2.AutoSize = true;
            lblSingleTraderProgress2.Location = new Point(302, 55);
            lblSingleTraderProgress2.Name = "lblSingleTraderProgress2";
            lblSingleTraderProgress2.Size = new Size(48, 15);
            lblSingleTraderProgress2.TabIndex = 17;
            lblSingleTraderProgress2.Text = "Ready...";
            // 
            // lblOptimizationProgress
            // 
            lblOptimizationProgress.AutoSize = true;
            lblOptimizationProgress.Location = new Point(302, 26);
            lblOptimizationProgress.Name = "lblOptimizationProgress";
            lblOptimizationProgress.Size = new Size(48, 15);
            lblOptimizationProgress.TabIndex = 16;
            lblOptimizationProgress.Text = "Ready...";
            // 
            // lblSkipIteration
            // 
            lblSkipIteration.AutoSize = true;
            lblSkipIteration.Location = new Point(485, 29);
            lblSkipIteration.Name = "lblSkipIteration";
            lblSkipIteration.Size = new Size(79, 15);
            lblSkipIteration.TabIndex = 17;
            lblSkipIteration.Text = "Skip Iteration:";
            // 
            // txtSkipIteration
            // 
            txtSkipIteration.Location = new Point(570, 26);
            txtSkipIteration.Name = "txtSkipIteration";
            txtSkipIteration.Size = new Size(60, 23);
            txtSkipIteration.TabIndex = 18;
            txtSkipIteration.Text = "-1";
            txtSkipIteration.TextAlign = HorizontalAlignment.Center;
            // 
            // lblMaxIteration
            // 
            lblMaxIteration.AutoSize = true;
            lblMaxIteration.Location = new Point(485, 58);
            lblMaxIteration.Name = "lblMaxIteration";
            lblMaxIteration.Size = new Size(79, 15);
            lblMaxIteration.TabIndex = 19;
            lblMaxIteration.Text = "Max Iteration:";
            // 
            // txtMaxIteration
            // 
            txtMaxIteration.Location = new Point(572, 55);
            txtMaxIteration.Name = "txtMaxIteration";
            txtMaxIteration.Size = new Size(60, 23);
            txtMaxIteration.TabIndex = 20;
            txtMaxIteration.Text = "-1";
            txtMaxIteration.TextAlign = HorizontalAlignment.Center;
            // 
            // progressBarSingleTraderProgress
            // 
            progressBarSingleTraderProgress.Location = new Point(111, 47);
            progressBarSingleTraderProgress.Name = "progressBarSingleTraderProgress";
            progressBarSingleTraderProgress.Size = new Size(185, 23);
            progressBarSingleTraderProgress.TabIndex = 15;
            // 
            // progressBarOptimizationProgress
            // 
            progressBarOptimizationProgress.Location = new Point(111, 18);
            progressBarOptimizationProgress.Name = "progressBarOptimizationProgress";
            progressBarOptimizationProgress.Size = new Size(185, 23);
            progressBarOptimizationProgress.TabIndex = 14;
            // 
            // richTextBoxSingleTraderOptimization
            // 
            richTextBoxSingleTraderOptimization.Location = new Point(40, 637);
            richTextBoxSingleTraderOptimization.Name = "richTextBoxSingleTraderOptimization";
            richTextBoxSingleTraderOptimization.Size = new Size(1264, 185);
            richTextBoxSingleTraderOptimization.TabIndex = 4;
            richTextBoxSingleTraderOptimization.Text = "";
            richTextBoxSingleTraderOptimization.DoubleClick += richTextBoxSingleTraderOptimization_DoubleClick;
            // 
            // lblOptimizationResult
            // 
            lblOptimizationResult.Location = new Point(155, 162);
            lblOptimizationResult.Name = "lblOptimizationResult";
            lblOptimizationResult.Size = new Size(533, 15);
            lblOptimizationResult.TabIndex = 3;
            lblOptimizationResult.Text = "..........";
            // 
            // label3
            // 
            label3.AutoSize = true;
            label3.Location = new Point(40, 162);
            label3.Name = "label3";
            label3.Size = new Size(109, 15);
            label3.TabIndex = 2;
            label3.Text = "Last Optimization : ";
            // 
            // lblOptimizationResults
            // 
            lblOptimizationResults.AutoSize = true;
            lblOptimizationResults.Location = new Point(1185, 162);
            lblOptimizationResults.Name = "lblOptimizationResults";
            lblOptimizationResults.Size = new Size(119, 15);
            lblOptimizationResults.TabIndex = 1;
            lblOptimizationResults.Text = "Optimization Results ";
            // 
            // dataGridViewOptimizationResults
            // 
            dataGridViewOptimizationResults.ColumnHeadersHeightSizeMode = DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            dataGridViewOptimizationResults.Location = new Point(40, 180);
            dataGridViewOptimizationResults.Name = "dataGridViewOptimizationResults";
            dataGridViewOptimizationResults.Size = new Size(1264, 451);
            dataGridViewOptimizationResults.TabIndex = 0;
            // 
            // openFileDialog1
            // 
            openFileDialog1.FileName = "openFileDialog1";
            // 
            // openFileDialog2
            // 
            openFileDialog2.FileName = "openFileDialog1";
            // 
            // btnStopMultipleTrader
            // 
            btnStopMultipleTrader.Location = new Point(917, 221);
            btnStopMultipleTrader.Name = "btnStopMultipleTrader";
            btnStopMultipleTrader.Size = new Size(131, 23);
            btnStopMultipleTrader.TabIndex = 8;
            btnStopMultipleTrader.Text = "Stop Multiple Trader";
            btnStopMultipleTrader.UseVisualStyleBackColor = true;
            btnStopMultipleTrader.Click += btnStopMultipleTrader_Click;
            // 
            // Form1
            // 
            AutoScaleDimensions = new SizeF(7F, 15F);
            AutoScaleMode = AutoScaleMode.Font;
            ClientSize = new Size(1599, 1061);
            Controls.Add(centerPanel);
            Controls.Add(rightPanel);
            Controls.Add(leftPanel);
            Controls.Add(bottomPanel);
            Controls.Add(topPanel);
            Controls.Add(mainToolStrip2);
            Controls.Add(mainToolStrip1);
            Controls.Add(mainMenuStrip);
            Controls.Add(statusStrip);
            MainMenuStrip = mainMenuStrip;
            Name = "Form1";
            StartPosition = FormStartPosition.CenterScreen;
            Text = "Algo Trade - Optimization Support";
            mainMenuStrip.ResumeLayout(false);
            mainMenuStrip.PerformLayout();
            mainToolStrip1.ResumeLayout(false);
            mainToolStrip1.PerformLayout();
            mainToolStrip2.ResumeLayout(false);
            mainToolStrip2.PerformLayout();
            statusStrip.ResumeLayout(false);
            statusStrip.PerformLayout();
            bottomPanel.ResumeLayout(false);
            centerPanel.ResumeLayout(false);
            mainTabControl.ResumeLayout(false);
            tabPageStockDataReader.ResumeLayout(false);
            panel2.ResumeLayout(false);
            panel1.ResumeLayout(false);
            panel1.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)stockDataGridView).EndInit();
            groupBox1.ResumeLayout(false);
            groupBox1.PerformLayout();
            tabPageSingleTrader.ResumeLayout(false);
            panel4.ResumeLayout(false);
            panel4.PerformLayout();
            tabPageSingleTraderOptimization.ResumeLayout(false);
            panel5.ResumeLayout(false);
            panel5.PerformLayout();
            groupBox2.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)dataGridViewOptimizationResults).EndInit();
            ResumeLayout(false);
            PerformLayout();
        }

        #endregion

        private MenuStrip mainMenuStrip;
        private ToolStrip mainToolStrip1;
        private ToolStrip mainToolStrip2;
        private StatusStrip statusStrip;
        private Panel topPanel;
        private Panel leftPanel;
        private Panel rightPanel;
        private Panel bottomPanel;
        private Panel centerPanel;
        private TabControl mainTabControl;
        private TabPage tabPageStockDataReader;
        private TabPage tabPageSingleTrader;
        private TabPage tabPageMultipleTraders;
        private TabPage tabPageSingleTraderOptimization;

        // Menu Items
        private ToolStripMenuItem fileToolStripMenuItem;
        private ToolStripMenuItem newToolStripMenuItem;
        private ToolStripMenuItem openToolStripMenuItem;
        private ToolStripMenuItem saveToolStripMenuItem;
        private ToolStripMenuItem saveAsToolStripMenuItem;
        private ToolStripMenuItem exitToolStripMenuItem;
        private ToolStripMenuItem editToolStripMenuItem;
        private ToolStripMenuItem undoToolStripMenuItem;
        private ToolStripMenuItem redoToolStripMenuItem;
        private ToolStripMenuItem cutToolStripMenuItem;
        private ToolStripMenuItem copyToolStripMenuItem;
        private ToolStripMenuItem pasteToolStripMenuItem;
        private ToolStripMenuItem selectAllToolStripMenuItem;
        private ToolStripMenuItem viewToolStripMenuItem;
        private ToolStripMenuItem toolbarsToolStripMenuItem;
        private ToolStripMenuItem defaultToolbarsToolStripMenuItem;
        private ToolStripMenuItem hideAllToolbarsToolStripMenuItem;
        private ToolStripMenuItem showAllToolbarsToolStripMenuItem;
        private ToolStripSeparator toolStripSeparator4;
        private ToolStripMenuItem mainToolStrip1ToolStripMenuItem;
        private ToolStripMenuItem mainToolStrip2ToolStripMenuItem;
        private ToolStripMenuItem statusBarToolStripMenuItem;
        private ToolStripMenuItem panelsToolStripMenuItem;
        private ToolStripMenuItem defaultPanelsToolStripMenuItem;
        private ToolStripMenuItem hideAllPanelsToolStripMenuItem;
        private ToolStripMenuItem showAllPanelsToolStripMenuItem;
        private ToolStripSeparator toolStripSeparator3;
        private ToolStripMenuItem topPanelToolStripMenuItem;
        private ToolStripMenuItem leftPanelToolStripMenuItem;
        private ToolStripMenuItem rightPanelToolStripMenuItem;
        private ToolStripMenuItem bottomPanelToolStripMenuItem;
        private ToolStripMenuItem fullScreenToolStripMenuItem;
        private ToolStripMenuItem toolsToolStripMenuItem;
        private ToolStripMenuItem optionsToolStripMenuItem;
        private ToolStripMenuItem preferencesToolStripMenuItem;
        private ToolStripMenuItem helpToolStripMenuItem;
        private ToolStripMenuItem aboutToolStripMenuItem;
        private ToolStripMenuItem documentationToolStripMenuItem;

        // ToolStrip1 Items
        private ToolStripButton newToolStripButton;
        private ToolStripButton openToolStripButton;
        private ToolStripButton saveToolStripButton;
        private ToolStripSeparator toolStripSeparator1;
        private ToolStripButton cutToolStripButton;
        private ToolStripButton copyToolStripButton;
        private ToolStripButton pasteToolStripButton;
        private ToolStripSeparator toolStripSeparator2;
        private ToolStripButton helpToolStripButton;

        // ToolStrip2 Items
        private ToolStripButton runStrategyButton;
        private ToolStripButton stopStrategyButton;
        private ToolStripSeparator toolStripSeparator5;
        private ToolStripButton optimizeButton;
        private ToolStripButton exportResultsButton;
        private ToolStripSeparator toolStripSeparator6;
        private ToolStripButton settingsButton;

        // StatusStrip Items
        private ToolStripStatusLabel statusLabel;
        private ToolStripProgressBar progressBar;
        private ToolStripStatusLabel spacerLabel;
        private ToolStripStatusLabel timeLabel;

        // Timer
        private System.Windows.Forms.Timer? statusTimer;
        private RichTextBox richTextBox1;
        private OpenFileDialog openFileDialog1;
        private SaveFileDialog saveFileDialog1;
        private Panel panel3;
        private Panel panel2;
        private Panel panel1;
        private DataGridView stockDataGridView;
        private TextBox textBoxMetaData;
        private GroupBox groupBox1;
        private Button btnClear;
        private DateTimePicker dtpFilterDateTime2;
        private DateTimePicker dtpFilterDateTime1;
        private TextBox txtFilterValue2;
        private Label lblFilterValue2;
        private TextBox txtFilterValue1;
        private Label lblFilterValue1;
        private ComboBox cmbFilterMode;
        private Label lblFilterMode;
        private Button btnReadStockData;
        private Button btnReadMetaData;
        private Button btnSaveFile1;
        private Button btnBrowseFile1;
        private TextBox txtDataFileName;
        private Label lblDataFileName;
        private Label stockDataGridViewLabel;
        private Button btnSaveConfigFile;
        private Button btnReadConfigFile;
        private Button btnUpdateFilters;
        private Button btnFirstRow;
        private Button btnPrevRow;
        private Button btnNextRow;
        private Button btnLastRow;
        private Button btnSaveFile2;
        private Button btnBrowseFile2;
        private TextBox txtConfigFileName;
        private Label lblConfigFileName;
        private OpenFileDialog openFileDialog2;
        private SaveFileDialog saveFileDialog2;
        private Button btnFirstPage;
        private Button btnPreviousPage;
        private Button btnNextPage;
        private Button btnLastPage;
        private ComboBox cmbPageSize;
        private Label lblPageSize;
        private Button btnClearStockData;
        private Button btnClearLogFiles;
        private Label lblReadStockDataTime;
        private Panel panel4;
        private Button btnTestAlgoTrader;
        private RichTextBox richTextBoxSingleTrader;
        private Button button1;
        private Label lblSingleTraderProgress;
        private ProgressBar progressBarSingleTrader;
        private Button btnStartMultipleTrader;
        private Button btnStartSingleTrader;
        private Button btnStartSingleTraderOpt;
        private Button btnStopSingleTraderOpt;
        private Panel panel5;
        private Label lblOptimizationResults;
        private DataGridView dataGridViewOptimizationResults;
        private Label lblOptimizationResult;
        private Label label3;
        private Label label2;
        private Label label1;
        private Label lblSingleTraderProgress2;
        private Label lblOptimizationProgress;
        private ProgressBar progressBarSingleTraderProgress;
        private ProgressBar progressBarOptimizationProgress;
        private RichTextBox richTextBoxSingleTraderOptimization;
        private GroupBox groupBox2;
        private Label label4;
        private Label lblSkipIteration;
        private TextBox txtSkipIteration;
        private Label lblMaxIteration;
        private TextBox txtMaxIteration;
        private Button btnStopSingleTrader;
        private Button btnStopMultipleTrader;
    }
}
