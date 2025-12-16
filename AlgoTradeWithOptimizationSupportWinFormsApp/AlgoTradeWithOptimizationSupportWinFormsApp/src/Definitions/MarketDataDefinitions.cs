using System;
using System.IO;
using System.Runtime.InteropServices;

namespace AlgoTradeWithOptimizationSupportWinFormsApp.Definitions
{
    // ====================================================================
    // MARKET DATA STRUCTURES - Merkezi Veri Tanımları
    // ====================================================================

    /// <summary>
    /// Hisse senedi (Stock) bar verisi
    /// </summary>
    public struct StockData
    {
        // ====================================================================
        // ANA VERİLER (Raw Data)
        // ====================================================================
        public int Id { get; set; }
        public DateTime DateTime { get; set; }
        public DateTime Date { get; set; }
        public TimeSpan Time { get; set; }
        public double Open { get; set; }
        public double High { get; set; }
        public double Low { get; set; }
        public double Close { get; set; }
        public long Volume { get; set; }
        public long Size { get; set; } // Lot değeri

        // ====================================================================
        // HESAPLANMIŞ DEĞERLER (Calculated/Computed Properties)
        // ====================================================================

        /// <summary>
        /// Unix Epoch Time (saniye cinsinden)
        /// </summary>
        public readonly long EpochTime => ((DateTimeOffset)DateTime).ToUnixTimeSeconds();

        /// <summary>
        /// Fiyat farkı (Close - Open)
        /// </summary>
        public readonly double Diff => Close - Open;

        /// <summary>
        /// Yüzdelik değişim (%) - Open'a göre
        /// </summary>
        public readonly double ChangePct => Open != 0 ? ((Close - Open) / Open) * 100.0 : 0.0;

        /// <summary>
        /// Yükseliş bayrağı (Close > Open)
        /// </summary>
        public readonly bool IsBullish => Close > Open;

        /// <summary>
        /// Düşüş bayrağı (Close < Open)
        /// </summary>
        public readonly bool IsBearish => Close < Open;

        /// <summary>
        /// Nötr (Close == Open veya çok küçük değişim)
        /// Eşik değeri: %0.01
        /// </summary>
        public readonly bool IsNeutral => Math.Abs(ChangePct) < 0.01;

        /// <summary>
        /// Bar aralığı (High - Low)
        /// </summary>
        public readonly double Range => High - Low;

        /// <summary>
        /// Mum gövde boyutu (|Close - Open|)
        /// </summary>
        public readonly double BodySize => Math.Abs(Close - Open);

        /// <summary>
        /// Üst gölge/fitil uzunluğu (High - Max(Open, Close))
        /// </summary>
        public readonly double UpperShadow => High - Math.Max(Open, Close);

        /// <summary>
        /// Alt gölge/fitil uzunluğu (Min(Open, Close) - Low)
        /// </summary>
        public readonly double LowerShadow => Math.Min(Open, Close) - Low;

        /// <summary>
        /// Orta fiyat (High + Low) / 2
        /// </summary>
        public readonly double MidPrice => (High + Low) / 2.0;

        /// <summary>
        /// Tipik fiyat (High + Low + Close) / 3
        /// Teknik analizde sıkça kullanılır
        /// </summary>
        public readonly double TypicalPrice => (High + Low + Close) / 3.0;

        /// <summary>
        /// Ağırlıklı kapanış fiyatı (High + Low + Close + Close) / 4
        /// Close'a daha fazla ağırlık verir
        /// </summary>
        public readonly double WeightedClose => (High + Low + Close + Close) / 4.0;
    }

    // ====================================================================
    // BINARY SERIALIZATION - Binary Dosya Desteği
    // ====================================================================

    /// <summary>
    /// Binary serialization için optimize edilmiş StockData
    /// - Computed properties yok (sadece raw data)
    /// - StructLayout ile bellek düzeni optimize edilmiş
    /// - Binary dosyaya direkt yazılabilir/okunabilir
    /// </summary>
    [Serializable]
    [StructLayout(LayoutKind.Sequential, Pack = 1)]
    public struct StockDataBinary
    {
        public int Id;
        public long DateTimeBinary;  // DateTime.ToBinary()
        public long DateBinary;      // Date.ToBinary()
        public long TimeTicks;       // Time.Ticks
        public double Open;
        public double High;
        public double Low;
        public double Close;
        public long Volume;
        public long Size;

        /// <summary>
        /// StockData'dan StockDataBinary'ye dönüşüm
        /// </summary>
        public static StockDataBinary FromStockData(StockData data)
        {
            return new StockDataBinary
            {
                Id = data.Id,
                DateTimeBinary = data.DateTime.ToBinary(),
                DateBinary = data.Date.ToBinary(),
                TimeTicks = data.Time.Ticks,
                Open = data.Open,
                High = data.High,
                Low = data.Low,
                Close = data.Close,
                Volume = data.Volume,
                Size = data.Size
            };
        }

        /// <summary>
        /// StockDataBinary'den StockData'ya dönüşüm
        /// Computed properties otomatik hesaplanır
        /// </summary>
        public StockData ToStockData()
        {
            return new StockData
            {
                Id = Id,
                DateTime = DateTime.FromBinary(DateTimeBinary),
                Date = DateTime.FromBinary(DateBinary),
                Time = new TimeSpan(TimeTicks),
                Open = Open,
                High = High,
                Low = Low,
                Close = Close,
                Volume = Volume,
                Size = Size
                // Computed properties otomatik hesaplanır
            };
        }

        /// <summary>
        /// Struct boyutu (byte cinsinden)
        /// </summary>
        public static int SizeInBytes => Marshal.SizeOf<StockDataBinary>();
    }

    // ====================================================================
    // BINARY I/O HELPERS - Binary Okuma/Yazma Yardımcıları
    // ====================================================================

