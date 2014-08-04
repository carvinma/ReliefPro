using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Text;
using System.Windows.Input;
using System.Linq;
using System.Windows;
using Microsoft.Practices.Prism.Commands;
using NHibernate;
using ReliefProLL;
using ReliefProMain.Model.ReactorLoops;
using ReliefProModel.ReactorLoops;
using ReliefProDAL;
using ReliefProModel;
using ReliefProMain.View;
using System.IO;
using System.Windows.Controls;

namespace ReliefProMain.ViewModel.ReactorLoops
{
    public class ReactorLoopVM
    {
        public ICommand OKCMD { get; set; }
        public ICommand SimulationCMD { get; set; }
        public ICommand ImportCMD { get; set; }
        public ICommand ProcessHXAddCMD { get; set; }
        public ICommand ProcessHXDelCMD { get; set; }

        public ICommand UtilityHXAddCMD { get; set; }
        public ICommand UtilityHXDelCMD { get; set; }

        public ICommand MixerSplitterAddCMD { get; set; }
        public ICommand MixerSplitterDelCMD { get; set; }

        private ISession SessionPS;
        private ISession SessionPF;
        public string DirPlant { set; get; }
        public string DirProtectedSystem { set; get; }
        public string PrzFile { set; get; }
        public string PrzVersion { set; get; }

        private int reactorLoopID;
        public ReactorLoopModel model { get; set; }
        private ReactorLoopBLL reactorBLL;


        public List<string> streams = new List<string>();

        private void InitCMD()
        {
            OKCMD = new DelegateCommand<object>(Save);
            SimulationCMD = new DelegateCommand<object>(Simulation);
            ImportCMD = new DelegateCommand<object>(Import);
            ProcessHXAddCMD = new DelegateCommand<object>(ProcessHXAdd);
            ProcessHXDelCMD = new DelegateCommand<object>(ProcessHXDel);

            UtilityHXAddCMD = new DelegateCommand<object>(UtilityHXAdd);
            UtilityHXDelCMD = new DelegateCommand<object>(UtilityHXDel);

            MixerSplitterAddCMD = new DelegateCommand<object>(MixerSplitterAdd);
            MixerSplitterDelCMD = new DelegateCommand<object>(MixerSplitterDel);

        }
        private void InitPage()
        {
            if (!string.IsNullOrEmpty(PrzFile))
            {
                ObservableCollection<string> list = GetProIIStreamNames();

                model.ColdHighPressureSeparatorSource = GetProIIFlashs();
                model.HotHighPressureSeparatorSource = GetProIIFlashs();
                model.EffluentStreamSource = list;
                model.ColdReactorFeedStreamSource = list;
                model.HXNetworkColdStreamSource = list;
                model.InjectionWaterStreamSource = list;
                model.ObcProcessHXSource = GetProIIHXs(0);
                model.ObcUtilityHXSource = GetProIIHXs(1);
                model.ObcMixerSplitterSource = GetProIIMixers();

                model.ObcProcessHX = new ObservableCollection<ReactorLoopDetail>();
                model.ObcUtilityHX = new ObservableCollection<ReactorLoopDetail>();
                model.ObcMixerSplitter = new ObservableCollection<ReactorLoopDetail>();


            }
        }
        private ObservableCollection<string> GetProIIStreamNames()
        {
            ObservableCollection<string> rlt = new ObservableCollection<string>();
            ProIIStreamDataDAL dal = new ProIIStreamDataDAL();
            IList<ProIIStreamData> list = dal.GetAllList(SessionPF, PrzFile);
            foreach (ProIIStreamData s in list)
            {
                rlt.Add(s.StreamName);
            }
            return rlt;
        }
        private ObservableCollection<string> GetProIIFlashs()
        {
            ObservableCollection<string> rlt = new ObservableCollection<string>();
            ProIIEqDataDAL dal = new ProIIEqDataDAL();
            IList<ProIIEqData> list = dal.GetAllList(SessionPF, PrzFile, "Flash");
            foreach (ProIIEqData eq in list)
            {
                rlt.Add(eq.EqName);
            }
            return rlt;
        }
        private ObservableCollection<ReactorLoopDetail> GetProIIHXs(int reactorType)
        {
            ObservableCollection<ReactorLoopDetail> rlt = new ObservableCollection<ReactorLoopDetail>();
            ProIIEqDataDAL dal = new ProIIEqDataDAL();
            IList<ProIIEqData> list = dal.GetAllList(SessionPF, PrzFile, "Hx");
            foreach (ProIIEqData eq in list)
            {
                ReactorLoopDetail d = new ReactorLoopDetail();
                d.DetailInfo = eq.EqName;
                d.ReactorType = reactorType;
                rlt.Add(d);
            }
            return rlt;
        }
        private ObservableCollection<ReactorLoopDetail> GetProIIMixers()
        {
            ObservableCollection<ReactorLoopDetail> rlt = new ObservableCollection<ReactorLoopDetail>();
            ProIIEqDataDAL dal = new ProIIEqDataDAL();
            IList<ProIIEqData> list = dal.GetAllList(SessionPF, PrzFile, "Mixer");
            foreach (ProIIEqData eq in list)
            {
                ReactorLoopDetail d = new ReactorLoopDetail();
                d.DetailInfo = eq.EqName;
                d.ReactorType = 2;
                rlt.Add(d);
            }
            IList<ProIIEqData> list2 = dal.GetAllList(SessionPF, PrzFile, "Spliter");
            foreach (ProIIEqData eq in list2)
            {
                ReactorLoopDetail d = new ReactorLoopDetail();
                d.DetailInfo = eq.EqName;
                d.ReactorType = 2;
                rlt.Add(d);
            }
            return rlt;
        }

