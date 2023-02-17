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

namespace CustomerSystem
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
        }
        private int CurrentSelectedCutsomerID = -1;

        private void button1_Click(object sender, EventArgs e)
        {
            SqlConnection sqlCon = new SqlConnection(txt_DBPath.Text);
            try
            {
                if (sqlCon.State == ConnectionState.Closed)
                    sqlCon.Open();

                SqlCommand sqlCmd = new SqlCommand("CustomerAddOrEdit", sqlCon);
                sqlCmd.CommandType = CommandType.StoredProcedure;
                sqlCmd.Parameters.AddWithValue("@mode", "Add");
                sqlCmd.Parameters.AddWithValue("@Id", 0);
                sqlCmd.Parameters.AddWithValue("@Name", txt_Name.Text.Trim());
                sqlCmd.Parameters.AddWithValue("@MobileNumber", txt_MobileNumber.Text.Trim());
                sqlCmd.Parameters.AddWithValue("@Address", txt_Address.Text.Trim());
                sqlCmd.ExecuteNonQuery();
                MessageBox.Show("Saved successfully");
                RefreshDBList();
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "Error Message");
            }
        }

        private void btn_Search_Click(object sender, EventArgs e)
        {
            SqlConnection sqlCon = new SqlConnection(txt_DBPath.Text);
            try
            {
                if (sqlCon.State == ConnectionState.Closed)
                    sqlCon.Open();
                SqlDataAdapter sqlDa = new SqlDataAdapter("SearchCustomer", sqlCon);
                sqlDa.SelectCommand.CommandType = CommandType.StoredProcedure;
                sqlDa.SelectCommand.Parameters.AddWithValue("@CustomerName", txt_SearchPattern.Text.Trim());
                DataTable dtbl = new DataTable();
                sqlDa.Fill(dtbl);
                dgv_DBList.DataSource = dtbl;
                dgv_DBList.Columns[0].Visible = false;
                sqlCon.Close();
                Reset();
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "Error Message");
            }
        }

        private void dgvDBList_DoubleClick(object sender, EventArgs e)
        {
            if (dgv_DBList.CurrentRow.Index != -1)
            {
                CurrentSelectedCutsomerID = Convert.ToInt32(dgv_DBList.CurrentRow.Cells[0].Value.ToString());
                txt_Name.Text = dgv_DBList.CurrentRow.Cells[1].Value.ToString();
                txt_MobileNumber.Text = dgv_DBList.CurrentRow.Cells[2].Value.ToString();
                txt_Address.Text = dgv_DBList.CurrentRow.Cells[3].Value.ToString();
                btn_Update.Enabled = btn_Delete.Enabled = true;

            }
        }

        private void btn_Delete_Click(object sender, EventArgs e)
        {
            var confirmResult = MessageBox.Show("是否刪除該筆資料",
                                     "是否刪除",
                                     MessageBoxButtons.YesNo);
            if (confirmResult != DialogResult.Yes)
            {
                return;
            }
 
            SqlConnection sqlCon = new SqlConnection(txt_DBPath.Text);
            try
            {
                if (sqlCon.State == ConnectionState.Closed)
                    sqlCon.Open();
                SqlCommand sqlCmd = new SqlCommand("CustomerDeletion", sqlCon);
                sqlCmd.CommandType = CommandType.StoredProcedure;
                sqlCmd.Parameters.AddWithValue("Id", CurrentSelectedCutsomerID);
                sqlCmd.ExecuteNonQuery();
                MessageBox.Show("Deleted successfully");

                RefreshDBList();
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "Error Message");
            }
        }

        private void Reset()
        {

            txt_Name.Text = txt_MobileNumber.Text = txt_Address.Text = "";
            btn_Update.Enabled = false;
            CurrentSelectedCutsomerID = -1;
            btn_Delete.Enabled = false;
        }

        private void RefreshDBList()
        {
            try
            {
                Reset();
                SqlConnection sqlCon = new SqlConnection(txt_DBPath.Text);
                if (sqlCon.State == ConnectionState.Closed)
                    sqlCon.Open();
                SqlDataAdapter sqlDa = new SqlDataAdapter("SearchCustomer", sqlCon);
                sqlDa.SelectCommand.CommandType = CommandType.StoredProcedure;
                sqlDa.SelectCommand.Parameters.AddWithValue("@CustomerName", "");
                DataTable dtbl = new DataTable();
                sqlDa.Fill(dtbl);
                dgv_DBList.DataSource = dtbl;
                dgv_DBList.Columns[0].Visible = false;
                sqlCon.Close();
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "Error Message");
            }
        }

        private void btn_Update_Click(object sender, EventArgs e)
        {
            SqlConnection sqlCon = new SqlConnection(txt_DBPath.Text);
            try
            {
                if (sqlCon.State == ConnectionState.Closed)
                    sqlCon.Open();

                SqlCommand sqlCmd = new SqlCommand("CustomerAddOrEdit", sqlCon);
                sqlCmd.CommandType = CommandType.StoredProcedure;
                sqlCmd.Parameters.AddWithValue("@mode", "Edit");
                sqlCmd.Parameters.AddWithValue("@Id", CurrentSelectedCutsomerID);
                sqlCmd.Parameters.AddWithValue("@Name", txt_Name.Text.Trim());
                sqlCmd.Parameters.AddWithValue("@MobileNumber", txt_MobileNumber.Text.Trim());
                sqlCmd.Parameters.AddWithValue("@Address", txt_Address.Text.Trim());
                sqlCmd.ExecuteNonQuery();
                MessageBox.Show("Updated successfully");
                RefreshDBList();
                Reset();
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "Error Message");
            }
        }

        private void btn_DatabastButton_Click(object sender, EventArgs e)
        {
            OpenFileDialog openFileDialog = new OpenFileDialog();
            openFileDialog.Filter = "mdf|*.mdf";
            if (openFileDialog.ShowDialog() == DialogResult.OK)
            {
                txt_DBPath.Text = @"Data Source=(LocalDB)\MSSQLLocalDB;AttachDbFilename=" + openFileDialog.FileName + ";Integrated Security=True;Connect Timeout=30";
                btn_Add.Enabled = true;
                btn_Search.Enabled = true;
            }
        }
    }
}
