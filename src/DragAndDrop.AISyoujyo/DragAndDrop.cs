using BepInEx;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using Manager;

namespace DragAndDrop
{
    [BepInPlugin(GUID, PluginName, Version)]
    public class DragAndDrop : DragAndDropCore
    {
        public const string Version = "1.1.0";

        private static readonly byte[] CharaToken = Encoding.UTF8.GetBytes("【AIS_Chara】");
        private static readonly byte[] SexToken = Encoding.UTF8.GetBytes("sex");
        private static readonly byte[] StudioToken = Encoding.UTF8.GetBytes("KStudio");
        private static readonly byte[] CoordinateToken = Encoding.UTF8.GetBytes("AIS_Clothes");
        private static readonly byte[] PoseToken = Encoding.UTF8.GetBytes("【pose】");
        private static readonly byte[] HouseToken = Encoding.UTF8.GetBytes("【AIS_Housing】");
        
        internal override void OnFiles(List<string> aFiles, POINT aPos)
        {
            var goodFiles = aFiles.Where(x =>
            {
                var ext = Path.GetExtension(x).ToLower();
                return ext == ".png";
            });

            if(goodFiles.Count() == 0)
            {
                Logger.LogMessage("No files to handle");
                return;
            }

            var cardHandler = CardHandlerMethods.GetActiveCardHandler();
            if(cardHandler != null)
            {
                bool inStudio = Scene.Instance && Scene.Instance.NowSceneNames.Any(x => x == "Studio");
                bool inHousing = Scene.Instance && Scene.Instance.NowSceneNames.Any(x => x == "Map") && FindObjectsOfType<Housing.UICtrl>().Any(i => i.IsInit);
                
                foreach(var file in goodFiles)
                {
                    var bytes = File.ReadAllBytes(file);

                    if(BoyerMoore.ContainsSequence(bytes, StudioToken))
                    {
                        if(!inStudio)
                            Logger.LogMessage("Can't load studio scene file in current scene");
                        else
                            cardHandler.Scene_Load(file, aPos);
                    }
                    else if(BoyerMoore.ContainsSequence(bytes, CharaToken))
                    {
                        var index = new BoyerMoore(SexToken).Search(bytes).First();
                        var sex = bytes[index + SexToken.Length];
                        cardHandler.Character_Load(file, aPos, sex);
                    }
                    else if(BoyerMoore.ContainsSequence(bytes, CoordinateToken))
                    {
                        cardHandler.Coordinate_Load(file, aPos);
                    }
                    else if (BoyerMoore.ContainsSequence(bytes, PoseToken))
                    {
                        cardHandler.PoseData_Load(file, aPos);
                    }
                    else if(BoyerMoore.ContainsSequence(bytes, HouseToken))
                    {
                        if(!inHousing)
                            Logger.LogMessage("Can't load housing file in current scene");
                        else
                            cardHandler.HouseData_Load(file, Logger);
                    }
                    else
                    {
                        Logger.LogMessage("This file does not contain any AIS related data");
                    }
                }
            }
            else
            {
                Logger.LogMessage("No handler found for this scene");
            }
        }
    }
}