    /// <summary>
    /// StockData için binary okuma/yazma yardımcı metodları
    /// </summary>
    public static class StockDataBinaryHelper
    {
        /// <summary>
        /// Tek bir StockData'yı binary dosyaya yaz
        /// </summary>
        public static void Write(BinaryWriter writer, StockData data)
        {
            if (writer == null)
                throw new ArgumentNullException(nameof(writer));

            var binaryData = StockDataBinary.FromStockData(data);

            writer.Write(binaryData.Id);
            writer.Write(binaryData.DateTimeBinary);
            writer.Write(binaryData.DateBinary);
            writer.Write(binaryData.TimeTicks);
            writer.Write(binaryData.Open);
            writer.Write(binaryData.High);
            writer.Write(binaryData.Low);
            writer.Write(binaryData.Close);
            writer.Write(binaryData.Volume);
            writer.Write(binaryData.Size);
        }

        /// <summary>
        /// Binary dosyadan tek bir StockData oku
        /// </summary>
        public static StockData Read(BinaryReader reader)
        {
            if (reader == null)
                throw new ArgumentNullException(nameof(reader));

            var binaryData = new StockDataBinary
            {
                Id = reader.ReadInt32(),
                DateTimeBinary = reader.ReadInt64(),
                DateBinary = reader.ReadInt64(),
                TimeTicks = reader.ReadInt64(),
                Open = reader.ReadDouble(),
                High = reader.ReadDouble(),
                Low = reader.ReadDouble(),
                Close = reader.ReadDouble(),
                Volume = reader.ReadInt64(),
                Size = reader.ReadInt64()
            };

            return binaryData.ToStockData();
        }

        /// <summary>
        /// StockData listesini binary dosyaya yaz
        /// </summary>
        public static void WriteList(string filePath, List<StockData> dataList)
        {
            if (string.IsNullOrEmpty(filePath))
                throw new ArgumentException("File path cannot be null or empty", nameof(filePath));

            if (dataList == null)
                throw new ArgumentNullException(nameof(dataList));

            using var fileStream = new FileStream(filePath, FileMode.Create, FileAccess.Write, FileShare.None);
            using var writer = new BinaryWriter(fileStream);

            // Header: Kayıt sayısı
            writer.Write(dataList.Count);

            // Data
            foreach (var data in dataList)
            {
                Write(writer, data);
            }
        }

        /// <summary>
        /// Binary dosyadan StockData listesi oku
        /// </summary>
        public static List<StockData> ReadList(string filePath)
        {
            if (string.IsNullOrEmpty(filePath))
                throw new ArgumentException("File path cannot be null or empty", nameof(filePath));

            if (!File.Exists(filePath))
                throw new FileNotFoundException("File not found", filePath);

            using var fileStream = new FileStream(filePath, FileMode.Open, FileAccess.Read, FileShare.Read);
            using var reader = new BinaryReader(fileStream);

            // Header: Kayıt sayısı
            int count = reader.ReadInt32();
            var dataList = new List<StockData>(count);

            // Data
            for (int i = 0; i < count; i++)
            {
                dataList.Add(Read(reader));
            }

            return dataList;
        }

        /// <summary>
        /// Binary dosyaya append (ekleme) modu
        /// </summary>
        public static void AppendToFile(string filePath, StockData data)
        {
            if (string.IsNullOrEmpty(filePath))
                throw new ArgumentException("File path cannot be null or empty", nameof(filePath));

            bool fileExists = File.Exists(filePath);

            using var fileStream = new FileStream(filePath, FileMode.Append, FileAccess.Write, FileShare.None);
            using var writer = new BinaryWriter(fileStream);

            // İlk kayıt ise header yaz
            if (!fileExists || fileStream.Length == 0)
            {
                writer.Write(1); // Count = 1
            }

            Write(writer, data);

            // Count'u güncelle (dosya başına git)
            if (fileExists && fileStream.Length > 4)
            {
                fileStream.Seek(0, SeekOrigin.Begin);
                using var updateWriter = new BinaryWriter(fileStream);

                // Mevcut count'u oku
                using var tempReader = new BinaryReader(fileStream);
                int currentCount = tempReader.ReadInt32();

                // Count'u artır
                fileStream.Seek(0, SeekOrigin.Begin);
                updateWriter.Write(currentCount + 1);
            }
        }

        /// <summary>
        /// Binary dosya boyutunu hesapla (kayıt sayısından)
        /// </summary>
        public static long CalculateFileSize(int recordCount)
        {
            return sizeof(int) + (recordCount * StockDataBinary.SizeInBytes);
        }

        /// <summary>
        /// Binary dosyadaki kayıt sayısını oku
        /// </summary>
        public static int GetRecordCount(string filePath)
        {
            if (string.IsNullOrEmpty(filePath))
                throw new ArgumentException("File path cannot be null or empty", nameof(filePath));

            if (!File.Exists(filePath))
                return 0;

            using var fileStream = new FileStream(filePath, FileMode.Open, FileAccess.Read, FileShare.Read);
            using var reader = new BinaryReader(fileStream);

            if (fileStream.Length < sizeof(int))
                return 0;

            return reader.ReadInt32();
        }

        // ====================================================================
        // RANDOM ACCESS METHODS - Rastgele Erişim Metodları
        // ====================================================================

