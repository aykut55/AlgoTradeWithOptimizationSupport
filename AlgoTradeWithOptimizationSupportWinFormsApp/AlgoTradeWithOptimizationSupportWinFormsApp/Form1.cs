using AlgoTradeWithOptimizationSupportWinFormsApp.ConsoleManagement;
using AlgoTradeWithOptimizationSupportWinFormsApp.DataReader;
using AlgoTradeWithOptimizationSupportWinFormsApp.Definitions;
using AlgoTradeWithOptimizationSupportWinFormsApp.Logging;
using AlgoTradeWithOptimizationSupportWinFormsApp.Logging.Sinks;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace AlgoTradeWithOptimizationSupportWinFormsApp
{
    public partial class Form1 : Form
    {
        private MainControlLoop? _mainLoop;
        private LogManager _logManager;

        private StockDataReader stockDataReader;
        private ConcurrentDictionary<string, string> stockMetaData;

        // Data management
        private List<StockData> stockDataList;        // Data read from file (filtered or full) - source data for processing

        // Data status properties
        public bool IsStockDataLoaded => stockDataList != null;                                    // True if stockDataList is initialized (can be empty)
        public bool IsStockDataReady => stockDataList != null && stockDataList.Count > 0;         // True if stockDataList has data and ready for processing

        // Pagination fields
        private List<StockData> pagedStockDataList;   // Copy of stockDataList for pagination - source for page navigation
        private List<StockData> currentPageDataList;  // Current page data displayed in grid
        private int currentPage = 1;
        private int pageSize = 1000;
        private int totalPages = 0;

        public Form1()
        {
            InitializeComponent();

            // DateTimePicker'ları tarih + saat gösterecek şekilde ayarla
            dtpFilterDateTime1.Format = DateTimePickerFormat.Custom;
            dtpFilterDateTime1.CustomFormat = "yyyy.MM.dd HH:mm:ss";

            dtpFilterDateTime2.Format = DateTimePickerFormat.Custom;
            dtpFilterDateTime2.CustomFormat = "yyyy.MM.dd HH:mm:ss";

            InitializeTabControl();
            LoadInitialTabs();
            InitializeStatusTimer();
            InitializePanelSpacing();
            InitializeLogManager();
            InitializeMainLoop();
            InitializeFilterModeComboBox();
            InitializeStockDataGridView();
            InitializeOptimizationResultsDataGridView();  // TODO 544
            InitializePagination();
            stockDataReader = new StockDataReader();
            stockDataReader.EnableLogManager(true);
            stockMetaData = new ConcurrentDictionary<string, string>();
            stockDataList = new List<StockData>();
            pagedStockDataList = new List<StockData>();
            currentPageDataList = new List<StockData>();

            txtDataFileName.Text = @"C:\data\csvFiles\VIP\\01\VIP-X030-T.csv";

            // AlgoTrader objelerini oluştur (Form1.AlgoTrader.cs)
            CreateObjects();
        }

        private void InitializeFilterModeComboBox()
        {
            // FilterMode enum değerlerini ComboBox'a ekle
            cmbFilterMode.Items.Clear();
            foreach (StockDataReader.FilterMode mode in Enum.GetValues(typeof(StockDataReader.FilterMode)))
            {
                cmbFilterMode.Items.Add(mode);
            }

            // Varsayılan olarak "All" seçili olsun
            cmbFilterMode.SelectedItem = StockDataReader.FilterMode.All;

            // ComboBox değiştiğinde ilgili kontrollerin görünürlüğünü ayarla
            cmbFilterMode.SelectedIndexChanged += CmbFilterMode_SelectedIndexChanged;

            // Başlangıçta filtre kontrollerini gizle
            UpdateFilterControlsVisibility();
        }

        private void CmbFilterMode_SelectedIndexChanged(object? sender, EventArgs e)
        {
            UpdateFilterControlsVisibility();
        }

        private void UpdateFilterControlsVisibility()
        {
            if (cmbFilterMode.SelectedItem == null) return;

            StockDataReader.FilterMode selectedMode = (StockDataReader.FilterMode)cmbFilterMode.SelectedItem;

            // Tüm kontrolleri gizle
            lblFilterValue1.Visible = false;
            txtFilterValue1.Visible = false;
            lblFilterValue2.Visible = false;
            txtFilterValue2.Visible = false;
            dtpFilterDateTime1.Visible = false;
            dtpFilterDateTime2.Visible = false;

            switch (selectedMode)
            {
                case StockDataReader.FilterMode.All:
                    // Hiçbir ek kontrol gösterme
                    break;

                case StockDataReader.FilterMode.LastN:
                case StockDataReader.FilterMode.FirstN:
                    // Sadece N değeri için bir TextBox göster
                    lblFilterValue1.Text = "N";
                    lblFilterValue1.Visible = true;
                    txtFilterValue1.Visible = true;
                    break;

                case StockDataReader.FilterMode.IndexRange:
                    // Start ve End index için iki TextBox göster
                    lblFilterValue1.Text = "Start Index";
                    lblFilterValue1.Visible = true;
                    txtFilterValue1.Visible = true;
                    lblFilterValue2.Text = "End Index";
                    lblFilterValue2.Visible = true;
                    txtFilterValue2.Visible = true;
                    break;

                case StockDataReader.FilterMode.AfterDateTime:
                    // DateTimePicker'ı txtFilterValue1'in yerine yerleştir
                    lblFilterValue1.Text = "After";
                    lblFilterValue1.Visible = true;
                    dtpFilterDateTime1.Location = txtFilterValue1.Location;
                    dtpFilterDateTime1.Visible = true;
                    break;

                case StockDataReader.FilterMode.BeforeDateTime:
                    // DateTimePicker'ı txtFilterValue1'in yerine yerleştir
                    lblFilterValue1.Text = "Before";
                    lblFilterValue1.Visible = true;
                    dtpFilterDateTime1.Location = txtFilterValue1.Location;
                    dtpFilterDateTime1.Visible = true;
                    break;

                case StockDataReader.FilterMode.DateTimeRange:
                    // DateTimePicker'ları yerleştir
                    lblFilterValue1.Text = "From";
                    lblFilterValue1.Visible = true;
                    dtpFilterDateTime1.Location = txtFilterValue1.Location;
                    dtpFilterDateTime1.Visible = true;

                    lblFilterValue2.Text = "To";
                    lblFilterValue2.Visible = true;
                    // lblFilterValue2 ve dtpFilterDateTime2'yi yan yana yerleştir
                    int spacing = 0; // Boşluk miktarı
                    lblFilterValue2.Location = new Point(dtpFilterDateTime1.Right + spacing, lblFilterValue1.Location.Y);
                    dtpFilterDateTime2.Location = new Point(lblFilterValue2.Left + 5, txtFilterValue1.Location.Y);
                    dtpFilterDateTime2.Visible = true;
                    break;
            }
        }

        private void InitializeStockDataGridView()
        {
            // Grid ayarları
            stockDataGridView.AutoGenerateColumns = false;
            stockDataGridView.ReadOnly = true;
            stockDataGridView.SelectionMode = DataGridViewSelectionMode.CellSelect;
            stockDataGridView.MultiSelect = false;
            stockDataGridView.AllowUserToAddRows = false;
            stockDataGridView.AllowUserToDeleteRows = false;
            stockDataGridView.RowHeadersVisible = false;
            stockDataGridView.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.AllCells;

            // Column Header Styling (Başlıklar için özel stil)
            stockDataGridView.EnableHeadersVisualStyles = false; // Özel stil için gerekli
            stockDataGridView.ColumnHeadersDefaultCellStyle.BackColor = Color.Orange;
            stockDataGridView.ColumnHeadersDefaultCellStyle.ForeColor = Color.White;
            stockDataGridView.ColumnHeadersDefaultCellStyle.Font = new Font("Segoe UI", 9F, FontStyle.Bold);
            stockDataGridView.ColumnHeadersDefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleCenter;
            stockDataGridView.ColumnHeadersHeight = 30;

            // Alternatif satır renklendirme (Zebra striping - daha okunabilir)
            stockDataGridView.AlternatingRowsDefaultCellStyle.BackColor = Color.LightGray;
            stockDataGridView.DefaultCellStyle.BackColor = Color.White;

            // Kolonları temizle
            stockDataGridView.Columns.Clear();

            // Ana Veri Kolonları
            stockDataGridView.Columns.Add(new DataGridViewTextBoxColumn { DataPropertyName = "Id", HeaderText = "Id" });
            stockDataGridView.Columns.Add(new DataGridViewTextBoxColumn { DataPropertyName = "DateTime", HeaderText = "DateTime", DefaultCellStyle = new DataGridViewCellStyle { Format = "yyyy.MM.dd HH:mm:ss" } });
            stockDataGridView.Columns.Add(new DataGridViewTextBoxColumn { DataPropertyName = "Date", HeaderText = "Date", DefaultCellStyle = new DataGridViewCellStyle { Format = "yyyy.MM.dd" } });
            stockDataGridView.Columns.Add(new DataGridViewTextBoxColumn { DataPropertyName = "Time", HeaderText = "Time" });
            stockDataGridView.Columns.Add(new DataGridViewTextBoxColumn { DataPropertyName = "Open", HeaderText = "Open", DefaultCellStyle = new DataGridViewCellStyle { Format = "N2" } });
            stockDataGridView.Columns.Add(new DataGridViewTextBoxColumn { DataPropertyName = "High", HeaderText = "High", DefaultCellStyle = new DataGridViewCellStyle { Format = "N2" } });
            stockDataGridView.Columns.Add(new DataGridViewTextBoxColumn { DataPropertyName = "Low", HeaderText = "Low", DefaultCellStyle = new DataGridViewCellStyle { Format = "N2" } });
            stockDataGridView.Columns.Add(new DataGridViewTextBoxColumn { DataPropertyName = "Close", HeaderText = "Close", DefaultCellStyle = new DataGridViewCellStyle { Format = "N2" } });
            stockDataGridView.Columns.Add(new DataGridViewTextBoxColumn { DataPropertyName = "Volume", HeaderText = "Volume", DefaultCellStyle = new DataGridViewCellStyle { Format = "N0" } });
            stockDataGridView.Columns.Add(new DataGridViewTextBoxColumn { DataPropertyName = "Size", HeaderText = "Size (Lot)", DefaultCellStyle = new DataGridViewCellStyle { Format = "N0" } });

            // Hesaplanmış Değerler
            stockDataGridView.Columns.Add(new DataGridViewTextBoxColumn { DataPropertyName = "Diff", HeaderText = "Diff", DefaultCellStyle = new DataGridViewCellStyle { Format = "N2" } });
            stockDataGridView.Columns.Add(new DataGridViewTextBoxColumn { DataPropertyName = "ChangePct", HeaderText = "Change %", DefaultCellStyle = new DataGridViewCellStyle { Format = "N2" } });
            stockDataGridView.Columns.Add(new DataGridViewTextBoxColumn { DataPropertyName = "Range", HeaderText = "Range", DefaultCellStyle = new DataGridViewCellStyle { Format = "N2" } });
            stockDataGridView.Columns.Add(new DataGridViewTextBoxColumn { DataPropertyName = "BodySize", HeaderText = "Body Size", DefaultCellStyle = new DataGridViewCellStyle { Format = "N2" } });

            // Bayraklar (Boolean) - CheckBoxColumn olarak gösterilebilir
            stockDataGridView.Columns.Add(new DataGridViewCheckBoxColumn { DataPropertyName = "IsBullish", HeaderText = "Bullish" });
            stockDataGridView.Columns.Add(new DataGridViewCheckBoxColumn { DataPropertyName = "IsBearish", HeaderText = "Bearish" });

            stockDataGridView.SelectionChanged += StockDataGridView_SelectionChanged;
        }

        /// <summary>
        /// TODO 544: Initialize Optimization Results DataGridView
        /// Optimization sonuçlarını göstermek için DataGridView'ı hazırla
        /// Kolonlar AppendSingleOptSummaryToFiles methodundaki sütun sıralamasına göre oluşturulur
        /// Stil stockDataGridView ile aynı (Orange header, LightGray/White rows)
        /// Hücre seçimi ve kopyalama aktif (Ctrl+C ile kopyalanabilir)
        /// </summary>
        private void InitializeOptimizationResultsDataGridView()
        {
            // Grid ayarları
            dataGridViewOptimizationResults.AutoGenerateColumns = false;
            dataGridViewOptimizationResults.ReadOnly = true;
            dataGridViewOptimizationResults.SelectionMode = DataGridViewSelectionMode.CellSelect;  // Hücre seçimi
            dataGridViewOptimizationResults.MultiSelect = true;  // Çoklu seçim aktif
            dataGridViewOptimizationResults.AllowUserToAddRows = false;
            dataGridViewOptimizationResults.AllowUserToDeleteRows = false;
            dataGridViewOptimizationResults.RowHeadersVisible = false;
            dataGridViewOptimizationResults.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.AllCells;

            // Kopyalama ayarları (Ctrl+C ile kopyalanabilir)
            dataGridViewOptimizationResults.ClipboardCopyMode = DataGridViewClipboardCopyMode.EnableWithAutoHeaderText;

            // Column Header Styling (stockDataGridView ile aynı stil)
            dataGridViewOptimizationResults.EnableHeadersVisualStyles = false; // Özel stil için gerekli
            dataGridViewOptimizationResults.ColumnHeadersDefaultCellStyle.BackColor = Color.Orange;
            dataGridViewOptimizationResults.ColumnHeadersDefaultCellStyle.ForeColor = Color.White;
            dataGridViewOptimizationResults.ColumnHeadersDefaultCellStyle.Font = new Font("Segoe UI", 9F, FontStyle.Bold);
            dataGridViewOptimizationResults.ColumnHeadersDefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleCenter;
            dataGridViewOptimizationResults.ColumnHeadersHeight = 30;

            // Alternatif satır renklendirme (Zebra striping - daha okunabilir)
            dataGridViewOptimizationResults.AlternatingRowsDefaultCellStyle.BackColor = Color.LightGray;
            dataGridViewOptimizationResults.DefaultCellStyle.BackColor = Color.White;

            // NOT: Kolonlar CSV/TXT dosyası okunurken dinamik olarak oluşturulacak
            // Bu method sadece grid'in genel ayarlarını yapar
            // Kolonların sıralaması AppendSingleOptSummaryToFiles methodundaki gibi olacak
        }

        private void InitializeLogManager()
        {
            // ConsoleManager ile ana console'u aç
            ConsoleManager.Instance.OpenMainConsole("AlgoTrade - Console");

            _logManager = LogManager.Instance;
            _logManager.RegisterSink(new RichTextBoxSink(richTextBox1));
            _logManager.RegisterSink(new ConsoleSink()); // Ana console (index 0)
            _logManager.RegisterSink(new FileSink("logs", "app.log"));

            // Add DoubleClick event to clear logs
            richTextBox1.DoubleClick += RichTextBox1_DoubleClick;

            // Test mesajı
            LogManager.LogInfo("Console window opened successfully!");
            ConsoleManager.WriteLine("=================================", ConsoleColor.Cyan);
            ConsoleManager.WriteLine("AlgoTrade Console Initialized", ConsoleColor.Green);
            ConsoleManager.WriteLine("=================================", ConsoleColor.Cyan);
        }

        private void RichTextBox1_DoubleClick(object? sender, EventArgs e)
        {
            richTextBox1.Clear();
        }

        private void InitializeMainLoop()
        {
            _mainLoop = new MainControlLoop(this);
            // Main loop'u başlatma butonuna bağlayabiliriz
            // veya form açılınca otomatik başlatılabilir
            _mainLoop.Start();
        }

        protected override void OnFormClosing(FormClosingEventArgs e)
        {
            // Main loop'u güvenli şekilde durdur
            _mainLoop?.Stop();

            // LogManager'ı temizle
            _logManager?.Dispose();

            // Tüm console'ları kapat
            ConsoleManager.Instance.Dispose();

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
            CorrectControlsZOrder();
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
            // Add main tabs (runtime - bunlar tasarım zamanında eklenen tabların peşine gelir)
            AddNewTab("SingleTrader1");
            AddNewTab("MultipleTraders1");
            AddNewTab("SingleTraderOptimization1");

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
            CorrectControlsZOrder();
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
            CorrectControlsZOrder();
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
            // bottomPanel.BackColor = Color.LightYellow;    // Açık sarı (Alt)
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

        /// <summary>
        /// Corrects the Z-Order of controls to ensure proper docking layout.
        /// Desired Packing Order (Edge to Center):
        /// Menu -> Strip1 -> Strip2 -> topPanel -> Status -> Bottom -> Sides -> Center
        /// </summary>
        public void CorrectControlsZOrder()
        {
            this.SuspendLayout();
            // BringToFront pushes item to Index 0 (Innermost).
            // So we call Outermost first, Innermost last.

            mainMenuStrip.BringToFront();
            mainToolStrip1.BringToFront();
            mainToolStrip2.BringToFront();
            topPanel.BringToFront();

            statusStrip.BringToFront();
            bottomPanel.BringToFront();

            leftPanel.BringToFront();
            rightPanel.BringToFront();

            centerPanel.BringToFront();
            this.ResumeLayout(false);
            this.PerformLayout();
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

        #region File Browse Event Handler

        private void BtnBrowseFile1_Click(object? sender, EventArgs e)
        {
            // OpenFileDialog ayarları
            openFileDialog1.Filter = "CSV Files (*.csv)|*.csv|Text Files (*.txt)|*.txt|Binary Files (*.bin)|*.bin|All Files (*.*)|*.*";
            openFileDialog1.Title = "Select Stock Data File";
            openFileDialog1.InitialDirectory = Application.StartupPath;

            // Dosya seçim dialogu aç
            if (openFileDialog1.ShowDialog() == DialogResult.OK)
            {
                txtDataFileName.Text = openFileDialog1.FileName;
                statusLabel.Text = $"File selected: {Path.GetFileName(openFileDialog1.FileName)}";
            }
        }

        private void BtnSaveFile1_Click(object? sender, EventArgs e)
        {
            // SaveFileDialog ayarları
            saveFileDialog1.Filter = "CSV Files (*.csv)|*.csv|Text Files (*.txt)|*.txt|Binary Files (*.bin)|*.bin|All Files (*.*)|*.*";
            saveFileDialog1.Title = "Save Stock Data File";
            saveFileDialog1.InitialDirectory = Application.StartupPath;

            // Eğer txtFileName'de bir dosya adı varsa, onu varsayılan olarak kullan
            if (!string.IsNullOrWhiteSpace(txtDataFileName.Text))
            {
                saveFileDialog1.FileName = Path.GetFileName(txtDataFileName.Text);
            }

            // Dosya kaydetme dialogu aç
            if (saveFileDialog1.ShowDialog() == DialogResult.OK)
            {
                txtDataFileName.Text = saveFileDialog1.FileName;
                statusLabel.Text = $"Save location: {Path.GetFileName(saveFileDialog1.FileName)}";
            }
        }

        private void BtnBrowseFile2_Click(object? sender, EventArgs e)
        {
            // OpenFileDialog ayarları
            openFileDialog2.Filter = "CSV Files (*.csv)|*.csv|Text Files (*.txt)|*.txt|Binary Files (*.bin)|*.bin|All Files (*.*)|*.*";
            openFileDialog2.Title = "Select Stock Data File";
            openFileDialog2.InitialDirectory = Application.StartupPath;

            // Dosya seçim dialogu aç
            if (openFileDialog2.ShowDialog() == DialogResult.OK)
            {
                txtConfigFileName.Text = openFileDialog2.FileName;
                statusLabel.Text = $"File selected: {Path.GetFileName(openFileDialog2.FileName)}";
            }
        }

        private void BtnSaveFile2_Click(object? sender, EventArgs e)
        {
            // SaveFileDialog ayarları
            saveFileDialog2.Filter = "CSV Files (*.csv)|*.csv|Text Files (*.txt)|*.txt|Binary Files (*.bin)|*.bin|All Files (*.*)|*.*";
            saveFileDialog2.Title = "Save Stock Data File";
            saveFileDialog2.InitialDirectory = Application.StartupPath;

            // Eğer txtFileName'de bir dosya adı varsa, onu varsayılan olarak kullan
            if (!string.IsNullOrWhiteSpace(txtConfigFileName.Text))
            {
                saveFileDialog2.FileName = Path.GetFileName(txtConfigFileName.Text);
            }

            // Dosya kaydetme dialogu aç
            if (saveFileDialog2.ShowDialog() == DialogResult.OK)
            {
                txtConfigFileName.Text = saveFileDialog2.FileName;
                statusLabel.Text = $"Save location: {Path.GetFileName(saveFileDialog2.FileName)}";
            }
        }

        #endregion

        #region Stock Data Event Handlers

        private void BtnClear_Click(object? sender, EventArgs e)
        {
            // TextBoxMetaData'yı temizle
            textBoxMetaData.Clear();
            stockMetaData.Clear();
            statusLabel.Text = "Metadata cleared";

            textBoxMetaData.Text = @"Kayit Zamani     : 
GrafikSembol     : 
GrafikPeriyot    : 
BarCount         : 
Başlangiç Tarihi : 
Bitiş Tarihi     : 
Format           : ";

            btnClearStockData_Click(sender, e);
        }

        private void BtnReadMetaData_Click(object? sender, EventArgs e)
        {
            string fileName = txtDataFileName.Text;
            string fileDir = "";
            string filePath = "";
            try
            {
                if (!File.Exists(fileName))
                {
                    statusLabel.Text = $"File does not exit : {fileName}";
                }
                else
                {
                    fileDir = Path.GetDirectoryName(fileName);
                    filePath = Path.Combine(fileDir, fileName);

                    statusLabel.Text = $"Reading Meta Data from : {filePath}";

                    stockMetaData = stockDataReader.ReadMetaData(filePath);

                    // Dictionary'den direkt eriş
                    var kayitZamani = stockMetaData.GetValueOrDefault("Kayit_Zamani", "N/A");
                    var grafikSembol = stockMetaData.GetValueOrDefault("GrafikSembol", "N/A");
                    var grafikPeriyot = stockMetaData.GetValueOrDefault("GrafikPeriyot", "N/A");
                    var barCount = stockMetaData.GetValueOrDefault("BarCount", "N/A");
                    var baslangicTarihi = stockMetaData.GetValueOrDefault("Baslangic_Tarihi", "N/A");
                    var bitisTarihi = stockMetaData.GetValueOrDefault("Bitis_Tarihi", "N/A");
                    var format = stockMetaData.GetValueOrDefault("Format", "N/A");

                    // Padding uzunluğu (en uzun label'ın uzunluğu)
                    int padding = 17;
                    var sb = new StringBuilder();
                    sb.AppendLine($"{"Kayit Zamani".PadRight(padding)}: {kayitZamani}");
                    sb.AppendLine($"{"GrafikSembol".PadRight(padding)}: {grafikSembol}");
                    sb.AppendLine($"{"GrafikPeriyot".PadRight(padding)}: {grafikPeriyot}");
                    sb.AppendLine($"{"BarCount".PadRight(padding)}: {barCount}");
                    sb.AppendLine($"{"Başlangıç Tarihi".PadRight(padding)}: {baslangicTarihi}");
                    sb.AppendLine($"{"Bitiş Tarihi".PadRight(padding)}: {bitisTarihi}");
                    sb.AppendLine($"{"Format".PadRight(padding)}: {format}");

                    textBoxMetaData.Text = sb.ToString();

                    statusLabel.Text = "Metadata loaded";
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"An error occurred while reading data: {ex.Message}", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void btnClearStockData_Click(object sender, EventArgs e)
        {
            stockDataList.Clear();
            stockDataReader.Clear();

            // Pagination verilerini temizle
            pagedStockDataList = new List<StockData>();
            currentPageDataList = new List<StockData>();
            currentPage = 1;
            totalPages = 0;

            // Grid'i temizle
            stockDataGridView.SuspendLayout();
            try
            {
                stockDataGridView.DataSource = null;
            }
            finally
            {
                stockDataGridView.ResumeLayout();
            }

            // UI bilgilerini güncelle
            UpdatePaginationInfo();
            UpdateStockDataGridViewLabel();

            string currentTime = DateTime.Now.ToString("yyyy.MM.dd HH:mm:ss", CultureInfo.InvariantCulture);
            lblReadStockDataTime.Text = $"Last StockData Read Time : ";
        }

        /// <summary>
        /// Clear all log files (.txt and .csv) in the "logs" folder
        /// </summary>
        private void btnClearLogFiles_Click(object sender, EventArgs e)
        {
            string logsFolder = "logs";

            try
            {
                // Logs klasörü var mı kontrol et
                if (!Directory.Exists(logsFolder))
                {
                    LogManager.Log($"Logs folder not found: {logsFolder}");
                    MessageBox.Show($"Logs folder not found: {logsFolder}", "Info",
                        MessageBoxButtons.OK, MessageBoxIcon.Information);
                    return;
                }
/*
                // Kullanıcıya onay sor
                var result = MessageBox.Show(
                    $"Are you sure you want to delete all .txt and .csv files in '{logsFolder}' folder?",
                    "Confirm Delete",
                    MessageBoxButtons.YesNo,
                    MessageBoxIcon.Warning);

                if (result != DialogResult.Yes)
                {
                    LogManager.Log("Log files deletion cancelled by user.");
                    return;
                }
*/
                // TXT dosyalarını bul ve sil
                var txtFiles = Directory.GetFiles(logsFolder, "*.txt");
                int txtDeleted = 0;
                int txtFailed = 0;

                LogManager.Log($"Found {txtFiles.Length} .txt files in {logsFolder}");

                foreach (var file in txtFiles)
                {
                    try
                    {
                        File.Delete(file);
                        LogManager.Log($"  ✓ Deleted: {file}");
                        txtDeleted++;
                    }
                    catch (Exception ex)
                    {
                        LogManager.Log($"  ✗ Failed to delete {file}: {ex.Message}");
                        txtFailed++;
                    }
                }

                // CSV dosyalarını bul ve sil
                var csvFiles = Directory.GetFiles(logsFolder, "*.csv");
                int csvDeleted = 0;
                int csvFailed = 0;

                LogManager.Log($"Found {csvFiles.Length} .csv files in {logsFolder}");

                foreach (var file in csvFiles)
                {
                    try
                    {
                        File.Delete(file);
                        LogManager.Log($"  ✓ Deleted: {file}");
                        csvDeleted++;
                    }
                    catch (Exception ex)
                    {
                        LogManager.Log($"  ✗ Failed to delete {file}: {ex.Message}");
                        csvFailed++;
                    }
                }

                // Özet
                int totalDeleted = txtDeleted + csvDeleted;
                int totalFailed = txtFailed + csvFailed;

                string summary = $"Log files deletion completed:\n\n" +
                                $"TXT files: {txtDeleted} deleted, {txtFailed} failed\n" +
                                $"CSV files: {csvDeleted} deleted, {csvFailed} failed\n\n" +
                                $"Total: {totalDeleted} deleted, {totalFailed} failed";

                LogManager.Log("========================================");
                LogManager.Log(summary.Replace("\n", " "));
                LogManager.Log("========================================");
/*
                MessageBox.Show(summary, "Log Files Cleared",
                    MessageBoxButtons.OK,
                    totalFailed > 0 ? MessageBoxIcon.Warning : MessageBoxIcon.Information);
*/
            }
            catch (Exception ex)
            {
                string errorMsg = $"Error clearing log files: {ex.Message}";
                LogManager.Log(errorMsg);
                MessageBox.Show(errorMsg, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private async void BtnReadStockData_Click(object? sender, EventArgs e)
        {
            string fileName = txtDataFileName.Text;
            string fileDir = "";
            string filePath = "";

            try
            {
                if (!File.Exists(fileName))
                {
                    statusLabel.Text = $"File does not exit : {fileName}";
                }
                else
                {
                    BtnReadMetaData_Click(sender, e);

                    // ComboBox'tan seçili modu al
                    if (cmbFilterMode.SelectedItem == null)
                        return;

                    fileDir = Path.GetDirectoryName(fileName);
                    filePath = Path.Combine(fileDir, fileName);

                    statusLabel.Text = $"Loading data from : {filePath}";

                    stockDataList.Clear();

                    stockDataReader.Clear();

                    stockDataReader.StartTimer();

                    int n1 = 0, n2 = 0;
                    DateTime? dt1 = null, dt2 = null;

                    if (int.TryParse(txtFilterValue1.Text, out int val1)) n1 = val1;
                    if (int.TryParse(txtFilterValue2.Text, out int val2)) n2 = val2;

                    dt1 = dtpFilterDateTime1.Value;
                    dt2 = dtpFilterDateTime2.Value;

                    StockDataReader.FilterMode mode = (StockDataReader.FilterMode)cmbFilterMode.SelectedItem;

                    // Async okuma - UI thread'i bloklamaz
                    await Task.Run(() =>
                    {
                        stockMetaData = stockDataReader.ReadMetaData(filePath);

                        if (mode == StockDataReader.FilterMode.All)
                        {
                            stockDataList = stockDataReader.ReadDataFast(filePath);
                        }
                        else if (mode == StockDataReader.FilterMode.LastN)
                        {
                            stockDataList = stockDataReader.ReadDataFast(filePath, StockDataReader.FilterMode.LastN, n1);
                        }
                        else if (mode == StockDataReader.FilterMode.FirstN)
                        {
                            stockDataList = stockDataReader.ReadDataFast(filePath, StockDataReader.FilterMode.FirstN, n1);
                        }
                        else if (mode == StockDataReader.FilterMode.IndexRange)
                        {
                            stockDataList = stockDataReader.ReadDataFast(filePath, StockDataReader.FilterMode.IndexRange, n1, n2);
                        }
                        else if (mode == StockDataReader.FilterMode.AfterDateTime)
                        {
                            stockDataList = stockDataReader.ReadDataFast(filePath, StockDataReader.FilterMode.AfterDateTime, dt1: dt1);
                        }
                        else if (mode == StockDataReader.FilterMode.BeforeDateTime)
                        {
                            stockDataList = stockDataReader.ReadDataFast(filePath, StockDataReader.FilterMode.BeforeDateTime, dt1: dt1);
                        }
                        else if (mode == StockDataReader.FilterMode.DateTimeRange)
                        {
                            stockDataList = stockDataReader.ReadDataFast(filePath, StockDataReader.FilterMode.DateTimeRange, dt1: dt1, dt2: dt2);
                        }
                    });

                    stockDataReader.StopTimer();

                    if (stockDataList == null || !stockDataList.Any())
                    {
                        MessageBox.Show("No valid data was read from the file.", "Information", MessageBoxButtons.OK, MessageBoxIcon.Information);
                        return;
                    }

                    long t1 = stockDataReader.GetElapsedTimeMsec();
                    int itemsCount = stockDataReader.ReadCount;

                    // Veriyi pagedStockDataList'e aktar ve pagination hazırla
                    pagedStockDataList = new List<StockData>(stockDataList);
                    totalPages = (int)Math.Ceiling((double)pagedStockDataList.Count / pageSize);

                    statusLabel.Text = $"Data loaded: {itemsCount:N0} records in {t1} ms. Preparing pagination...";

                    // İlk sayfayı yükle
                    await LoadPageAsync(1);

                    string currentTime = DateTime.Now.ToString("yyyy.MM.dd HH:mm:ss", CultureInfo.InvariantCulture);
                    lblReadStockDataTime.Text = $"Last StockData Read Time : {currentTime}";

                    var grafikSembol = "";
                    var grafikPeriyot = "0";                    
                    if (!stockMetaData.IsEmpty)
                    {
                        grafikSembol = stockMetaData.GetValueOrDefault("GrafikSembol", "N/A");
                        grafikPeriyot = stockMetaData.GetValueOrDefault("GrafikPeriyot", "N/A");                        
                    }
                    if (algoTrader != null)
                    {
                        algoTrader.SymbolName = grafikSembol;
                        algoTrader.SymbolPeriod = grafikPeriyot;
                        algoTrader.SystemId = "...";
                        algoTrader.SystemName = "...";
                        algoTrader.StrategyId = "...";
                        algoTrader.StrategyName = "...";

                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"An error occurred while reading data: {ex.Message}", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        /// <summary>
        /// DataGridView'e veriyi performanslı şekilde async yükler
        /// </summary>
        private async Task LoadDataToGridAsync(List<StockData> dataList)
        {
            await Task.Run(() =>
            {
                // UI thread'de çalışması gereken kısım
                this.Invoke((MethodInvoker)delegate
                {
                    stockDataGridView.SuspendLayout();
                    try
                    {
                        stockDataGridView.DataSource = null; // Önce temizle
                        stockDataGridView.DataSource = dataList;
                    }
                    finally
                    {
                        stockDataGridView.ResumeLayout();
                    }
                });
            });
        }

        #endregion

        #region Pagination Methods

        /// <summary>
        /// Pagination kontrollerini initialize eder
        /// </summary>
        private void InitializePagination()
        {
            // Page Size ComboBox default değerini ayarla (1000)
            cmbPageSize.SelectedIndex = 2;

            currentPage = 1;
            pageSize = 1000;
            totalPages = 0;
        }

        /// <summary>
        /// Belirtilen sayfayı DataGridView'e yükler
        /// </summary>
        private async Task LoadPageAsync(int pageNumber)
        {
            if (pagedStockDataList == null || pagedStockDataList.Count == 0)
                return;

            // Sayfa sınırlarını kontrol et
            if (pageNumber < 1)
                pageNumber = 1;
            if (pageNumber > totalPages && totalPages > 0)
                pageNumber = totalPages;

            currentPage = pageNumber;

            // Sayfa için veriyi hazırla
            int skip = (currentPage - 1) * pageSize;
            currentPageDataList = pagedStockDataList.Skip(skip).Take(pageSize).ToList();

            // SelectionChanged event'ini geçici olarak devre dışı bırak (flickering önleme)
            stockDataGridView.SelectionChanged -= StockDataGridView_SelectionChanged;

            try
            {
                // DataGridView'e yükle
                await LoadDataToGridAsync(currentPageDataList);

                // Pagination bilgisini güncelle
                UpdatePaginationInfo();
                UpdateStockDataGridViewLabel();
            }
            finally
            {
                // SelectionChanged event'ini tekrar aktif et
                stockDataGridView.SelectionChanged += StockDataGridView_SelectionChanged;
            }
        }

        /// <summary>
        /// Pagination bilgilerini günceller (Label, buton durumları vb.)
        /// </summary>
        private void UpdatePaginationInfo()
        {
            if (pagedStockDataList == null || pagedStockDataList.Count == 0)
            {
                statusLabel.Text = "No data loaded";
                return;
            }

            int startRow = (currentPage - 1) * pageSize + 1;
            int endRow = Math.Min(currentPage * pageSize, pagedStockDataList.Count);

            statusLabel.Text = $"Showing rows {startRow:N0}-{endRow:N0} of {pagedStockDataList.Count:N0} | Page {currentPage}/{totalPages} | Page size: {pageSize}";
        }

        /// <summary>
        /// İlk sayfaya git
        /// </summary>
        private async void BtnFirstPage_Click(object sender, EventArgs e)
        {
            await LoadPageAsync(1);
        }

        /// <summary>
        /// Önceki sayfaya git
        /// </summary>
        private async void BtnPreviousPage_Click(object sender, EventArgs e)
        {
            await LoadPageAsync(currentPage - 1);
        }

        /// <summary>
        /// Sonraki sayfaya git
        /// </summary>
        private async void BtnNextPage_Click(object sender, EventArgs e)
        {
            await LoadPageAsync(currentPage + 1);
        }

        /// <summary>
        /// Son sayfaya git
        /// </summary>
        private async void BtnLastPage_Click(object sender, EventArgs e)
        {
            await LoadPageAsync(totalPages);
        }

        /// <summary>
        /// Sayfa boyutunu değiştir
        /// </summary>
        private async void CmbPageSize_SelectedIndexChanged(object sender, EventArgs e)
        {
            // ComboBox'tan seçilen değeri al
            if (sender is ComboBox cmb && cmb.SelectedItem != null)
            {
                if (int.TryParse(cmb.SelectedItem.ToString(), out int newPageSize))
                {
                    pageSize = newPageSize;

                    // Toplam sayfa sayısını yeniden hesapla
                    if (pagedStockDataList != null && pagedStockDataList.Count > 0)
                    {
                        totalPages = (int)Math.Ceiling((double)pagedStockDataList.Count / pageSize);

                        // Mevcut sayfayı yeniden yükle
                        await LoadPageAsync(1); // İlk sayfaya dön
                    }
                }
            }
        }

        #endregion

        private void BtnFirstRow_Click(object sender, EventArgs e)
        {
            if (stockDataGridView.Rows.Count > 0)
            {
                stockDataGridView.CurrentCell = stockDataGridView.Rows[0].Cells[0];
                stockDataGridView.FirstDisplayedScrollingRowIndex = 0;
                UpdateStockDataGridViewLabel();
            }
        }

        private void BtnPrevRow_Click(object sender, EventArgs e)
        {
            if (stockDataGridView.Rows.Count > 0 && stockDataGridView.CurrentRow != null)
            {
                int prevIndex = stockDataGridView.CurrentRow.Index - 1;
                if (prevIndex >= 0)
                {
                    stockDataGridView.CurrentCell = stockDataGridView.Rows[prevIndex].Cells[0];
                    stockDataGridView.FirstDisplayedScrollingRowIndex = prevIndex;
                    UpdateStockDataGridViewLabel();
                }
            }
        }

        private void BtnNextRow_Click(object sender, EventArgs e)
        {
            if (stockDataGridView.Rows.Count > 0 && stockDataGridView.CurrentRow != null)
            {
                int nextIndex = stockDataGridView.CurrentRow.Index + 1;
                if (nextIndex < stockDataGridView.Rows.Count)
                {
                    stockDataGridView.CurrentCell = stockDataGridView.Rows[nextIndex].Cells[0];
                    stockDataGridView.FirstDisplayedScrollingRowIndex = nextIndex;
                    UpdateStockDataGridViewLabel();
                }
            }
        }

        private void BtnLastRow_Click(object sender, EventArgs e)
        {
            if (stockDataGridView.Rows.Count > 0)
            {
                int lastIndex = stockDataGridView.Rows.Count - 1;
                stockDataGridView.CurrentCell = stockDataGridView.Rows[lastIndex].Cells[0];
                stockDataGridView.FirstDisplayedScrollingRowIndex = lastIndex;
                UpdateStockDataGridViewLabel();
            }
        }

        private void StockDataGridView_SelectionChanged(object? sender, EventArgs e)
        {
            UpdateStockDataGridViewLabel();
        }

        private void UpdateStockDataGridViewLabel()
        {
            if (stockDataGridView.Rows.Count > 0 && stockDataGridView.CurrentRow != null)
            {
                // Grid içindeki satır bilgisi
                int currentRowIndex = stockDataGridView.CurrentRow.Index + 1;
                int pageRowCount = stockDataGridView.Rows.Count;

                // Pagination bilgisi
                if (pagedStockDataList != null && pagedStockDataList.Count > 0)
                {
                    // Format: "Row: 3/1000 | Page: 5/2000 | Total: 2,000,000"
                    stockDataGridViewLabel.Text = $"Row: {currentRowIndex}/{pageRowCount} | Page: {currentPage}/{totalPages} | Total: {pagedStockDataList.Count:N0}";
                }
                else
                {
                    // Pagination olmadan (eski format)
                    stockDataGridViewLabel.Text = $"Row: {currentRowIndex}/{pageRowCount}";
                }
            }
            else
            {
                if (pagedStockDataList != null && pagedStockDataList.Count > 0)
                {
                    stockDataGridViewLabel.Text = $"Row: 0/0 | Page: {currentPage}/{totalPages} | Total: {pagedStockDataList.Count:N0}";
                }
                else
                {
                    stockDataGridViewLabel.Text = "Row: 0/0";
                }
            }
        }

        private void btnUpdateFilters_Click(object sender, EventArgs e)
        {
            if (!stockMetaData.IsEmpty)
            {
                txtFilterValue1.Text = "0";
                txtFilterValue2.Text = stockMetaData.GetValueOrDefault("BarCount", "0");

                SetDateTimePicker(dtpFilterDateTime1, stockMetaData.GetValueOrDefault("Baslangic_Tarihi", ""));
                SetDateTimePicker(dtpFilterDateTime2, stockMetaData.GetValueOrDefault("Bitis_Tarihi", ""));
            }
        }

        /// <summary>
        /// DateTimePicker kontrolüne string formatındaki tarih değerini set eder
        /// Format: yyyy.MM.dd HH:mm:ss
        /// </summary>
        private void SetDateTimePicker(DateTimePicker dtp, string dateTimeStr)
        {
            if (string.IsNullOrEmpty(dateTimeStr) || dateTimeStr == "N/A")
                return;

            if (DateTime.TryParseExact(dateTimeStr, "yyyy.MM.dd HH:mm:ss",
                CultureInfo.InvariantCulture, DateTimeStyles.None, out var dateTime))
            {
                dtp.Value = dateTime;
            }
        }
    }
}
