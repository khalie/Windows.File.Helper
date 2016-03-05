using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;

namespace Windows.File.Helper.Model
{
  public class Folder
  {
    /// <summary>
    /// DisplayName for the saved Folder Path
    /// </summary>
    public string Name { get; set; }

    /// <summary>
    /// Contains the shown Path with @"Path" extra Chars for Commands
    /// </summary>
    public string PathForCommand { get; private set; }

    /// <summary>
    /// Contains the shown Path
    /// </summary>
    public string Path { get; set; }

    /// <summary>
    /// Include Subfolders in this Path
    /// </summary>
    public bool Subfolders { get; set; }
  }
}
