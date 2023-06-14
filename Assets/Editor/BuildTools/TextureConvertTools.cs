/*
* 文件名：TextureConvertTools
* 文件描述：
* 作者：aronliang
* 创建时间：2023/05/24 17:25:03
* 修改记录：
*/

using System.Collections.Generic;
using System.IO;
using System.Text;
using UnityEditor;
using UnityEditor.U2D;
using UnityEngine;
using UnityEngine.U2D;
public class TextureConvertTools
    {
        [MenuItem("Assets/TextureTools/压缩贴图为ASTC")]
        public static void ConvertTexturesAstc()
        {
            string dirPath = GetSelectedDirPath();
            if (dirPath != null)
            {
                string[] paths = Directory.GetFiles(dirPath, "*.*", SearchOption.AllDirectories);
                // string[] paths = Directory.GetFiles(Application.dataPath + ConstString.XGAME_RESOURCES_FOLDER + "/EZFunUI/Texture/BgImg/map/", "*.*", SearchOption.AllDirectories);
                for (var index = 0; index < paths.Length; index++)
                {
                    string path = paths[index];
                    EditorUtility.DisplayProgressBar("贴图检查", path, (float) index / paths.Length);
                    if (!string.IsNullOrEmpty(path) && IsTextureFile(path)) //full name    
                    {
                        string assetRelativePath = GetRelativeAssetPath(path);
                        var importer = (TextureImporter) AssetImporter.GetAtPath(assetRelativePath);
                        var androidSetting = importer.GetPlatformTextureSettings("Android");
                        var iosSetting = importer.GetPlatformTextureSettings("iPhone");
                        var change = false;
                        if (!androidSetting.overridden)
                        {
                            androidSetting.overridden = true;
                            change = true;
                        }
                        if (!iosSetting.overridden)
                        {
                            iosSetting.overridden = true;
                            change = true;
                        }
                        var format = TextureImporterFormat.ASTC_8x8;

                        if (path.ToLower().Contains("mask") || path.ToLower().Contains("ramp"))
                        {
                            format = TextureImporterFormat.ASTC_4x4;
                        }

                        if (androidSetting.format != format)
                        {
                            androidSetting.format = format;
                            change = true;
                        }
                        if (iosSetting.format != format)
                        {
                            iosSetting.format = format;
                            change = true;
                        }
                        if (change)
                        {
                            importer.SetPlatformTextureSettings(iosSetting);
                            importer.SetPlatformTextureSettings(androidSetting);
                            AssetDatabase.ImportAsset(assetRelativePath);
                        }

                    }
                }

                EditorUtility.ClearProgressBar();
            }
        }
        
        static string GetSelectedDirPath()
        {
            if (Selection.assetGUIDs.Length > 1 || Selection.assetGUIDs.Length == 0)
            {
                EditorUtility.DisplayDialog("提示", "请选择一个文件夹！", "OK");
                return null;
            }

            var selectPath = AssetDatabase.GUIDToAssetPath(Selection.assetGUIDs[0]);
            if (!Directory.Exists(selectPath))
            {
                EditorUtility.DisplayDialog("提示", "请选择一个文件夹！", "OK");
                return null;
            }
            string dirPath = new DirectoryInfo(selectPath).FullName;
            return dirPath;
        }
        
        public static string GetRelativeAssetPath(string _fullPath)
        {
            _fullPath = GetRightFormatPath(_fullPath);
            int idx = _fullPath.IndexOf("Assets");
            string assetRelativePath = _fullPath.Substring(idx);
            return assetRelativePath;
        }
        
        static string GetRightFormatPath(string _path)
        {
            return _path.Replace("\\", "/");
        }
        
        public static bool IsTextureFile(string _path)
        {
            string path = _path.ToLower();
            return path.EndsWith(".exr") || path.EndsWith(".psd") || path.EndsWith(".tga") || path.EndsWith(".png") ||
                   path.EndsWith(".jpg") || path.EndsWith(".bmp") || path.EndsWith(".tif") || path.EndsWith(".gif");
        }

        private static string atlasPath = "Assets/GameMain/UIAtlas";
        private static string atlasPathCompress = "Assets/GameMain/UIAtlasCompress";
        private static string SpriteConfigPath = $"Assets/GameMain/Lua/Global/SpriteConfig.lua";
        private static string imagePath = "Assets/GameMain/UIImage";
        
        

        [MenuItem("Tools/TextureTools/一键生成图集")]
        public static void OnKeyGenAtlas()
        {
            AutoGenAtlas();
            AutoGenSpriteCompress();
            AutoGenImgType();
        }
        
        [MenuItem("Tools/TextureTools/生成图集不压缩")]
        public static void AutoGenAtlas()
        {
            if(!Directory.Exists(atlasPath))
                return;
            Dictionary<string, string> configDic = new Dictionary<string, string>();
            var dirs = Directory.GetDirectories(atlasPath);
            int index = 0;
            foreach (var dir in dirs)
            {
                EditorUtility.DisplayProgressBar("生成图集", dir, (float) index / dirs.Length);
                var atlasName = GenAtlasByDir(dir);
                SetDirTextureType(dir);
                // WriteSpriteConfig(dir,atlasName,configDic);
                index++;
            }

            // SaveSpriteConfig(configDic);
            EditorUtility.ClearProgressBar();
        }
        
        [MenuItem("Tools/TextureTools/生成图集压缩")]
        public static void AutoGenSpriteCompress()
        {
            if(!Directory.Exists(atlasPathCompress))
                return;
            Dictionary<string, string> configDic = new Dictionary<string, string>();
            var dirs = Directory.GetDirectories(atlasPathCompress);
            int index = 0;
            foreach (var dir in dirs)
            {
                EditorUtility.DisplayProgressBar("生成图集压缩", dir, (float) index / dirs.Length);
                var atlasName = GenAtlasByDir(dir,true);
                SetDirTextureType(dir);
                index++;
            }
            EditorUtility.ClearProgressBar();
        }

        [MenuItem("Tools/TextureTools/自动设置image格式")]
        public static void AutoGenImgType()
        {
            if(!Directory.Exists(imagePath))
                return;
            int index = 0;
            var files = Directory.GetFiles(imagePath,"*.*", SearchOption.AllDirectories);
            foreach (var file in files)
            {
                EditorUtility.DisplayProgressBar("设置图片格式", file, (float) index / files.Length);
                index++;
                if(!IsTextureFile(file))
                    continue;
                SetTextureType_Low(file);
            }
            EditorUtility.ClearProgressBar();
        }

        public static bool HasSameSprite(string assetPath,out string samePath)
        {
            samePath = null;
            if(!Directory.Exists(atlasPath))
                return false;
            string[] files = Directory.GetFiles(atlasPath, "*.*", SearchOption.AllDirectories);
            foreach (var filePath in files)
            {
                if(!IsTextureFile(filePath))
                    continue;
                var fPath = GetRightFormatPath(filePath);
                if(assetPath == fPath)
                    continue;
                var fileName = Path.GetFileNameWithoutExtension(fPath);
                if (Path.GetFileNameWithoutExtension(assetPath) == fileName)
                {
                    samePath = fPath;
                    return true;
                }
            }

            return false;
        }
        
        private static void SaveSpriteConfig(Dictionary<string, string> config)
        {
            if (File.Exists(SpriteConfigPath))
                AssetDatabase.DeleteAsset(SpriteConfigPath);
            StringBuilder sb = new StringBuilder();
            sb.Append("return {\n");
            foreach (var item in config)
            {
                sb.Append($"\t['{item.Key}'] = '{item.Value}',\n");
            }
            sb.Append("}");
            using (FileStream fs = new FileStream(SpriteConfigPath,FileMode.OpenOrCreate))
            {
                StreamWriter sw = new StreamWriter(fs, Encoding.UTF8);
                sw.Write(sb);
                sw.Close();
            }
            AssetDatabase.Refresh();
        }
        
        private static void WriteSpriteConfig(string dirPath,string atlasName,Dictionary<string,string> dic)
        {
            if(!Directory.Exists(dirPath))
                return;
            DirectoryInfo dirInfo = new DirectoryInfo(dirPath);
            var files = dirInfo.GetFiles("*.*", SearchOption.AllDirectories);
            foreach (var file in files)
            {
                string texturePath = file.FullName;
                if(!File.Exists(texturePath))
                    continue;
                if(!IsTextureFile(texturePath))
                    continue;
                string fileName = Path.GetFileNameWithoutExtension(texturePath);
                if (dic.ContainsKey(fileName))
                {
                    Debug.LogWarning($"已经存在相同名字的图片:{fileName},请修改名字");
                    continue;
                }
                dic.Add(fileName,atlasName);
            }
        }

        //生成图集
        private static string GenAtlasByDir(string dirPath,bool compress = false)
        {
            if(!Directory.Exists(dirPath))
                return null;
            DirectoryInfo dirInfo = new DirectoryInfo(dirPath);
            string atlasName = $"{dirInfo.Name}.spriteatlas";
            string atlasPath = $"{dirPath}/{atlasName}";
            SpriteAtlas spriteAtlas = null;
            bool isNew = false;
            if (File.Exists(atlasPath))
            {
                spriteAtlas = AssetDatabase.LoadAssetAtPath<SpriteAtlas>(atlasPath);
            }
            else
            {
                isNew = true;
                spriteAtlas = new SpriteAtlas();    
            }
            
            SpriteAtlasPackingSettings packingSettings = new SpriteAtlasPackingSettings()
            {
                blockOffset = 1,
                enableRotation = false,
                enableTightPacking = false,
                enableAlphaDilation = false,
                padding = 4,
            };
            spriteAtlas.SetPackingSettings(packingSettings);

            SpriteAtlasTextureSettings spriteAtlasTextureSettings = new SpriteAtlasTextureSettings()
            {
                readable = false,
                generateMipMaps = false,
                sRGB = true,
                filterMode = FilterMode.Bilinear,
            };
            spriteAtlas.SetTextureSettings(spriteAtlasTextureSettings);

            //通用
            TextureImporterPlatformSettings textureImporterPlatformSettings = new TextureImporterPlatformSettings()
            {
                maxTextureSize = 2048,
                format = TextureImporterFormat.Automatic,
                textureCompression = TextureImporterCompression.Uncompressed,
            };
            spriteAtlas.SetPlatformSettings(textureImporterPlatformSettings);
            
            //安卓
            TextureImporterPlatformSettings androidSetting = new TextureImporterPlatformSettings()
            {
                name = "Android",
                maxTextureSize = 2048,
                format = TextureImporterFormat.ASTC_4x4,
                overridden = compress,
            };
            spriteAtlas.SetPlatformSettings(androidSetting);
            
            //IOS
            TextureImporterPlatformSettings iOSSetting = new TextureImporterPlatformSettings()
            {
                name = "iPhone",
                maxTextureSize = 2048,
                format = TextureImporterFormat.ASTC_4x4,
                overridden = compress,
            };
            spriteAtlas.SetPlatformSettings(iOSSetting);
                
            if(isNew)
                AssetDatabase.CreateAsset(spriteAtlas,atlasPath);
                
            // SpriteAtlas atlas = AssetDatabase.LoadAssetAtPath<SpriteAtlas>(atlasPath);
            Object obj = AssetDatabase.LoadAssetAtPath(dirPath,typeof(Object));
            spriteAtlas.Remove(spriteAtlas.GetPackables());
            spriteAtlas.Add(new []{obj});

            AssetDatabase.SaveAssets();
            AssetDatabase.Refresh();
            return dirInfo.Name;
        }

        //修改文件夹下图片的格式
        private static void SetDirTextureType(string dirPath)
        {
            if(!Directory.Exists(dirPath))
                return;
            DirectoryInfo dirInfo = new DirectoryInfo(dirPath);
            var files = dirInfo.GetFiles("*.*", SearchOption.AllDirectories);
            foreach (var file in files)
            {
                string texturePath = file.FullName;
                SetTextureType_High(texturePath);
            }
        }

        //高图片 完全不压缩
        private static void SetTextureType_High(string texturePath)
        {
            if(!File.Exists(texturePath))
                return;
            if(!IsTextureFile(texturePath))
                return;
            texturePath = GetRelativeAssetPath(texturePath);
            var importer = (TextureImporter) AssetImporter.GetAtPath(texturePath);
            importer.textureType = TextureImporterType.Sprite;
            //通用
            TextureImporterPlatformSettings textureImporterPlatformSettings = new TextureImporterPlatformSettings()
            {
                maxTextureSize = 2048,
                format = TextureImporterFormat.Automatic,
                textureCompression = TextureImporterCompression.Uncompressed,
            };
            importer.SetPlatformTextureSettings(textureImporterPlatformSettings);
            
            //安卓
            TextureImporterPlatformSettings androidSetting = new TextureImporterPlatformSettings()
            {
                name = "Android",
                maxTextureSize = 2048,
                format = TextureImporterFormat.ASTC_4x4,
                overridden = false,
            };
            importer.SetPlatformTextureSettings(androidSetting);
            
            //IOS
            TextureImporterPlatformSettings iOSSetting = new TextureImporterPlatformSettings()
            {
                name = "iPhone",
                maxTextureSize = 2048,
                format = TextureImporterFormat.ASTC_4x4,
                overridden = false,
            };
            importer.SetPlatformTextureSettings(iOSSetting);

            AssetDatabase.ImportAsset(texturePath);
        }
        
        //middle 压缩4x4
        private static void SetTextureType_Middle(string texturePath)
        {
            if(!File.Exists(texturePath))
                return;
            if(!IsTextureFile(texturePath))
                return;
            texturePath = GetRelativeAssetPath(texturePath);
            var importer = (TextureImporter) AssetImporter.GetAtPath(texturePath);
            importer.textureType = TextureImporterType.Sprite;
            //通用
            TextureImporterPlatformSettings textureImporterPlatformSettings = new TextureImporterPlatformSettings()
            {
                maxTextureSize = 2048,
                format = TextureImporterFormat.Automatic,
                textureCompression = TextureImporterCompression.Uncompressed,
            };
            importer.SetPlatformTextureSettings(textureImporterPlatformSettings);
            
            //安卓
            TextureImporterPlatformSettings androidSetting = new TextureImporterPlatformSettings()
            {
                name = "Android",
                maxTextureSize = 2048,
                format = TextureImporterFormat.ASTC_4x4,
                overridden = true,
            };
            importer.SetPlatformTextureSettings(androidSetting);
            
            //IOS
            TextureImporterPlatformSettings iOSSetting = new TextureImporterPlatformSettings()
            {
                name = "iPhone",
                maxTextureSize = 2048,
                format = TextureImporterFormat.ASTC_4x4,
                overridden = true,
            };
            importer.SetPlatformTextureSettings(iOSSetting);

            AssetDatabase.ImportAsset(texturePath);
        }
        
        //low 压缩6x6
        private static void SetTextureType_Low(string texturePath)
        {
            if(!File.Exists(texturePath))
                return;
            if(!IsTextureFile(texturePath))
                return;
            texturePath = GetRelativeAssetPath(texturePath);
            var importer = (TextureImporter) AssetImporter.GetAtPath(texturePath);
            importer.textureType = TextureImporterType.Sprite;
            //通用
            TextureImporterPlatformSettings textureImporterPlatformSettings = new TextureImporterPlatformSettings()
            {
                maxTextureSize = 2048,
                format = TextureImporterFormat.Automatic,
                textureCompression = TextureImporterCompression.Uncompressed,
            };
            importer.SetPlatformTextureSettings(textureImporterPlatformSettings);
            
            //安卓
            TextureImporterPlatformSettings androidSetting = new TextureImporterPlatformSettings()
            {
                name = "Android",
                maxTextureSize = 2048,
                format = TextureImporterFormat.ASTC_6x6,
                overridden = true,
            };
            importer.SetPlatformTextureSettings(androidSetting);
            
            //IOS
            TextureImporterPlatformSettings iOSSetting = new TextureImporterPlatformSettings()
            {
                name = "iPhone",
                maxTextureSize = 2048,
                format = TextureImporterFormat.ASTC_6x6,
                overridden = true,
            };
            importer.SetPlatformTextureSettings(iOSSetting);

            AssetDatabase.ImportAsset(texturePath);
        }
    }