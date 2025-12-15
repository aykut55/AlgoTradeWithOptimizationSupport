namespace AlgoTradeWithOptimizationSupportWinFormsApp
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
            InitializeTabControl();
            LoadInitialTabs();
            InitializeStatusTimer();
        }

        private void InitializeStatusTimer()
        {
            statusTimer = new System.Windows.Forms.Timer();
            statusTimer.Interval = 1000; // Update every second
            statusTimer.Tick += StatusTimer_Tick;
            statusTimer.Start();
        }

        private void StatusTimer_Tick(object? sender, EventArgs e)
        {
            // Update time label with current date and time
            timeLabel.Text = DateTime.Now.ToString("dd.MM.yyyy HH:mm:ss");
        }

        private void LoadInitialTabs()
        {
            // Add sample tabs for testing
            AddStrategyTab("AAPL - MA Strategy");
            AddStrategyTab("GOOG - RSI Strategy");
            AddStrategyTab("TSLA - MACD Strategy");
            AddStrategyTab("MSFT - Bollinger");
            AddStrategyTab("AMZN - Stochastic");
            AddStrategyTab("META - EMA Cross");
            AddStrategyTab("NVDA - Volume");
            AddStrategyTab("AMD - Momentum");
            AddStrategyTab("NFLX - ATR");
            AddStrategyTab("BIST100 - Combo");
        }

        private void InitializeTabControl()
        {
            // TabControl configuration
            mainTabControl.Multiline = false;
            mainTabControl.DrawMode = TabDrawMode.OwnerDrawFixed;
            mainTabControl.SizeMode = TabSizeMode.Fixed;
            mainTabControl.ItemSize = new Size(120, 30);
            mainTabControl.DrawItem += MainTabControl_DrawItem;
        }

        private void MainTabControl_DrawItem(object? sender, DrawItemEventArgs e)
        {
            // Custom tab drawing for close button support (optional)
            TabPage tabPage = mainTabControl.TabPages[e.Index];
            Rectangle tabRect = mainTabControl.GetTabRect(e.Index);

            // Draw tab background
            using (SolidBrush brush = new SolidBrush(mainTabControl.SelectedIndex == e.Index ? Color.White : Color.LightGray))
            {
                e.Graphics.FillRectangle(brush, tabRect);
            }

            // Draw tab text
            using (SolidBrush textBrush = new SolidBrush(Color.Black))
            {
                StringFormat sf = new StringFormat
                {
                    Alignment = StringAlignment.Center,
                    LineAlignment = StringAlignment.Center
                };
                e.Graphics.DrawString(tabPage.Text, mainTabControl.Font, textBrush, tabRect, sf);
            }

            // Draw tab border
            e.Graphics.DrawRectangle(Pens.Gray, tabRect);
        }

        public void AddStrategyTab(string tabName)
        {
            TabPage newTab = new TabPage(tabName);
            newTab.BackColor = Color.White;
            mainTabControl.TabPages.Add(newTab);
            mainTabControl.SelectedTab = newTab;
        }

        public TabControl GetMainTabControl() => mainTabControl;

        #region Menu Event Handlers - File

        private void newToolStripMenuItem_Click(object sender, EventArgs e)
        {
            MessageBox.Show("New file functionality will be implemented here.\n\nThis will create a new strategy tab.",
                "New Strategy", MessageBoxButtons.OK, MessageBoxIcon.Information);
            statusLabel.Text = "New file clicked";
        }

        private void openToolStripMenuItem_Click(object sender, EventArgs e)
        {
            MessageBox.Show("Open file functionality will be implemented here.\n\nThis will load a strategy from file.",
                "Open Strategy", MessageBoxButtons.OK, MessageBoxIcon.Information);
            statusLabel.Text = "Open file clicked";
        }

        private void saveToolStripMenuItem_Click(object sender, EventArgs e)
        {
            MessageBox.Show("Save file functionality will be implemented here.\n\nThis will save the current strategy.",
                "Save Strategy", MessageBoxButtons.OK, MessageBoxIcon.Information);
            statusLabel.Text = "Save file clicked";
        }

        private void saveAsToolStripMenuItem_Click(object sender, EventArgs e)
        {
            MessageBox.Show("Save As functionality will be implemented here.\n\nThis will save the strategy with a new name.",
                "Save As", MessageBoxButtons.OK, MessageBoxIcon.Information);
            statusLabel.Text = "Save As clicked";
        }

        private void exitToolStripMenuItem_Click(object sender, EventArgs e)
        {
            var result = MessageBox.Show("Are you sure you want to exit?",
                "Exit Application", MessageBoxButtons.YesNo, MessageBoxIcon.Question);

            if (result == DialogResult.Yes)
            {
                Application.Exit();
            }
        }

        #endregion

        #region Menu Event Handlers - Edit

        private void undoToolStripMenuItem_Click(object sender, EventArgs e)
        {
            MessageBox.Show("Undo functionality will be implemented here.\n\nThis will undo the last operation.",
                "Undo", MessageBoxButtons.OK, MessageBoxIcon.Information);
            statusLabel.Text = "Undo clicked";
        }

        private void redoToolStripMenuItem_Click(object sender, EventArgs e)
        {
            MessageBox.Show("Redo functionality will be implemented here.\n\nThis will redo the last undone operation.",
                "Redo", MessageBoxButtons.OK, MessageBoxIcon.Information);
            statusLabel.Text = "Redo clicked";
        }

        private void cutToolStripMenuItem_Click(object sender, EventArgs e)
        {
            MessageBox.Show("Cut functionality will be implemented here.\n\nThis will cut selected content to clipboard.",
                "Cut", MessageBoxButtons.OK, MessageBoxIcon.Information);
            statusLabel.Text = "Cut clicked";
        }

        private void copyToolStripMenuItem_Click(object sender, EventArgs e)
        {
            MessageBox.Show("Copy functionality will be implemented here.\n\nThis will copy selected content to clipboard.",
                "Copy", MessageBoxButtons.OK, MessageBoxIcon.Information);
            statusLabel.Text = "Copy clicked";
        }

        private void pasteToolStripMenuItem_Click(object sender, EventArgs e)
        {
            MessageBox.Show("Paste functionality will be implemented here.\n\nThis will paste content from clipboard.",
                "Paste", MessageBoxButtons.OK, MessageBoxIcon.Information);
            statusLabel.Text = "Paste clicked";
        }

        private void selectAllToolStripMenuItem_Click(object sender, EventArgs e)
        {
            MessageBox.Show("Select All functionality will be implemented here.\n\nThis will select all content.",
                "Select All", MessageBoxButtons.OK, MessageBoxIcon.Information);
            statusLabel.Text = "Select All clicked";
        }

        #endregion

        #region Menu Event Handlers - View

        private void toolbarsToolStripMenuItem_Click(object sender, EventArgs e)
        {
            mainToolStrip.Visible = !mainToolStrip.Visible;
            toolbarsToolStripMenuItem.Checked = mainToolStrip.Visible;
            statusLabel.Text = mainToolStrip.Visible ? "Toolbar shown" : "Toolbar hidden";
        }

        private void statusBarToolStripMenuItem_Click(object sender, EventArgs e)
        {
            statusStrip.Visible = !statusStrip.Visible;
            statusBarToolStripMenuItem.Checked = statusStrip.Visible;
        }

        private void fullScreenToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (this.WindowState == FormWindowState.Normal)
            {
                this.FormBorderStyle = FormBorderStyle.None;
                this.WindowState = FormWindowState.Maximized;
                fullScreenToolStripMenuItem.Text = "Exit &Full Screen";
                statusLabel.Text = "Full screen mode activated";
            }
            else
            {
                this.FormBorderStyle = FormBorderStyle.Sizable;
                this.WindowState = FormWindowState.Normal;
                fullScreenToolStripMenuItem.Text = "&Full Screen";
                statusLabel.Text = "Normal mode restored";
            }
        }

        #endregion

        #region Menu Event Handlers - Tools

        private void optionsToolStripMenuItem_Click(object sender, EventArgs e)
        {
            MessageBox.Show("Options dialog will be implemented here.\n\nThis will show application settings.",
                "Options", MessageBoxButtons.OK, MessageBoxIcon.Information);
            statusLabel.Text = "Options clicked";
        }

        private void preferencesToolStripMenuItem_Click(object sender, EventArgs e)
        {
            MessageBox.Show("Preferences dialog will be implemented here.\n\nThis will show user preferences.",
                "Preferences", MessageBoxButtons.OK, MessageBoxIcon.Information);
            statusLabel.Text = "Preferences clicked";
        }

        #endregion

        #region Menu Event Handlers - Help

        private void documentationToolStripMenuItem_Click(object sender, EventArgs e)
        {
            MessageBox.Show("Documentation will be implemented here.\n\nThis will show help documentation.\n\nPress F1 for quick help.",
                "Documentation", MessageBoxButtons.OK, MessageBoxIcon.Information);
            statusLabel.Text = "Documentation clicked";
        }

        private void aboutToolStripMenuItem_Click(object sender, EventArgs e)
        {
            MessageBox.Show("Algo Trade - Optimization Support\nVersion 1.0\n\nBuilt with .NET 9.0 and Windows Forms\n\n© 2024",
                "About", MessageBoxButtons.OK, MessageBoxIcon.Information);
            statusLabel.Text = "About clicked";
        }

        #endregion

        #region ToolStrip Event Handlers

        private void mainToolStrip_ItemClicked(object sender, ToolStripItemClickedEventArgs e)
        {
            // Handle toolbar clicks through the ItemClicked event
            switch (e.ClickedItem?.Name)
            {
                case "newToolStripButton":
                    newToolStripMenuItem_Click(sender, e);
                    break;
                case "openToolStripButton":
                    openToolStripMenuItem_Click(sender, e);
                    break;
                case "saveToolStripButton":
                    saveToolStripMenuItem_Click(sender, e);
                    break;
                case "cutToolStripButton":
                    cutToolStripMenuItem_Click(sender, e);
                    break;
                case "copyToolStripButton":
                    copyToolStripMenuItem_Click(sender, e);
                    break;
                case "pasteToolStripButton":
                    pasteToolStripMenuItem_Click(sender, e);
                    break;
                case "helpToolStripButton":
                    documentationToolStripMenuItem_Click(sender, e);
                    break;
            }
        }

        #endregion
    }
}
