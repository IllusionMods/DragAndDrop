using Housing;
using System.Linq;
using BepInEx.Logging;
using UnityEngine;
using Manager;

namespace DragAndDrop
{
    internal class HousingHandler : CardHandlerMethods
    {
        public override bool Condition => Singleton<Scene>.Instance.NowSceneNames.Any(sceneName => sceneName == "Map") && Object.FindObjectsOfType<UICtrl>().Any(i => i.IsInit);
        
        public override void HouseData_Load(string path, ManualLogSource Logger)
        {
            var craftInfo = CraftInfo.LoadStatic(path);
            var housingID = Singleton<CraftScene>.Instance.HousingID;
            
            if (Singleton<Manager.Housing>.Instance.dicAreaInfo.TryGetValue(housingID, out var areaInfo))
            {
                if (Singleton<Manager.Housing>.Instance.dicAreaSizeInfo.TryGetValue(areaInfo.size, out var areaSizeInfo))
                {
                    if (areaSizeInfo.compatibility.Contains(Singleton<Manager.Housing>.Instance.GetSizeType(craftInfo.AreaNo)))
                    {
                        Singleton<Selection>.Instance.SetSelectObjects(null);
                        Singleton<UndoRedoManager>.Instance.Clear();
                        Singleton<Manager.Housing>.Instance.Load(path, true, true);
                        Singleton<Manager.Housing>.Instance.CheckOverlap();
                        Object.FindObjectsOfType<UICtrl>().First(i => i.IsInit).ListUICtrl.UpdateUI();
                    }
                    else
                    {
                        Logger.LogMessage("Incorrect housing location");
                    }
                }
            }
        }
    }
}