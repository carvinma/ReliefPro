using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ReliefProModel;
using System.Collections.ObjectModel;
using ReliefProCommon.Enum;

namespace ReliefProMain.Models.Compressors
{
    public class CompressorModel: ModelBase
    {
        public Compressor dbmodel { get; set; }
        private ObservableCollection<string> _Drivers;
        public ObservableCollection<string> Drivers
        {
            get
            {
                return this._Drivers;
            }
            set
            {
                this._Drivers = value;
                OnPropertyChanged("Drivers");
            }
        }
        private ObservableCollection<string> _CompressorTypes;
        public ObservableCollection<string> CompressorTypes
        {
            get
            {
                return this._CompressorTypes;
            }
            set
            {
                this._CompressorTypes = value;
                OnPropertyChanged("CompressorTypes");
            }
        }
        private string _Driver;
        public string Driver
        {
            get
            {
                return this._Driver;
            }
            set
            {
                this._Driver = value;
                dbmodel.Driver = value;
                this.Driver_Color = ColorBorder.blue.ToString();
                
                OnPropertyChanged("Driver");
            }
        }

        private string _CompressorName;
        public string CompressorName
        {
            get
            {
                return this._CompressorName;
            }
            set
            {
                this._CompressorName = value;
                dbmodel.CompressorName = value;
                OnPropertyChanged("CompressorName");
            }
        }
        private string _SourceFile;
        public string SourceFile
        {
            get
            {
                return this._SourceFile;
            }
            set
            {
                this._SourceFile = value;
                dbmodel.SourceFile = value;
                OnPropertyChanged("SourceFile");
            }
        }
        private string _CompressorType;
        public string CompressorType
        {
            get
            {
                return this._CompressorType;
            }
            set
            {
                this._CompressorType = value;
                dbmodel.CompressorType = value;
                this.CompressorType_Color = ColorBorder.blue.ToString();
                OnPropertyChanged("CompressorType");
            }
        }

        private string _CompressorType_Color;
        public string CompressorType_Color
        {
            get
            {
                return this._CompressorType_Color;
            }
            set
            {
                this._CompressorType_Color = value;
                dbmodel.CompressorType_Color = value;

                OnPropertyChanged("CompressorType_Color");
            }
        }

        private string _Driver_Color;
        public string Driver_Color
        {
            get
            {
                return this._Driver_Color;
            }
            set
            {
                this._Driver_Color = value;
                dbmodel.CompressorType_Color = value;

                OnPropertyChanged("Driver_Color");
            }
        }


        private ObservableCollection<CustomStream> _Feeds;
        public ObservableCollection<CustomStream> Feeds
        {
            get { return _Feeds; }
            set
            {
                _Feeds = value;

                OnPropertyChanged("Feeds");
            }
        }
        private ObservableCollection<CustomStream> _Products;
        public ObservableCollection<CustomStream> Products
        {
            get { return _Products; }
            set
            {
                _Products = value;
                OnPropertyChanged("Products");
            }
        }


        public CompressorModel(Compressor compressor, ObservableCollection<CustomStream> Feeds, ObservableCollection<CustomStream> Products)
        {
            this.dbmodel = compressor;
            this.CompressorName = compressor.CompressorName;
            this.CompressorTypes = GetCompressorTypes();
            this.Drivers = GetDrivers();
            this.CompressorType = compressor.CompressorType;
            this.CompressorType_Color = compressor.CompressorType_Color;
            this.Feeds = Feeds;
            this.Products = Products;
            this.CompressorName = compressor.CompressorName;
            this.CompressorType = compressor.CompressorType;
            this.Driver = compressor.Driver;
        }


        private ObservableCollection<string> GetCompressorTypes()
        {
            ObservableCollection<string> list = new ObservableCollection<string>();
            list.Add("Centrifugal");
            list.Add("Piston");
            return list;
        }

        private ObservableCollection<string> GetDrivers()
        {
            ObservableCollection<string> list = new ObservableCollection<string>();
            list.Add("Turbine");
            list.Add("Non-Turbine");
            return list;
        }

    }
}
