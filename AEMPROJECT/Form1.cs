using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Configuration;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Diagnostics;
using System.Text.RegularExpressions;
using Newtonsoft.Json;

namespace AEMPROJECT
{
    public partial class Form1 : Form
    {
        #region URL API
        public string AccountLogin = ConfigurationManager.AppSettings["AccountLogin"];
        #endregion
        #region Default
        public Form1()
        {
            InitializeComponent();
            if (Log.InitLog())
            {
                Log.LogEvents("AEM ENERSOL - Project Test", EventLogEntryType.Information);
            }
        }
        private void BtnClear_Click(object sender, EventArgs e)
        {
            txtUsername.Clear();
            txtPassword.Clear();
            txtToken.Clear();
            txtResponseWell.Clear();
            rbDummy.Checked = false;
            rbWell.Checked = false;
            rbDummy.Enabled = false;
            rbWell.Enabled = false;
            btnTest.Enabled = false;
        }
        #endregion
        #region Login
        private void BtnLogin_Click(object sender, EventArgs e)
        {
            if (ValidationLogin(txtUsername.Text.Trim(), txtPassword.Text.Trim()))
            {
                rbDummy.Checked = false;
                rbWell.Checked = false;

                string url = txtURL.Text.Trim();
                string returntoken = GetResponseLogin(txtUsername.Text.Trim(), txtPassword.Text.Trim(), url);
                if (returntoken != string.Empty)
                {
                    rbDummy.Enabled = true;
                    rbWell.Enabled = true;
                    btnTest.Enabled = true;
                }
            }
        }
        private bool ValidationLogin(string username, string password)
        {
            if (username == string.Empty || username.Length < 5)
            {
                MessageBox.Show("Please enter username or username need more than 5 characters", "Warning!", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return false;
            }
            if (password == string.Empty || password.Length < 5)
            {
                MessageBox.Show("Please enter username or username need more than 5 characters", "Warning!", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return false;
            }
            return true;
        }
        private string GetResponseLogin(string username, string password, string url)
        {
            string result = string.Empty;
            txtToken.Clear();
            try
            {
                Model.LoginRequest LoginModel = new Model.LoginRequest { username = username, password = password };

                HttpClient Client = new HttpClient();
                Client.BaseAddress = new Uri(url);
                var response = Client.PostAsJsonAsync(AccountLogin, LoginModel).Result;

                if (response.IsSuccessStatusCode)
                {
                    result = response.Content.ReadAsStringAsync().Result.ToString().Trim();
                    txtToken.Text = result;
                    Log.LogEvents($"[GetResponseLogin] Login account success. Username:{username}, Password:{password}", EventLogEntryType.Information);
                }
                else
                {
                    MessageBox.Show("Username and/or password is incorrect or wrong", "Error!", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    Log.LogEvents($"[GetResponseLogin] Username/Password incorrect. Username:{username}, Password:{password}", EventLogEntryType.Error);
                }
            }
            catch (Exception ex)
            {
                //throw ex;
                Log.LogEvents($"[GetResponseLogin] GetResponseLogin Error: {ex.Message}", EventLogEntryType.Error);
                MessageBox.Show("Username and Password is incorrect or this account have problem", "Technical Problem", MessageBoxButtons.OK, MessageBoxIcon.Error);
                result = string.Empty;
            }
            return result;
        }
        #endregion
        #region Platform        
        private void btnTest_Click(object sender, EventArgs e)
        {
            if (ValidationAPI())
            {
                string api = string.Empty;
                if (rbDummy.Checked)
                    api = rbDummy.Text;
                else if (rbWell.Checked)
                    api = rbWell.Text;

                string returnToken = txtToken.Text;
                string url = txtURL.Text;
                returnToken = Regex.Replace(returnToken, "[\"]", string.Empty, RegexOptions.Compiled);

                GetPlatformWell(returnToken, api, url);
            }
        }
        private bool ValidationAPI()
        {
            if (rbWell.Checked == false && rbDummy.Checked == false)
            {
                MessageBox.Show("Please select API need to be test", "Warning!", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return false;
            }
            return true;
        }
        private void GetPlatformWell(string token, string api, string url)
        {
            txtResponseWell.Clear();
            try
            {
                HttpClient Client = new HttpClient();
                Client.BaseAddress = new Uri(url);
                Client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);
                var response = Client.GetAsync(api).Result;
                if (response.IsSuccessStatusCode)
                {
                    List<Model.PlatformDto> platformDtos = new List<Model.PlatformDto>();
                    string JsonData = response.Content.ReadAsStringAsync().Result;
                    txtResponseWell.Text = JsonData;
                    Log.LogEvents($"[GetPlatformWell] Get data PlatformWell success.", EventLogEntryType.Information);

                    platformDtos = JsonConvert.DeserializeObject<List<Model.PlatformDto>>(JsonData);
                    if (platformDtos.Count != 0)
                    {
                        DBClass DB = new DBClass();
                        Log.LogEvents($"[GetPlatformWell] Start save and/or update DataJson into DB.", EventLogEntryType.Information);
                        foreach (var val in platformDtos)
                        {
                            DB.InsertPlatform(val.id, val.uniqueName, val.latitude, val.longitude, val.createdAt, val.updatedAt);
                            if (val.well.Count != 0)
                            {
                                foreach (var subv in val.well)
                                {
                                    DB.InsertWell(subv.id, subv.platformId, subv.uniqueName, subv.latitude, subv.longitude, subv.createdAt, subv.updatedAt);
                                }
                            }
                        }
                        DB.Disconnect();
                        Log.LogEvents($"[GetPlatformWell] Complete save and/or update DataJson into DB.", EventLogEntryType.Information);
                    }

                    MessageBox.Show("Data already collected and store into database", "Information", MessageBoxButtons.OK, MessageBoxIcon.Information);
                }
                else
                {
                    Log.LogEvents($"[GetPlatformWell] No data PlatformWell.", EventLogEntryType.Warning);
                    MessageBox.Show("No data record that collect", "Warning!", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                }

            }
            catch (Exception ex)
            {
                Log.LogEvents($"[GetPlatformWell] GetPlatformWell Error: {ex.Message}", EventLogEntryType.Error);
                MessageBox.Show("Collect data get some error problem. Please test another URL", "Technical Problem", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }
        #endregion       
    }
}
