using System.Collections.Generic;
using UnityEngine;

namespace Ironcow.Data
{

    [System.Serializable]
    public class SheetInfoSO
    {
        public string className;
        public string sheetId;
        public string key;
        public List<Dictionary<string, string>> datas;
        public bool isUpdate;
    }
}