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
    public partial class Form2 : Form
    {
        public Form2()
        {
            InitializeComponent();
        }
        SqlConnection cn = new SqlConnection(@"Data Source=.;Initial Catalog=Vente;Integrated Security=True");
        SqlCommand cmd = new SqlCommand();
        SqlDataReader dr;
        private void Form2_Load(object sender, EventArgs e)
        {
            try
            {
                //remplaire combobox des clients
                cn.Open();
                cmd.CommandText = "select CodeCl from Client";
                cmd.Connection = cn;

                dr = cmd.ExecuteReader();
                while (dr.Read())
                {
                    combo_Cl.Items.Add(dr[0].ToString());
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

        private void combo_Cl_SelectedIndexChanged(object sender, EventArgs e)
        {
            try
            {
                cn.Open();
                //afficher nom et ville
                cmd.CommandText = "select Nom,Ville from Client where CodeCl=@a";
                cmd.Parameters.Clear();
                cmd.Parameters.AddWithValue("a", combo_Cl.Text);
                cmd.Connection = cn;
                
                dr = cmd.ExecuteReader();
                if (dr.Read())
                {
                    textNom.Text = dr[0].ToString();
                    textVille.Text = dr[1].ToString();
                }
                dr.Close();
                //remlaire dgv1
                
                cmd.CommandText = "select NumCom, DateCom from Commande where CodeCl=@a";
                cmd.Parameters.Clear();
                cmd.Parameters.AddWithValue("a", combo_Cl.Text);
                cmd.Connection = cn;
                dr = cmd.ExecuteReader();
                bool aUnCmd = false; //pour afficher chiffre d'affaire on a besoins aux moin un commande
                if (dr.HasRows) //using HasRows instant of Read() beacouse we gonna bring data from db to datasource 
                {
                    
                    DataTable dt = new DataTable();
                    dt.Load(dr);
                    dgv1.DataSource = dt;
               
                    aUnCmd = true;
                }
                dr.Close();
                //chiffre d'affaire
                if (aUnCmd)
                {
                    cmd.CommandText = @"select sum(D.QteCommandée*A.PU) from Commande C join  Détail D
                                    on C.NumCom=D.NumCom join Article A on A.CodeArt=D.CodeArt
                                    where CodeCl=@a";
                    cmd.Parameters.Clear();
                    cmd.Parameters.AddWithValue("a", combo_Cl.Text);
                    cmd.Connection = cn;
                    int chifreAff = (int)cmd.ExecuteScalar();
                    text_ChifAff.Text = chifreAff.ToString();
                }
                else
                {
                    MessageBox.Show("aucun commande effectuer par cette client");
                }
                
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

        private void dgv1_CellClick(object sender, DataGridViewCellEventArgs e)
        {
            try
            {
                cn.Open();
                int Ncmd = int.Parse(dgv1.CurrentRow.Cells[0].Value.ToString());
                cmd.CommandText = @"select A.CodeArt,Désignation,PU,QteCommandée,
                 D.QteCommandée*A.PU as Montant
                  from Article A join  Détail D on A.CodeArt=D.CodeArt 
                  where NumCom=@a";
                cmd.Parameters.Clear();
                cmd.Parameters.AddWithValue("a", Ncmd);
                dr = cmd.ExecuteReader();
                if (dr.HasRows)
                {
                    DataTable dt = new DataTable();
                    dt.Load(dr);
                    dgv2.DataSource = dt;
                }
                dr.Close();
                //rempalire les textboxes
                double total = 0;
                foreach (DataGridViewRow r in dgv2.Rows)
                {
                    total = total + double.Parse(r.Cells[4].Value.ToString());
                }
                double tva = total * 0.2;
                double ttc = total + tva;

                text_Total.Text = total.ToString();
                text_TVA.Text = tva.ToString();
                text_ttc.Text = ttc.ToString();
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

        private void gunaGradientButton1_Click(object sender, EventArgs e)
        {
            try
            {
                
                int idx = dgv1.CurrentCell.RowIndex;

                //recalculer chiffre d'affaire
                double mnt = double.Parse(dgv2.Rows[idx].Cells[4].Value.ToString());
                double chifreAff = double.Parse(text_ChifAff.Text) - mnt;
                text_ChifAff.Text = chifreAff.ToString();
                
                //supprimer command
                cn.Open();
                int Ncmd = int.Parse(dgv1.Rows[idx].Cells[0].Value.ToString());
                dgv1.Rows.RemoveAt(idx);
                cmd.CommandText = "delete Commande where NumCom=@a";
                cmd.Connection = cn;
                cmd.Parameters.Clear();
                cmd.Parameters.AddWithValue("a", Ncmd);

                if (cmd.ExecuteNonQuery() > 0)
                {
                    MessageBox.Show("comannde a ete supprimer");
                }

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

        private void gunaGradientButton2_Click(object sender, EventArgs e)
        {
            this.Hide();
            Form3 f3 = new Form3();
            f3.ShowDialog();
        }
    }
}
