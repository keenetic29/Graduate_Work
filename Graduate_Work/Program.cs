using System;
using System.Threading.Tasks;
using System.Windows.Forms;
using YandexDisk.Client.Http;
using YandexDisk.Client.Protocol;

namespace Graduate_Work
{
  static class Program
  {
    /// <summary>
    /// Главная точка входа для приложения.
    /// </summary>
    [STAThread]
    static void Main()
    {
      //http://localhost:12345/callback#access_token=y0_AgAAAABsexpOAAtyiwAAAAD-PFE4AABRJVxPHOJCTZOfW7I8lWqk_1XBPg&token_type=bearer&expires_in=31536000

      Application.EnableVisualStyles();
      Application.SetCompatibleTextRenderingDefault(false);
      Application.Run(new Form1());
    }
  }
}
