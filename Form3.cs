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
    public partial class Form3 : Form
    {
        SqlConnection cn = new SqlConnection(@"Data Source=.;Initial Catalog=Vente;Integrated Security=True");
        SqlCommand cmd = new SqlCommand();
        SqlDataReader dr;
        public Form3()
        {
            InitializeComponent();
        }

        private void Form3_Shown(object sender, EventArgs e)
        {
            text_User.Focus(); //to focus in the username textbox
        }

        private void gunaGradientButton1_Click(object sender, EventArgs e)
        {
            try
            {
                string username = text_User.Text;
                string password = text_Pass.Text;
                if (username != "" || password != "")
                {
                    cn.Open();
                    cmd.CommandText = "select * from conn where username=@a and password=@b";
                    cmd.Connection = cn;
                    cmd.Parameters.Clear();
                    cmd.Parameters.AddWithValue("a", username);
                    cmd.Parameters.AddWithValue("b", password);
                    dr = cmd.ExecuteReader();
                    if (dr.Read())
                    {
                        string role = dr[3].ToString();
                        if(role== "admin")
                        {
                            dr.Close();
                            this.Hide();
                            Form2 f2 = new Form2();
                            f2.ShowDialog();
                        }
                        else
                        {
                            dr.Close();
                            this.Hide();
                            Form1 f1 = new Form1();
                            f1.ShowDialog();
                        }
                    }
                    else
                    {
                        dr.Close();
                        MessageBox.Show("inccorect ");
                    }
                }
                else
                {
                    MessageBox.Show("tu peut remplaire les zone de sasire S.V.P");
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

        private void gunaGradientButton2_Click(object sender, EventArgs e)
        {
            Application.Exit();
        }
    }
}
