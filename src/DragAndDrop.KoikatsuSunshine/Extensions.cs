using System.IO;

namespace DragAndDrop
{
    internal static class Extensions
    {
        public static bool LoadOrImportCharaFile(this ChaFileControl chaFileControl, string path)
        {
            if (chaFileControl.LoadCharaFile(path, 255, false, true)) return true;
            switch (chaFileControl.GetLastErrorCode())
            {
                case 0:
                    return true;
                case -1:
                    DragAndDropCore.Logger.LogMessage("Attempting to import a Koikatu card.\nIt might not load properly unless you copy it to UserData\\chara and let the game import it.");
                    return chaFileControl.LoadCharaFileKoikatsu(path, 255, false, true);
                default:
                    return false;
            }
        }

        public static bool LoadOrImportFileLimited(this ChaFileControl chaFileControl, string filename, byte sex = 255,
            bool face = true, bool body = true, bool hair = true, bool parameter = true, bool coordinate = true, bool extension = true)
        {
            var oldpng = chaFileControl.pngData;
            chaFileControl.LoadFileLimited(filename, sex, face, body, hair, parameter, coordinate, extension);

            // This is the only way to tell if LoadFileLimited failed, pngdata is always replaced if it succeedes
            if (oldpng == chaFileControl.pngData)
            {
                DragAndDropCore.Logger.LogMessage("Attempting to import a Koikatu card.\nIt might not load properly unless you copy it to UserData\\chara and let the game import it.");

                // bug? this seems to lose ext save data
                var tempcfc = new ChaFileControl();
                if (tempcfc.LoadCharaFileKoikatsu(filename, sex, false, true))
                {
                    var tempfile = Path.GetTempFileName();
                    DragAndDropCore.Logger.LogDebug("Saving converted KK card to " + tempfile);
                    tempcfc.SaveCharaFile(tempfile);
                    chaFileControl.LoadFileLimited(tempfile, sex, face, body, hair, parameter, coordinate, extension);
                }
            }

            // switch (chaFileControl.GetLastErrorCode())
            // {
            //     case 0:
            //         return true;
            //     case -1:
            //         DragAndDropCore.Logger.LogMessage("Attempting to import a Koikatu card");
            // 
            //         var tempcfc = new ChaFileControl();
            // System.Console.WriteLine(tempcfc);
            //         if (tempcfc.LoadCharaFileKoikatsu(filename, sex, false, true))
            //         {
            //             var tempfile = Path.GetTempFileName();
            //             tempcfc.SaveCharaFile(tempfile);
            //             chaFileControl.LoadFileLimited(tempfile, sex, face, body, hair, parameter, coordinate, extension);
            //         }
            // System.Console.WriteLine(chaFileControl.GetLastErrorCode());
            //         return chaFileControl.GetLastErrorCode() == 0;
            //     default:
            //         return false;
            // }
            return false;
        }
    }
}