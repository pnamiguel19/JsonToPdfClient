using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using JsonToPdfClient.Services;

namespace JsonToPdfClient
{
    public partial class LoginAuth : Form
    {
        public bool Authenticated { get; private set; }

        public LoginAuth()
        {
            InitializeComponent();
            Authenticated = false;
        }

        private async void button1_Click(object sender, EventArgs e)
        {
            btnLogin.Enabled = false;
            try
            {
                await AuthService.LoginAsync(txtLogin.Text.Trim(), txtPassword.Text);

                var mainForm = new Form1();

                mainForm.FormClosed += (s, args) => this.Close();

                mainForm.Show();
                this.Hide();
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "Error de login",
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            finally
            {
                btnLogin.Enabled = true;
            }
        }


    }
}
