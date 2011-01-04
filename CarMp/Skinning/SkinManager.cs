using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;

namespace CarMP.Skinning
{
    public class SkinManager
    {
        public List<Skin> AvailableSkins { get; private set; }

        public SkinManager()
        {
            AvailableSkins = new List<Skin>();
        }

        public void LoadSkins()
        {
            AvailableSkins.Clear();
            
            foreach (string dir in IO.FileSystem.GetDirectories(AppMain.Settings.SkinPath))
                foreach (FileInfo file in IO.FileSystem.GetFiles(dir, new List<string> { Constants.SKIN_FILE_EXTENSION }))
                    if (string.Compare(file.Name, Constants.SKIN_FULL_FILE_NAME, true) == 0)
                    {
                        var skin = new Skin(dir);
                        // Add to collection if there is an error, or
                        // there's no duplicate name already here
                        if(skin.SkinLoadError == null
                            || GetSkin(skin.Name) == null)
                            AvailableSkins.Add(new Skin(dir));
                        
                        break;
                    }
        }

        public Skin GetSkin(string pSkinName)
        {
            return AvailableSkins
                .Where(pSkin => string.Compare(pSkin.Name, pSkinName, true) == 0)
                .FirstOrDefault();
        }
    }
}
