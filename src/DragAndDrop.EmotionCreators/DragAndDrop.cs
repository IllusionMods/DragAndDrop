using BepInEx;
using System.IO;
using System.Linq;
using System.Text;
using System.Collections.Generic;

namespace DragAndDrop.EmotionCreators {
    [BepInPlugin(GUID, PluginName, Version)]
    public class DragAndDrop : DragAndDropCore {

        private static readonly byte[] CharaToken = Encoding.UTF8.GetBytes("【EroMakeChara】");
        private static readonly byte[] SexToken = Encoding.UTF8.GetBytes("sex");
        private static readonly byte[] CoordinateToken = Encoding.UTF8.GetBytes("【EroMakeClothes】");
        private static readonly byte[] KoiCharaToken = Encoding.UTF8.GetBytes("【KoiKatuChara");
        private static readonly byte[] KoiCoordinateToken = Encoding.UTF8.GetBytes("【KoiKatuClothes】");


        internal override void OnFiles(List<string> aFiles, POINT aPos) {

            var goodFiles = aFiles.Where(x => {
                var ext = Path.GetExtension(x).ToLower();
                return ext == ".png";
            });

            if (goodFiles.Count() == 0) {
                Logger.LogMessage("No files to handle");
                return;
            }

            if (CardHandlerMethods.GetActiveCardHandler(out var cardHandler)) {
                foreach (var file in goodFiles) {
                    var bytes = File.ReadAllBytes(file);                    

                    if (BoyerMoore.ContainsSequence(bytes, CharaToken)) {
                        var index = new BoyerMoore(SexToken).Search(bytes).First();
                        var sex = bytes[index + SexToken.Length];
                        cardHandler.Character_Load(file, aPos, sex);
                    } else if (BoyerMoore.ContainsSequence(bytes, KoiCharaToken)) {
                        var index = new BoyerMoore(SexToken).Search(bytes).First();
                        var sex = bytes[index + SexToken.Length];
                        cardHandler.CharacterConvert_Load(file, aPos, sex);
                    } else if (BoyerMoore.ContainsSequence(bytes, CoordinateToken)) {
                        cardHandler.Coordinate_Load(file, aPos);
                    } else if (BoyerMoore.ContainsSequence(bytes, KoiCoordinateToken)) {
                        cardHandler.CoordinateConvert_Load(file, aPos);
                    } else {
                        Logger.LogMessage("This file does not contain any EmotionCreators related data");
                    }
                }
            } else {
                Logger.LogMessage("No handler found for this scene");
            }
        }
    }

}
