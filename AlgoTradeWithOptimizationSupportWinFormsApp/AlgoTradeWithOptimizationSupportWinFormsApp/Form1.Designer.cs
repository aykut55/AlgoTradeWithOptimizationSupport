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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(Form1));
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
            btnLastRow = new Button();
            btnNextRow = new Button();
            btnPrevRow = new Button();
            btnFirstRow = new Button();
            stockDataGridViewLabel = new Label();
            stockDataGridView = new DataGridView();
            textBoxMetaData = new TextBox();
            groupBox1 = new GroupBox();
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
            btnSaveFile = new Button();
            btnBrowseFile = new Button();
            txtFileName = new TextBox();
            lblFileName = new Label();
            tabPageSingleTrader = new TabPage();
            tabPageMultipleTraders = new TabPage();
            tabPageSingleTraderOptimization = new TabPage();
            openFileDialog1 = new OpenFileDialog();
            saveFileDialog1 = new SaveFileDialog();
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
            SuspendLayout();
            // 
            // mainMenuStrip
            // 
            mainMenuStrip.Items.AddRange(new ToolStripItem[] { fileToolStripMenuItem, editToolStripMenuItem, viewToolStripMenuItem, toolsToolStripMenuItem, helpToolStripMenuItem });
            mainMenuStrip.Location = new Point(0, 0);
            mainMenuStrip.Name = "mainMenuStrip";
            mainMenuStrip.Size = new Size(1484, 24);
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
            mainToolStrip1.Size = new Size(1484, 25);
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
            statusStrip.Size = new Size(1484, 25);
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
            spacerLabel.Size = new Size(1279, 20);
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
            topPanel.Size = new Size(1484, 27);
            topPanel.TabIndex = 3;
            topPanel.Visible = false;
            // 
            // leftPanel
            // 
            leftPanel.BorderStyle = BorderStyle.FixedSingle;
            leftPanel.Dock = DockStyle.Left;
            leftPanel.Location = new Point(0, 76);
            leftPanel.Name = "leftPanel";
            leftPanel.Size = new Size(103, 803);
            leftPanel.TabIndex = 5;
            // 
            // rightPanel
            // 
            rightPanel.BorderStyle = BorderStyle.FixedSingle;
            rightPanel.Dock = DockStyle.Right;
            rightPanel.Location = new Point(1413, 76);
            rightPanel.Name = "rightPanel";
            rightPanel.Size = new Size(71, 803);
            rightPanel.TabIndex = 6;
            rightPanel.Visible = false;
            // 
            // bottomPanel
            // 
            bottomPanel.BorderStyle = BorderStyle.FixedSingle;
            bottomPanel.Controls.Add(richTextBox1);
            bottomPanel.Dock = DockStyle.Bottom;
            bottomPanel.Location = new Point(0, 879);
            bottomPanel.Name = "bottomPanel";
            bottomPanel.Size = new Size(1484, 157);
            bottomPanel.TabIndex = 4;
            // 
            // richTextBox1
            // 
            richTextBox1.Dock = DockStyle.Fill;
            richTextBox1.Location = new Point(0, 0);
            richTextBox1.Name = "richTextBox1";
            richTextBox1.Size = new Size(1482, 155);
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
            centerPanel.Size = new Size(1310, 803);
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
            mainTabControl.Size = new Size(1308, 801);
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
            tabPageStockDataReader.Size = new Size(1300, 773);
            tabPageStockDataReader.TabIndex = 0;
            tabPageStockDataReader.Text = "StockDataReader";
            // 
            // panel3
            // 
            panel3.Dock = DockStyle.Fill;
            panel3.Location = new Point(1042, 3);
            panel3.Name = "panel3";
            panel3.Size = new Size(255, 767);
            panel3.TabIndex = 3;
            // 
            // panel2
            // 
            panel2.Controls.Add(panel1);
            panel2.Controls.Add(groupBox1);
            panel2.Dock = DockStyle.Left;
            panel2.Location = new Point(3, 3);
            panel2.Name = "panel2";
            panel2.Size = new Size(1039, 767);
            panel2.TabIndex = 2;
            // 
            // panel1
            // 
            panel1.Controls.Add(btnLastRow);
            panel1.Controls.Add(btnNextRow);
            panel1.Controls.Add(btnPrevRow);
            panel1.Controls.Add(btnFirstRow);
            panel1.Controls.Add(stockDataGridViewLabel);
            panel1.Controls.Add(stockDataGridView);
            panel1.Controls.Add(textBoxMetaData);
            panel1.Dock = DockStyle.Fill;
            panel1.Location = new Point(0, 148);
            panel1.Name = "panel1";
            panel1.Size = new Size(1039, 619);
            panel1.TabIndex = 2;
            // 
            // btnLastRow
            // 
            btnLastRow.Location = new Point(833, 95);
            btnLastRow.Name = "btnLastRow";
            btnLastRow.Size = new Size(75, 23);
            btnLastRow.TabIndex = 22;
            btnLastRow.Text = "Last >|";
            btnLastRow.UseVisualStyleBackColor = true;
            btnLastRow.Click += BtnLastRow_Click;
            // 
            // btnNextRow
            // 
            btnNextRow.Location = new Point(752, 95);
            btnNextRow.Name = "btnNextRow";
            btnNextRow.Size = new Size(75, 23);
            btnNextRow.TabIndex = 21;
            btnNextRow.Text = "Next >";
            btnNextRow.UseVisualStyleBackColor = true;
            btnNextRow.Click += BtnNextRow_Click;
            // 
            // btnPrevRow
            // 
            btnPrevRow.Location = new Point(671, 95);
            btnPrevRow.Name = "btnPrevRow";
            btnPrevRow.Size = new Size(75, 23);
            btnPrevRow.TabIndex = 20;
            btnPrevRow.Text = "< Prev";
            btnPrevRow.UseVisualStyleBackColor = true;
            btnPrevRow.Click += BtnPrevRow_Click;
            // 
            // btnFirstRow
            // 
            btnFirstRow.Location = new Point(590, 95);
            btnFirstRow.Name = "btnFirstRow";
            btnFirstRow.Size = new Size(75, 23);
            btnFirstRow.TabIndex = 19;
            btnFirstRow.Text = "|< First";
            btnFirstRow.UseVisualStyleBackColor = true;
            btnFirstRow.Click += BtnFirstRow_Click;
            // 
            // stockDataGridViewLabel
            // 
            stockDataGridViewLabel.AutoSize = true;
            stockDataGridViewLabel.Location = new Point(993, 103);
            stockDataGridViewLabel.Name = "stockDataGridViewLabel";
            stockDataGridViewLabel.Size = new Size(24, 15);
            stockDataGridViewLabel.TabIndex = 18;
            stockDataGridViewLabel.Text = "0/0";
            // 
            // stockDataGridView
            // 
            stockDataGridView.ColumnHeadersHeightSizeMode = DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            stockDataGridView.Location = new Point(15, 124);
            stockDataGridView.Name = "stockDataGridView";
            stockDataGridView.Size = new Size(1016, 475);
            stockDataGridView.TabIndex = 17;
            // 
            // textBoxMetaData
            // 
            textBoxMetaData.Font = new Font("Consolas", 9F);
            textBoxMetaData.Location = new Point(15, 6);
            textBoxMetaData.Multiline = true;
            textBoxMetaData.Name = "textBoxMetaData";
            textBoxMetaData.ReadOnly = true;
            textBoxMetaData.ScrollBars = ScrollBars.Vertical;
            textBoxMetaData.Size = new Size(533, 112);
            textBoxMetaData.TabIndex = 16;
            textBoxMetaData.Text = resources.GetString("textBoxMetaData.Text");
            // 
            // groupBox1
            // 
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
            groupBox1.Controls.Add(btnSaveFile);
            groupBox1.Controls.Add(btnBrowseFile);
            groupBox1.Controls.Add(txtFileName);
            groupBox1.Controls.Add(lblFileName);
            groupBox1.Dock = DockStyle.Top;
            groupBox1.Location = new Point(0, 0);
            groupBox1.Name = "groupBox1";
            groupBox1.Size = new Size(1039, 148);
            groupBox1.TabIndex = 1;
            groupBox1.TabStop = false;
            groupBox1.Text = "Stock Data Reader";
            // 
            // btnUpdateFilters
            // 
            btnUpdateFilters.Location = new Point(843, 72);
            btnUpdateFilters.Name = "btnUpdateFilters";
            btnUpdateFilters.Size = new Size(120, 25);
            btnUpdateFilters.TabIndex = 17;
            btnUpdateFilters.Text = "Update Filters";
            btnUpdateFilters.UseVisualStyleBackColor = true;
            // 
            // btnSaveConfigFile
            // 
            btnSaveConfigFile.Location = new Point(843, 19);
            btnSaveConfigFile.Name = "btnSaveConfigFile";
            btnSaveConfigFile.Size = new Size(120, 25);
            btnSaveConfigFile.TabIndex = 16;
            btnSaveConfigFile.Text = "Save Config File";
            btnSaveConfigFile.UseVisualStyleBackColor = true;
            // 
            // btnReadConfigFile
            // 
            btnReadConfigFile.Location = new Point(717, 19);
            btnReadConfigFile.Name = "btnReadConfigFile";
            btnReadConfigFile.Size = new Size(120, 25);
            btnReadConfigFile.TabIndex = 15;
            btnReadConfigFile.Text = "Read Config File";
            btnReadConfigFile.UseVisualStyleBackColor = true;
            // 
            // btnClear
            // 
            btnClear.Location = new Point(717, 72);
            btnClear.Name = "btnClear";
            btnClear.Size = new Size(120, 25);
            btnClear.TabIndex = 14;
            btnClear.Text = "Clear";
            btnClear.UseVisualStyleBackColor = true;
            btnClear.Click += BtnClear_Click;
            // 
            // dtpFilterDateTime2
            // 
            dtpFilterDateTime2.Format = DateTimePickerFormat.Short;
            dtpFilterDateTime2.Location = new Point(384, 50);
            dtpFilterDateTime2.Name = "dtpFilterDateTime2";
            dtpFilterDateTime2.Size = new Size(100, 23);
            dtpFilterDateTime2.TabIndex = 11;
            // 
            // dtpFilterDateTime1
            // 
            dtpFilterDateTime1.Format = DateTimePickerFormat.Short;
            dtpFilterDateTime1.Location = new Point(384, 21);
            dtpFilterDateTime1.Name = "dtpFilterDateTime1";
            dtpFilterDateTime1.Size = new Size(100, 23);
            dtpFilterDateTime1.TabIndex = 10;
            // 
            // txtFilterValue2
            // 
            txtFilterValue2.Location = new Point(277, 47);
            txtFilterValue2.Name = "txtFilterValue2";
            txtFilterValue2.Size = new Size(100, 23);
            txtFilterValue2.TabIndex = 9;
            // 
            // lblFilterValue2
            // 
            lblFilterValue2.AutoSize = true;
            lblFilterValue2.Location = new Point(277, 29);
            lblFilterValue2.Name = "lblFilterValue2";
            lblFilterValue2.Size = new Size(47, 15);
            lblFilterValue2.TabIndex = 8;
            lblFilterValue2.Text = "Value 2:";
            // 
            // txtFilterValue1
            // 
            txtFilterValue1.Location = new Point(171, 47);
            txtFilterValue1.Name = "txtFilterValue1";
            txtFilterValue1.Size = new Size(100, 23);
            txtFilterValue1.TabIndex = 7;
            // 
            // lblFilterValue1
            // 
            lblFilterValue1.AutoSize = true;
            lblFilterValue1.Location = new Point(171, 29);
            lblFilterValue1.Name = "lblFilterValue1";
            lblFilterValue1.Size = new Size(47, 15);
            lblFilterValue1.TabIndex = 6;
            lblFilterValue1.Text = "Value 1:";
            // 
            // cmbFilterMode
            // 
            cmbFilterMode.DropDownStyle = ComboBoxStyle.DropDownList;
            cmbFilterMode.FormattingEnabled = true;
            cmbFilterMode.Location = new Point(15, 47);
            cmbFilterMode.Name = "cmbFilterMode";
            cmbFilterMode.Size = new Size(150, 23);
            cmbFilterMode.TabIndex = 5;
            // 
            // lblFilterMode
            // 
            lblFilterMode.AutoSize = true;
            lblFilterMode.Location = new Point(15, 29);
            lblFilterMode.Name = "lblFilterMode";
            lblFilterMode.Size = new Size(70, 15);
            lblFilterMode.TabIndex = 4;
            lblFilterMode.Text = "Filter Mode:";
            // 
            // btnReadStockData
            // 
            btnReadStockData.Location = new Point(843, 103);
            btnReadStockData.Name = "btnReadStockData";
            btnReadStockData.Size = new Size(120, 25);
            btnReadStockData.TabIndex = 3;
            btnReadStockData.Text = "Read StockData";
            btnReadStockData.UseVisualStyleBackColor = true;
            btnReadStockData.Click += BtnReadStockData_Click;
            // 
            // btnReadMetaData
            // 
            btnReadMetaData.Location = new Point(717, 103);
            btnReadMetaData.Name = "btnReadMetaData";
            btnReadMetaData.Size = new Size(120, 25);
            btnReadMetaData.TabIndex = 2;
            btnReadMetaData.Text = "Read MetaData";
            btnReadMetaData.UseVisualStyleBackColor = true;
            btnReadMetaData.Click += BtnReadMetaData_Click;
            // 
            // btnSaveFile
            // 
            btnSaveFile.Location = new Point(671, 100);
            btnSaveFile.Name = "btnSaveFile";
            btnSaveFile.Size = new Size(30, 25);
            btnSaveFile.TabIndex = 13;
            btnSaveFile.Text = "💾";
            btnSaveFile.UseVisualStyleBackColor = true;
            btnSaveFile.Click += BtnSaveFile_Click;
            // 
            // btnBrowseFile
            // 
            btnBrowseFile.Location = new Point(635, 100);
            btnBrowseFile.Name = "btnBrowseFile";
            btnBrowseFile.Size = new Size(30, 25);
            btnBrowseFile.TabIndex = 12;
            btnBrowseFile.Text = "📁";
            btnBrowseFile.UseVisualStyleBackColor = true;
            btnBrowseFile.Click += BtnBrowseFile_Click;
            // 
            // txtFileName
            // 
            txtFileName.Location = new Point(15, 100);
            txtFileName.Name = "txtFileName";
            txtFileName.Size = new Size(614, 23);
            txtFileName.TabIndex = 1;
            txtFileName.Text = "Select File...";
            // 
            // lblFileName
            // 
            lblFileName.AutoSize = true;
            lblFileName.Location = new Point(15, 82);
            lblFileName.Name = "lblFileName";
            lblFileName.Size = new Size(63, 15);
            lblFileName.TabIndex = 0;
            lblFileName.Text = "File Name:";
            // 
            // tabPageSingleTrader
            // 
            tabPageSingleTrader.BackColor = Color.White;
            tabPageSingleTrader.Location = new Point(4, 24);
            tabPageSingleTrader.Name = "tabPageSingleTrader";
            tabPageSingleTrader.Padding = new Padding(3);
            tabPageSingleTrader.Size = new Size(1300, 773);
            tabPageSingleTrader.TabIndex = 1;
            tabPageSingleTrader.Text = "SingleTrader";
            // 
            // tabPageMultipleTraders
            // 
            tabPageMultipleTraders.BackColor = Color.White;
            tabPageMultipleTraders.Location = new Point(4, 24);
            tabPageMultipleTraders.Name = "tabPageMultipleTraders";
            tabPageMultipleTraders.Padding = new Padding(3);
            tabPageMultipleTraders.Size = new Size(1300, 773);
            tabPageMultipleTraders.TabIndex = 2;
            tabPageMultipleTraders.Text = "MultipleTraders";
            // 
            // tabPageSingleTraderOptimization
            // 
            tabPageSingleTraderOptimization.BackColor = Color.White;
            tabPageSingleTraderOptimization.Location = new Point(4, 24);
            tabPageSingleTraderOptimization.Name = "tabPageSingleTraderOptimization";
            tabPageSingleTraderOptimization.Padding = new Padding(3);
            tabPageSingleTraderOptimization.Size = new Size(1300, 773);
            tabPageSingleTraderOptimization.TabIndex = 3;
            tabPageSingleTraderOptimization.Text = "SingleTraderOptimization";
            // 
            // openFileDialog1
            // 
            openFileDialog1.FileName = "openFileDialog1";
            // 
            // Form1
            // 
            AutoScaleDimensions = new SizeF(7F, 15F);
            AutoScaleMode = AutoScaleMode.Font;
            ClientSize = new Size(1484, 1061);
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
            WindowState = FormWindowState.Maximized;
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
        private Button btnSaveFile;
        private Button btnBrowseFile;
        private TextBox txtFileName;
        private Label lblFileName;
        private Label stockDataGridViewLabel;
        private Button btnSaveConfigFile;
        private Button btnReadConfigFile;
        private Button btnUpdateFilters;
        private Button btnFirstRow;
        private Button btnPrevRow;
        private Button btnNextRow;
        private Button btnLastRow;
    }
}
