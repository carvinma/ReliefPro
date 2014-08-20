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
using ReliefProMain.Models.ReactorLoops;
using ReliefProModel.ReactorLoops;
using ReliefProDAL;
using ReliefProModel;
using ReliefProMain.View;
using ReliefProMain.ViewModel.ReactorLoops;
using ReliefProMain.View.ReactorLoops;
using System.IO;
using System.Windows.Controls;
using ReliefProBLL;


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
        
        private int reactorLoopID;
        public ReactorLoopModel model { get; set; }
        private ReactorLoopBLL reactorBLL;
        public SourceFile SourceFileInfo { get; set; }
        public string FileFullPath { get; set; }
        public string FileName { set; get; }

        public List<string> streams = new List<string>();
        List<string> processHxList = new List<string>();
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

            ObservableCollection<string> list = GetProIIStreamNames();

            model.SeparatorSource = GetProIIFlashs();
            model.StreamSource = list;
           
            if (SourceFileInfo != null)
            {
                model.ObcProcessHXSource = GetProIIHXs(0);
                model.ObcUtilityHXSource = GetProIIHXs(1);
                model.ObcMixerSplitterSource = GetProIIMixers();
            }
            if (model.dbModel==null)
            {
                model.ObcProcessHX = new ObservableCollection<ReactorLoopDetail>();
                model.ObcUtilityHX = new ObservableCollection<ReactorLoopDetail>();
                model.ObcMixerSplitter = new ObservableCollection<ReactorLoopDetail>();
            }
        }
        private ObservableCollection<string> GetProIIStreamNames()
        {
            ObservableCollection<string> rlt = new ObservableCollection<string>();
            ProIIStreamDataDAL dal = new ProIIStreamDataDAL();
            IList<ProIIStreamData> list = dal.GetAllList(SessionPF, FileName);
            var query = from s in list
                        orderby s.StreamName
                        select s;
            foreach (ProIIStreamData s in query.ToList())
            {
                rlt.Add(s.StreamName);
            }
            return rlt;
        }
        private ObservableCollection<string> GetProIIFlashs()
        {
            ObservableCollection<string> rlt = new ObservableCollection<string>();
            ProIIEqDataDAL dal = new ProIIEqDataDAL();
            IList<ProIIEqData> list = dal.GetAllList(SessionPF, FileName, "Flash");
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
            IList<ProIIEqData> list = dal.GetAllList(SessionPF, SourceFileInfo.FileName, "Hx");
            var query = from s in list
                        orderby s.EqName
                        select s;
            foreach (ProIIEqData eq in query.ToList())
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
            IList<ProIIEqData> list = dal.GetAllList(SessionPF, SourceFileInfo.FileName, "Mixer");
            var query = from s in list
                        orderby s.EqName
                        select s;
            foreach (ProIIEqData eq in query.ToList())
            {
                ReactorLoopDetail d = new ReactorLoopDetail();
                d.DetailInfo = eq.EqName;
                d.ReactorType = 2;
                rlt.Add(d);
            }
            IList<ProIIEqData> list2 = dal.GetAllList(SessionPF, SourceFileInfo.FileName, "Spliter");
            var query2 = from s in list2
                        orderby s.EqName
                        select s;
            foreach (ProIIEqData eq in query2.ToList())
            {
                ReactorLoopDetail d = new ReactorLoopDetail();
                d.DetailInfo = eq.EqName;
                d.ReactorType = 2;
                rlt.Add(d);
            }
            return rlt;
        }

        public ReactorLoopVM( ISession SessionPF, ISession SessionPS, string dirPlant, string dirProtectedSystem)
        {
            model = new ReactorLoopModel();
            this.SessionPS = SessionPS;
            this.SessionPF = SessionPF;
            DirPlant = dirPlant;
            DirProtectedSystem = dirProtectedSystem;
            
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
                SourceFileBLL sfbll = new SourceFileBLL(SessionPF);
                SourceFileInfo = sfbll.GetSourceFileInfo(model.dbModel.SourceFile);
                FileFullPath = DirPlant + @"\" + SourceFileInfo.FileNameNoExt + @"\" + SourceFileInfo.FileName;
                FileName = SourceFileInfo.FileName;
                InitPage();
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
            SelectPathVM vm = new SelectPathVM(SessionPF);
            view.DataContext = vm;
            view.WindowStartupLocation = WindowStartupLocation.CenterScreen;
            if (view.ShowDialog() == true)
            {
                FileName = vm.SelectedFile;
                SourceFileInfo = vm.SourceFileInfo;
                InitPage();
            }
        }
        private void Simulation(object obj)
        {
            streams.Clear();
            List<CustomStream> csList = new List<CustomStream>();
            List<string> eqList = new List<string>();
             processHxList = new List<string>();
            foreach (ReactorLoopDetail d in model.ObcProcessHX)
            {
                processHxList.Add(d.DetailInfo);
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
            csList = GetReactorLoopStreamsFromProII(eqList);
            List<CustomStream> csList2 = GetReactorLoopStreamsFromProII(processHxList);
            foreach (CustomStream cs in csList2)
            {
                csList.Add(cs);
            }
            if (!string.IsNullOrEmpty(model.ColdReactorFeedStream) && !streams.Contains(model.ColdReactorFeedStream))
            {               
                if (!streams.Contains(model.ColdReactorFeedStream))
                {
                    streams.Add(model.ColdReactorFeedStream);
                    CustomStream cs = GetReactorLoopStreamInfoFromProII(model.ColdReactorFeedStream);
                    csList.Add(cs);
                }
            }
            if (!string.IsNullOrEmpty(model.HXNetworkColdStream) && !streams.Contains(model.HXNetworkColdStream))
            {
                if (!streams.Contains(model.HXNetworkColdStream))
                {
                    CustomStream cs = GetReactorLoopStreamInfoFromProII(model.HXNetworkColdStream);
                    csList.Add(cs);
                    streams.Add(model.HXNetworkColdStream);
                }
            }
            if (!string.IsNullOrEmpty(model.InjectionWaterStream) && !streams.Contains(model.InjectionWaterStream))
            {
                if (!streams.Contains(model.InjectionWaterStream))
                {
                    CustomStream cs = GetReactorLoopStreamInfoFromProII(model.InjectionWaterStream);
                    csList.Add(cs);
                    streams.Add(model.InjectionWaterStream);
                }
            }
            if (!string.IsNullOrEmpty(model.EffluentStream) && !streams.Contains(model.EffluentStream))
            {
                if (!streams.Contains(model.EffluentStream))
                {
                    CustomStream cs = GetReactorLoopStreamInfoFromProII(model.EffluentStream);
                    csList.Add(cs);
                    streams.Add(model.EffluentStream);
                }
            }
            if (!string.IsNullOrEmpty(model.EffluentStream2) && !streams.Contains(model.EffluentStream2))
            {
                if (!streams.Contains(model.EffluentStream2))
                {
                    CustomStream cs = GetReactorLoopStreamInfoFromProII(model.EffluentStream2);
                    csList.Add(cs);
                    streams.Add(model.EffluentStream2);
                }
            }


            string dir = DirPlant + @"\" + SourceFileInfo.FileNameNoExt;

            int errorTag = 0;
            string inpData = CreateReactorLoopInpData(dir, csList, eqList,processHxList,ref errorTag);
            if (errorTag == -1)
            {
                MessageBox.Show("New inp file has error ","Message Box");
                return;
            }
            string newInpFile = DirProtectedSystem + @"\" + SourceFileInfo.FileNameNoExt+ @"\"+SourceFileInfo.FileNameNoExt+".inp";
            string sourcePrzFile = DirPlant + @"\" + SourceFileInfo.FileNameNoExt + @"\" + SourceFileInfo.FileName;

            string newInpDir = DirProtectedSystem + @"\" + SourceFileInfo.FileNameNoExt;
            if (!Directory.Exists(newInpDir))
            {
                Directory.CreateDirectory(newInpDir);
            }
            File.Create(newInpFile).Close();
            File.WriteAllText(newInpFile, inpData);
            
            ReactorLoopSimulationView v = new ReactorLoopSimulationView();
            ReactorLoopSimulationVM vm = new ReactorLoopSimulationVM(newInpFile,sourcePrzFile,SourceFileInfo.FileVersion,processHxList,SessionPF);
            v.DataContext = vm;
            v.ShowDialog();


        }
        private void Save(object obj)
        {
            if (obj != null)
            {
                System.Windows.Window wd = obj as System.Windows.Window;
                if (wd != null)
                {
                    model.SourceFile = FileName;
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
                    ProtectedSystemDAL psDAL = new ProtectedSystemDAL();
                    ProtectedSystem ps = new ProtectedSystem();
                    ps.PSType = 6;
                    psDAL.Add(ps, SessionPS);
                    SourceFileDAL sfdal = new SourceFileDAL();
                    SourceFileInfo = sfdal.GetModel(model.SourceFile, SessionPF);
                    SessionPS.Flush();


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
        public string CreateReactorLoopInpData(string rootDir, List<CustomStream> csList, List<string> eqList, List<string> processHxList,ref int errorTag)
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
                if (line.Contains("  SEQUENCE"))
                {
                    i = removeTagInfo(lines, i, "  SEQUENCE");
                    break;
                }
                else
                {
                    sb.Append(line).Append("\n");
                    i++;
                }
            }
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

            string assayLVs = GetAssayLVStreamInfo(lines, i);
            sb.Append(assayLVs);
            
            foreach (CustomStream cs in csList)
            {
                if (cs != null)
                {
                   string s= getStreamData(cs);
                   sb.Append(s);
                }
            }
            
            while (i < lines.Length)
            {
                i = removeTagInfo(lines, i, "  OUTPUT ");
                i = removeRowTagInfo(lines, i, "RXDATA");
                i = removeRowTagInfo(lines, i, "  RXSET ID=");
                i = removeRowTagInfo(lines, i, "    REACTION ID=");
                int end = removeRowTagInfo(lines, i, "      HORX ");
                string line = lines[i];
                if (line.Contains("UNIT OPERATIONS"))
                {
                    break;
                }
                else
                    i = end;
                
            }

            int unitStart = 0;
            while (i < lines.Length)
            {
                string line = lines[i];
                if (line.Contains("UNIT OPERATIONS"))
                {
                    unitStart = i;
                    sb.Append("UNIT OPERATIONS").Append("\n");
                    foreach (string eq in eqList)
                    {
                        string eqinfo = CopyEqInfo(lines, eq);
                        sb.Append(eqinfo).Append("\n");
                    }

                    break;
                }
                else
                {
                    //sb.Append(line).Append("\n");
                    i++;
                }
            }
            i = unitStart;
            while (i < lines.Length)
            {
                foreach (string eq in processHxList)
                {
                    string eqinfo = FindProcessHxInfo(lines, i, eq, ref errorTag);
                    sb.Append(eqinfo).Append("\n");                   
                }
                break;
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

        public string GetAssayLVStreamInfo(string[] lines,int start)
        {
            StringBuilder sb = new StringBuilder();
            int i = start;
            while (i < lines.Length-1)
            {
                string line = lines[i];
                string line2 = lines[i + 1];
                if (line.Contains(" STREAM=") && line2.Contains("ASSAY=LV"))
                {
                    sb.Append(line).Append("\n");
                    sb.Append(line2).Append("\n");
                    i = i + 2;
                    line = lines[i];
                    while (line.Contains("    "))
                    {
                        sb.Append(line).Append("\n");
                        i++;
                        line = lines[i];
                    }
                }
                else
                {
                    i = i + 2;
                    if (line.Contains("UNIT OPERATIONS") || line2.Contains("UNIT OPERATIONS"))
                    {
                        break;
                    }
                }
               
            }
            return sb.ToString();
            
        }

        public string CopyEqInfo(string[] lines, string eqName)
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

        public string FindProcessHxInfo(string[] lines, int start, string eqName, ref int errorTag)
        {
            StringBuilder sb = new StringBuilder();
            string EqInfo = "HX   UID=" + eqName.ToUpper();
            int i = start;
            while (i < lines.Length)
            {
                string line = lines[i];
                if (line.Contains(EqInfo))
                {
                    string eqInfo = GetNewHXInfo(lines, i, eqName, ref errorTag);
                    sb.Append(eqInfo);
                    break;
                }
                else
                {
                    i++;
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
            int len = tag.Length;
            while (i < lines.Length)
            {
                string line = lines[i];
                if (line.Contains(tag) && line.Substring(0,len)==tag)
                {
                    b = true;
                    break;
                }
                else
                {
                    i++;
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
                        end = i+1;
                        break;
                    }

                }
            }
            return end;

        }

        private int removeRowTagInfo(string[] lines, int start, string tag)
        {
            int end = start;
            int i = start;
            int len = tag.Length;
            while (i < lines.Length)
            {
                string line = lines[i];
                if (line.Contains(tag) && line.Substring(0, len) == tag)
                {
                    end = i+1;
                    break;
                }
                else
                {
                    i++;
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
                ProIIEqData eqData = eqdal.GetModel(SessionPF, SourceFileInfo.FileName, eq);
                string feeddata = eqData.FeedData;
                if (!string.IsNullOrEmpty(feeddata))
                {
                    string[] feeds = feeddata.Split(',');
                    foreach (string s in feeds)
                    {
                        if (!streams.Contains(s) && !string.IsNullOrEmpty(s))
                        {
                            streams.Add(s);
                            ProIIStreamData piis = streamdal.GetModel(SessionPF, s, SourceFileInfo.FileName);
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
                            ProIIStreamData piis = streamdal.GetModel(SessionPF, s, SourceFileInfo.FileName);
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
            ProIIEqData eqData = eqdal.GetModel(SessionPF, SourceFileInfo.FileName, eqName);
            string feeddata = eqData.FeedData;
            string[] feeds = feeddata.Split(',');
            foreach (string s in feeds)
            {
                if (!streams.Contains(s) && !string.IsNullOrEmpty(s))
                {
                    streams.Add(s);
                    ProIIStreamData piis = streamdal.GetModel(SessionPF, s, SourceFileInfo.FileName);
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
                    ProIIStreamData piis = streamdal.GetModel(SessionPF, s, SourceFileInfo.FileName);
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
                ProIIStreamData piis = streamdal.GetModel(SessionPF, s, SourceFileInfo.FileName);
                cs = ProIIToDefault.ConvertProIIStreamToCustomStream(piis);

            }
            return cs;


        }

        //get eq duty,ltmd, 然后讲HX的值进行替换。


        private double GetLTMD(ISession sessionPlant,string fileName,string eqName,out double duty)
        {
            double tin=0;
            double tout=0;
            double Tin=0;
            double Tout=0;
            double ltmd = -1;
            ProIIEqDataDAL eqDAL = new ProIIEqDataDAL();
            ProIIStreamDataDAL streamDAL = new ProIIStreamDataDAL();
            ProIIEqData eqData = eqDAL.GetModel(sessionPlant, fileName, eqName);
            duty = double.Parse(eqData.DutyCalc);
            string feeddata = eqData.FeedData;
            string[] feeds = feeddata.Split(',');
             string productdata = eqData.ProductData;
            string[] products = productdata.Split(',');
            if (feeds.Length >= 2 && products.Length > -2)
            {
                ProIIStreamData prostream = streamDAL.GetModel(sessionPlant, feeds[0], fileName);
                if(!string.IsNullOrEmpty(prostream.Temperature))
                {
                    tin = double.Parse(prostream.Temperature);
                    Tin = tin;
                }
                prostream = streamDAL.GetModel(sessionPlant, feeds[1], fileName);
                if (!string.IsNullOrEmpty(prostream.Temperature))
                {
                    double t = double.Parse(prostream.Temperature);
                    if (t > tin)
                        Tin = t;
                    else
                        tin = t;
                }

                prostream = streamDAL.GetModel(sessionPlant, products[0], fileName);
                if (!string.IsNullOrEmpty(prostream.Temperature))
                {
                    tout = double.Parse(prostream.Temperature);
                    Tout = tout;
                }
                prostream = streamDAL.GetModel(sessionPlant, products[1], fileName);
                if (!string.IsNullOrEmpty(prostream.Temperature))
                {
                    double t = double.Parse(prostream.Temperature);
                    if (t > tout)
                        Tout = t;
                    else
                        tout = t;
                }
                
                double a = Tin - tout;
                double b = Tout - tin;
                double c = Math.Log(a / b);
                ltmd = (a - b) / c;
            }
            return ltmd;
        }

        private string GetNewHXInfo(string[] lines, int start,string hxName,ref int errorTag)
        {
            StringBuilder sb = new StringBuilder();
            int i = start;
            string line = lines[i];
            sb.Append(line).Append("\n");
            i++;
            line = lines[i];
            sb.Append(line).Append("\n");
            i++;
            line = lines[i];
            sb.Append(line).Append("\n");
            i++;
            line = lines[i];
            sb.Append(line);
            double duty=0;
            double ltmd = GetLTMD(SessionPF, FileName, hxName,out  duty);
            double k = 300;
            double a = duty / ltmd / k;
            sb.Append(" ,U=").Append(ltmd).Append(",AREA=").Append(a).Append("\n");
            if (ltmd < 0)
                errorTag = -1;
            return sb.ToString();
            
        }

        private string[] ReplaceStreamInfo(string[] lines, int start, string streamName, double temp)
        {
            List<string> list = new List<string>();
            int i = start;
            string temperKey="TEMPERATURE";
            string key = "PROPERTY STREAM=" + streamName.ToUpper();
            while (i < lines.Length)
            {
                string line = lines[i];
                if (line.Contains(key) && line.Contains(temperKey))
                {
                    string oldValue=string.Empty;
                    string newValue=temperKey+temp;

                    int s=line.IndexOf(temperKey);
                    string sub=line.Substring(s);
                    s=sub.IndexOf(",");
                     oldValue=sub.Substring(0,s);
                    string newLine=line.Replace(oldValue,newValue);
                    list.Add(newLine);
                    break;
                }
            }
            while (i < lines.Length)
            {
                string line = lines[i];
                list.Add(line);
                i++;
            }
            return list.ToArray();
        }

            
    }

}
