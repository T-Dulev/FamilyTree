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

        GedcomRecordReader gedcomData;

        private void buttonLoadTree_Click(object sender, EventArgs e)
        {
            gedcomData = GedcomRecordReader.CreateReader("J:\\Family\\dulev2020.ged");
            toolStripLabel.Text = "Loaded " + gedcomData.Database.Count + " records";

            var individual = gedcomData.Database.Individuals.Select(f => f.Names).ToList();

            var ID = 0;

            foreach (var item in gedcomData.Database.Individuals)
            {
                var parent = MergeList(item.ChildIn);
                string parentFound = "";
                string parentFullName = "";

                var parentID = from db in gedcomData.Database.Families
                               where db.XRefID == parent //&& db.Husband != null
                               select db.Husband;

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
                                parentFullName = item2[0].Given + " " + item2[0].Surname;
                                break;
                            }
                        }
                }
                var res = gedcomData.Database.Individuals.FindAll(x => (x.XRefID == parentFound));

                var father = GetFatherName(parent);

                foreach (var itemPerson in item.Names)
                {
                    ID++;
                    listPersons.Items.Add(new ListViewItem(new string[4] { ID.ToString(), itemPerson.Given + " " + itemPerson.Surname, parent, father }));
                }

            }
        }

        public string MergeList(GedcomRecordList<GedcomFamilyLink> list)
        {
            string res = String.Join(" ", list.Select(f => f.Family));
            return res;
        }

        public string GetFatherName(string parent)
        {
            string parentFullName = "";

            var parentID = from db in gedcomData.Database.Families
                           where db.XRefID == parent //&& db.Husband != null
                           select db.Husband;

            if (parentID.Count() > 0)
            {
                List<string> names = new List<string>();

                if (parentID.Count() > 1)
                {
                    //
                }
                foreach (var item in parentID)
                {
                    var person = GetPerson(item);
                    if (person != null)
                    {
                        foreach (var item2 in person)
                        {
                            if (item2.Names.Count > 0)
                            {
                                names.Add(item2.Names[0].Given + " " + item2.Names[0].Surname);
                            }
                        }
                    }

                }
                parentFullName = String.Join(", ", names);
            }
            return parentFullName;
        }

        public List<GedcomIndividualRecord> GetPerson(string PersonXfer)
        {
            if (PersonXfer != null)
            {
                return gedcomData.Database.Individuals.FindAll(x => (x.XRefID == PersonXfer));
            }
            else
            {
                return null;
            }
        }
    }
}
