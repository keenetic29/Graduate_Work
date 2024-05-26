using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace Graduate_Work
{
    public class YandexAPI : IYandex
    {
        private string filePath = "data.json";
        private string AccessToken { get; set; }

        public bool DownloadFile(string Url, string FilePath)
        {
            var request = WebRequest.Create(Url);
            request.Method = "GET";

            try
            {
                using (WebResponse response = request.GetResponse())
                {
                    using (Stream responseStream = response.GetResponseStream())
                    {
                        using (FileStream fileStream = new FileStream(FilePath, FileMode.Create, FileAccess.Write))
                        {
                            byte[] buffer = new byte[4096];
                            int bytesRead;
                            while ((bytesRead = responseStream.Read(buffer, 0, buffer.Length)) > 0)
                            {
                                fileStream.Write(buffer, 0, bytesRead);
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                // Обработка исключений
                return false;
            }
            return true;
        }


        public string GetDownloadUrl(string YandexDir, string FileName)
        {
            var request = YandexDir == "/" ? (WebRequest.Create("https://cloud-api.yandex.net/v1/disk/resources/download?path=/" + FileName+ "&overwrite=true")) :
                (WebRequest.Create("https://cloud-api.yandex.net/v1/disk/resources/download?path=" + YandexDir + '/' + FileName+ "&overwrite=true"));
            var data = JsonConvert.DeserializeObject<dynamic>(File.ReadAllText(filePath));
            AccessToken = data.TextBoxToken?.ToString() ?? "";
            request.Headers["Authorization"] = "OAuth " + AccessToken;
            request.Method = "GET";

            HttpWebResponse response = request.GetResponse() as HttpWebResponse;
            using (Stream responseStream = response.GetResponseStream())
            {
                StreamReader reader = new StreamReader(responseStream, Encoding.UTF8);
                var url = JsonConvert.DeserializeObject<ReceiveURL>(reader.ReadToEnd());
                return url.href;
            }
        }

        public string GetUploadUrl(string YandexDir, string FileName)
        {
            var request = YandexDir == "/" ? (WebRequest.Create("https://cloud-api.yandex.net/v1/disk/resources/upload?path=/" + FileName + "&overwrite=true")) :
                (WebRequest.Create("https://cloud-api.yandex.net/v1/disk/resources/upload?path=" + YandexDir + '/' + FileName + "&overwrite=true"));
            var data = JsonConvert.DeserializeObject<dynamic>(File.ReadAllText(filePath));
            AccessToken = data.TextBoxToken?.ToString() ?? "";
            request.Headers["Authorization"] = "OAuth " + AccessToken;
            request.Method = "GET";

            HttpWebResponse response = request.GetResponse() as HttpWebResponse;
            using (Stream responseStream = response.GetResponseStream())
            {
                StreamReader reader = new StreamReader(responseStream, Encoding.UTF8);
                var url = JsonConvert.DeserializeObject<ReceiveURL>(reader.ReadToEnd());
                return url.href;
            }
        }

        //Отправляем файл на ЯД по указанной ссылке.
        public bool UploadFile(string Url, string FilePath)
        {
            var request = WebRequest.Create(Url);
            request.Method = "PUT";
            request.ContentType = "application/binary";
            try
            {
                using (Stream myReqStream = request.GetRequestStream())
                {
                    using (FileStream myFile = new FileStream(FilePath, FileMode.Open, FileAccess.Read))
                    {
                        using (BinaryReader myReader = new BinaryReader(myFile))
                        {
                            byte[] buffer = myReader.ReadBytes(2048);
                            while (buffer.Length > 0)
                            {
                                myReqStream.Write(buffer, 0, buffer.Length);
                                buffer = myReader.ReadBytes(2048);
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                return false;
            }

            try
            {
                HttpWebResponse resp = (HttpWebResponse)request.GetResponse();
            }
            catch (Exception ex)
            {
                return false;
            }
            return true;
        }


    }
}