        /// <summary>
        /// Belirli bir index'teki kaydı oku (Random Access)
        /// Tüm dosyayı okumadan tek kayda erişim sağlar
        /// </summary>
        /// <param name="filePath">Binary dosya yolu</param>
        /// <param name="recordIndex">Kayıt index'i (0-based)</param>
        /// <returns>İstenen StockData kaydı</returns>
        public static StockData GetRecord(string filePath, int recordIndex)
        {
            if (string.IsNullOrEmpty(filePath))
                throw new ArgumentException("File path cannot be null or empty", nameof(filePath));

            if (!File.Exists(filePath))
                throw new FileNotFoundException("File not found", filePath);

            if (recordIndex < 0)
                throw new ArgumentOutOfRangeException(nameof(recordIndex), "Record index cannot be negative");

            using var fileStream = new FileStream(filePath, FileMode.Open, FileAccess.Read, FileShare.Read);
            using var reader = new BinaryReader(fileStream);

            // Header oku
            int totalRecords = reader.ReadInt32();

            if (recordIndex >= totalRecords)
                throw new ArgumentOutOfRangeException(nameof(recordIndex),
                    $"Record index {recordIndex} is out of range (0-{totalRecords - 1})");

            // Kaydın başlangıç pozisyonunu hesapla
            long offset = sizeof(int) + (recordIndex * StockDataBinary.SizeInBytes);
            fileStream.Seek(offset, SeekOrigin.Begin);

            // Kaydı oku
            return Read(reader);
        }

        /// <summary>
        /// Belirli bir aralıktaki kayıtları oku (Pagination için ideal)
        /// </summary>
        /// <param name="filePath">Binary dosya yolu</param>
        /// <param name="startIndex">Başlangıç index'i (0-based)</param>
        /// <param name="count">Okunacak kayıt sayısı</param>
        /// <returns>StockData listesi</returns>
        public static List<StockData> GetRecordRange(string filePath, int startIndex, int count)
        {
            if (string.IsNullOrEmpty(filePath))
                throw new ArgumentException("File path cannot be null or empty", nameof(filePath));

            if (!File.Exists(filePath))
                throw new FileNotFoundException("File not found", filePath);

            if (startIndex < 0)
                throw new ArgumentOutOfRangeException(nameof(startIndex), "Start index cannot be negative");

            if (count <= 0)
                throw new ArgumentOutOfRangeException(nameof(count), "Count must be greater than zero");

            using var fileStream = new FileStream(filePath, FileMode.Open, FileAccess.Read, FileShare.Read);
            using var reader = new BinaryReader(fileStream);

            // Header oku
            int totalRecords = reader.ReadInt32();

            if (startIndex >= totalRecords)
                throw new ArgumentOutOfRangeException(nameof(startIndex),
                    $"Start index {startIndex} is out of range (0-{totalRecords - 1})");

            // Gerçek okunacak kayıt sayısını hesapla
            int actualCount = Math.Min(count, totalRecords - startIndex);

            // Başlangıç pozisyonuna git
            long offset = sizeof(int) + (startIndex * StockDataBinary.SizeInBytes);
            fileStream.Seek(offset, SeekOrigin.Begin);

            // Kayıtları oku
            var dataList = new List<StockData>(actualCount);
            for (int i = 0; i < actualCount; i++)
            {
                dataList.Add(Read(reader));
            }

            return dataList;
        }

        /// <summary>
        /// Dosyanın sonundan N kayıt oku (Real-time uygulamalar için)
        /// </summary>
        /// <param name="filePath">Binary dosya yolu</param>
        /// <param name="count">Okunacak kayıt sayısı</param>
        /// <returns>Son N kayıt (StockData listesi)</returns>
        public static List<StockData> GetLastRecords(string filePath, int count)
        {
            if (string.IsNullOrEmpty(filePath))
                throw new ArgumentException("File path cannot be null or empty", nameof(filePath));

            if (!File.Exists(filePath))
                throw new FileNotFoundException("File not found", filePath);

            if (count <= 0)
                throw new ArgumentOutOfRangeException(nameof(count), "Count must be greater than zero");

            int totalRecords = GetRecordCount(filePath);

            if (totalRecords == 0)
                return new List<StockData>();

            // Başlangıç index'ini hesapla
            int startIndex = Math.Max(0, totalRecords - count);

            return GetRecordRange(filePath, startIndex, count);
        }

        /// <summary>
        /// Dosyanın başından N kayıt oku (Preview için)
        /// </summary>
        /// <param name="filePath">Binary dosya yolu</param>
        /// <param name="count">Okunacak kayıt sayısı</param>
        /// <returns>İlk N kayıt (StockData listesi)</returns>
        public static List<StockData> GetFirstRecords(string filePath, int count)
        {
            if (string.IsNullOrEmpty(filePath))
                throw new ArgumentException("File path cannot be null or empty", nameof(filePath));

            if (!File.Exists(filePath))
                throw new FileNotFoundException("File not found", filePath);

            if (count <= 0)
                throw new ArgumentOutOfRangeException(nameof(count), "Count must be greater than zero");

            return GetRecordRange(filePath, 0, count);
        }

        /// <summary>
        /// Sayfa bazlı okuma (Pagination helper)
        /// </summary>
        /// <param name="filePath">Binary dosya yolu</param>
        /// <param name="pageNumber">Sayfa numarası (1-based)</param>
        /// <param name="pageSize">Sayfa başına kayıt sayısı</param>
        /// <returns>Belirtilen sayfadaki kayıtlar</returns>
        public static List<StockData> GetPage(string filePath, int pageNumber, int pageSize)
        {
            if (pageNumber < 1)
                throw new ArgumentOutOfRangeException(nameof(pageNumber), "Page number must be 1 or greater");

            if (pageSize <= 0)
                throw new ArgumentOutOfRangeException(nameof(pageSize), "Page size must be greater than zero");

            int startIndex = (pageNumber - 1) * pageSize;
            return GetRecordRange(filePath, startIndex, pageSize);
        }

