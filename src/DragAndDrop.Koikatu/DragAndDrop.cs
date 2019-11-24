using BepInEx;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

namespace DragAndDrop
{
    [BepInPlugin(GUID, PluginName, Version)]
    public class DragAndDrop : DragAndDropCore
    {
        private static readonly byte[] StudioToken = Encoding.UTF8.GetBytes("【KStudio】");
        private static readonly byte[] CharaToken = Encoding.UTF8.GetBytes("【KoiKatuChara");
        private static readonly byte[] SexToken = Encoding.UTF8.GetBytes("sex");
        private static readonly byte[] CoordinateToken = Encoding.UTF8.GetBytes("【KoiKatuClothes】");
        private static readonly byte[] PoseToken = Encoding.UTF8.GetBytes("【pose】");

        internal override void OnFiles(List<string> aFiles, POINT aPos)
        {
            var goodFiles = aFiles.Where(x =>
            {
                var ext = Path.GetExtension(x).ToLower();
                return ext == ".png" || ext == ".dat";
            });

            if(goodFiles.Count() == 0)
            {
                Logger.LogMessage("No files to handle");
                return;
            }

            var cardHandler = CardHandlerMethods.GetActiveCardHandler();
            if(cardHandler != null)
            {
                foreach(var file in goodFiles)
                {
                    var bytes = File.ReadAllBytes(file);

                    if(BoyerMoore.ContainsSequence(bytes, StudioToken))
                    {
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
                    else if(BoyerMoore.ContainsSequence(bytes, PoseToken))
                    {
                        cardHandler.PoseData_Load(file, aPos);
                    }
                    else
                    {
                        Logger.LogMessage("This file does not contain any koikatu related data");
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
