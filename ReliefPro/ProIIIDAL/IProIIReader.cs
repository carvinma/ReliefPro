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
        int GetAllEqAndStreamTotal(IList<ProIIEqType> eqTypeList, ref IList<ProIIEqData> eqList, ref IList<string> streamList);
        void GetSteamInfo(string name, ref IList<ProIIStreamData> streamListData);
        void GetEqInfo(string otype, string name, ref IList<ProIIEqData> eqListData);
        void CopyStream(string columnName, int tray, int phase, int trayFlow, ref CustomStream cstream);
        ProIIEqData GetEqInfo(string otype, string name);
        ProIIStreamData GetSteamInfo(string name);
        string GetCriticalPressure(string PH);


        void ReleaseProIIReader();
    }
}