        /// <summary>
        /// Toplam sayfa sayısını hesapla
        /// </summary>
        /// <param name="filePath">Binary dosya yolu</param>
        /// <param name="pageSize">Sayfa başına kayıt sayısı</param>
        /// <returns>Toplam sayfa sayısı</returns>
        public static int GetPageCount(string filePath, int pageSize)
        {
            if (pageSize <= 0)
                throw new ArgumentOutOfRangeException(nameof(pageSize), "Page size must be greater than zero");

            int totalRecords = GetRecordCount(filePath);
            return (int)Math.Ceiling((double)totalRecords / pageSize);
        }
    }

    // ====================================================================
    // GELECEK TANIMLAR İÇİN ALAN
    // ====================================================================

    /*
    /// <summary>
    /// Forex (Döviz) bar verisi
    /// </summary>
    public struct ForexData
    {
        public int Id { get; set; }
        public DateTime DateTime { get; set; }
        public string Symbol { get; set; }
        public double Bid { get; set; }
        public double Ask { get; set; }
        public double Open { get; set; }
        public double High { get; set; }
        public double Low { get; set; }
        public double Close { get; set; }
        public long Volume { get; set; }
    }

    /// <summary>
    /// Kripto para bar verisi
    /// </summary>
    public struct CryptoData
    {
        public int Id { get; set; }
        public DateTime DateTime { get; set; }
        public string Symbol { get; set; }
        public double Open { get; set; }
        public double High { get; set; }
        public double Low { get; set; }
        public double Close { get; set; }
        public long Volume { get; set; }
        public double QuoteVolume { get; set; }
        public int TradeCount { get; set; }
    }

    /// <summary>
    /// Futures (Vadeli İşlem) bar verisi
    /// </summary>
    public struct FuturesData
    {
        public int Id { get; set; }
        public DateTime DateTime { get; set; }
        public string Symbol { get; set; }
        public double Open { get; set; }
        public double High { get; set; }
        public double Low { get; set; }
        public double Close { get; set; }
        public long Volume { get; set; }
        public long OpenInterest { get; set; }
        public DateTime ExpiryDate { get; set; }
    }

    /// <summary>
    /// Tick verisi (anlık fiyat hareketi)
    /// </summary>
    public struct TickData
    {
        public long Id { get; set; }
        public DateTime DateTime { get; set; }
        public string Symbol { get; set; }
        public double Price { get; set; }
        public long Size { get; set; }
        public TickType Type { get; set; } // Bid, Ask, Trade
    }

    public enum TickType
    {
        Bid,
        Ask,
        Trade
    }

    /// <summary>
    /// Order Book (Emir Defteri) seviyesi
    /// </summary>
    public struct OrderBookLevel
    {
        public double Price { get; set; }
        public long Size { get; set; }
        public int OrderCount { get; set; }
    }

    /// <summary>
    /// Market Depth (Piyasa Derinliği)
    /// </summary>
    public struct MarketDepth
    {
        public DateTime DateTime { get; set; }
        public string Symbol { get; set; }
        public OrderBookLevel[] Bids { get; set; }
        public OrderBookLevel[] Asks { get; set; }
    }
    */

    // ====================================================================
    // USAGE EXAMPLES - Kullanım Örnekleri
    // ====================================================================

