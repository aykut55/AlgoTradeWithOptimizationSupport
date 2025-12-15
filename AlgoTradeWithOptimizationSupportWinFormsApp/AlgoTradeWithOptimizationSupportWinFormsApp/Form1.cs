namespace AlgoTradeWithOptimizationSupportWinFormsApp
{
    public partial class Form1 : Form
    {
        private MainControlLoop? _mainLoop;

        public Form1()
        {
            InitializeComponent();
            InitializeTabControl();
            LoadInitialTabs();
            InitializeStatusTimer();
            InitializePanelSpacing();
            InitializeMainLoop();
        }

        private void InitializeMainLoop()
        {
            _mainLoop = new MainControlLoop(this);
            // Main loop'u başlatma butonuna bağlayabiliriz
            // veya form açılınca otomatik başlatılabilir
            // _mainLoop.Start();
        }

        protected override void OnFormClosing(FormClosingEventArgs e)
        {
            // Main loop'u güvenli şekilde durdur
            _mainLoop?.Stop();
            base.OnFormClosing(e);
        }

        private void InitializePanelSpacing()
        {
            // centerPanel'e padding ekle (TabControl kenarlardan uzak durur)
            centerPanel.Padding = new Padding(5);

            // Diğer panellere de padding ekleyebilirsiniz
            leftPanel.Padding = new Padding(5);
            rightPanel.Padding = new Padding(5);
            topPanel.Padding = new Padding(5);
            bottomPanel.Padding = new Padding(5);

            // Panel kenarlarını ayarla
            SetAllPanelsBorderStyle(BorderStyle.None); // None, FixedSingle, veya Fixed3D

            // DEBUG: Panelleri renklendir (işiniz bitince bu satırı yorum satırı yapın)
            SetPanelDebugColors();
            //ResetPanelColors();
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
            // Add main tabs first
            AddNewTab("SingleTrader");
            AddNewTab("MultipleTraders");
            AddNewTab("SingleTraderOptimization");

            // Add sample tabs for testing
            //AddNewTab("AAPL - MA Strategy");
            //AddNewTab("GOOG - RSI Strategy");
            //AddNewTab("TSLA - MACD Strategy");
            //AddNewTab("MSFT - Bollinger");
            //AddNewTab("AMZN - Stochastic");
            //AddNewTab("META - EMA Cross");
            //AddNewTab("NVDA - Volume");
            //AddNewTab("AMD - Momentum");
            //AddNewTab("NFLX - ATR");
            //AddNewTab("BIST100 - Combo");

            // Select the first tab after all tabs are added
            SelectTab(0);
        }

        private void InitializeTabControl()
        {
            // TabControl configuration
            mainTabControl.Multiline = false;
            mainTabControl.DrawMode = TabDrawMode.OwnerDrawFixed;
            mainTabControl.SizeMode = TabSizeMode.Fixed;
            mainTabControl.ItemSize = new Size(200, 30); // Width: 120 → 200 (daha uzun caption'lar için)
            mainTabControl.DrawItem += MainTabControl_DrawItem;

            // TabControl border/appearance
            // Appearance.Normal (default - kalın border)
            // Appearance.Buttons (buton görünümü)
            // Appearance.FlatButtons (düz butonlar - ince border)
            mainTabControl.Appearance = TabAppearance.FlatButtons;
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

        public TabPage AddNewTab(string tabName)
        {
            TabPage newTab = new TabPage(tabName);
            newTab.BackColor = Color.White;
            mainTabControl.TabPages.Add(newTab);
            // Not: SelectedTab burada ayarlanmıyor, tüm tab'ler eklendikten sonra ilki seçilecek
            return newTab;
        }

        /// <summary>
        /// Get a TabPage by index
        /// </summary>
        /// <param name="index">Tab index (0-based)</param>
        /// <returns>TabPage at the specified index, or null if not found</returns>
        public TabPage? GetTab(int index)
        {
            if (index >= 0 && index < mainTabControl.TabPages.Count)
            {
                return mainTabControl.TabPages[index];
            }
            return null;
        }

        /// <summary>
        /// Get a TabPage by name
        /// </summary>
        /// <param name="tabName">Tab name (Text property)</param>
        /// <returns>First TabPage with matching name, or null if not found</returns>
        public TabPage? GetTab(string tabName)
        {
            foreach (TabPage tab in mainTabControl.TabPages)
            {
                if (tab.Text == tabName)
                {
                    return tab;
                }
            }
            return null;
        }

        /// <summary>
        /// Select (activate) a tab by index
        /// </summary>
        /// <param name="index">Tab index (0-based)</param>
        public void SelectTab(int index)
        {
            if (index >= 0 && index < mainTabControl.TabPages.Count)
            {
                mainTabControl.SelectedIndex = index;
            }
        }

        /// <summary>
        /// Get the currently active (selected) tab
        /// </summary>
        /// <returns>Currently selected TabPage, or null if no tab is selected</returns>
        public TabPage? GetActiveTab()
        {
            return mainTabControl.SelectedTab;
        }

        /// <summary>
        /// Get the index of the currently active (selected) tab
        /// </summary>
        /// <returns>Index of selected tab (0-based), or -1 if no tab is selected</returns>
        public int GetActiveTabIndex()
        {
            return mainTabControl.SelectedIndex;
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

        private void defaultToolbarsToolStripMenuItem_Click(object sender, EventArgs e)
        {
            // Default: mainToolStrip1 visible, mainToolStrip2 hidden
            mainToolStrip1.Visible = true;
            mainToolStrip2.Visible = false;
            mainToolStrip1ToolStripMenuItem.Checked = true;
            mainToolStrip2ToolStripMenuItem.Checked = false;
            statusLabel.Text = "Toolbars set to default";
        }

        private void hideAllToolbarsToolStripMenuItem_Click(object sender, EventArgs e)
        {
            // Hide all toolbars
            mainToolStrip1.Visible = false;
            mainToolStrip2.Visible = false;
            mainToolStrip1ToolStripMenuItem.Checked = false;
            mainToolStrip2ToolStripMenuItem.Checked = false;
            statusLabel.Text = "All toolbars hidden";
        }

        private void showAllToolbarsToolStripMenuItem_Click(object sender, EventArgs e)
        {
            // Show all toolbars
            mainToolStrip1.Visible = true;
            mainToolStrip2.Visible = true;
            mainToolStrip1ToolStripMenuItem.Checked = true;
            mainToolStrip2ToolStripMenuItem.Checked = true;
            statusLabel.Text = "All toolbars visible";
        }

        private void mainToolStrip1ToolStripMenuItem_Click(object sender, EventArgs e)
        {
            mainToolStrip1.Visible = !mainToolStrip1.Visible;
            mainToolStrip1ToolStripMenuItem.Checked = mainToolStrip1.Visible;
            statusLabel.Text = mainToolStrip1.Visible ? "ToolStrip 1 shown" : "ToolStrip 1 hidden";
        }

        private void mainToolStrip2ToolStripMenuItem_Click(object sender, EventArgs e)
        {
            mainToolStrip2.Visible = !mainToolStrip2.Visible;
            mainToolStrip2ToolStripMenuItem.Checked = mainToolStrip2.Visible;
            statusLabel.Text = mainToolStrip2.Visible ? "ToolStrip 2 shown" : "ToolStrip 2 hidden";
        }

        private void statusBarToolStripMenuItem_Click(object sender, EventArgs e)
        {
            statusStrip.Visible = !statusStrip.Visible;
            statusBarToolStripMenuItem.Checked = statusStrip.Visible;
        }

        private void defaultPanelsToolStripMenuItem_Click(object sender, EventArgs e)
        {
            // Default: Left + Bottom visible, Top + Right hidden
            SetPanelsVisibility(new Dictionary<Panel, bool>
            {
                { topPanel, false },
                { leftPanel, true },
                { rightPanel, false },
                { bottomPanel, true }
            });
            statusLabel.Text = "Panels set to default layout";
        }

        private void hideAllPanelsToolStripMenuItem_Click(object sender, EventArgs e)
        {
            // Hide all panels, only centerPanel visible
            SetPanelsVisibility(new Dictionary<Panel, bool>
            {
                { topPanel, false },
                { leftPanel, false },
                { rightPanel, false },
                { bottomPanel, false }
            });
            statusLabel.Text = "All panels hidden";
        }

        private void showAllPanelsToolStripMenuItem_Click(object sender, EventArgs e)
        {
            // Show all panels
            SetPanelsVisibility(new Dictionary<Panel, bool>
            {
                { topPanel, true },
                { leftPanel, true },
                { rightPanel, true },
                { bottomPanel, true }
            });
            statusLabel.Text = "All panels visible";
        }

        private void topPanelToolStripMenuItem_Click(object sender, EventArgs e)
        {
            SetPanelVisibility(topPanel, topPanelToolStripMenuItem);
        }

        private void leftPanelToolStripMenuItem_Click(object sender, EventArgs e)
        {
            SetPanelVisibility(leftPanel, leftPanelToolStripMenuItem);
        }

        private void rightPanelToolStripMenuItem_Click(object sender, EventArgs e)
        {
            SetPanelVisibility(rightPanel, rightPanelToolStripMenuItem);
        }

        private void bottomPanelToolStripMenuItem_Click(object sender, EventArgs e)
        {
            SetPanelVisibility(bottomPanel, bottomPanelToolStripMenuItem);
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

        #region Panel Visibility Helper Methods

        /// <summary>
        /// Toggle visibility of a single panel and update its menu item check state
        /// </summary>
        private void SetPanelVisibility(Panel panel, ToolStripMenuItem menuItem)
        {
            panel.Visible = !panel.Visible;
            menuItem.Checked = panel.Visible;
            statusLabel.Text = $"{panel.Name}: {(panel.Visible ? "Visible" : "Hidden")}";
        }

        /// <summary>
        /// Set visibility for multiple panels at once
        /// </summary>
        /// <param name="panelVisibilityList">Dictionary with Panel and desired visibility state</param>
        public void SetPanelsVisibility(Dictionary<Panel, bool> panelVisibilityList)
        {
            foreach (var kvp in panelVisibilityList)
            {
                kvp.Key.Visible = kvp.Value;

                // Update corresponding menu item
                if (kvp.Key == topPanel)
                    topPanelToolStripMenuItem.Checked = kvp.Value;
                else if (kvp.Key == leftPanel)
                    leftPanelToolStripMenuItem.Checked = kvp.Value;
                else if (kvp.Key == rightPanel)
                    rightPanelToolStripMenuItem.Checked = kvp.Value;
                else if (kvp.Key == bottomPanel)
                    bottomPanelToolStripMenuItem.Checked = kvp.Value;
            }

            statusLabel.Text = "Panel visibility updated";
        }

        #endregion

        #region Panel Spacing Helper Methods

        /// <summary>
        /// Set margin (spacing) for a single panel
        /// </summary>
        /// <param name="panel">Panel to apply margin to</param>
        /// <param name="margin">Margin size in pixels (applied to all sides)</param>
        public void SetPanelMargin(Panel panel, int margin)
        {
            panel.Margin = new Padding(margin);
        }

        /// <summary>
        /// Set margin (spacing) for a single panel with individual side values
        /// </summary>
        /// <param name="panel">Panel to apply margin to</param>
        /// <param name="left">Left margin in pixels</param>
        /// <param name="top">Top margin in pixels</param>
        /// <param name="right">Right margin in pixels</param>
        /// <param name="bottom">Bottom margin in pixels</param>
        public void SetPanelMargin(Panel panel, int left, int top, int right, int bottom)
        {
            panel.Margin = new Padding(left, top, right, bottom);
        }

        /// <summary>
        /// Set margin (spacing) for all panels at once
        /// </summary>
        /// <param name="margin">Margin size in pixels (applied to all sides of all panels)</param>
        public void SetAllPanelsMargin(int margin)
        {
            topPanel.Margin = new Padding(margin);
            leftPanel.Margin = new Padding(margin);
            rightPanel.Margin = new Padding(margin);
            bottomPanel.Margin = new Padding(margin);
            centerPanel.Margin = new Padding(margin);
        }

        /// <summary>
        /// Set custom margins for each panel individually
        /// </summary>
        /// <param name="panelMargins">Dictionary with Panel and Padding values</param>
        public void SetPanelsMargin(Dictionary<Panel, Padding> panelMargins)
        {
            foreach (var kvp in panelMargins)
            {
                kvp.Key.Margin = kvp.Value;
            }
        }

        /// <summary>
        /// Set border style for a single panel
        /// </summary>
        /// <param name="panel">Panel to apply border style to</param>
        /// <param name="borderStyle">BorderStyle: None, FixedSingle, or Fixed3D</param>
        public void SetPanelBorderStyle(Panel panel, BorderStyle borderStyle)
        {
            panel.BorderStyle = borderStyle;
        }

        /// <summary>
        /// Set border style for all panels at once
        /// </summary>
        /// <param name="borderStyle">BorderStyle: None (no border), FixedSingle (thin line), Fixed3D (3D effect)</param>
        public void SetAllPanelsBorderStyle(BorderStyle borderStyle)
        {
            topPanel.BorderStyle = borderStyle;
            leftPanel.BorderStyle = borderStyle;
            rightPanel.BorderStyle = borderStyle;
            bottomPanel.BorderStyle = borderStyle;
            centerPanel.BorderStyle = borderStyle;
        }

        /// <summary>
        /// Set custom border styles for each panel individually
        /// </summary>
        /// <param name="panelBorderStyles">Dictionary with Panel and BorderStyle values</param>
        public void SetPanelsBorderStyle(Dictionary<Panel, BorderStyle> panelBorderStyles)
        {
            foreach (var kvp in panelBorderStyles)
            {
                kvp.Key.BorderStyle = kvp.Value;
            }
        }

        /// <summary>
        /// DEBUG: Set different colors for each panel to identify them easily
        /// Call this during development, disable when done
        /// </summary>
        public void SetPanelDebugColors()
        {
            topPanel.BackColor = Color.LightCoral;        // Açık kırmızı (Üst)
            leftPanel.BackColor = Color.LightBlue;        // Açık mavi (Sol)
            rightPanel.BackColor = Color.LightGreen;      // Açık yeşil (Sağ)
            bottomPanel.BackColor = Color.LightYellow;    // Açık sarı (Alt)
            //centerPanel.BackColor = Color.LightGray;      // Açık gri (Merkez)
        }

        /// <summary>
        /// Reset all panels to default white background
        /// </summary>
        public void ResetPanelColors()
        {
            topPanel.BackColor = SystemColors.Control;    // Varsayılan form rengi
            leftPanel.BackColor = SystemColors.Control;
            rightPanel.BackColor = SystemColors.Control;
            bottomPanel.BackColor = SystemColors.Control;
            centerPanel.BackColor = SystemColors.Control;
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
            // Handle mainToolStrip1 clicks through the ItemClicked event
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

        private void mainToolStrip2_ItemClicked(object sender, ToolStripItemClickedEventArgs e)
        {
            // Handle mainToolStrip2 clicks through the ItemClicked event (centralized)
            switch (e.ClickedItem?.Name)
            {
                case "runStrategyButton":
                    if (_mainLoop != null && !_mainLoop.IsRunning)
                    {
                        _mainLoop.Start();
                        _mainLoop.SetStrategyRunning(true);
                        statusLabel.Text = "Main loop started";
                    }
                    else
                    {
                        statusLabel.Text = "Main loop already running";
                    }
                    break;

                case "stopStrategyButton":
                    if (_mainLoop != null && _mainLoop.IsRunning)
                    {
                        _mainLoop.SetStrategyRunning(false);
                        _mainLoop.Stop();
                        statusLabel.Text = "Main loop stopped";
                    }
                    else
                    {
                        statusLabel.Text = "Main loop not running";
                    }
                    break;

                case "optimizeButton":
                    MessageBox.Show("Optimize functionality will be implemented here.\n\nThis will optimize strategy parameters using historical data.",
                        "Optimize", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    statusLabel.Text = "Optimize clicked";
                    break;

                case "exportResultsButton":
                    MessageBox.Show("Export Results functionality will be implemented here.\n\nThis will export backtest/optimization results to Excel or CSV.",
                        "Export Results", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    statusLabel.Text = "Export Results clicked";
                    break;

                case "settingsButton":
                    MessageBox.Show("Settings functionality will be implemented here.\n\nThis will open strategy settings dialog.",
                        "Settings", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    statusLabel.Text = "Settings clicked";
                    break;
            }
        }

        #endregion
    }
}
