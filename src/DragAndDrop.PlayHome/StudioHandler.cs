using BepInEx.Logging;
using HarmonyLib;
using Studio;
using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace DragAndDrop
{
    internal class StudioHandler : CardHandlerMethods
    {
        public override bool Condition => SceneManager.GetActiveScene().name == "Studio";

        public override void Scene_Load(string path, POINT pos)
        {
            var studio = Studio.Studio.Instance;

            studio.colorPaletteCtrl.visible = false;

            var empty = Singleton<Studio.Scene>.Instance.commonSpace.transform.childCount == 0;
            if (empty || !DragAndDropCore.ShowSceneOverwriteWarnings.Value)
            {
                studio.LoadScene(path);
            }
            else
            {
                Studio.CheckScene.unityActionYes = () =>
                {
                    Singleton<Studio.Scene>.Instance.UnLoad();
                    studio.LoadScene(path);
                };
                Studio.CheckScene.unityActionNo = () =>
                {
                    Singleton<Studio.Scene>.Instance.UnLoad();
                };
                // Need to use the scene reset sprite because the scene load sprite isn't loaded unless the scene load window is opened
                Studio.CheckScene.sprite = Traverse.Create(studio.systemButtonCtrl).Field<Sprite>("spriteInit").Value;

                Singleton<Studio.Scene>.Instance.Load(new Studio.Scene.Data
                {
                    sceneName = "StudioCheck",
                    isLoading = false,
                    isAsync = false,
                    isFade = false,
                    isOverlap = true,
                    isAdd = true
                });
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
                    //chara.charInfo.fileParam.sex = sex;
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
                DragAndDrop.Logger.Log(LogLevel.Error, ex);
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
                int select = Traverse.Create(mpCharCtrl).Field("select").GetValue<int>();
                if(select == 0) mpCharCtrl.OnClickRoot(0);
            }
        }
    }
}
