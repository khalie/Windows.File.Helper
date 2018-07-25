using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;
using Windows.File.Helper.Model;

namespace Windows.File.Helper.ViewModel
{
  public class MainWindowViewModel : INotifyPropertyChanged
  {
    /* What comes next?
       - Edit the Search Criteria and save it to the Folder Object
       - Move instead of Delete

    */
    #region objects
    public ObservableCollection<Folder> Folders { get; private set; }

    #endregion

    #region constructor
    public MainWindowViewModel()
    {
      // Initialize ObservableCollections
      Folders = new ObservableCollection<Folder>(DataStorage.LoadFromFile());

      // Initialize RelayCommands
      _addCommand = new RelayCommand(this.AddFolder, () => true);
      _removeCommand = new RelayCommand(this.RemoveFolder, this.CanRemoveFolder);
      _saveCommand = new RelayCommand(this.SaveFolder, () => true);
      _deleteCommand = new RelayCommand(this.DeleteFiles, this.CanDeleteFiles);
      _moveCommand = new RelayCommand(this.MoveFiles, this.CanMoveFiles);
      _moveAllCommand = new RelayCommand(this.MoveAllFiles, this.CanMoveFiles);

      // Selection on Startup
      SelectedFolder = Folders.FirstOrDefault();
    }

    #endregion

    #region commands

    /// <summary>
    /// Databinding: AddCommand. The Add Command adds a new Folder Object to the Observable Collection Folders
    /// </summary>
    private readonly RelayCommand _addCommand;
    public ICommand AddCommand
    {
      get { return _addCommand; }
    }

    /// <summary>
    /// Databinding: RemoveCommand. The RemoveCommand removes the selected Folder Object from the Observable Collection Folders
    /// </summary>
    private readonly RelayCommand _removeCommand;
    public ICommand RemoveCommand
    {
      get { return _removeCommand; }
    }

    /// <summary>
    /// Databinding: SaveCommand. The SaveCommand saves all Folder Objects to the Folders.txt
    /// </summary>
    private readonly RelayCommand _saveCommand;
    public ICommand SaveCommand
    {
      get { return _saveCommand; }
    }

    /// <summary>
    /// Databinding: DeleteCommand. The DeleteCommand deletes all unwanted Files from the selected Folder.
    /// </summary>
    private readonly RelayCommand _deleteCommand;
    public ICommand DeleteCommand
    {
      get { return _deleteCommand; }
    }

    /// <summary>
    /// Databinding: MoveCommand. The MoveCommand moves all wanted Files from Subfolders to the SelectedFolder.
    /// </summary>
    private readonly RelayCommand _moveCommand;
    public ICommand MoveCommand
    {
      get { return _moveCommand; }
    }

    /// <summary>
    /// Databinding: MoveAllCommand. The MoveCommand moves all Files from Subfolders to the SelectedFolder.
    /// </summary>
    private readonly RelayCommand _moveAllCommand;
    public ICommand MoveAllCommand
    {
      get { return _moveAllCommand; }
    }
    #endregion

    #region methods

    /// <summary>
    /// The Add Method adds a new Folder Object to the Observable Collection Folders
    /// </summary>
    private void AddFolder()
    {
      Folders.Add(new Folder { Name = "New" });
    }

    /// <summary>
    /// The CanRemoveFolder Method checks if a Folder is selected
    /// </summary>
    /// <returns>Returns <bool>true if Folder can be removed</returns>
    private bool CanRemoveFolder()
    {
      return this.SelectedFolder != null;
    }

    /// <summary>
    /// The Remove Method removes the selected Folder Object from the Observable Collection Folders
    /// </summary>
    private void RemoveFolder()
    {
      // Save the Position of the Selected Folder
      int nIndex = Folders.IndexOf(SelectedFolder);

      Folders.Remove(SelectedFolder);

      // In Case we removed the Last Folder, select the next last Folder afterwards, else select the next Folder
      if (Folders.Count() == nIndex)
        SelectedFolder = this.Folders.LastOrDefault();
      else
        SelectedFolder = this.Folders.ElementAt(nIndex);

    }

    /// <summary>
    /// The Save Method saves all Folder Objects to the Folders.txt
    /// </summary>
    private void SaveFolder()
    {
      try
      {
        DataStorage.SaveInFile(Folders.ToArray());
        //MessageBox.Show("Speichern erfolgreich!");
      }
      catch
      {
        throw;
        //MessageBox.Show("Speichern gescheitert!");
      }
    }

    /// <summary>
    /// The CanDeleteFiles Method checks if a Folder is selected
    /// </summary>
    /// <returns>Returns <bool>true if Folder can be removed</returns>
    private bool CanDeleteFiles()
    {
      return this.SelectedFolder != null;
    }

