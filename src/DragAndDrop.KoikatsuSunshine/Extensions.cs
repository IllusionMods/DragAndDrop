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
                // bug? this seems to lose ext save data of the kk card
                var tempcfc = new ChaFileControl();
                if (tempcfc.LoadCharaFileKoikatsu(filename, sex, false, true))
                {
                    DragAndDropCore.Logger.LogMessage("Attempting to import a Koikatu card.\nIt might not load properly unless you copy it to UserData\\chara and let the game import it.");

                    var tempfile = Path.GetTempFileName();
                    DragAndDropCore.Logger.LogDebug("Saving converted KK card to " + tempfile);
                    tempcfc.SaveCharaFile(tempfile);
                    chaFileControl.LoadFileLimited(tempfile, sex, face, body, hair, parameter, coordinate, extension);
                    return true;
                }
            }
            else
            {
                return true;
            }

            return false;
        }
    }
}