using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using GeneGenie.Gedcom.Parser;
using GeneGenie.Gedcom;

namespace FamilyTree
{
    public partial class FamilyTreeForm : Form
    {
        public FamilyTreeForm()
        {
            InitializeComponent();
        }

        private void FamilyTreeForm_Load(object sender, EventArgs e)
        {

        }

        private void buttonLoadTree_Click(object sender, EventArgs e)
        {
            var gedcomData = GedcomRecordReader.CreateReader("J:\\Family\\dulev2020.ged");
            toolStripLabel.Text = "Loaded " + gedcomData.Database.Count + " records";

            var db = new GedcomDatabase();

            var individual = gedcomData.Database.Individuals.Select(f => f.Names).ToList();

            var ID = 0;
            foreach (var item in individual)
            {
                foreach (var itemPerson in item)
                {
                    ID++;
                    listPersons.Items.Add(new ListViewItem(new string[4] { ID.ToString(), itemPerson.Name, "", "" }));
                }
            }


            //foreach (var item in gedcomData.Database.Individuals)
            //{
            //    foreach (var itemPerson in item.Names)
            //    {
            //        listPersons.Items.Add(new ListViewItem(new string[4] { itemPerson.Name, itemPerson.Surname, "", "" }));

            //    }

            //}
        }
    }
}
