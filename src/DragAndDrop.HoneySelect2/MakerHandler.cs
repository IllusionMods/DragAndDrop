﻿using CharaCustom;
using HarmonyLib;
using UnityEngine;
using UnityEngine.UI;

namespace DragAndDrop
{
    internal class MakerHandler : CardHandlerMethods
    {
        public override bool Condition => GameObject.FindObjectOfType<CharaCustom.CharaCustom>();

        public override void Character_Load(string path, POINT pos, byte sex)
        {
            var cvsCharaLoad = GameObject.FindObjectOfType<CvsO_CharaLoad>();
            var tglLoadOption = cvsCharaLoad.charaLoadWin.tglLoadOption;
            int num = 0;

            if(tglLoadOption != null)
            {
                if(tglLoadOption[0].isOn) num |= 1;
                if(tglLoadOption[1].isOn) num |= 2;
                if(tglLoadOption[2].isOn) num |= 4;
                if(tglLoadOption[3].isOn) num |= 8;
                if(tglLoadOption[4].isOn) num |= 16;
            }

            cvsCharaLoad.charaLoadWin.onClick03.Invoke(new CustomCharaFileInfo { FullPath = path }, num);
        }

        public override void Coordinate_Load(string path, POINT pos)
        {
            var cvsClothesLoad = GameObject.FindObjectOfType<CvsC_ClothesLoad>();
            cvsClothesLoad.clothesLoadWin.onClick03.Invoke(new CustomClothesFileInfo { FullPath = path });
        }
    }
}
