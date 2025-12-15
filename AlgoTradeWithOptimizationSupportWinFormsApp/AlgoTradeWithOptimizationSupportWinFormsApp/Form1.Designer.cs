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
            centerPanel = new Panel();
            mainTabControl = new TabControl();
            mainMenuStrip.SuspendLayout();
            mainToolStrip1.SuspendLayout();
            statusStrip.SuspendLayout();
            centerPanel.SuspendLayout();
            SuspendLayout();
            // 
            // mainMenuStrip
            // 
            mainMenuStrip.Items.AddRange(new ToolStripItem[] { fileToolStripMenuItem, editToolStripMenuItem, viewToolStripMenuItem, toolsToolStripMenuItem, helpToolStripMenuItem });
            mainMenuStrip.Location = new Point(0, 0);
            mainMenuStrip.Name = "mainMenuStrip";
            mainMenuStrip.Size = new Size(1200, 24);
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
            defaultToolbarsToolStripMenuItem.Size = new Size(180, 22);
            defaultToolbarsToolStripMenuItem.Text = "&Default";
            defaultToolbarsToolStripMenuItem.Click += defaultToolbarsToolStripMenuItem_Click;

            //
            // hideAllToolbarsToolStripMenuItem
            //
            hideAllToolbarsToolStripMenuItem.Name = "hideAllToolbarsToolStripMenuItem";
            hideAllToolbarsToolStripMenuItem.Size = new Size(180, 22);
            hideAllToolbarsToolStripMenuItem.Text = "&Hide All";
            hideAllToolbarsToolStripMenuItem.Click += hideAllToolbarsToolStripMenuItem_Click;

            //
            // showAllToolbarsToolStripMenuItem
            //
            showAllToolbarsToolStripMenuItem.Name = "showAllToolbarsToolStripMenuItem";
            showAllToolbarsToolStripMenuItem.Size = new Size(180, 22);
            showAllToolbarsToolStripMenuItem.Text = "&Show All";
            showAllToolbarsToolStripMenuItem.Click += showAllToolbarsToolStripMenuItem_Click;

            //
            // toolStripSeparator4
            //
            toolStripSeparator4.Name = "toolStripSeparator4";
            toolStripSeparator4.Size = new Size(177, 6);

            //
            // mainToolStrip1ToolStripMenuItem
            //
            mainToolStrip1ToolStripMenuItem.Checked = true;
            mainToolStrip1ToolStripMenuItem.CheckState = CheckState.Checked;
            mainToolStrip1ToolStripMenuItem.Name = "mainToolStrip1ToolStripMenuItem";
            mainToolStrip1ToolStripMenuItem.Size = new Size(180, 22);
            mainToolStrip1ToolStripMenuItem.Text = "Main ToolStrip &1";
            mainToolStrip1ToolStripMenuItem.Click += mainToolStrip1ToolStripMenuItem_Click;

            //
            // mainToolStrip2ToolStripMenuItem
            //
            mainToolStrip2ToolStripMenuItem.Name = "mainToolStrip2ToolStripMenuItem";
            mainToolStrip2ToolStripMenuItem.Size = new Size(180, 22);
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
            mainToolStrip1.Size = new Size(1200, 25);
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
            runStrategyButton.Size = new Size(80, 22);
            runStrategyButton.Text = "▶️ Run";
            runStrategyButton.ToolTipText = "Run selected strategy";

            //
            // stopStrategyButton
            //
            stopStrategyButton.DisplayStyle = ToolStripItemDisplayStyle.Text;
            stopStrategyButton.Name = "stopStrategyButton";
            stopStrategyButton.Size = new Size(60, 22);
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
            optimizeButton.Size = new Size(85, 22);
            optimizeButton.Text = "🎯 Optimize";
            optimizeButton.ToolTipText = "Optimize strategy parameters";

            //
            // exportResultsButton
            //
            exportResultsButton.DisplayStyle = ToolStripItemDisplayStyle.Text;
            exportResultsButton.Name = "exportResultsButton";
            exportResultsButton.Size = new Size(75, 22);
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
            settingsButton.Size = new Size(75, 22);
            settingsButton.Text = "⚙️ Settings";
            settingsButton.ToolTipText = "Strategy settings";
            //
            // statusStrip
            // 
            statusStrip.Items.AddRange(new ToolStripItem[] { statusLabel, progressBar, spacerLabel, timeLabel });
            statusStrip.Location = new Point(0, 725);
            statusStrip.Name = "statusStrip";
            statusStrip.Size = new Size(1200, 25);
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
            spacerLabel.Size = new Size(995, 20);
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
            topPanel.Size = new Size(1200, 50);
            topPanel.TabIndex = 3;
            topPanel.Visible = false;
            // 
            // leftPanel
            // 
            leftPanel.BorderStyle = BorderStyle.FixedSingle;
            leftPanel.Dock = DockStyle.Left;
            leftPanel.Location = new Point(0, 99);
            leftPanel.Name = "leftPanel";
            leftPanel.Size = new Size(103, 526);
            leftPanel.TabIndex = 5;
            // 
            // rightPanel
            // 
            rightPanel.BorderStyle = BorderStyle.FixedSingle;
            rightPanel.Dock = DockStyle.Right;
            rightPanel.Location = new Point(1082, 99);
            rightPanel.Name = "rightPanel";
            rightPanel.Size = new Size(118, 526);
            rightPanel.TabIndex = 6;
            rightPanel.Visible = false;
            // 
            // bottomPanel
            // 
            bottomPanel.BorderStyle = BorderStyle.FixedSingle;
            bottomPanel.Dock = DockStyle.Bottom;
            bottomPanel.Location = new Point(0, 625);
            bottomPanel.Name = "bottomPanel";
            bottomPanel.Size = new Size(1200, 100);
            bottomPanel.TabIndex = 4;
            // 
            // centerPanel
            // 
            centerPanel.BorderStyle = BorderStyle.FixedSingle;
            centerPanel.Controls.Add(mainTabControl);
            centerPanel.Dock = DockStyle.Fill;
            centerPanel.Location = new Point(103, 99);
            centerPanel.Name = "centerPanel";
            centerPanel.Size = new Size(979, 526);
            centerPanel.TabIndex = 7;
            // 
            // mainTabControl
            // 
            mainTabControl.Dock = DockStyle.Fill;
            mainTabControl.Location = new Point(0, 0);
            mainTabControl.Name = "mainTabControl";
            mainTabControl.SelectedIndex = 0;
            mainTabControl.Size = new Size(977, 524);
            mainTabControl.TabIndex = 0;
            // 
            // Form1
            // 
            AutoScaleDimensions = new SizeF(7F, 15F);
            AutoScaleMode = AutoScaleMode.Font;
            ClientSize = new Size(1200, 750);
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
            statusStrip.ResumeLayout(false);
            statusStrip.PerformLayout();
            centerPanel.ResumeLayout(false);
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
    }
}
