using System;
using System.IO;
using Bright.Serialization;
using UnityEngine;

namespace Core
{
    public class TestLoadConfig:MonoBehaviour
    {
        private void Awake()
        {
            Debug.Log("Awake------------");
            string gameConfDir = @"Assets/GameMain/DataTables/LubanByteDatas";
            var tables = new cfg.Tables(file => new ByteBuf(File.ReadAllBytes(gameConfDir + "/" + file + ".bytes")));
            cfg.luban.Item itemInfo = tables.TbItem.Get(10000);
            Debug.Log(string.Format("id:{0} name:{1} desc:{2}" ,
                                    itemInfo.Id, itemInfo.Name, itemInfo.Desc));
            var dataLists = tables.TbItem.DataList;
            foreach (var itemInfo2 in dataLists)
            {
                Debug.Log(string.Format("id:{0} name:{1} desc:{2}" ,
                    itemInfo2.Id, itemInfo2.Name, itemInfo2.Desc));
            }
        }

        private void Update()
        {
            // if (Input.GetKeyDown(KeyCode.A))
            // {
            //     var table = ConfigsManager.Instance.GetExcelTableByName(TableNameDefine.AbilityuseconfigTable) as
            //         AbilityuseconfigTable;
            //     var line = table.GetRecorder(402003001);
            //     Debug.LogError(line.AbilityParameter + "Name:" + line.Name);
            //     var EntranceTypes = line.EntranceType;
            //     for (int i = 0; i < EntranceTypes.Count; i++)
            //     {
            //         Debug.LogError("EntranceTypes[i]:" + EntranceTypes[i]);
            //     }
            //
            //     var DescriptionParams = line.DescriptionParams;
            //     for (int i = 0; i < DescriptionParams.Count; i++)
            //     {
            //         var DescriptionParamsi = DescriptionParams[i];
            //         Debug.LogError("DescriptionParamsi[i] Value:" + DescriptionParamsi.Value + " Add:" + DescriptionParamsi.CanAdd);
            //     }
            // }
        }
    }
}