    /*
    /// <summary>
    /// MarketDataDefinitions Usage Examples
    /// Binary ve Text dosya okuma/yazma örnekleri
    /// </summary>
    public static class MarketDataUsageExamples
    {
        // ====================================================================
        // ÖRNEK 1: Temel StockData Kullanımı
        // ====================================================================
        public static void Example1_BasicStockData()
        {
            var stockData = new StockData
            {
                Id = 1,
                DateTime = DateTime.Now,
                Date = DateTime.Today,
                Time = DateTime.Now.TimeOfDay,
                Open = 100.0,
                High = 105.5,
                Low = 98.2,
                Close = 103.0,
                Volume = 1000000,
                Size = 5000
            };

            // Computed properties otomatik hesaplanır
            Console.WriteLine($"Diff: {stockData.Diff}");              // 3.0
            Console.WriteLine($"ChangePct: {stockData.ChangePct}%");   // 3.0%
            Console.WriteLine($"IsBullish: {stockData.IsBullish}");    // true
            Console.WriteLine($"Range: {stockData.Range}");            // 7.3
            Console.WriteLine($"BodySize: {stockData.BodySize}");      // 3.0
            Console.WriteLine($"UpperShadow: {stockData.UpperShadow}"); // 2.5
            Console.WriteLine($"LowerShadow: {stockData.LowerShadow}"); // 1.8
            Console.WriteLine($"TypicalPrice: {stockData.TypicalPrice}"); // 102.23
            Console.WriteLine($"EpochTime: {stockData.EpochTime}");    // Unix timestamp
        }

        // ====================================================================
        // ÖRNEK 2: Binary Dosyaya Yazma - Tek Kayıt
        // ====================================================================
        public static void Example2_BinaryWriteSingle()
        {
            var stockData = new StockData
            {
                Id = 1,
                DateTime = new DateTime(2024, 1, 1, 9, 30, 0),
                Date = new DateTime(2024, 1, 1),
                Time = new TimeSpan(9, 30, 0),
                Open = 100.0,
                High = 105.0,
                Low = 99.0,
                Close = 103.0,
                Volume = 1000000,
                Size = 5000
            };

            using var fileStream = new FileStream("data/stock_single.bin", FileMode.Create);
            using var writer = new BinaryWriter(fileStream);

            StockDataBinaryHelper.Write(writer, stockData);

            Console.WriteLine("Single record written to binary file");
        }

        // ====================================================================
        // ÖRNEK 3: Binary Dosyadan Okuma - Tek Kayıt
        // ====================================================================
        public static void Example3_BinaryReadSingle()
        {
            using var fileStream = new FileStream("data/stock_single.bin", FileMode.Open);
            using var reader = new BinaryReader(fileStream);

            var stockData = StockDataBinaryHelper.Read(reader);

            Console.WriteLine($"Read: {stockData.DateTime}, O:{stockData.Open}, H:{stockData.High}, " +
                            $"L:{stockData.Low}, C:{stockData.Close}, V:{stockData.Volume}");
            Console.WriteLine($"Computed - ChangePct: {stockData.ChangePct}%, Bullish: {stockData.IsBullish}");
        }

        // ====================================================================
        // ÖRNEK 4: Binary Dosyaya Yazma - Liste (En Sık Kullanılan)
        // ====================================================================
        public static void Example4_BinaryWriteList()
        {
            var dataList = new List<StockData>
            {
                new StockData
                {
                    Id = 1,
                    DateTime = new DateTime(2024, 1, 1, 9, 30, 0),
                    Date = new DateTime(2024, 1, 1),
                    Time = new TimeSpan(9, 30, 0),
                    Open = 100.0, High = 105.0, Low = 99.0, Close = 103.0,
                    Volume = 1000000, Size = 5000
                },
                new StockData
                {
                    Id = 2,
                    DateTime = new DateTime(2024, 1, 1, 9, 31, 0),
                    Date = new DateTime(2024, 1, 1),
                    Time = new TimeSpan(9, 31, 0),
                    Open = 103.0, High = 107.0, Low = 102.0, Close = 105.0,
                    Volume = 1200000, Size = 6000
                },
                new StockData
                {
                    Id = 3,
                    DateTime = new DateTime(2024, 1, 1, 9, 32, 0),
                    Date = new DateTime(2024, 1, 1),
                    Time = new TimeSpan(9, 32, 0),
                    Open = 105.0, High = 106.0, Low = 103.5, Close = 104.0,
                    Volume = 900000, Size = 4500
                }
            };

            // Liste olarak kaydet
            StockDataBinaryHelper.WriteList("data/stock_list.bin", dataList);

            Console.WriteLine($"Written {dataList.Count} records to binary file");
            Console.WriteLine($"File size: {StockDataBinaryHelper.CalculateFileSize(dataList.Count)} bytes");
        }

        // ====================================================================
        // ÖRNEK 5: Binary Dosyadan Okuma - Liste (En Sık Kullanılan)
        // ====================================================================
        public static void Example5_BinaryReadList()
        {
            // Kayıt sayısını kontrol et
            int recordCount = StockDataBinaryHelper.GetRecordCount("data/stock_list.bin");
            Console.WriteLine($"Record count in file: {recordCount}");

            // Listeyi oku
            var dataList = StockDataBinaryHelper.ReadList("data/stock_list.bin");

            Console.WriteLine($"Read {dataList.Count} records from binary file");

            foreach (var data in dataList)
            {
                Console.WriteLine($"ID:{data.Id}, {data.DateTime:yyyy-MM-dd HH:mm:ss}, " +
                                $"O:{data.Open}, H:{data.High}, L:{data.Low}, C:{data.Close}, " +
                                $"V:{data.Volume}, Change:{data.ChangePct:F2}%");
            }
        }

        // ====================================================================
        // ÖRNEK 6: Binary Dosyaya Append (Ekleme)
        // ====================================================================
        public static void Example6_BinaryAppend()
        {
            var newData = new StockData
            {
                Id = 4,
                DateTime = new DateTime(2024, 1, 1, 9, 33, 0),
                Date = new DateTime(2024, 1, 1),
                Time = new TimeSpan(9, 33, 0),
                Open = 104.0, High = 108.0, Low = 103.0, Close = 106.5,
                Volume = 1100000, Size = 5500
            };

            // Mevcut dosyaya ekle
            StockDataBinaryHelper.AppendToFile("data/stock_list.bin", newData);

            Console.WriteLine("New record appended to binary file");
            Console.WriteLine($"Total records: {StockDataBinaryHelper.GetRecordCount("data/stock_list.bin")}");
        }

        // ====================================================================
        // ÖRNEK 7: Text'ten Binary'ye Dönüştürme
        // ====================================================================
        public static void Example7_TextToBinaryConversion()
        {
            // Text dosyadan oku (StockDataReader kullanarak)
            // var reader = new StockDataReader();
            // var textData = reader.ReadData("data/stock_data.txt");

            // Örnek veri (normalde StockDataReader'dan gelecek)
            var textData = new List<StockData>
            {
                new StockData { Id = 1, DateTime = DateTime.Now, Open = 100, High = 105, Low = 99, Close = 103, Volume = 1000000, Size = 5000 },
                new StockData { Id = 2, DateTime = DateTime.Now.AddMinutes(1), Open = 103, High = 107, Low = 102, Close = 105, Volume = 1200000, Size = 6000 }
            };

            // Binary dosyaya kaydet
            StockDataBinaryHelper.WriteList("data/converted_stock.bin", textData);

            Console.WriteLine($"Converted {textData.Count} records from text to binary");
            Console.WriteLine($"Binary file size: {new FileInfo("data/converted_stock.bin").Length} bytes");
        }

        // ====================================================================
        // ÖRNEK 8: Binary'den Text'e Dönüştürme
        // ====================================================================
        public static void Example8_BinaryToTextConversion()
        {
            // Binary dosyadan oku
            var binaryData = StockDataBinaryHelper.ReadList("data/stock_list.bin");

            // Text dosyaya yaz (CSV formatında)
            using var writer = new StreamWriter("data/stock_output.csv");

            // Header
            writer.WriteLine("Id;DateTime;Open;High;Low;Close;Volume;Size;Diff;ChangePct;IsBullish");

            // Data
            foreach (var data in binaryData)
            {
                writer.WriteLine($"{data.Id};{data.DateTime:yyyy.MM.dd HH:mm:ss};" +
                               $"{data.Open};{data.High};{data.Low};{data.Close};" +
                               $"{data.Volume};{data.Size};" +
                               $"{data.Diff};{data.ChangePct};{data.IsBullish}");
            }

            Console.WriteLine($"Converted {binaryData.Count} records from binary to CSV");
        }

        // ====================================================================
        // ÖRNEK 9: Performans Karşılaştırması - Binary vs Text
        // ====================================================================
        public static void Example9_PerformanceComparison()
        {
            int recordCount = 100000;
            var testData = GenerateTestData(recordCount);

            // Binary yazma
            var sw1 = System.Diagnostics.Stopwatch.StartNew();
            StockDataBinaryHelper.WriteList("data/perf_binary.bin", testData);
            sw1.Stop();

            // Binary okuma
            var sw2 = System.Diagnostics.Stopwatch.StartNew();
            var binaryData = StockDataBinaryHelper.ReadList("data/perf_binary.bin");
            sw2.Stop();

            // Text yazma (CSV)
            var sw3 = System.Diagnostics.Stopwatch.StartNew();
            using (var writer = new StreamWriter("data/perf_text.csv"))
            {
                writer.WriteLine("Id;DateTime;Open;High;Low;Close;Volume;Size");
                foreach (var data in testData)
                {
                    writer.WriteLine($"{data.Id};{data.DateTime:yyyy.MM.dd HH:mm:ss};" +
                                   $"{data.Open};{data.High};{data.Low};{data.Close};" +
                                   $"{data.Volume};{data.Size}");
                }
            }
            sw3.Stop();

            // Dosya boyutları
            var binarySize = new FileInfo("data/perf_binary.bin").Length;
            var textSize = new FileInfo("data/perf_text.csv").Length;

            Console.WriteLine("=== Performance Comparison ===");
            Console.WriteLine($"Records: {recordCount:N0}");
            Console.WriteLine();
            Console.WriteLine($"Binary Write: {sw1.ElapsedMilliseconds}ms");
            Console.WriteLine($"Binary Read:  {sw2.ElapsedMilliseconds}ms");
            Console.WriteLine($"Binary Size:  {binarySize:N0} bytes ({binarySize / 1024.0:F2} KB)");
            Console.WriteLine();
            Console.WriteLine($"Text Write:   {sw3.ElapsedMilliseconds}ms");
            Console.WriteLine($"Text Size:    {textSize:N0} bytes ({textSize / 1024.0:F2} KB)");
            Console.WriteLine();
            Console.WriteLine($"Space Saved:  {(1 - (double)binarySize / textSize) * 100:F1}%");
        }

        // ====================================================================
        // ÖRNEK 10: Filtreleme ile Binary Okuma
        // ====================================================================
        public static void Example10_BinaryReadWithFilter()
        {
            // Tüm veriyi oku
            var allData = StockDataBinaryHelper.ReadList("data/stock_list.bin");

            // Sadece yükselişleri filtrele
            var bullishData = allData.Where(d => d.IsBullish).ToList();
            Console.WriteLine($"Bullish bars: {bullishData.Count} / {allData.Count}");

            // %2'den fazla değişenleri filtrele
            var significantChanges = allData.Where(d => Math.Abs(d.ChangePct) > 2.0).ToList();
            Console.WriteLine($"Significant changes (>2%): {significantChanges.Count}");

            // Yüksek volume'lü barları filtrele
            var avgVolume = allData.Average(d => d.Volume);
            var highVolumeData = allData.Where(d => d.Volume > avgVolume).ToList();
            Console.WriteLine($"High volume bars: {highVolumeData.Count}");

            // Tarih aralığı filtresi
            var startDate = new DateTime(2024, 1, 1, 9, 30, 0);
            var endDate = new DateTime(2024, 1, 1, 10, 0, 0);
            var dateRangeData = allData.Where(d => d.DateTime >= startDate && d.DateTime <= endDate).ToList();
            Console.WriteLine($"Date range bars: {dateRangeData.Count}");
        }

        // ====================================================================
        // ÖRNEK 11: Memory Mapped File ile Hızlı Erişim
        // ====================================================================
        public static void Example11_MemoryMappedFileAccess()
        {
            // Not: Bu örnek büyük dosyalar için daha verimlidir
            // Binary dosyayı memory-mapped olarak aç

            using var mmf = System.IO.MemoryMappedFiles.MemoryMappedFile.CreateFromFile(
                "data/stock_list.bin",
                FileMode.Open,
                "StockDataMap");

            using var accessor = mmf.CreateViewAccessor();

            // Header oku (ilk 4 byte = record count)
            int recordCount = accessor.ReadInt32(0);
            Console.WriteLine($"Record count (via MMF): {recordCount}");

            // İlk kaydı oku
            int offset = sizeof(int); // Header'ı atla
            var binaryData = new StockDataBinary
            {
                Id = accessor.ReadInt32(offset),
                DateTimeBinary = accessor.ReadInt64(offset + 4),
                DateBinary = accessor.ReadInt64(offset + 12),
                TimeTicks = accessor.ReadInt64(offset + 20),
                Open = accessor.ReadDouble(offset + 28),
                High = accessor.ReadDouble(offset + 36),
                Low = accessor.ReadDouble(offset + 44),
                Close = accessor.ReadDouble(offset + 52),
                Volume = accessor.ReadInt64(offset + 60),
                Size = accessor.ReadInt64(offset + 68)
            };

            var stockData = binaryData.ToStockData();
            Console.WriteLine($"First record: {stockData.DateTime}, O:{stockData.Open}, C:{stockData.Close}");
        }

        // ====================================================================
        // ÖRNEK 12: Random Access - Tek Kayıt Okuma (YENİ)
        // ====================================================================
        public static void Example12_RandomAccessSingleRecord()
        {
            string filePath = "data/stock_list.bin";

            // Toplam kayıt sayısı
            int totalRecords = StockDataBinaryHelper.GetRecordCount(filePath);
            Console.WriteLine($"Total records: {totalRecords}");

            // 100. kaydı oku (tüm dosyayı okumadan!)
            var record100 = StockDataBinaryHelper.GetRecord(filePath, 99); // 0-based index
            Console.WriteLine($"\nRecord 100: {record100.DateTime}, Close: {record100.Close}");

            // 5000. kaydı oku
            var record5000 = StockDataBinaryHelper.GetRecord(filePath, 4999);
            Console.WriteLine($"Record 5000: {record5000.DateTime}, Close: {record5000.Close}");

            // Son kaydı oku
            var lastRecord = StockDataBinaryHelper.GetRecord(filePath, totalRecords - 1);
            Console.WriteLine($"Last record: {lastRecord.DateTime}, Close: {lastRecord.Close}");

            // Avantaj: Tüm dosyayı okumadık, sadece istediğimiz kayıtları aldık!
        }

        // ====================================================================
        // ÖRNEK 13: Range Access - Aralık Okuma (YENİ)
        // ====================================================================
        public static void Example13_RangeAccess()
        {
            string filePath = "data/stock_list.bin";

            // 1000-1100 arası kayıtları oku (100 kayıt)
            var range = StockDataBinaryHelper.GetRecordRange(filePath, startIndex: 1000, count: 100);
            Console.WriteLine($"Read {range.Count} records from index 1000-1099");

            // Ortalama hesapla
            var avgClose = range.Average(r => r.Close);
            var avgChange = range.Average(r => r.ChangePct);

            Console.WriteLine($"Average Close: {avgClose:F2}");
            Console.WriteLine($"Average Change: {avgChange:F2}%");
            Console.WriteLine($"Bullish: {range.Count(r => r.IsBullish)}");
        }

        // ====================================================================
        // ÖRNEK 14: Son N Kayıt - Real-time Uygulamalar (YENİ)
        // ====================================================================
        public static void Example14_LastRecords()
        {
            string filePath = "data/stock_list.bin";

            // Son 50 kaydı oku (güncel veriler için)
            var last50 = StockDataBinaryHelper.GetLastRecords(filePath, 50);
            Console.WriteLine($"Last 50 records:");

            foreach (var record in last50)
            {
                Console.WriteLine($"{record.DateTime:HH:mm:ss} - Close: {record.Close}, " +
                                $"Change: {record.ChangePct:F2}%");
            }

            // Son kayıtta yükseliş var mı?
            var latest = last50.Last();
            Console.WriteLine($"\nLatest: {(latest.IsBullish ? "YUKSELIŞ" : "DÜŞÜŞ")}");
        }

        // ====================================================================
        // ÖRNEK 15: İlk N Kayıt - Preview (YENİ)
        // ====================================================================
        public static void Example15_FirstRecords()
        {
            string filePath = "data/stock_list.bin";

            // İlk 10 kaydı oku (dosya önizlemesi)
            var first10 = StockDataBinaryHelper.GetFirstRecords(filePath, 10);

            Console.WriteLine("File Preview - First 10 records:");
            Console.WriteLine("=" + new string('=', 80));

            foreach (var record in first10)
            {
                Console.WriteLine($"ID:{record.Id,5} | {record.DateTime:yyyy-MM-dd HH:mm:ss} | " +
                                $"O:{record.Open,7:F2} H:{record.High,7:F2} " +
                                $"L:{record.Low,7:F2} C:{record.Close,7:F2} | " +
                                $"V:{record.Volume,10} | {(record.IsBullish ? "↑" : "↓")}");
            }
        }

        // ====================================================================
        // ÖRNEK 16: Pagination - Sayfa Bazlı Okuma (YENİ)
        // ====================================================================
        public static void Example16_Pagination()
        {
            string filePath = "data/stock_list.bin";
            int pageSize = 100; // Sayfa başına 100 kayıt

            // Toplam sayfa sayısı
            int totalPages = StockDataBinaryHelper.GetPageCount(filePath, pageSize);
            Console.WriteLine($"Total pages: {totalPages} (Page size: {pageSize})");

            // 5. sayfayı oku
            int pageNumber = 5;
            var page5 = StockDataBinaryHelper.GetPage(filePath, pageNumber, pageSize);

            Console.WriteLine($"\nPage {pageNumber}:");
            Console.WriteLine($"Records: {page5.Count}");
            Console.WriteLine($"First: {page5.First().DateTime}");
            Console.WriteLine($"Last: {page5.Last().DateTime}");

            // UI'da kullanım örneği
            Console.WriteLine("\n--- UI Pagination Example ---");
            for (int page = 1; page <= Math.Min(3, totalPages); page++)
            {
                var pageData = StockDataBinaryHelper.GetPage(filePath, page, pageSize);
                Console.WriteLine($"Page {page}: {pageData.Count} records, " +
                                $"Range: {pageData.First().Id} - {pageData.Last().Id}");
            }
        }

        // ====================================================================
        // ÖRNEK 17: DataGrid/ListView Pagination (YENİ)
        // ====================================================================
        public static void Example17_DataGridPagination()
        {
            string filePath = "data/stock_list.bin";
            int pageSize = 20; // DataGrid'de 20 satır göster

            int totalRecords = StockDataBinaryHelper.GetRecordCount(filePath);
            int totalPages = StockDataBinaryHelper.GetPageCount(filePath, pageSize);

            Console.WriteLine("=== DataGrid Pagination Simulation ===");
            Console.WriteLine($"Total Records: {totalRecords}");
            Console.WriteLine($"Page Size: {pageSize}");
            Console.WriteLine($"Total Pages: {totalPages}");

            // Kullanıcı sayfa 3'e gitti
            int currentPage = 3;
            var currentPageData = StockDataBinaryHelper.GetPage(filePath, currentPage, pageSize);

            Console.WriteLine($"\n--- Page {currentPage} of {totalPages} ---");
            Console.WriteLine($"{"ID",-6} {"DateTime",-20} {"Open",-10} {"High",-10} {"Low",-10} {"Close",-10} {"Change%",-10}");
            Console.WriteLine(new string('-', 86));

            foreach (var record in currentPageData)
            {
                Console.WriteLine($"{record.Id,-6} {record.DateTime,-20:yyyy-MM-dd HH:mm:ss} " +
                                $"{record.Open,-10:F2} {record.High,-10:F2} {record.Low,-10:F2} " +
                                $"{record.Close,-10:F2} {record.ChangePct,-10:F2}");
            }

            Console.WriteLine($"\n[Prev] Page {currentPage} of {totalPages} [Next]");
        }

        // ====================================================================
        // ÖRNEK 18: Performans Karşılaştırması - ReadList vs GetRecordRange (YENİ)
        // ====================================================================
        public static void Example18_PerformanceComparison_RandomAccess()
        {
            string filePath = "data/perf_binary.bin";
            int targetRecordCount = 1000; // Sadece 1000 kayıt lazım

            Console.WriteLine("=== Performance Comparison: Full Read vs Range Read ===\n");

            // Yöntem 1: Tüm dosyayı oku, sonra filtrele
            var sw1 = System.Diagnostics.Stopwatch.StartNew();
            var allData = StockDataBinaryHelper.ReadList(filePath);
            var filtered1 = allData.Take(targetRecordCount).ToList();
            sw1.Stop();

            // Yöntem 2: Sadece ihtiyacımız olanı oku (GetRecordRange)
            var sw2 = System.Diagnostics.Stopwatch.StartNew();
            var filtered2 = StockDataBinaryHelper.GetRecordRange(filePath, 0, targetRecordCount);
            sw2.Stop();

            // Yöntem 3: Tek tek GetRecord (en yavaş, gösterim amaçlı)
            var sw3 = System.Diagnostics.Stopwatch.StartNew();
            var filtered3 = new List<StockData>();
            for (int i = 0; i < targetRecordCount; i++)
            {
                filtered3.Add(StockDataBinaryHelper.GetRecord(filePath, i));
            }
            sw3.Stop();

            Console.WriteLine($"Total Records in File: {allData.Count}");
            Console.WriteLine($"Target Records: {targetRecordCount}");
            Console.WriteLine();
            Console.WriteLine($"Method 1 (ReadList + Take):  {sw1.ElapsedMilliseconds}ms");
            Console.WriteLine($"Method 2 (GetRecordRange):   {sw2.ElapsedMilliseconds}ms ⭐ FASTEST");
            Console.WriteLine($"Method 3 (Loop GetRecord):   {sw3.ElapsedMilliseconds}ms");
            Console.WriteLine();
            Console.WriteLine($"Speed Improvement: {(double)sw1.ElapsedMilliseconds / sw2.ElapsedMilliseconds:F1}x faster");
        }

        // ====================================================================
        // ÖRNEK 19: Real-time Chart Update Simulation (YENİ)
        // ====================================================================
        public static void Example19_RealTimeChartUpdate()
        {
            string filePath = "data/stock_list.bin";
            int chartBarCount = 100; // Chart'ta 100 bar göster

            Console.WriteLine("=== Real-time Chart Update Simulation ===\n");

            // İlk yükleme: Son 100 barı al
            var chartData = StockDataBinaryHelper.GetLastRecords(filePath, chartBarCount);
            Console.WriteLine($"Initial chart load: {chartData.Count} bars");
            Console.WriteLine($"Chart range: {chartData.First().DateTime} - {chartData.Last().DateTime}");

            // Yeni veri geldi simülasyonu
            Console.WriteLine("\n--- New data arrives ---");

            // Son kaydı al
            int totalRecords = StockDataBinaryHelper.GetRecordCount(filePath);
            var latestBar = StockDataBinaryHelper.GetRecord(filePath, totalRecords - 1);

            Console.WriteLine($"Latest bar: {latestBar.DateTime}, Close: {latestBar.Close}, " +
                            $"Change: {latestBar.ChangePct:F2}% {(latestBar.IsBullish ? "↑" : "↓")}");

            // Chart'ı güncelle: İlk barı çıkar, yeni barı ekle
            chartData.RemoveAt(0);
            chartData.Add(latestBar);

            Console.WriteLine($"Chart updated: {chartData.Count} bars");
            Console.WriteLine($"New range: {chartData.First().DateTime} - {chartData.Last().DateTime}");
        }

        // ====================================================================
        // Helper Methods
        // ====================================================================

        private static List<StockData> GenerateTestData(int count)
        {
            var random = new Random();
            var dataList = new List<StockData>(count);
            var baseDate = new DateTime(2024, 1, 1, 9, 30, 0);
            double price = 100.0;

            for (int i = 0; i < count; i++)
            {
                var open = price;
                var change = (random.NextDouble() - 0.5) * 5.0; // -2.5 to +2.5
                var close = open + change;
                var high = Math.Max(open, close) + random.NextDouble() * 2.0;
                var low = Math.Min(open, close) - random.NextDouble() * 2.0;

                dataList.Add(new StockData
                {
                    Id = i + 1,
                    DateTime = baseDate.AddMinutes(i),
                    Date = baseDate.Date,
                    Time = baseDate.AddMinutes(i).TimeOfDay,
                    Open = open,
                    High = high,
                    Low = low,
                    Close = close,
                    Volume = random.Next(500000, 2000000),
                    Size = random.Next(2000, 10000)
                });

                price = close; // Next bar başlangıcı
            }

            return dataList;
        }
    }
    */
}
