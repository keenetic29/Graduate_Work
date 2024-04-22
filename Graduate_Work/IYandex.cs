using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Graduate_Work
{
    interface IYandex
    {
        string GetUploadUrl(string YandexDir, string FileName);
        bool UploadFile(string Url, string FilePath);
        string GetDownloadUrl(string YandexDir, string FileName);
        bool DownloadFile(string Url, string FilePath);
    }
}
