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
        //this is by refrence, appearently
        private TeamEntryData bound_data = new();


        public TeamEntryWindow()
        {
            InitializeComponent();

        }

        public void bind_data(TeamEntryData bound_data) {

            this.bound_data = bound_data;

            team_color_label.Text = this.bound_data.form_label;
            layout_frame.BackColor = this.bound_data.form_color;

        }

        private void team_field_lost_focus(object sender, EventArgs e)
        {
            bound_data.team_name = team_field.Text;
            //throw new NotImplementedException();
        }

    }

    public class TeamEntryData {

        public String team_name = String.Empty;
        public String bot_name = String.Empty;

        public String form_label = String.Empty;
        public Color form_color = Color.White;

    }
}
