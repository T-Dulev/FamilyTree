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

            //var db = new GedcomDatabase();

            var individual = gedcomData.Database.Individuals.Select(f => f.Names).ToList();

            var ID = 0;
            //foreach (var item in individual)
            //{
            //    foreach (var itemPerson in item)
            //    {
            //        ID++;
            //        listPersons.Items.Add(new ListViewItem(new string[4] { ID.ToString(), itemPerson.Name, "", "" }));
            //    }
            //}


            foreach (var item in gedcomData.Database.Individuals)
            {
                var parent = MergeList(item.ChildIn);
                var parentID = from db in gedcomData.Database.Families
                               where db.XRefID == parent //&& db.Husband != null
                               select db.Husband;

                string parentFound = "";
                string parentFullName = "";
                if (parentID.Count() > 0)
                {
                    parentFound = parentID.First();
                    var parentName = from db in gedcomData.Database.Individuals
                                     where db.XRefID == parentFound && (db.Names != null)
                                     select db.Names;
                    if (parentName.Count() > 0)
                        foreach (var item2 in parentName)
                        {
                            if (item2.Count > 0)
                            {
                                parentFullName = item2.First().Name;
                                break;
                            }
                        }
                }
                var res = gedcomData.Database.Individuals.FindAll(x => (x.XRefID == parentFound));

                foreach (var itemPerson in item.Names)
                {
                    ID++;
                    listPersons.Items.Add(new ListViewItem(new string[4] { ID.ToString(), itemPerson.Name, parent, parentFullName }));

                }

            }
        }

        public string MergeList(GedcomRecordList<GedcomFamilyLink> list)
        {
            string res = String.Join(" ", list.Select(f => f.Family));
            //foreach (var item in list)
            //{
            //    item.Individual
            //}
            return res;
        }
    }
}
