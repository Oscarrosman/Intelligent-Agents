using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using NLP;
using NLP.NGrams;

namespace AutocompleteApplication
{
    public partial class MainForm : Form
    {
        public MainForm()
        {
            InitializeComponent();
        }

        // Write this method (and, of course, all other relevant methods, placed in
        // appropriately named classes, placed in a suitable folder.
        //
        // Note: To add a folder, right-click on the project in the Solution Explorer,
        // (e.g., NLP), then select Add - New Folder. 
        // Do NOT add folders externally (outside Visual Studio).
        //
        // Here, no class labels are needed. Instead you simply
        // need to read text and then tokenize it, after which you can generate
        // the n-grams (for n = 1,2, and 3).
        private void loadDataSetToolStripMenuItem_Click(object sender, EventArgs e)
        {

        }

        private void exitToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Application.Exit();
        }

    }
}
