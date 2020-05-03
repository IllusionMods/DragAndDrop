using Housing;
using System.Linq;
using AIProject.Scene;
using BepInEx.Logging;
using Illusion.Extensions;
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
            var craftScene = Singleton<CraftScene>.Instance;
            var housingID = craftScene.HousingID;

            if (Singleton<Manager.Housing>.Instance.dicAreaInfo.TryGetValue(housingID, out var areaInfo))
            {
                if (Singleton<Manager.Housing>.Instance.dicAreaSizeInfo.TryGetValue(areaInfo.size, out var areaSizeInfo))
                {
                    if (areaSizeInfo.compatibility.Contains(Singleton<Manager.Housing>.Instance.GetSizeType(craftInfo.AreaNo)))
                    {
                        ConfirmScene.Sentence = "データを読込みますか？\n" + "セットされたアイテムは削除されます。".Coloring("#DE4529FF").Size(24);
                        ConfirmScene.OnClickedYes = () =>
                        {
                            Singleton<Selection>.Instance.SetSelectObjects(null);
                            craftScene.UICtrl.ListUICtrl.ClearList();
                            Singleton<UndoRedoManager>.Instance.Clear();
                            craftScene.WorkingUICtrl.Visible = true;
                            Singleton<Manager.Housing>.Instance.LoadAsync(path, obj =>
                            {
                                craftScene.UICtrl.ListUICtrl.UpdateUI();
                                craftScene.WorkingUICtrl.Visible = false;
                            });
                        };
                        ConfirmScene.OnClickedNo = () => { };
                        Singleton<Game>.Instance.LoadDialog();
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