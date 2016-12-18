using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Windows.File.Helper.Model
{
  public class FileExtension
  {
    public string extension { get; set; }


    // Constructor
    public FileExtension (string extension)
    {
      this.extension = extension;
    }

  }
}
