using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace DungeonMaster
{
    /// <summary>
    /// Interaction logic for TeamEntryWindow.xaml
    /// </summary>
    public partial class TeamEntryWindow : UserControl
    {
        //this is by refrence, appearently (changing this outside here will change what's seen inside?)
        private TeamEntryData BoundData = new();


        private readonly OpenFileDialog FilePicker = new();

        public TeamEntryWindow()
        {
            InitializeComponent();

            FilePicker.Filter = "Supported|*.bmp;*.jpg;*.jpeg;*.png;*.tif;*.tiff|BMP|*.bmp|JPG|*.jpg;*.jpeg|PNG|*.png|TIFF|*.tif;*.tiff|All|*.*";

        }

        public void BindData(TeamEntryData bound_data)
        {

            this.BoundData = bound_data;

            ColorLabel.Content = this.BoundData.FormLabel;

            this.BoundData.ImgEventHandler += OnImageUpdate;
            this.BoundData.TxtEventHandler += OnTextUpdate;

        }

        private void TeamFieldLostFocus(object sender, RoutedEventArgs e)
        {
            BoundData.TeamName = TeamField.Text;
            //throw new NotImplementedException();
        }

        private void BotFieldLostFocus(object sender, RoutedEventArgs e)
        {
            BoundData.BotName = BotField.Text;
            //throw new NotImplementedException();
        }

        private void BrowseButtonClick(object sender, RoutedEventArgs e)
        {
            var result = FilePicker.ShowDialog();
            if (result.HasValue? result.Value : false)
            {
                BoundData.BotImage = new BitmapImage(new Uri(FilePicker.FileName)); //.FromFile(file_picker.FileName);
                //BotImageWindow.Image = bound_data.BotImage;
            }

        }


        //invoked whenever text fields of the bound data change (may not be needed)
        private void OnTextUpdate()
        {
            //note: this window should be the only thing updating this field, so we don't really need this callback here.
        }

        //invoked whenever image fields of the bound data change
        delegate void ParametrizedMethodInvoker();
        private void OnImageUpdate()
        {
            //update hande from outside threads
            if (!Dispatcher.CheckAccess())
            {
                Dispatcher.Invoke(new ParametrizedMethodInvoker(OnImageUpdate));
                return;
            }

            BotImageWindow.Source = BoundData.BotImage;

        }

    }

    //holds info like team name and image, and runs callbacks whenever those fields are updated
    public class TeamEntryData
    {

        private String team_name = String.Empty;
        public String TeamName
        {
            get
            {
                return team_name;
            }

            set
            {
                team_name = value;
                TxtEventHandler?.Invoke();
            }
        }


        private String bot_name = String.Empty;
        public String BotName
        {
            get
            {
                return bot_name;
            }

            set
            {
                bot_name = value;
                TxtEventHandler?.Invoke();
            }
        }
        
        
        public String FormLabel = String.Empty;
        //public Color FormColor = Color.FromRgb(255, 255, 255);

        private BitmapImage bot_image = new();
        public BitmapImage BotImage
        {
            get
            {
                return bot_image;
            }
            set
            {
                bot_image = value;
                ImgEventHandler?.Invoke();
            }
        }

        //runs these callbacks whenever something inside the class has changed
        public delegate void OnTextUpdateEventHandler();
        public event OnTextUpdateEventHandler? TxtEventHandler;

        public delegate void OnImageUpdateEventHandler();
        public event OnImageUpdateEventHandler? ImgEventHandler;

    }

}
