using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ReliefProModel;

namespace ProII
{
    public interface IProIIReader
    {
        void InitProIIReader(string przFile);
        int GetAllEqAndStreamTotal(IList<systbProIIEqType> eqTypeList, ref IList<tbProIIEqData> eqList, ref IList<string> streamList);
        void GetSteamInfo(string name, ref IList<tbProIIStreamData> streamListData);
        void GetEqInfo(string otype, string name, ref IList<tbProIIEqData> eqListData);
        tbProIIStreamData CopyStream(string columnName, int tray, int phase, int trayFlow);
        tbProIIEqData GetEqInfo(string otype, string name);
        tbProIIStreamData GetSteamInfo(string name);
        string GetCriticalPressure(string PH);
        string GetCriticalTemperature(string PH);
        string GetCricondenbarPress(string PH);
        string GetCricondenbarTemp(string PH);
        double[] GetCompInInfo(string CompName);
        //Dictionary<string, ProIIStreamData> GetTowerStreamInfoExtra(string otype, string eqname);

        void ReleaseProIIReader();
    }
}