        public ReactorLoopVM(string przFile, string przVersion, ISession SessionPF, ISession SessionPS, string dirPlant, string dirProtectedSystem)
        {
            model = new ReactorLoopModel();
            this.SessionPS = SessionPS;
            this.SessionPF = SessionPF;
            DirPlant = dirPlant;
            DirProtectedSystem = dirProtectedSystem;
            PrzFile = przFile;
            PrzVersion = przVersion;
            InitCMD();
            InitPage();

            reactorBLL = new ReactorLoopBLL(SessionPS, SessionPF);
            var RLModel = reactorBLL.GetReactorLoopModel();

            model.dbModel = RLModel;
            if (RLModel.ID > 0)
            {
                reactorLoopID = RLModel.ID;
                model.ObcProcessHX = reactorBLL.GetProcessHX(reactorLoopID);
                model.ObcUtilityHX = reactorBLL.GetUtilityHX(reactorLoopID);
                model.ObcMixerSplitter = reactorBLL.GetMixerSplitter(reactorLoopID);
            }
            else
            {

            }
        }
        private void ProcessHXAdd(object obj)
        {
            if (model.SelectedHXModel != null)
            {
                model.ObcProcessHX.Add(model.SelectedHXModel);
                var find = model.ObcProcessHXSource.FirstOrDefault(p => p.DetailInfo == model.SelectedHXModel.DetailInfo && p.ReactorType == 0);
                model.ObcProcessHXSource.Remove(find);
            }
        }
        private void ProcessHXDel(object obj)
        {
            if (model.SelectedHXModel != null)
            {
                model.ObcProcessHXSource.Add(model.SelectedHXModel);
                var find = model.ObcProcessHX.FirstOrDefault(p => p.DetailInfo == model.SelectedHXModel.DetailInfo && p.ReactorType == 0);
                model.ObcProcessHX.Remove(find);
            }
        }
        private void UtilityHXAdd(object obj)
        {
            if (model.SelectedUtilityHXModel != null)
            {
                model.ObcUtilityHX.Add(model.SelectedUtilityHXModel);
                var find = model.ObcUtilityHXSource.FirstOrDefault(p => p.DetailInfo == model.SelectedUtilityHXModel.DetailInfo && p.ReactorType == 1);
                model.ObcUtilityHXSource.Remove(find);
            }
        }
        private void UtilityHXDel(object obj)
        {
            if (model.SelectedUtilityHXModel != null)
            {
                model.ObcUtilityHXSource.Add(model.SelectedUtilityHXModel);
                var find = model.ObcUtilityHX.FirstOrDefault(p => p.DetailInfo == model.SelectedUtilityHXModel.DetailInfo && p.ReactorType == 1);
                model.ObcUtilityHX.Remove(find);
            }
        }
        private void MixerSplitterAdd(object obj)
        {
            if (model.SelectedMixerModel != null)
            {
                model.ObcMixerSplitter.Add(model.SelectedMixerModel);
                var find = model.ObcMixerSplitterSource.FirstOrDefault(p => p.DetailInfo == model.SelectedMixerModel.DetailInfo && p.ReactorType == 2);
                model.ObcMixerSplitterSource.Remove(find);
            }
        }
        private void MixerSplitterDel(object obj)
        {
            if (model.SelectedMixerModel != null)
            {
                model.ObcMixerSplitterSource.Add(model.SelectedMixerModel);
                var find = model.ObcMixerSplitter.FirstOrDefault(p => p.DetailInfo == model.SelectedMixerModel.DetailInfo && p.ReactorType == 2);
                model.ObcMixerSplitter.Remove(find);
            }
        }
        private void Import(object obj)
        {
            SelectPathView view = new SelectPathView();
            SelectPathVM vm = new SelectPathVM(DirPlant);
            view.DataContext = vm;
            view.WindowStartupLocation = WindowStartupLocation.CenterScreen;
            if (view.ShowDialog() == true)
            {
                PrzFile = vm.SelectedFile + ".prz";
                InitPage();
            }
        }
        private void Simulation(object obj)
        {
            streams.Clear();
            List<CustomStream> streamList = new List<CustomStream>();
            List<string> eqList = new List<string>();
            foreach (ReactorLoopDetail d in model.ObcProcessHX)
            {
                eqList.Add(d.DetailInfo);
            }
            foreach (ReactorLoopDetail d in model.ObcUtilityHX)
            {
                eqList.Add(d.DetailInfo);
            }
            foreach (ReactorLoopDetail d in model.ObcMixerSplitter)
            {
                eqList.Add(d.DetailInfo);
            }
            if (!string.IsNullOrEmpty(model.ColdHighPressureSeparator))
            {
                eqList.Add(model.ColdHighPressureSeparator);
            }
            if (!string.IsNullOrEmpty(model.HotHighPressureSeparator))
            {
                eqList.Add(model.HotHighPressureSeparator);
            }
            streamList = GetReactorLoopStreamsFromProII(eqList);
            if (!string.IsNullOrEmpty(model.ColdReactorFeedStream) && !streams.Contains(model.ColdReactorFeedStream))
            {
                CustomStream cs = GetReactorLoopStreamInfoFromProII(model.ColdReactorFeedStream);
                streamList.Add(cs);
                streams.Add(model.ColdReactorFeedStream);
            }
            if (!string.IsNullOrEmpty(model.HXNetworkColdStream) && !streams.Contains(model.HXNetworkColdStream))
            {
                CustomStream cs = GetReactorLoopStreamInfoFromProII(model.HXNetworkColdStream);
                streamList.Add(cs);
                streams.Add(model.HXNetworkColdStream);
            }
            if (!string.IsNullOrEmpty(model.InjectionWaterStream) && !streams.Contains(model.InjectionWaterStream))
            {
                CustomStream cs = GetReactorLoopStreamInfoFromProII(model.InjectionWaterStream);
                streamList.Add(cs);
                streams.Add(model.InjectionWaterStream);
            }
            if (!string.IsNullOrEmpty(model.EffluentStream) && !streams.Contains(model.EffluentStream))
            {
                CustomStream cs = GetReactorLoopStreamInfoFromProII(model.EffluentStream);
                streamList.Add(cs);
                streams.Add(model.EffluentStream);
            }




            string inpData = CreateReactorLoopInpData(DirPlant, streamList, eqList);



        }
        private void Save(object obj)
        {
            if (obj != null)
            {
                System.Windows.Window wd = obj as System.Windows.Window;
                if (wd != null)
                {
                    var allSelectedInfo = new ObservableCollection<ReactorLoopDetail>();
                    if (model.ObcProcessHX != null)
                    {
                        foreach (var hx in model.ObcProcessHX)
                        {
                            hx.ID = 0;
                            allSelectedInfo.Add(hx);
                        }
                    }
                    if (model.ObcUtilityHX != null)
                    {
                        foreach (var hx in model.ObcUtilityHX)
                        {
                            hx.ID = 0;
                            allSelectedInfo.Add(hx);
                        }
                    }
                    if (model.ObcMixerSplitter != null)
                    {
                        foreach (var hx in model.ObcMixerSplitter)
                        {
                            hx.ID = 0;
                            allSelectedInfo.Add(hx);
                        }
                    }
                    if (allSelectedInfo.Count > 0)
                        reactorBLL.Save(model.dbModel, allSelectedInfo);
                    wd.DialogResult = true;
                }
            }
        }

