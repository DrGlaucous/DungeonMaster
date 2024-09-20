using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace DungeonMaster
{
    public partial class TeamEntryWindow : UserControl
    {
        //this is by refrence, appearently (changing this outside here will change what's seen inside?)
        private TeamEntryData bound_data = new();
        private OpenFileDialog file_picker = new();

        public TeamEntryWindow()
        {
            InitializeComponent();
            file_picker.Filter = "BMP|*.bmp|JPG|*.jpg;*.jpeg|PNG|*.png|TIFF|*.tif;*.tiff|All|*.*";

            //BotImageWindow.SizeMode = PictureBoxSizeMode.StretchImage;
        }

        public void BindData(TeamEntryData bound_data)
        {

            this.bound_data = bound_data;

            team_color_label.Text = this.bound_data.FormLabel;
            layout_frame.BackColor = this.bound_data.FormColor;

            this.bound_data.ImgEventHandler += OnImageUpdate;
            this.bound_data.TxtEventHandler += OnTextUpdate;

        }

        private void team_field_lost_focus(object sender, EventArgs e)
        {
            bound_data.TeamName = team_field.Text;
            //throw new NotImplementedException();
        }

        private void bot_field_lost_focus(object sender, EventArgs e)
        {
            bound_data.BotName = bot_field.Text;
            //throw new NotImplementedException();
        }

        private void browse_button_Click(object sender, EventArgs e)
        {
            if (file_picker.ShowDialog() == DialogResult.OK) {
                bound_data.BotImage = (Bitmap)Bitmap.FromFile(file_picker.FileName);
                //BotImageWindow.Image = bound_data.BotImage;
            }

        }


        //invoked whenever text fields of the bound data change (may not be needed)
        private void OnTextUpdate()
        {
            //note: this window should be the only thing updating this field, so we don't really need this callback here.
        }

        //invoked whenever image fields of the bound data change
        private void OnImageUpdate()
        {
            //update hande from outside threads
            if (BotImageWindow.InvokeRequired)
            {
                BotImageWindow.Invoke(new MethodInvoker(() => { OnImageUpdate(); }));
                return;
            }

            BotImageWindow.Image = bound_data.BotImage;


        }
    
    
    
    }

    public class TeamEntryData {

        private String team_name = String.Empty;
        public String TeamName {
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
        public Color FormColor = Color.White;

        private Bitmap bot_image = new(1, 1);
        public Bitmap BotImage
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
