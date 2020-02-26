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
using System.IO;

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
            // correct surnames from mariname
            var text = File.ReadAllLines("J:\\Family\\dulev2020.ged",Encoding.Default);
            for (int i = 0; i < text.Length; i++)
            {
                if (text[i].Contains("//"))
                {
                    if (text[i + 2].Contains("_MARNM"))
                    {
                        var surname = text[i + 2].Substring(9);
                        text[i]=text[i].Replace("//", "/" + surname + "/");
                    }
                }
            }
            File.WriteAllLines("J:\\Family\\dulev2020.txt",text,Encoding.Default);

            gedcomData = GedcomRecordReader.CreateReader("J:\\Family\\dulev2020.txt");
            toolStripLabel.Text = "Loaded " + gedcomData.Database.Count + " records";

            var individual = gedcomData.Database.Individuals.Select(f => f.Names).ToList();

            var ID = 0;
            foreach (var item in gedcomData.Database.Individuals)
            {
                //string parentFound = "";
                //string parentFullName = "";

                List<string> fathers = new List<string>();

                foreach (var parent in item.ChildIn)
                {
                    //var parent = MergeList(item.ChildIn);
                    //var parentID = from db in gedcomData.Database.Families
                    //               where db.XRefID == parent.Family
                    //               select db.Husband;

                    //if (parentID.Count() > 0)
                    //{
                    //    parentFound = parentID.First();
                    //    var parentName = from db in gedcomData.Database.Individuals
                    //                     where db.XRefID == parentFound && (db.Names != null)
                    //                     select db.Names;
                    //    if (parentName.Count() > 0)
                    //        foreach (var item2 in parentName)
                    //        {
                    //            if (item2.Count > 0)
                    //            {
                    //                parentFullName = item2[0].Given + " " + item2[0].Surname;
                    //                break;
                    //            }
                    //        }
                    //}
                    //var res = gedcomData.Database.Individuals.FindAll(x => (x.XRefID == parentFound));

                    fathers.Add(GetFatherName(parent.Family));
                    fathers.Add(GetMotherName(parent.Family));
                }


                foreach (var itemPerson in item.Names)
                {
                    ID++;
                    listPersons.Items.Add(new ListViewItem(new string[4] { ID.ToString(), itemPerson.Given + " " + itemPerson.Surname, String.Join(", ", fathers), "" }));
                }

            }

            foreach (var item in gedcomData.Database.Families)
            {
                List<string> names = new List<string>();

                var husband = GetPerson(item.Husband);
                if (husband != null)
                {
                    foreach (var itemPerson in husband[0].Names)
                    {
                        //names.Add(husband[0].Names[0].Given + " " + husband[0].Names[0].Surname);
                        names.Add(itemPerson.Given + " " + itemPerson.Surname);
                    }
                }
                var wife = GetPerson(item.Wife);
                if (wife != null)
                {
                    //names.Add(wife[0].Names[0].Given + " " + wife[0].Names[0].Surname);
                    foreach (var itemPerson in wife[0].Names)
                    {
                        names.Add(itemPerson.Given + " " + itemPerson.Surname);
                    }

                }
                //names.Add(GetFatherName(item.XRefID));
                //names.Add(GetMotherName(item.XRefID)); 
                ID++;
                listFamily.Items.Add(new ListViewItem(new string[4] { ID.ToString(), String.Join(", ", names), "", "" }));
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
        public string GetMotherName(string parent)
        {
            string parentFullName = "";

            var parentID = from db in gedcomData.Database.Families
                           where db.XRefID == parent
                           select db.Wife;

            if (parentID.Count() > 0)
            {
                List<string> names = new List<string>();
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
