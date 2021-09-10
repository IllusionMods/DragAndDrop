using HarmonyLib;
using Manager;
using Studio;
using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace DragAndDrop
{
    internal class StudioHandler : CardHandlerMethods
    {
        public override bool Condition => Scene.initialized && Scene.NowSceneNames.Any(x => x == "Studio");

        public override void Scene_Load(string path, POINT pos)
        {
            var studio = Studio.Studio.Instance;

            studio.colorPalette.visible = false;

            var empty = Scene.commonSpace.transform.childCount == 0;
            if (empty || !DragAndDropCore.ShowSceneOverwriteWarnings.Value)
            {
                studio.StartCoroutine(studio.LoadSceneCoroutine(path));
            }
            else
            {
                Studio.CheckScene.unityActionYes = () =>
                {
                    Manager.Scene.Unload();
                    // Wait for a frame for the dialog to unload before loading the scene (not necessary?)
                    studio.StartCoroutine(new[] { null, studio.LoadSceneCoroutine(path) }.GetEnumerator());
                };
                Studio.CheckScene.unityActionNo = () =>
                {
                    Manager.Scene.Unload();
                };
                // Need to use the scene reset sprite because the scene load sprite isn't loaded unless the scene load window is opened
                Studio.CheckScene.sprite = studio.systemButtonCtrl.spriteInit;

                Scene.LoadReserve(new Scene.Data
                {
                    levelName = "StudioCheck",
                    isAdd = true
                }, false);
            }
        }

        public override void Scene_Import(string path, POINT pos)
        {
            Studio.Studio.Instance.ImportScene(path);
        }

        public override void Character_Load(string path, POINT pos, byte sex)
        {
            var characters = GetSelectedCharacters();
            if(characters.Count > 0)
            {
                foreach(var chara in characters)
                {
                    chara.charInfo.fileParam.sex = sex;
                    chara.ChangeChara(path);
                }

                UpdateStateInfo();
            }
            else if(sex == 1)
            {
                Studio.Studio.Instance.AddFemale(path);
            }
            else if(sex == 0)
            {
                Studio.Studio.Instance.AddMale(path);
            }
        }

        public override void Coordinate_Load(string path, POINT pos)
        {
            var characters = GetSelectedCharacters();
            if(characters.Count > 0)
            {
                foreach(var chara in characters)
                    chara.LoadClothesFile(path);

                UpdateStateInfo();
            }
        }

        public override void PoseData_Load(string path, POINT pos)
        {
            try
            {
                var characters = GetSelectedCharacters();
                if(characters.Count > 0)
                {
                    foreach(var chara in characters)
                        PauseCtrl.Load(chara, path);
                }
            }
            catch(Exception ex)
            {
                DragAndDrop.Logger.Log(BepInEx.Logging.LogLevel.Error, ex);
            }
        }

        private List<OCIChar> GetSelectedCharacters()
        {
            return GuideObjectManager.Instance.selectObjectKey.Select(x => Studio.Studio.GetCtrlInfo(x) as OCIChar).Where(x => x != null).ToList();
        }

        private void UpdateStateInfo()
        {
            var mpCharCtrl = GameObject.FindObjectOfType<MPCharCtrl>();
            if(mpCharCtrl)
            {
                int select = mpCharCtrl.select;
                if(select == 0) mpCharCtrl.OnClickRoot(0);
            }
        }
    }
}
