namespace AdMaiora.Sweety
{
    using System;
    using System.Collections.Generic;
    using System.Threading.Tasks;
    using System.Threading;
    using System.Linq;

    using RestSharp.Portable;

    using AdMaiora.AppKit.Utils;
    using AdMaiora.AppKit.Data;
    using AdMaiora.AppKit.Services;
    using AdMaiora.AppKit.IO;
    
    public class User
    {
        public string FullName
        {
            get;
            set;
        }

        public Card Card
        {
            get;
            set;
        }
    }

    public class Card
    {
        public decimal Value
        {
            get;
            set;
        }

        public decimal Credit
        {
            get;
            set;
        }

        public decimal Change
        {
            get;
            set;
        }

        public Marker[] Markers
        {
            get;
            set;
        }

        public Transaction[] Transactions
        {
            get;
            set;
        }
    }

    public class Transaction
    {
        public int TransactionId
        {
            get;
            set;
        }

        public decimal Value
        {
            get;
            set;
        }

        public DateTime? ConsumptionDate
        {
            get;
            set;
        }

        public string MarkerColor
        {
            get;
            set;
        }
    }

    public class Marker
    {
        #region Properties

        public int TransactionId
        {
            get;
            set;
        }

        public decimal Value
        {
            get;
            set;
        }

        public bool IsUsed
        {
            get
            {
                return this.Value != 0;
            }
        }

        public bool IsConsumed
        {
            get
            {
                return this.Value == AppController.Globals.MarkerValue;
            }
        }

        public bool IsPartial
        {
            get
            {
                return this.Value > 0 && this.Value < AppController.Globals.MarkerValue;
            }
        }

        public bool IsTransacted
        {
            get;
            set;
        }

        public bool IsChange
        {
            get;
            set;
        }

        public string TransactionColor
        {
            get;
            set;
        }

        #endregion
    }

    public class PushEventArgs : EventArgs
    {
        public int Action
        {
            get;
            private set;
        }

        public string Payload
        {
            get;
            private set;
        }

        public Exception Error
        {
            get;
            private set;
        }

        public PushEventArgs(int action, string payload)
        {
            this.Action = action;
            this.Payload = payload;
        }

        public PushEventArgs(Exception error)
        {
            this.Error = error;
        }
    }

    public class TextInputDoneEventArgs : EventArgs
    {
        public TextInputDoneEventArgs(string text)
        {
            this.Text = text;
        }

        public string Text
        {
            get;
            private set;
        }
    }

    public class AppSettings
    {
        #region Constants and Fields

        private UserSettings _settings;

        #endregion

        #region Constructors

        public AppSettings(UserSettings settings)
        {
            _settings = settings;
        }

        #endregion

        #region Properties
        #endregion
    }

    public static class AppController
    {
        #region Inner Classes
        #endregion

        #region Constants and Fields
        public static class Globals
        {
            // Splash screen timeout (milliseconds)
            public const int SplashScreenTimeout = 2000;

            // Data storage file uri
            public const string DatabaseFilePath = "internal://database.db3";

            // Image storage folder
            public const string ImageCacheFolderPath = "external://images";

            // Base URL for service client endpoints
            public const string ServicesBaseUrl = "https://sweety-api.azurewebsites.net/";
            // Default service client timeout in seconds
            public const int ServicesDefaultRequestTimeout = 60;

            public const decimal CardValue = 15M;
            public const decimal MarkerValue = 1M;
        }

        public static class Colors
        {
            public const string Glitter = "FE4A49";
            public const string PastelGray = "FDE74C";
            public const string QuickSilver = "3A3335";
            public const string CharlestoneGreen = "FDF0D5";
            public const string BlueLagoon = "9BC53D";
            public const string Black = "000000";
            public const string White = "FFFFFF";
        }

        private static AppSettings _settings;

        private static Executor _utility;
        private static FileSystem _filesystem;
        private static ImageLoader _imageLoader;
        private static DataStorage _database;

        #endregion

        #region Properties

        public static AppSettings Settings
        {
            get
            {
                return _settings;
            }
        }

        public static Executor Utility
        {
            get
            {
                return _utility;
            }
        }

        public static ImageLoader Images
        {
            get
            {
                return _imageLoader;
            }
        }

        #endregion

        #region Initialization Methods

        public static void EnableSettings(IUserSettingsPlatform userSettingsPlatform)
        {
            _settings = new AppSettings(new UserSettings(userSettingsPlatform));
        }

        public static void EnableUtilities(IExecutorPlatform utilityPlatform)
        {
            _utility = new Executor(utilityPlatform);
        }

        public static void EnableFileSystem(IFileSystemPlatform fileSystemPlatform)
        {
            _filesystem = new FileSystem(fileSystemPlatform);
        }

        public static void EnableImageLoader(IImageLoaderPlatform imageLoaderPlatform)
        {
            FolderUri storageUri = _filesystem.CreateFolderUri(AppController.Globals.ImageCacheFolderPath);
            _imageLoader = new ImageLoader(imageLoaderPlatform, 12);
            _imageLoader.StorageUri = storageUri;
        }

        public static void EnableDataStorage(IDataStoragePlatform sqlitePlatform)
        {
            FileUri storageUri = _filesystem.CreateFileUri(AppController.Globals.DatabaseFilePath);
            _database = new DataStorage(sqlitePlatform, storageUri);
        }

        #endregion

        #region Helper Methods

        #endregion
    }
}
