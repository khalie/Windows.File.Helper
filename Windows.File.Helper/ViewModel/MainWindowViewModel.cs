using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
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

      // Add Test Data if needed
      //UseTestData();

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
    #endregion

    #region methods
    /// <summary>
    /// Adds Testdata, if needed.
    /// </summary>
    private void UseTestData()
    {
      Folders.Add(new Folder { Name = "Downloads", Path = "G:\\Test" });
    }

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
        MessageBox.Show("Speichern erfolgreich!");
      }
      catch
      { 
       MessageBox.Show("Speichern gescheitert!");
      }
    }

    

    


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
      if (PropertyChanged != null)
      {
        PropertyChanged(this, new PropertyChangedEventArgs(propertyName));
      }
    }
    #endregion
  }
}
