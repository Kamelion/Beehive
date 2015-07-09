using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace Ch13exer4AdvancedBeehiveOverhaul
{
    public partial class FieldForm : Form
    {
        public Renderer Renderer { get; set; }

        public FieldForm()
        {
            InitializeComponent();
        }

        private void FieldForm_MouseClick(object sender, MouseEventArgs e)
        {
            MessageBox.Show(e.Location.ToString());
        }

        private void FieldForm_Paint(object sender, PaintEventArgs e)
        {
            Renderer.PaintField(e.Graphics);
        }
    }
}
