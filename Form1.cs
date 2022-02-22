using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Data.SqlClient;

namespace GestionDesCommand
{
    public partial class Form1 : Form
    {
        SqlConnection cn = new SqlConnection(@"Data Source=.;Initial Catalog=Vente;Integrated Security=True");
        SqlCommand cmd = new SqlCommand();
        SqlDataReader dr;
        public Form1()
        {
            InitializeComponent();
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            try
            {
                cn.Open();
                //remplaire combobox des clients

                cmd.CommandText = "select CodeCl from Client";
                cmd.Connection = cn;
                
                dr = cmd.ExecuteReader();
                while (dr.Read())
                {
                    combo_Cl.Items.Add(dr[0].ToString());
                }
                dr.Close();

                //remplaire combobox des articles
                cmd.CommandText = "select CodeArt from Article";
                cmd.Connection = cn;
                
                dr = cmd.ExecuteReader();
                while (dr.Read())
                {
                    combo_CodeArt.Items.Add(dr[0].ToString());
                }
                dr.Close();
                //aficher numero de command
                cmd.CommandText = "select top 1 NumCom from Commande order by NumCom desc";
                cmd.Connection = cn;
                dr = cmd.ExecuteReader();
                int NC=0;
                //0 if there is no preverent command in DB
                if (dr.Read())
                {
                    NC = int.Parse(dr[0].ToString());
                }
                text_Ncmd.Text = (NC+1).ToString();
            }
            catch(Exception ex)
            {
                
                MessageBox.Show(ex.Message);

            }
            finally
            {
                cn.Close();
            }

        }

        private void combo_Cl_SelectionChangeCommitted(object sender, EventArgs e)
        {
           
            
        }

        private void combo_Cl_SelectedIndexChanged(object sender, EventArgs e)
        {
            try
            {
                cn.Open();
                cmd.CommandText = "select Nom,Ville from Client where CodeCl=@a";
                cmd.Parameters.Clear();
                cmd.Parameters.AddWithValue("a", combo_Cl.SelectedItem);
                cmd.Connection = cn;

                dr = cmd.ExecuteReader();
                if (dr.Read())
                {

                    textNom.Text = (string)dr[0];
                    textVille.Text = (string)dr[1];

                }
                dr.Close();

            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
            finally
            {
                cn.Close();
            }

        }

        private void combo_CodeArt_SelectedIndexChanged(object sender, EventArgs e)
        {
            try
            {
                cn.Open();
                cmd.CommandText = "select Désignation, PU from Article where CodeArt=@a";
                cmd.Parameters.Clear();
                cmd.Parameters.AddWithValue("a", combo_CodeArt.Text);
                cmd.Connection= cn;
                
                dr = cmd.ExecuteReader();
                if (dr.Read())
                {
                    text_Desin.Text = dr[0].ToString();
                    text_PU.Text = dr[1].ToString();
                }
                dr.Close();

            }
            catch(Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
            finally
            {
                cn.Close();
            }
        }

        private void gunaGradientButton1_Click(object sender, EventArgs e)
        {
            try
            {
                if (text_QT.Text == "")
                {
                    MessageBox.Show("tu peut sasir la quantite S.V.P");
                    return;
                }
                cn.Open();
                cmd.CommandText = "select QStock from Article where CodeArt=@a";
                cmd.Parameters.Clear();
                cmd.Parameters.AddWithValue("a", combo_CodeArt.Text);
                cmd.Connection = cn;
                
                int QS = (int)cmd.ExecuteScalar();
                int QT = int.Parse(text_QT.Text);
                if (QS >= QT)
                {
                    double Mnt = int.Parse(text_QT.Text) * double.Parse(text_PU.Text);
                    dgv1.Rows.Add(combo_CodeArt.Text, text_Desin.Text, text_PU.Text, QT.ToString(), Mnt.ToString());
                    double Total;
                    if (text_Total.Text == "")
                    {
                        Total = Mnt;
                    }
                    else
                    {
                        Total = double.Parse(text_Total.Text) + Mnt;
                    }
                    text_Total.Text = Total.ToString();
                }
                else
                {
                    lb_msg.Text = "Stock insuffisant !";
                }
                combo_Cl.Enabled=false;
            }
            catch(Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
            finally
            {
                cn.Close();
            }
        }

        private void gunaGradientButton2_Click(object sender, EventArgs e)
        {
            try
            {
                int idx = dgv1.CurrentCell.RowIndex;
                double Mnt = double.Parse(dgv1.Rows[idx].Cells[4].Value.ToString());
                dgv1.Rows.RemoveAt(idx);
                double Total = double.Parse(text_Total.Text) - Mnt;
                if (Total == 0)
                {
                    text_Total.Text = "";
                }
                else
                {
                    text_Total.Text = Total.ToString();
                }

            }
            catch(Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
            finally
            {
                cn.Close();
            }
        }

        private void gunaGradientButton4_Click(object sender, EventArgs e)
        {
            try
            {
                if (dgv1.Rows.Count > 0)
                {
                    cn.Open();
                    //enregester command 
                    cmd.CommandText = "insert into Commande values(@b,@c)";
                    cmd.Parameters.Clear();
                    //numero de commande est auto increment
                    cmd.Parameters.AddWithValue("b", date_cmd.Value.ToShortDateString());
                    cmd.Parameters.AddWithValue("c", combo_Cl.Text);
                    cmd.Connection = cn;
                    cmd.ExecuteNonQuery();
                    MessageBox.Show("insert 1");
                    //datail command
                    foreach (DataGridViewRow r in dgv1.Rows)
                    {
                        int Qte = int.Parse(r.Cells[3].Value.ToString());
                        int CodeArt = int.Parse(r.Cells[0].Value.ToString());
                        cmd.CommandText = "insert into Détail values(@a,@b,@c)";
                        cmd.Parameters.Clear();
                        cmd.Parameters.AddWithValue("a", text_Ncmd.Text);
                        cmd.Parameters.AddWithValue("b", CodeArt);
                        cmd.Parameters.AddWithValue("c", Qte);
                        cmd.Connection = cn;
                        cmd.ExecuteNonQuery();
                        //mise a jour stock
                        cmd.CommandText = "update Article set QStock=QStock-@a";
                        cmd.Parameters.Clear();
                        cmd.Parameters.AddWithValue("a", Qte);
                        cmd.ExecuteNonQuery();
                    }
                    //suprimer la formula
                    textNom.Text = textVille.Text = text_Desin.Text = "";
                    text_PU.Text = text_QT.Text = text_Total.Text = lb_msg.Text = "";
                    combo_Cl.Text = combo_CodeArt.Text = "";
                    date_cmd.Value = DateTime.Now;
                    int nc = int.Parse(text_Ncmd.Text);
                    text_Ncmd.Text = (nc + 1).ToString();
                    dgv1.Rows.Clear();
                    combo_Cl.Enabled = true;
                    MessageBox.Show("commande est enregestrer");
                }

            }
            catch(Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
            finally
            {
                cn.Close();
            }
        }

        private void button1_Click(object sender, EventArgs e)
        {
            foreach(DataGridViewRow r in dgv1.Rows)
            {
                MessageBox.Show(r.Cells[0].Value.ToString());
            }
        }

        private void gunaGradientButton3_Click(object sender, EventArgs e)
        {
            this.Hide();
            Form3 f3 = new Form3();
            f3.ShowDialog();
        }
    }
}
