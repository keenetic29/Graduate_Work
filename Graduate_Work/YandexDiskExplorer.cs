using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using Newtonsoft.Json;

namespace Graduate_Work
{
    public class YandexDiskExplorer
    {
        static string AccessToken = "y0_AgAAAABsexpOAAtyiwAAAAD-PFE4AABRJVxPHOJCTZOfW7I8lWqk_1XBPg";

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
    }


    public class ResourcesResponse
    {
        public Embedded _embedded { get; set; }
    }

    public class Resource
    {
        public string Name { get; set; }
        public string Type { get; set; }
        public string Path { get; set; }
    }

    public class Embedded
    {
        public List<Resource> items { get; set; }
    }
}
