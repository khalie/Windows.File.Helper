using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Windows.File.Helper.Model
{
  public static class DataStorage
  {
    public static void SaveInFile(Folder[] folders)
    {
      JsonSerializer serializer = new JsonSerializer();
      StringWriter writer = new StringWriter();
      serializer.Serialize(writer, folders);
      System.IO.File.WriteAllText("..\\..\\Folders.txt", writer.ToString());
    }

    public static void SaveBlacklist(FileExtension[] fileExtensions)
    {
      JsonSerializer serializer = new JsonSerializer();
      StringWriter writer = new StringWriter();
      serializer.Serialize(writer, fileExtensions);
      System.IO.File.WriteAllText("..\\..\\Blacklist.txt", writer.ToString());
    }

    public static void SaveWhitelist(FileExtension[] fileExtensions)
    {
      JsonSerializer serializer = new JsonSerializer();
      StringWriter writer = new StringWriter();
      serializer.Serialize(writer, fileExtensions);
      System.IO.File.WriteAllText("..\\..\\Whitelist.txt", writer.ToString());
    }

    public static Folder[] LoadFromFile()
    {
      JsonSerializer serializer = new JsonSerializer();
      StringReader reader = new StringReader(System.IO.File.ReadAllText("..\\..\\Folders.txt"));
      return (Folder[])serializer.Deserialize(reader, typeof(Folder[]));
    }

    public static FileExtension[] LoadBlacklist()
    {
      JsonSerializer serializer = new JsonSerializer();
      StringReader reader = new StringReader(System.IO.File.ReadAllText("..\\..\\Blacklist.txt"));
      return (FileExtension[])serializer.Deserialize(reader, typeof(FileExtension[]));
    }

    public static FileExtension[] LoadWhitelist()
    {
      JsonSerializer serializer = new JsonSerializer();
      StringReader reader = new StringReader(System.IO.File.ReadAllText("..\\..\\Whitelist.txt"));
      return (FileExtension[])serializer.Deserialize(reader, typeof(FileExtension[]));
    }

  }

}

