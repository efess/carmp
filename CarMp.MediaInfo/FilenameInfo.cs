using System;
using System.Collections.Generic;
using System.Text;

namespace CarMP.MediaInfo
{
    public class FilenameInfo
    {
        public string Artist {get; set;}
        public string Title { get; set;}
        public string Track { get; set;}

        public void Parse(string pFilename)
        {
            string fileName = pFilename.Replace('_', ' ');
            int result;
            try
            {
                if (Int32.TryParse(fileName.Substring(0, 1), out result))
                {
                    if (fileName.IndexOf(" - ") > 0)
                    {
                        Track = (Int32.TryParse(fileName.Substring(0, fileName.IndexOf("-")), out result)) ? fileName.Substring(0, fileName.IndexOf("-")) : "";
                        if (fileName.IndexOf(" - ") == fileName.LastIndexOf(" - "))
                        {
                            Title = fileName.Substring(fileName.IndexOf(" - ") + 2, fileName.Length - fileName.IndexOf(" - ") - 2);
                        }
                        else
                        {
                            string[] temp;
                            temp = fileName.Substring(0, fileName.Length - 4).Split(new char[] { '-' });
                            Track = temp[0];
                            Artist = temp[1].Trim();
                            Title = temp[2].Trim();
                        }
                    }
                    else
                        Title = fileName.Substring(0, fileName.Length - 4);
                }
                else
                {
                    if (fileName.IndexOf(" - ") > 0)
                    {
                        Artist = fileName.Substring(0, fileName.IndexOf(" - "));
                        Title = fileName.Substring(fileName.LastIndexOf(" - ") + 3, fileName.Length - fileName.LastIndexOf(" - ") - 7);
                    }
                    else
                        if (fileName.IndexOf("-") > 0)
                        {
                            Artist = fileName.Substring(0, fileName.IndexOf("-"));
                            Title = fileName.Substring(fileName.LastIndexOf("-") + 1, fileName.Length - fileName.LastIndexOf("-") - 5);
                        }
                        else
                            Title = fileName.Substring(0, fileName.Length - 4);

                }
            }
            catch(Exception e)
            {
                throw(e);
            }
        }
    }
}
