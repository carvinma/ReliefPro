using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ReliefProModel;
using System.Collections.ObjectModel;

namespace ReliefProMain.Models.Compressors
{
    public class CompressorModel: ModelBase
    {
        public Compressor dbmodel { get; set; }
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
                OnPropertyChanged("CompressorType");
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
            this.CompressorType = CompressorTypes[0];
            this.Feeds = Feeds;
            this.Products = Products;
            this.CompressorName = compressor.CompressorName;
            this.CompressorType = compressor.CompressorType;
        }


        private ObservableCollection<string> GetCompressorTypes()
        {
            ObservableCollection<string> list = new ObservableCollection<string>();
            list.Add("Centrifugal");
            list.Add("Piston");
            return list;
        }

    }
}