    /// <summary>
    /// Delete defined Filetypes in the selected Folder
    /// </summary>
    private void DeleteFiles()
    {
      try
      {
        // If the Checkbox is checked, Subfolders are included in the Search
        SearchOption searchOption = SearchOption.TopDirectoryOnly;

        if (SelectedFolder.Subfolders)
          searchOption = SearchOption.AllDirectories;

        // Create Testdata
        FileExtension txt = new FileExtension("*.txt");
        FileExtension nfo = new FileExtension("*.nfo");
        FileExtension url = new FileExtension("*.url");
        FileExtension DS_Store = new FileExtension("*.DS_Store");
        FileExtension pdf = new FileExtension("*.pdf");
        FileExtension[] Blacklist = { txt, nfo, url, DS_Store, pdf };

        foreach (FileExtension ext in Blacklist)
        {
          string[] deleteItems = Directory.GetFiles(SelectedFolder.Path, ext.extension, searchOption);

          foreach (string f in deleteItems)
            System.IO.File.Delete(f);
        }

        DeleteEmptyFolders(SelectedFolder.Path);

        //MessageBox.Show("Dateien erfolgreich gelöscht.");
        
      }
      catch (DirectoryNotFoundException dirNotFound)
      {
        MessageBox.Show(dirNotFound.Message);
      }
    }

    /// <summary>
    /// Returns the state of the checkbox "Move from Subfolders"
    /// </summary>
    /// <returns></returns>
    private bool CanMoveFiles()
    {
      try
      {
        return SelectedFolder.MoveFilesFromSubfolders;
      }
      catch (NullReferenceException )
      {
        return false;
      }
      
    }

    /// <summary>
    /// Moves Files from Subfolders to the SelectedFolder
    /// </summary>
    private void MoveFiles()
    {
      if (SelectedFolder.MoveFilesFromSubfolders)
      {
        SearchOption searchOption = SearchOption.AllDirectories;


        // Create Testdata
        FileExtension mkv = new FileExtension("*.mkv");
        FileExtension mp4 = new FileExtension("*.mp4");
        FileExtension avi = new FileExtension("*.avi");
        FileExtension flv = new FileExtension("*.flv");
        FileExtension wmv = new FileExtension("*.wmv");
        FileExtension mpg = new FileExtension("*.mpg");
        FileExtension[] Whitelist = { mkv, mp4, avi, flv, wmv, mpg };

        foreach (FileExtension ext in Whitelist)
        {
          string[] move = Directory.GetFiles(SelectedFolder.Path, ext.extension, searchOption);

          foreach (string f in move)
          {
            string path = SelectedFolder.Path + "\\" + Path.GetFileName(f);
            // File isnt currently at the destination folder
            if (!f.Equals(path))
            {
              // If File does already exist in the destination folder, delete it
              if (System.IO.File.Exists(path))
              {
                System.IO.File.Delete(path);
              }
              System.IO.File.Move(f, path);
            }
          }
        }

        // Delete empty Folders
        DeleteEmptyFolders(SelectedFolder.Path);

      }
    }

    /// <summary>
    /// Deletes Folders that are either empty, or only contain a single preview (.jpg) Image
    /// </summary>
    /// <param name="startpath"></param>
    private void DeleteEmptyFolders(string startpath)
    {
      foreach (String dir in Directory.GetDirectories(startpath))
      {
        DeleteEmptyFolders(dir);
        if (Directory.GetFiles(dir).Length == 0 && Directory.GetDirectories(dir).Length == 0)
        {
          Directory.Delete(dir, false);
        }
        // Delete Folders that only contain one (preview) image
        else if (Directory.GetFiles(dir).Length == 1 && Directory.GetDirectories(dir).Length == 0)
        {
          if (Directory.GetFiles(dir).First().EndsWith(".jpg"))
            Directory.Delete(dir, true);
        }
      }

    }

    private void MoveAllFiles()
    {
      DirectoryInfo dir = new DirectoryInfo(SelectedFolder.Path);
      
      foreach(var folder in dir.GetDirectories())
      {
        MoveAllFiles(folder);
      }
      
    }

    private void MoveAllFiles(DirectoryInfo dir)
    {

      foreach (var folder in dir.GetDirectories())
      {
        MoveAllFiles(folder);
      }
      foreach(var file in dir.GetFiles())
      {

        try
        {
          file.MoveTo(SelectedFolder.Path + "\\" + file.Name);
        }
        catch (Exception)
        {
          // a file with the same name already exists - do nothing
        }
      }
    }

    /*
    private void renameFiles()
    {
      string args = " -rename " + SelectedFolder.Path + " --db TheTVDB --lang de -non-strict";
      try
      {
        //Process process = new Process();
        //process.StartInfo.FileName = @"C:\Users\Peter Henniger\AppData\Roaming\Microsoft\Windows\Start Menu\Programs\FileBot\filebot.exe";
        //process.StartInfo.Arguments = args;
        //process.Start();
        Console.WriteLine("filebot" + args);
      }
      catch (Exception ex)
      {
        throw ex;
      }

    }
    */

    #endregion

    #region properties

    /// <summary>
    /// Property which saves the selected Folder
    /// </summary>
    private Folder _selectedFolder;
    public Folder SelectedFolder
    {
      get
      {
        return _selectedFolder;
      }
      set
      {
        _selectedFolder = value;
        OnPropertyChanged();
        _removeCommand.RaiseCanExecuteChanged();
      }
    }


    public event PropertyChangedEventHandler PropertyChanged;
    protected void OnPropertyChanged([CallerMemberName] string propertyName = null)
    {
      PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
    }
    #endregion
  }
}
