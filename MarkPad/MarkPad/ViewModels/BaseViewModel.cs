using PropertyChanged;
using System.ComponentModel;

namespace MarkPad.ViewModels
{
    [AddINotifyPropertyChangedInterface]
    public class BaseViewModel
    {
        public event PropertyChangedEventHandler PropertyChanged = ( sender, e ) => {};
    }
}