        public void MouseLeftButtonUp(object sender, MouseButtonEventArgs e)
        {
            if (e.LeftButton == MouseButtonState.Pressed)
            {
                ((Window)sender).DragMove();
            }
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="rootDir"></param>
        /// <param name="streamList"></param>
        /// <param name="details"></param>
        /// <returns></returns>
        public string CreateReactorLoopInpData(string rootDir, List<CustomStream> streamList, List<string> eqList)
        {
            StringBuilder sb = new StringBuilder();
            string[] files = Directory.GetFiles(rootDir, "*.inp");
            string sourceFile = files[0];
            string[] lines = System.IO.File.ReadAllLines(sourceFile);
            List<int> list = new List<int>();
            int i = 0;
            while (i < lines.Length)
            {
                string line = lines[i];
                if (line.Contains("STREAM DATA"))
                {
                    sb.Append(line).Append("\n");
                    i++;
                    break;
                }
                else
                {
                    i++;
                    sb.Append(line).Append("\n");
                }
            }

            List<string> newStreamInfos = new List<string>();
            foreach (CustomStream cs in streamList)
            {
                if (cs != null)
                {
                    string newStreamInfo = string.Empty;
                    lines = FindStreamInfo(lines, cs, ref newStreamInfo);
                    newStreamInfos.Add(newStreamInfo);
                }
            }
            while (i < lines.Length)
            {
                i = removeTagInfo(lines, i, "OUTPUT");
                i = removeTagInfo(lines, i, "RXDATA");
                i = removeTagInfo(lines, i, "RXSET");
                i = removeTagInfo(lines, i, "REACTION");
                i = removeTagInfo(lines, i, "HORX");
            }
            while (i < lines.Length)
            {
                string line = lines[i];
                sb.Append(line).Append("\n");
                i++;

            }



            while (i < lines.Length)
            {
                string line = lines[i];
                if (line.Contains("UNIT OPERATIONS"))
                {
                    sb.Append("UNIT OPERATIONS").Append("\n");
                    foreach (string eq in eqList)
                    {
                        string eqinfo = FindEqInfo(lines, eq);
                        sb.Append(eqinfo).Append("\n");
                    }
                    break;
                }
                else
                {
                    sb.Append(line).Append("\n");
                    i++;
                }
            }
            sb.Append("END");

            return sb.ToString();
        }

        public string[] FindStreamInfo(string[] lines, CustomStream cs, ref string newStreamInfo)
        {
            List<string> list = new List<string>();
            string PropStream = "PROPERTY STREAM=" + cs.StreamName.ToUpper();
            int i = 0;
            while (i < lines.Length)
            {
                string line = lines[i];
                if (line.Contains(PropStream))
                {
                    i++;
                    break;
                }
                else
                {
                    list.Add(line);
                    i++;
                }
            }

            while (i < lines.Length)
            {
                string line = lines[i];
                if (line.Contains("  PROP") || line.Substring(0, 2) != "  ")
                {
                    break;
                }
                else
                {
                    i++;
                }
            }
            while (i < lines.Length)
            {
                string line = lines[i];
                list.Add(line);
                i++;
            }
            newStreamInfo = getStreamData(cs);
            string[] newLines = new string[list.Count];
            newLines = list.ToArray();
            return newLines;
        }



        public string FindEqInfo(string[] lines, string eqName)
        {
            StringBuilder sb = new StringBuilder();
            string EqInfo = "UID=" + eqName.ToUpper();
            int i = 0;
            while (i < lines.Length)
            {
                string line = lines[i];
                if (line.Contains(EqInfo))
                {
                    i++;
                    sb.Append(line).Append("\n");
                    break;
                }
                else
                {
                    i++;
                }
            }

            while (i < lines.Length)
            {
                string line = lines[i];
                if (line.Contains("     "))
                {
                    sb.Append(line).Append("\n");
                    i++;
                }
                else
                {
                    break;
                }
            }
            return sb.ToString();

        }

        private string getStreamData(CustomStream stream)
        {
            StringBuilder data1 = new StringBuilder();
            string streamName = stream.StreamName;
            data1.Append("\tPROP STRM=").Append(streamName.ToUpper()).Append(",&\n");
            data1.Append("\t PRESSURE(MPAG)=").Append(stream.Pressure).Append(",&\n");
            data1.Append("\t TEMPERATURE(C)=").Append(stream.Temperature).Append(",&\n");
            double rate = stream.TotalMolarRate.Value;
            if (rate == 0)
                rate = 1;
            data1.Append("\t RATE(KGM/S)=").Append(rate).Append(",&\n");
            string com = stream.TotalComposition;
            string Componentid = stream.Componentid;
            string CompIn = stream.CompIn;
            string PrintNumber = stream.PrintNumber;
            Dictionary<string, string> compdict = new Dictionary<string, string>();
            data1.Append("\t COMP=&\n");
            string[] coms = com.Split(',');
            string[] PrintNumbers = PrintNumber.Split(',');
            StringBuilder sbCom = new StringBuilder();
            string[] Componentids = Componentid.Split(',');
            string[] CompIns = CompIn.Split(',');
            int comCount = coms.Length;
            for (int i = 0; i < comCount; i++)
            {
                compdict.Add(Componentids[i], coms[i]);
            }
            for (int i = 0; i < comCount; i++)
            {
                //string s = CompIns[i];
                sbCom.Append("/&\n").Append(PrintNumbers[i]).Append(",").Append(coms[i]);
            }
            data1.Append("\t").Append(sbCom.Remove(0, 2)).Append("\n");
            return data1.ToString();
        }

        private int removeTagInfo(string[] lines, int start, string tag)
        {
            int end = start;
            int i = start;
            bool b = false;
            while (i < lines.Length)
            {
                string line = lines[i];
                if (line.Contains(tag))
                {
                    b = true;
                    break;
                }
            }
            if (b)
            {
                i = i + 1;
                while (i < lines.Length)
                {
                    string line = lines[i];
                    if (line.Contains("&"))
                    {
                        i++;
                    }
                    else
                    {
                        end = i;
                        break;
                    }

                }
            }
            return end;

        }

        private List<CustomStream> GetReactorLoopStreamsFromProII(List<string> eqList)
        {
            List<CustomStream> list = new List<CustomStream>();
            ProIIEqDataDAL eqdal = new ProIIEqDataDAL();
            ProIIStreamDataDAL streamdal = new ProIIStreamDataDAL();
            foreach (string eq in eqList)
            {
                ProIIEqData eqData = eqdal.GetModel(SessionPF, PrzFile, eq);
                string feeddata = eqData.FeedData;
                if (!string.IsNullOrEmpty(feeddata))
                {
                    string[] feeds = feeddata.Split(',');
                    foreach (string s in feeds)
                    {
                        if (!streams.Contains(s) && !string.IsNullOrEmpty(s))
                        {
                            streams.Add(s);
                            ProIIStreamData piis = streamdal.GetModel(SessionPF, s, PrzFile);
                            CustomStream cs = ProIIToDefault.ConvertProIIStreamToCustomStream(piis);
                            if (cs != null)
                            {
                                list.Add(cs);
                            }
                        }
                    }
                }
                string productdata = eqData.ProductData;
                if (!string.IsNullOrEmpty(productdata))
                {
                    string[] products = productdata.Split(',');
                    foreach (string s in products)
                    {
                        if (!streams.Contains(s) && !string.IsNullOrEmpty(s))
                        {
                            streams.Add(s);
                            ProIIStreamData piis = streamdal.GetModel(SessionPF, s, PrzFile);
                            CustomStream cs = ProIIToDefault.ConvertProIIStreamToCustomStream(piis);
                            if (cs != null)
                            {
                                list.Add(cs);
                            }
                        }
                    }
                }
            }
            return list;
        }

        private List<CustomStream> GetReactorLoopStreamsFromProII(string eqName)
        {
            ProIIEqDataDAL eqdal = new ProIIEqDataDAL();
            ProIIStreamDataDAL streamdal = new ProIIStreamDataDAL();
            List<CustomStream> list = new List<CustomStream>();
            ProIIEqData eqData = eqdal.GetModel(SessionPF, PrzFile, eqName);
            string feeddata = eqData.FeedData;
            string[] feeds = feeddata.Split(',');
            foreach (string s in feeds)
            {
                if (!streams.Contains(s) && !string.IsNullOrEmpty(s))
                {
                    streams.Add(s);
                    ProIIStreamData piis = streamdal.GetModel(SessionPF, s, PrzFile);
                    CustomStream cs = ProIIToDefault.ConvertProIIStreamToCustomStream(piis);
                    list.Add(cs);
                }
            }
            string productdata = eqData.ProductData;
            string[] products = productdata.Split(',');
            foreach (string s in products)
            {
                if (!streams.Contains(s) && !string.IsNullOrEmpty(s))
                {
                    streams.Add(s);
                    ProIIStreamData piis = streamdal.GetModel(SessionPF, s, PrzFile);
                    CustomStream cs = ProIIToDefault.ConvertProIIStreamToCustomStream(piis);
                    list.Add(cs);
                }
            }
            return list;
        }

        private CustomStream GetReactorLoopStreamInfoFromProII(string streamName)
        {
            CustomStream cs = null;
            string s = streamName;
            if (!streams.Contains(s) && !string.IsNullOrEmpty(s))
            {
                ProIIStreamDataDAL streamdal = new ProIIStreamDataDAL();
                streams.Add(s);
                ProIIStreamData piis = streamdal.GetModel(SessionPF, s, PrzFile);
                cs = ProIIToDefault.ConvertProIIStreamToCustomStream(piis);

            }
            return cs;


        }

        //get eq duty,ltmd, 然后讲HX的值进行替换。







    }

}
