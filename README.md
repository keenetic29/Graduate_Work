# Программа для безопасного хранения файлов в облачном хранилище (Яндекс Диск)
![image](https://github.com/keenetic29/Graduate_Work/assets/122115141/8036409f-99d6-4d4d-9874-be67c119616f)

Данная программа позволяет взаимодействовать с Яндекс Диском через REST API.
### Возможности:
- Просмотр структуры хранилища любой степени вложенности
- Загрузка файлов на Диск с предварительным шифрованием
- Скачивание файлов на локальный диск пользователя с последующим расшифрованием
## Предварительная работа для взаимодействия с API
Яндекс Диск позволяет управлять ресурсами Диска (файлами и папками) посредством HTTP-запросов REST API.
API Диска предназначен для приложений, которые работают с файлами пользователей Яндекс Диска или хранят на Диске собственные файлы и настройки. 

Таким образом, необходимо зарегистрировать приложение, выдав соответствующие права для доступа к диску. 
- Запись в любом месте на Диске — cloud_api:disk.write;
- Чтение всего Диска — cloud_api:disk.read;
- Доступ к папке приложения на Диске — cloud_api:disk.app_folder;
- Доступ к информации о Диске — cloud_api:disk.info.
![image](https://github.com/keenetic29/Graduate_Work/assets/122115141/aba6e15b-8106-4796-8c49-5b87a08fbaa7)

После чего нужно получить OAuth-токен. Яндекс Диск авторизует приложения с помощью OAuth-токенов. 
Каждый токен предоставляет определенному приложению доступ к данным определенного пользователя.

## Алгоритм шифрования RC5
RC5 – симметричный алгоритм блочного шифрования.
Часть основных параметров алгоритма RC5 являются переменными. Помимо секретного ключа, параметрами алгоритма являются следующие:
1. Размер слова w (в битах); RC5 шифрует блоками по два слова; допустимыми значениями w являются 16, 32 или 64;
2. Количество раундов алгоритма R — в качестве значения допустимо любое целое число от 0 до 255 включительно;
3. Размер секретного ключа в байтах b - любое целое значение от 0 до 255 включительно.

Под раундом подразумевается преобразования, соответствующее двум раундам обычных алгоритмов, сконструированных на основе сетей Фейстеля. За раунд RC5 обрабатывает блок целиком, в отличии от раунда сети Фейстеля обрабатывающего половину блока.

![image](https://github.com/keenetic29/Graduate_Work/assets/122115141/f4d96e1f-fea7-49ab-8427-1de87fb833f8)

## Блок схемы ПО
![image](https://github.com/keenetic29/Graduate_Work/assets/122115141/334e26b2-7ab5-48da-8fc8-95ff58097ef8)

## Демонстрация модуля загрузки файлов
1. Если пользователь хочет загрузить файл на Диск, ему необходимо нажать на кнопку "Загрузить" в верхней части панели, после чего подгрузится структура Яндекс Диска.

![image](https://github.com/keenetic29/Graduate_Work/assets/122115141/e5b08444-4549-4931-ad73-459fa9f62ace)

Кусочек кода из `Form1.cs`:
```csharp
private async void UploadToolStripMenuItem_Click(object sender, EventArgs e)
{
      textBox1.Text = "Для шифрования и загрузки файла в облачное хранилище необходимо выбрать папку в структуре Яндекс Диска, " +
        "в которую будет загружен файл. После чего нажать на кнопку 'Загрузить'. Выбранный файл автоматически шифруется, затем " +
        "загружается в облачное хранилище.";
      button1.Text = "Загрузить";
      button1.Visible = true;
      //treeView1.Nodes.Clear();
      menuStatus = "upload";

      //YandexDiskExplorer yandexDiskExplorer = new YandexDiskExplorer();
      //treeView1.Nodes.Add(yandexDiskExplorer.GetYandexDiskStructure());

      await Task.Run(() =>
      {
          YandexDiskExplorer yandexDiskExplorer = new YandexDiskExplorer();
          var yandexDiskStructure = yandexDiskExplorer.GetYandexDiskStructure();

          // Используем Control.Invoke для обновления UI из фонового потока
          pBox.Invoke(new Action(() =>
          {
              treeView1.Nodes.Clear();
              treeView1.Nodes.Add(yandexDiskStructure);
          }));
      });
}
```

Кусочек кода из `YandexDiskExplorer.cs` класса `YandexDiskExplorer` (построение структуры диска):
```csharp
public TreeNode GetYandexDiskStructure(string yandexDir = "/")
{
      var rootNode = new TreeNode(yandexDir);
      FillYandexDiskStructure(rootNode, yandexDir);
      return rootNode;
}

private void FillYandexDiskStructure(TreeNode parentNode, string yandexDir)
{
      var resources = GetResources(yandexDir);
      foreach (var resource in resources)
      {
          var node = new TreeNode(resource.Name);
          if (resource.Type == "dir")
          {
              FillYandexDiskStructure(node, resource.Path);
          }
          parentNode.Nodes.Add(node);
      }
}

private List<Resource> GetResources(string yandexDir)
{
      var request = WebRequest.Create($"https://cloud-api.yandex.net/v1/disk/resources?path={yandexDir}");
      request.Headers["Authorization"] = "OAuth " + AccessToken;
      request.Method = "GET";

      using (var response = request.GetResponse())
      using (var stream = response.GetResponseStream())
      using (var reader = new StreamReader(stream, Encoding.UTF8))
      {
          var json = reader.ReadToEnd();
          var resourcesResponse = JsonConvert.DeserializeObject<ResourcesResponse>(json);
          return resourcesResponse._embedded.items;
      }
}
 ```  
2. Чтобы загрузить файл, необходимо выбрать папку в структуре Диска, в которую будет помещен файл.

![image](https://github.com/keenetic29/Graduate_Work/assets/122115141/cbd30316-faf8-43a3-8332-d8a3c62a0fc7)

3. Далее необходимо выбрать сам файл, который необходимо загрузить, нажав на кнопку "Загрузить" в нижней части панели.

![image](https://github.com/keenetic29/Graduate_Work/assets/122115141/8a8c33d0-5be6-47f0-b976-460d0b36bd13)

Кусочек кода из `Form1.cs`:
```csharp
private async void button1_Click(object sender, EventArgs e)
{
   if (menuStatus == "download")
   {
      // чтобы не нагромождать, данный блок кода опущен
   }
    else if (menuStatus == "upload")
    {
         if (openFileDialog1.ShowDialog() == DialogResult.OK)
         {
               if (treeView1.SelectedNode != null /* && проверка что эта нода папка, а не файл*/)
               {
                        YandexAPI yandexAPI = new YandexAPI();
                        TreeNode selectedNode = treeView1.SelectedNode;
                        var YandexDir = selectedNode.FullPath;
                        YandexDir = YandexDir.Substring(1, YandexDir.Length - 1);
                        YandexDir = YandexDir.Replace('\\', '/');

                        // получаем выбранный файл
                        fileName = Path.GetFileName(openFileDialog1.FileName);
                        path = openFileDialog1.FileName;

                        byte[] key = new byte[] { 0x01, 0x02, 0x03, 0x04, 0x05, 0x06, 0x07, 0x08, 0x09, 0x0A, 0x0B, 0x0C, 0x0D, 0x0E, 0x0F, 0x10 }; // Пример ключа, будет в дальнейшем извлекаться
                                                                                                                                                   // из полей формы Form2, как и другие параметры алгоритма
                        RC5FileProcessor processor = new RC5FileProcessor(key);
                        string encryptPath = Path.GetDirectoryName(fileName)+'_'+fileName;
                        processor.EncryptFile(path, encryptPath);

                        await Task.Run(() =>
                        {
                            yandexAPI.UploadFile(yandexAPI.GetUploadUrl(YandexDir, '_'+fileName), encryptPath);

                        });
               }
               else
               {
                   MessageBox.Show("Не выбрана папка для загрузки");
               }
                   
         }

    }


}
```

Кусочек кода из `YandexAPI.cs` (получение ссылки на загрузку):
```csharp
public string GetUploadUrl(string YandexDir, string FileName)
{
      var request = YandexDir == "/" ? (WebRequest.Create("https://cloud-api.yandex.net/v1/disk/resources/upload?path=/" + FileName)) : (WebRequest.Create("https://cloud-api.yandex.net/v1/disk/resources/upload?path=" + YandexDir + '/' + FileName));
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
```

Кусочек кода из `YandexAPI.cs` (загрузка файла по ссылке):
```csharp
//Отправляем файл на ЯД по указанной ссылке.
public bool UploadFile(string Url, string FilePath)
{
      //Console.WriteLine(Url);
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
```

4. Подождав определенное время, которое занимает шифрование и загрузка файла на диск, можно убедиться, что файл действительно загружен, обновив структуру диска, нажав на значок обновления правее от структуры.

![image](https://github.com/keenetic29/Graduate_Work/assets/122115141/4f7477ad-7822-4973-a1be-8d730929f20e)

Кусочек кода из `Form1.cs` (обновление структуры диска):
```csharp
private async void pictureBox1_Click(object sender, EventArgs e)
{
            await Task.Run(() =>
            {
                YandexDiskExplorer yandexDiskExplorer = new YandexDiskExplorer();
                var yandexDiskStructure = yandexDiskExplorer.GetYandexDiskStructure();

                // Используем Control.Invoke для обновления UI из фонового потока
                pBox.Invoke(new Action(() =>
                {
                    treeView1.Nodes.Clear();
                    treeView1.Nodes.Add(yandexDiskStructure);
                }));
            });
}
```

Кусочек кода из `RC5.cs`, класс `RC5FileProcessor` (запуск процесса шифрования):
```csharp
public void EncryptFile(string inputFilePath, string outputFilePath)
{
      using (var inputStream = new FileStream(inputFilePath, FileMode.Open, FileAccess.Read))
      using (var outputStream = new FileStream(outputFilePath, FileMode.Create, FileAccess.Write))
      {
          byte[] buffer = new byte[16]; //byte = 8 бит. Размер блока для RC5 (2 слова по 64 бита) = 128бит = 8 * 16
          int bytesRead;
          while ((bytesRead = inputStream.Read(buffer, 0, buffer.Length)) > 0)
          {
              byte[] encryptedData = new byte[buffer.Length];
              rc5.Cipher(buffer, encryptedData); 
              outputStream.Write(encryptedData, 0, buffer.Length);
          }
      }
}
```

Кусочек кода из `RC5.cs`, класс `RС5` (метод шифрования):
```csharp
/* 
* Операция шифрования
* inBuf: входной буфер для шифруемых данных (64 бита)
* outBuf: выходной буфер (64 бита)
*/
public void Cipher(byte[] inBuf, byte[] outBuf)
{
   UInt64 a = BytesToUInt64(inBuf, 0);
   UInt64 b = BytesToUInt64(inBuf, 8);

   a = a + S[0];
   b = b + S[1];

   for (int i = 1; i < R + 1; i++)
   {
       a = ROL((a ^ b), (int)b) + S[2 * i];
       b = ROL((b ^ a), (int)a) + S[2 * i + 1];
   }

   UInt64ToBytes(a, outBuf, 0);
   UInt64ToBytes(b, outBuf, 8);
}
```  
5. В том, что файл действительно загрузился, можно также убедиться, перейдя непосредственно в сам Яндекс Диск.

![image](https://github.com/keenetic29/Graduate_Work/assets/122115141/bdfb67a8-51cd-4e9a-8b03-1e9e82bf7e0a)

6. При попытке предварительного открытия и просмотра файла, можно понять, что файл действительно зашифрован.  

![image](https://github.com/keenetic29/Graduate_Work/assets/122115141/de2ee28a-6793-4316-8a25-590879c4641d)


## Демонстрация модуля скачивания файлов
1. Если пользователь хочет скачать файл из Яндекс Диска к себе на локальный диск, ему необходимо нажать на кнопку "Скачать" в верхней части панели, после чего подгрузится структура Яндекс Диска.

![image](https://github.com/keenetic29/Graduate_Work/assets/122115141/c07a800b-cc27-4c86-858a-3081833df065)

Кусочек кода из `Form1.cs`:
```csharp
private async void DownloadToolStripMenuItem_Click(object sender, EventArgs e)
{
      textBox1.Text = "Для расшифровки и загрузки файла на локальный диск необходимо" +
        " выбрать интересующий Вас файл в структуре Яндекс Диска и нажать на кнопку 'Скачать'.";
      button1.Text = "Скачать";
      button1.Visible = true;
      //treeView1.Nodes.Clear();
      menuStatus = "download";

      //YandexDiskExplorer yandexDiskExplorer = new YandexDiskExplorer();
      //treeView1.Nodes.Add(yandexDiskExplorer.GetYandexDiskStructure());

      await Task.Run(() =>
      {
          YandexDiskExplorer yandexDiskExplorer = new YandexDiskExplorer();
          var yandexDiskStructure = yandexDiskExplorer.GetYandexDiskStructure();

          // Используем Control.Invoke для обновления UI из фонового потока
          pBox.Invoke(new Action(() =>
          {
              treeView1.Nodes.Clear();
              treeView1.Nodes.Add(yandexDiskStructure);
          }));
      });

}
```
2. Чтобы скачать файл, необходимо выбрать интересующий нас файл в структуре Диска.

![image](https://github.com/keenetic29/Graduate_Work/assets/122115141/ac4225c3-19eb-4481-9db6-2613c91e643e)

3. Далее необходимо выбрать директорию, куда будет помещен скаченный файл, нажав на кнопку "Скачать" в нижней части панели.

![image](https://github.com/keenetic29/Graduate_Work/assets/122115141/dc49eb6e-3f37-4b79-9326-579df3a18a8d)

Кусочек кода из `Form1.cs`:
```csharp
private async void button1_Click(object sender, EventArgs e)
{
      if (menuStatus == "download")
      {
             FolderBrowserDialog FBD = new FolderBrowserDialog();
             FBD.ShowNewFolderButton = false;
             if (FBD.ShowDialog() == DialogResult.OK)
             {
                 if (treeView1.SelectedNode != null)
                 {
                     // Выбранная нода
                     TreeNode selectedNode = treeView1.SelectedNode;
                     var fileName = selectedNode.Text;
                     var filePath = (selectedNode.Parent).FullPath;
                     filePath = filePath.Substring(1, filePath.Length-1);
                     filePath = filePath.Replace('\\', '/');
                     path = FBD.SelectedPath + '\\' + fileName;


                     //создание апи, метод скачивания
                     YandexAPI yandexAPI = new YandexAPI();
                     await Task.Run(() =>
                     {
                         yandexAPI.DownloadFile(yandexAPI.GetDownloadUrl(filePath, fileName), path);

                         byte[] key = new byte[] { 0x01, 0x02, 0x03, 0x04, 0x05, 0x06, 0x07, 0x08, 0x09, 0x0A, 0x0B, 0x0C, 0x0D, 0x0E, 0x0F, 0x10 }; // Пример ключа, будет в дальнейшем извлекаться
                                                                                                                                                   // из полей формы Form2, как и другие параметры алгоритма
                         RC5FileProcessor processor = new RC5FileProcessor(key);
                         string decryptedFileName = fileName.Substring(1);
                         string decryptedPath = FBD.SelectedPath + '\\' + decryptedFileName;
                         processor.DecryptFile(path, decryptedPath);

                     });

                 }
                 else
                 {
                     MessageBox.Show("Не выбран файл для скачивания");
                 }
             }

                
      }
      else if (menuStatus == "upload")
      {
            // чтобы не нагромождать код, данный блок кода опущен       
      }

}
```

Кусочек кода из `YandexAPI.cs` (получение ссылки на скачивание файла):
```csharp
public string GetDownloadUrl(string YandexDir, string FileName)
{
      var request = YandexDir == "/" ? (WebRequest.Create("https://cloud-api.yandex.net/v1/disk/resources/download?path=/" + FileName)) : (WebRequest.Create("https://cloud-api.yandex.net/v1/disk/resources/download?path=" + YandexDir + '/' + FileName));
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
```

Кусочек кода из `YandexAPI.cs` (скачивание файла по ссылке):
```csharp
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
``` 


Кусочек кода из `RC5.cs`, класс `RC5FileProcessor` (запуск процесса расшифрования):
```csharp
public void DecryptFile(string inputFilePath, string outputFilePath)
{
      using (var inputStream = new FileStream(inputFilePath, FileMode.Open, FileAccess.Read))
      using (var outputStream = new FileStream(outputFilePath, FileMode.Create, FileAccess.Write))
      {
          byte[] buffer = new byte[16]; // byte = 8 бит. Размер блока для RC5 (2 слова по 64 бита) = 128бит = 8 * 16
          int bytesRead;
          while ((bytesRead = inputStream.Read(buffer, 0, buffer.Length)) > 0)
          {
              byte[] decryptedData = new byte[buffer.Length];
              rc5.Decipher(buffer, decryptedData);
              outputStream.Write(decryptedData, 0, buffer.Length);
          }
      }
}
```

Кусочек кода из `RC5.cs`, класс `RС5` (метод расшифрования):
```csharp
/*
* Операция расшифрования
* inBuf: входной буфер для шифруемых данных (64 бита)
* outBuf: выходной буфер (64 бита)
*/
public void Decipher(byte[] inBuf, byte[] outBuf)
{
   UInt64 a = BytesToUInt64(inBuf, 0);
   UInt64 b = BytesToUInt64(inBuf, 8);

   for (int i = R; i > 0; i--)
   {
       b = ROR((b - S[2 * i + 1]), (int)a) ^ a;
       a = ROR((a - S[2 * i]), (int)b) ^ b;
   }

   b = b - S[1];
   a = a - S[0];

   UInt64ToBytes(a, outBuf, 0);
   UInt64ToBytes(b, outBuf, 8);
}
```  

4. Открыв выбранную папку и подождав некоторое время, можно убедиться, что файл действительно скачен.

![image](https://github.com/keenetic29/Graduate_Work/assets/122115141/e1ac1066-06c6-47c4-b370-c8f38d14b1f9)


5. При попытке открытия и просмотра файла, можно понять, что файл действительно расшифрован.

![image](https://github.com/keenetic29/Graduate_Work/assets/122115141/4248fbdb-e20c-40d9-8518-3588630a9c4a)




